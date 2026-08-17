using System.Collections;
using System.Globalization;
using System.Reflection;
using AtomUIGallery.ShowCases.Calendar;
using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
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
        source.ShouldContain("<gallery:GalleryShowCaseHost");
        source.ShouldNotContain("<gallery:GalleryStickyTabsHost");
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
        source.ShouldContain("CalendarShowCaseLangResource SemanticPartStyleTitle");
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
    public void Calendar_ShowCase_Declares_The_Semantic_Previews_And_Style_Example()
    {
        var source = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Views/CalendarShowCase.axaml");
        var english = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/DataDisplay/Calendar/Localization/en-US.xlf");
        var semanticSource = ExtractShowCaseItem(source, "calendar-semantic-part");

        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("Name=\"CalendarSemanticPreview\"");
        source.ShouldContain("SemanticOwner=\"{Binding #CalendarSemanticOwner}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:Calendar}\"");
        source.ShouldContain("<atom:Calendar Name=\"CalendarSemanticOwner\" />");
        CountOccurrences(source, "<gallery:SemanticPartDescription").ShouldBe(6);
        foreach (var path in new[] { "root", "header", "body", "content", "item", "itemContent" })
        {
            source.ShouldContain($"Path=\"{path}\"");
        }

        semanticSource.ShouldContain("SourceKey=\"calendar-semantic-part\"");
        semanticSource.ShouldContain("BadgeText=\"v6.1.3\"");
        semanticSource.ShouldContain("CalendarShowCaseLangResource SemanticPartStyleTitle");
        semanticSource.ShouldContain("CalendarShowCaseLangResource SemanticPartStyleDescription");
        semanticSource.ShouldContain("Selector=\"atom|Calendar.semantic-object\"");
        semanticSource.ShouldContain("Selector=\"atom|Calendar.semantic-function\"");
        semanticSource.ShouldContain("<Setter Property=\"Fullscreen\" Value=\"False\" />");
        semanticSource.ShouldContain("<Setter Property=\"Width\" Value=\"600\" />");
        semanticSource.ShouldContain("<Setter Property=\"CornerRadius\" Value=\"8\" />");
        CountOccurrences(semanticSource, "<Setter Property=\"Padding\" Value=\"10\" />").ShouldBe(2);
        semanticSource.ShouldContain("<Setter Property=\"Background\" Value=\"{atom:SharedTokenResource ColorPrimaryBg}\" />");
        semanticSource.ShouldContain("<Setter Property=\"BorderThickness\" Value=\"2\" />");
        semanticSource.ShouldContain("<Setter Property=\"BorderBrush\" Value=\"#BDE3C3\" />");
        semanticSource.ShouldContain("<Setter Property=\"CornerRadius\" Value=\"10\" />");
        semanticSource.ShouldContain("<Setter Property=\"Background\" Value=\"#4DBDE3C3\" />");
        CountOccurrences(semanticSource, "<atom:Calendar ").ShouldBe(2);
        semanticSource.ShouldNotContain("/template/");
        semanticSource.ShouldNotContain("classNames", Case.Insensitive);
        semanticSource.ShouldNotContain("semantic dom", Case.Insensitive);

        english.ShouldContain("Custom Semantic Part styling");
        english.ShouldContain(
            "Root element containing background, border, border-radius and overall layout structure of the calendar component");
        english.ShouldContain(
            "Header element with layout and style control for year selector, month selector and mode switcher");
        english.ShouldContain(
            "Body element with padding and layout control for the calendar table that contains the calendar grid");
        english.ShouldContain(
            "Content element with width, height and table styling control for the calendar table");
        english.ShouldContain(
            "Item element with background, border, hover state, selected state and other interactive styles for calendar cells");
        english.ShouldContain(
            "Item content element with height, overflow and other style control for custom content area inside calendar cells");
        english.ShouldNotContain("classNames", Case.Insensitive);
        english.ShouldNotContain("semantic dom", Case.Insensitive);
    }

    [Fact]
    public void Calendar_Semantic_Preview_Is_Materialized_Only_After_The_Tab_Is_Selected()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new CalendarShowCase
        {
            ViewModel = new CalendarViewModel(null!)
        };

        ShowInWindow(page, 1200, 900, () =>
        {
            page.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();
            page.GetVisualDescendants()
                .OfType<AtomUICalendar>()
                .ShouldNotContain(static calendar => calendar.Name == "CalendarSemanticOwner");

            var host = page.GetVisualDescendants().OfType<GalleryShowCaseHost>().Single();
            host.SelectedTab = GalleryShowCaseTab.SemanticParts;
            Dispatcher.UIThread.RunJobs();

            page.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldHaveSingleItem();
            var preview = page.GetVisualDescendants()
                              .OfType<SemanticPartPreview>()
                              .Single(static candidate => candidate.Name == "CalendarSemanticPreview");
            var calendar = preview.SemanticOwner.ShouldBeOfType<AtomUICalendar>();
            calendar.Name.ShouldBe("CalendarSemanticOwner");
            calendar.GetVisualDescendants()
                    .OfType<TemplatedControl>()
                    .Count(static cell => cell.Classes.Contains("semantic-item"))
                    .ShouldBe(42);
            calendar.GetVisualDescendants()
                    .OfType<ContentControl>()
                    .Count(static content => content.Classes.Contains("semantic-item-content"))
                    .ShouldBe(42);

            var previewItemsValue = typeof(SemanticPartPreview)
                                    .GetProperty("Items", BindingFlags.Instance | BindingFlags.NonPublic)
                                    ?.GetValue(preview);
            previewItemsValue.ShouldNotBeNull();
            var previewItems = previewItemsValue.ShouldBeAssignableTo<IEnumerable>()
                                                .Cast<object>()
                                                .ToArray();
            previewItems.Length.ShouldBe(6);

            host.SelectedTab = GalleryShowCaseTab.Examples;
            Dispatcher.UIThread.RunJobs();
            page.GetVisualDescendants().OfType<SemanticPartPreview>().ShouldBeEmpty();
        });
    }

    [Fact]
    public void Calendar_Semantic_Style_Example_Applies_The_Official_Style_Values()
    {
        AvaloniaTestApp.EnsureInitialized();

        var page = new CalendarShowCase
        {
            ViewModel = new CalendarViewModel(null!)
        };

        ShowInWindow(page, 1280, 1200, () =>
        {
            var panel = page.GetVisualDescendants().OfType<ShowCasePanel>().Single();
            var item = panel.Children
                            .OfType<ShowCaseItem>()
                            .Single(static candidate => candidate.SourceKey == "calendar-semantic-part");
            item.BadgeText.ShouldBe("v6.1.3");
            item.MaterializeDeferredContent();
            Dispatcher.UIThread.RunJobs();

            var demos = page.GetVisualDescendants()
                            .OfType<AtomUICalendar>()
                            .Where(static calendar => calendar.Classes.Contains("semantic-object") ||
                                                      calendar.Classes.Contains("semantic-function"))
                            .ToArray();
            demos.Length.ShouldBe(2);

            var objectDemo = demos.Single(static calendar => calendar.Classes.Contains("semantic-object"));
            objectDemo.Fullscreen.ShouldBeFalse();
            objectDemo.Bounds.Width.ShouldBe(600);
            objectDemo.CornerRadius.ShouldBe(new CornerRadius(8));
            objectDemo.Padding.ShouldBe(new Thickness(10));
            AssertSolidColor(objectDemo.Background, "#E6F4FF");
            var objectContent = objectDemo.GetVisualDescendants()
                                           .OfType<TemplatedControl>()
                                           .Single(static control => control.Classes.Contains("semantic-content"));
            AssertSolidColor(objectContent.Background, "#FFFFFF");

            var functionDemo = demos.Single(static calendar => calendar.Classes.Contains("semantic-function"));
            functionDemo.Fullscreen.ShouldBeTrue();
            functionDemo.BorderThickness.ShouldBe(new Thickness(2));
            AssertSolidColor(functionDemo.BorderBrush, "#BDE3C3");
            functionDemo.CornerRadius.ShouldBe(new CornerRadius(10));
            functionDemo.Padding.ShouldBe(new Thickness(10));
            AssertSolidColor(functionDemo.Background, "#4DBDE3C3");
            var functionContent = functionDemo.GetVisualDescendants()
                                              .OfType<TemplatedControl>()
                                              .Single(static control => control.Classes.Contains("semantic-content"));
            AssertSolidColor(functionContent.Background, "#FFFFFF");
        });
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
            "<gallery:ShowCaseItem",
            customHeaderIndex + 1,
            StringComparison.Ordinal);
        var semanticPartIndex = source.IndexOf(
            "CalendarShowCaseLangResource SemanticPartStyleTitle",
            StringComparison.Ordinal);

        cardIndex.ShouldBeGreaterThanOrEqualTo(0);
        showWeekIndex.ShouldBeGreaterThan(cardIndex);
        customHeaderIndex.ShouldBeGreaterThan(showWeekIndex);
        nextShowCaseItemIndex.ShouldBeGreaterThan(customHeaderIndex);
        semanticPartIndex.ShouldBeGreaterThan(nextShowCaseItemIndex);

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
            items[^1].SourceKey.ShouldBe("calendar-semantic-part");
            var customHeaderItem = items[^2];

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
            new AtomUI.Desktop.Controls.CalendarDateRange(new DateTime(2024, 2, 1), new DateTime(2024, 2, 29)),
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

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        const string semanticItemMarker = "SourceKey=\"calendar-semantic-part\"";
        var semanticItemStart = source.IndexOf(semanticItemMarker, firstItemStart, StringComparison.Ordinal);
        semanticItemStart.ShouldBeGreaterThan(firstItemStart);
        var semanticItemStartTag = source.LastIndexOf(firstItemMarker, semanticItemStart, StringComparison.Ordinal);
        semanticItemStartTag.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..semanticItemStartTag];
    }

    private static string ExtractShowCaseItem(string source, string sourceKey)
    {
        var sourceKeyIndex = source.IndexOf($"SourceKey=\"{sourceKey}\"", StringComparison.Ordinal);
        sourceKeyIndex.ShouldBeGreaterThanOrEqualTo(0);

        var itemStart = source.LastIndexOf("<gallery:ShowCaseItem", sourceKeyIndex, StringComparison.Ordinal);
        itemStart.ShouldBeGreaterThanOrEqualTo(0);

        const string itemEndMarker = "</gallery:ShowCaseItem>";
        var itemEnd = source.IndexOf(itemEndMarker, sourceKeyIndex, StringComparison.Ordinal);
        itemEnd.ShouldBeGreaterThan(sourceKeyIndex);

        return source[itemStart..(itemEnd + itemEndMarker.Length)];
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

    private static void ShowInWindow(Control content, double width, double height, Action assertion)
    {
        var visualLayerManager = new VisualLayerManager
        {
            EnableAdornerLayer = true,
            Child = content
        };
        var window = new AvaloniaWindow
        {
            Content = visualLayerManager,
            Width = width,
            Height = height
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static void AssertSolidColor(IBrush? actual, string expected)
    {
        actual.ShouldNotBeNull()
              .ShouldBeAssignableTo<ISolidColorBrush>()
              .Color.ShouldBe(Color.Parse(expected));
    }
}
