using System.Globalization;
using AtomUIGallery.ShowCases.Calendar;
using AtomUI.Desktop.Controls;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class CalendarShowCasePageTests
{
    [Fact]
    public void Calendar_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");

        source.ShouldContain("CalendarShowCaseLangResource PageSubtitle");
        source.ShouldContain("CalendarShowCaseLangResource PageDescription");
        source.ShouldNotContain("CalendarShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("CalendarShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("CalendarShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("CalendarShowCaseLangResource ComponentCategory");
        source.ShouldContain("CalendarShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("CalendarShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("CalendarShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("CalendarShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("MaxColumns=\"1\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:CalendarShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("CalendarShowCaseLangResource BasicTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Calendar_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/CalendarShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractCalendarExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    [Fact]
    public void Calendar_Card_ShowCase_UsesExternalContainerAndReferenceCopy()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");
        var enUs = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/zh_TW.cs");

        source.ShouldContain("CalendarShowCaseLangResource CardTitle");
        source.ShouldContain("CalendarShowCaseLangResource CardDescription");
        source.ShouldContain("<Border Width=\"300\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        source.ShouldContain("BorderBrush=\"{atom:SharedTokenResource ColorBorderSecondary}\"");
        source.ShouldContain("BorderThickness=\"{atom:SharedTokenResource BorderThickness}\"");
        source.ShouldContain("CornerRadius=\"{atom:SharedTokenResource BorderRadiusLG}\"");

        enUs.ShouldContain("public const string CardTitle = \"Card\";");
        enUs.ShouldContain("public const string CardDescription = \"Nested inside a container element for rendering in limited space.\";");
        zhCn.ShouldContain("public const string CardTitle = \"卡片模式\";");
        zhCn.ShouldContain("public const string CardDescription = \"用于嵌套在空间有限的容器中。\";");
        zhTw.ShouldContain("public const string CardTitle = \"卡片模式\";");
        zhTw.ShouldContain("public const string CardDescription = \"用於嵌套在空間有限的容器中。\";");
    }

    [Fact]
    public void Calendar_Lunar_ShowCases_Follow_Card_And_Precede_Selectable()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");
        var enUs = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/zh_TW.cs");

        var cardIndex = source.IndexOf("CalendarShowCaseLangResource CardTitle", StringComparison.Ordinal);
        var lunarIndex = source.IndexOf("CalendarShowCaseLangResource LunarCalendarTitle", StringComparison.Ordinal);
        var lunarCardIndex = source.IndexOf("CalendarShowCaseLangResource LunarCalendarCardTitle", StringComparison.Ordinal);
        var selectableIndex = source.IndexOf("CalendarShowCaseLangResource SelectableCalendarTitle", StringComparison.Ordinal);

        cardIndex.ShouldBeGreaterThanOrEqualTo(0);
        lunarIndex.ShouldBeGreaterThan(cardIndex);
        lunarCardIndex.ShouldBeGreaterThan(lunarIndex);
        selectableIndex.ShouldBeGreaterThan(lunarCardIndex);
        var lunarSource = source[lunarIndex..lunarCardIndex];
        var lunarCardSource = source[lunarCardIndex..selectableIndex];
        source.ShouldContain("<atom:LunarCalendar Value=\"{Binding LunarCalendarSampleDate}\"");
        source.ShouldContain("Fullscreen=\"False\"");
        source.ShouldContain("ShowSolarTerms=\"True\"");
        source.ShouldContain("ShowTraditionalFestivals=\"True\"");
        lunarSource.ShouldContain("HolidayProvider=\"{Binding LunarCalendarHolidayProvider}\"");
        lunarCardSource.ShouldNotContain("HolidayProvider=\"{Binding LunarCalendarHolidayProvider}\"");
        lunarCardSource.ShouldContain("<Border MinWidth=\"300\"");
        lunarCardSource.ShouldNotContain("<Border Width=\"300\"");

        enUs.ShouldContain("public const string LunarCalendarTitle = \"Lunar Calendar\";");
        enUs.ShouldContain("public const string LunarCalendarCardTitle = \"Lunar Calendar Card\";");
        zhCn.ShouldContain("public const string LunarCalendarTitle = \"农历日历\";");
        zhCn.ShouldContain("public const string LunarCalendarCardTitle = \"农历卡片日历\";");
        zhTw.ShouldContain("public const string LunarCalendarTitle = \"農曆日曆\";");
        zhTw.ShouldContain("public const string LunarCalendarCardTitle = \"農曆卡片日曆\";");
    }

    [Fact]
    public void Calendar_Lunar_ViewModel_Provides_Static_Holiday_Data()
    {
        var viewModel = new CalendarViewModel(null!);

        viewModel.LunarCalendarSampleDate.ShouldBe(new DateTime(2024, 2, 10));
        viewModel.LunarCalendarHolidayProvider.ShouldNotBeNull();
        viewModel.LunarCalendarHolidayProvider.TryGetHolidays(
            new CalendarDateRange(new DateTime(2024, 2, 1), new DateTime(2024, 2, 29)),
            CultureInfo.GetCultureInfo("en-US"),
            out var holidays).ShouldBeTrue();
        holidays.ShouldContain(item =>
            item.Date == new DateTime(2024, 2, 9) &&
            item.Name == "除夕" &&
            item.Kind == LunarCalendarHolidayKind.Holiday);
        holidays.ShouldContain(item =>
            item.Date == new DateTime(2024, 2, 18) &&
            item.Name == "调休工作日" &&
            item.Kind == LunarCalendarHolidayKind.Workday);
    }

    [Fact]
    public void Calendar_Selectable_ShowCase_Follows_Card_IsLast_And_Matches_AntDesign_State_Flow()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");
        var enUs = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/zh_TW.cs");

        var cardIndex = source.IndexOf("CalendarShowCaseLangResource CardTitle", StringComparison.Ordinal);
        var selectableIndex = source.IndexOf("CalendarShowCaseLangResource SelectableCalendarTitle", StringComparison.Ordinal);
        var nextShowCaseItemIndex = source.IndexOf(
            "<gallery:ShowCaseItem Title=",
            selectableIndex + 1,
            StringComparison.Ordinal);

        cardIndex.ShouldBeGreaterThanOrEqualTo(0);
        selectableIndex.ShouldBeGreaterThan(cardIndex);
        nextShowCaseItemIndex.ShouldBe(-1);
        source.ShouldContain("<atom:Alert Type=\"Info\"");
        source.ShouldContain("Message=\"{Binding SelectableCalendarSelectedText}\"");
        source.ShouldContain("Value=\"{Binding SelectableCalendarValue}\"");
        source.ShouldContain("Selected=\"OnSelectableCalendarSelected\"");

        enUs.ShouldContain("public const string SelectableCalendarTitle = \"Selectable Calendar\";");
        enUs.ShouldContain("public const string SelectableCalendarSelectedMessage = \"You selected date: {0:yyyy-MM-dd}\";");
        zhCn.ShouldContain("public const string SelectableCalendarTitle = \"可选择的日历\";");
        zhCn.ShouldContain("public const string SelectableCalendarSelectedMessage = \"你选择的日期：{0:yyyy-MM-dd}\";");
        zhTw.ShouldContain("public const string SelectableCalendarTitle = \"可選擇的日曆\";");
        zhTw.ShouldContain("public const string SelectableCalendarSelectedMessage = \"你選擇的日期：{0:yyyy-MM-dd}\";");
    }

    [Fact]
    public void Calendar_Selectable_ViewModel_Separates_Panel_Value_From_Selected_Value()
    {
        var viewModel = new CalendarViewModel(null!);

        var initialDate = new DateTime(2017, 1, 25);
        viewModel.SelectableCalendarValue.ShouldBe(initialDate);
        viewModel.SelectableCalendarSelectedValue.ShouldBe(initialDate);
        viewModel.SelectableCalendarSelectedText.ShouldBe("You selected date: 2017-01-25");

        viewModel.SelectableCalendarValue = new DateTime(2017, 2, 25);
        viewModel.SelectableCalendarSelectedValue.ShouldBe(initialDate);
        viewModel.SelectableCalendarSelectedText.ShouldBe("You selected date: 2017-01-25");

        var selectedDate = new DateTime(2017, 3, 12);
        viewModel.SelectSelectableCalendarDate(selectedDate);

        viewModel.SelectableCalendarValue.ShouldBe(selectedDate);
        viewModel.SelectableCalendarSelectedValue.ShouldBe(selectedDate);
        viewModel.SelectableCalendarSelectedText.ShouldBe("You selected date: 2017-03-12");
    }

    private static string ExtractCalendarExampleItems(string source)
    {
        const string firstItemMarker = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..panelCloseStart];
    }

    private static string NormalizeMarkup(string source)
    {
        return ShowCaseSnapshotMarkup.Normalize(source);
    }

    private static int CountOccurrences(string source, string value)
    {
        var count = 0;
        var startIndex = 0;
        while (true)
        {
            var matchIndex = source.IndexOf(value, startIndex, StringComparison.Ordinal);
            if (matchIndex < 0)
            {
                return count;
            }

            count++;
            startIndex = matchIndex + value.Length;
        }
    }

    private static string ReadRepoFile(string relativePath)
    {
        var path = GetRepoFile(relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
