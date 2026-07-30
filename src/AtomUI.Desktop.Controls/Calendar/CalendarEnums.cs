namespace AtomUI.Desktop.Controls;

/// <summary>
/// Calendar 的公开显示模式。
/// </summary>
public enum CalendarMode
{
    /// <summary>显示一个月的日期网格。</summary>
    Month,

    /// <summary>显示一个年份的月份网格。</summary>
    Year
}

/// <summary>
/// 一次用户选择的来源。
/// </summary>
public enum CalendarSelectSource
{
    Year,
    Month,
    Date,
    Customize
}

/// <summary>
/// Cell 模板上下文的单元格类型。
/// </summary>
public enum CalendarCellType
{
    Date,
    Month
}
