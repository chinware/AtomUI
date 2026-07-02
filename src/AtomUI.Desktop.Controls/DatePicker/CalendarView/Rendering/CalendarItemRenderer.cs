using AtomUI.Desktop.Controls.CalendarView.Models;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls.CalendarView.Rendering;

internal static class CalendarItemRenderer
{
    public static void RenderMonthPanel(Calendar? owner, Grid monthView, CalendarMonthPanelModel model)
    {
        RenderDayTitles(monthView, model.DayTitles);

        var dayButtons = monthView.Children.OfType<CalendarDayButton>().ToArray();
        var cellCount  = Math.Min(dayButtons.Length, model.Cells.Count);
        for (var i = 0; i < cellCount; i++)
        {
            RenderDayButton(owner, dayButtons[i], model.Cells[i], Calendar.ColumnsPerMonth + i);
        }

        for (var i = cellCount; i < dayButtons.Length; i++)
        {
            ClearDayButton(owner, dayButtons[i]);
        }
    }

    public static void RenderYearPanel(Calendar? owner, Grid yearView, CalendarYearPanelModel model)
    {
        RenderCalendarButtons(owner, yearView, model.Months);
    }

    public static void RenderQuarterPanel(Calendar? owner, Grid yearView, CalendarQuarterPanelModel model)
    {
        RenderCalendarButtons(owner, yearView, model.Quarters);
    }

    public static void RenderDecadePanel(Calendar? owner, Grid yearView, CalendarDecadePanelModel model)
    {
        RenderCalendarButtons(owner, yearView, model.Years);
    }

    private static void RenderDayTitles(Grid monthView, IReadOnlyList<string> dayTitles)
    {
        var titleCount = Math.Min(Calendar.ColumnsPerMonth, dayTitles.Count);
        for (var i = 0; i < titleCount && i < monthView.Children.Count; i++)
        {
            monthView.Children[i].DataContext = dayTitles[i];
        }
    }

    private static void RenderDayButton(Calendar? owner, CalendarDayButton button, CalendarCellState cell, int index)
    {
        button.Index         = index;
        button.Content       = cell.Text;
        button.DataContext   = cell.Date;
        button.Opacity       = cell.IsHidden ? 0 : 1;
        button.IsEnabled     = !cell.IsDisabled;
        button.IsBlackout    = cell.IsBlackout;
        button.IsInactive    = cell.IsInactive;
        button.IsToday       = cell.IsToday;
        button.IsSelected    = cell.IsSelected;
        button.IsRangeStart  = cell.IsRangeStart;
        button.IsRangeEnd    = cell.IsRangeEnd;
        button.IsRangeMiddle = cell.IsRangeMiddle;
        button.IsRangePreviewStart  = cell.IsRangePreviewStart;
        button.IsRangePreviewEnd    = cell.IsRangePreviewEnd;
        button.IsRangePreviewMiddle = cell.IsRangePreviewMiddle;

        ApplyFocus(owner, button, cell.IsFocused);
    }

    private static void ClearDayButton(Calendar? owner, CalendarDayButton button)
    {
        button.Content       = string.Empty;
        button.DataContext   = null;
        button.Opacity       = 0;
        button.IsEnabled     = false;
        button.IsBlackout    = false;
        button.IsInactive    = false;
        button.IsToday       = false;
        button.IsSelected    = false;
        button.IsRangeStart  = false;
        button.IsRangeEnd    = false;
        button.IsRangeMiddle = false;
        button.IsRangePreviewStart  = false;
        button.IsRangePreviewEnd    = false;
        button.IsRangePreviewMiddle = false;
        ApplyFocus(owner, button, false);
    }

    private static void RenderCalendarButtons(
        Calendar? owner,
        Grid yearView,
        IReadOnlyList<CalendarCellState> cells)
    {
        var buttons   = yearView.Children.OfType<CalendarButton>().ToArray();
        var cellCount = Math.Min(buttons.Length, cells.Count);
        for (var i = 0; i < cellCount; i++)
        {
            RenderCalendarButton(owner, buttons[i], cells[i]);
        }

        for (var i = cellCount; i < buttons.Length; i++)
        {
            ClearCalendarButton(owner, buttons[i]);
        }
    }

    private static void RenderCalendarButton(Calendar? owner, CalendarButton button, CalendarCellState cell)
    {
        button.Content                 = cell.Text;
        button.DataContext             = cell.IsHidden ? null : cell.Date;
        button.Opacity                 = cell.IsHidden ? 0 : 1;
        button.IsEnabled               = !cell.IsDisabled;
        button.IsInactive              = cell.IsInactive;
        button.IsSelected              = cell.IsSelected;
        button.IsVisible               = true;
        ApplyCalendarButtonFocus(owner, button, cell.IsFocused);
    }

    private static void ClearCalendarButton(Calendar? owner, CalendarButton button)
    {
        button.Content    = string.Empty;
        button.DataContext = null;
        button.Opacity    = 0;
        button.IsEnabled  = false;
        button.IsInactive = false;
        button.IsSelected = false;
        ApplyCalendarButtonFocus(owner, button, false);
    }

    private static void ApplyFocus(Calendar? owner, CalendarDayButton button, bool isFocused)
    {
        if (owner?.FocusButton == button && !isFocused)
        {
            button.IsCurrent = false;
            owner.FocusButton = null;
        }

        if (owner is null || !isFocused)
        {
            button.IsCurrent = false;
            return;
        }

        if (owner.FocusButton is not null && owner.FocusButton != button)
        {
            owner.FocusButton.IsCurrent = false;
        }

        owner.FocusButton = button;
        button.IsCurrent  = owner.HasFocusInternal;
    }

    private static void ApplyCalendarButtonFocus(Calendar? owner, CalendarButton button, bool isFocused)
    {
        if (owner?.FocusCalendarButton == button && !isFocused)
        {
            button.IsCalendarButtonFocused = false;
            owner.FocusCalendarButton      = null;
        }

        if (owner is null || !isFocused)
        {
            button.IsCalendarButtonFocused = false;
            return;
        }

        if (owner.FocusCalendarButton is not null && owner.FocusCalendarButton != button)
        {
            owner.FocusCalendarButton.IsCalendarButtonFocused = false;
        }

        owner.FocusCalendarButton      = button;
        button.IsCalendarButtonFocused = owner.HasFocusInternal;
    }
}
