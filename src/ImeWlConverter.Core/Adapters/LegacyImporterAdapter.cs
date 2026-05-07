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
/// Adapts legacy <see cref="IWordLibraryImport"/> to the new <see cref="IFormatImporter"/> interface.
/// </summary>
public sealed class LegacyImporterAdapter : IFormatImporter
{
    private readonly IWordLibraryImport _legacyImporter;

    /// <summary>
    /// Initializes a new instance of <see cref="LegacyImporterAdapter"/>.
    /// </summary>
    /// <param name="legacyImporter">The legacy importer to wrap.</param>
    /// <param name="metadata">Format metadata describing this importer.</param>
    public LegacyImporterAdapter(IWordLibraryImport legacyImporter, FormatMetadata metadata)
    {
        _legacyImporter = legacyImporter;
        Metadata = metadata;
    }

    /// <inheritdoc/>
    public FormatMetadata Metadata { get; }

    /// <inheritdoc/>
    public Task<ImportResult> ImportAsync(Stream input, ImportOptions? options = null, CancellationToken ct = default)
    {
        // The legacy importer works with file paths, not streams.
        // Write the stream to a temp file for the legacy API.
        var tempFile = Path.GetTempFileName();
        try
        {
            using (var fs = File.Create(tempFile))
            {
                input.CopyTo(fs);
            }

            var legacyList = _legacyImporter.Import(tempFile);
            var entries = ConvertToWordEntries(legacyList);
            return Task.FromResult(new ImportResult
            {
                Entries = entries,
                ErrorCount = 0
            });
        }
        finally
        {
            if (File.Exists(tempFile))
                File.Delete(tempFile);
        }
    }

    /// <summary>Import directly from a file path (legacy compatibility).</summary>
    /// <param name="filePath">Path to the input file.</param>
    /// <returns>The import result containing converted word entries.</returns>
    public ImportResult ImportFromFile(string filePath)
    {
        var legacyList = _legacyImporter.Import(filePath);
        return new ImportResult
        {
            Entries = ConvertToWordEntries(legacyList),
            ErrorCount = 0
        };
    }

    private static List<WordEntry> ConvertToWordEntries(WordLibraryList legacyList)
    {
        var entries = new List<WordEntry>(legacyList.Count);
        foreach (var wl in legacyList)
        {
            entries.Add(new WordEntry
            {
                Word = wl.Word,
                Rank = wl.Rank,
                CodeType = MapCodeType(wl.CodeType),
                IsEnglish = wl.IsEnglish,
                Code = ConvertToWordCode(wl.Codes)
            });
        }

        return entries;
    }

    private static WordCode? ConvertToWordCode(Code? legacyCodes)
    {
        if (legacyCodes is null || legacyCodes.Count == 0)
            return null;

        var segments = new List<IReadOnlyList<string>>(legacyCodes.Count);
        foreach (var segment in legacyCodes)
        {
            segments.Add(segment.ToList());
        }

        return new WordCode { Segments = segments };
    }

    /// <summary>Maps legacy <see cref="OldCodeType"/> to new <see cref="NewCodeType"/>.</summary>
    internal static NewCodeType MapCodeType(OldCodeType old) => old switch
    {
        OldCodeType.Pinyin => NewCodeType.Pinyin,
        OldCodeType.Wubi => NewCodeType.Wubi86,
        OldCodeType.Wubi98 => NewCodeType.Wubi98,
        OldCodeType.WubiNewAge => NewCodeType.WubiNewAge,
        OldCodeType.Zhengma => NewCodeType.Zhengma,
        OldCodeType.Cangjie => NewCodeType.Cangjie5,
        OldCodeType.TerraPinyin => NewCodeType.TerraPinyin,
        OldCodeType.Zhuyin => NewCodeType.Zhuyin,
        OldCodeType.English => NewCodeType.English,
        OldCodeType.UserDefine => NewCodeType.UserDefine,
        OldCodeType.NoCode => NewCodeType.NoCode,
        OldCodeType.QingsongErbi => NewCodeType.QingsongErbi,
        OldCodeType.ChaoqiangErbi => NewCodeType.ChaoqiangErbi,
        OldCodeType.XiandaiErbi => NewCodeType.XiandaiErbi,
        OldCodeType.Chaoyin => NewCodeType.Chaoyin,
        _ => NewCodeType.Pinyin
    };
}
