using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class UploadShowCasePageTests
{
    [Fact]
    public void Upload_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadShowCase.axaml");

        source.ShouldContain("UploadShowCaseLangResource PageSubtitle");
        source.ShouldContain("UploadShowCaseLangResource PageDescription");
        source.ShouldContain("UploadShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("UploadShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("UploadShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("UploadShowCaseLangResource ComponentCategory");
        source.ShouldContain("UploadShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("UploadShowCaseLangResource ScenarioExamples");
        source.ShouldContain("UploadShowCaseLangResource ScenarioApi");
        source.ShouldContain("UploadShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Text=\"{gallery:UploadShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(10);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(10);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:UploadViewModel\"").ShouldBe(10);
        source.ShouldContain("UploadShowCaseLangResource UploadByClickingTitle");
        source.ShouldContain("UploadShowCaseLangResource PicturesWallTitle");
        source.ShouldContain("UploadShowCaseLangResource UploadPngOnlyTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Upload_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadDesignTokenDataGrid.axaml.cs");

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
        codeBehindSource.ShouldContain("new UploadApiDataGrid()");
        codeBehindSource.ShouldContain("new UploadDesignTokenDataGrid()");
        codeBehindSource.ShouldNotContain("AttachUpload(");
        codeBehindSource.ShouldNotContain("new UploadBasicShowCase()");
        codeBehindSource.ShouldNotContain("new UploadPicturesShowCase()");
        codeBehindSource.ShouldNotContain("new UploadConstraintsShowCase()");

        CountOccurrences(pageSource, "UploadTransport=\"{Binding UploadTransport}\"").ShouldBe(12);
        CountOccurrences(pageSource, "UploadTaskFailed=\"HandleUploadFailed\"").ShouldBe(12);
        CountOccurrences(pageSource, "UploadTaskCompleted=\"HandleUploadCompleted\"").ShouldBe(12);
        CountOccurrences(pageSource, "UploadTaskAboutToScheduling=\"HandleImageUploadAboutToScheduling\"").ShouldBe(2);
        CountOccurrences(pageSource, "UploadTaskAboutToScheduling=\"HandlePngUploadAboutToScheduling\"").ShouldBe(1);
        codeBehindSource.ShouldContain("HandleImageUploadAboutToScheduling");
        codeBehindSource.ShouldContain("HandlePngUploadAboutToScheduling");
        codeBehindSource.ShouldContain("HandleUploadFailed");
        codeBehindSource.ShouldContain("HandleUploadCompleted");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:UploadApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("UploadShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("UploadShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("UploadShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("UploadShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:UploadDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("UploadShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("UploadShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("UploadShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("UploadShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void Upload_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyAccepts");
            source.ShouldContain("ApiPropertyMaxCount");
            source.ShouldContain("ApiPropertyUploadTransport");
            source.ShouldContain("ApiPropertyDefaultTaskList");
            source.ShouldContain("TokenNameActionsColor");
            source.ShouldContain("TokenNamePictureCardSize");
            source.ShouldContain("TokenNameDragIconSize");
        }
    }

    [Fact]
    public void Upload_ShowCase_Removes_Orphaned_SubShowCase_Files()
    {
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadBasicShowCase.axaml")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadBasicShowCase.axaml.cs")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadPicturesShowCase.axaml")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadPicturesShowCase.axaml.cs")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadConstraintsShowCase.axaml")).ShouldBeFalse();
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadConstraintsShowCase.axaml.cs")).ShouldBeFalse();
    }

    [Fact]
    public void Upload_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Upload/Views/UploadShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/UploadShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractUploadExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractUploadExampleItems(string source)
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
        return ShowCaseSnapshotMarkup.Normalize(StripUploadBehaviorMarkup(source));
    }

    private static string StripUploadBehaviorMarkup(string source)
    {
        var normalized = Regex.Replace(
            source,
            "\\s*UploadTransport=\"\\{Binding UploadTransport\\}\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "\\s*UploadTaskFailed=\"HandleUploadFailed\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "\\s*UploadTaskCompleted=\"HandleUploadCompleted\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        normalized = Regex.Replace(
            normalized,
            "\\s*UploadTaskAboutToScheduling=\"Handle(?:Image|Png)UploadAboutToScheduling\"",
            string.Empty,
            RegexOptions.CultureInvariant);

        return normalized;
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
