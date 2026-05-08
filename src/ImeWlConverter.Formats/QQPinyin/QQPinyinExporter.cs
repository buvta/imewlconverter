namespace ImeWlConverter.Formats.QQPinyin;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>QQ Pinyin dictionary exporter (text format).</summary>
[FormatPlugin("qqpy", "QQ拼音", 50)]
public sealed partial class QQPinyinExporter : TextFormatExporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    protected override string? FormatEntry(WordEntry entry)
    {
        var pinyin = entry.Code?.GetPrimaryCode("'") ?? "";
        if (string.IsNullOrEmpty(pinyin))
            return null;
        return $"{pinyin} {entry.Word} {entry.Rank}";
    }
}
