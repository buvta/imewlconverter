namespace ImeWlConverter.Formats.QQShouji;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>QQ Mobile dictionary importer. Format: pinyin word number Z, pinyin number</summary>
[FormatPlugin("qqsj", "QQ手机", 1030)]
public sealed partial class QQShoujiImporter : TextFormatImporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        if (!line.Contains("Z,"))
            yield break;

        var parts = line.Split(' ');
        if (parts.Length < 2)
            yield break;

        var py = parts[0];
        var word = parts[1];
        var pinyinParts = py.Split(new[] { '\'' }, StringSplitOptions.RemoveEmptyEntries);

        yield return new WordEntry
        {
            Word = word,
            Rank = 1,
            CodeType = CodeType.Pinyin,
            Code = WordCode.FromSingle(pinyinParts)
        };
    }
}
