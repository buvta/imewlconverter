namespace ImeWlConverter.Formats.QQPinyinQpyd;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;
using Studyzy.IMEWLConverter.IME;

/// <summary>QQ Pinyin qpyd dictionary importer (binary, delegates to legacy).</summary>
[FormatPlugin("qpyd", "QQ拼音qpyd", 60)]
public sealed partial class QQPinyinQpydImporter : BinaryFormatImporter
{
    static QQPinyinQpydImporter()
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

            var legacy = new QQPinyinQpyd();
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
