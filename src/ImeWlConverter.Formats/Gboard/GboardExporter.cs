namespace ImeWlConverter.Formats.Gboard;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Gboard dictionary exporter (tab-separated text format).</summary>
[FormatPlugin("gboard", "Gboard", 111)]
public sealed partial class GboardExporter : TextFormatExporter
{
    protected override Encoding FileEncoding => new UTF8Encoding(false);
    protected override string LineEnding => "\n";
    protected override string? GetHeader() => "# Gboard Dictionary version:1";

    protected override string? FormatEntry(WordEntry entry)
    {
        var pinyin = entry.Code?.GetPrimaryCode("") ?? "";
        if (string.IsNullOrWhiteSpace(pinyin) || string.IsNullOrWhiteSpace(entry.Word))
            return null;

        return $"{pinyin}\t{entry.Word}\tzh-CN";
    }
}
