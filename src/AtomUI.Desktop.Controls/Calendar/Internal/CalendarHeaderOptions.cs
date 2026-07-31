namespace AtomUI.Desktop.Controls.Internal.Calendar;

/// <summary>
/// 默认 CalendarHeader 的年/月选项计算。纯逻辑，无控件依赖，可独立单测。
/// </summary>
internal static class CalendarHeaderOptions
{
    /// <summary>
    /// 计算 Year Select 的年份选项（首尾包含）。
    /// 无 ValidRange 时为 [current-10, current+9] 共 20 年；有 ValidRange 时为 Start.Year..End.Year。
    /// </summary>
    public static IReadOnlyList<int> BuildYearOptions(int currentYear, int? rangeStartYear, int? rangeEndYear)
    {
        int first, last;
        if (rangeStartYear is { } s && rangeEndYear is { } e)
        {
            first = s;
            last = e;
        }
        else
        {
            first = Math.Max(DateTime.MinValue.Year, currentYear - 10);
            last = Math.Min(DateTime.MaxValue.Year, currentYear + 9);
        }

        var years = new List<int>(Math.Max(0, last - first + 1));
        for (var y = first; y <= last; y++)
        {
            years.Add(y);
        }

        return years;
    }

    /// <summary>
    /// 计算某年 Month Select 的可选月份（1..12，首尾包含）。
    /// 当该年处于 ValidRange 边界年份时，只返回与范围相交的月份。
    /// </summary>
    public static IReadOnlyList<int> BuildMonthOptions(int year, DateTime? rangeStart, DateTime? rangeEnd)
    {
        var firstMonth = 1;
        var lastMonth = 12;

        if (rangeStart is { } s && s.Year == year)
        {
            firstMonth = Math.Max(firstMonth, s.Month);
        }

        if (rangeStart is { } s2 && year < s2.Year)
        {
            return Array.Empty<int>();
        }

        if (rangeEnd is { } e && e.Year == year)
        {
            lastMonth = Math.Min(lastMonth, e.Month);
        }

        if (rangeEnd is { } e2 && year > e2.Year)
        {
            return Array.Empty<int>();
        }

        if (lastMonth < firstMonth)
        {
            return Array.Empty<int>();
        }

        var months = new List<int>(lastMonth - firstMonth + 1);
        for (var m = firstMonth; m <= lastMonth; m++)
        {
            months.Add(m);
        }

        return months;
    }

    /// <summary>
    /// 切换到某年时，把当前月份收敛到该年可选月份范围内。
    /// </summary>
    public static int ClampMonthToYear(int desiredMonth, int year, DateTime? rangeStart, DateTime? rangeEnd)
    {
        var months = BuildMonthOptions(year, rangeStart, rangeEnd);
        if (months.Count == 0)
        {
            return desiredMonth;
        }

        if (desiredMonth < months[0])
        {
            return months[0];
        }

        if (desiredMonth > months[^1])
        {
            return months[^1];
        }

        return desiredMonth;
    }
}
