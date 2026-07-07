using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ButtonSpinnerShowCasePageTests
{
    [Fact]
    public void ButtonSpinner_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Views/ButtonSpinnerShowCase.axaml");

        source.ShouldContain("ButtonSpinnerShowCaseLangResource PageSubtitle");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource PageDescription");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource ComponentCategory");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("ButtonSpinnerShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:ButtonSpinnerShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(7);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(7);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(7);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:ButtonSpinnerViewModel\"").ShouldBe(7);
        source.ShouldContain("ButtonSpinnerShowCaseLangResource BasicTitle");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource ThreeSizesTitle");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource P2LabelSizeTypeLarge");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource P2LabelSizeTypeMiddle");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource P2LabelSizeTypeSmall");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource P2LabelSizeTypeCustom");
        source.ShouldContain("SizeType=\"Custom\"");
        source.ShouldContain("ButtonSpinnerShowCaseLangResource StatusTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void ButtonSpinner_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Views/ButtonSpinnerShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Views/ButtonSpinnerShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Views/ButtonSpinnerApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Views/ButtonSpinnerApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Views/ButtonSpinnerDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Views/ButtonSpinnerDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldNotContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldNotContain("Tag=\"Api\"");
        pageSource.ShouldNotContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        codeBehindSource.ShouldNotContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldNotContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldNotContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldNotContain("new ButtonSpinnerApiDataGrid()");
        codeBehindSource.ShouldNotContain("new ButtonSpinnerDesignTokenDataGrid()");
        codeBehindSource.ShouldContain("AddHandler(Spinner.SpinEvent, HandleSpin)");
        codeBehindSource.ShouldContain("HandleSpin(object? sender, SpinEventArgs args)");
        codeBehindSource.ShouldNotContain("GetVisualChildren");
        codeBehindSource.ShouldNotContain("BindSpinHandleRecursively");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:ButtonSpinnerApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("ButtonSpinnerShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("ButtonSpinnerShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("ButtonSpinnerShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("ButtonSpinnerShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:ButtonSpinnerDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("ButtonSpinnerShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("ButtonSpinnerShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("ButtonSpinnerShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("ButtonSpinnerShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void ButtonSpinner_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("P2LabelSizeTypeLarge");
            source.ShouldContain("P2LabelSizeTypeMiddle");
            source.ShouldContain("P2LabelSizeTypeSmall");
            source.ShouldContain("P2LabelSizeTypeCustom");
            source.ShouldContain("ApiPropertyIsSpinEnabled");
            source.ShouldContain("ApiPropertyButtonSpinnerLocation");
            source.ShouldContain("ApiPropertyInnerLeftContent");
            source.ShouldContain("ApiPropertySpin");
            source.ShouldContain("TokenNameControlWidth");
            source.ShouldContain("TokenNameHandleWidth");
            source.ShouldContain("TokenNameHandleIconSize");
        }
    }

    [Fact]
    public void ButtonSpinner_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Navigation/ButtonSpinner/Views/ButtonSpinnerShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/ButtonSpinnerShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractButtonSpinnerExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractButtonSpinnerExampleItems(string source)
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
