using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ProgressBarShowCasePageTests
{
    [Fact]
    public void ProgressBar_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarShowCase.axaml");

        source.ShouldContain("ProgressBarShowCaseLangResource PageSubtitle");
        source.ShouldContain("ProgressBarShowCaseLangResource PageDescription");
        source.ShouldContain("ProgressBarShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("ProgressBarShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("ProgressBarShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("ProgressBarShowCaseLangResource ComponentCategory");
        source.ShouldContain("ProgressBarShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("ProgressBarShowCaseLangResource ScenarioExamples");
        source.ShouldContain("ProgressBarShowCaseLangResource ScenarioApi");
        source.ShouldContain("ProgressBarShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Selector=\"atom|CircleProgress\"");
        source.ShouldContain("Selector=\"atom|DashboardProgress\"");
        source.ShouldContain("Selector=\"#CircleWithStep atom|StepsProgressBar\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(3);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(3);
        source.ShouldContain("LineHeight=\"22\"");
        source.ShouldContain("Text=\"{gallery:ProgressBarShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(19);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(19);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:ProgressBarViewModel\"").ShouldBe(19);
        source.ShouldContain("ProgressBarShowCaseLangResource ProgressBarTitle");
        source.ShouldContain("ProgressBarShowCaseLangResource CustomTextFormatTitle");
        source.ShouldContain("ProgressBarShowCaseLangResource PercentPositionTitle");
        source.ShouldContain("ProgressBarShowCaseLangResource ToggleDisabledStatusTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void ProgressBar_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldContain("Tag=\"Api\"");
        pageSource.ShouldContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        codeBehindSource.ShouldContain("ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged");
        codeBehindSource.ShouldContain("EnsureSelectedScenarioContent");
        codeBehindSource.ShouldContain("ScenarioContentHost.Content = content");
        codeBehindSource.ShouldContain("ExamplesContent");
        codeBehindSource.ShouldContain("new ProgressBarApiDataGrid()");
        codeBehindSource.ShouldContain("new ProgressBarDesignTokenDataGrid()");
        codeBehindSource.ShouldNotContain("new ProgressBarBasicShowCase()");
        codeBehindSource.ShouldNotContain("new ProgressBarAdvancedShowCase()");
        codeBehindSource.ShouldNotContain("new ProgressBarLayoutShowCase()");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:ProgressBarApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("ProgressBarShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("ProgressBarShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("ProgressBarShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("ProgressBarShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:ProgressBarDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("ProgressBarShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("ProgressBarShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("ProgressBarShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("ProgressBarShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void ProgressBar_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyValue");
            source.ShouldContain("ApiPropertyStatus");
            source.ShouldContain("ApiPropertyPercentPosition");
            source.ShouldContain("ApiPropertyDashboardGapPosition");
            source.ShouldContain("TokenNameDefaultColor");
            source.ShouldContain("TokenNameLineBorderRadius");
            source.ShouldContain("TokenNameLineProgressPadding");
        }
    }

    [Fact]
    public void ProgressBar_ShowCase_Removes_Orphaned_SubShowCase_Files()
    {
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarBasicShowCase.axaml")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarBasicShowCase.axaml.cs")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarAdvancedShowCase.axaml")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarAdvancedShowCase.axaml.cs")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarLayoutShowCase.axaml")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarLayoutShowCase.axaml.cs")).ShouldBeFalse();
    }

    [Fact]
    public void ProgressBar_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/ProgressBar/Views/ProgressBarShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/ProgressBarShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractProgressBarExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractProgressBarExampleItems(string source)
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
