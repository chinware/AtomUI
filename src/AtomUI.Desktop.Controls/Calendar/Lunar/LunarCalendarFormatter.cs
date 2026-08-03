namespace AtomUI.Desktop.Controls.Internal.Calendar.Lunar;

internal static class LunarCalendarFormatter
{
    internal static string FormatLunarDay(LunarCalendarDateInfo info)
    {
        return ChineseLunarNames.LunarDays[info.LunarDay - 1];
    }

    internal static string FormatSolarTerm(ChineseSolarTerm term) =>
        ChineseLunarNames.SolarTerms[(int)term];

    internal static string FormatFestival(ChineseTraditionalFestival festival) =>
        ChineseLunarNames.Festivals[(int)festival];

    internal static string FormatStemBranch(
        ChineseHeavenlyStem stem,
        ChineseEarthlyBranch branch)
    {
        return ChineseLunarNames.HeavenlyStems[(int)stem] + ChineseLunarNames.EarthlyBranches[(int)branch];
    }

    internal static string FormatZodiac(ChineseZodiac zodiac) =>
        ChineseLunarNames.Zodiacs[(int)zodiac];

    internal static string FormatLunarMonth(LunarCalendarMonthInfo month)
    {
        var value = ChineseLunarNames.LunarMonths[month.LunarMonth - 1];
        return (month.IsLeapMonth ? ChineseLunarNames.LeapMonthPrefix : string.Empty) +
               value +
               ChineseLunarNames.LunarMonthSuffix;
    }

    internal static string FormatMonthRange(IReadOnlyList<LunarCalendarMonthInfo> months)
    {
        if (months.Count == 0)
        {
            return string.Empty;
        }

        if (months.Count == 1)
        {
            return FormatLunarMonth(months[0]);
        }

        return $"{FormatLunarMonth(months[0])}-{FormatLunarMonth(months[^1])}";
    }
}
