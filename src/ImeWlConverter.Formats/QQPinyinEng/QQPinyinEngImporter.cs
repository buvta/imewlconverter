namespace ImeWlConverter.Formats.QQPinyinEng;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>QQ Pinyin English dictionary importer. Format: word,rank</summary>
[FormatPlugin("qqpye", "QQ拼音英文", 80)]
public sealed partial class QQPinyinEngImporter : TextFormatImporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        var parts = line.Split(',');
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
