namespace ImeWlConverter.Formats.Win10Ms;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;
using Studyzy.IMEWLConverter.IME;

/// <summary>Win10 Microsoft Wubi dictionary importer (binary, delegates to legacy).</summary>
[FormatPlugin("win10mswb", "Win10微软五笔", 131)]
public sealed partial class Win10MsWubiImporter : BinaryFormatImporter
{
    static Win10MsWubiImporter()
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

            var legacy = new Win10MsWubi();
            var legacyResult = legacy.Import(tempFile);

            return legacyResult.Select(wl => new WordEntry
            {
                Word = wl.Word,
                Rank = wl.Rank,
                CodeType = CodeType.Wubi98,
                Code = wl.Codes?.Count > 0
                    ? new WordCode
                    {
                        Segments = wl.Codes.Select(s => (IReadOnlyList<string>)s.ToList()).ToList()
                    }
                    : null
            }).ToList();
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
