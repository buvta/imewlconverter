namespace ImeWlConverter.Formats.Jidian;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>JidianZhengma (极点郑码) dictionary importer. Same format as Jidian but uses Zhengma code type.</summary>
[FormatPlugin("jdzm", "极点郑码", 190)]
public sealed partial class JidianZhengmaImporter : TextFormatImporter
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
                CodeType = CodeType.Zhengma,
                Code = WordCode.FromSingle(new[] { code })
            };
        }
    }
}
