namespace AtomUI.Desktop.Controls.Internal.Calendar.Lunar;

internal static class TraditionalFestivalResolver
{
    internal static IReadOnlyList<ChineseTraditionalFestival> Resolve(
        ChineseLunarDate lunarDate,
        ChineseSolarTerm? solarTerm)
    {
        List<ChineseTraditionalFestival>? festivals = null;
        if (!lunarDate.IsLeapMonth && TryResolveFixedFestival(lunarDate.Month, lunarDate.Day, out var fixedFestival))
        {
            festivals = [fixedFestival];
        }

        if (!lunarDate.IsLeapMonth &&
            lunarDate.Month == 12 &&
            lunarDate.Day == ChineseLunarCalendarEngine.GetDaysInMonth(lunarDate.Year, 12, false))
        {
            festivals ??= [];
            festivals.Add(ChineseTraditionalFestival.LunarNewYearsEve);
        }

        if (solarTerm == ChineseSolarTerm.PureBrightness)
        {
            festivals ??= [];
            festivals.Add(ChineseTraditionalFestival.QingmingFestival);
        }

        return festivals is null
            ? Array.Empty<ChineseTraditionalFestival>()
            : festivals.AsReadOnly();
    }

    private static bool TryResolveFixedFestival(
        int month,
        int day,
        out ChineseTraditionalFestival festival)
    {
        festival = (month, day) switch
        {
            (1, 1) => ChineseTraditionalFestival.SpringFestival,
            (1, 15) => ChineseTraditionalFestival.LanternFestival,
            (2, 2) => ChineseTraditionalFestival.DragonHeadFestival,
            (5, 5) => ChineseTraditionalFestival.DragonBoatFestival,
            (7, 7) => ChineseTraditionalFestival.QixiFestival,
            (7, 15) => ChineseTraditionalFestival.ZhongyuanFestival,
            (8, 15) => ChineseTraditionalFestival.MidAutumnFestival,
            (9, 9) => ChineseTraditionalFestival.DoubleNinthFestival,
            (12, 8) => ChineseTraditionalFestival.LabaFestival,
            _ => default
        };
        return (month, day) is (1, 1) or (1, 15) or (2, 2) or (5, 5) or
            (7, 7) or (7, 15) or (8, 15) or (9, 9) or (12, 8);
    }
}
