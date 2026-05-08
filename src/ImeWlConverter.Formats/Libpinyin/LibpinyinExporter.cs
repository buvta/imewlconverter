namespace ImeWlConverter.Formats.Libpinyin;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Libpinyin dictionary exporter (text format). Format: word pinyin</summary>
[FormatPlugin("libpy", "Libpinyin", 175)]
public sealed partial class LibpinyinExporter : TextFormatExporter
{
    protected override Encoding FileEncoding => new UTF8Encoding(false);

    protected override string LineEnding => "\n";
    protected override string? FormatEntry(WordEntry entry)
    {
        var pinyin = entry.Code?.GetPrimaryCode("'") ?? "";
        if (string.IsNullOrEmpty(pinyin))
            return null;
        return $"{entry.Word} {pinyin}";
    }
}
