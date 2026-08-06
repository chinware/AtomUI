using System.Globalization;
using AtomUIGallery.ShowCases.Calendar;
using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUICalendar = AtomUI.Desktop.Controls.Calendar;
using AvaloniaWindow = Avalonia.Controls.Window;

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
        var enUs = ReadCalendarLocalization("en-US");
        var zhCn = ReadCalendarLocalization("zh-CN");
        var zhTw = ReadCalendarLocalization("zh-TW");

        source.ShouldContain("CalendarShowCaseLangResource CardTitle");
        source.ShouldContain("CalendarShowCaseLangResource CardDescription");
        source.ShouldContain("<Border Width=\"300\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        source.ShouldContain("BorderBrush=\"{atom:SharedTokenResource ColorBorderSecondary}\"");
        source.ShouldContain("BorderThickness=\"{atom:SharedTokenResource BorderThickness}\"");
        source.ShouldContain("CornerRadius=\"{atom:SharedTokenResource BorderRadiusLG}\"");

        enUs["CardTitle"].ShouldBe("Card");
        enUs["CardDescription"].ShouldBe("Nested inside a container element for rendering in limited space.");
        zhCn["CardTitle"].ShouldBe("卡片模式");
        zhCn["CardDescription"].ShouldBe("用于嵌套在空间有限的容器中。");
        zhTw["CardTitle"].ShouldBe("卡片模式");
        zhTw["CardDescription"].ShouldBe("用於嵌套在空間有限的容器中。");
    }

    [Fact]
    public void Calendar_CustomHeader_ShowCase_Uses_Public_HeaderTemplate_Contract()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");
        var enUs = ReadCalendarLocalization("en-US");
        var zhCn = ReadCalendarLocalization("zh-CN");
        var zhTw = ReadCalendarLocalization("zh-TW");

        var cardIndex = source.IndexOf("CalendarShowCaseLangResource CardTitle", StringComparison.Ordinal);
        var customHeaderIndex = source.IndexOf(
            "CalendarShowCaseLangResource CustomHeaderTitle",
            StringComparison.Ordinal);
        var showWeekIndex = source.IndexOf("CalendarShowCaseLangResource ShowWeekTitle", StringComparison.Ordinal);
        var nextShowCaseItemIndex = source.IndexOf(
            "<gallery:ShowCaseItem Title=",
            customHeaderIndex + 1,
            StringComparison.Ordinal);

        cardIndex.ShouldBeGreaterThanOrEqualTo(0);
        showWeekIndex.ShouldBeGreaterThan(cardIndex);
        customHeaderIndex.ShouldBeGreaterThan(showWeekIndex);
        nextShowCaseItemIndex.ShouldBe(-1);

        var customHeaderSource = source[customHeaderIndex..];
        customHeaderSource.ShouldContain("<atom:Calendar Value=\"{Binding SampleDate}\"");
        customHeaderSource.ShouldContain("Fullscreen=\"False\"");
        customHeaderSource.ShouldContain("<atom:Calendar.HeaderTemplate>");
        customHeaderSource.ShouldContain("<DataTemplate x:DataType=\"atom:CalendarHeaderContext\">");
        customHeaderSource.ShouldContain("CalendarShowCaseLangResource CustomHeaderContentTitle");
        customHeaderSource.ShouldContain("<atom:OptionButtonGroup");
        customHeaderSource.ShouldContain("SelectionChanged=\"OnCustomHeaderModeSelectionChanged\"");
        customHeaderSource.ShouldContain("Name=\"CustomHeaderYearSelect\"");
        customHeaderSource.ShouldContain("SelectionChanged=\"OnCustomHeaderYearSelectionChanged\"");
        customHeaderSource.ShouldContain("Name=\"CustomHeaderMonthSelect\"");
        customHeaderSource.ShouldContain("SelectionChanged=\"OnCustomHeaderMonthSelectionChanged\"");
        customHeaderSource.ShouldNotContain("PART_");
        customHeaderSource.ShouldNotContain("/template/");

        enUs["CustomHeaderTitle"].ShouldBe("Customize Header");
        enUs["CustomHeaderDescription"].ShouldBe("Customize Calendar header content.");
        zhCn["CustomHeaderTitle"].ShouldBe("自定义 Header");
        zhTw["CustomHeaderTitle"].ShouldBe("自訂 Header");
    }

    [Fact]
    public void Calendar_CustomHeader_ShowCase_Materializes_Without_Resource_Type_Errors()
    {
        AvaloniaTestApp.EnsureInitialized();
        var page = new CalendarShowCase
        {
            ViewModel = new CalendarViewModel(null!)
        };
        var window = new AvaloniaWindow
        {
            Width = 1200,
            Height = 900,
            Content = page
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            page.ApplyTemplate();
            page.Measure(new Size(1200, 900));
            page.Arrange(new Rect(0, 0, 1200, 900));
            Dispatcher.UIThread.RunJobs();

            var items = page.GetVisualDescendants().OfType<ShowCaseItem>().ToArray();
            items.Length.ShouldBeGreaterThan(4);
            var customHeaderItem = items[^1];

            customHeaderItem.MaterializeDeferredContent();
            customHeaderItem.Measure(new Size(1200, 900));
            customHeaderItem.Arrange(new Rect(customHeaderItem.DesiredSize));
            Dispatcher.UIThread.RunJobs();

            customHeaderItem.GetVisualDescendants()
                .OfType<AtomUICalendar>()
                .Single()
                .HeaderTemplate.ShouldNotBeNull();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Calendar_Lunar_ShowCases_Follow_Card_And_Precede_Selectable()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");
        var enUs = ReadCalendarLocalization("en-US");
        var zhCn = ReadCalendarLocalization("zh-CN");
        var zhTw = ReadCalendarLocalization("zh-TW");

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
        CountOccurrences(source, "HighlightWeekends=\"False\"").ShouldBe(2);
        source.ShouldNotContain("HighlightWeekends=\"True\"");
        lunarSource.ShouldContain("HolidayProvider=\"{Binding LunarCalendarHolidayProvider}\"");
        lunarCardSource.ShouldNotContain("HolidayProvider=\"{Binding LunarCalendarHolidayProvider}\"");
        lunarCardSource.ShouldContain("<Border MinWidth=\"300\"");
        lunarCardSource.ShouldNotContain("<Border Width=\"300\"");

        enUs["LunarCalendarTitle"].ShouldBe("Lunar Calendar");
        enUs["LunarCalendarCardTitle"].ShouldBe("Lunar Calendar Card");
        zhCn["LunarCalendarTitle"].ShouldBe("农历日历");
        zhCn["LunarCalendarCardTitle"].ShouldBe("农历卡片日历");
        zhTw["LunarCalendarTitle"].ShouldBe("農曆日曆");
        zhTw["LunarCalendarCardTitle"].ShouldBe("農曆卡片日曆");
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
    public void Calendar_Selectable_ShowCase_Follows_Card_And_Matches_AntDesign_State_Flow()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");
        var enUs = ReadCalendarLocalization("en-US");
        var zhCn = ReadCalendarLocalization("zh-CN");
        var zhTw = ReadCalendarLocalization("zh-TW");

        var cardIndex = source.IndexOf("CalendarShowCaseLangResource CardTitle", StringComparison.Ordinal);
        var selectableIndex = source.IndexOf("CalendarShowCaseLangResource SelectableCalendarTitle", StringComparison.Ordinal);
        cardIndex.ShouldBeGreaterThanOrEqualTo(0);
        selectableIndex.ShouldBeGreaterThan(cardIndex);
        source.ShouldContain("<atom:Alert Type=\"Info\"");
        source.ShouldContain("Message=\"{Binding SelectableCalendarSelectedText}\"");
        source.ShouldContain("Value=\"{Binding SelectableCalendarValue}\"");
        source.ShouldContain("Selected=\"OnSelectableCalendarSelected\"");

        enUs["SelectableCalendarTitle"].ShouldBe("Selectable Calendar");
        enUs["SelectableCalendarSelectedMessage"].ShouldBe("You selected date: {0:yyyy-MM-dd}");
        zhCn["SelectableCalendarTitle"].ShouldBe("可选择的日历");
        zhCn["SelectableCalendarSelectedMessage"].ShouldBe("你选择的日期：{0:yyyy-MM-dd}");
        zhTw["SelectableCalendarTitle"].ShouldBe("可選擇的日曆");
        zhTw["SelectableCalendarSelectedMessage"].ShouldBe("你選擇的日期：{0:yyyy-MM-dd}");
    }

    [Fact]
    public void Calendar_ShowWeek_ShowCase_Follows_Selectable_And_Precedes_CustomHeader()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");
        var enUs = ReadCalendarLocalization("en-US");
        var zhCn = ReadCalendarLocalization("zh-CN");
        var zhTw = ReadCalendarLocalization("zh-TW");

        var selectableIndex = source.IndexOf("CalendarShowCaseLangResource SelectableCalendarTitle", StringComparison.Ordinal);
        var showWeekIndex = source.IndexOf("CalendarShowCaseLangResource ShowWeekTitle", StringComparison.Ordinal);
        var customHeaderIndex = source.IndexOf("CalendarShowCaseLangResource CustomHeaderTitle", StringComparison.Ordinal);

        selectableIndex.ShouldBeGreaterThanOrEqualTo(0);
        showWeekIndex.ShouldBeGreaterThan(selectableIndex);
        customHeaderIndex.ShouldBeGreaterThan(showWeekIndex);

        var showWeekSource = source[showWeekIndex..customHeaderIndex];
        CountOccurrences(showWeekSource, "ShowWeek=\"True\"").ShouldBe(2);
        showWeekSource.ShouldContain("Fullscreen=\"True\"");
        showWeekSource.ShouldContain("MinWidth=\"640\"");
        showWeekSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        showWeekSource.ShouldContain("<Border Width=\"300\"");
        showWeekSource.ShouldContain("HorizontalAlignment=\"Left\"");
        showWeekSource.ShouldContain("BorderBrush=\"{atom:SharedTokenResource ColorBorderSecondary}\"");
        showWeekSource.ShouldContain("BorderThickness=\"{atom:SharedTokenResource BorderThickness}\"");
        showWeekSource.ShouldContain("CornerRadius=\"{atom:SharedTokenResource BorderRadiusLG}\"");
        showWeekSource.ShouldContain("Fullscreen=\"False\"");

        enUs["ShowWeekTitle"].ShouldBe("Show Week");
        enUs["ShowWeekDescription"].ShouldBe(
            "Show week numbers in full-screen and card calendars by setting ShowWeek to True.");
        zhCn["ShowWeekTitle"].ShouldBe("显示周数");
        zhCn["ShowWeekDescription"].ShouldBe(
            "通过将 ShowWeek 设置为 True，在完整模式和卡片模式日历中显示周数。");
        zhTw["ShowWeekTitle"].ShouldBe("顯示週數");
        zhTw["ShowWeekDescription"].ShouldBe(
            "透過將 ShowWeek 設定為 True，在完整模式和卡片模式日曆中顯示週數。");
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

    private static IReadOnlyDictionary<string, string> ReadCalendarLocalization(string languageTag)
    {
        return XliffTestDocument.Read(
            $"controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/{languageTag}.xlf");
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
