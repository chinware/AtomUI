using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class TabStripShowCasePageTests
{
    [Fact]
    public void TabStrip_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripShowCase.axaml");

        source.ShouldContain("TabStripShowCaseLangResource PageSubtitle");
        source.ShouldContain("TabStripShowCaseLangResource PageDescription");
        source.ShouldNotContain("TabStripShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("TabStripShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("TabStripShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("TabStripShowCaseLangResource ComponentCategory");
        source.ShouldContain("TabStripShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("TabStripShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("TabStripShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("TabStripShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("IsDeferredLoadingEnabled=\"True\"");
        source.ShouldContain("InitialDeferredLoadItemCount=\"4\"");
        source.ShouldContain("DeferredLoadBatchSize=\"2\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:TabStripShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(12);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(12);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(12);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:TabStripViewModel\"").ShouldBe(12);
        source.ShouldContain("TabStripShowCaseLangResource TabStripBasicTitle");
        source.ShouldContain("TabStripShowCaseLangResource TabStripItemsSourceTitle");
        source.ShouldContain("TabStripShowCaseLangResource TabStripAddCloseTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void TabStrip_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldNotContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldNotContain("Tag=\"Api\"");
        pageSource.ShouldNotContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        pageSource.ShouldContain("OptionCheckedChanged=\"HandleTabStripPlacementOptionCheckedChanged\"");
        pageSource.ShouldContain("OptionCheckedChanged=\"HandleCardTabStripPlacementOptionCheckedChanged\"");
        pageSource.ShouldContain("OptionCheckedChanged=\"HandleTabStripSizeTypeOptionCheckedChanged\"");
        pageSource.ShouldContain("AddTabRequest=\"HandleTabStripAddTabRequest\"");

        codeBehindSource.ShouldNotContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldNotContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldNotContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldNotContain("new TabStripApiDataGrid()");
        codeBehindSource.ShouldNotContain("new TabStripDesignTokenDataGrid()");
        codeBehindSource.ShouldNotContain("PositionTabStripOptionGroup");
        codeBehindSource.ShouldNotContain("PositionCardTabStripOptionGroup");
        codeBehindSource.ShouldNotContain("SizeTypeTabStripOptionGroup");
        codeBehindSource.ShouldNotContain("AddTabDemoStrip");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:TabStripApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("TabStripShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("TabStripShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("TabStripShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("TabStripShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:TabStripDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("TabStripShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("TabStripShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("TabStripShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("TabStripShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void TabStrip_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertySelectedIndex");
            source.ShouldContain("ApiPropertyTabStripPlacement");
            source.ShouldContain("ApiPropertyIsTabClosable");
            source.ShouldContain("ApiPropertyIsShowAddTabButton");
            source.ShouldContain("ApiPropertyAddTabRequest");
            source.ShouldContain("TokenNameCardBg");
            source.ShouldContain("TokenNameInkBarColor");
            source.ShouldContain("TokenNameHorizontalItemGutter");
            source.ShouldContain("TokenNameItemSelectedColor");
            source.ShouldContain("TokenNameAddTabButtonMarginHorizontal");
        }
    }

    [Fact]
    public void TabStrip_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/TabStrip/Views/TabStripShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/TabStripShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractTabStripExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractTabStripExampleItems(string source)
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
        return ShowCaseSnapshotMarkup.Normalize(StripTabStripBehaviorMarkup(source));
    }

    private static string StripTabStripBehaviorMarkup(string source)
    {
        var normalized = Regex.Replace(
            source,
            "\\s*OptionCheckedChanged=\"Handle(?:Card)?TabStrip(?:Placement|SizeType)OptionCheckedChanged\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "\\s*AddTabRequest=\"HandleTabStripAddTabRequest\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        return normalized;
    }

    private static string ComputeSha256(string source)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(source));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string ReadSnapshotHash(string source)
    {
        return source
            .Split('\n')
            .First(line => line.StartsWith("sha256:", StringComparison.Ordinal))
            .Split(':', 2)[1]
            .Trim();
    }

    private static int ReadSnapshotCount(string source)
    {
        return int.Parse(source
            .Split('\n')
            .First(line => line.StartsWith("count:", StringComparison.Ordinal))
            .Split(':', 2)[1]
            .Trim());
    }

    private static int CountShowCaseItemElements(string source)
    {
        return Regex.Matches(source, @"<gallery:ShowCaseItem(\s|>)", RegexOptions.CultureInvariant).Count;
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
