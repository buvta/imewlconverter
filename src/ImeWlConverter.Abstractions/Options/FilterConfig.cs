namespace ImeWlConverter.Abstractions.Options;

/// <summary>
/// Configuration for word filtering in the conversion pipeline.
/// Shared across CLI, WinForms GUI, and Mac GUI.
/// </summary>
public class FilterConfig
{
    public bool NoFilter { get; set; }
    public int WordLengthFrom { get; set; } = 1;
    public int WordLengthTo { get; set; } = 9999;
    public int WordRankFrom { get; set; } = 1;
    public int WordRankTo { get; set; } = 999999;
    public int WordRankPercentage { get; set; } = 100;

    // Ignore filters (remove entire entry)
    public bool IgnoreEnglish { get; set; }
    public bool IgnoreNumber { get; set; }
    public bool IgnoreSpace { get; set; }
    public bool IgnorePunctuation { get; set; }
    public bool IgnoreNoAlphabetCode { get; set; }
    public bool IgnoreFirstCJK { get; set; }

    // Replace transforms (strip characters from entry)
    public bool ReplaceEnglish { get; set; }
    public bool ReplaceNumber { get; set; }
    public bool ReplaceSpace { get; set; }
    public bool ReplacePunctuation { get; set; }
}
