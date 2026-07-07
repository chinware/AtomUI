using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class RadioButtonShowCasePageTests
{
    [Fact]
    public void RadioButton_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonShowCase.axaml");

        source.ShouldContain("RadioButtonShowCaseLangResource PageSubtitle");
        source.ShouldContain("RadioButtonShowCaseLangResource PageDescription");
        source.ShouldNotContain("RadioButtonShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("RadioButtonShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("RadioButtonShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("RadioButtonShowCaseLangResource ComponentCategory");
        source.ShouldContain("RadioButtonShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("RadioButtonShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("RadioButtonShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("RadioButtonShowCaseLangResource ScenarioDesignToken");
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
        source.ShouldContain("Description=\"{gallery:RadioButtonShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        CountOccurrences(source, "IsDeferredContentEnabled=\"True\"").ShouldBe(11);
        CountOccurrences(source, "<gallery:ShowCaseItem.DeferredContentTemplate>").ShouldBe(11);
        CountOccurrences(source, "DataTemplate x:DataType=\"vm:RadioButtonViewModel\"").ShouldBe(11);
        source.ShouldContain("RadioButtonShowCaseLangResource BasicTitle");
        source.ShouldContain("RadioButtonShowCaseLangResource RadioGroupTitle");
        source.ShouldContain("RadioButtonShowCaseLangResource CheckedItemBindingTitle");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("RadioButtonShowCaseLangResource OptionButtonTitle");
        source.ShouldContain("RadioButtonShowCaseLangResource SizeTypeTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:TabItem");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void RadioButton_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonDesignTokenDataGrid.axaml.cs");

        pageSource.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        pageSource.ShouldNotContain("Name=\"ScenarioContentHost\"");
        pageSource.ShouldNotContain("Tag=\"Api\"");
        pageSource.ShouldNotContain("Tag=\"DesignToken\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding ApiRows}\"");
        pageSource.ShouldNotContain("ItemsSource=\"{Binding DesignTokenRows}\"");
        pageSource.ShouldContain("ItemsSource=\"{Binding RadioOptions}\"");
        pageSource.ShouldContain("ItemsSource=\"{Binding TwoWayRadioOptions}\"");
        pageSource.ShouldContain("CheckedItem=\"{Binding TwoWayCheckedItem}\"");
        pageSource.ShouldContain("Text=\"{Binding TwoWayCheckedSummary}\"");
        pageSource.ShouldContain("Command=\"{Binding SelectChengduCommand}\"");
        pageSource.ShouldContain("Command=\"{Binding ClearTwoWayCheckedItemCommand}\"");

        codeBehindSource.ShouldNotContain("new GalleryShowCaseScenarioController");
        codeBehindSource.ShouldNotContain("_scenarioController.Attach(DataContext)");
        codeBehindSource.ShouldNotContain("_scenarioController.UpdateDataContext(DataContext)");
        codeBehindSource.ShouldNotContain("new RadioButtonApiDataGrid()");
        codeBehindSource.ShouldNotContain("new RadioButtonDesignTokenDataGrid()");
        codeBehindSource.ShouldContain("ConfigureRadioOptions(viewModel)");
        codeBehindSource.ShouldNotContain("new RadioButtonBasicShowCase()");
        codeBehindSource.ShouldNotContain("new RadioButtonGroupsShowCase()");
        codeBehindSource.ShouldNotContain("new RadioButtonOptionsShowCase()");
        codeBehindSource.ShouldNotContain("new RadioButtonStylesShowCase()");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:RadioButtonApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldContain("RadioButtonShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("RadioButtonShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("RadioButtonShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("RadioButtonShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:RadioButtonDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldContain("RadioButtonShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("RadioButtonShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("RadioButtonShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("RadioButtonShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void RadioButton_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("CheckedItemBindingTitle");
            source.ShouldContain("CheckedItemBindingDescription");
            source.ShouldNotContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyIsChecked");
            source.ShouldContain("ApiPropertyCheckedItem");
            source.ShouldContain("ApiPropertyItemsSource");
            source.ShouldContain("ApiPropertyButtonStyle");
            source.ShouldContain("ApiPropertyIcon");
            source.ShouldContain("P2CheckedItemSummaryFormat");
            source.ShouldContain("TokenNameRadioSize");
            source.ShouldContain("TokenNameButtonBackground");
            source.ShouldContain("TokenNameButtonPadding");
        }
    }

    [Fact]
    public void RadioButton_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/RadioButton/Views/RadioButtonShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/RadioButtonShowCaseExamples.snapshot");

        var normalized = NormalizeMarkup(ExtractRadioButtonExampleItems(source));
        CountOccurrences(normalized, "<gallery:ShowCaseItem").ShouldBe(ReadSnapshotCount(approved));
        ComputeSha256(normalized).ShouldBe(ReadSnapshotHash(approved));
    }

    private static string ExtractRadioButtonExampleItems(string source)
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
