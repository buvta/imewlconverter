using System.Text;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Abstractions.Options;
using ImeWlConverter.Abstractions.Results;
using Studyzy.IMEWLConverter;
using Studyzy.IMEWLConverter.Entities;
using OldCodeType = Studyzy.IMEWLConverter.Entities.CodeType;
using NewCodeType = ImeWlConverter.Abstractions.Enums.CodeType;

namespace ImeWlConverter.Core.Adapters;

/// <summary>
/// Adapts legacy <see cref="IWordLibraryExport"/> to the new <see cref="IFormatExporter"/> interface.
/// </summary>
public sealed class LegacyExporterAdapter : IFormatExporter
{
    private readonly IWordLibraryExport _legacyExporter;

    /// <summary>
    /// Initializes a new instance of <see cref="LegacyExporterAdapter"/>.
    /// </summary>
    /// <param name="legacyExporter">The legacy exporter to wrap.</param>
    /// <param name="metadata">Format metadata describing this exporter.</param>
    public LegacyExporterAdapter(IWordLibraryExport legacyExporter, FormatMetadata metadata)
    {
        _legacyExporter = legacyExporter;
        Metadata = metadata;
    }

    /// <inheritdoc/>
    public FormatMetadata Metadata { get; }

    /// <inheritdoc/>
    public Task<ExportResult> ExportAsync(
        IReadOnlyList<WordEntry> entries,
        Stream output,
        ExportOptions? options = null,
        CancellationToken ct = default)
    {
        var legacyList = ConvertToWordLibraryList(entries);
        var lines = _legacyExporter.Export(legacyList);

        var encoding = _legacyExporter.Encoding ?? Encoding.UTF8;
        using var writer = new StreamWriter(output, encoding, leaveOpen: true);

        var exportedCount = 0;
        foreach (var line in lines)
        {
            ct.ThrowIfCancellationRequested();
            writer.Write(line);
            exportedCount++;
        }

        writer.Flush();

        return Task.FromResult(new ExportResult
        {
            EntryCount = exportedCount,
            ErrorCount = 0
        });
    }

    private static WordLibraryList ConvertToWordLibraryList(IReadOnlyList<WordEntry> entries)
    {
        var list = new WordLibraryList();
        foreach (var entry in entries)
        {
            var wl = new WordLibrary
            {
                Word = entry.Word,
                Rank = entry.Rank,
                CodeType = MapCodeType(entry.CodeType),
                IsEnglish = entry.IsEnglish
            };

            if (entry.Code is not null)
            {
                wl.Codes = ConvertToLegacyCode(entry.Code);
            }

            list.Add(wl);
        }

        return list;
    }

    private static Code ConvertToLegacyCode(WordCode wordCode)
    {
        var code = new Code();
        foreach (var segment in wordCode.Segments)
        {
            code.Add(segment.ToList());
        }

        return code;
    }

    /// <summary>Maps new <see cref="NewCodeType"/> to legacy <see cref="OldCodeType"/>.</summary>
    internal static OldCodeType MapCodeType(NewCodeType newType) => newType switch
    {
        NewCodeType.Pinyin => OldCodeType.Pinyin,
        NewCodeType.Wubi86 => OldCodeType.Wubi,
        NewCodeType.Wubi98 => OldCodeType.Wubi98,
        NewCodeType.WubiNewAge => OldCodeType.WubiNewAge,
        NewCodeType.Zhengma => OldCodeType.Zhengma,
        NewCodeType.Cangjie5 => OldCodeType.Cangjie,
        NewCodeType.TerraPinyin => OldCodeType.TerraPinyin,
        NewCodeType.Zhuyin => OldCodeType.Zhuyin,
        NewCodeType.English => OldCodeType.English,
        NewCodeType.UserDefine => OldCodeType.UserDefine,
        NewCodeType.NoCode => OldCodeType.NoCode,
        NewCodeType.QingsongErbi => OldCodeType.QingsongErbi,
        NewCodeType.ChaoqiangErbi => OldCodeType.ChaoqiangErbi,
        NewCodeType.XiandaiErbi => OldCodeType.XiandaiErbi,
        NewCodeType.Chaoyin => OldCodeType.Chaoyin,
        _ => OldCodeType.Pinyin
    };
}
