using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class AutoCompleteShowCasePageTests
{
    [Fact]
    public void AutoComplete_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteShowCase.axaml");

        source.ShouldContain("AutoCompleteShowCaseLangResource PageSubtitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource PageDescription");
        source.ShouldContain("AutoCompleteShowCaseLangResource ComponentCategory");
        source.ShouldContain("AutoCompleteShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("AutoCompleteShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("AutoCompleteShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("AutoCompleteShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("<gallery:GalleryShowCaseHeader");
        source.ShouldContain("Title=\"AutoComplete\"");
        source.ShouldContain("Category=\"{gallery:AutoCompleteShowCaseLangResource ComponentCategory}\"");
        source.ShouldContain("Status=\"{gallery:AutoCompleteShowCaseLangResource ComponentStatusStable}\"");
        source.ShouldContain("Subtitle=\"{gallery:AutoCompleteShowCaseLangResource PageSubtitle}\"");
        source.ShouldContain("Description=\"{gallery:AutoCompleteShowCaseLangResource PageDescription}\"");
        source.ShouldContain("Namespace=\"AtomUI.Desktop.Controls\"");
        source.ShouldContain("Package=\"AtomUI.Desktop.Controls\"");
        source.ShouldContain("BaseClass=\"AbstractAutoComplete\"");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        source.ShouldNotContain("Classes=\"info-label\"");
        source.ShouldNotContain("Classes=\"info-value\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("AutoCompleteShowCaseLangResource BasicUsageTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource CustomOptionRenderingTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource SizeTypeTitle");
        source.ShouldContain("PlaceholderText=\"{gallery:AutoCompleteShowCaseLangResource P2PlaceholderSizeTypeLarge}\"");
        source.ShouldContain("PlaceholderText=\"{gallery:AutoCompleteShowCaseLangResource P2PlaceholderSizeTypeMiddle}\"");
        source.ShouldContain("PlaceholderText=\"{gallery:AutoCompleteShowCaseLangResource P2PlaceholderSizeTypeSmall}\"");
        source.ShouldContain("PlaceholderText=\"{gallery:AutoCompleteShowCaseLangResource P2PlaceholderSizeTypeCustom}\"");
        source.ShouldContain("SizeType=\"Large\"");
        source.ShouldContain("SizeType=\"Middle\"");
        source.ShouldContain("SizeType=\"Small\"");
        source.ShouldContain("SizeType=\"Custom\"");
        source.ShouldContain("Height=\"36\"");
        source.ShouldContain("AutoCompleteShowCaseLangResource TextAreaAutoCompletionTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource CustomizeClearButtonTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void AutoComplete_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/AutoCompleteShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractAutoCompleteExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractAutoCompleteExampleItems(string source)
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
