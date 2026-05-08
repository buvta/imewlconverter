namespace ImeWlConverter.Formats.BaiduShoujiEng;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Baidu Mobile English dictionary importer. Format: word\trank</summary>
[FormatPlugin("bdsje", "百度手机英文", 1010)]
public sealed partial class BaiduShoujiEngImporter : TextFormatImporter
{
    protected override Encoding FileEncoding => Encoding.ASCII;
    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        var parts = line.Split('\t');
        if (parts.Length < 2)
            yield break;

        var word = parts[0];
        var rank = int.TryParse(parts[1], out var r) ? r : 0;

        yield return new WordEntry
        {
            Word = word,
            Rank = rank,
            CodeType = CodeType.English,
            IsEnglish = true
        };
    }
}
