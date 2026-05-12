using System.Collections.Generic;
using System.IO;
using System.Text;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.SelfDefining;
using Xunit;

namespace Studyzy.IMEWLConverter.Test;

public class SelfDefiningTest
{
    /// <summary>
    /// Import "word code rank" format with space separator, pinyin comma-separated.
    /// Old test: Sort={2,1,3}, SplitString=" ", CodeSplitString=","
    /// </summary>
    [Fact]
    public void TestPinyinString2WL()
    {
        var importer = new SelfDefiningImporter
        {
            OrderSpec = "213",
            FieldSeparator = ' ',
            PinyinSeparator = ',',
            ShowPinyin = true,
            ShowWord = true,
            ShowRank = true
        };

        var text = "深蓝 shen,lan 1";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var result = importer.ImportAsync(stream).GetAwaiter().GetResult();
        var wl = result.Entries[0];

        Assert.Equal("深蓝", wl.Word);
        Assert.Equal("shen", wl.Code!.Segments[0][0]);
        Assert.Equal("lan", wl.Code.Segments[1][0]);
        Assert.Equal(1, wl.Rank);
    }

    /// <summary>
    /// Export word entry to "word|code|rank" format.
    /// Old test: Sort={2,1,3}, SplitString="|", CodeSplitString=","
    /// </summary>
    [Fact]
    public void TestWordLibrary2String()
    {
        var exporter = new SelfDefiningExporter
        {
            OrderSpec = "213",
            FieldSeparator = '|',
            PinyinSeparator = ',',
            ShowPinyin = true,
            ShowWord = true,
            ShowRank = true
        };

        var wl = new WordEntry
        {
            Word = "深蓝",
            Rank = 123,
            CodeType = CodeType.Pinyin,
            Code = WordCode.FromSingle(new[] { "shen", "lan" })
        };

        using var stream = new MemoryStream();
        exporter.ExportAsync(new[] { wl }, stream).GetAwaiter().GetResult();
        stream.Position = 0;
        var output = new StreamReader(stream).ReadToEnd().Trim();

        Assert.Equal("深蓝|shen,lan|123", output);
    }

    /// <summary>
    /// Export with tilde pinyin separator.
    /// Old test: CodeSplitString="~", SplitString="|", Sort={2,1,3}
    /// </summary>
    [Fact]
    public void TestGeneratePinyinThen2String()
    {
        var exporter = new SelfDefiningExporter
        {
            OrderSpec = "213",
            FieldSeparator = '|',
            PinyinSeparator = '~',
            ShowPinyin = true,
            ShowWord = true,
            ShowRank = true
        };

        var wl = new WordEntry
        {
            Word = "深蓝",
            Rank = 123,
            CodeType = CodeType.Pinyin,
            Code = WordCode.FromSingle(new[] { "shen", "lan" })
        };

        using var stream = new MemoryStream();
        exporter.ExportAsync(new[] { wl }, stream).GetAwaiter().GetResult();
        stream.Position = 0;
        var output = new StreamReader(stream).ReadToEnd().Trim();

        Assert.Equal("深蓝|shen~lan|123", output);
    }

    /// <summary>
    /// Export with different format: Sort={3,2,1}, word first, underscore pinyin sep.
    /// Old test: Sort={3,2,1}, SplitString="$", CodeSplitString="_", no rank
    /// </summary>
    [Fact]
    public void TestExportPinyinDifferentFormatWL()
    {
        var exporter = new SelfDefiningExporter
        {
            OrderSpec = "321",
            FieldSeparator = '$',
            PinyinSeparator = '_',
            ShowPinyin = true,
            ShowWord = true,
            ShowRank = false
        };

        var wl = new WordEntry
        {
            Word = "深蓝测试",
            Rank = 10,
            CodeType = CodeType.Pinyin,
            Code = WordCode.FromSingle(new[] { "shen", "lan", "ce", "shi" })
        };

        using var stream = new MemoryStream();
        exporter.ExportAsync(new[] { wl }, stream).GetAwaiter().GetResult();
        stream.Position = 0;
        var output = new StreamReader(stream).ReadToEnd().Trim();

        Assert.Equal("深蓝测试$shen_lan_ce_shi", output);
    }

    /// <summary>
    /// Export with pinyin code, verifying basic export works for a single entry.
    /// Replaces old external code table test (CodeType.UserDefine).
    /// </summary>
    [Fact]
    public void TestExportExtCodeWL()
    {
        var exporter = new SelfDefiningExporter
        {
            OrderSpec = "12",
            FieldSeparator = ' ',
            PinyinSeparator = '\'',
            ShowPinyin = true,
            ShowWord = true,
            ShowRank = false
        };

        var wl = new WordEntry
        {
            Word = "深蓝测试",
            Rank = 10,
            CodeType = CodeType.Pinyin,
            Code = WordCode.FromSingle(new[] { "shen", "lan", "ce", "shi" })
        };

        using var stream = new MemoryStream();
        exporter.ExportAsync(new[] { wl }, stream).GetAwaiter().GetResult();
        stream.Position = 0;
        var output = new StreamReader(stream).ReadToEnd().Trim();

        Assert.Equal("shen'lan'ce'shi 深蓝测试", output);
    }

    /// <summary>
    /// Export multiple entries with incrementally longer words.
    /// Replaces old external code table batch test.
    /// </summary>
    [Fact]
    public void TestExportExtCodeLots()
    {
        var exporter = new SelfDefiningExporter
        {
            OrderSpec = "21",
            FieldSeparator = ' ',
            PinyinSeparator = '\'',
            ShowPinyin = true,
            ShowWord = true,
            ShowRank = false
        };

        var words = "深蓝词库转换测试代码";
        var entries = new List<WordEntry>();
        var accumulated = "";
        foreach (var c in words)
        {
            accumulated += c;
            entries.Add(new WordEntry
            {
                Word = accumulated,
                Rank = 10,
                CodeType = CodeType.Pinyin,
                Code = WordCode.FromSingle(new[] { "py" })
            });
        }

        using var stream = new MemoryStream();
        exporter.ExportAsync(entries, stream).GetAwaiter().GetResult();
        stream.Position = 0;
        var output = new StreamReader(stream).ReadToEnd();

        Assert.Contains("深 py", output);
        Assert.Contains("深蓝词库转换测试代码 py", output);
        var lines = output.Trim().Split('\n');
        Assert.Equal(words.Length, lines.Length);
    }

    /// <summary>
    /// Import a simple "word code rank" line.
    /// Old test: Sort={2,1,3}, SplitString=" ", CodeSplitString=","
    /// </summary>
    [Fact]
    public void TestImportTxt()
    {
        var importer = new SelfDefiningImporter
        {
            OrderSpec = "213",
            FieldSeparator = ' ',
            PinyinSeparator = ',',
            ShowPinyin = true,
            ShowWord = true,
            ShowRank = true
        };

        var text = "深藍 shen,lan 12345";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var result = importer.ImportAsync(stream).GetAwaiter().GetResult();
        var wl = result.Entries[0];

        Assert.Equal("深藍", wl.Word);
        Assert.Equal("shen", wl.Code!.Segments[0][0]);
        Assert.Equal("lan", wl.Code.Segments[1][0]);
        Assert.Equal(12345, wl.Rank);
    }
}
