using System.Globalization;

namespace AtomUI.Desktop.Controls;

public interface ILunarCalendarHolidayProvider
{
    bool TryGetHolidays(
        CalendarDateRange visibleRange,
        CultureInfo culture,
        out IReadOnlyList<LunarCalendarHoliday> holidays);
}

public sealed record LunarCalendarHoliday
{
    public LunarCalendarHoliday(DateTime date, string name, LunarCalendarHolidayKind kind)
    {
        Date = date.Date;
        Name = name ?? string.Empty;
        Kind = kind;
    }

    public DateTime Date { get; }
    public string Name { get; }
    public LunarCalendarHolidayKind Kind { get; }
}
