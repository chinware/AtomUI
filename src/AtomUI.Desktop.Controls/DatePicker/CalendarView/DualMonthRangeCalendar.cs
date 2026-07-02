namespace AtomUI.Desktop.Controls.CalendarView;

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

}
