namespace AtomUI.Desktop.Controls.CalendarView;

internal class RangeCalendarItem : CalendarItem
{
    protected override Type StyleKeyOverride => typeof(CalendarItem);

    protected override void CheckButtonSelectedState(CalendarDayButton childButton, DateTime dateToAdd)
    {
        // SET IF THE DAY IS SELECTED OR NOT
        childButton.IsSelected    = false;
        childButton.IsRangeStart  = false;
        childButton.IsRangeEnd    = false;
        childButton.IsRangeMiddle = false;
        if (Owner is RangeCalendar owner)
        {
            DateTime? rangeStart = default;
            DateTime? rangeEnd   = default;
            owner.SortHoverIndexes(out rangeStart, out rangeEnd);
            if (rangeStart != null && rangeEnd != null)
            {
                childButton.IsSelected = DateTimeHelper.InRange(dateToAdd, rangeStart.Value, rangeEnd.Value);
                if (childButton.IsSelected)
                {
                    if (DateTimeHelper.CompareDays(dateToAdd, rangeStart.Value) == 0)
                    {
                        childButton.IsRangeStart  = true;
                    }
                    else if (DateTimeHelper.CompareDays(dateToAdd, rangeEnd.Value) == 0)
                    {
                        childButton.IsRangeEnd    = true;
                    }
                    else
                    {
                        childButton.IsRangeMiddle = true;
                    }
                }
            }
        }
    }

    protected override bool CheckDayInactiveState(CalendarDayButton childButton, DateTime dateToAdd)
    {
        if (childButton.Parent == MonthView)
        {
            return base.CheckDayInactiveState(childButton, dateToAdd);
        }

        var isSecondaryDayInactive = false;
        if (Owner is RangeCalendar owner)
        {
            isSecondaryDayInactive =
                DateTimeHelper.CompareYearMonth(dateToAdd, owner.SecondaryDisplayDateInternal) != 0;
        }

        return isSecondaryDayInactive;
    }

    protected override void NotifyCellMouseEntered(CalendarDayButton dayButton, DateTime selectedDate)
    {
        if (Owner is RangeCalendar owner)
        {
            owner.NotifyHoverDateChanged(selectedDate);
            owner.UpdateHighlightDays();
        }
    }

    protected override void NotifyCellMouseLeftButtonDown(CalendarDayButton dayButton)
    {
        if (Owner is RangeCalendar owner)
        {
            if (dayButton.IsEnabled && !dayButton.IsBlackout && dayButton.DataContext is DateTime selectedDate)
            {
                // Set the start or end of the selection
                // range
                if (owner.IsSelectRangeStart)
                {
                    owner.SelectedDate = selectedDate;
                    owner.NotifyDateSelected(selectedDate);
                }
                else
                {
                    owner.SecondarySelectedDate = selectedDate;
                    owner.NotifyDateSelected(selectedDate);
                }

                if (owner.SelectedDate is not null && owner.SecondarySelectedDate is not null)
                {
                    owner.NotifyRangeDateSelected();
                }

                owner.HoverDateTime = null;
                owner.UpdateHighlightDays();
            }
        }
    }

    protected override void NotifyPointerOutMonthView(bool originInMonthView)
    {
        if (Owner is RangeCalendar owner)
        {
            owner.HoverDateTime = null;
            Owner.UpdateHighlightDays();
        }
    }
}