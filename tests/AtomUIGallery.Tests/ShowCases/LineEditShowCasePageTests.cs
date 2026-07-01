using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class LineEditShowCasePageTests
{
    [Fact]
    public void LineEdit_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");

        source.ShouldContain("LineEditShowCaseLangResource PageSubtitle");
        source.ShouldContain("LineEditShowCaseLangResource PageDescription");
        source.ShouldNotContain("LineEditShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("LineEditShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("LineEditShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("LineEditShowCaseLangResource ComponentCategory");
        source.ShouldContain("LineEditShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("LineEditShowCaseLangResource ScenarioExamples");
        source.ShouldContain("LineEditShowCaseLangResource ScenarioApi");
        source.ShouldContain("LineEditShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:LineEditShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(17);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(17);
        source.ShouldContain("LineEditShowCaseLangResource BasicUsageTitle");
        source.ShouldContain("LineEditShowCaseLangResource InputSizesTitle");
        source.ShouldContain("LineEditShowCaseLangResource P2PlaceholderTextCustom");
        source.ShouldContain("SizeType=\"Custom\"");
        source.ShouldContain("Height=\"38\"");
        source.ShouldContain("FontSize=\"15\"");
        source.ShouldContain("LineEditShowCaseLangResource InputStatusTitle");
        source.ShouldContain("LineEditShowCaseLangResource SearchBoxTitle");
        source.ShouldContain("LineEditShowCaseLangResource SearchEditSizeTypeTitle");
        source.ShouldContain("LineEditShowCaseLangResource SearchEditSizeTypeDescription");
        source.ShouldContain("Name=\"CustomSizeTypeSearchEdit\"");
        source.ShouldContain("LineEditShowCaseLangResource TextAreaTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void LineEdit_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditDesignTokenDataGrid.axaml.cs");

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
        codeBehindSource.ShouldContain("new LineEditApiDataGrid()");
        codeBehindSource.ShouldContain("new LineEditDesignTokenDataGrid()");
        codeBehindSource.ShouldNotContain("new LineEditBasicShowCase()");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:LineEditApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("LineEditShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("LineEditShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("LineEditShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("LineEditShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:LineEditDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("LineEditShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("LineEditShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("LineEditShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("LineEditShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void LineEdit_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyIsAllowClear");
            source.ShouldContain("ApiPropertySearchButtonStyle");
            source.ShouldContain("ApiPropertyIsAutoSize");
            source.ShouldContain("SearchEditSizeTypeTitle");
            source.ShouldContain("SearchEditSizeTypeDescription");
            source.ShouldContain("P2PlaceholderTextCustom");
            source.ShouldContain("TokenNameInputFontSize");
            source.ShouldContain("TokenNameRightAddOnPadding");
        }
    }

    [Fact]
    public void LineEdit_ShowCase_SearchEdit_SizeType_Item_Demonstrates_All_Size_Modes()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");
        var item   = ExtractShowCaseItem(source, "LineEditShowCaseLangResource SearchEditSizeTypeTitle");

        CountOccurrences(item, "<atom:SearchEdit").ShouldBe(4);
        item.ShouldContain("Name=\"CustomSizeTypeSearchEdit\"");
        item.ShouldContain("SizeType=\"Large\"");
        item.ShouldContain("SizeType=\"Middle\"");
        item.ShouldContain("SizeType=\"Small\"");
        item.ShouldContain("SizeType=\"Custom\"");
        item.ShouldContain("Height=\"38\"");
        item.ShouldContain("FontSize=\"15\"");
    }

    [Fact]
    public void LineEdit_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/LineEdit/Views/LineEditShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/LineEditShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractLineEditExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractLineEditExampleItems(string source)
    {
        const string firstItemMarker  = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return StripDeferredLoadingMarkup(source[firstItemStart..panelCloseStart]);
    }

    private static string ExtractShowCaseItem(string source, string titleMarker)
    {
        var titleIndex = source.IndexOf(titleMarker, StringComparison.Ordinal);
        titleIndex.ShouldBeGreaterThanOrEqualTo(0);

        const string itemStartMarker = "<gallery:ShowCaseItem";
        const string itemEndMarker   = "</gallery:ShowCaseItem>";

        var itemStart = source.LastIndexOf(itemStartMarker, titleIndex, StringComparison.Ordinal);
        itemStart.ShouldBeGreaterThanOrEqualTo(0);

        var itemEnd = source.IndexOf(itemEndMarker, titleIndex, StringComparison.Ordinal);
        itemEnd.ShouldBeGreaterThan(titleIndex);

        return source[itemStart..(itemEnd + itemEndMarker.Length)];
    }

    private static string StripDeferredLoadingMarkup(string source)
    {
        return ShowCaseSnapshotMarkup.StripDeferredLoadingMarkup(source);
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
