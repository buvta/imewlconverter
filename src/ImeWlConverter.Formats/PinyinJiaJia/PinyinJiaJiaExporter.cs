namespace ImeWlConverter.Formats.PinyinJiaJia;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>PinyinJiaJia dictionary exporter (interleaved Chinese+pinyin format).</summary>
[FormatPlugin("pyjj", "拼音加加", 120)]
public sealed partial class PinyinJiaJiaExporter : TextFormatExporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    /// <summary>
    /// Formats as interleaved Chinese+pinyin, e.g. "深shen蓝lan".
    /// </summary>
    protected override string? FormatEntry(WordEntry entry)
    {
        var codes = entry.Code?.Segments;
        if (codes == null || codes.Count != entry.Word.Length)
            return null;

        var sb = new StringBuilder();
        for (var i = 0; i < entry.Word.Length; i++)
        {
            sb.Append(entry.Word[i]);
            sb.Append(codes[i][0]);
        }

        return sb.ToString();
    }
}
