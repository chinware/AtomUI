using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ListShowCasePageTests
{
    [Fact]
    public void List_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/Views/ListShowCase.axaml");

        source.ShouldContain("ListShowCaseLangResource PageSubtitle");
        source.ShouldContain("ListShowCaseLangResource PageDescription");
        source.ShouldNotContain("ListShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("ListShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("ListShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("ListShowCaseLangResource ComponentCategory");
        source.ShouldContain("ListShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("ListShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("ListShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("ListShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:ListShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("ListShowCaseLangResource BasicUsageTitle");
        source.ShouldContain("ListShowCaseLangResource SelectionTitle");
        source.ShouldContain("ListShowCaseLangResource SelectedItemsBindingTitle");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("ListShowCaseLangResource FilterTitle");
        source.ShouldContain("ListShowCaseLangResource SearchableTitle");
        source.ShouldContain("ListShowCaseLangResource PaginationListTitle");
        source.ShouldContain("ListShowCaseLangResource ListViewEmptyIndicatorTitle");
        source.ShouldContain("ListShowCaseLangResource ListBoxEmptyIndicatorTitle");
        CountOccurrences(source, "<atom:ListView.EmptyIndicator>").ShouldBe(1);
        CountOccurrences(source, "<atom:ListBox.EmptyIndicator>").ShouldBe(1);
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void List_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataDisplay/List/Views/ListShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/ListShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractListExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractListExampleItems(string source)
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
