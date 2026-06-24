using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class MenuShowCasePageTests
{
    [Fact]
    public void Menu_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuShowCase.axaml");

        source.ShouldContain("MenuShowCaseLangResource PageSubtitle");
        source.ShouldContain("MenuShowCaseLangResource PageDescription");
        source.ShouldContain("MenuShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("MenuShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("MenuShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("MenuShowCaseLangResource ComponentCategory");
        source.ShouldContain("MenuShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("MenuShowCaseLangResource ScenarioExamples");
        source.ShouldContain("MenuShowCaseLangResource ScenarioApi");
        source.ShouldContain("MenuShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Text=\"{gallery:MenuShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        CountShowCaseItemElements(source).ShouldBe(16);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(16);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(16);
        CountOccurrences(source, "DataTemplate x:DataType=\"viewModels:MenuViewModel\"").ShouldBe(16);
        source.ShouldContain("MenuShowCaseLangResource BasicTitle");
        source.ShouldContain("MenuShowCaseLangResource IconAndSubmenuTitle");
        source.ShouldContain("MenuShowCaseLangResource MenuItemItemsSourceTitle");
        source.ShouldContain("MenuShowCaseLangResource ContextMenuTitle");
        source.ShouldContain("MenuShowCaseLangResource VerticalNavMenuTitle");
        source.ShouldContain("MenuShowCaseLangResource InlineCollapsedMenuTitle");
        source.ShouldContain("IsInlineCollapsed=\"{Binding IsInlineCollapsed}\"");
        source.ShouldContain("Click=\"HandleToggleInlineCollapsedClick\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Menu_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuDesignTokenDataGrid.axaml.cs");

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
        codeBehindSource.ShouldContain("new MenuApiDataGrid()");
        codeBehindSource.ShouldContain("new MenuDesignTokenDataGrid()");
        codeBehindSource.ShouldContain("HandleChangeModeCheckChanged");
        codeBehindSource.ShouldContain("HandleChangeStyleCheckChanged");
        codeBehindSource.ShouldContain("HandleToggleInlineCollapsedClick");
        codeBehindSource.ShouldNotContain("new MenuBasicShowCase()");
        codeBehindSource.ShouldNotContain("new MenuFeaturesShowCase()");
        codeBehindSource.ShouldNotContain("new MenuItemsSourceShowCase()");
        codeBehindSource.ShouldNotContain("new MenuContextShowCase()");
        codeBehindSource.ShouldNotContain("new MenuNavigationShowCase()");
        codeBehindSource.ShouldNotContain("GalleryBindingUtils.OneWay");
        codeBehindSource.ShouldNotContain("BasicItemsSourceMenu");
        codeBehindSource.ShouldNotContain("InlineModeMenu");
        codeBehindSource.ShouldNotContain("ItemsSourceDemoNavMenu,");
        codeBehindSource.ShouldNotContain("BasicContextMenu");
        codeBehindSource.ShouldNotContain("ChangeModeSwitch.IsCheckedChanged");
        codeBehindSource.ShouldNotContain("ChangeStyleSwitch.IsCheckedChanged");
        pageSource.ShouldContain("ItemsSource=\"{Binding MenuItems}\"");
        pageSource.ShouldContain("ItemsSource=\"{Binding InlineNavMenuNodes}\"");
        pageSource.ShouldContain("ItemsSource=\"{Binding ItemsSourceDemoNavMenuNodes}\"");
        pageSource.ShouldContain("ItemsSource=\"{Binding ContextMenuItems}\"");
        pageSource.ShouldContain("IsInlineCollapsed=\"{Binding IsInlineCollapsed}\"");
        pageSource.ShouldContain("IsCheckedChanged=\"HandleChangeModeCheckChanged\"");
        pageSource.ShouldContain("IsCheckedChanged=\"HandleChangeStyleCheckChanged\"");
        pageSource.ShouldContain("Click=\"HandleToggleInlineCollapsedClick\"");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:MenuApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("MenuShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("MenuShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("MenuShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("MenuShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:MenuDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("MenuShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("MenuShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("MenuShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("MenuShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void Menu_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyMenuSizeType");
            source.ShouldContain("ApiPropertyMenuDisplayPageSize");
            source.ShouldContain("ApiPropertyNavMenuMode");
            source.ShouldContain("ApiPropertyNavMenuDefaultOpenPaths");
            source.ShouldContain("TokenNameMenuItemHeight");
            source.ShouldContain("TokenNameNavMenuItemHeight");
            source.ShouldContain("InlineCollapsedMenuTitle");
            source.ShouldContain("InlineCollapsedMenuDescription");
            source.ShouldContain("P2HeaderOptionN5");
            source.ShouldContain("P2HeaderOptionN8");
        }
    }

    [Fact]
    public void Menu_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Menu/Views/MenuShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/MenuShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractMenuExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractMenuExampleItems(string source)
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
        return ShowCaseSnapshotMarkup.Normalize(StripMenuRuntimeBindingMarkup(source));
    }

    private static string StripMenuRuntimeBindingMarkup(string source)
    {
        var normalized = Regex.Replace(
            source,
            @"\s+ItemsSource=""\{Binding (MenuItems|InlineNavMenuNodes|ItemsSourceDemoNavMenuNodes|ContextMenuItems)\}""",
            string.Empty,
            RegexOptions.CultureInvariant);

        return Regex.Replace(
            normalized,
            @"\s+(IsCheckedChanged=""HandleChange(Mode|Style)CheckChanged""|Click=""HandleToggleInlineCollapsedClick"")",
            string.Empty,
            RegexOptions.CultureInvariant);
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
