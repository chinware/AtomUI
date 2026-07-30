namespace AtomUI.Desktop.Controls;

public sealed record CalendarCellContext(
    DateTime Value,
    DateTime Today,
    CalendarCellType CellType,
    string DisplayValue,
    bool IsToday,
    bool IsInView,
    bool IsSelected,
    bool IsDisabled);
