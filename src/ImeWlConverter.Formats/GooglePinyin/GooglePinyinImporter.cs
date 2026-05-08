namespace ImeWlConverter.Formats.GooglePinyin;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Google Pinyin dictionary importer (text format).</summary>
[FormatPlugin("ggpy", "谷歌拼音", 110)]
public sealed partial class GooglePinyinImporter : TextFormatImporter
{
    static GooglePinyinImporter()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    protected override Encoding FileEncoding => Encoding.GetEncoding("GBK");
    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        var parts = line.Split('\t');
        if (parts.Length < 3)
            yield break;

        var word = parts[0];
        var rank = int.TryParse(parts[1], out var r) ? r : 0;
        var pinyinParts = parts[2].Split(' ');

        yield return new WordEntry
        {
            Word = word,
            Rank = rank,
            CodeType = CodeType.Pinyin,
            Code = WordCode.FromSingle(pinyinParts)
        };
    }
}
