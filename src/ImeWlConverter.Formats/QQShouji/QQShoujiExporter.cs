namespace ImeWlConverter.Formats.QQShouji;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Abstractions.Options;
using ImeWlConverter.Abstractions.Results;
using ImeWlConverter.Formats.Shared;

/// <summary>QQ Mobile dictionary exporter. Format: pinyin word number Z, pinyin number</summary>
[FormatPlugin("qqsj", "QQ手机", 1030)]
public sealed partial class QQShoujiExporter : IFormatExporter
{

    public Task<ExportResult> ExportAsync(
        IReadOnlyList<WordEntry> entries, Stream output,
        ExportOptions? options = null, CancellationToken ct = default)
    {
        using var writer = new StreamWriter(output, Encoding.Unicode, leaveOpen: true);
        var count = 0;
        var total = entries.Count;

        for (var i = 0; i < total; i++)
        {
            ct.ThrowIfCancellationRequested();
            var entry = entries[i];
            var pinyin = entry.Code?.GetPrimaryCode("'") ?? "";
            if (string.IsNullOrEmpty(pinyin))
                continue;

            var number = (int)Math.Ceiling((total - i) * 100.0 / total);
            writer.Write($"{pinyin} {entry.Word} {number} Z, {pinyin} {number}\r\n");
            count++;
        }

        writer.Flush();
        return Task.FromResult(new ExportResult
        {
            EntryCount = count,
            ErrorCount = 0
        });
    }
}
