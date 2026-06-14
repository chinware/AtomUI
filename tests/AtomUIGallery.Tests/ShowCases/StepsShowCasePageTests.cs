using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class StepsShowCasePageTests
{
    [Fact]
    public void Steps_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml");

        source.ShouldContain("StepsShowCaseLangResource PageSubtitle");
        source.ShouldContain("StepsShowCaseLangResource PageDescription");
        source.ShouldContain("StepsShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("StepsShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("StepsShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("StepsShowCaseLangResource ComponentCategory");
        source.ShouldContain("StepsShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("StepsShowCaseLangResource ScenarioExamples");
        source.ShouldContain("StepsShowCaseLangResource ScenarioApi");
        source.ShouldContain("StepsShowCaseLangResource ScenarioDesignToken");
        source.ShouldContain("Tag=\"Examples\"");
        source.ShouldContain("Tag=\"Api\"");
        source.ShouldContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("IsDeferredLoadingEnabled=\"True\"");
        source.ShouldContain("InitialDeferredLoadItemCount=\"4\"");
        source.ShouldContain("DeferredLoadBatchSize=\"2\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(3);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(3);
        source.ShouldContain("LineHeight=\"22\"");
        source.ShouldContain("Text=\"{gallery:StepsShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        CountShowCaseItemElements(source).ShouldBe(14);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(14);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(14);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:StepsViewModel\"").ShouldBe(14);
        source.ShouldContain("StepsShowCaseLangResource BasicTitle");
        source.ShouldContain("StepsShowCaseLangResource SwitchStepTitle");
        source.ShouldContain("StepsShowCaseLangResource NavigationStepsTitle");
        source.ShouldContain("StepsShowCaseLangResource InlineStepsTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Steps_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldContain("Tag=\"Api\"");
        pageSource.ShouldContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");
        pageSource.ShouldContain("Click=\"HandleNextButtonClick\"");
        pageSource.ShouldContain("Click=\"HandlePreviousButtonClick\"");

        codeBehindSource.ShouldContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldContain("ExamplesContent");
        codeBehindSource.ShouldContain("new StepsApiDataGrid()");
        codeBehindSource.ShouldContain("new StepsDesignTokenDataGrid()");
        codeBehindSource.ShouldContain("HandleNextButtonClick");
        codeBehindSource.ShouldContain("HandlePreviousButtonClick");
        codeBehindSource.ShouldNotContain("new StepsBasicShowCase()");
        codeBehindSource.ShouldNotContain("new StepsInteractiveShowCase()");
        codeBehindSource.ShouldNotContain("new StepsVerticalShowCase()");
        codeBehindSource.ShouldNotContain("new StepsDotClickableShowCase()");
        codeBehindSource.ShouldNotContain("new StepsNavigationShowCase()");
        codeBehindSource.ShouldNotContain("new StepsProgressShowCase()");
        codeBehindSource.ShouldNotContain("new StepsInlineShowCase()");
        codeBehindSource.ShouldNotContain("NextStepButton.Click +=");
        codeBehindSource.ShouldNotContain("PreviousButton.Click +=");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:StepsApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("StepsShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("StepsShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("StepsShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("StepsShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:StepsDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("StepsShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("StepsShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("StepsShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("StepsShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void Steps_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyCurrentStep");
            source.ShouldContain("ApiPropertyProgressValue");
            source.ShouldContain("ApiPropertyItemIndicatorType");
            source.ShouldContain("ApiPropertyIsItemClickable");
            source.ShouldContain("TokenNameDescriptionMaxWidth");
            source.ShouldContain("TokenNameIconSize");
            source.ShouldContain("TokenNameDotSize");
            source.ShouldContain("TokenNameStepsNavActiveColor");
            source.ShouldContain("TokenNameInlineItemPadding");
        }
    }

    [Fact]
    public void Steps_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/StepsShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractStepsExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractStepsExampleItems(string source)
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
        return ShowCaseSnapshotMarkup.Normalize(StripStepsBehaviorMarkup(source));
    }

    private static string StripStepsBehaviorMarkup(string source)
    {
        var normalized = Regex.Replace(
            source,
            @"\s+Click=""Handle(Next|Previous)ButtonClick""",
            string.Empty,
            RegexOptions.CultureInvariant);

        return Regex.Replace(
            normalized,
            @"\s+Content=""\{Binding NextButtonText\}""",
            " Content=\"{gallery:StepsShowCaseLangResource P2ContentNext}\"",
            RegexOptions.CultureInvariant);
    }

    private static string ComputeSha256(string source)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(source));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    private static string ReadSnapshotHash(string source)
    {
        return source
            .Split('\n')
            .First(line => line.StartsWith("sha256:", StringComparison.Ordinal))
            .Split(':', 2)[1]
            .Trim();
    }

    private static int ReadSnapshotCount(string source)
    {
        return int.Parse(source
            .Split('\n')
            .First(line => line.StartsWith("count:", StringComparison.Ordinal))
            .Split(':', 2)[1]
            .Trim());
    }

    private static int CountShowCaseItemElements(string source)
    {
        return Regex.Matches(source, @"<gallery:ShowCaseItem(\s|>)", RegexOptions.CultureInvariant).Count;
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
