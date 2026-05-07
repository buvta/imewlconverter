using System;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;
using Studyzy.IMEWLConverter.Entities;
using Studyzy.IMEWLConverter.IME;

namespace Studyzy.IMEWLConverter.Test;

public class SougouPinyinScelExportTest : IDisposable
{
    private readonly SougouPinyinScel exporter;
    private readonly string tempDir;

    public SougouPinyinScelExportTest()
    {
        exporter = new SougouPinyinScel();
        tempDir = Path.Combine(Path.GetTempPath(), "scel_export_test_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);
    }

    private string GetFullPath(string fileName)
    {
        var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        return Path.Combine(assemblyLocation!, "Test", fileName);
    }

    [Fact]
    public void TestExportBasicScel()
    {
        var wlList = new WordLibraryList
        {
            new WordLibrary { Word = "测试", PinYin = new[] { "ce", "shi" }, Rank = 1 },
            new WordLibrary { Word = "你好", PinYin = new[] { "ni", "hao" }, Rank = 2 },
            new WordLibrary { Word = "世界", PinYin = new[] { "shi", "jie" }, Rank = 3 }
        };

        var outputPath = Path.Combine(tempDir, "basic.scel");
        exporter.ExportToBinary(wlList, outputPath);

        Assert.True(File.Exists(outputPath));
        var data = File.ReadAllBytes(outputPath);

        // 验证 magic number
        Assert.Equal(0x40, data[0]);
        Assert.Equal(0x15, data[1]);
        Assert.Equal(0x00, data[2]);
        Assert.Equal(0x00, data[3]);
        Assert.Equal(0x44, data[4]); // 'D'
        Assert.Equal(0x43, data[5]); // 'C'
        Assert.Equal(0x53, data[6]); // 'S'
        Assert.Equal(0x01, data[7]);

        // 验证词组数
        var groupCount = BitConverter.ToInt32(data, 0x120);
        Assert.Equal(3, groupCount);

        // 验证词条总数
        var wordCount = BitConverter.ToInt32(data, 0x124);
        Assert.Equal(3, wordCount);

        // 验证拼音表条目数
        var pyCount = BitConverter.ToInt32(data, 0x1540);
        Assert.Equal(413, pyCount);
    }

    [Fact]
    public void TestExportWithSamePinyin()
    {
        // 同音词测试
        var wlList = new WordLibraryList
        {
            new WordLibrary { Word = "世界", PinYin = new[] { "shi", "jie" }, Rank = 1 },
            new WordLibrary { Word = "实际", PinYin = new[] { "shi", "ji" }, Rank = 2 },
            new WordLibrary { Word = "石阶", PinYin = new[] { "shi", "jie" }, Rank = 3 }
        };

        var outputPath = Path.Combine(tempDir, "same_pinyin.scel");
        exporter.ExportToBinary(wlList, outputPath);

        var data = File.ReadAllBytes(outputPath);

        // "世界"和"石阶"拼音相同(shi'jie)，应归为一组
        // "实际"拼音不同(shi'ji)，独立一组
        var groupCount = BitConverter.ToInt32(data, 0x120);
        Assert.Equal(2, groupCount); // 2个词组

        var wordCount = BitConverter.ToInt32(data, 0x124);
        Assert.Equal(3, wordCount); // 3个词条
    }

    [Fact]
    public void TestExportMetaInfo()
    {
        var wlList = new WordLibraryList
        {
            new WordLibrary { Word = "深蓝", PinYin = new[] { "shen", "lan" }, Rank = 1 }
        };

        var outputPath = Path.Combine(tempDir, "meta.scel");
        exporter.ExportToBinary(wlList, outputPath);

        var data = File.ReadAllBytes(outputPath);

        // 验证名称
        var nameBytes = new byte[520];
        Array.Copy(data, 0x130, nameBytes, 0, 520);
        var name = Encoding.Unicode.GetString(nameBytes);
        var nameEnd = name.IndexOf('\0');
        name = name[..nameEnd];
        Assert.Equal("深蓝词库转换", name);

        // 验证描述
        var infoBytes = new byte[2048];
        Array.Copy(data, 0x540, infoBytes, 0, 2048);
        var info = Encoding.Unicode.GetString(infoBytes);
        var infoEnd = info.IndexOf('\0');
        info = info[..infoEnd];
        Assert.Equal("由深蓝词库转换工具生成", info);
    }

    [Fact]
    public void TestRoundTrip()
    {
        // 往返测试：导出后再导入，验证数据一致
        var wlList = new WordLibraryList
        {
            new WordLibrary { Word = "深蓝测试", PinYin = new[] { "shen", "lan", "ce", "shi" }, Rank = 1 },
            new WordLibrary { Word = "词库转换", PinYin = new[] { "ci", "ku", "zhuan", "huan" }, Rank = 2 },
            new WordLibrary { Word = "你好世界", PinYin = new[] { "ni", "hao", "shi", "jie" }, Rank = 3 }
        };

        var outputPath = Path.Combine(tempDir, "roundtrip.scel");
        exporter.ExportToBinary(wlList, outputPath);

        // 重新导入
        var importer = new SougouPinyinScel();
        var imported = importer.Import(outputPath);

        Assert.Equal(3, imported.Count);

        // 验证词条和拼音（顺序可能因拼音排序而变化）
        var wordSet = new System.Collections.Generic.HashSet<string>();
        foreach (var wl in imported)
        {
            wordSet.Add(wl.Word + "|" + wl.PinYinString);
        }

        Assert.True(wordSet.Contains("深蓝测试|shen'lan'ce'shi"));
        Assert.True(wordSet.Contains("词库转换|ci'ku'zhuan'huan"));
        Assert.True(wordSet.Contains("你好世界|ni'hao'shi'jie"));
    }

    [Fact]
    public void TestRoundTripWithRealFile()
    {
        // 使用真实 scel 文件进行往返测试
        var testFile = GetFullPath("唐诗300首【官方推荐】.scel");
        if (!File.Exists(testFile))
        {
            return; // 测试文件不存在，跳过
        }

        var importer = new SougouPinyinScel();
        var original = importer.Import(testFile);

        var outputPath = Path.Combine(tempDir, "roundtrip_real.scel");
        exporter.ExportToBinary(original, outputPath);

        // 重新导入
        var reimported = importer.Import(outputPath);

        Assert.Equal(original.Count, reimported.Count);

        // 验证所有词条和拼音都保持一致
        var originalSet = new System.Collections.Generic.HashSet<string>();
        foreach (var wl in original)
            originalSet.Add(wl.Word + "|" + wl.PinYinString);

        var reimportedSet = new System.Collections.Generic.HashSet<string>();
        foreach (var wl in reimported)
            reimportedSet.Add(wl.Word + "|" + wl.PinYinString);

        Assert.Equal(originalSet, reimportedSet);
    }

    [Fact]
    public void TestExportLineThrows()
    {
        Assert.Throws<Exception>(() => exporter.ExportLine(new WordLibrary()));
    }

    [Fact]
    public void TestSkipWordsWithInvalidPinyin()
    {
        var wlList = new WordLibraryList
        {
            new WordLibrary { Word = "有拼音", PinYin = new[] { "you", "pin", "yin" }, Rank = 1 },
            new WordLibrary { Word = "非标拼音", PinYin = new[] { "xxx", "yyy" }, Rank = 3 }
        };

        var outputPath = Path.Combine(tempDir, "skip_invalid_pinyin.scel");
        exporter.ExportToBinary(wlList, outputPath);

        var data = File.ReadAllBytes(outputPath);
        var wordCount = BitConverter.ToInt32(data, 0x124);
        // "非标拼音"的拼音 xxx/yyy 不在标准拼音表中，应被跳过
        Assert.Equal(1, wordCount);
    }
}
