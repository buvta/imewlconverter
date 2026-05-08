namespace ImeWlConverter.Formats.LingoesLd2;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;
using Studyzy.IMEWLConverter.IME;

/// <summary>Lingoes ld2 dictionary importer (binary, delegates to legacy).</summary>
[FormatPlugin("ld2", "灵格斯ld2", 200)]
public sealed partial class LingoesLd2Importer : BinaryFormatImporter
{
    static LingoesLd2Importer()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
    protected override IReadOnlyList<WordEntry> ParseBinary(Stream input, CancellationToken ct)
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            using (var fs = File.Create(tempFile))
                input.CopyTo(fs);

            var legacy = new LingoesLd2();
            var legacyResult = legacy.Import(tempFile);

            return legacyResult.Select(wl => new WordEntry
            {
                Word = wl.Word,
                Rank = wl.Rank,
                CodeType = CodeType.English,
                IsEnglish = wl.IsEnglish
            }).ToList();
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
