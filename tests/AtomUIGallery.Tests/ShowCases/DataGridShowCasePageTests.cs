using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class DataGridShowCasePageTests
{
    [Fact]
    public void DataGrid_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/DataGrid/Views/DataGridShowCase.axaml");

        source.ShouldContain("DataGridShowCaseLangResource PageSubtitle");
        source.ShouldContain("DataGridShowCaseLangResource PageDescription");
        source.ShouldNotContain("DataGridShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("DataGridShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("DataGridShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("DataGridShowCaseLangResource ComponentCategory");
        source.ShouldContain("DataGridShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("DataGridShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("DataGridShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("DataGridShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:DataGridShowCaseLangResource PageDescription}\"");
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
        source.ShouldNotContain("Filters=\"{Binding NameFilters, DataType={x:Type vm:DataGridViewModel}}\"");
        source.ShouldNotContain("Filters=\"{Binding AddressFilters, DataType={x:Type vm:DataGridViewModel}}\"");
        source.ShouldNotContain("SelectedFilterValues=\"{Binding FilterAndSorterSelectedNames, DataType={x:Type vm:DataGridViewModel}}\"");
        source.ShouldNotContain("SelectedFilterValues=\"{Binding FilterAndSorterSelectedAddresses, DataType={x:Type vm:DataGridViewModel}}\"");
        source.ShouldNotContain("SelectedFilterValues=\"{Binding TreeFilterSelectedNames, DataType={x:Type vm:DataGridViewModel}}\"");
        source.ShouldNotContain("SelectedFilterValues=\"{Binding TreeFilterSelectedAddresses, DataType={x:Type vm:DataGridViewModel}}\"");
        source.ShouldNotContain("SelectedFilterValues=\"{Binding ResetSelectedNames, DataType={x:Type vm:DataGridViewModel}}\"");
        source.ShouldNotContain("SelectedFilterValues=\"{Binding ResetSelectedAddresses, DataType={x:Type vm:DataGridViewModel}}\"");
        source.ShouldNotContain("<atom:DataGridTextColumn.Filters>");
        source.ShouldNotContain("<atom:DataGridTemplateColumn.Filters>");
        source.ShouldNotContain("IsMultipleFilterEnabled");
        source.ShouldNotContain("FilterMode=\"Tree\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain(">Gallery<");
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
