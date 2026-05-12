using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.Filters;

public sealed class RankPercentageFilter : IBatchFilter
{
    public int Percentage { get; init; } = 100;

    public IReadOnlyList<WordEntry> Filter(IReadOnlyList<WordEntry> entries)
    {
        if (Percentage >= 100)
            return entries;

        var count = entries.Count * Percentage / 100;
        var sorted = entries.OrderByDescending(e => e.Rank).ToList();
        return sorted.Take(count).ToList();
    }
}
