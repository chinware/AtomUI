namespace AtomUI.Desktop.Controls.CalendarView;

internal class DualMonthRangeCalendar : RangeCalendar
{
    internal override void ResetStates()
    {
        base.ResetStates();
        if (CalendarItem is DualMonthCalendarItem dualMonthCalendarItem)
        {
            if (dualMonthCalendarItem.SecondaryMonthView is not null)
            {
                var monthView = dualMonthCalendarItem.SecondaryMonthView;
                foreach (var d in monthView.Children.OfType<CalendarDayButton>())
                {
                    d.IgnoreMouseOverState();
                }
            }
        }
    }

}
