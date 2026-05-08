namespace ImeWlConverter.Formats.BaiduPinyin;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Baidu Pinyin dictionary importer (text format). Supports both Chinese and English entries.</summary>
[FormatPlugin("bdpy", "百度拼音", 90)]
public sealed partial class BaiduPinyinImporter : TextFormatImporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        var array = line.Split('\t');
        if (array.Length < 2)
            yield break;

        var word = array[0];

        if (array.Length == 2)
        {
            // English entry: word\trank
            var rank = int.TryParse(array[1], out var r) ? r : 0;
            yield return new WordEntry
            {
                Word = word,
                Rank = rank,
                CodeType = CodeType.Pinyin,
                IsEnglish = true
            };
        }
        else
        {
            // Chinese entry: word\tpinyin\trank
            var py = array[1];
            var rank = int.TryParse(array[2], out var r) ? r : 0;
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
}
