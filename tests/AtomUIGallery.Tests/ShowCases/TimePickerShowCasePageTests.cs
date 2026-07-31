using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class TimePickerShowCasePageTests
{
    [Fact]
    public void TimePicker_ShowCase_Uses_Document_Layout_With_Examples()
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
