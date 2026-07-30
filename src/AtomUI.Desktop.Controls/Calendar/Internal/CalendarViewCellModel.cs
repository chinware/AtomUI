namespace AtomUI.Desktop.Controls.Internal.Calendar;

internal enum CalendarViewCellKind
{
    Date,
    Month,
    Week
}

internal sealed record CalendarViewCellModel(
    DateTime Value,
    CalendarViewCellKind Kind,
    string DisplayText,
    bool IsToday,
    bool IsInView,
    bool IsSelected,
    bool IsDisabled,
    bool IsFocusable);
