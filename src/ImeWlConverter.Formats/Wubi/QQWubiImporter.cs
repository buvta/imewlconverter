namespace ImeWlConverter.Formats.Wubi;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>QQ Wubi dictionary importer. Same Jidian format: "code word1 word2 word3".</summary>
[FormatPlugin("qqwb", "QQ五笔", 70)]
public sealed partial class QQWubiImporter : TextFormatImporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        var parts = line.Split(' ');
        if (parts.Length < 2)
            yield break;

        var code = parts[0];
        for (var i = 1; i < parts.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(parts[i]))
                continue;

            yield return new WordEntry
            {
                Word = parts[i],
                CodeType = CodeType.Wubi86,
                Code = WordCode.FromSingle(new[] { code })
            };
        }
    }
}
