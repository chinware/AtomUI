using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class DataGridShowCasePageTests
{
    [Fact]
    public void DataGrid_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml");

        source.ShouldContain("DataGridShowCaseLangResource PageSubtitle");
        source.ShouldContain("DataGridShowCaseLangResource PageDescription");
        source.ShouldContain("DataGridShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("DataGridShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("DataGridShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("DataGridShowCaseLangResource ComponentCategory");
        source.ShouldContain("DataGridShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("DataGridShowCaseLangResource ScenarioExamples");
        source.ShouldContain("DataGridShowCaseLangResource ScenarioApi");
        source.ShouldContain("DataGridShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Text=\"{gallery:DataGridShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        CountShowCaseItemElements(source).ShouldBe(22);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(22);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(22);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:DataGridViewModel\"").ShouldBe(22);
        source.ShouldContain("DataGridShowCaseLangResource BasicTitle");
        source.ShouldContain("DataGridShowCaseLangResource SelectionTitle");
        source.ShouldContain("DataGridShowCaseLangResource FilterAndSorterTitle");
        source.ShouldContain("DataGridShowCaseLangResource ExpandableRowTitle");
        source.ShouldContain("DataGridShowCaseLangResource FixedHeaderTitle");
        source.ShouldContain("DataGridShowCaseLangResource DragColumnSortingTitle");
        source.ShouldContain("DataGridShowCaseLangResource EditableCellsTitle");
        source.ShouldContain("DataGridShowCaseLangResource BasicPagingTitle");
        source.ShouldContain("AttachedToVisualTree=\"HandleExampleDataGridAttached\"");
        source.ShouldContain("Click=\"HandleSortAgeBtnClick\"");
        source.ShouldContain("IsCheckedChanged=\"HandleColumnVisibleChanged\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void DataGrid_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridDesignTokenDataGrid.axaml.cs");

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
        codeBehindSource.ShouldContain("new DataGridApiDataGrid()");
        codeBehindSource.ShouldContain("new DataGridDesignTokenDataGrid()");
        codeBehindSource.ShouldNotContain("new DataGridBasicShowCase()");
        codeBehindSource.ShouldNotContain("ScenarioTabs.Items.OfType<AtomUI.Desktop.Controls.TabItem>()");
        codeBehindSource.ShouldNotContain("FilterAndSortGrid.ItemsSource");
        codeBehindSource.ShouldNotContain("BasicPagingCaseGrid.ItemsSource");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:DataGridApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("DataGridShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("DataGridShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("DataGridShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("DataGridShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:DataGridDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("DataGridShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("DataGridShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("DataGridShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("DataGridShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void DataGrid_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyItemsSource");
            source.ShouldContain("ApiPropertyAutoGenerateColumns");
            source.ShouldContain("ApiPropertyCanUserFilterColumns");
            source.ShouldContain("ApiPropertyLeftFrozenColumnCount");
            source.ShouldContain("ApiPropertyPaginationVisibility");
            source.ShouldContain("TokenNameHeaderBg");
            source.ShouldContain("TokenNameRowHoverBg");
            source.ShouldContain("TokenNameCellPadding");
            source.ShouldContain("TokenNameSelectionColumnWidth");
            source.ShouldContain("TokenNamePaginationMargin");
        }
    }

    [Fact]
    public void DataGrid_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/DataGridShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractDataGridExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractDataGridExampleItems(string source)
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
        return ShowCaseSnapshotMarkup.Normalize(StripDataGridRuntimeBindingMarkup(source));
    }

    private static string StripDataGridRuntimeBindingMarkup(string source)
    {
        var normalized = Regex.Replace(
            source,
            "\\s*AttachedToVisualTree=\"HandleExampleDataGridAttached\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "\\s*Click=\"Handle(?:SortAgeBtn|ClearFiltersBtn|ClearFiltersAndSortersBtn)Click\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "\\s*IsCheckedChanged=\"Handle(?:SelectionModeCheckedChanged|ColumnVisibleChanged|ShowTopPaginationCheckBoxChanged|ShowBottomPaginationCheckBoxChanged)\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "\\s*OptionCheckedChanged=\"Handle(?:TopPaginationAlignChanged|BottomPaginationAlignChanged)\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "(<atom:CheckBox\\s+Name=\"ColumnCheckBox[1-6]\"[^>]*?)\\s+IsChecked=\"True\"",
            "$1",
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
