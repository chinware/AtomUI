using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class NumberUpDownShowCasePageTests
{
    [Fact]
    public void NumberUpDown_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownShowCase.axaml");

        source.ShouldContain("NumberUpDownShowCaseLangResource PageSubtitle");
        source.ShouldContain("NumberUpDownShowCaseLangResource PageDescription");
        source.ShouldContain("NumberUpDownShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("NumberUpDownShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("NumberUpDownShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("NumberUpDownShowCaseLangResource ComponentCategory");
        source.ShouldContain("NumberUpDownShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("NumberUpDownShowCaseLangResource ScenarioExamples");
        source.ShouldContain("NumberUpDownShowCaseLangResource ScenarioApi");
        source.ShouldContain("NumberUpDownShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Selector=\"atom|NumericUpDown\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(3);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(3);
        source.ShouldContain("LineHeight=\"22\"");
        source.ShouldContain("Text=\"{gallery:NumberUpDownShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(14);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(14);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:NumberUpDownViewModel\"").ShouldBe(14);
        source.ShouldContain("NumberUpDownShowCaseLangResource BasicUsageTitle");
        source.ShouldContain("NumberUpDownShowCaseLangResource SpinnerModeTitle");
        source.ShouldContain("Title=\"{gallery:NumberUpDownShowCaseLangResource SpinnerModeTitle}\"\n            BadgeText=\"v6.0.5\"");
        CountOccurrences(source, "BadgeText=\"v6.0.5\"").ShouldBe(1);
        source.ShouldContain("Spacing=\"{atom:SharedTokenResource UniformlyMargin}\"");
        source.ShouldContain("Mode=\"Spinner\"");
        source.ShouldContain("FormatString=\"0\"");
        source.ShouldContain("PlaceholderText=\"Outlined\"");
        source.ShouldContain("PlaceholderText=\"Filled\"");
        source.ShouldContain("StyleVariant=\"Filled\"");
        source.ShouldContain("NumberUpDownShowCaseLangResource StringModeTitle");
        source.ShouldContain("NumberUpDownShowCaseLangResource DecimalStepTitle");
        source.ShouldContain("NumberUpDownShowCaseLangResource StatusTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void NumberUpDown_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownDesignTokenDataGrid.axaml.cs");

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
        codeBehindSource.ShouldContain("new NumberUpDownApiDataGrid()");
        codeBehindSource.ShouldContain("new NumberUpDownDesignTokenDataGrid()");
        codeBehindSource.ShouldNotContain("new NumberUpDownBasicShowCase()");
        codeBehindSource.ShouldNotContain("new NumberUpDownRangeShowCase()");
        codeBehindSource.ShouldNotContain("new NumberUpDownStyleShowCase()");
        codeBehindSource.ShouldNotContain("new NumberUpDownAddonShowCase()");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:NumberUpDownApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("NumberUpDownShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("NumberUpDownShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("NumberUpDownShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("NumberUpDownShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:NumberUpDownDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("NumberUpDownShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("NumberUpDownShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("NumberUpDownShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("NumberUpDownShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void NumberUpDown_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyValue");
            source.ShouldContain("ApiPropertyStringValue");
            source.ShouldContain("ApiPropertyIsStringMode");
            source.ShouldContain("ApiPropertyIsAllowClear");
            source.ShouldContain("ApiPropertyIsKeyboardEnabled");
            source.ShouldContain("SpinnerModeTitle");
            source.ShouldContain("TokenNameControlWidth");
            source.ShouldContain("TokenNameHandleWidth");
            source.ShouldContain("TokenNameHandleIconSize");
        }
    }

    [Fact]
    public void NumberUpDown_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/NumberUpDown/Views/NumberUpDownShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/NumberUpDownShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractNumberUpDownExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractNumberUpDownExampleItems(string source)
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
