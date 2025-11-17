using System.Diagnostics;

namespace AtomUI.Controls.CalendarView;

internal class DualMonthRangeCalendar : RangeCalendar
{
    internal override void ResetStates()
    {
        base.ResetStates();
        var count = RowsPerMonth * ColumnsPerMonth;
        if (CalendarItem is DualMonthCalendarItem dualMonthCalendarItem)
        {
            if (dualMonthCalendarItem.SecondaryMonthView is not null)
            {
                var monthView = dualMonthCalendarItem.SecondaryMonthView;
                for (var childIndex = ColumnsPerMonth; childIndex < count; childIndex++)
                {
                    var d = (CalendarDayButton)monthView.Children[childIndex];
                    d.IgnoreMouseOverState();
                }
            }
        }
    }

    internal override void UpdateHighlightDays()
    {
        DateTime? rangeStart = default;
        DateTime? rangeEnd   = default;
        SortHoverIndexes(out rangeStart, out rangeEnd);
        Debug.Assert(CalendarItem is not null);
        if (CalendarItem is DualMonthCalendarItem dualMonthCalendarItem)
        {
            if (dualMonthCalendarItem.MonthView is not null)
            {
                UpdateHighlightDays(dualMonthCalendarItem.MonthView, rangeStart, rangeEnd);
            }

            if (dualMonthCalendarItem.SecondaryMonthView is not null)
            {
                UpdateHighlightDays(dualMonthCalendarItem.SecondaryMonthView, rangeStart, rangeEnd);
            }
        }
    }

    internal override void UnHighlightDays()
    {
        base.UnHighlightDays();
        if (CalendarItem is DualMonthCalendarItem dualMonthCalendarItem)
        {
            if (dualMonthCalendarItem.SecondaryMonthView is not null)
            {
                UnHighlightDays(dualMonthCalendarItem.SecondaryMonthView);
            }
        }
    }
}