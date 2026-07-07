using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class TreeSelectShowCasePageTests
{
    [Fact]
    public void TreeSelect_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectShowCase.axaml");

        source.ShouldContain("TreeSelectShowCaseLangResource PageSubtitle");
        source.ShouldContain("TreeSelectShowCaseLangResource PageDescription");
        source.ShouldNotContain("TreeSelectShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("TreeSelectShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("TreeSelectShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("TreeSelectShowCaseLangResource ComponentCategory");
        source.ShouldContain("TreeSelectShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("TreeSelectShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("TreeSelectShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("TreeSelectShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:TreeSelectShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(13);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(13);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(13);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:TreeSelectViewModel\"").ShouldBe(13);
        source.ShouldContain("TreeSelectShowCaseLangResource BasicTitle");
        source.ShouldContain("TreeSelectShowCaseLangResource BindingTitle");
        source.ShouldContain("TreeSelectShowCaseLangResource BindingDescription");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("ItemsSource=\"{Binding BindingSingleTreeNodes}\"");
        source.ShouldContain("ItemsSource=\"{Binding BindingMultipleTreeNodes}\"");
        source.ShouldContain("SelectedItem=\"{Binding BoundSelectedItem}\"");
        source.ShouldContain("SelectedItems=\"{Binding BoundSelectedItems}\"");
        source.ShouldContain("TreeSelectShowCaseLangResource PlacementTitle");
        source.ShouldContain("TreeSelectShowCaseLangResource SizeTypeTitle");
        source.ShouldContain("OptionCheckedChanged=\"HandleSizeTypeChanged\"");
        source.ShouldContain("SizeType=\"{Binding TreeSelectSizeType}\"");
        source.ShouldContain("Selector=\"atom|TreeSelect.size-demo-tree-select[SizeType=Custom]\"");
        source.ShouldContain("TreeSelectShowCaseLangResource VariantsTitle");
        source.ShouldContain("TreeSelectShowCaseLangResource PrefixAndSuffixTitle");
        source.ShouldContain("OptionCheckedChanged=\"HandlePlacementOptionCheckedChanged\"");
        source.ShouldNotContain("{Binding #");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void TreeSelect_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldNotContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldNotContain("Tag=\"Api\"");
        pageSource.ShouldNotContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        codeBehindSource.ShouldNotContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldNotContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldNotContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldNotContain("new TreeSelectApiDataGrid()");
        codeBehindSource.ShouldNotContain("new TreeSelectDesignTokenDataGrid()");
        codeBehindSource.ShouldNotContain("new TreeSelectBasicShowCase()");
        codeBehindSource.ShouldNotContain("new TreeSelectBehaviorShowCase()");
        codeBehindSource.ShouldNotContain("new TreeSelectAppearanceShowCase()");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:TreeSelectApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("TreeSelectShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("TreeSelectShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("TreeSelectShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("TreeSelectShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:TreeSelectDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("TreeSelectShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("TreeSelectShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("TreeSelectShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("TreeSelectShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
        pageSource.ShouldContain("IsShowIcon=\"{Binding IsShowTreeSelectIcon}\"");
        pageSource.ShouldContain("IsShowLeafIcon=\"{Binding IsShowTreeSelectLeafIcon}\"");
        pageSource.ShouldContain("IsShowLine=\"{Binding IsShowTreeSelectLine}\"");
        pageSource.ShouldNotContain("{Binding #");
    }

    [Fact]
    public void TreeSelect_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyItemsSource");
            source.ShouldContain("ApiPropertySelectedItem");
            source.ShouldContain("ApiPropertySelectedItems");
            source.ShouldContain("ApiPropertyIsMultiple");
            source.ShouldContain("ApiPropertyIsTreeCheckable");
            source.ShouldContain("ApiPropertyIsDefaultExpandAll");
            source.ShouldContain("ApiPropertyDataLoader");
            source.ShouldContain("ApiPropertyPlacement");
            source.ShouldContain("ApiPropertyStatus");
            source.ShouldContain("TokenNameMinPopupWidth");
            source.ShouldContain("BindingTitle");
            source.ShouldContain("BindingDescription");
            source.ShouldContain("BindingSingleLabel");
            source.ShouldContain("BindingMultipleLabel");
            source.ShouldContain("SizeTypeTitle");
            source.ShouldContain("SizeTypeDescription");
            source.ShouldContain("P2ContentLarge");
            source.ShouldContain("P2ContentDefault");
            source.ShouldContain("P2ContentSmall");
            source.ShouldContain("P2ContentCustom");
        }
    }

    [Fact]
    public void TreeSelect_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TreeSelect/Views/TreeSelectShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/TreeSelectShowCaseExamples.snapshot");

        var normalized = ShowCaseSnapshotMarkup.Normalize(ExtractTreeSelectExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractTreeSelectExampleItems(string source)
    {
        const string firstItemMarker  = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..panelCloseStart];
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
