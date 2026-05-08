namespace ImeWlConverter.Formats.QQPinyin;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>QQ Pinyin dictionary importer (text format).</summary>
[FormatPlugin("qqpy", "QQ拼音", 50)]
public sealed partial class QQPinyinImporter : TextFormatImporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        line = line.Split(',')[0];
        var sp = line.Split(' ');
        if (sp.Length < 3)
            yield break;

        var py = sp[0];
        var word = sp[1];
        var rank = int.TryParse(sp[2], out var r) ? r : 0;
        var pinyinParts = py.Split(new[] { '\'' }, StringSplitOptions.RemoveEmptyEntries);

        yield return new WordEntry
        {
            Word = word,
            Rank = rank,
            CodeType = CodeType.Pinyin,
            Code = WordCode.FromSingle(pinyinParts)
        };
    }
}
