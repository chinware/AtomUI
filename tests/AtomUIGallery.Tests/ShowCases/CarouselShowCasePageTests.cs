using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class CarouselShowCasePageTests
{
    [Fact]
    public void Carousel_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Carousel/Views/CarouselShowCase.axaml");

        source.ShouldContain("CarouselShowCaseLangResource PageSubtitle");
        source.ShouldContain("CarouselShowCaseLangResource PageDescription");
        source.ShouldNotContain("CarouselShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("CarouselShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("CarouselShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("CarouselShowCaseLangResource ComponentCategory");
        source.ShouldContain("CarouselShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("CarouselShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("CarouselShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("CarouselShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:CarouselShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("CarouselShowCaseLangResource BasicTitle");
        source.ShouldContain("CarouselShowCaseLangResource CardShapePositionTitle");
        source.ShouldContain("CarouselShowCaseLangResource AutoScrollTitle");
        source.ShouldContain("CarouselShowCaseLangResource FadeInTitle");
        source.ShouldContain("CarouselShowCaseLangResource SwitchArrowsTitle");
        source.ShouldContain("CarouselShowCaseLangResource DotsProgressTitle");
        source.ShouldContain("Selector=\"atom|Carousel\"");
        source.ShouldContain("Selector=\"atom|CarouselPage\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Carousel_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Carousel/Views/CarouselShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/CarouselShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractCarouselExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractCarouselExampleItems(string source)
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
