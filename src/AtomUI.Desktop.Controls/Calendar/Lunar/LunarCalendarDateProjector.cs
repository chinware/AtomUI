namespace AtomUI.Desktop.Controls.Internal.Calendar.Lunar;

internal static class LunarCalendarDateProjector
{
    internal static LunarCalendarDateInfo Create(DateTime solarDate)
    {
        var date = solarDate.Date;
        var lunarDate = ChineseLunarCalendarEngine.FromSolar(date);
        var solarTerm = SolarTermResolver.GetSolarTerm(date);
        var festivals = TraditionalFestivalResolver.Resolve(lunarDate, solarTerm);
        var stemIndex = PositiveModulo(lunarDate.Year - 4, 10);
        var branchIndex = PositiveModulo(lunarDate.Year - 4, 12);

        return new LunarCalendarDateInfo(
            date,
            lunarDate.Year,
            lunarDate.Month,
            lunarDate.Day,
            lunarDate.IsLeapMonth,
            (ChineseHeavenlyStem)stemIndex,
            (ChineseEarthlyBranch)branchIndex,
            (ChineseZodiac)branchIndex,
            solarTerm,
            festivals);
    }

    private static int PositiveModulo(int value, int divisor)
    {
        var result = value % divisor;
        return result < 0 ? result + divisor : result;
    }
}
