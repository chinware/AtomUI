using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class NotificationShowCasePageTests
{
    [Fact]
    public void Notification_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationShowCase.axaml");

        source.ShouldContain("NotificationShowCaseLangResource PageSubtitle");
        source.ShouldContain("NotificationShowCaseLangResource PageDescription");
        source.ShouldContain("NotificationShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("NotificationShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("NotificationShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("NotificationShowCaseLangResource ComponentCategory");
        source.ShouldContain("NotificationShowCaseLangResource ComponentStatusStable");
        source.ShouldContain("NotificationShowCaseLangResource ScenarioExamples");
        source.ShouldContain("NotificationShowCaseLangResource ScenarioApi");
        source.ShouldContain("NotificationShowCaseLangResource ScenarioDesignToken");
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
        CountShowCaseItemElements(source).ShouldBe(6);
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(6);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(6);
        CountOccurrences(source, "DataTemplate x:DataType=\"viewModels:NotificationViewModel\"").ShouldBe(6);
        source.ShouldContain("NotificationShowCaseLangResource BasicTitle");
        source.ShouldContain("NotificationShowCaseLangResource PlacementTitle");
        source.ShouldContain("NotificationShowCaseLangResource ProgressTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Notification_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationDesignTokenDataGrid.axaml.cs");

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
        codeBehindSource.ShouldContain("new NotificationApiDataGrid()");
        codeBehindSource.ShouldContain("new NotificationDesignTokenDataGrid()");
        codeBehindSource.ShouldContain("HandleHoverOptionGroupCheckedChanged");
        codeBehindSource.ShouldNotContain("HoverOptionGroup.OptionCheckedChanged +=");
        codeBehindSource.ShouldNotContain("HoverOptionGroup.SelectedIndex");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:NotificationApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("NotificationShowCaseLangResource ApiColumnMember");
        apiSource.ShouldContain("NotificationShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("NotificationShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("NotificationShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:NotificationDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("NotificationShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("NotificationShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("NotificationShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("NotificationShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void Notification_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyNotificationTitle");
            source.ShouldContain("ApiPropertyManagerPosition");
            source.ShouldContain("ApiMethodManagerShow");
            source.ShouldContain("TokenNameNotificationBg");
            source.ShouldContain("TokenNameNotificationProgressHeight");
        }
    }

    [Fact]
    public void Notification_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Feedback/Notification/Views/NotificationShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/NotificationShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractNotificationExampleItems(source));
        CountShowCaseItemElements(normalized).ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractNotificationExampleItems(string source)
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
