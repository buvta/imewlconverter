namespace ImeWlConverter.Formats.JidianMBDict;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;
using Studyzy.IMEWLConverter.IME;

/// <summary>Jidian MB dictionary importer (binary, delegates to legacy).</summary>
[FormatPlugin("jdmb", "极点码表", 190)]
public sealed partial class JidianMBDictImporter : BinaryFormatImporter
{
    static JidianMBDictImporter()
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

            var legacy = new Jidian_MBDict();
            var legacyResult = legacy.Import(tempFile);

            return legacyResult.Select(wl => new WordEntry
            {
                Word = wl.Word,
                Rank = wl.Rank,
                CodeType = wl.CodeType == Studyzy.IMEWLConverter.Entities.CodeType.Pinyin
                    ? CodeType.Pinyin : CodeType.Wubi98,
                Code = wl.PinYin?.Length > 0
                    ? WordCode.FromSingle(wl.PinYin)
                    : wl.Codes?.Count > 0
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
