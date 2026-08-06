using AtomUIGallery.ShowCases.Slider;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class SliderShowCasePageTests
{
    [Fact]
    public void Slider_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml");

        source.ShouldContain("SliderShowCaseLangResource PageSubtitle");
        source.ShouldContain("SliderShowCaseLangResource PageDescription");
        source.ShouldNotContain("SliderShowCaseLangResource InfoNamespaceLabel");
        source.ShouldNotContain("SliderShowCaseLangResource InfoPackageLabel");
        source.ShouldNotContain("SliderShowCaseLangResource InfoBaseClassLabel");
        source.ShouldContain("SliderShowCaseLangResource ComponentCategory");
        source.ShouldContain("SliderShowCaseLangResource ComponentStatusStable");
        source.ShouldNotContain("SliderShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("SliderShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("SliderShowCaseLangResource ScenarioDesignToken");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("ContentMargin=\"28,10,28,28\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-label\"");
        source.ShouldNotContain("Selector=\"atom|TextBlock.info-value\"");
        CountOccurrences(source, "Classes=\"info-label\"").ShouldBe(0);
        CountOccurrences(source, "Classes=\"info-value\"").ShouldBe(0);
        source.ShouldNotContain("LineHeight=\"22\"");
        source.ShouldContain("Description=\"{gallery:SliderShowCaseLangResource PageDescription}\"");
        source.ShouldContain("<gallery:ShowCaseItem");
        source.ShouldContain("SliderShowCaseLangResource BasicTitle");
        source.ShouldContain("SliderShowCaseLangResource RangeValuesBindingTitle");
        source.ShouldContain("SliderShowCaseLangResource MultiHandleTitle");
        source.ShouldContain("RangeValues=\"");
        source.ShouldNotContain("RangeValue=\"");
        source.ShouldContain("BadgeText=\"v6.0.8\"");
        source.ShouldContain("SliderShowCaseLangResource CustomizeTooltipTitle");
        source.ShouldContain("SliderShowCaseLangResource VerticalTitle");
        source.ShouldContain("SliderShowCaseLangResource GraduatedSliderTitle");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
        source.ShouldNotContain(">Gallery<");
    }

    [Fact]
    public void Slider_ShowCase_Examples_Match_Approved_Control_Demo_Content()
    {
        var source   = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml");
        var approved = ReadRepoFile("tests/AtomUIGallery.Tests/ShowCases/SliderShowCaseExamples.snapshot");

        NormalizeMarkup(ExtractSliderExampleItems(source))
            .ShouldBe(NormalizeMarkup(approved));
    }

    [Fact]
    public void Multi_Handle_Example_Matches_Ant_Design_Composition()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml");
        var item = ExtractShowCaseItem(source, "MultiHandleTitle");

        CountOccurrences(item, "RangeValues=\"").ShouldBe(1);
        item.ShouldContain("RangeValues=\"{Binding MultiHandleRangeValues, Mode=OneWay}\"");
        item.ShouldContain("<atom:Slider.TracksBrush>");
        item.ShouldContain("<LinearGradientBrush StartPoint=\"0%,50%\" EndPoint=\"100%,50%\">");
        item.ShouldContain("Color=\"#52C41A\"");
        item.ShouldContain("Color=\"#FAAD14\"");
        item.ShouldContain("Color=\"#FF4D4F\"");
        item.ShouldContain("TrackBarBrush=\"{x:Null}\"");
        item.ShouldNotContain("BadgeText=");
        item.ShouldNotContain("IsSnapToTickEnabled=");
        item.ShouldNotContain("TickFrequency=");
        item.ShouldNotContain("IsDraggableTrack=");
        item.ShouldNotContain("DisabledHandles=");
    }

    [Fact]
    public void Multi_Handle_Localization_Uses_Approved_Copy()
    {
        var zhCn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Localization/zh-CN.xlf");
        zhCn["MultiHandleTitle"].ShouldBe("多点组合");
        zhCn["MultiHandleDescription"].ShouldBe("范围多个点组合。");

        var zhTw = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Localization/zh-TW.xlf");
        zhTw["MultiHandleTitle"].ShouldBe("多點組合");
        zhTw["MultiHandleDescription"].ShouldBe("範圍多個點組合。");

        var enUs = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Localization/en-US.xlf");
        enUs["MultiHandleTitle"].ShouldBe("Multiple points");
        enUs["MultiHandleDescription"].ShouldBe("Combine multiple points in a range.");
    }

    [Fact]
    public void Disabled_Handle_Example_Matches_Ant_Design_Composition()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Views/SliderShowCase.axaml");
        var item = ExtractShowCaseItem(source, "DisabledHandleTitle");

        CountOccurrences(item, "RangeValues=\"").ShouldBe(1);
        CountOccurrences(item, "<atom:CheckBox").ShouldBe(3);
        item.ShouldContain("RangeValues=\"{Binding DisabledHandleRangeValues, Mode=OneWay}\"");
        item.ShouldContain("DisabledHandles=\"{Binding DisabledHandles}\"");
        item.ShouldContain("IsChecked=\"{Binding IsHandle1Disabled, Mode=TwoWay}\"");
        item.ShouldContain("IsChecked=\"{Binding IsHandle2Disabled, Mode=TwoWay}\"");
        item.ShouldContain("IsChecked=\"{Binding IsHandle3Disabled, Mode=TwoWay}\"");
        item.ShouldNotContain("BadgeText=");
        item.ShouldNotContain("IsSnapToTickEnabled=");
        item.ShouldNotContain("TickFrequency=");
    }

    [Fact]
    public void Disabled_Handle_ViewModel_Replaces_DisabledHandles_Snapshot()
    {
        var viewModel = new SliderViewModel(null!);
        var original = viewModel.DisabledHandles;
        var changedProperties = new List<string?>();
        viewModel.PropertyChanged += (_, args) => changedProperties.Add(args.PropertyName);

        viewModel.IsHandle2Disabled = true;

        viewModel.DisabledHandleRangeValues.ShouldBe([20, 50, 80]);
        viewModel.DisabledHandles.ShouldBe([false, true, false]);
        viewModel.DisabledHandles.ShouldNotBeSameAs(original);
        changedProperties.ShouldContain(nameof(SliderViewModel.IsHandle2Disabled));
        changedProperties.ShouldContain(nameof(SliderViewModel.DisabledHandles));
    }

    [Fact]
    public void Disabled_Handle_Localization_Uses_Approved_Copy()
    {
        var zhCn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Localization/zh-CN.xlf");
        zhCn["DisabledHandleTitle"].ShouldBe("禁用指定滑块");
        zhCn["DisabledHandleDescription"].ShouldBe(
            "设置 disabled 为数组，可以单独禁用 range 模式下特定的 handle。禁用后该 handle 作为移动边界，其他 handle 无法越过。");

        var zhTw = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Localization/zh-TW.xlf");
        zhTw["DisabledHandleTitle"].ShouldBe("禁用指定滑塊");

        var enUs = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/DataEntry/Slider/Localization/en-US.xlf");
        enUs["DisabledHandleTitle"].ShouldBe("Disabled handles");
    }

    private static string ExtractSliderExampleItems(string source)
    {
        const string firstItemMarker  = "<gallery:ShowCaseItem";
        const string panelCloseMarker = "</gallery:ShowCasePanel>";

        var firstItemStart = source.IndexOf(firstItemMarker, StringComparison.Ordinal);
        firstItemStart.ShouldBeGreaterThanOrEqualTo(0);

        var panelCloseStart = source.IndexOf(panelCloseMarker, firstItemStart, StringComparison.Ordinal);
        panelCloseStart.ShouldBeGreaterThan(firstItemStart);

        return source[firstItemStart..panelCloseStart];
    }

    private static string ExtractShowCaseItem(string source, string titleResource)
    {
        var titleMarker = $"Title=\"{{gallery:SliderShowCaseLangResource {titleResource}}}\"";
        var titleIndex = source.IndexOf(titleMarker, StringComparison.Ordinal);
        titleIndex.ShouldBeGreaterThanOrEqualTo(0);

        var itemStart = source.LastIndexOf("<gallery:ShowCaseItem", titleIndex, StringComparison.Ordinal);
        itemStart.ShouldBeGreaterThanOrEqualTo(0);

        const string itemCloseMarker = "</gallery:ShowCaseItem>";
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
