using System;
using System.IO;
using System.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ButtonShowCasePageTests
{
    [Fact]
    public void Button_ShowCase_Uses_Document_Layout_With_Grouped_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");

        source.ShouldContain("ButtonShowCaseLangResource PageSubtitle");
        source.ShouldContain("ButtonShowCaseLangResource PageDescription");
        source.ShouldContain("ButtonShowCaseLangResource InfoNamespaceLabel");
        source.ShouldContain("ButtonShowCaseLangResource InfoPackageLabel");
        source.ShouldContain("ButtonShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("MinHeight=\"48\"");
        source.ShouldContain("Padding=\"16,8\"");
        source.ShouldContain("ItemSpacing=\"48\"");
        source.ShouldContain("LineSpacing=\"8\"");
        source.ShouldContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldContain("<Setter Property=\"Width\" Value=\"84\" />");
        source.ShouldContain("Selector=\"atom|TextBlock.info-value\"");
        source.ShouldContain("<Setter Property=\"Width\" Value=\"200\" />");
        source.ShouldContain("<Setter Property=\"TextWrapping\" Value=\"NoWrap\" />");
        source.ShouldContain("<Setter Property=\"TextTrimming\" Value=\"CharacterEllipsis\" />");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(3);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(3);
        source.ShouldNotContain("Classes=\"info-pair\"");
        source.ShouldNotContain("Classes=\"info-value-frame\"");
        source.ShouldNotContain("Width=\"300\"");
        source.ShouldNotContain("ColumnDefinitions=\"Auto,*\"");
        source.ShouldNotContain("<Setter Property=\"MaxWidth\" Value=\"220\" />");
        source.ShouldNotContain("<Setter Property=\"Width\" Value=\"104\" />");
        source.ShouldNotContain("<Setter Property=\"Width\" Value=\"128\" />");
        source.ShouldContain("LineHeight=\"22\"");
        source.ShouldNotContain("ColumnDefinitions=\"Auto,*,Auto,*,Auto,*\"");
        source.ShouldContain("ButtonShowCaseLangResource ScenarioExamples");
        source.ShouldContain("ButtonShowCaseLangResource ScenarioApi");
        source.ShouldContain("ButtonShowCaseLangResource ScenarioDesignToken");
        source.ShouldContain("Tag=\"Examples\"");
        source.ShouldContain("Tag=\"Api\"");
        source.ShouldContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("ContentPadding=\"0,10,0,0\"");
        source.ShouldNotContain("Margin=\"28,0,12,24\"");
        source.ShouldNotContain("<atom:TabControl.Styles>");
        source.ShouldNotContain("<gallery:ShowCasePanel ContentMargin=\"0,20,16,0\">");
        source.ShouldNotContain("<gallery:ShowCasePanel ContentMargin=\"0,10,16,0\">");
        source.ShouldNotContain("<gallery:ShowCasePanel ContentMargin=\"0,0,16,0\">");
        source.ShouldNotContain("ContentPadding=\"0,10,16,0\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("ButtonShowCaseLangResource TypeTitle");
        source.ShouldContain("ButtonShowCaseLangResource ColorVariantTitle");
        source.ShouldContain("ButtonShowCaseLangResource GradientButtonTitle");
        source.ShouldContain("ButtonShowCaseLangResource ButtonShapeTitle");
        source.ShouldContain("ButtonShowCaseLangResource SizeTitle");
        source.ShouldContain("ButtonShowCaseLangResource IconPlacementTitle");
        source.ShouldContain("ButtonShowCaseLangResource LoadingTitle");
        source.ShouldContain("Name=\"ScenarioTabs\"");
        source.ShouldContain("Text=\"{gallery:ButtonShowCaseLangResource PageDescription}\"");
        source.ShouldContain("HorizontalAlignment=\"Left\"");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain("Classes=\"showcase-table-row\"");
        source.ShouldNotContain("showcase-table-divider");
        source.ShouldNotContain("MinWidth=\"780\"");
        source.ShouldNotContain("ColumnDefinitions=\"220,*\"");
        source.ShouldNotContain("Width=\"280\"");
        source.ShouldNotContain("UsagePrimaryActionTitle");
        source.ShouldNotContain("OverviewTitle");
        source.ShouldNotContain(">Gallery<");
        source.ShouldNotContain("ButtonShowCaseLangResource ScenarioGallery");
    }

    [Fact]
    public void Button_Color_And_Variant_Example_Is_Last_Full_Row_With_Version_Badge()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var examples = ExtractButtonExampleItems(source);

        var colorVariantIndex = examples.IndexOf("ButtonShowCaseLangResource ColorVariantTitle", StringComparison.Ordinal);
        colorVariantIndex.ShouldBeGreaterThanOrEqualTo(0);
        colorVariantIndex.ShouldBeGreaterThan(
            examples.IndexOf("ButtonShowCaseLangResource DisabledTitle", StringComparison.Ordinal));
        colorVariantIndex.ShouldBeGreaterThan(
            examples.IndexOf("ButtonShowCaseLangResource GhostButtonTitle", StringComparison.Ordinal));
        colorVariantIndex.ShouldBeGreaterThan(
            examples.IndexOf("ButtonShowCaseLangResource GradientButtonTitle", StringComparison.Ordinal));

        var colorVariantItem = examples[colorVariantIndex..];
        colorVariantItem.ShouldContain("Span=\"Full\"");
        colorVariantItem.ShouldContain("BadgeText=\"v6.0.5\"");
        colorVariantItem.ShouldNotContain("CustomBackground");
        colorVariantItem.ShouldNotContain("LinearGradientBrush");
        colorVariantItem.ShouldNotContain("P2ColorCustom");
        colorVariantItem.ShouldNotContain("P2ContentGradient");
        colorVariantItem.ShouldNotContain("RibbonBadgeText=\"v6.0.5\"");
    }

    [Fact]
    public void Button_Gradient_Example_Is_Separate_ShowCase_With_Two_Custom_Backgrounds()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var examples = ExtractButtonExampleItems(source);

        var colorVariantIndex = examples.IndexOf("ButtonShowCaseLangResource ColorVariantTitle", StringComparison.Ordinal);
        var gradientIndex     = examples.IndexOf("ButtonShowCaseLangResource GradientButtonTitle", StringComparison.Ordinal);
        gradientIndex.ShouldBeGreaterThanOrEqualTo(0);
        gradientIndex.ShouldBeLessThan(colorVariantIndex);

        var gradientItem = ExtractShowCaseItemByTitle(examples, "ButtonShowCaseLangResource GradientButtonTitle");
        gradientItem.ShouldContain("BadgeText=\"v6.0.5\"");
        gradientItem.ShouldNotContain("Span=\"Full\"");
        gradientItem.ShouldContain("CustomBackground");
        CountOccurrences(gradientItem, "<atom:Button.CustomBackground>").ShouldBe(2);
        CountOccurrences(gradientItem, "<LinearGradientBrush").ShouldBe(2);
        gradientItem.ShouldContain("#6253E1");
        gradientItem.ShouldContain("#04BEFE");
        gradientItem.ShouldContain("#FF7A45");
        gradientItem.ShouldContain("#FFD666");
        gradientItem.ShouldNotContain("/template/");
        gradientItem.ShouldNotContain("RibbonBadgeText=\"v6.0.5\"");
    }

    [Fact]
    public void Button_Size_Example_Uses_Custom_Option_To_Resize_All_Bound_Buttons()
    {
        var source           = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml.cs");
        var examples         = ExtractButtonExampleItems(source);

        var sizeItem = ExtractShowCaseItemByTitle(examples, "ButtonShowCaseLangResource SizeTitle");
        CountOccurrences(sizeItem, "<atom:OptionButton ").ShouldBe(4);
        sizeItem.ShouldContain("ButtonShowCaseLangResource P2ContentCustom");
        CountOccurrences(sizeItem, "SizeType=\"{Binding ButtonSizeType}\"").ShouldBe(10);
        CountOccurrences(sizeItem, "Classes=\"size-demo-button\"").ShouldBe(10);
        sizeItem.ShouldContain("Selector=\"atom|Button.size-demo-button[SizeType=Custom]\"");
        sizeItem.ShouldContain("<Setter Property=\"Height\" Value=\"44\" />");
        sizeItem.ShouldContain("<Setter Property=\"Padding\" Value=\"18,0\" />");
        sizeItem.ShouldContain("<Setter Property=\"FontSize\" Value=\"15\" />");
        sizeItem.ShouldNotContain("SizeType=\"Custom\"");
        sizeItem.ShouldNotContain("Height=\"44\"");
        sizeItem.ShouldNotContain("Padding=\"18,0\"");
        sizeItem.ShouldNotContain("FontSize=\"15\"");

        codeBehindSource.ShouldContain("CustomizableSizeType.Custom");
    }

    [Fact]
    public void Button_Icon_Placement_Example_Is_Separate_ShowCase_With_V606_Badge()
    {
        var source           = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml.cs");
        var viewModelSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/ViewModels/ButtonViewModel.cs");
        var examples         = ExtractButtonExampleItems(source);

        var iconItem          = ExtractShowCaseItemByTitle(examples, "ButtonShowCaseLangResource IconTitle");
        var iconPlacementItem = ExtractShowCaseItemByTitle(examples, "ButtonShowCaseLangResource IconPlacementTitle");

        iconItem.ShouldNotContain("IconPlacement=\"End\"");
        iconPlacementItem.ShouldContain("BadgeText=\"v6.0.6\"");
        iconPlacementItem.ShouldContain("ButtonShowCaseLangResource IconPlacementDescription");
        iconPlacementItem.ShouldContain("ButtonShowCaseLangResource P2TextIconPlacement");
        iconPlacementItem.ShouldContain("OptionCheckedChanged=\"HandleButtonIconPlacementOptionCheckedChanged\"");
        iconPlacementItem.ShouldContain("IconPlacement=\"{Binding ButtonIconPlacement}\"");
        CountOccurrences(iconPlacementItem, "IconPlacement=\"{Binding ButtonIconPlacement}\"").ShouldBe(8);

        codeBehindSource.ShouldContain("HandleButtonIconPlacementOptionCheckedChanged");
        codeBehindSource.ShouldContain("ButtonIconPlacement.Start");
        codeBehindSource.ShouldContain("ButtonIconPlacement.End");
        viewModelSource.ShouldContain("ButtonIconPlacement ButtonIconPlacement");
    }

    [Fact]
    public void Button_ShowCase_Lazy_Loads_Api_And_DesignToken_DataGrids()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml.cs");
        var apiSource        = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonApiDataGrid.axaml");
        var apiCodeSource    = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonApiDataGrid.axaml.cs");
        var tokenSource      = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonDesignTokenDataGrid.axaml");
        var tokenCodeSource  = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonDesignTokenDataGrid.axaml.cs");

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
        codeBehindSource.ShouldContain("new ButtonApiDataGrid()");
        codeBehindSource.ShouldContain("new ButtonDesignTokenDataGrid()");

        apiSource.ShouldContain("<atom:DataGrid");
        apiSource.ShouldContain("x:DataType=\"viewModels:ButtonApiRow\"");
        apiSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        apiSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        apiSource.ShouldContain("PaginationVisibility=\"None\"");
        apiSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        apiSource.ShouldContain("Margin=\"28,10,28,28\"");
        apiSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        apiSource.ShouldContain("VerticalAlignment=\"Top\"");
        apiSource.ShouldNotContain("Margin=\"0,20,0,0\"");
        apiSource.ShouldContain("ButtonShowCaseLangResource ApiColumnProperty");
        apiSource.ShouldContain("ButtonShowCaseLangResource ApiColumnDescription");
        apiSource.ShouldContain("ButtonShowCaseLangResource ApiColumnType");
        apiSource.ShouldContain("ButtonShowCaseLangResource ApiColumnDefault");
        apiSource.ShouldContain("Header=\"{gallery:ButtonShowCaseLangResource ApiColumnDescription}\"");
        apiSource.ShouldContain("MinWidth=\"520\"");
        apiSource.ShouldContain("Width=\"*\"");
        apiCodeSource.ShouldContain("viewModel.EnsureApiRows()");
        apiCodeSource.ShouldContain("ApiDataGrid.ItemsSource = viewModel.ApiRows");

        tokenSource.ShouldContain("<atom:DataGrid");
        tokenSource.ShouldContain("x:DataType=\"viewModels:ButtonDesignTokenRow\"");
        tokenSource.ShouldContain("GridLinesVisibility=\"Horizontal\"");
        tokenSource.ShouldContain("IsFrameBorderVisible=\"True\"");
        tokenSource.ShouldContain("PaginationVisibility=\"None\"");
        tokenSource.ShouldContain("LeftFrozenColumnCount=\"1\"");
        tokenSource.ShouldContain("Margin=\"28,10,28,28\"");
        tokenSource.ShouldContain("HorizontalAlignment=\"Stretch\"");
        tokenSource.ShouldContain("VerticalAlignment=\"Top\"");
        tokenSource.ShouldNotContain("Margin=\"0,20,0,0\"");
        tokenSource.ShouldContain("ButtonShowCaseLangResource TokenColumnToken");
        tokenSource.ShouldContain("ButtonShowCaseLangResource TokenColumnDescription");
        tokenSource.ShouldContain("ButtonShowCaseLangResource TokenColumnScope");
        tokenSource.ShouldContain("ButtonShowCaseLangResource TokenColumnStatus");
        tokenSource.ShouldContain("Header=\"{gallery:ButtonShowCaseLangResource TokenColumnDescription}\"");
        tokenSource.ShouldContain("MinWidth=\"520\"");
        tokenSource.ShouldContain("Width=\"*\"");
        tokenCodeSource.ShouldContain("viewModel.EnsureDesignTokenRows()");
        tokenCodeSource.ShouldContain("DesignTokenDataGrid.ItemsSource = viewModel.DesignTokenRows");
    }

    [Fact]
    public void Button_ShowCase_Localization_Includes_Page_And_Api_Copy()
    {
        var en   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Localization/en_US.cs");
        var zhCn = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Localization/zh_CN.cs");
        var zhTw = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Localization/zh_TW.cs");

        foreach (var source in new[] { en, zhCn, zhTw })
        {
            source.ShouldContain("ScenarioExamples");
            source.ShouldContain("ScenarioApi");
            source.ShouldContain("ScenarioDesignToken");
            source.ShouldContain("PageSubtitle");
            source.ShouldContain("InfoNamespaceLabel");
            source.ShouldContain("ApiPropertyButtonType");
            source.ShouldContain("ApiPropertyColor");
            source.ShouldContain("ApiPropertyVariant");
            source.ShouldContain("ApiPropertyCustomBackground");
            source.ShouldContain("GradientButtonTitle");
            source.ShouldContain("GradientButtonDescription");
            source.ShouldContain("IconPlacementTitle");
            source.ShouldContain("IconPlacementDescription");
            source.ShouldContain("ApiPropertyLoading");
            source.ShouldContain("ApiPropertyIcon");
            source.ShouldContain("P2TextIconPlacement");
            source.ShouldContain("P2ContentCustom");
            source.ShouldContain("TokenNameColorPrimary");
            source.ShouldContain("TokenNameControlHeight");
        }
    }

    [Fact]
    public void Button_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Button/Views/ButtonShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/ButtonShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractButtonExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    private static string ExtractButtonExampleItems(string source)
    {
        const string firstItemMarker = "<gallery:ShowCaseItem";
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

    private static int CountOccurrences(string source, string value)
    {
        var count = 0;
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
