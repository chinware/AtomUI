using AtomUI.Desktop.Controls.Internal.Calendar.Lunar;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class LunarCalendarResolverTests
{
    [Theory]
    [InlineData(ChineseSolarTerm.MinorCold, 1, 6)]
    [InlineData(ChineseSolarTerm.MajorCold, 1, 20)]
    [InlineData(ChineseSolarTerm.StartOfSpring, 2, 4)]
    [InlineData(ChineseSolarTerm.RainWater, 2, 19)]
    [InlineData(ChineseSolarTerm.AwakeningOfInsects, 3, 5)]
    [InlineData(ChineseSolarTerm.SpringEquinox, 3, 20)]
    [InlineData(ChineseSolarTerm.PureBrightness, 4, 4)]
    [InlineData(ChineseSolarTerm.GrainRain, 4, 19)]
    [InlineData(ChineseSolarTerm.StartOfSummer, 5, 5)]
    [InlineData(ChineseSolarTerm.GrainBuds, 5, 20)]
    [InlineData(ChineseSolarTerm.GrainInEar, 6, 5)]
    [InlineData(ChineseSolarTerm.SummerSolstice, 6, 21)]
    [InlineData(ChineseSolarTerm.MinorHeat, 7, 6)]
    [InlineData(ChineseSolarTerm.MajorHeat, 7, 22)]
    [InlineData(ChineseSolarTerm.StartOfAutumn, 8, 7)]
    [InlineData(ChineseSolarTerm.EndOfHeat, 8, 22)]
    [InlineData(ChineseSolarTerm.WhiteDew, 9, 7)]
    [InlineData(ChineseSolarTerm.AutumnEquinox, 9, 22)]
    [InlineData(ChineseSolarTerm.ColdDew, 10, 8)]
    [InlineData(ChineseSolarTerm.FrostDescent, 10, 23)]
    [InlineData(ChineseSolarTerm.StartOfWinter, 11, 7)]
    [InlineData(ChineseSolarTerm.MinorSnow, 11, 22)]
    [InlineData(ChineseSolarTerm.MajorSnow, 12, 6)]
    [InlineData(ChineseSolarTerm.WinterSolstice, 12, 21)]
    public void SolarTermTable_ReturnsKnown2024Dates(ChineseSolarTerm term, int month, int day)
    {
        var date = SolarTermResolver.GetDate(2024, term);

        date.ShouldBe(new DateTime(2024, month, day));
        SolarTermResolver.GetSolarTerm(date).ShouldBe(term);
    }

    [Fact]
    public void SolarTermTable_ContainsEveryTermForEverySupportedYear()
    {
        for (var year = 1900; year <= 2100; year++)
        {
            foreach (var term in Enum.GetValues<ChineseSolarTerm>())
            {
                var date = SolarTermResolver.GetDate(year, term);
                date.Year.ShouldBe(year);
                date.Month.ShouldBe((int)term / 2 + 1);
                SolarTermResolver.GetSolarTerm(date).ShouldBe(term);
            }
        }
    }

    [Theory]
    [InlineData(2024, 1, 1, false, ChineseTraditionalFestival.SpringFestival)]
    [InlineData(2024, 1, 15, false, ChineseTraditionalFestival.LanternFestival)]
    [InlineData(2024, 2, 2, false, ChineseTraditionalFestival.DragonHeadFestival)]
    [InlineData(2024, 5, 5, false, ChineseTraditionalFestival.DragonBoatFestival)]
    [InlineData(2024, 7, 7, false, ChineseTraditionalFestival.QixiFestival)]
    [InlineData(2024, 7, 15, false, ChineseTraditionalFestival.ZhongyuanFestival)]
    [InlineData(2024, 8, 15, false, ChineseTraditionalFestival.MidAutumnFestival)]
    [InlineData(2024, 9, 9, false, ChineseTraditionalFestival.DoubleNinthFestival)]
    [InlineData(2024, 12, 8, false, ChineseTraditionalFestival.LabaFestival)]
    public void TraditionalFestivalResolver_ReturnsFixedLunarFestivals(
        int year,
        int month,
        int day,
        bool isLeap,
        ChineseTraditionalFestival expected)
    {
        TraditionalFestivalResolver.Resolve(new ChineseLunarDate(year, month, day, isLeap), null)
            .ShouldContain(expected);
    }

    [Fact]
    public void TraditionalFestivalResolver_DoesNotRepeatFixedFestivalsInLeapMonth()
    {
        TraditionalFestivalResolver.Resolve(new ChineseLunarDate(2023, 2, 2, true), null)
            .ShouldBeEmpty();
    }

    [Fact]
    public void TraditionalFestivalResolver_DetectsDynamicNewYearsEveAndQingming()
    {
        var newYearsEve = ChineseLunarCalendarEngine.FromSolar(new DateTime(2025, 1, 28));
        TraditionalFestivalResolver.Resolve(newYearsEve, null)
            .ShouldContain(ChineseTraditionalFestival.LunarNewYearsEve);

        var qingming = ChineseLunarCalendarEngine.FromSolar(new DateTime(2024, 4, 4));
        TraditionalFestivalResolver.Resolve(qingming, ChineseSolarTerm.PureBrightness)
            .ShouldBe([ChineseTraditionalFestival.QingmingFestival]);
    }

    [Fact]
    public void DateProjector_CreatesStructuredDateInfo()
    {
        var info = LunarCalendarDateProjector.Create(new DateTime(2024, 2, 10, 23, 0, 0));

        info.SolarDate.ShouldBe(new DateTime(2024, 2, 10));
        info.LunarYear.ShouldBe(2024);
        info.LunarMonth.ShouldBe(1);
        info.LunarDay.ShouldBe(1);
        info.HeavenlyStem.ShouldBe(ChineseHeavenlyStem.Jia);
        info.EarthlyBranch.ShouldBe(ChineseEarthlyBranch.Chen);
        info.Zodiac.ShouldBe(ChineseZodiac.Dragon);
        info.TraditionalFestivals.ShouldBe([ChineseTraditionalFestival.SpringFestival]);
    }

    [Fact]
    public void MonthIntersectionResolver_CollectsAllIntersectingLunarMonthsInOrder()
    {
        LunarCalendarMonthIntersectionResolver.Resolve(2023, 3).ShouldBe([
            new LunarCalendarMonthInfo(2023, 2, false),
            new LunarCalendarMonthInfo(2023, 2, true)
        ]);
        LunarCalendarMonthIntersectionResolver.Resolve(2024, 2).ShouldBe([
            new LunarCalendarMonthInfo(2023, 12, false),
            new LunarCalendarMonthInfo(2024, 1, false)
        ]);
    }

    [Fact]
    public void Formatter_UsesFixedChineseLunarTerms()
    {
        var info = LunarCalendarDateProjector.Create(new DateTime(2024, 2, 10));

        LunarCalendarFormatter.FormatLunarDay(info).ShouldBe("初一");
        LunarCalendarFormatter.FormatFestival(ChineseTraditionalFestival.SpringFestival).ShouldBe("春节");
        LunarCalendarFormatter.FormatSolarTerm(ChineseSolarTerm.PureBrightness).ShouldBe("清明");
        LunarCalendarFormatter.FormatStemBranch(info.HeavenlyStem, info.EarthlyBranch).ShouldBe("甲辰");
        LunarCalendarFormatter.FormatZodiac(info.Zodiac).ShouldBe("龙");
    }
}
