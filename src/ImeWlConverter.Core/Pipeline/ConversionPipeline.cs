using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Abstractions.Options;
using ImeWlConverter.Abstractions.Results;

namespace ImeWlConverter.Core.Pipeline;

/// <summary>
/// Orchestrates the complete conversion pipeline:
/// Import → Filter → Code Generation → Export.
/// </summary>
public sealed class ConversionPipeline : IConversionPipeline
{
    private readonly IEnumerable<IFormatImporter> _importers;
    private readonly IEnumerable<IFormatExporter> _exporters;
    private readonly IProgress<ProgressInfo>? _progress;

    /// <summary>
    /// Initializes a new instance of <see cref="ConversionPipeline"/>.
    /// </summary>
    public ConversionPipeline(
        IEnumerable<IFormatImporter> importers,
        IEnumerable<IFormatExporter> exporters,
        IProgress<ProgressInfo>? progress = null)
    {
        _importers = importers;
        _exporters = exporters;
        _progress = progress;
    }

    /// <inheritdoc/>
    public async Task<Result<ConversionResult>> ExecuteAsync(
        ConversionRequest request,
        CancellationToken ct = default)
    {
        // 1. Find importer/exporter by format ID
        var importer = _importers.FirstOrDefault(i => i.Metadata.Id == request.InputFormatId);
        if (importer is null)
            return Result<ConversionResult>.Failure($"Unknown input format: {request.InputFormatId}");

        var exporter = _exporters.FirstOrDefault(e => e.Metadata.Id == request.OutputFormatId);
        if (exporter is null)
            return Result<ConversionResult>.Failure($"Unknown output format: {request.OutputFormatId}");

        // 2. Import all input files
        var allEntries = new List<WordEntry>();
        foreach (var inputPath in request.InputPaths)
        {
            ct.ThrowIfCancellationRequested();
            _progress?.Report(new ProgressInfo(0, 0, $"Importing {Path.GetFileName(inputPath)}..."));

            using var stream = File.OpenRead(inputPath);
            var importResult = await importer.ImportAsync(stream, request.Options.Import, ct);
            allEntries.AddRange(importResult.Entries);
        }

        var importedCount = allEntries.Count;

        // 3. Filter pipeline (to be wired up; pass-through for now)
        var exportedCount = allEntries.Count;
        var filteredCount = importedCount - exportedCount;

        // 4. Export
        _progress?.Report(new ProgressInfo(0, 0, "Exporting..."));
        using var outputStream = File.Create(request.OutputPath);
        await exporter.ExportAsync(allEntries, outputStream, request.Options.Export, ct);

        return Result<ConversionResult>.Success(new ConversionResult
        {
            ImportedCount = importedCount,
            ExportedCount = exportedCount,
            FilteredCount = filteredCount
        });
    }
}
