using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.SougouScel;
using Xunit;

namespace Studyzy.IMEWLConverter.Test;

public class SougouPinyinScelExportTest : BaseTest, IDisposable
{
    private readonly SougouScelExporter _exporter;
    private readonly string _tempDir;

    public SougouPinyinScelExportTest()
    {
        _exporter = new SougouScelExporter();
        _tempDir = Path.Combine(Path.GetTempPath(), "scel_export_test_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    protected override string StringData => throw new NotImplementedException();

    private byte[] ExportToBytes(IReadOnlyList<WordEntry> entries)
    {
        using var stream = new MemoryStream();
        _exporter.ExportAsync(entries, stream).GetAwaiter().GetResult();
        return stream.ToArray();
    }

    [Fact]
    public void TestExportBasicScel()
    {
        var entries = new List<WordEntry>
        {
            new() { Word = "测试", Code = WordCode.FromSingle(new[] { "ce", "shi" }), Rank = 1, CodeType = CodeType.Pinyin },
            new() { Word = "你好", Code = WordCode.FromSingle(new[] { "ni", "hao" }), Rank = 2, CodeType = CodeType.Pinyin },
            new() { Word = "世界", Code = WordCode.FromSingle(new[] { "shi", "jie" }), Rank = 3, CodeType = CodeType.Pinyin }
        };

        var data = ExportToBytes(entries);

        // 验证 magic number
        Assert.Equal(0x40, data[0]);
        Assert.Equal(0x15, data[1]);
        Assert.Equal(0x00, data[2]);
        Assert.Equal(0x00, data[3]);
        Assert.Equal(0x44, data[4]);
        Assert.Equal(0x43, data[5]);
        Assert.Equal(0x53, data[6]);
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
        var entries = new List<WordEntry>
        {
            new() { Word = "世界", Code = WordCode.FromSingle(new[] { "shi", "jie" }), Rank = 1, CodeType = CodeType.Pinyin },
            new() { Word = "实际", Code = WordCode.FromSingle(new[] { "shi", "ji" }), Rank = 2, CodeType = CodeType.Pinyin },
            new() { Word = "石阶", Code = WordCode.FromSingle(new[] { "shi", "jie" }), Rank = 3, CodeType = CodeType.Pinyin }
        };

        var data = ExportToBytes(entries);

        // "世界"和"石阶"拼音相同(shi'jie)应归为一组，"实际"独立一组
        var groupCount = BitConverter.ToInt32(data, 0x120);
        Assert.Equal(2, groupCount);

        var wordCount = BitConverter.ToInt32(data, 0x124);
        Assert.Equal(3, wordCount);
    }

    [Fact]
    public void TestExportMetaInfo()
    {
        var entries = new List<WordEntry>
        {
            new() { Word = "深蓝", Code = WordCode.FromSingle(new[] { "shen", "lan" }), Rank = 1, CodeType = CodeType.Pinyin }
        };

        var data = ExportToBytes(entries);

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
        var entries = new List<WordEntry>
        {
            new() { Word = "深蓝测试", Code = WordCode.FromSingle(new[] { "shen", "lan", "ce", "shi" }), Rank = 1, CodeType = CodeType.Pinyin },
            new() { Word = "词库转换", Code = WordCode.FromSingle(new[] { "ci", "ku", "zhuan", "huan" }), Rank = 2, CodeType = CodeType.Pinyin },
            new() { Word = "你好世界", Code = WordCode.FromSingle(new[] { "ni", "hao", "shi", "jie" }), Rank = 3, CodeType = CodeType.Pinyin }
        };

        // 导出到临时文件（Importer 需要可寻址流）
        var outputPath = Path.Combine(_tempDir, "roundtrip.scel");
        using (var fs = File.Create(outputPath))
        {
            _exporter.ExportAsync(entries, fs).GetAwaiter().GetResult();
        }

        // 重新导入
        var scelImporter = new SougouScelImporter();
        using var readStream = File.OpenRead(outputPath);
        var imported = scelImporter.ImportAsync(readStream).GetAwaiter().GetResult();

        Assert.Equal(3, imported.Entries.Count);

        var wordSet = new HashSet<string>();
        foreach (var wl in imported.Entries)
            wordSet.Add(wl.Word + "|" + wl.Code?.GetPrimaryCode("'"));

        Assert.Contains("深蓝测试|shen'lan'ce'shi", wordSet);
        Assert.Contains("词库转换|ci'ku'zhuan'huan", wordSet);
        Assert.Contains("你好世界|ni'hao'shi'jie", wordSet);
    }

    [Fact]
    public void TestRoundTripWithRealFile()
    {
        var testFile = GetFullPath("唐诗300首【官方推荐】.scel");
        if (!File.Exists(testFile))
            return;

        // 导入原始文件
        var scelImporter = new SougouScelImporter();
        using var originalStream = File.OpenRead(testFile);
        var originalResult = scelImporter.ImportAsync(originalStream).GetAwaiter().GetResult();
        var original = originalResult.Entries;

        // 导出
        var outputPath = Path.Combine(_tempDir, "roundtrip_real.scel");
        using (var fs = File.Create(outputPath))
        {
            _exporter.ExportAsync(original, fs).GetAwaiter().GetResult();
        }

        // 重新导入
        using var reimportStream = File.OpenRead(outputPath);
        var reimportResult = scelImporter.ImportAsync(reimportStream).GetAwaiter().GetResult();
        var reimported = reimportResult.Entries;

        Assert.Equal(original.Count, reimported.Count);

        var originalSet = new HashSet<string>();
        foreach (var wl in original)
            originalSet.Add(wl.Word + "|" + wl.Code?.GetPrimaryCode("'"));

        var reimportedSet = new HashSet<string>();
        foreach (var wl in reimported)
            reimportedSet.Add(wl.Word + "|" + wl.Code?.GetPrimaryCode("'"));

        Assert.Equal(originalSet, reimportedSet);
    }

    [Fact]
    public void TestSkipWordsWithInvalidPinyin()
    {
        var entries = new List<WordEntry>
        {
            new() { Word = "有拼音", Code = WordCode.FromSingle(new[] { "you", "pin", "yin" }), Rank = 1, CodeType = CodeType.Pinyin },
            new() { Word = "非标拼音", Code = WordCode.FromSingle(new[] { "xxx", "yyy" }), Rank = 3, CodeType = CodeType.Pinyin }
        };

        var data = ExportToBytes(entries);
        var wordCount = BitConverter.ToInt32(data, 0x124);
        Assert.Equal(1, wordCount);
    }
}
