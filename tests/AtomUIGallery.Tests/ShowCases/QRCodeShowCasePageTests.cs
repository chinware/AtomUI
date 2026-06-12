using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class QRCodeShowCasePageTests
{
    [Fact]
    public void QRCode_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeShowCase.axaml");

        source.ShouldContain("QRCodeShowCaseLangResource PageSubtitle");
        source.ShouldContain("QRCodeShowCaseLangResource PageDescription");
        source.ShouldContain("QRCodeShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("QRCodeShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("QRCodeShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("QRCodeShowCaseLangResource ComponentCategory");
        source.ShouldContain("QRCodeShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("QRCodeShowCaseLangResource ScenarioExamples");
        source.ShouldContain("QRCodeShowCaseLangResource ScenarioApi");
        source.ShouldContain("QRCodeShowCaseLangResource ScenarioDesignToken");
        source.ShouldContain("Tag=\"Examples\"");
        source.ShouldContain("Tag=\"Api\"");
        source.ShouldContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,10,28,0\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(3);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(3);
        source.ShouldContain("LineHeight=\"22\"");
        source.ShouldContain("Text=\"{gallery:QRCodeShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("QRCodeShowCaseLangResource BasicUsageTitle");
        source.ShouldContain("QRCodeShowCaseLangResource WithIconTitle");
        source.ShouldContain("QRCodeShowCaseLangResource DifferentStatusTitle");
        source.ShouldContain("QRCodeShowCaseLangResource AdvancedUsageTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void QRCode_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldContain("Tag=\"Api\"");
        pageSource.ShouldContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        codeBehindSource.ShouldContain("ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged");
        codeBehindSource.ShouldContain("EnsureSelectedScenarioContent");
        codeBehindSource.ShouldContain("ScenarioContentHost.Content = content");
        codeBehindSource.ShouldContain("ExamplesContent");
        codeBehindSource.ShouldContain("new QRCodeApiDataGrid()");
        codeBehindSource.ShouldContain("new QRCodeDesignTokenDataGrid()");
        codeBehindSource.ShouldContain("GalleryBindingUtils.BindCommand(SmallerBtn");
        codeBehindSource.ShouldContain("GalleryBindingUtils.BindCommand(LargerBtn");
        codeBehindSource.ShouldContain("EccLevelSegmented");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:QRCodeApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,0\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("QRCodeShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("QRCodeShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("QRCodeShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("QRCodeShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:QRCodeDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,0\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("QRCodeShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("QRCodeShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("QRCodeShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("QRCodeShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void QRCode_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyValue");
            source.ShouldContain("ApiPropertyStatus");
            source.ShouldContain("ApiPropertyEccLevel");
            source.ShouldContain("ApiEventRefreshRequested");
            source.ShouldContain("TokenNameQRCodeTextColor");
        }
    }

    [Fact]
    public void QRCode_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/QRCode/Views/QRCodeShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/QRCodeShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractQRCodeExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractQRCodeExampleItems(string source)
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
        return string.Join(
            "\n",
            source
                .Replace("\r\n", "\n")
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => line.Length > 0));
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
