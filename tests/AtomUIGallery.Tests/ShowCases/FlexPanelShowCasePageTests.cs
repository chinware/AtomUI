using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class FlexPanelShowCasePageTests
{
    [Fact]
    public void FlexPanel_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml");

        source.ShouldContain("FlexPanelShowCaseLangResource PageSubtitle");
        source.ShouldContain("FlexPanelShowCaseLangResource PageDescription");
        source.ShouldContain("FlexPanelShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("FlexPanelShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("FlexPanelShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("FlexPanelShowCaseLangResource ComponentCategory");
        source.ShouldContain("FlexPanelShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("FlexPanelShowCaseLangResource ScenarioExamples");
        source.ShouldContain("FlexPanelShowCaseLangResource ScenarioApi");
        source.ShouldContain("FlexPanelShowCaseLangResource ScenarioDesignToken");
        source.ShouldContain("Tag=\"Examples\"");
        source.ShouldContain("Tag=\"Api\"");
        source.ShouldContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("IsDeferredLoadingEnabled=\"True\"");
        source.ShouldContain("InitialDeferredLoadItemCount=\"4\"");
        source.ShouldContain("DeferredLoadBatchSize=\"2\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(3);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(3);
        source.ShouldContain("LineHeight=\"22\"");
        source.ShouldContain("Text=\"{gallery:FlexPanelShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        CountShowCaseItemElements(source).ShouldBe(11);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(11);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(11);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:FlexPanelViewModel\"").ShouldBe(11);
        source.ShouldContain("FlexPanelShowCaseLangResource BasicLayoutTitle");
        source.ShouldContain("FlexPanelShowCaseLangResource AlignmentTitle");
        source.ShouldContain("FlexPanelShowCaseLangResource AlignSelfTitle");
        source.ShouldContain("FlexPanelShowCaseLangResource PlaygroundTitle");
        source.ShouldContain("AttachedToVisualTree=\"InitializeBasicExample\"");
        source.ShouldContain("AttachedToVisualTree=\"InitializePlaygroundExample\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void FlexPanel_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelDesignTokenDataGrid.axaml.cs");

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
        codeBehindSource.ShouldContain("new FlexPanelApiDataGrid()");
        codeBehindSource.ShouldContain("new FlexPanelDesignTokenDataGrid()");
        codeBehindSource.ShouldNotContain("new FlexPanelBasicShowCase()");
        codeBehindSource.ShouldNotContain("new FlexPanelAlignmentShowCase()");
        codeBehindSource.ShouldNotContain("new FlexPanelItemShowCase()");
        codeBehindSource.ShouldNotContain("new FlexPanelCombinationShowCase()");
        codeBehindSource.ShouldNotContain("new FlexPanelPlaygroundShowCase()");
        codeBehindSource.ShouldContain("InitializeBasicExample");
        codeBehindSource.ShouldContain("InitializePlaygroundExample");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:FlexPanelApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("FlexPanelShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("FlexPanelShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("FlexPanelShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("FlexPanelShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:FlexPanelDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("FlexPanelShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("FlexPanelShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("FlexPanelShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("FlexPanelShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void FlexPanel_ShowCase_Initializers_Use_Avalonia_Slider_Type_For_Bare_Slider_Elements()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml.cs");

        var bareSliderNames = Regex.Matches(
                pageSource,
                @"<Slider\s+x:Name=""([^""]+)""",
                RegexOptions.CultureInvariant)
            .Select(match => match.Groups[1].Value)
            .ToArray();

        bareSliderNames.ShouldNotBeEmpty();
        foreach (var sliderName in bareSliderNames)
        {
            codeBehindSource.ShouldContain($"FindRequired<AvaloniaSlider>(root, \"{sliderName}\")");
            codeBehindSource.ShouldNotContain($"FindRequired<AtomSlider>(root, \"{sliderName}\")");
        }
    }

    [Fact]
    public void FlexPanel_ShowCase_Template_Initializers_Search_Visual_And_Logical_Trees()
    {
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml.cs");

        codeBehindSource.ShouldContain("using Avalonia.LogicalTree;");
        codeBehindSource.ShouldContain("root.GetLogicalDescendants()");
        codeBehindSource.ShouldContain("root is T typedRoot && typedRoot.Name == name");
    }

    [Fact]
    public void FlexPanel_ShowCase_Blue_Demo_Tile_Text_Uses_White_Foreground()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml");

        var blueTileBlocks = Regex.Matches(
                source,
                @"<Border\b(?=[^>]*Background=""#(?:4F7CF5|1F5BFF)"")[^>]*>(.*?)</Border>",
                RegexOptions.Singleline | RegexOptions.CultureInvariant)
            .Select(match => match.Groups[1].Value)
            .Where(block => block.Contains("<atom:TextBlock", StringComparison.Ordinal))
            .ToArray();

        blueTileBlocks.ShouldNotBeEmpty();
        foreach (var block in blueTileBlocks)
        {
            block.ShouldContain("Foreground=\"White\"");
        }
    }

    [Fact]
    public void FlexPanel_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyDirection");
            source.ShouldContain("ApiPropertyWrap");
            source.ShouldContain("ApiPropertyJustifyContent");
            source.ShouldContain("ApiPropertyAlignItems");
            source.ShouldContain("ApiPropertyAlignContent");
            source.ShouldContain("ApiPropertyColumnSpacing");
            source.ShouldContain("ApiPropertyFlexGrow");
            source.ShouldContain("ApiPropertyFlexBasis");
            source.ShouldContain("ApiPropertyFlexAlignSelf");
            source.ShouldContain("TokenNameNoComponentToken");
        }
    }

    [Fact]
    public void FlexPanel_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Layout/FlexPanel/Views/FlexPanelShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/FlexPanelShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractFlexPanelExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractFlexPanelExampleItems(string source)
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
        source = Regex.Replace(
            source,
            @"\s*AttachedToVisualTree=""Initialize[A-Za-z0-9]+Example""",
            string.Empty,
            RegexOptions.CultureInvariant);

        return ShowCaseSnapshotMarkup.Normalize(source);
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
        var repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));
        return Path.Combine(repoRoot, relativePath);
    }
}
