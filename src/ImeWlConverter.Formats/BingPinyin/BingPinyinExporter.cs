namespace ImeWlConverter.Formats.BingPinyin;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Bing Pinyin dictionary exporter (text format). Format: word py1 py2 ...</summary>
[FormatPlugin("bing", "必应拼音", 135)]
public sealed partial class BingPinyinExporter : TextFormatExporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    protected override string? FormatEntry(WordEntry entry)
    {
        var pinyin = entry.Code?.GetPrimaryCode(" ") ?? "";
        if (string.IsNullOrEmpty(pinyin))
            return null;
        return $"{entry.Word} {pinyin}";
    }
}
