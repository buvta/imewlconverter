using System.Text;
using System.Text.RegularExpressions;

namespace ImeWlConverter.Core.Utilities;

/// <summary>
/// Converts numeric characters in strings to their Chinese character equivalents.
/// </summary>
public static partial class NumberToChineseConverter
{
    private static readonly string ChineseDigits = "零一二三四五六七八九";
    private static readonly string ChineseUnits = "个十百千万亿兆京垓秭穰沟涧正载极";
    private static readonly Regex Num2ChsRegex = CreateNum2ChsRegex();

    [GeneratedRegex("[1-9].+(0{2,100})")]
    private static partial Regex CreateNum2ChsRegex();

    /// <summary>
    /// Converts numbers in a string to Chinese characters.
    /// When a number doesn't start with 0 and ends with multiple 0s,
    /// converts using x千x百 format. Otherwise reads digits individually.
    /// </summary>
    public static string TranslateNumbers(string str)
    {
        var builder = new StringBuilder();
        var buffer = new StringBuilder();

        foreach (var c in str)
        {
            if (c is >= '0' and <= '9')
            {
                buffer.Append(c);
            }
            else
            {
                if (buffer.Length > 0)
                {
                    builder.Append(NumberToChineseString(buffer.ToString()));
                    buffer.Clear();
                }

                builder.Append(c);
            }
        }

        if (buffer.Length > 0)
            builder.Append(NumberToChineseString(buffer.ToString()));

        return builder.ToString();
    }

    /// <summary>
    /// Converts a numeric string to Chinese.
    /// If the number matches the pattern (non-zero start, trailing zeros), uses positional format.
    /// Otherwise reads each digit individually.
    /// </summary>
    public static string NumberToChineseString(string str)
    {
        if (Num2ChsRegex.IsMatch(str))
            return IntegerToChinese(long.Parse(str));

        var chars = new char[str.Length];
        for (var i = 0; i < str.Length; i++)
            chars[i] = DigitToChinese(str[i]);
        return new string(chars);
    }

    /// <summary>Converts a single digit character ('0'-'9') to its Chinese equivalent.</summary>
    public static char DigitToChinese(char c) => c switch
    {
        '1' => '一',
        '2' => '二',
        '3' => '三',
        '4' => '四',
        '5' => '五',
        '6' => '六',
        '7' => '七',
        '8' => '八',
        '9' => '九',
        _ => '零'
    };

    /// <summary>
    /// Converts an integer to its Chinese positional representation (e.g., 1234 → 一千二百三十四).
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the input exceeds the maximum supported value.</exception>
    public static string IntegerToChinese(long input)
    {
        var absValue = Math.Abs(input);
        var wanIndex = ChineseUnits.IndexOf('万');
        var maxPower = 4 * (ChineseUnits.Length - wanIndex);

        if (absValue > Math.Pow(10, maxPower))
            throw new ArgumentOutOfRangeException(nameof(input), $"The input is too large: {input}");

        var groups = SplitIntoGroups(absValue);
        var result = ConvertGroupsToChinese(groups);

        if (input < 0)
            result = "-" + result;

        return result;
    }

    /// <summary>
    /// Splits a number into groups of up to 4 digits (ones through thousands).
    /// Groups are ordered from most significant to least significant.
    /// </summary>
    private static List<List<int>> SplitIntoGroups(long value)
    {
        var digitIndex = 0;
        var remaining = value;
        var groups = new List<List<int>>();
        List<int> currentGroup;

        do
        {
            var digit = (int)(remaining % 10);
            remaining /= 10;

            if (digitIndex % 4 == 0)
            {
                currentGroup = [digit];
                if (groups.Count == 0)
                    groups.Add(currentGroup);
                else
                    groups.Insert(0, currentGroup);
            }
            else
            {
                currentGroup = groups[0];
                currentGroup.Insert(0, digit);
            }

            digitIndex++;
        } while (remaining > 0);

        return groups;
    }

    /// <summary>
    /// Converts the digit groups into a Chinese string with appropriate unit markers.
    /// Handles zero compression, leading 一十 simplification, and group joining with 万/亿/兆 etc.
    /// </summary>
    private static string ConvertGroupsToChinese(List<List<int>> groups)
    {
        var groupStrings = new List<StringBuilder>();

        for (var i = 0; i < groups.Count; i++)
        {
            var digits = groups[i];
            var sb = new StringBuilder();

            var j = 0;
            while (j < digits.Count)
            {
                if (digits[j] == 0)
                {
                    // Collapse consecutive zeros into a single 零
                    var k = j + 1;
                    while (k < digits.Count && digits[k] == 0)
                        k++;
                    sb.Append('零');
                    j = k;
                }
                else
                {
                    sb.Append(ChineseDigits[digits[j]]);
                    sb.Append(ChineseUnits[digits.Count - 1 - j]);
                    j++;
                }
            }

            // Remove trailing 零 (if more than one char)
            if (sb.Length > 1 && sb[sb.Length - 1] == '零')
                sb.Remove(sb.Length - 1, 1);
            // Remove trailing 个 unit marker
            else if (sb[sb.Length - 1] == '个')
                sb.Remove(sb.Length - 1, 1);

            // Simplify leading 一十 to 十 for the first group
            if (i == 0 && sb.Length > 1 && sb[0] == '一' && sb[1] == '十')
                sb.Remove(0, 1);

            groupStrings.Add(sb);
        }

        var result = new StringBuilder();
        var qianIndex = ChineseUnits.IndexOf('千');

        for (var i = 0; i < groupStrings.Count; i++)
        {
            if (groupStrings.Count == 1)
            {
                result.Append(groupStrings[i]);
            }
            else
            {
                if (i == groupStrings.Count - 1)
                {
                    // Last group: only append if it's not just 零
                    if (groupStrings[i][groupStrings[i].Length - 1] != '零')
                        result.Append(groupStrings[i]);
                }
                else
                {
                    // Non-last groups: append with 万/亿/兆 unit, skip if group is 零
                    if (groupStrings[i][0] != '零')
                    {
                        result.Append(groupStrings[i]);
                        result.Append(ChineseUnits[qianIndex + groupStrings.Count - 1 - i]);
                    }
                }
            }
        }

        return result.ToString();
    }
}
