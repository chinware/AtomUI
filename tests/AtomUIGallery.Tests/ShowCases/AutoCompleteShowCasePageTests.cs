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
        source.ShouldNotContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("<gallery:GalleryShowCaseHost");
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
        source.ShouldContain("<gallery:GalleryShowCaseHost.SemanticPartsContentTemplate>");
        source.ShouldContain("<gallery:SemanticPartPreview ");
        source.ShouldContain("SemanticOwnerType=\"{x:Type atom:AutoComplete}\"");
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
        source.ShouldNotContain("Classes=\"info-label\"");
        source.ShouldNotContain("Classes=\"info-value\"");
        CountShowCaseItemElements(source).ShouldBe(10);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(10);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(10);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:AutoCompleteViewModel\"").ShouldBe(11);
        source.ShouldContain("AutoCompleteShowCaseLangResource BasicTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource CustomOptionsTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource CustomInputTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource NonCaseSensitiveTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource CertainCategoryTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource UncertainCategoryTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource StatusTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource VariantTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource AllowClearTitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource StyleClassTitle");
        source.ShouldContain("SourceKey=\"auto-complete-semantic-part\"");
        source.ShouldContain("StyleVariant=\"Outlined\"");
        source.ShouldContain("StyleVariant=\"Filled\"");
        source.ShouldContain("StyleVariant=\"Borderless\"");
        source.ShouldContain("StyleVariant=\"Underlined\"");
        source.ShouldContain("Status=\"Error\"");
        source.ShouldContain("Status=\"Warning\"");
        source.ShouldContain("<atom:AutoCompleteTextArea ");
        source.ShouldContain("<atom:AutoCompleteSearchEdit ");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
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

private static int CountShowCaseItemElements(string source)
    {
        return CountOccurrences(source, "<gallery:ShowCaseItem ");
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
