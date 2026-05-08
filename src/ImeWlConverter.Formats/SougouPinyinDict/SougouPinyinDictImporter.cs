namespace ImeWlConverter.Formats.SougouPinyinDict;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Sougou Pinyin dict helper class (data structure only, no import/export).</summary>
/// <remarks>
/// SougouPinyinDict is a helper class used by SougouPinyinBinFromPython.
/// It does not implement import/export itself, so we register it as a metadata-only plugin.
/// The actual import is handled by SougouBinImporter which delegates to the legacy code.
/// </remarks>
[FormatPlugin("sgpydict", "搜狗拼音备份词典", 30)]
public sealed partial class SougouPinyinDictImporter : BinaryFormatImporter
{
    static SougouPinyinDictImporter()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
    protected override IReadOnlyList<WordEntry> ParseBinary(Stream input, CancellationToken ct)
    {
        // SougouPinyinDict is a helper class for the bin format.
        // Actual import is through SougouBinImporter (sgpybin).
        // This delegates to the same legacy code path.
        var tempFile = Path.GetTempFileName();
        try
        {
            using (var fs = File.Create(tempFile))
                input.CopyTo(fs);

            var legacy = new Studyzy.IMEWLConverter.IME.SougouPinyinBinFromPython();
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
