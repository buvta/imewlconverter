namespace ImeWlConverter.Formats.Chaoyin;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Chaoyin (超音速录) dictionary importer. Format: "code = rank,word".</summary>
[FormatPlugin("cysl", "超音速录", 190)]
public sealed partial class ChaoyinImporter : TextFormatImporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        // Format: code = rank,word
        var eqIndex = line.IndexOf(" = ");
        if (eqIndex < 0)
            yield break;

        var code = line[..eqIndex];
        var rest = line[(eqIndex + 3)..];

        var commaIndex = rest.IndexOf(',');
        if (commaIndex < 0)
            yield break;

        var rank = int.TryParse(rest[..commaIndex], out var r) ? r : 1;
        var word = rest[(commaIndex + 1)..];

        if (string.IsNullOrEmpty(word))
            yield break;

        yield return new WordEntry
        {
            Word = word,
            Rank = rank,
            CodeType = CodeType.Chaoyin,
            Code = WordCode.FromSingle(new[] { code })
        };
    }
}
