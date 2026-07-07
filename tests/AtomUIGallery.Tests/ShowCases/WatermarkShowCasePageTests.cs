using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class WatermarkShowCasePageTests
{
    [Fact]
    public void Watermark_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkShowCase.axaml");

        source.ShouldContain("WatermarkShowCaseLangResource PageSubtitle");
        source.ShouldContain("WatermarkShowCaseLangResource PageDescription");
        source.ShouldNotContain("WatermarkShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("WatermarkShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("WatermarkShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("WatermarkShowCaseLangResource ComponentCategory");
        source.ShouldContain("WatermarkShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("WatermarkShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("WatermarkShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("WatermarkShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:WatermarkShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("WatermarkShowCaseLangResource BasicTitle");
        source.ShouldContain("WatermarkShowCaseLangResource MultiLineTitle");
        source.ShouldContain("WatermarkShowCaseLangResource ImageWatermarkTitle");
        source.ShouldContain("WatermarkShowCaseLangResource CustomConfigurationTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Watermark_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldNotContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldNotContain("Tag=\"Api\"");
        pageSource.ShouldNotContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        codeBehindSource.ShouldNotContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldNotContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldNotContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldNotContain("new WatermarkApiDataGrid()");
        codeBehindSource.ShouldNotContain("new WatermarkDesignTokenDataGrid()");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:WatermarkApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("WatermarkShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("WatermarkShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("WatermarkShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("WatermarkShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:WatermarkDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("WatermarkShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("WatermarkShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("WatermarkShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("WatermarkShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void Watermark_ShowCase_Uses_DataGrid_Empty_State_When_No_Design_Tokens()
    {
        var viewModel = new AtomUIGallery.ShowCases.Watermark.WatermarkViewModel(null!);

        viewModel.EnsureDesignTokenRows();

        viewModel.DesignTokenRows.ShouldNotBeNull();
        viewModel.DesignTokenRows!.ShouldBeEmpty();
    }

    [Fact]
    public void Watermark_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyGlyph");
            source.ShouldContain("ApiPropertyText");
            source.ShouldContain("ApiPropertySource");
            source.ShouldContain("ApiPropertyRotate");
        }
    }

    [Fact]
    public void Watermark_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Watermark/Views/WatermarkShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/WatermarkShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractWatermarkExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractWatermarkExampleItems(string source)
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
