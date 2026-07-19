using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class CascaderShowCasePageTests
{
    [Fact]
    public void Cascader_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderShowCase.axaml");

        source.ShouldContain("CascaderShowCaseLangResource PageSubtitle");
        source.ShouldContain("CascaderShowCaseLangResource PageDescription");
        source.ShouldNotContain("CascaderShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("CascaderShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("CascaderShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("CascaderShowCaseLangResource ComponentCategory");
        source.ShouldContain("CascaderShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("CascaderShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("CascaderShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("CascaderShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:CascaderShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(22);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(22);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(22);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:CascaderViewModel\"").ShouldBe(22);
        source.ShouldContain("CascaderShowCaseLangResource BasicTitle");
        source.ShouldContain("CascaderShowCaseLangResource MultipleTitle");
        source.ShouldContain("CascaderShowCaseLangResource PrefixAndSuffixTitle");
        source.ShouldContain("CascaderShowCaseLangResource SizeTitle");
        source.ShouldContain("PlaceholderText=\"{gallery:CascaderShowCaseLangResource P2PlaceholderSizeTypeLarge}\"");
        source.ShouldContain("PlaceholderText=\"{gallery:CascaderShowCaseLangResource P2PlaceholderSizeTypeMiddle}\"");
        source.ShouldContain("PlaceholderText=\"{gallery:CascaderShowCaseLangResource P2PlaceholderSizeTypeSmall}\"");
        source.ShouldContain("PlaceholderText=\"{gallery:CascaderShowCaseLangResource P2PlaceholderSizeTypeCustom}\"");
        source.ShouldContain("SizeType=\"Large\"");
        source.ShouldContain("SizeType=\"Middle\"");
        source.ShouldContain("SizeType=\"Small\"");
        source.ShouldContain("SizeType=\"Custom\"");
        source.ShouldContain("Height=\"36\"");
        source.ShouldContain("CascaderShowCaseLangResource BasicCascaderViewTitle");
        source.ShouldContain("CascaderShowCaseLangResource EmptyIndicatorTitle");
        CountOccurrences(source, "<atom:CascaderView.EmptyIndicator>").ShouldBe(1);
        source.ShouldContain("OptionCheckedChanged=\"HandlePlacementOptionCheckedChanged\"");
        source.ShouldContain("SearchButtonClick=\"HandleFilterCascaderViewClicked\"");
        source.ShouldContain("SearchButtonClick=\"HandleFilterCascaderViewItemsSourceClicked\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Cascader_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldNotContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldNotContain("Tag=\"Api\"");
        pageSource.ShouldNotContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        codeBehindSource.ShouldNotContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldNotContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldNotContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldNotContain("new CascaderApiDataGrid()");
        codeBehindSource.ShouldNotContain("new CascaderDesignTokenDataGrid()");
        codeBehindSource.ShouldNotContain("new CascaderBasicShowCase()");
        codeBehindSource.ShouldNotContain("new CascaderMultipleShowCase()");
        codeBehindSource.ShouldNotContain("new CascaderAdvancedShowCase()");
        codeBehindSource.ShouldNotContain("new CascaderViewShowCase()");
        codeBehindSource.ShouldNotContain("SearchCascaderView.FilterValue");
        codeBehindSource.ShouldNotContain("SearchCascaderViewItemsSource.FilterValue");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:CascaderApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("CascaderShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("CascaderShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("CascaderShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("CascaderShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:CascaderDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("CascaderShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("CascaderShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("CascaderShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("CascaderShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void Cascader_ShowCase_Clears_External_Search_Input_After_Result_Selection()
    {
        var pageSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderShowCase.axaml.cs");

        CountOccurrences(pageSource, "OptionSelected=\"HandleSearchCascaderViewOptionSelected\"").ShouldBe(2);
        codeBehindSource.ShouldContain("HandleSearchCascaderViewOptionSelected");
        codeBehindSource.ShouldContain("searchEdit.Clear();");
    }

    [Fact]
    public void Cascader_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("SizeDescription");
            source.ShouldContain("Custom");
            source.ShouldContain("P2PlaceholderSizeTypeLarge");
            source.ShouldContain("P2PlaceholderSizeTypeMiddle");
            source.ShouldContain("P2PlaceholderSizeTypeSmall");
            source.ShouldContain("P2PlaceholderSizeTypeCustom");
            source.ShouldContain("EmptyIndicatorTitle");
            source.ShouldContain("EmptyIndicatorDescription");
            source.ShouldContain("P2TextCustomEmptyIndicator");
            source.ShouldContain("ApiPropertyOptionsSource");
            source.ShouldContain("ApiPropertyOptionTemplate");
            source.ShouldContain("ApiPropertyShowCheckedStrategy");
            source.ShouldContain("ApiPropertyIsMultiple");
            source.ShouldContain("ApiPropertyDataLoader");
            source.ShouldContain("ApiPropertyDefaultSelectOptionPath");
            source.ShouldContain("ApiPropertyCascaderViewFilterValue");
            source.ShouldContain("TokenNameControlWidth");
            source.ShouldContain("TokenNameControlItemWidth");
            source.ShouldContain("TokenNameDropdownHeight");
            source.ShouldContain("TokenNameOptionHoverBg");
            source.ShouldContain("TokenNameOptionPadding");
        }
    }

    [Fact]
    public void Cascader_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Cascader/Views/CascaderShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/CascaderShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractCascaderExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractCascaderExampleItems(string source)
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
