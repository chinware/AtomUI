using System.Windows.Input;
using AtomUIGallery.ShowCases.Segmented;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class SegmentedShowCasePageTests
{
    [Fact]
    public void Segmented_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented/Views/SegmentedShowCase.axaml");

        source.ShouldContain("SegmentedShowCaseLangResource PageSubtitle");
        source.ShouldContain("SegmentedShowCaseLangResource PageDescription");
        source.ShouldNotContain("SegmentedShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("SegmentedShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("SegmentedShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("SegmentedShowCaseLangResource ComponentCategory");
        source.ShouldContain("SegmentedShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("SegmentedShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("SegmentedShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("SegmentedShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldNotContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("<gallery:GalleryShowCaseHost");
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
        source.ShouldContain("Description=\"{gallery:SegmentedShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("SegmentedShowCaseLangResource BasicTitle");
        source.ShouldContain("SegmentedShowCaseLangResource BlockSegmentedTitle");
        source.ShouldContain("SegmentedShowCaseLangResource DisabledTitle");
        source.ShouldContain("SegmentedShowCaseLangResource ThreeSizesTitle");
        source.ShouldContain("SegmentedShowCaseLangResource VerticalTitle");
        source.ShouldContain("SegmentedShowCaseLangResource RoundShapeTitle");
        source.ShouldContain("SegmentedShowCaseLangResource DynamicTitle");
        source.ShouldContain("SegmentedShowCaseLangResource IconOnlyTitle");
        source.ShouldContain("SegmentedShowCaseLangResource WithIconTitle");
        source.ShouldContain("Orientation=\"Vertical\"");
        source.ShouldContain("Shape=\"Round\"");
        source.ShouldContain("SizeType=\"{Binding RoundShapeSizeType}\"");
        source.ShouldContain("SelectionChanged=\"HandleRoundShapeSizeSelectionChanged\"");
        source.ShouldContain("Kind=SunOutlined");
        source.ShouldContain("Kind=MoonOutlined");
        source.ShouldContain("SourceKey=\"segmented-dynamic\"");
        source.ShouldContain("ItemsSource=\"{Binding DynamicOptions}\"");
        source.ShouldContain("Command=\"{Binding LoadMoreOptionsCommand}\"");
        source.ShouldContain("IsEnabled=\"{Binding IsDynamicOptionsLoaded, Converter={x:Static BoolConverters.Not}}\"");
        source.ShouldContain("Content=\"Load more options\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Segmented_ShowCase_Declares_The_Semantic_Previews_And_Style_Example()
    {
        var source = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented/Views/SegmentedShowCase.axaml");
        var english = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented/Localization/en-US.xlf");
        var semanticSource = ExtractSemanticStyleItem(source);

        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("Name=\"SegmentedSemanticPreview\"");
        source.ShouldContain("SemanticOwner=\"{Binding #SegmentedSemanticOwner}\"");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:Segmented}\"");
        source.ShouldNotContain("Name=\"VerticalSegmentedSemanticPreview\"");
        source.ShouldContain("Name=\"VerticalSegmentedSemanticOwner\"");
        source.ShouldContain("Orientation=\"Vertical\"");
        CountOccurrences(source, "<gallery:SemanticPartDescription").ShouldBe(4);
        CountOccurrences(source, "Path=\"root\"").ShouldBe(1);
        CountOccurrences(source, "Path=\"item\"").ShouldBe(1);
        CountOccurrences(source, "Path=\"icon\"").ShouldBe(1);
        CountOccurrences(source, "Path=\"label\"").ShouldBe(1);
        source.ShouldContain("SegmentedShowCaseLangResource SemanticRootDescription");
        source.ShouldContain("SegmentedShowCaseLangResource SemanticItemDescription");
        source.ShouldContain("SegmentedShowCaseLangResource SemanticIconDescription");
        source.ShouldContain("SegmentedShowCaseLangResource SemanticLabelDescription");

        semanticSource.ShouldContain("SourceKey=\"segmented-semantic-part\"");
        semanticSource.ShouldContain("BadgeText=\"{x:Static gallery:GalleryVersionInfo.DisplayVersion}\"");
        semanticSource.ShouldContain("SegmentedShowCaseLangResource SemanticPartStyleTitle");
        semanticSource.ShouldContain("SegmentedShowCaseLangResource SemanticPartStyleDescription");
        semanticSource.ShouldContain("Selector=\"atom|Segmented.semantic-object\"");
        semanticSource.ShouldContain("<atom:SegmentedItemStyle x:SetterTargetType=\"atom:SegmentedItem\">");
        semanticSource.ShouldContain("<atom:SegmentedIconStyle x:SetterTargetType=\"atom:IconPresenter\">");
        semanticSource.ShouldContain("<atom:SegmentedLabelStyle x:SetterTargetType=\"ContentPresenter\">");
        semanticSource.ShouldContain("Classes=\"semantic-object\"");
        CountOccurrences(semanticSource, "Classes=\"semantic-object\"").ShouldBe(2);
        semanticSource.ShouldContain("Orientation=\"Vertical\"");

        foreach (var key in new[]
                 {
                     "SemanticRootDescription",
                     "SemanticItemDescription",
                     "SemanticIconDescription",
                     "SemanticLabelDescription",
                     "SemanticPartStyleTitle",
                     "SemanticPartStyleDescription"
                 })
        {
            english.ShouldContain($"<unit id=\"{key}\">");
        }
    }

    private static string ExtractSemanticStyleItem(string source)
    {
        const string sourceKeyMarker = "segmented-semantic-part";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var keyIndex = source.IndexOf(sourceKeyMarker, StringComparison.Ordinal);
        keyIndex.ShouldBeGreaterThanOrEqualTo(0);

        var itemStart = source.LastIndexOf("<gallery:ShowCaseItem", keyIndex, StringComparison.Ordinal);
        itemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, keyIndex, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(keyIndex);

        return source[itemStart..panelCloseStart];
    }

    [Fact]
    public void Segmented_ShowCase_Dynamic_Demo_Loads_More_Options_Once()
    {
        var viewModel = new SegmentedViewModel(null!);
        var command   = (ICommand)viewModel.LoadMoreOptionsCommand;

        viewModel.DynamicOptions.ShouldBe(new[] { "Daily", "Weekly", "Monthly" });
        viewModel.IsDynamicOptionsLoaded.ShouldBeFalse();

        command.Execute(null);

        viewModel.DynamicOptions.ShouldBe(new[] { "Daily", "Weekly", "Monthly", "Quarterly", "Yearly" });
        viewModel.IsDynamicOptionsLoaded.ShouldBeTrue();

        command.Execute(null);
        viewModel.DynamicOptions.ShouldBe(new[] { "Daily", "Weekly", "Monthly", "Quarterly", "Yearly" });
    }

    [Fact]
    public void Segmented_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Segmented/Views/SegmentedShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/SegmentedShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractSegmentedExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractSegmentedExampleItems(string source)
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
