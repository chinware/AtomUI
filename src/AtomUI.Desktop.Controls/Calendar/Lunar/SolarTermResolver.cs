namespace AtomUI.Desktop.Controls.Internal.Calendar.Lunar;

internal static class SolarTermResolver
{
    internal static DateTime GetDate(int year, ChineseSolarTerm term)
    {
        if (year is < SolarTermData.FirstYear or > SolarTermData.LastYear)
        {
            throw new ArgumentOutOfRangeException(nameof(year));
        }

        var termIndex = (int)term;
        if (termIndex is < 0 or >= 24)
        {
            throw new ArgumentOutOfRangeException(nameof(term));
        }

        var packed = SolarTermData.PackedDays[(year - SolarTermData.FirstYear) * 2 + termIndex / 12];
        var day = (int)((packed >> ((termIndex % 12) * 5)) & 0x1FUL);
        return new DateTime(year, termIndex / 2 + 1, day);
    }

    internal static ChineseSolarTerm? GetSolarTerm(DateTime solarDate)
    {
        var date = solarDate.Date;
        if (date < ChineseLunarCalendarEngine.SupportedStart ||
            date > ChineseLunarCalendarEngine.SupportedEnd)
        {
            return null;
        }

        var firstTerm = (date.Month - 1) * 2;
        for (var offset = 0; offset < 2; offset++)
        {
            var term = (ChineseSolarTerm)(firstTerm + offset);
            if (GetDate(date.Year, term) == date)
            {
                return term;
            }
        }

        return null;
    }
}
