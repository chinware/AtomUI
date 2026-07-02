using Avalonia.Controls;

namespace AtomUI.Desktop.Controls.CalendarView.Rendering;

internal sealed class CalendarGeneratedContainerManager
{
    private readonly CalendarItem _ownerItem;

    public CalendarGeneratedContainerManager(CalendarItem ownerItem)
    {
        _ownerItem = ownerItem;
    }

    public void ReleaseMonthView(Grid? monthView)
    {
        if (monthView is null)
        {
            return;
        }

        foreach (var child in monthView.Children)
        {
            if (child is CalendarDayButton dayButton)
            {
                ReleaseDayButton(dayButton);
            }
        }

        monthView.Children.Clear();
    }

    public void ReleaseYearView(Grid? yearView)
    {
        if (yearView is null)
        {
            return;
        }

        foreach (var child in yearView.Children)
        {
            if (child is CalendarButton calendarButton)
            {
                ReleaseCalendarButton(calendarButton);
            }
        }

        yearView.Children.Clear();
    }

    private void ReleaseDayButton(CalendarDayButton dayButton)
    {
        var owner = dayButton.Owner;
        if (owner?.FocusButton == dayButton)
        {
            dayButton.IsCurrent = false;
            owner.FocusButton   = null;
        }

        dayButton.CalendarDayButtonMouseDown -= _ownerItem.HandleCellMouseLeftButtonDown;
        dayButton.CalendarDayButtonMouseUp   -= _ownerItem.HandleCellMouseLeftButtonUp;
        dayButton.PointerEntered             -= _ownerItem.HandleCellMouseEntered;
        dayButton.ClearValue(CalendarDayButton.IsMotionEnabledProperty);
        dayButton.Owner = null;
    }

    private void ReleaseCalendarButton(CalendarButton calendarButton)
    {
        var owner = calendarButton.Owner;
        if (owner?.FocusCalendarButton == calendarButton)
        {
            calendarButton.IsCalendarButtonFocused = false;
            owner.FocusCalendarButton              = null;
        }

        calendarButton.CalendarLeftMouseButtonDown -= _ownerItem.HandleMonthCalendarButtonMouseDown;
        calendarButton.CalendarLeftMouseButtonUp   -= _ownerItem.HandleMonthCalendarButtonMouseUp;
        calendarButton.PointerEntered              -= _ownerItem.HandleMonthMouseEntered;
        calendarButton.ClearValue(CalendarButton.IsMotionEnabledProperty);
        calendarButton.Owner = null;
    }
}
