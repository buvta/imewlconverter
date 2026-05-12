using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Win10MsSelfStudy;
using Xunit;

namespace ImeWlConverterCoreTest;

public class Win10MsPinyinSelfStudyTest
{
    [Fact]
    public void TestExport1()
    {
        var entries = new List<WordEntry>
        {
            new()
            {
                Word = "曾毅曾诚",
                CodeType = CodeType.Pinyin,
                Code = WordCode.FromSingle(new[] { "zeng", "yi", "zeng", "cheng" })
            }
        };

        var exporter = new Win10MsPinyinSelfStudyExporter();
        using var stream = new MemoryStream();
        var result = exporter.ExportAsync(entries, stream).GetAwaiter().GetResult();

        Assert.Equal(1, result.EntryCount);
        Assert.True(stream.Length > 0);

        // Verify header magic
        stream.Position = 0;
        var header = new byte[4];
        stream.Read(header, 0, 4);
        Assert.Equal(0x55, header[0]);
        Assert.Equal(0xAA, header[1]);
        Assert.Equal(0x88, header[2]);
        Assert.Equal(0x81, header[3]);

        // Verify word count
        stream.Position = 12;
        var countBytes = new byte[8];
        stream.Read(countBytes, 0, 8);
        var count = BitConverter.ToInt64(countBytes, 0);
        Assert.Equal(1, count);

        // Verify word at entry 0, offset 0x2400 + 12
        stream.Position = 0x2400 + 12;
        var wordBytes = new byte[8]; // 4 chars * 2 bytes
        stream.Read(wordBytes, 0, 8);
        var word = Encoding.Unicode.GetString(wordBytes);
        Assert.Equal("曾毅曾诚", word);

        // Verify file is padded to 1KB boundary
        Assert.Equal(0, stream.Length % 1024);
    }

    [Fact]
    public void TestExportRoundTrip()
    {
        var entries = new List<WordEntry>
        {
            new()
            {
                Word = "深蓝词库",
                CodeType = CodeType.Pinyin,
                Code = WordCode.FromSingle(new[] { "shen", "lan", "ci", "ku" })
            }
        };

        var exporter = new Win10MsPinyinSelfStudyExporter();
        using var stream = new MemoryStream();
        exporter.ExportAsync(entries, stream).GetAwaiter().GetResult();

        // Import back
        stream.Position = 0;
        var importer = new Win10MsPinyinSelfStudyImporter();
        var importResult = importer.ImportAsync(stream).GetAwaiter().GetResult();

        Assert.Single(importResult.Entries);
        Assert.Equal("深蓝词库", importResult.Entries[0].Word);
    }
}
