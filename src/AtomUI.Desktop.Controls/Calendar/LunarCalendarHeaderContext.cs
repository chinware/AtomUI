using System.Windows.Input;

namespace AtomUI.Desktop.Controls;

public sealed class LunarCalendarHeaderContext : CalendarHeaderContext
{
    public LunarCalendarHeaderContext(
        DateTime value,
        CalendarMode mode,
        ICommand changeValueCommand,
        ICommand changeModeCommand,
        LunarCalendarDateInfo lunarDateInfo,
        CalendarDateRange supportedRange,
        string lunarYearText,
        string lunarMonthRangeText)
        : base(value, mode, changeValueCommand, changeModeCommand)
    {
        LunarDateInfo = lunarDateInfo;
        SupportedRange = supportedRange;
        LunarYearText = lunarYearText;
        LunarMonthRangeText = lunarMonthRangeText;
    }

    public LunarCalendarDateInfo LunarDateInfo { get; }
    public CalendarDateRange SupportedRange { get; }
    public string LunarYearText { get; }
    public string LunarMonthRangeText { get; }
}
