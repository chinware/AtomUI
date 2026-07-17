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
        source.ShouldNotContain("StepsShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("StepsShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("StepsShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("StepsShowCaseLangResource ComponentCategory");
        source.ShouldContain("StepsShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("StepsShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("StepsShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("StepsShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
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
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:StepsShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(14);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(14);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(14);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:StepsViewModel\"").ShouldBe(14);
        source.ShouldContain("StepsShowCaseLangResource BasicTitle");
        source.ShouldContain("StepsShowCaseLangResource SwitchStepTitle");
        source.ShouldContain("StepsShowCaseLangResource P2TextCurrent");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("StepsShowCaseLangResource NavigationStepsTitle");
        source.ShouldContain("StepsShowCaseLangResource InlineStepsTitle");
        source.ShouldNotContain("{Binding #");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Steps_ShowCase_Uses_Redesigned_Control_Contract()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml.cs");
        var viewModelSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/ViewModels/StepsViewModel.cs");
        var combinedSource   = string.Join('\n', pageSource, codeBehindSource, viewModelSource);

        pageSource.ShouldContain("BaseClass=\"ItemsControl\"");
        pageSource.ShouldContain("Current=\"{Binding Current}\"");
        pageSource.ShouldContain("CurrentChangeRequested=\"HandleCurrentChangeRequested\"");
        pageSource.ShouldContain("Content=\"{Binding InteractivePageContent}\"");
        pageSource.ShouldContain("Type=\"Dot\"");
        pageSource.ShouldContain("Type=\"Navigation\"");
        pageSource.ShouldContain("Type=\"Inline\"");
        pageSource.ShouldContain("TitlePlacement=\"Vertical\"");
        pageSource.ShouldContain("Percent=\"60\"");
        CountOccurrences(pageSource, "SourceKey=\"").ShouldBe(14);

        codeBehindSource.ShouldContain("HandleCurrentChangeRequested");
        codeBehindSource.ShouldContain("viewModel.Current = args.Current");
        codeBehindSource.ShouldNotContain("CurrentContentProperty");
        codeBehindSource.ShouldNotContain("CurrentContentTemplateProperty");
        codeBehindSource.ShouldNotContain("FindDescendantByName");
        codeBehindSource.ShouldNotContain("HandleInteractiveStepsLoaded");

        viewModelSource.ShouldContain("Steps.Current\"");
        viewModelSource.ShouldContain("Steps.Initial\"");
        viewModelSource.ShouldContain("Steps.Status\"");
        viewModelSource.ShouldContain("Steps.Percent\"");
        viewModelSource.ShouldContain("Steps.Type\"");
        viewModelSource.ShouldContain("Steps.TitlePlacement\"");
        viewModelSource.ShouldContain("Steps.CurrentChangeRequested\"");
        viewModelSource.ShouldContain("StepsItem.Content\"");
        viewModelSource.ShouldContain("StepsItem.ContentTemplate\"");
        viewModelSource.ShouldContain("StepsItem.Status\"");
        viewModelSource.ShouldContain("\"StepsStatus?\"");
        viewModelSource.ShouldContain("InteractivePageContent");

        foreach (var removedApi in new[]
                 {
                     "CurrentStep", "InitialStep", "CurrentStepStatus", "ProgressValue",
                     "IsShowItemProgress", "ItemIndicatorType", "StepsStyle", "LabelPlacement",
                     "CurrentContent", "StepsItem.Description", "SelectingItemsControl"
                 })
        {
            combinedSource.ShouldNotContain(removedApi);
        }

        Regex.IsMatch(pageSource, @"<atom:StepsItem\b[^>]*\bDescription=").ShouldBeFalse();
        pageSource.ShouldNotContain("Style=\"Navigation\"");
        pageSource.ShouldNotContain("Style=\"Inline\"");
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

        pageSource.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldNotContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldNotContain("Tag=\"Api\"");
        pageSource.ShouldNotContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");
        pageSource.ShouldContain("Click=\"HandleNextButtonClick\"");
        pageSource.ShouldContain("Click=\"HandlePreviousButtonClick\"");
        pageSource.ShouldContain("CurrentChangeRequested=\"HandleCurrentChangeRequested\"");

        codeBehindSource.ShouldNotContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldNotContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldNotContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldNotContain("new StepsApiDataGrid()");
        codeBehindSource.ShouldNotContain("new StepsDesignTokenDataGrid()");
        codeBehindSource.ShouldContain("HandleNextButtonClick");
        codeBehindSource.ShouldContain("HandlePreviousButtonClick");
        codeBehindSource.ShouldContain("HandleCurrentChangeRequested");
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
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyCurrent");
            source.ShouldContain("P2TextCurrent");
            source.ShouldContain("ApiPropertyInitial");
            source.ShouldContain("ApiPropertyStatus");
            source.ShouldContain("ApiPropertyPercent");
            source.ShouldContain("ApiPropertyType");
            source.ShouldContain("ApiPropertyTitlePlacement");
            source.ShouldContain("ApiPropertyIsItemClickable");
            source.ShouldContain("ApiPropertyIsMotionEnabled");
            source.ShouldContain("ApiEventCurrentChangeRequested");
            source.ShouldContain("ApiPropertyStepsItemContent");
            source.ShouldContain("ApiPropertyStepsItemContentTemplate");
            source.ShouldContain("TokenNameDescriptionMaxWidth");
            source.ShouldContain("TokenNameIconSize");
            source.ShouldContain("TokenNameDotSize");
            source.ShouldContain("TokenNameStepsNavActiveColor");
            source.ShouldContain("TokenNameInlineItemPadding");
            source.ShouldNotContain("ApiPropertyCurrentStep");
            source.ShouldNotContain("ApiPropertyProgressValue");
            source.ShouldNotContain("ApiPropertyItemIndicatorType");
            source.ShouldNotContain("TokenNameStepsProgressSize");
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
