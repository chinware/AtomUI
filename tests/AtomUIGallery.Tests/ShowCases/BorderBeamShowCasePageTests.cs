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

        var basicDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource BasicTitle");
        basicDemo.ShouldContain("<atom:BorderBeam");
        basicDemo.ShouldContain("<atom:Card Header=\"Workspace overview\"");
        basicDemo.ShouldContain("Users");
        basicDemo.ShouldContain("Projects");
        basicDemo.ShouldContain("Tasks");

        var customizedColorDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource CustomizedColorTitle");
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
        var nonUniformRadiusIndex = source.IndexOf(
            "BorderBeamShowCaseLangResource NonUniformRadiusTitle",
            StringComparison.Ordinal);
        var customizedColorIndex = source.IndexOf(
            "BorderBeamShowCaseLangResource CustomizedColorTitle",
            StringComparison.Ordinal);

        basicIndex.ShouldBeGreaterThanOrEqualTo(0);
        nonUniformRadiusIndex.ShouldBeGreaterThan(basicIndex);
        customizedColorIndex.ShouldBeGreaterThan(nonUniformRadiusIndex);

        var nonUniformRadiusDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource NonUniformRadiusTitle");
        nonUniformRadiusDemo.ShouldNotContain("IsOccupyEntireRow=\"True\"");

        var customizedColorDemo = ExtractShowCaseItemMarkup(source, "BorderBeamShowCaseLangResource CustomizedColorTitle");
        customizedColorDemo.ShouldContain("IsOccupyEntireRow=\"True\"");
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
