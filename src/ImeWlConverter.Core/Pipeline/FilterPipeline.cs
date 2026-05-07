using ImeWlConverter.Abstractions.Contracts;
using ImeWlConverter.Abstractions.Models;

namespace ImeWlConverter.Core.Pipeline;

/// <summary>
/// Applies a chain of filters and transforms to word entries.
/// Filters remove entries, transforms modify them, and batch filters operate on the full collection.
/// </summary>
public sealed class FilterPipeline
{
    private readonly IReadOnlyList<IWordFilter> _filters;
    private readonly IReadOnlyList<IWordTransform> _transforms;
    private readonly IReadOnlyList<IBatchFilter> _batchFilters;

    /// <summary>
    /// Initializes a new instance of <see cref="FilterPipeline"/>.
    /// </summary>
    /// <param name="filters">Individual entry filters (keep/reject).</param>
    /// <param name="transforms">Individual entry transforms (modify/remove).</param>
    /// <param name="batchFilters">Batch-level filters (e.g., deduplication).</param>
    public FilterPipeline(
        IEnumerable<IWordFilter>? filters = null,
        IEnumerable<IWordTransform>? transforms = null,
        IEnumerable<IBatchFilter>? batchFilters = null)
    {
        _filters = filters?.ToList() ?? [];
        _transforms = transforms?.ToList() ?? [];
        _batchFilters = batchFilters?.ToList() ?? [];
    }

    /// <summary>
    /// Apply all filters and transforms to the given entries.
    /// Processing order: single filters → transforms → batch filters.
    /// </summary>
    /// <param name="entries">The entries to process.</param>
    /// <returns>The filtered and transformed entries.</returns>
    public IReadOnlyList<WordEntry> Apply(IReadOnlyList<WordEntry> entries)
    {
        // Apply single-entry filters and transforms
        var result = new List<WordEntry>(entries.Count);

        foreach (var entry in entries)
        {
            if (!_filters.All(f => f.ShouldKeep(entry)))
                continue;

            var transformed = ApplyTransforms(entry);
            if (transformed is not null && !string.IsNullOrEmpty(transformed.Word))
                result.Add(transformed);
        }

        // Apply batch filters
        IReadOnlyList<WordEntry> batchResult = result;
        foreach (var batchFilter in _batchFilters)
        {
            batchResult = batchFilter.Filter(batchResult);
        }

        return batchResult;
    }

    /// <summary>
    /// Applies all transforms to a single entry.
    /// Returns null if any transform removes the entry.
    /// </summary>
    private WordEntry? ApplyTransforms(WordEntry entry)
    {
        var current = entry;
        foreach (var transform in _transforms)
        {
            current = transform.Transform(current);
            if (current is null)
                return null;
        }

        return current;
    }
}
