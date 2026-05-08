namespace ImeWlConverter.Formats.MsPinyin;

using System.Text;
using System.Xml;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Enums;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Abstractions.Options;
using ImeWlConverter.Abstractions.Results;

/// <summary>Microsoft Pinyin dictionary importer (XML format).</summary>
[FormatPlugin("mspy", "微软拼音", 135)]
public sealed partial class MsPinyinImporter : IFormatImporter
{

    public Task<ImportResult> ImportAsync(Stream input, ImportOptions? options = null, CancellationToken ct = default)
    {
        var entries = new List<WordEntry>();
        var errors = new List<string>();

        using var reader = new StreamReader(input, Encoding.UTF8);
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(reader.ReadToEnd());
        var nsMgr = new XmlNamespaceManager(xmlDoc.NameTable);
        nsMgr.AddNamespace("ns1", "http://www.microsoft.com/ime/dctx");

        var nodes = xmlDoc.SelectNodes("//ns1:Dictionary/ns1:DictionaryEntry", nsMgr);
        if (nodes == null)
            return Task.FromResult(new ImportResult { Entries = entries, Errors = errors });

        for (var i = 0; i < nodes.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                var xn = nodes[i]!;
                var py = xn.SelectSingleNode("ns1:InputString", nsMgr)!.InnerText;
                var word = xn.SelectSingleNode("ns1:OutputString", nsMgr)!.InnerText;

                // Remove tone numbers from pinyin
                var pinyinParts = py.Split(new[] { ' ', '1', '2', '3', '4' },
                    StringSplitOptions.RemoveEmptyEntries);

                entries.Add(new WordEntry
                {
                    Word = word,
                    Rank = 1,
                    CodeType = CodeType.Pinyin,
                    Code = WordCode.FromSingle(pinyinParts)
                });
            }
            catch (Exception ex)
            {
                errors.Add($"Entry {i}: {ex.Message}");
            }
        }

        return Task.FromResult(new ImportResult
        {
            Entries = entries,
            ErrorCount = errors.Count,
            Errors = errors
        });
    }
}
