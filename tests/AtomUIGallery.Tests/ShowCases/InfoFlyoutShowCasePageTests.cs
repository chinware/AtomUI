using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class InfoFlyoutShowCasePageTests
{
    [Fact]
    public void InfoFlyout_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout/Views/InfoFlyoutShowCase.axaml");

        source.ShouldContain("InfoFlyoutShowCaseLangResource PageSubtitle");
        source.ShouldContain("InfoFlyoutShowCaseLangResource PageDescription");
        source.ShouldNotContain("InfoFlyoutShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("InfoFlyoutShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("InfoFlyoutShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("InfoFlyoutShowCaseLangResource ComponentCategory");
        source.ShouldContain("InfoFlyoutShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("InfoFlyoutShowCaseLangResource ScenarioExamples");
        source.ShouldContain("InfoFlyoutShowCaseLangResource ScenarioApi");
        source.ShouldContain("InfoFlyoutShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:InfoFlyoutShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("InfoFlyoutShowCaseLangResource BasicTitle");
        source.ShouldContain("InfoFlyoutShowCaseLangResource TriggerWaysTitle");
        source.ShouldContain("InfoFlyoutShowCaseLangResource PlacementTitle");
        source.ShouldContain("InfoFlyoutShowCaseLangResource ArrowTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void InfoFlyout_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout/Views/InfoFlyoutShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout/Views/InfoFlyoutShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout/Views/InfoFlyoutApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout/Views/InfoFlyoutApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout/Views/InfoFlyoutDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout/Views/InfoFlyoutDesignTokenDataGrid.axaml.cs");

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
        codeBehindSource.ShouldContain("new InfoFlyoutApiDataGrid()");
        codeBehindSource.ShouldContain("new InfoFlyoutDesignTokenDataGrid()");
        pageSource.ShouldContain("SelectionChanged=\"HandleArrowSegmentedSelectionChanged\"");
        codeBehindSource.ShouldContain("HandleArrowSegmentedSelectionChanged");
        codeBehindSource.ShouldContain("viewModel.HandleSelectionChanged(sender, args)");
        codeBehindSource.ShouldNotContain("ArrowSegmented.SelectionChanged += viewModel.HandleSelectionChanged");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:InfoFlyoutApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("InfoFlyoutShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("InfoFlyoutShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("InfoFlyoutShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("InfoFlyoutShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:InfoFlyoutDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("InfoFlyoutShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("InfoFlyoutShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("InfoFlyoutShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("InfoFlyoutShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void InfoFlyout_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyTrigger");
            source.ShouldContain("ApiPropertyPlacement");
            source.ShouldContain("ApiPropertyIsArrowVisible");
            source.ShouldContain("ApiPropertyShouldUseOverlayPopup");
            source.ShouldContain("TokenNameMarginToAnchor");
            source.ShouldContain("TokenNameOverlayHostShadow");
        }
    }

    [Fact]
    public void InfoFlyout_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/InfoFlyout/Views/InfoFlyoutShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/InfoFlyoutShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractInfoFlyoutExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractInfoFlyoutExampleItems(string source)
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
