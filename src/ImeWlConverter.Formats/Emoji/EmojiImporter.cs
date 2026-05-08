namespace ImeWlConverter.Formats.Emoji;

using System.Text;
using System.Text.RegularExpressions;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Emoji dictionary importer. Import only, format: "emoji\tword".</summary>
[FormatPlugin("emoji", "Emoji", 999)]
public sealed partial class EmojiImporter : TextFormatImporter
{
    private static readonly Regex EnglishRegex = new("^[a-zA-Z]+$");

    protected override Encoding FileEncoding => Encoding.UTF8;
    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        var parts = line.Split('\t');
        if (parts.Length < 2)
            yield break;

        var word = parts[1];
        var isEnglish = EnglishRegex.IsMatch(word);

        yield return new WordEntry
        {
            Word = word,
            CodeType = isEnglish ? CodeType.English : CodeType.NoCode,
            IsEnglish = isEnglish,
            Code = isEnglish ? WordCode.FromSingle(new[] { word }) : null
        };
    }
}
