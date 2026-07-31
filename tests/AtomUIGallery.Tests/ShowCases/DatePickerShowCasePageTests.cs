using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class DatePickerShowCasePageTests
{
    [Fact]
    public void DatePicker_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml");

        source.ShouldContain("DatePickerShowCaseLangResource PageSubtitle");
        source.ShouldContain("DatePickerShowCaseLangResource PageDescription");
        source.ShouldNotContain("DatePickerShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("DatePickerShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("DatePickerShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("DatePickerShowCaseLangResource ComponentCategory");
        source.ShouldContain("DatePickerShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("DatePickerShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("DatePickerShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("DatePickerShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:DatePickerShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("DatePickerShowCaseLangResource BasicTitle");
        source.ShouldContain("DatePickerShowCaseLangResource SwitchableTitle");
        source.ShouldContain("DatePickerShowCaseLangResource BindingTitle");
        source.ShouldContain("SelectedDateTime=\"{Binding BoundSelectedDateTime}\"");
        source.ShouldContain("RangeStartSelectedDate=\"{Binding BoundRangeStartSelectedDate}\"");
        source.ShouldContain("RangeEndSelectedDate=\"{Binding BoundRangeEndSelectedDate}\"");
        source.ShouldContain("Text=\"{Binding BoundRangeSelectedDateText}\"");
        source.ShouldContain("Click=\"SetBoundSelectedDateRangeThisWeek\"");
        source.ShouldContain("Click=\"ClearBoundSelectedDateRange\"");
        source.ShouldNotContain("DatePickerShowCaseLangResource RangeBindingTitle");
        source.ShouldNotContain("DatePickerShowCaseLangResource RangeBindingDescription");
        AssertResourceOrder(
            ExtractShowCaseItemByTitle(source, "DatePickerShowCaseLangResource BindingTitle"),
            "SelectedDateTime=\"{Binding BoundSelectedDateTime}\"",
            "RangeStartSelectedDate=\"{Binding BoundRangeStartSelectedDate}\"");
        source.ShouldContain("DatePickerShowCaseLangResource PickerDisplayDateTitle");
        source.ShouldContain("PickerDisplayDate=\"2026-10-20\"");
        AssertResourceOrder(
            source,
            "DatePickerShowCaseLangResource BindingTitle",
            "DatePickerShowCaseLangResource PickerDisplayDateTitle");
        AssertResourceOrder(
            source,
            "DatePickerShowCaseLangResource BasicTitle",
            "DatePickerShowCaseLangResource SwitchableTitle",
            "DatePickerShowCaseLangResource BindingTitle");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("DatePickerShowCaseLangResource PlacementTitle");
        source.ShouldContain("Name=\"PickerSizeTypeOptionGroup\"");
        source.ShouldContain("DatePickerShowCaseLangResource P2ContentCustom");
        source.ShouldContain("SizeType=\"{Binding PickerSizeType}\"");
        source.ShouldContain("Selector=\"atom|DatePicker.size-demo-picker[SizeType=Custom]\"");
        source.ShouldContain("Selector=\"atom|RangeDatePicker.size-demo-picker[SizeType=Custom]\"");
        source.ShouldContain("Property=\"Height\" Value=\"38\"");
        source.ShouldContain("Property=\"FontSize\" Value=\"15\"");
        source.ShouldContain("Name=\"PickerPlacementOptionGroup\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void DatePicker_Switchable_Example_Uses_Select_To_Swap_Pickers()
    {
        var source = ExtractShowCaseItemByTitle(
            ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml"),
            "DatePickerShowCaseLangResource SwitchableTitle");

        source.ShouldContain("OptionsSource=\"{Binding PickerTypeOptions}\"");
        source.ShouldContain("SelectedOption=\"{Binding SelectedPickerOption}\"");
        source.ShouldContain("Text=\"{Binding SelectedPickerPlaceholderText}\"");
        source.ShouldContain("IsVisible=\"{Binding IsTimePickerVisible}\"");
        source.ShouldContain("IsVisible=\"{Binding IsDatePickerVisible}\"");
        source.ShouldContain("PickerMode=\"{Binding SelectedPickerMode}\"");
        source.ShouldContain("Width=\"240\"");
        source.ShouldContain("<atom:Select Width=\"104\"");
        source.ShouldContain("<atom:TimePicker");
        source.ShouldContain("<atom:DatePicker");
    }

    [Fact]
    public void DatePicker_Placement_Example_Stacks_Label_And_Options_To_Avoid_Default_Card_Overflow()
    {
        var source = ExtractShowCaseItemByTitle(
            ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml"),
            "DatePickerShowCaseLangResource PlacementTitle");

        source.ShouldContain("<StackPanel Spacing=\"20\">");
        source.ShouldContain("<StackPanel Spacing=\"8\">");
        source.ShouldContain("<atom:TextBlock Text=\"{gallery:DatePickerShowCaseLangResource P2TextPlacement}\" />");
        source.ShouldNotContain("<StackPanel Orientation=\"Horizontal\" Spacing=\"5\" DockPanel.Dock=\"Top\">");
        AssertResourceOrder(
            source,
            "DatePickerShowCaseLangResource P2TextPlacement",
            "Name=\"PickerPlacementOptionGroup\"",
            "<atom:DatePicker");
    }

    [Fact]
    public void DatePicker_ShowCase_Basic_Example_Exposes_All_AntDesign_Picker_Modes()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml");

        source.ShouldContain("PickerMode=\"Date\"");
        source.ShouldContain("PickerMode=\"Week\"");
        source.ShouldContain("PickerMode=\"Month\"");
        source.ShouldContain("PickerMode=\"Quarter\"");
        source.ShouldContain("PickerMode=\"Year\"");
    }

    [Fact]
    public void DatePicker_ShowCase_RangePicker_Example_Exposes_All_AntDesign_Range_Picker_Modes()
    {
        var source = ExtractShowCaseItemByTitle(
            ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml"),
            "DatePickerShowCaseLangResource RangePickerTitle");

        CountOccurrences(source, "<atom:RangeDatePicker PickerMode=\"Date\"").ShouldBe(2);
        source.ShouldContain("PickerMode=\"Date\"\n                                  IsShowTime=\"True\"");
        source.ShouldContain("<atom:RangeDatePicker PickerMode=\"Week\"");
        source.ShouldContain("P2PlaceholderTextStartWeek");
        source.ShouldContain("P2SecondaryPlaceholderTextEndWeek");
        source.ShouldContain("<atom:RangeDatePicker PickerMode=\"Month\"");
        source.ShouldContain("P2PlaceholderTextStartMonth");
        source.ShouldContain("P2SecondaryPlaceholderTextEndMonth");
        source.ShouldContain("<atom:RangeDatePicker PickerMode=\"Quarter\"");
        source.ShouldContain("P2PlaceholderTextStartQuarter");
        source.ShouldContain("P2SecondaryPlaceholderTextEndQuarter");
        source.ShouldContain("<atom:RangeDatePicker PickerMode=\"Year\"");
        source.ShouldContain("P2PlaceholderTextStartYear");
        source.ShouldContain("P2SecondaryPlaceholderTextEndYear");
    }

    [Fact]
    public void DatePicker_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/DatePicker/Views/DatePickerShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/DatePickerShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractDatePickerExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractDatePickerExampleItems(string source)
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
