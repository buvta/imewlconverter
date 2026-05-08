namespace ImeWlConverter.Formats.Rime;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Rime dictionary exporter (text format). Format: word\tcode\trank</summary>
[FormatPlugin("rime", "Rime", 150)]
public sealed partial class RimeExporter : TextFormatExporter
{
    protected override Encoding FileEncoding => new UTF8Encoding(false);

    protected override string LineEnding => "\n";
    protected override string? FormatEntry(WordEntry entry)
    {
        var code = entry.Code?.GetPrimaryCode(" ") ?? "";
        if (string.IsNullOrEmpty(code))
            return null;
        return $"{entry.Word}\t{code}\t{entry.Rank}";
    }
}
