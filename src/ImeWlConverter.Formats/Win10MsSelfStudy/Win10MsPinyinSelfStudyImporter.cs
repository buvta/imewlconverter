namespace ImeWlConverter.Formats.Win10MsSelfStudy;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;
using Studyzy.IMEWLConverter.IME;

/// <summary>Win10 Microsoft Pinyin self-study dictionary importer (binary, delegates to legacy).</summary>
[FormatPlugin("win10mspyss", "Win10微软拼音自学习", 130)]
public sealed partial class Win10MsPinyinSelfStudyImporter : BinaryFormatImporter
{
    static Win10MsPinyinSelfStudyImporter()
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

            var legacy = new Win10MsPinyinSelfStudy();
            var legacyResult = legacy.Import(tempFile);

            return legacyResult.Select(wl => new WordEntry
            {
                Word = wl.Word,
                Rank = wl.Rank,
                CodeType = CodeType.Pinyin,
                Code = wl.PinYin?.Length > 0
                    ? WordCode.FromSingle(wl.PinYin)
                    : null
            }).ToList();
        }
        finally
        {
            if (File.Exists(tempFile)) File.Delete(tempFile);
        }
    }
}
