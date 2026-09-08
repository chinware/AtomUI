using System.Text.RegularExpressions;
using AtomUI.Toolkits.GalleryBase.Localization;
using AtomUI.Toolkits.GalleryBase.Navigation;
using AtomUIGallery.ShowCases.BorderBeam;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class BorderBeamShowCasePageTests
{
    [Fact]
    public void BorderBeam_ShowCase_Is_Registered_Under_Other_Category()
    {
        var configuration = global::AtomUIGallery.AtomUIGalleryModule.CreateConfiguration();
        var registerSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/ShowCaseRegister.cs");
        var navigationEn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/en-US.xlf");
        var navigationZhCn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/zh-CN.xlf");
        var navigationZhTw = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/zh-TW.xlf");

        var otherNode = Walk(configuration.NavigationNodes)
            .First(node => node.Key == "Other");
        var borderBeamNode = Walk(configuration.NavigationNodes)
            .First(node => node.Key == BorderBeamViewModel.ID);

        otherNode.IsRoute.ShouldBeFalse();
        otherNode.Header.ShouldBeAssignableTo<IGalleryLocalizedText>();
        otherNode.Icon.ShouldNotBeNull();
        borderBeamNode.IsRoute.ShouldBeTrue();
        borderBeamNode.Header.ShouldBeAssignableTo<IGalleryLocalizedText>();
        configuration.Routes.ContainsRoute(BorderBeamViewModel.ID).ShouldBeTrue();
        registerSource.ShouldContain("AtomUIGalleryModule.RegisterViews(locator)");

        foreach (var localization in new[] { navigationEn, navigationZhCn, navigationZhTw })
        {
            localization.ContainsKey("Other").ShouldBeTrue();
            localization.ContainsKey("Other_BorderBeam").ShouldBeTrue();
        }
    }

    [Fact]
    public void BorderBeam_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Views/BorderBeamShowCase.axaml");

        source.ShouldContain("BorderBeamShowCaseLangResource PageSubtitle");
        source.ShouldContain("BorderBeamShowCaseLangResource PageDescription");
        source.ShouldContain("BorderBeamShowCaseLangResource ComponentCategory");
        source.ShouldContain("BorderBeamShowCaseLangResource ComponentIntroducedVersion");
        source.ShouldNotContain("BorderBeamShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("BorderBeamShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("BorderBeamShowCaseLangResource ScenarioDesignToken");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("<gallery:GalleryShowCaseHeader");
        source.ShouldContain("Title=\"BorderBeam\"");
        source.ShouldContain("Category=\"{gallery:BorderBeamShowCaseLangResource ComponentCategory}\"");
        source.ShouldContain("IntroducedVersion=\"{gallery:BorderBeamShowCaseLangResource ComponentIntroducedVersion}\"");
        source.ShouldContain("Subtitle=\"{gallery:BorderBeamShowCaseLangResource PageSubtitle}\"");
        source.ShouldContain("Description=\"{gallery:BorderBeamShowCaseLangResource PageDescription}\"");
        source.ShouldContain("Namespace=\"AtomUI.Desktop.Controls\"");
        source.ShouldContain("Package=\"AtomUI.Desktop.Controls\"");
        source.ShouldContain("BaseClass=\"ContentControl\"");
        source.ShouldContain("MetadataValueWidth=\"220\"");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldNotContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldNotContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("IsDeferredLoadingEnabled=\"True\"");
        source.ShouldNotContain("Tag=\"Examples\"");
        source.ShouldNotContain("Tag=\"Api\"");
        source.ShouldNotContain("Tag=\"DesignToken\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
    }

    [Fact]
    public void BorderBeam_ShowCase_Header_Centers_Title_Tags_And_Uses_Blue_Introduced_Version_Tag()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Views/BorderBeamShowCase.axaml");
        var en = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Localization/en-US.xlf");
        var zhCn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Localization/zh-CN.xlf");
        var zhTw = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Localization/zh-TW.xlf");

        source.ShouldContain("<gallery:GalleryShowCaseHeader");
        source.ShouldContain("IntroducedVersion=\"{gallery:BorderBeamShowCaseLangResource ComponentIntroducedVersion}\"");
        source.ShouldNotContain("Status=\"{gallery:BorderBeamShowCaseLangResource ComponentStatusPreview}\"");
        source.ShouldNotContain("IntroducedVersionTagColor=");
        source.ShouldNotContain("IsIntroducedVersionTagBordered=");

        foreach (var localization in new[] { en, zhCn, zhTw })
        {
            localization["ComponentIntroducedVersion"].ShouldBe("v6.0.5");
            localization.ContainsKey("ComponentStatusPreview").ShouldBeFalse();
        }
    }

    [Fact]
    public void BorderBeam_ShowCase_Demos_Match_Ant_Design_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Views/BorderBeamShowCase.axaml");

        Regex.Matches(source, @"<gallery:ShowCaseItem\s").Count.ShouldBe(5);
        source.Split("<gallery:ShowCaseItem.DeferredContentTemplate>").Length.ShouldBe(6);

        var basicDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource BasicTitle");
        basicDemo.ShouldContain("SourceKey=\"border-beam-basic\"");
        basicDemo.ShouldContain("<atom:BorderBeam");
        basicDemo.ShouldContain("<atom:Card Header=\"Workspace overview\"");
        basicDemo.ShouldContain("Users");
        basicDemo.ShouldContain("Projects");
        basicDemo.ShouldContain("Tasks");

        var hoverDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource ShowOnHoverTitle");
        hoverDemo.ShouldContain("SourceKey=\"border-beam-show-on-hover\"");
        hoverDemo.ShouldContain("Classes=\"show-on-hover\"");
        hoverDemo.ShouldContain("BorderBeamShowCaseLangResource ShowOnHoverCardTitle");
        hoverDemo.ShouldContain("BorderBeamShowCaseLangResource ShowOnHoverCardDescription");

        source.ShouldContain("<Style Selector=\"atom|BorderBeam.show-on-hover\">");
        source.ShouldContain("<Style Selector=\"atom|BorderBeam.show-on-hover:pointerover\">");
        source.ShouldContain("<Setter Property=\"IsMotionEnabled\" Value=\"False\" />");
        source.ShouldContain("<Setter Property=\"IsMotionEnabled\" Value=\"True\" />");

        var multipleBeamsDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource MultipleBeamsTitle");
        multipleBeamsDemo.ShouldContain("SourceKey=\"border-beam-multiple-beams\"");
        multipleBeamsDemo.ShouldContain("Count=\"3\"");
        multipleBeamsDemo.ShouldContain("BorderBeamShowCaseLangResource MultipleBeamsCardTitle");
        multipleBeamsDemo.ShouldContain("BorderBeamShowCaseLangResource MultipleBeamsCardDescription");

        var customizedColorDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource CustomizedColorTitle");
        customizedColorDemo.ShouldContain("SourceKey=\"border-beam-customized-color\"");
        customizedColorDemo.ShouldContain("<atom:Segmented");
        customizedColorDemo.ShouldContain("ItemsSource=\"{Binding ColorPresets}\"");
        customizedColorDemo.ShouldContain("SelectedItem=\"{Binding SelectedColorPreset}\"");
        customizedColorDemo.ShouldContain("ColorStops=\"{Binding SelectedColorStops}\"");
        customizedColorDemo.ShouldContain("#1677FF");
        customizedColorDemo.ShouldContain("#36CFC9");
        customizedColorDemo.ShouldContain("#F759AB");
        customizedColorDemo.ShouldContain("#B37FEB");
        customizedColorDemo.ShouldContain("TextWrapping=\"Wrap\"");

        var nonUniformRadiusDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource NonUniformRadiusTitle");
        nonUniformRadiusDemo.ShouldContain("SourceKey=\"border-beam-non-uniform-radius\"");
        nonUniformRadiusDemo.ShouldContain("Outset=\"0\"");
        nonUniformRadiusDemo.ShouldContain("CornerRadius=\"20,20,0,0\"");
        nonUniformRadiusDemo.ShouldContain("ClipToBounds=\"True\"");
        nonUniformRadiusDemo.ShouldContain("Non-uniform radius");
    }

    [Fact]
    public void BorderBeam_ShowCase_Pairs_Half_Width_Examples_Before_Full_Row_Demo()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Views/BorderBeamShowCase.axaml");

        var basicIndex = source.IndexOf(
            "BorderBeamShowCaseLangResource BasicTitle",
            StringComparison.Ordinal);
        var showOnHoverIndex = source.IndexOf(
            "BorderBeamShowCaseLangResource ShowOnHoverTitle",
            StringComparison.Ordinal);
        var multipleBeamsIndex = source.IndexOf(
            "BorderBeamShowCaseLangResource MultipleBeamsTitle",
            StringComparison.Ordinal);
        var nonUniformRadiusIndex = source.IndexOf(
            "BorderBeamShowCaseLangResource NonUniformRadiusTitle",
            StringComparison.Ordinal);
        var customizedColorIndex = source.IndexOf(
            "BorderBeamShowCaseLangResource CustomizedColorTitle",
            StringComparison.Ordinal);

        basicIndex.ShouldBeGreaterThanOrEqualTo(0);
        showOnHoverIndex.ShouldBeGreaterThan(basicIndex);
        multipleBeamsIndex.ShouldBeGreaterThan(showOnHoverIndex);
        nonUniformRadiusIndex.ShouldBeGreaterThan(multipleBeamsIndex);
        customizedColorIndex.ShouldBeGreaterThan(nonUniformRadiusIndex);

        var basicDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource BasicTitle");
        basicDemo.ShouldNotContain("IsOccupyEntireRow=\"True\"");

        var hoverDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource ShowOnHoverTitle");
        hoverDemo.ShouldNotContain("IsOccupyEntireRow=\"True\"");

        var multipleBeamsDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource MultipleBeamsTitle");
        multipleBeamsDemo.ShouldNotContain("IsOccupyEntireRow=\"True\"");

        var nonUniformRadiusDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource NonUniformRadiusTitle");
        nonUniformRadiusDemo.ShouldNotContain("IsOccupyEntireRow=\"True\"");

        var customizedColorDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource CustomizedColorTitle");
        customizedColorDemo.ShouldContain("IsOccupyEntireRow=\"True\"");
    }

    [Fact]
    public void BorderBeam_ShowCase_Localizes_Hover_And_Multiple_Beam_Examples()
    {
        var localizationPaths = new[]
        {
            "controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Localization/en-US.xlf",
            "controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Localization/pt-BR.xlf",
            "controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Localization/zh-CN.xlf",
            "controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Localization/zh-TW.xlf"
        };
        var expectedKeys = new[]
        {
            "ShowOnHoverTitle",
            "ShowOnHoverDescription",
            "ShowOnHoverCardTitle",
            "ShowOnHoverCardDescription",
            "MultipleBeamsTitle",
            "MultipleBeamsDescription",
            "MultipleBeamsCardTitle",
            "MultipleBeamsCardDescription"
        };

        foreach (var path in localizationPaths)
        {
            var localization = XliffTestDocument.Read(path);
            foreach (var key in expectedKeys)
            {
                localization.ContainsKey(key).ShouldBeTrue($"{path} should contain {key}");
            }
        }
    }

    [Fact]
    public void BorderBeam_ShowCase_ViewModel_Provides_Customized_Color_Presets()
    {
        var viewModel = new BorderBeamViewModel(null!);

        viewModel.ColorPresets.Select(preset => preset.Name).ShouldBe(new[]
        {
            "Ocean",
            "Sunset",
            "Aurora",
            "Forest",
            "Ember",
            "Nebula"
        });
        viewModel.SelectedColorPreset.ShouldBe(viewModel.ColorPresets[0]);
        viewModel.SelectedColorStops.Select(stop => stop.Color).ShouldBe(new[]
        {
            Color.Parse("#1677FF"),
            Color.Parse("#36CFC9"),
            Color.Parse("#95DE64")
        });
    }

    private static string ExtractShowCaseItemMarkup(string source, string titleMarker)
    {
        var titleIndex = source.IndexOf(titleMarker, StringComparison.Ordinal);
        titleIndex.ShouldBeGreaterThanOrEqualTo(0);

        const string itemStartMarker = "<gallery:ShowCaseItem";
        const string itemEndMarker = "</gallery:ShowCaseItem>";

        var itemStart = source.LastIndexOf(itemStartMarker, titleIndex, StringComparison.Ordinal);
        itemStart.ShouldBeGreaterThanOrEqualTo(0);

        var itemEnd = source.IndexOf(itemEndMarker, titleIndex, StringComparison.Ordinal);
        itemEnd.ShouldBeGreaterThan(itemStart);

        return source[itemStart..(itemEnd + itemEndMarker.Length)];
    }

    private static string ExtractHeaderTitleMarkup(string source)
    {
        const string headerStartMarker = "Text=\"BorderBeam\"";
        const string headerEndMarker = "BorderBeamShowCaseLangResource PageSubtitle";

        var titleIndex = source.IndexOf(headerStartMarker, StringComparison.Ordinal);
        titleIndex.ShouldBeGreaterThanOrEqualTo(0);
        var headerStart = source.LastIndexOf("<Grid", titleIndex, StringComparison.Ordinal);
        headerStart.ShouldBeGreaterThanOrEqualTo(0);
        var headerEnd = source.IndexOf(headerEndMarker, headerStart, StringComparison.Ordinal);
        headerEnd.ShouldBeGreaterThan(headerStart);

        return source[headerStart..headerEnd];
    }

    private static IEnumerable<GalleryNavigationNode> Walk(IEnumerable<GalleryNavigationNode> nodes)
    {
        foreach (var node in nodes)
        {
            yield return node;
            foreach (var child in Walk(node.Children))
            {
                yield return child;
            }
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
