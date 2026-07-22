using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AtomUIGallery.ShowCases.Steps;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class StepsShowCasePageTests
{
    [Fact]
    public void Steps_ShowCase_Uses_Document_Layout_With_Examples()
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
        CountShowCaseItemElements(source).ShouldBe(15);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(15);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(15);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:StepsViewModel\"").ShouldBe(15);
        source.ShouldContain("StepsShowCaseLangResource BasicTitle");
        source.ShouldContain("StepsShowCaseLangResource SwitchStepTitle");
        source.ShouldContain("StepsShowCaseLangResource P2TextCurrent");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("StepsShowCaseLangResource NavigationStepsTitle");
        source.ShouldContain("StepsShowCaseLangResource InlineStepsTitle");
        source.ShouldContain("StepsShowCaseLangResource InlineStyleCombinationTitle");
        source.ShouldContain("SourceKey=\"steps-inline-style-combination\"");
        source.ShouldContain("Offset=\"2\"");
        source.ShouldContain("StepsShowCaseLangResource P2SubHeaderSubTitle");
        source.ShouldContain("StepsShowCaseLangResource P2HeaderStepN5");
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
        pageSource.ShouldContain("Text=\"{Binding InteractivePageContent}\"");
        pageSource.ShouldContain("Type=\"Dot\"");
        pageSource.ShouldContain("Type=\"OutlineDot\"");
        pageSource.ShouldContain("Type=\"Navigation\"");
        pageSource.ShouldContain("Type=\"Inline\"");
        pageSource.ShouldContain("TitlePlacement=\"Vertical\"");
        pageSource.ShouldContain("Percent=\"60\"");
        pageSource.ShouldContain("Offset=\"2\"");
        CountOccurrences(pageSource, "SourceKey=\"").ShouldBe(15);

        codeBehindSource.ShouldContain("HandleCurrentChangeRequested");
        codeBehindSource.ShouldContain("viewModel.Current = args.Current");
        codeBehindSource.ShouldNotContain("CurrentContentProperty");
        codeBehindSource.ShouldNotContain("CurrentContentTemplateProperty");
        codeBehindSource.ShouldNotContain("FindDescendantByName");
        codeBehindSource.ShouldNotContain("HandleInteractiveStepsLoaded");

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
    public void Steps_ShowCase_Controlled_Content_Uses_TextBlock_Instead_Of_ContentPresenter()
    {
        var pageSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/Steps/Views/StepsShowCase.axaml");

        pageSource.ShouldContain("<atom:TextBlock Text=\"{Binding InteractivePageContent}\"");
        pageSource.ShouldNotContain("<ContentPresenter Content=\"{Binding InteractivePageContent}\"");
    }

    [Fact]
    public void Steps_ShowCase_Interactive_Content_Is_Not_Empty_For_Each_Controlled_Step()
    {
        var viewModel = new StepsViewModel(null!);

        viewModel.Current = 0;
        viewModel.InteractivePageContent.ShouldBe("First-content");

        viewModel.Current = 1;
        viewModel.InteractivePageContent.ShouldBe("Second-content");

        viewModel.Current = 2;
        viewModel.InteractivePageContent.ShouldBe("Last-content");
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
