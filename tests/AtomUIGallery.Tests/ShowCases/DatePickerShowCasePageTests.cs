using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class DatePickerShowCasePageTests
{
    [Fact]
    public void DatePicker_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml");

        source.ShouldContain("DatePickerShowCaseLangResource PageSubtitle");
        source.ShouldContain("DatePickerShowCaseLangResource PageDescription");
        source.ShouldNotContain("DatePickerShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("DatePickerShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("DatePickerShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("DatePickerShowCaseLangResource ComponentCategory");
        source.ShouldContain("DatePickerShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("DatePickerShowCaseLangResource ScenarioExamples");
        source.ShouldContain("DatePickerShowCaseLangResource ScenarioApi");
        source.ShouldContain("DatePickerShowCaseLangResource ScenarioDesignToken");
        source.ShouldContain("Tag=\"Examples\"");
        source.ShouldContain("Tag=\"Api\"");
        source.ShouldContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:DatePickerShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("DatePickerShowCaseLangResource BasicTitle");
        source.ShouldContain("DatePickerShowCaseLangResource PickerDisplayDateTitle");
        source.ShouldContain("PickerDisplayDate=\"2026-10-20\"");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("DatePickerShowCaseLangResource PlacementTitle");
        source.ShouldContain("Name=\"PickerSizeTypeOptionGroup\"");
        source.ShouldContain("DatePickerShowCaseLangResource P2ContentCustom");
        source.ShouldContain("SizeType=\"{Binding PickerSizeType}\"");
        source.ShouldContain("Selector=\"atom|DatePicker.size-demo-picker[SizeType=Custom]\"");
        source.ShouldContain("Selector=\"atom|RangeDatePicker.size-demo-picker[SizeType=Custom]\"");
        source.ShouldContain("Property=\"Height\" Value=\"38\"");
        source.ShouldContain("Property=\"FontSize\" Value=\"15\"");
        source.ShouldContain("Name=\"PickerPlacementOptionGroup\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void DatePicker_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldContain("Tag=\"Api\"");
        pageSource.ShouldContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        codeBehindSource.ShouldContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldContain("ExamplesContent");
        codeBehindSource.ShouldContain("new DatePickerApiDataGrid()");
        codeBehindSource.ShouldContain("new DatePickerDesignTokenDataGrid()");
        pageSource.ShouldContain("OptionCheckedChanged=\"HandlePickerSizeTypeOptionCheckedChanged\"");
        pageSource.ShouldContain("OptionCheckedChanged=\"HandlePickerPlacementCheckedChanged\"");
        codeBehindSource.ShouldContain("HandlePickerSizeTypeOptionCheckedChanged");
        codeBehindSource.ShouldContain("HandlePickerPlacementCheckedChanged");
        codeBehindSource.ShouldNotContain("PickerSizeTypeOptionGroup.OptionCheckedChanged");
        codeBehindSource.ShouldNotContain("PickerPlacementOptionGroup.OptionCheckedChanged");
        ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/ViewModels/DatePickerViewModel.cs")
            .ShouldContain("new DatePickerApiRow(\"PickerMode\"");
        ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/ViewModels/DatePickerViewModel.cs")
            .ShouldContain("new DatePickerApiRow(\"PickerDisplayDate\"");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:DatePickerApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("DatePickerShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("DatePickerShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("DatePickerShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("DatePickerShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:DatePickerDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("DatePickerShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("DatePickerShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("DatePickerShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("DatePickerShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void DatePicker_ShowCase_Basic_Example_Exposes_All_AntDesign_Picker_Modes()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml");

        source.ShouldContain("PickerMode=\"Date\"");
        source.ShouldContain("PickerMode=\"Week\"");
        source.ShouldContain("PickerMode=\"Month\"");
        source.ShouldContain("PickerMode=\"Quarter\"");
        source.ShouldContain("PickerMode=\"Year\"");
    }

    [Fact]
    public void DatePicker_ShowCase_RangePicker_Example_Exposes_All_AntDesign_Range_Picker_Modes()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml");

        CountOccurrences(source, "<atom:RangeDatePicker PickerMode=\"Date\"").ShouldBe(2);
        source.ShouldContain("PickerMode=\"Date\"\n                                      IsShowTime=\"True\"");
        source.ShouldContain("<atom:RangeDatePicker PickerMode=\"Week\"");
        source.ShouldContain("P2PlaceholderTextStartWeek");
        source.ShouldContain("P2SecondaryPlaceholderTextEndWeek");
        source.ShouldContain("<atom:RangeDatePicker PickerMode=\"Month\"");
        source.ShouldContain("P2PlaceholderTextStartMonth");
        source.ShouldContain("P2SecondaryPlaceholderTextEndMonth");
        source.ShouldContain("<atom:RangeDatePicker PickerMode=\"Quarter\"");
        source.ShouldContain("P2PlaceholderTextStartQuarter");
        source.ShouldContain("P2SecondaryPlaceholderTextEndQuarter");
        source.ShouldContain("<atom:RangeDatePicker PickerMode=\"Year\"");
        source.ShouldContain("P2PlaceholderTextStartYear");
        source.ShouldContain("P2SecondaryPlaceholderTextEndYear");
    }

    [Fact]
    public void DatePicker_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("P2ContentCustom");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("PickerDisplayDateTitle");
            source.ShouldContain("ApiPropertySelectedDateTime");
            source.ShouldContain("ApiPropertyPickerDisplayDate");
            source.ShouldContain("ApiPropertyPickerMode");
            source.ShouldContain("ApiPropertyPickerPlacement");
            source.ShouldContain("ApiPropertyIsNeedConfirm");
            source.ShouldContain("P2PlaceholderTextSelectWeek");
            source.ShouldContain("P2PlaceholderTextSelectMonth");
            source.ShouldContain("P2PlaceholderTextSelectQuarter");
            source.ShouldContain("P2PlaceholderTextSelectYear");
            source.ShouldContain("P2PlaceholderTextStartWeek");
            source.ShouldContain("P2SecondaryPlaceholderTextEndWeek");
            source.ShouldContain("P2PlaceholderTextStartMonth");
            source.ShouldContain("P2SecondaryPlaceholderTextEndMonth");
            source.ShouldContain("P2PlaceholderTextStartQuarter");
            source.ShouldContain("P2SecondaryPlaceholderTextEndQuarter");
            source.ShouldContain("P2PlaceholderTextStartYear");
            source.ShouldContain("P2SecondaryPlaceholderTextEndYear");
            source.ShouldContain("TokenNameCellActiveWithRangeBg");
            source.ShouldContain("TokenNameCellHoverBg");
        }
    }

    [Fact]
    public void DatePicker_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/DatePickerShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractDatePickerExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractDatePickerExampleItems(string source)
    {
        const string firstItemMarker  = "<gallery:ShowCaseItem";
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
        var count      = 0;
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
