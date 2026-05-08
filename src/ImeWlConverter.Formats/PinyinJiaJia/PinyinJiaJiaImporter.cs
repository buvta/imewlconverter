namespace ImeWlConverter.Formats.PinyinJiaJia;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;

/// <summary>PinyinJiaJia dictionary importer (text format, interleaved Chinese+pinyin).</summary>
[FormatPlugin("pyjj", "拼音加加", 120)]
public sealed partial class PinyinJiaJiaImporter : TextFormatImporter
{
    protected override Encoding FileEncoding => Encoding.Unicode;
    /// <summary>
    /// Parses lines like "深shen蓝lan" where Chinese chars and pinyin are interleaved.
    /// Only polyphones get annotated; normal chars use default reading.
    /// </summary>
    protected override IEnumerable<WordEntry> ParseLine(string line)
    {
        var hz = new StringBuilder();
        var py = new List<string>();
        int j;

        for (j = 0; j < line.Length - 1; j++)
        {
            hz.Append(line[j]);
            if (line[j + 1] > 'z') // next char is Chinese, no pinyin annotation
            {
                // Use empty string as placeholder - the old code used PinyinGenerater
                // In new architecture, pipeline will regenerate pinyin if needed
                py.Add("");
            }
            else // followed by pinyin
            {
                var k = 1;
                var py1 = new StringBuilder();
                while (j + k < line.Length && line[j + k] <= 'z')
                {
                    py1.Append(line[j + k]);
                    k++;
                }
                py.Add(py1.ToString());
                j += k - 1; // -1 because the loop will j++
            }
        }

        if (j == line.Length - 1) // last char is Chinese
        {
            hz.Append(line[j]);
            py.Add("");
        }

        yield return new WordEntry
        {
            Word = hz.ToString(),
            CodeType = CodeType.Pinyin,
            Code = WordCode.FromSingle(py)
        };
    }
}
