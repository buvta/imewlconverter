namespace ImeWlConverter.Formats.Xiaoxiao;

using System.Text;
using System.Text.RegularExpressions;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>Xiaoxiao IME dictionary importer (text format with code+word pairs).</summary>
[FormatPlugin("xiaoxiao", "小小输入法", 100)]
public sealed partial class XiaoxiaoImporter : TextFormatImporter
{
    private static readonly Regex ContentRegex = new(@"[^\s#]+( [\u4E00-\u9FA5]+)+");

    static XiaoxiaoImporter()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    protected override Encoding FileEncoding
    {
        get
        {
            try { return Encoding.GetEncoding("GB18030"); }
            catch { return Encoding.GetEncoding("GB2312"); }
        }
    }
    protected override bool IsContentLine(string line) => ContentRegex.IsMatch(line);

    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        var words = line.Split(' ');
        if (words.Length < 2) yield break;

        var code = words[0];
        for (var i = 1; i < words.Length; i++)
        {
            yield return new WordEntry
            {
                Word = words[i],
                Rank = 0,
                CodeType = CodeType.Pinyin,
                Code = new WordCode { Segments = [new[] { code }] }
            };
        }
    }
}
