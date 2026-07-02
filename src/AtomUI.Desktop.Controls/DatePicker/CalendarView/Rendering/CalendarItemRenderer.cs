using AtomUI.Desktop.Controls.CalendarView.Models;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls.CalendarView.Rendering;

internal static class CalendarItemRenderer
{
    public static void RenderMonthPanel(Calendar? owner, Grid monthView, CalendarMonthPanelModel model)
    {
        if (model.WeekCells.Count > 0)
        {
            RenderWeekMonthPanel(owner, monthView, model);
            return;
        }

        RenderDayTitles(monthView, model.DayTitles, 0);
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

    private static void RenderWeekMonthPanel(Calendar? owner, Grid monthView, CalendarMonthPanelModel model)
    {
        RenderDayTitles(monthView, model.DayTitles, 1);

        if (monthView.Children.Count > 0)
        {
            monthView.Children[0].DataContext = string.Empty;
        }

        var dayButtons = monthView.Children.OfType<CalendarDayButton>().ToArray();
        var columnCount = Calendar.ColumnsPerMonth + 1;
        for (var row = 0; row < model.WeekCells.Count; row++)
        {
            var weekButtonIndex = row * columnCount;
            if (weekButtonIndex >= dayButtons.Length)
            {
                return;
            }

            RenderDayButton(owner, dayButtons[weekButtonIndex], model.WeekCells[row], Calendar.ColumnsPerWeekPanel + weekButtonIndex);

            for (var column = 0; column < Calendar.ColumnsPerMonth; column++)
            {
                var cellIndex   = row * Calendar.ColumnsPerMonth + column;
                var buttonIndex = weekButtonIndex + column + 1;
                if (buttonIndex >= dayButtons.Length || cellIndex >= model.Cells.Count)
                {
                    return;
                }

                RenderDayButton(owner, dayButtons[buttonIndex], model.Cells[cellIndex], Calendar.ColumnsPerWeekPanel + buttonIndex);
            }
        }

        var renderedCount = model.WeekCells.Count * columnCount;
        for (var i = renderedCount; i < dayButtons.Length; i++)
        {
            ClearDayButton(owner, dayButtons[i]);
        }
    }

    private static void RenderDayTitles(Grid monthView, IReadOnlyList<string> dayTitles, int columnOffset)
    {
        var titleCount = Math.Min(Calendar.ColumnsPerMonth, dayTitles.Count);
        for (var i = 0; i < titleCount && i + columnOffset < monthView.Children.Count; i++)
        {
            monthView.Children[i + columnOffset].DataContext = dayTitles[i];
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
        button.IsWeekNumber          = cell.IsWeekNumber;
        button.IsWeekSelectionStart  = cell.IsWeekSelectionStart;
        button.IsWeekSelectionMiddle = cell.IsWeekSelectionMiddle;
        button.IsWeekSelectionEnd    = cell.IsWeekSelectionEnd;
        button.IsWeekRangeStart      = cell.IsWeekRangeStart;
        button.IsWeekRangeMiddle     = cell.IsWeekRangeMiddle;
        button.IsWeekRangeEnd        = cell.IsWeekRangeEnd;
        button.IsWeekHoverStart      = cell.IsWeekHoverStart;
        button.IsWeekHoverMiddle     = cell.IsWeekHoverMiddle;
        button.IsWeekHoverEnd        = cell.IsWeekHoverEnd;

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
        button.IsWeekNumber          = false;
        button.IsWeekSelectionStart  = false;
        button.IsWeekSelectionMiddle = false;
        button.IsWeekSelectionEnd    = false;
        button.IsWeekRangeStart      = false;
        button.IsWeekRangeMiddle     = false;
        button.IsWeekRangeEnd        = false;
        button.IsWeekHoverStart      = false;
        button.IsWeekHoverMiddle     = false;
        button.IsWeekHoverEnd        = false;
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
        button.IsRangeStart            = cell.IsRangeStart;
        button.IsRangeEnd              = cell.IsRangeEnd;
        button.IsRangeMiddle           = cell.IsRangeMiddle;
        button.IsRangePreviewStart     = cell.IsRangePreviewStart;
        button.IsRangePreviewEnd       = cell.IsRangePreviewEnd;
        button.IsRangePreviewMiddle    = cell.IsRangePreviewMiddle;
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
        button.IsRangeStart         = false;
        button.IsRangeEnd           = false;
        button.IsRangeMiddle        = false;
        button.IsRangePreviewStart  = false;
        button.IsRangePreviewEnd    = false;
        button.IsRangePreviewMiddle = false;
        button.IsVisible  = false;
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
