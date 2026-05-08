namespace ImeWlConverter.Formats.iFlyIME;

using System.Text;
using ImeWlConverter.Abstractions;
using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;
using ImeWlConverter.Abstractions.Options;
using ImeWlConverter.Abstractions.Results;

/// <summary>iFlyIME (讯飞输入法) dictionary exporter. Splits into 16000-entry files with iFly header.</summary>
[FormatPlugin("ifly", "讯飞输入法", 1050)]
public sealed partial class iFlyIMEExporter : IFormatExporter
{
    private const string HeaderFormat =
        @"###注释部分，请勿修改###
#此文本文件为讯飞输入法用户词库导出所生成
#版本信息:30000008
#词库容量:16384
#词条个数:{0}
#讯飞输入法下载地址:
#http://ime.voicecloud.cn

###格式说明###
#文本编码方式:UTF-8
#词条 类型
#词条部分：限纯中文或纯英文词条，不含空格。中文词条长度上限为16，英文词条长度上限为32
#类型取值：1－联系人，0－其它，若无此项或取值非法，则默认做0处理

###以下为正文内容###";
    public Task<ExportResult> ExportAsync(
        IReadOnlyList<WordEntry> entries, Stream output,
        ExportOptions? options = null, CancellationToken ct = default)
    {
        using var writer = new StreamWriter(output, Encoding.UTF8, leaveOpen: true);
        var count = 0;

        // Filter valid entries first
        var validEntries = new List<string>();
        foreach (var entry in entries)
        {
            ct.ThrowIfCancellationRequested();
            if (entry.Word.Length > 1 && entry.Word.Length < 17)
            {
                validEntries.Add(entry.Word);
            }
        }

        writer.Write(string.Format(HeaderFormat, validEntries.Count));
        writer.Write('\n');

        foreach (var word in validEntries)
        {
            writer.Write(word);
            writer.Write('\n');
            count++;
        }

        writer.Flush();
        return Task.FromResult(new ExportResult { EntryCount = count });
    }
}
