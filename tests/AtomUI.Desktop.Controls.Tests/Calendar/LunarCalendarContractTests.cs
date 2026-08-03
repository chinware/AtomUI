using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Calendar;

public class LunarCalendarContractTests
{
    [Fact]
    public void PublicEnums_HaveTheDocumentedStableValues()
    {
        Enum.GetNames<ChineseSolarTerm>().ShouldBe([
            "MinorCold", "MajorCold", "StartOfSpring", "RainWater",
            "AwakeningOfInsects", "SpringEquinox", "PureBrightness", "GrainRain",
            "StartOfSummer", "GrainBuds", "GrainInEar", "SummerSolstice",
            "MinorHeat", "MajorHeat", "StartOfAutumn", "EndOfHeat",
            "WhiteDew", "AutumnEquinox", "ColdDew", "FrostDescent",
            "StartOfWinter", "MinorSnow", "MajorSnow", "WinterSolstice"
        ]);
        Enum.GetNames<ChineseTraditionalFestival>().ShouldBe([
            "SpringFestival", "LanternFestival", "DragonHeadFestival", "DragonBoatFestival",
            "QixiFestival", "ZhongyuanFestival", "MidAutumnFestival", "DoubleNinthFestival",
            "LabaFestival", "LunarNewYearsEve", "QingmingFestival"
        ]);
        Enum.GetNames<ChineseZodiac>().ShouldBe([
            "Rat", "Ox", "Tiger", "Rabbit", "Dragon", "Snake",
            "Horse", "Goat", "Monkey", "Rooster", "Dog", "Pig"
        ]);
        Enum.GetNames<ChineseHeavenlyStem>().ShouldBe([
            "Jia", "Yi", "Bing", "Ding", "Wu", "Ji", "Geng", "Xin", "Ren", "Gui"
        ]);
        Enum.GetNames<ChineseEarthlyBranch>().ShouldBe([
            "Zi", "Chou", "Yin", "Mao", "Chen", "Si", "Wu", "Wei", "Shen", "You", "Xu", "Hai"
        ]);
        Enum.GetNames<LunarCalendarHolidayKind>().ShouldBe(["Holiday", "Workday"]);
        Enum.GetNames<LunarCalendarSecondaryContentKind>().ShouldBe([
            "LunarDay", "SolarTerm", "TraditionalFestival", "Holiday", "Workday"
        ]);
    }

    [Fact]
    public void DateInfo_NormalizesSolarDateAndCopiesFestivalCollection()
    {
        var festivals = new List<ChineseTraditionalFestival>
        {
            ChineseTraditionalFestival.SpringFestival
        };

        var info = new LunarCalendarDateInfo(
            new DateTime(2024, 2, 10, 18, 30, 0),
            2024,
            1,
            1,
            false,
            ChineseHeavenlyStem.Jia,
            ChineseEarthlyBranch.Chen,
            ChineseZodiac.Dragon,
            null,
            festivals);
        festivals.Clear();

        info.SolarDate.ShouldBe(new DateTime(2024, 2, 10));
        info.TraditionalFestivals.ShouldBe([ChineseTraditionalFestival.SpringFestival]);
        Should.Throw<NotSupportedException>(() =>
            ((IList<ChineseTraditionalFestival>)info.TraditionalFestivals).Add(
                ChineseTraditionalFestival.LanternFestival));
    }

    [Fact]
    public void Holiday_NormalizesDate()
    {
        var holiday = new LunarCalendarHoliday(
            new DateTime(2026, 10, 1, 12, 0, 0),
            "National Day",
            LunarCalendarHolidayKind.Holiday);

        holiday.Date.ShouldBe(new DateTime(2026, 10, 1));
    }

    [Fact]
    public void InternalPanelData_UsesPanelDataVocabulary()
    {
        var assembly = typeof(LunarCalendar).Assembly;

        assembly.GetType("AtomUI.Desktop.Controls.Internal.Calendar.Lunar.LunarCalendarPanelData")
            .ShouldNotBeNull();
        assembly.GetType("AtomUI.Desktop.Controls.Internal.Calendar.Lunar.LunarCalendarPanelDataKey")
            .ShouldNotBeNull();
        assembly.GetType("AtomUI.Desktop.Controls.Internal.Calendar.Lunar.LunarCalendarSnapshot")
            .ShouldBeNull();
        assembly.GetType("AtomUI.Desktop.Controls.Internal.Calendar.Lunar.LunarCalendarSnapshotKey")
            .ShouldBeNull();
    }
}
