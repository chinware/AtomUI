namespace AtomUI.Desktop.Controls.Internal.Calendar.Lunar;

internal readonly record struct ChineseLunarDate(int Year, int Month, int Day, bool IsLeapMonth);

internal static class ChineseLunarCalendarEngine
{
    internal static readonly DateTime SupportedStart = new(1900, 1, 1);
    internal static readonly DateTime SupportedEnd = new(2100, 12, 31);

    private static readonly DateTime Epoch = new(1899, 2, 10);

    internal static ChineseLunarDate FromSolar(DateTime solarDate)
    {
        var date = solarDate.Date;
        EnsureSupportedSolarDate(date);

        var offset = (date - Epoch).Days;
        var year = ChineseLunarCalendarData.FirstLunarYear;
        while (year <= ChineseLunarCalendarData.LastLunarYear)
        {
            var yearDays = GetDaysInYear(year);
            if (offset < yearDays)
            {
                break;
            }

            offset -= yearDays;
            year++;
        }

        var leapMonth = GetLeapMonth(year);
        for (var month = 1; month <= 12; month++)
        {
            var monthDays = GetDaysInMonth(year, month, false);
            if (offset < monthDays)
            {
                return new ChineseLunarDate(year, month, offset + 1, false);
            }

            offset -= monthDays;
            if (leapMonth != month)
            {
                continue;
            }

            var leapDays = GetDaysInMonth(year, month, true);
            if (offset < leapDays)
            {
                return new ChineseLunarDate(year, month, offset + 1, true);
            }

            offset -= leapDays;
        }

        throw new InvalidOperationException("The embedded lunar year data does not cover the requested solar date.");
    }

    internal static DateTime ToSolar(ChineseLunarDate lunarDate)
    {
        ValidateLunarDate(lunarDate);

        var offset = 0;
        for (var year = ChineseLunarCalendarData.FirstLunarYear; year < lunarDate.Year; year++)
        {
            offset += GetDaysInYear(year);
        }

        var leapMonth = GetLeapMonth(lunarDate.Year);
        for (var month = 1; month < lunarDate.Month; month++)
        {
            offset += GetDaysInMonth(lunarDate.Year, month, false);
            if (leapMonth == month)
            {
                offset += GetDaysInMonth(lunarDate.Year, month, true);
            }
        }

        if (lunarDate.IsLeapMonth)
        {
            offset += GetDaysInMonth(lunarDate.Year, lunarDate.Month, false);
        }

        offset += lunarDate.Day - 1;
        var result = Epoch.AddDays(offset);
        EnsureSupportedSolarDate(result);
        return result;
    }

    internal static int GetDaysInMonth(int year, int month, bool isLeapMonth)
    {
        EnsureSupportedLunarYear(year);
        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month));
        }

        var info = GetYearInfo(year);
        if (isLeapMonth)
        {
            if ((info & 0xF) != month)
            {
                throw new ArgumentOutOfRangeException(nameof(isLeapMonth));
            }

            return (info & 0x10000) != 0 ? 30 : 29;
        }

        return (info & (0x10 << (12 - month))) != 0 ? 30 : 29;
    }

    internal static int GetLeapMonth(int year)
    {
        EnsureSupportedLunarYear(year);
        return GetYearInfo(year) & 0xF;
    }

    private static int GetDaysInYear(int year)
    {
        var info = GetYearInfo(year);
        var days = 12 * 29;
        for (var mask = 0x8000; mask >= 0x10; mask >>= 1)
        {
            if ((info & mask) != 0)
            {
                days++;
            }
        }

        if ((info & 0xF) != 0)
        {
            days += (info & 0x10000) != 0 ? 30 : 29;
        }

        return days;
    }

    private static int GetYearInfo(int year) =>
        ChineseLunarCalendarData.YearInfo[year - ChineseLunarCalendarData.FirstLunarYear];

    private static void ValidateLunarDate(ChineseLunarDate lunarDate)
    {
        EnsureSupportedLunarYear(lunarDate.Year);
        if (lunarDate.Month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(lunarDate));
        }

        var days = GetDaysInMonth(lunarDate.Year, lunarDate.Month, lunarDate.IsLeapMonth);
        if (lunarDate.Day < 1 || lunarDate.Day > days)
        {
            throw new ArgumentOutOfRangeException(nameof(lunarDate));
        }
    }

    private static void EnsureSupportedLunarYear(int year)
    {
        if (year is < ChineseLunarCalendarData.FirstLunarYear or > ChineseLunarCalendarData.LastLunarYear)
        {
            throw new ArgumentOutOfRangeException(nameof(year));
        }
    }

    private static void EnsureSupportedSolarDate(DateTime date)
    {
        if (date < SupportedStart || date > SupportedEnd)
        {
            throw new ArgumentOutOfRangeException(nameof(date));
        }
    }
}
