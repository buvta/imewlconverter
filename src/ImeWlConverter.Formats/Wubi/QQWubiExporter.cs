namespace ImeWlConverter.Formats.Wubi;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Abstractions.Options;
using ImeWlConverter.Abstractions.Results;

/// <summary>QQ Wubi dictionary exporter. Groups words by code: "code word1 word2".</summary>
[FormatPlugin("qqwb", "QQ五笔", 70)]
public sealed partial class QQWubiExporter : IFormatExporter
{

    public Task<ExportResult> ExportAsync(
        IReadOnlyList<WordEntry> entries, Stream output,
        ExportOptions? options = null, CancellationToken ct = default)
    {
        using var writer = new StreamWriter(output, Encoding.Unicode, leaveOpen: true);
        var count = 0;

        var dict = new Dictionary<string, List<string>>();
        foreach (var entry in entries)
        {
            ct.ThrowIfCancellationRequested();
            var code = entry.Code?.GetPrimaryCode("") ?? "";
            if (string.IsNullOrEmpty(code))
                continue;

            if (!dict.TryGetValue(code, out var list))
            {
                list = new List<string>();
                dict[code] = list;
            }
            list.Add(entry.Word);
            count++;
        }

        foreach (var kvp in dict)
        {
            writer.Write(kvp.Key);
            foreach (var word in kvp.Value)
            {
                writer.Write(' ');
                writer.Write(word);
            }
            writer.Write("\r\n");
        }

        writer.Flush();
        return Task.FromResult(new ExportResult { EntryCount = count });
    }
}
