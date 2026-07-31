using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ToggleSwitchShowCasePageTests
{
    [Fact]
    public void ToggleSwitch_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchShowCase.axaml");

        source.ShouldContain("ToggleSwitchShowCaseLangResource PageSubtitle");
        source.ShouldContain("ToggleSwitchShowCaseLangResource PageDescription");
        source.ShouldNotContain("ToggleSwitchShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("ToggleSwitchShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("ToggleSwitchShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("ToggleSwitchShowCaseLangResource ComponentCategory");
        source.ShouldContain("ToggleSwitchShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("ToggleSwitchShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("ToggleSwitchShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("ToggleSwitchShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:ToggleSwitchShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("ToggleSwitchShowCaseLangResource BasicTitle");
        source.ShouldContain("ToggleSwitchShowCaseLangResource DisabledTitle");
        source.ShouldContain("ToggleSwitchShowCaseLangResource TextAndIconTitle");
        source.ShouldContain("ToggleSwitchShowCaseLangResource TwoSizesTitle");
        source.ShouldContain("ToggleSwitchShowCaseLangResource LoadingTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void ToggleSwitch_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/ToggleSwitchShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractToggleSwitchExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    [Fact]
    public void ToggleSwitch_ShowCase_Toggle_Action_Buttons_Do_Not_Disable_Themselves_During_Click()
    {
        var source          = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchShowCase.axaml");
        var viewModelSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/ViewModels/ToggleSwitchViewModel.cs");

        source.ShouldContain("Click=\"HandleToggleDisabledButtonClick\"");
        source.ShouldContain("Click=\"HandleToggleLoadingButtonClick\"");
        source.ShouldNotContain("Command=\"{Binding ToggleDisabledCommand}\"");
        source.ShouldNotContain("Command=\"{Binding ToggleLoadingCommand}\"");

        viewModelSource.ShouldNotContain("ToggleDisabledCommand");
        viewModelSource.ShouldNotContain("ToggleLoadingCommand");
    }

    [Fact]
    public void ToggleSwitch_ShowCase_Includes_Custom_SizeType_Example()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchShowCase.axaml");

        source.ShouldContain("Name=\"CustomSizeTypeToggleSwitch\"");
        source.ShouldContain("SizeType=\"Custom\"");
        source.ShouldContain("P2ContentCustom");
    }

    private static string ExtractToggleSwitchExampleItems(string source)
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
