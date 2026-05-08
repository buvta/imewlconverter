namespace ImeWlConverter.Formats.SelfDefining;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Formats.Shared;
/// <summary>Self-defining format importer (tab-separated: code, word, rank).</summary>
[FormatPlugin("self", "自定义", 2000)]
public sealed partial class SelfDefiningImporter : BinaryFormatImporter
{
    protected override IReadOnlyList<WordEntry> ParseBinary(Stream input, CancellationToken ct)
    {
        // SelfDefining requires ParsePattern configuration which is set externally.
        // Use a simple line-by-line tab-separated fallback here.
        using var reader = new StreamReader(input, Encoding.UTF8);
        var entries = new List<WordEntry>();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('\t');
            if (parts.Length < 2) continue;

            entries.Add(new WordEntry
            {
                Word = parts[1],
                Rank = parts.Length > 2 && int.TryParse(parts[2], out var r) ? r : 0,
                CodeType = CodeType.Pinyin,
                Code = WordCode.FromSingle(parts[0].Split('\''))
            });
        }

        return entries;
    }
}
