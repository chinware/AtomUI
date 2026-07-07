using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class SelectShowCasePageTests
{
    [Fact]
    public void Select_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectShowCase.axaml");

        source.ShouldContain("SelectShowCaseLangResource PageSubtitle");
        source.ShouldContain("SelectShowCaseLangResource PageDescription");
        source.ShouldNotContain("SelectShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("SelectShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("SelectShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("SelectShowCaseLangResource ComponentCategory");
        source.ShouldContain("SelectShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("SelectShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("SelectShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("SelectShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:SelectShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(15);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(15);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(15);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:SelectViewModel\"").ShouldBe(15);
        source.ShouldContain("SelectShowCaseLangResource BasicTitle");
        source.ShouldContain("SelectShowCaseLangResource BindingTitle");
        source.ShouldContain("SelectShowCaseLangResource BindingDescription");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("SelectedOption=\"{Binding BoundSelectedOption}\"");
        source.ShouldContain("SelectedOptions=\"{Binding BoundSelectedOptions}\"");
        source.ShouldContain("SelectShowCaseLangResource CustomDropdownOptionsTitle");
        source.ShouldContain("SelectShowCaseLangResource SizesTitle");
        source.ShouldContain("OptionCheckedChanged=\"HandleSizeTypeChanged\"");
        source.ShouldContain("CustomizableSizeType.Custom");
        source.ShouldContain("SelectShowCaseLangResource P2ContentCustom");
        source.ShouldContain("atom|Select.size-demo-select[SizeType=Custom]");
        source.ShouldContain("Property=\"Height\" Value=\"38\"");
        source.ShouldContain("Property=\"FontSize\" Value=\"15\"");
        source.ShouldContain("AttachedToVisualTree=\"HandleCustomSearchSelectAttached\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Select_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldNotContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldNotContain("Tag=\"Api\"");
        pageSource.ShouldNotContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        codeBehindSource.ShouldNotContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldNotContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldNotContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldNotContain("new SelectApiDataGrid()");
        codeBehindSource.ShouldNotContain("new SelectDesignTokenDataGrid()");
        codeBehindSource.ShouldNotContain("new SelectBasicShowCase()");
        codeBehindSource.ShouldNotContain("new SelectOptionsShowCase()");
        codeBehindSource.ShouldNotContain("new SelectAppearanceShowCase()");
        codeBehindSource.ShouldContain("CustomSearchSelect.Filter = new CustomFilter()");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:SelectApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("SelectShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("SelectShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("SelectShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("SelectShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:SelectDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("SelectShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("SelectShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("SelectShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("SelectShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void Select_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("BindingTitle");
            source.ShouldContain("BindingDescription");
            source.ShouldContain("BindingSingleLabel");
            source.ShouldContain("BindingMultipleLabel");
            source.ShouldContain("ApiPropertyMode");
            source.ShouldContain("ApiPropertyOptionsSource");
            source.ShouldContain("ApiPropertySelectedOption");
            source.ShouldContain("ApiPropertySelectedOptions");
            source.ShouldContain("ApiPropertyIsFilterEnabled");
            source.ShouldContain("ApiPropertyIsResponsiveTagMode");
            source.ShouldContain("TokenNameMultipleItemBg");
            source.ShouldContain("TokenNameMultipleItemHeight");
            source.ShouldContain("TokenNameOptionSelectedColor");
            source.ShouldContain("TokenNameOptionPadding");
            source.ShouldContain("TokenNamePopupContentPadding");
            source.ShouldContain("P2ContentCustom");
        }
    }

    [Fact]
    public void Select_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Select/Views/SelectShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/SelectShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractSelectExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractSelectExampleItems(string source)
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
            @"\s*AttachedToVisualTree=""HandleCustomSearchSelectAttached""",
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
