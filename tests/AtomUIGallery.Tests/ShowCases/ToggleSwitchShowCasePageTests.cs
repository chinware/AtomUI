using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ToggleSwitchShowCasePageTests
{
    [Fact]
    public void ToggleSwitch_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchShowCase.axaml");

        source.ShouldContain("ToggleSwitchShowCaseLangResource PageSubtitle");
        source.ShouldContain("ToggleSwitchShowCaseLangResource PageDescription");
        source.ShouldContain("ToggleSwitchShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("ToggleSwitchShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("ToggleSwitchShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("ToggleSwitchShowCaseLangResource ComponentCategory");
        source.ShouldContain("ToggleSwitchShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("ToggleSwitchShowCaseLangResource ScenarioExamples");
        source.ShouldContain("ToggleSwitchShowCaseLangResource ScenarioApi");
        source.ShouldContain("ToggleSwitchShowCaseLangResource ScenarioDesignToken");
        source.ShouldContain("Tag=\"Examples\"");
        source.ShouldContain("Tag=\"Api\"");
        source.ShouldContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(3);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(3);
        source.ShouldContain("LineHeight=\"22\"");
        source.ShouldContain("Text=\"{gallery:ToggleSwitchShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
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
    public void ToggleSwitch_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Views/ToggleSwitchDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldContain("Tag=\"Api\"");
        pageSource.ShouldContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        codeBehindSource.ShouldContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldContain("ExamplesContent");
        codeBehindSource.ShouldContain("new ToggleSwitchApiDataGrid()");
        codeBehindSource.ShouldContain("new ToggleSwitchDesignTokenDataGrid()");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:ToggleSwitchApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("ToggleSwitchShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("ToggleSwitchShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("ToggleSwitchShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("ToggleSwitchShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:ToggleSwitchDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("ToggleSwitchShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("ToggleSwitchShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("ToggleSwitchShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("ToggleSwitchShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void ToggleSwitch_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/ToggleSwitch/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyOnContent");
            source.ShouldContain("ApiPropertyOffContent");
            source.ShouldContain("ApiPropertySizeType");
            source.ShouldContain("ApiPropertyIsLoading");
            source.ShouldContain("P2ContentCustom");
            source.ShouldContain("TokenNameTrackHeight");
            source.ShouldContain("TokenNameSwitchColor");
        }
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
