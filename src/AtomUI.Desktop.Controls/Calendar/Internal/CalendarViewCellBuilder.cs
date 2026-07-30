using System;
using System.Collections.Generic;
using System.Globalization;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

internal static class CalendarViewCellBuilder
{
    public static IReadOnlyList<CalendarViewCellModel> BuildDateCells(
        DateTime anchor,
        DateTime today,
        DayOfWeek firstDayOfWeek,
        DateTime? validRangeStart,
        DateTime? validRangeEnd,
        Func<DateTime, bool>? disabledDate)
    {
        anchor = anchor.Date;
        today  = today.Date;
        var monthStart = new DateTime(anchor.Year, anchor.Month, 1);
        var offset     = ((int)monthStart.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
        var gridStart  = monthStart.AddDays(-offset);

        var cells = new List<CalendarViewCellModel>(42);
        for (var i = 0; i < 42; i++)
        {
            var date       = gridStart.AddDays(i);
            var isInView   = date.Year == anchor.Year && date.Month == anchor.Month;
            var isDisabled = IsOutsideRange(date, validRangeStart, validRangeEnd)
                             || (disabledDate?.Invoke(date) ?? false);
            cells.Add(new CalendarViewCellModel(
                Value:       date,
                Kind:        CalendarViewCellKind.Date,
                DisplayText: date.Day.ToString("D2", CultureInfo.InvariantCulture),
                IsToday:     date == today,
                IsInView:    isInView,
                IsSelected:  date == anchor,
                IsDisabled:  isDisabled,
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
}
