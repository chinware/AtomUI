using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class TooltipShowCasePageTests
{
    [Fact]
    public void Tooltip_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tooltip/Views/TooltipShowCase.axaml");

        source.ShouldContain("TooltipShowCaseLangResource PageSubtitle");
        source.ShouldContain("TooltipShowCaseLangResource PageDescription");
        source.ShouldNotContain("TooltipShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("TooltipShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("TooltipShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("TooltipShowCaseLangResource ComponentCategory");
        source.ShouldContain("TooltipShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("TooltipShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("TooltipShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("TooltipShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryShowCaseHost");
        source.ShouldNotContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
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
        source.ShouldContain("Description=\"{gallery:TooltipShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("TooltipShowCaseLangResource BasicTitle");
        source.ShouldContain("TooltipShowCaseLangResource PlacementTitle");
        source.ShouldContain("TooltipShowCaseLangResource ArrowTitle");
        source.ShouldContain("TooltipShowCaseLangResource ColorfulTooltipTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Tooltip_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tooltip/Views/TooltipShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/TooltipShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractTooltipExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    [Fact]
    public void Tooltip_ShowCase_Exposes_Semantic_Parts_Tab()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tooltip/Views/TooltipShowCase.axaml");

        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:ToolTip}\"");
        source.ShouldContain("SemanticOwner=\"{Binding #TooltipSemanticOwner}\"");
        foreach (var partPath in new[]
                 {
                     "root", "container", "arrow"
                 })
        {
            source.ShouldContain($"Path=\"{partPath}\"");
        }

        source.ShouldContain("SemanticRootDescription");
        source.ShouldContain("SemanticContainerDescription");
        source.ShouldContain("SemanticArrowDescription");
        source.ShouldContain("SemanticPartStyleTitle");
        source.ShouldContain("SemanticPartStyleDescription");

        var lowered = source.ToLowerInvariant();
        lowered.ShouldNotContain("semantic dom");
        lowered.ShouldNotContain("classnames");
    }

    [Fact]
    public void Tooltip_ShowCase_Semantic_Preview_Content_Hugs_Its_Content_Width()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Tooltip/Views/TooltipShowCase.axaml");

        // 预览舞台以 Stretch 测量内容，ToolTip 若不声明非 Stretch 的对齐会被
        // 撑到 ToolTipMaxWidth(250) 满宽，胶囊宽度远超 "prompt text" 所需。
        const string ownerStart = "<atom:ToolTip Name=\"TooltipSemanticOwner\"";
        var ownerStartIndex = source.IndexOf(ownerStart, StringComparison.Ordinal);
        ownerStartIndex.ShouldBeGreaterThanOrEqualTo(0);

        var ownerEndIndex = source.IndexOf("/>", ownerStartIndex, StringComparison.Ordinal);
        ownerEndIndex.ShouldBeGreaterThan(ownerStartIndex);

        var ownerElement = source[ownerStartIndex..ownerEndIndex];
        ownerElement.ShouldContain("HorizontalAlignment=\"Center\"");
    }

    private static string ExtractTooltipExampleItems(string source)
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
