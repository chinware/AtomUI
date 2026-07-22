using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class BadgeShowCasePageTests
{
    [Fact]
    public void Badge_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml");

        source.ShouldContain("BadgeShowCaseLangResource PageSubtitle");
        source.ShouldContain("BadgeShowCaseLangResource PageDescription");
        source.ShouldNotContain("BadgeShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("BadgeShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("BadgeShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("BadgeShowCaseLangResource ComponentCategory");
        source.ShouldContain("BadgeShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("BadgeShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("BadgeShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("BadgeShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,28,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:BadgeShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("BadgeShowCaseLangResource BasicTitle");
        source.ShouldContain("BadgeShowCaseLangResource DynamicTitle");
        source.ShouldContain("BadgeShowCaseLangResource RibbonTitle");
        source.ShouldContain("BadgeShowCaseLangResource ColorfulBadgeTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Badge_ShowCase_Reserves_Top_Space_For_Adorner_Badges()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml");

        source.ShouldContain("ContentMargin=\"28,28,28,28\"");
        source.ShouldNotContain("ContentMargin=\"28,10,28,28\"");
    }

    [Fact]
    public void Badge_Ribbon_Examples_Use_Right_Placement()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml");

        ExtractBadgeExampleItems(source).ShouldNotContain("Placement=\"Start\"");
    }

    [Fact]
    public void Badge_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/Badge/Views/BadgeShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/BadgeShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractBadgeExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractBadgeExampleItems(string source)
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
