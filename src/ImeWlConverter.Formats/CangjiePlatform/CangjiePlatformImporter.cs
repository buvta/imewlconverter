namespace ImeWlConverter.Formats.CangjiePlatform;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>CangjiePlatform dictionary importer (space-separated code+word).</summary>
[FormatPlugin("cjpt", "仓颉平台", 230)]
public sealed partial class CangjiePlatformImporter : TextFormatImporter
{
    static CangjiePlatformImporter()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    protected override Encoding FileEncoding => Encoding.GetEncoding("GBK");
    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        var parts = line.Split(' ');
        if (parts.Length < 2)
            yield break;

        yield return new WordEntry
        {
            Word = parts[1],
            CodeType = CodeType.Cangjie5,
            Code = WordCode.FromSingle(new[] { parts[0] })
        };
    }
}
