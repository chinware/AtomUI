namespace AtomUI.Desktop.Controls.CalendarView.Models;

internal readonly record struct CalendarCellState(
    DateTime Date,
    string Text,
    bool IsToday,
    bool IsBlackout,
    bool IsDisabled,
    bool IsInactive,
    bool IsSelected,
    bool IsRangeStart,
    bool IsRangeEnd,
    bool IsRangeMiddle,
    bool IsFocused,
    bool IsHidden,
    bool IsRangePreviewStart = false,
    bool IsRangePreviewEnd = false,
    bool IsRangePreviewMiddle = false);
