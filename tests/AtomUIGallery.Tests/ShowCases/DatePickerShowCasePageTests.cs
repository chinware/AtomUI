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
        source.ShouldContain("DatePickerShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("DatePickerShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("DatePickerShowCaseLangResource InfoBaseClassLabel");
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
        source.ShouldContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(3);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(3);
        source.ShouldContain("LineHeight=\"22\"");
        source.ShouldContain("Text=\"{gallery:DatePickerShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("DatePickerShowCaseLangResource BasicTitle");
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
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertySelectedDateTime");
            source.ShouldContain("ApiPropertyPickerPlacement");
            source.ShouldContain("ApiPropertyIsNeedConfirm");
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
