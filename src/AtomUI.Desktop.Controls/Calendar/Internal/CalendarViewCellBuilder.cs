using System.Globalization;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

internal static class CalendarViewCellBuilder
{
    internal const int DateGridCellCount = 42;
    internal const int DateGridRows = 6;
    internal const int DateGridColumns = 7;

    public static IReadOnlyList<CalendarViewCellModel> BuildDateCells(
        DateTime anchor,
        DateTime today,
        DayOfWeek firstDayOfWeek,
        DateTime? validRangeStart,
        DateTime? validRangeEnd,
        Func<DateTime, bool>? disabledDate)
    {
        anchor = anchor.Date;
        today = today.Date;
        var gridStart = GetDateGridStart(anchor, firstDayOfWeek);

        var cells = new List<CalendarViewCellModel>(DateGridCellCount);
        for (var i = 0; i < DateGridCellCount; i++)
        {
            var date = gridStart.AddDays(i);
            var isInView = date.Year == anchor.Year && date.Month == anchor.Month;
            var isDisabled = IsOutsideRange(date, validRangeStart, validRangeEnd)
                             || (disabledDate?.Invoke(date) ?? false);
            cells.Add(new CalendarViewCellModel(
                Value: date,
                Kind: CalendarViewCellKind.Date,
                DisplayText: date.Day.ToString("D2", CultureInfo.InvariantCulture),
                IsToday: date == today,
                IsInView: isInView,
                IsSelected: date == anchor,
                IsDisabled: isDisabled,
                IsFocusable: !isDisabled));
        }

        return cells;
    }

    internal static DateTime GetDateGridStart(DateTime anchor, DayOfWeek firstDayOfWeek)
    {
        anchor = anchor.Date;
        var monthStart = new DateTime(anchor.Year, anchor.Month, 1);
        var offset = ((int)monthStart.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        var gridStartTicks = Math.Max(DateTime.MinValue.Ticks, monthStart.Ticks - offset * TimeSpan.TicksPerDay);
        var gridStart = new DateTime(gridStartTicks);
        var latestGridStart = DateTime.MaxValue.Date.AddDays(-(DateGridCellCount - 1));
        if (gridStart > latestGridStart)
        {
            gridStart = latestGridStart;
        }

        return gridStart;
    }

    public static IReadOnlyList<CalendarViewCellModel> BuildWeekNumberCells(
        IReadOnlyList<CalendarViewCellModel> dateCells,
        CultureInfo culture,
        CalendarWeekRule weekRule,
        DayOfWeek firstDayOfWeek)
    {
        var weeks = new List<CalendarViewCellModel>(6);
        for (var row = 0; row < 6; row++)
        {
            var rowStart = dateCells[row * 7].Value;
            var weekNum = culture.Calendar.GetWeekOfYear(rowStart, weekRule, firstDayOfWeek);
            weeks.Add(new CalendarViewCellModel(
                Value: rowStart,
                Kind: CalendarViewCellKind.Week,
                DisplayText: weekNum.ToString(CultureInfo.InvariantCulture),
                IsToday: false,
                IsInView: true,
                IsSelected: false,
                IsDisabled: dateCells[row * 7].IsDisabled,
                IsFocusable: false));
        }

        return weeks;
    }

    public static IReadOnlyList<CalendarViewCellModel> BuildMonthCells(
        DateTime anchor,
        DateTime today,
        CultureInfo culture,
        DateTime? validRangeStart,
        DateTime? validRangeEnd,
        Func<DateTime, bool>? disabledDate)
    {
        anchor = anchor.Date;
        today = today.Date;
        var year = anchor.Year;
        var monthNames = culture.DateTimeFormat.AbbreviatedMonthNames; // index 0..11 有效
        var cells = new List<CalendarViewCellModel>(12);

        for (var month = 1; month <= 12; month++)
        {
            var lastDay = DateTime.DaysInMonth(year, month);
            var day = Math.Min(anchor.Day, lastDay);
            var candidate = new DateTime(year, month, day);
            var monthFirst = new DateTime(year, month, 1);
            var monthLast = new DateTime(year, month, lastDay);

            var isDisabled = IsDisabledAtMonthBoundary(monthFirst, monthLast, validRangeStart, validRangeEnd, disabledDate);

            cells.Add(new CalendarViewCellModel(
                Value: candidate,
                Kind: CalendarViewCellKind.Month,
                DisplayText: monthNames[month - 1],
                IsToday: today.Year == year && today.Month == month,
                IsInView: true,
                IsSelected: anchor.Month == month,
                IsDisabled: isDisabled,
                IsFocusable: !isDisabled));
        }

        return cells;
    }

    private static bool IsOutsideRange(DateTime date, DateTime? start, DateTime? end)
    {
        if (start is { } s && date < s.Date)
        {
            return true;
        }

        if (end is { } e && date > e.Date)
        {
            return true;
        }

        return false;
    }

    private static bool MonthOutsideRange(DateTime monthFirst, DateTime monthLast, DateTime? start, DateTime? end)
    {
        if (start is { } s && monthLast < s.Date)
        {
            return true;
        }

        if (end is { } e && monthFirst > e.Date)
        {
            return true;
        }

        return false;
    }

    private static bool IsDisabledAtMonthBoundary(
        DateTime monthFirst,
        DateTime monthLast,
        DateTime? start,
        DateTime? end,
        Func<DateTime, bool>? disabledDate)
    {
        var firstDisabled = IsOutsideRange(monthFirst, start, end) || (disabledDate?.Invoke(monthFirst) ?? false);
        var lastDisabled = IsOutsideRange(monthLast, start, end) || (disabledDate?.Invoke(monthLast) ?? false);
        return firstDisabled && lastDisabled;
    }
}
