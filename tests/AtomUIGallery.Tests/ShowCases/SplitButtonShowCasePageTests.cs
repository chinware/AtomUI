using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class SplitButtonShowCasePageTests
{
    [Fact]
    public void SplitButton_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Views/SplitButtonShowCase.axaml");

        source.ShouldContain("SplitButtonShowCaseLangResource PageSubtitle");
        source.ShouldContain("SplitButtonShowCaseLangResource PageDescription");
        source.ShouldNotContain("SplitButtonShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("SplitButtonShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("SplitButtonShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("SplitButtonShowCaseLangResource ComponentCategory");
        source.ShouldContain("SplitButtonShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("SplitButtonShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("SplitButtonShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("SplitButtonShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:SplitButtonShowCaseLangResource PageDescription}\"");
        CountShowCaseItemElements(source).ShouldBe(5);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(5);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(5);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:SplitButtonViewModel\"").ShouldBe(5);
        CountOccurrences(source, "<gallery:ShowCaseItem.Styles>").ShouldBe(4);
        source.ShouldContain("SplitButtonShowCaseLangResource BasicTitle");
        source.ShouldContain("SplitButtonShowCaseLangResource SizeTitle");
        source.ShouldContain("SplitButtonShowCaseLangResource FlyoutTriggerTypeTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void SplitButton_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Views/SplitButtonShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Views/SplitButtonShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Views/SplitButtonApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Views/SplitButtonApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Views/SplitButtonDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Views/SplitButtonDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldNotContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldNotContain("Tag=\"Api\"");
        pageSource.ShouldNotContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");

        codeBehindSource.ShouldNotContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldNotContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldNotContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldNotContain("new SplitButtonApiDataGrid()");
        codeBehindSource.ShouldNotContain("new SplitButtonDesignTokenDataGrid()");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:SplitButtonApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("SplitButtonShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("SplitButtonShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("SplitButtonShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("SplitButtonShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:SplitButtonDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("SplitButtonShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("SplitButtonShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("SplitButtonShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("SplitButtonShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void SplitButton_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyCommand");
            source.ShouldContain("ApiPropertyFlyout");
            source.ShouldContain("ApiPropertyTriggerType");
            source.ShouldContain("ApiPropertyIsPrimaryButtonType");
            source.ShouldContain("P2ContentCustom");
            source.ShouldContain("TokenNameGutterToFlyout");
            source.ShouldContain("TokenNamePadding");
            source.ShouldContain("TokenNameIconSize");
        }
    }

    [Fact]
    public void SplitButton_Size_Example_Includes_Custom_Size_Demo()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Views/SplitButtonShowCase.axaml");
        var examples = ExtractSplitButtonExampleItems(source);

        var sizeItem = ExtractShowCaseItemByTitle(examples, "SplitButtonShowCaseLangResource SizeTitle");
        sizeItem.ShouldContain("SizeType=\"Custom\"");
        sizeItem.ShouldContain("Height=\"44\"");
        sizeItem.ShouldContain("Padding=\"18,0\"");
        sizeItem.ShouldContain("FontSize=\"15\"");
        sizeItem.ShouldContain("SplitButtonShowCaseLangResource P2ContentCustom");
    }

    [Fact]
    public void SplitButton_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/SplitButton/Views/SplitButtonShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/SplitButtonShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractSplitButtonExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractSplitButtonExampleItems(string source)
    {
        const string firstItemMarker  = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..panelCloseStart];
    }

    private static string ExtractShowCaseItemByTitle(string source, string titleResource)
    {
        const string itemStartMarker = "<gallery:ShowCaseItem";
        const string itemCloseMarker = "</gallery:ShowCaseItem>";

        var titleIndex = source.IndexOf(titleResource, StringComparison.Ordinal);
        titleIndex.ShouldBeGreaterThanOrEqualTo(0);

        var itemStart = source.LastIndexOf(itemStartMarker, titleIndex, StringComparison.Ordinal);
        itemStart.ShouldBeGreaterThanOrEqualTo(0);

        var itemClose = source.IndexOf(itemCloseMarker, titleIndex, StringComparison.Ordinal);
        itemClose.ShouldBeGreaterThan(titleIndex);

        return source[itemStart..(itemClose + itemCloseMarker.Length)];
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
