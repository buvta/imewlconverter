namespace ImeWlConverter.Formats.Chaoyin;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Chaoyin (超音速录) dictionary exporter. Export only, format: "code = rank,word".</summary>
[FormatPlugin("cysl", "超音速录", 190)]
public sealed partial class ChaoyinExporter : TextFormatExporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    protected override string? FormatEntry(WordEntry entry)
    {
        var code = entry.Code?.GetPrimaryCode("") ?? "";
        if (string.IsNullOrEmpty(code))
            return null;

        return $"{code} = {entry.Rank},{entry.Word}";
    }
}
