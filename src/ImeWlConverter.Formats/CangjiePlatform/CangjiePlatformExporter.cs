namespace ImeWlConverter.Formats.CangjiePlatform;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Abstractions.Options;
using ImeWlConverter.Abstractions.Results;

/// <summary>CangjiePlatform dictionary exporter (space-separated code+word, one code per line).</summary>
[FormatPlugin("cjpt", "仓颉平台", 230)]
public sealed partial class CangjiePlatformExporter : IFormatExporter
{
    static CangjiePlatformExporter()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
    public Task<ExportResult> ExportAsync(
        IReadOnlyList<WordEntry> entries, Stream output,
        ExportOptions? options = null, CancellationToken ct = default)
    {
        using var writer = new StreamWriter(output, Encoding.GetEncoding("GBK"), leaveOpen: true);
        var count = 0;
        var errorCount = 0;

        foreach (var entry in entries)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var codes = entry.Code?.Segments;
                if (codes == null || codes.Count == 0)
                    continue;

                // Each code variation gets its own line
                for (var i = 0; i < codes[0].Count; i++)
                {
                    writer.Write(codes[0][i]);
                    writer.Write(' ');
                    writer.Write(entry.Word);
                    writer.Write("\r\n");
                    count++;
                }
            }
            catch
            {
                errorCount++;
            }
        }

        writer.Flush();
        return Task.FromResult(new ExportResult
        {
            EntryCount = count,
            ErrorCount = errorCount
        });
    }
}
