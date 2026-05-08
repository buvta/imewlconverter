namespace ImeWlConverter.Formats.iFlyIME;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>iFlyIME (讯飞输入法) dictionary importer. Format: "word type" or "word".</summary>
[FormatPlugin("ifly", "讯飞输入法", 1050)]
public sealed partial class iFlyIMEImporter : TextFormatImporter
{
    protected override Encoding FileEncoding => Encoding.UTF8;
    protected override bool IsContentLine(string line)
        => !string.IsNullOrWhiteSpace(line) && !line.StartsWith('#');

    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        var word = line.Split(' ')[0];
        if (string.IsNullOrEmpty(word))
            yield break;

        yield return new WordEntry
        {
            Word = word,
            CodeType = CodeType.NoCode
        };
    }
}
