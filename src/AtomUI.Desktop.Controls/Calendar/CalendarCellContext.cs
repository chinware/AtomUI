namespace AtomUI.Desktop.Controls;

/// <summary>
/// Cell 模板（<see cref="Calendar.CellTemplate"/> / <see cref="Calendar.FullCellTemplate"/>）的数据上下文。
/// </summary>
public sealed record CalendarCellContext(
    DateTime Value,
    DateTime Today,
    CalendarCellType CellType,
    string DisplayValue,
    bool IsToday,
    bool IsInView,
    bool IsSelected,
    bool IsDisabled);
