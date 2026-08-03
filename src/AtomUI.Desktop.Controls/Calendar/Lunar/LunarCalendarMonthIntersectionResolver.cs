namespace AtomUI.Desktop.Controls.Internal.Calendar.Lunar;

internal static class LunarCalendarMonthIntersectionResolver
{
    internal static IReadOnlyList<LunarCalendarMonthInfo> Resolve(int year, int month)
    {
        if (year is < 1900 or > 2100)
        {
            throw new ArgumentOutOfRangeException(nameof(year));
        }

        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        var result = new List<LunarCalendarMonthInfo>(3);
        var end = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        for (var date = new DateTime(year, month, 1); date <= end; date = date.AddDays(1))
        {
            var lunarDate = ChineseLunarCalendarEngine.FromSolar(date);
            var current = new LunarCalendarMonthInfo(
                lunarDate.Year,
                lunarDate.Month,
                lunarDate.IsLeapMonth);
            if (result.Count == 0 || result[^1] != current)
            {
                result.Add(current);
            }
        }

        return result.AsReadOnly();
    }
}
