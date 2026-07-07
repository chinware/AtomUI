using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class TimePickerShowCasePageTests
{
    [Fact]
    public void TimePicker_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerShowCase.axaml");

        source.ShouldContain("TimePickerShowCaseLangResource PageSubtitle");
        source.ShouldContain("TimePickerShowCaseLangResource PageDescription");
        source.ShouldNotContain("TimePickerShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("TimePickerShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("TimePickerShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("TimePickerShowCaseLangResource ComponentCategory");
        source.ShouldContain("TimePickerShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("TimePickerShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("TimePickerShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("TimePickerShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:TimePickerShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(11);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(11);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:TimePickerViewModel\"").ShouldBe(11);
        source.ShouldContain("TimePickerShowCaseLangResource BasicTitle");
        source.ShouldContain("TimePickerShowCaseLangResource BindingTitle");
        source.ShouldContain("SelectedTime=\"{Binding BoundSelectedTime}\"");
        source.ShouldContain("Text=\"{Binding BoundSelectedTimeText}\"");
        source.ShouldContain("Click=\"SetBoundSelectedTimeToNoon\"");
        source.ShouldContain("Click=\"ClearBoundSelectedTime\"");
        source.ShouldContain("RangeStartSelectedTime=\"{Binding BoundRangeStartSelectedTime}\"");
        source.ShouldContain("RangeEndSelectedTime=\"{Binding BoundRangeEndSelectedTime}\"");
        source.ShouldContain("Text=\"{Binding BoundRangeSelectedTimeText}\"");
        source.ShouldContain("Click=\"SetBoundSelectedTimeRangeToWorkHours\"");
        source.ShouldContain("Click=\"ClearBoundSelectedTimeRange\"");
        source.ShouldNotContain("TimePickerShowCaseLangResource RangeBindingTitle");
        source.ShouldNotContain("TimePickerShowCaseLangResource RangeBindingDescription");
        AssertResourceOrder(
            ExtractShowCaseItemByTitle(source, "TimePickerShowCaseLangResource BindingTitle"),
            "SelectedTime=\"{Binding BoundSelectedTime}\"",
            "RangeStartSelectedTime=\"{Binding BoundRangeStartSelectedTime}\"");
        source.ShouldContain("TimePickerShowCaseLangResource PickerDisplayTimeTitle");
        source.ShouldContain("PickerDisplayTime=\"14:25:30\"");
        AssertResourceOrder(
            source,
            "TimePickerShowCaseLangResource BindingTitle",
            "TimePickerShowCaseLangResource PickerDisplayTimeTitle");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("TimePickerShowCaseLangResource HourFormatsTitle");
        source.ShouldContain("Name=\"PickerSizeTypeOptionGroup\"");
        source.ShouldContain("OptionCheckedChanged=\"HandlePickerSizeTypeOptionCheckedChanged\"");
        source.ShouldContain("TimePickerShowCaseLangResource P2TextExpandDirection");
        source.ShouldContain("TimePickerShowCaseLangResource P2ContentLarge");
        source.ShouldContain("TimePickerShowCaseLangResource P2ContentDefault");
        source.ShouldContain("TimePickerShowCaseLangResource P2ContentSmall");
        source.ShouldContain("TimePickerShowCaseLangResource P2ContentCustom");
        source.ShouldContain("SizeType=\"{Binding PickerSizeType}\"");
        source.ShouldContain("Selector=\"atom|TimePicker.size-demo-picker[SizeType=Custom]\"");
        source.ShouldContain("Selector=\"atom|RangeTimePicker.size-demo-picker[SizeType=Custom]\"");
        source.ShouldContain("Property=\"Height\" Value=\"38\"");
        source.ShouldContain("Property=\"FontSize\" Value=\"15\"");
        source.ShouldContain("TimePickerShowCaseLangResource VariantsTitle");
        source.ShouldContain("TimePickerShowCaseLangResource TimeRangePickerTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void TimePicker_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldNotContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldNotContain("Tag=\"Api\"");
        pageSource.ShouldNotContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        codeBehindSource.ShouldNotContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldNotContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldNotContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldNotContain("new TimePickerApiDataGrid()");
        codeBehindSource.ShouldNotContain("new TimePickerDesignTokenDataGrid()");
        codeBehindSource.ShouldContain("HandlePickerSizeTypeOptionCheckedChanged");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:TimePickerApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("TimePickerShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("TimePickerShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("TimePickerShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("TimePickerShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:TimePickerDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("TimePickerShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("TimePickerShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("TimePickerShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("TimePickerShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
        ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/ViewModels/TimePickerViewModel.cs")
            .ShouldContain("new TimePickerApiRow(\"TimePicker.PickerDisplayTime\"");
    }

    [Fact]
    public void TimePicker_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("P2TextExpandDirection");
            source.ShouldContain("P2ContentCustom");
            source.ShouldContain("BindingTitle");
            source.ShouldContain("BindingDescription");
            source.ShouldNotContain("RangeBindingTitle");
            source.ShouldNotContain("RangeBindingDescription");
            source.ShouldContain("P2TextSelectedTime");
            source.ShouldContain("P2TextSelectedTimeRange");
            source.ShouldContain("P2ContentSetNoon");
            source.ShouldContain("P2ContentSetWorkHours");
            source.ShouldContain("P2ContentClear");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertySelectedTime");
            source.ShouldContain("ApiPropertyPickerDisplayTime");
            source.ShouldContain("ApiPropertyClockIdentifier");
            source.ShouldContain("ApiPropertyMinuteIncrement");
            source.ShouldContain("ApiPropertyRangeStartSelectedTime");
            source.ShouldContain("TokenNameItemHeight");
            source.ShouldContain("TokenNameItemWidth");
            source.ShouldContain("TokenNameButtonsMargin");
        }
    }

    [Fact]
    public void TimePicker_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/TimePicker/Views/TimePickerShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/TimePickerShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractTimePickerExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractTimePickerExampleItems(string source)
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

    private static void AssertResourceOrder(string source, params string[] resources)
    {
        var previousIndex = -1;
        foreach (var resource in resources)
        {
            var index = source.IndexOf(resource, StringComparison.Ordinal);
            index.ShouldBeGreaterThan(previousIndex, $"{resource} should appear after the previous resource.");
            previousIndex = index;
        }
    }

    private static string ExtractShowCaseItemByTitle(string source, string titleResource)
    {
        var titleIndex = source.IndexOf(titleResource, StringComparison.Ordinal);
        titleIndex.ShouldBeGreaterThanOrEqualTo(0);

        const string itemStartMarker = "<gallery:ShowCaseItem";
        const string itemEndMarker   = "</gallery:ShowCaseItem>";

        var itemStart = source.LastIndexOf(itemStartMarker, titleIndex, StringComparison.Ordinal);
        itemStart.ShouldBeGreaterThanOrEqualTo(0);

        var itemEnd = source.IndexOf(itemEndMarker, titleIndex, StringComparison.Ordinal);
        itemEnd.ShouldBeGreaterThan(titleIndex);

        return source[itemStart..(itemEnd + itemEndMarker.Length)];
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
