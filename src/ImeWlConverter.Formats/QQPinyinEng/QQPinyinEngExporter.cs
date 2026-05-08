namespace ImeWlConverter.Formats.QQPinyinEng;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>QQ Pinyin English dictionary exporter. Format: word,rank</summary>
[FormatPlugin("qqpye", "QQ拼音英文", 80)]
public sealed partial class QQPinyinEngExporter : TextFormatExporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    protected override string? FormatEntry(WordEntry entry)
    {
        return $"{entry.Word},{entry.Rank}";
    }
}
