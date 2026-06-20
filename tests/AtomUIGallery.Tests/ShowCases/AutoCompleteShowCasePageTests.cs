using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class AutoCompleteShowCasePageTests
{
    [Fact]
    public void AutoComplete_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteShowCase.axaml");

        source.ShouldContain("AutoCompleteShowCaseLangResource PageSubtitle");
        source.ShouldContain("AutoCompleteShowCaseLangResource PageDescription");
        source.ShouldContain("AutoCompleteShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("AutoCompleteShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("AutoCompleteShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("AutoCompleteShowCaseLangResource ComponentCategory");
        source.ShouldContain("AutoCompleteShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("AutoCompleteShowCaseLangResource ScenarioExamples");
        source.ShouldContain("AutoCompleteShowCaseLangResource ScenarioApi");
        source.ShouldContain("AutoCompleteShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Text=\"{gallery:AutoCompleteShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
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
    public void AutoComplete_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Views/AutoCompleteDesignTokenDataGrid.axaml.cs");

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
        codeBehindSource.ShouldContain("new AutoCompleteApiDataGrid()");
        codeBehindSource.ShouldContain("new AutoCompleteDesignTokenDataGrid()");
        codeBehindSource.ShouldNotContain("GalleryBindingUtils.OneWay");
        pageSource.ShouldContain("Name=\"BasicAutoComplete\"");
        pageSource.ShouldContain("OptionsAsyncLoader=\"{Binding BasicOptionsAsyncLoader}\"");
        pageSource.ShouldContain("OptionsAsyncLoader=\"{Binding CustomLabelOptionsAsyncLoader}\"");
        pageSource.ShouldContain("OptionsAsyncLoader=\"{Binding SearchEditOptionsAsyncLoader}\"");
        pageSource.ShouldContain("OptionsSource=\"{Binding FilterCaseOptions}\"");
        pageSource.ShouldContain("OptionsSource=\"{Binding CityOptions}\"");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:AutoCompleteApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("AutoCompleteShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("AutoCompleteShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("AutoCompleteShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("AutoCompleteShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:AutoCompleteDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("AutoCompleteShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("AutoCompleteShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("AutoCompleteShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("AutoCompleteShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void AutoComplete_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/AutoComplete/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("SizeTypeTitle");
            source.ShouldContain("SizeTypeDescription");
            source.ShouldContain("P2PlaceholderSizeTypeLarge");
            source.ShouldContain("P2PlaceholderSizeTypeMiddle");
            source.ShouldContain("P2PlaceholderSizeTypeSmall");
            source.ShouldContain("P2PlaceholderSizeTypeCustom");
            source.ShouldContain("ApiPropertyOptionsSource");
            source.ShouldContain("ApiPropertyOptionsAsyncLoader");
            source.ShouldContain("ApiPropertyOptionTemplate");
            source.ShouldContain("ApiPropertyIsAllowClear");
            source.ShouldContain("ApiPropertyAutoCompleteTextAreaLines");
            source.ShouldContain("TokenNamePopupContentPadding");
            source.ShouldContain("TokenNameOptionHeight");
        }
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
