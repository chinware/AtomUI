using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class SplitterShowCasePageTests
{
    [Fact]
    public void Splitter_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterShowCase.axaml");

        source.ShouldContain("SplitterShowCaseLangResource PageSubtitle");
        source.ShouldContain("SplitterShowCaseLangResource PageDescription");
        source.ShouldNotContain("SplitterShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("SplitterShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("SplitterShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("SplitterShowCaseLangResource ComponentCategory");
        source.ShouldContain("SplitterShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("SplitterShowCaseLangResource ScenarioExamples");
        source.ShouldContain("SplitterShowCaseLangResource ScenarioApi");
        source.ShouldContain("SplitterShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:SplitterShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("SplitterShowCaseLangResource BasicTitle");
        source.ShouldContain("SplitterShowCaseLangResource HorizontalTitle");
        source.ShouldContain("SplitterShowCaseLangResource CompositeTitle");
        source.ShouldContain("SplitterShowCaseLangResource ResizableDisabledTitle");
        source.ShouldContain("SplitterShowCaseLangResource ShowCollapsibleIconTitle");
        source.ShouldContain("SplitterShowCaseLangResource MultiPanelsTitle");
        source.ShouldContain("SplitterShowCaseLangResource LazyTitle");
        source.ShouldContain("SplitterShowCaseLangResource LineStyleTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Splitter_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterDesignTokenDataGrid.axaml.cs");

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
        codeBehindSource.ShouldContain("new SplitterApiDataGrid()");
        codeBehindSource.ShouldContain("new SplitterDesignTokenDataGrid()");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:SplitterApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("SplitterShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("SplitterShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("SplitterShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("SplitterShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:SplitterDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("SplitterShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("SplitterShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("SplitterShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("SplitterShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void Splitter_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyOrientation");
            source.ShouldContain("ApiPropertyIsLazy");
            source.ShouldContain("ApiPropertyHandleSize");
            source.ShouldContain("ApiPropertyLineThickness");
            source.ShouldContain("ApiPropertyLineCornerRadius");
            source.ShouldContain("ApiPropertyCollapsible");
            source.ShouldContain("TokenNameSplitBarSize");
            source.ShouldContain("TokenNameHandleIconSize");
            source.ShouldContain("LineStyleTitle");
            source.ShouldContain("LineStyleDescription");
        }
    }

    [Fact]
    public void Splitter_ShowCase_Api_Includes_Line_Style_Properties()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/ViewModels/SplitterViewModel.cs");

        source.ShouldContain("new SplitterApiRow(\"LineThickness\", Lang(SplitterShowCaseLangResourceKind.ApiPropertyLineThickness), \"double\", \"cyan\", \"token\")");
        source.ShouldContain("new SplitterApiRow(\"LineCornerRadius\", Lang(SplitterShowCaseLangResourceKind.ApiPropertyLineCornerRadius), \"CornerRadius\", \"cyan\", \"token\")");
    }

    [Fact]
    public void Splitter_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/Splitter/Views/SplitterShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/SplitterShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractSplitterExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractSplitterExampleItems(string source)
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
