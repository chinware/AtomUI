using System;
using System.IO;
using System.Linq;
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
        var navigationSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml");
        var navigationVmSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/ViewModels/CaseNavigationViewModel.cs");
        var registerSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/ShowCaseRegister.cs");
        var navigationEnSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/en_US.cs");
        var navigationZhCnSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/zh_CN.cs");
        var navigationZhTwSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/zh_TW.cs");

        navigationSource.ShouldContain("CaseNavigationLangResource Other");
        navigationSource.ShouldContain("Icon=\"{antdicons:AntDesignIconProvider Kind=BlockOutlined}\"");
        navigationSource.ShouldNotContain("Icon=\"{antdicons:AntDesignIconProvider Kind=MoreOutlined}\"");
        navigationSource.ShouldContain("CaseNavigationLangResource Other_BorderBeam");
        navigationSource.ShouldContain("ItemKey=\"{x:Static viewmodels:BorderBeamViewModel.ID}\"");
        navigationVmSource.ShouldContain("_showCaseViewModelFactories.Add(BorderBeamViewModel.ID");
        registerSource.ShouldContain("locator.Map<BorderBeamViewModel, BorderBeamShowCase>");

        navigationEnSource.ShouldContain("public const string Other");
        navigationEnSource.ShouldContain("public const string Other_BorderBeam");
        navigationZhCnSource.ShouldContain("public const string Other");
        navigationZhCnSource.ShouldContain("public const string Other_BorderBeam");
        navigationZhTwSource.ShouldContain("public const string Other");
        navigationZhTwSource.ShouldContain("public const string Other_BorderBeam");
    }

    [Fact]
    public void BorderBeam_ShowCase_Uses_Document_Layout_With_Examples_And_Tables()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/BorderBeam/Views/BorderBeamShowCase.axaml");

        source.ShouldContain("BorderBeamShowCaseLangResource PageSubtitle");
        source.ShouldContain("BorderBeamShowCaseLangResource PageDescription");
        source.ShouldContain("BorderBeamShowCaseLangResource ComponentCategory");
        source.ShouldContain("BorderBeamShowCaseLangResource ComponentStatusPreview");
        source.ShouldContain("BorderBeamShowCaseLangResource ScenarioExamples");
        source.ShouldContain("BorderBeamShowCaseLangResource ScenarioApi");
        source.ShouldContain("BorderBeamShowCaseLangResource ScenarioDesignToken");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("StickyContentPadding=\"28,0,28,0\"");
        source.ShouldContain("<atom:TabStrip Name=\"ScenarioTabs\"");
        source.ShouldContain("<ContentControl Name=\"ScenarioContentHost\">");
        source.ShouldContain("Name=\"ExamplesContent\"");
        source.ShouldContain("IsScrollEnabled=\"False\"");
        source.ShouldContain("IsDeferredLoadingEnabled=\"True\"");
        source.ShouldContain("Tag=\"Examples\"");
        source.ShouldContain("Tag=\"Api\"");
        source.ShouldContain("Tag=\"DesignToken\"");
        source.ShouldNotContain("<atom:TabControl");
        source.ShouldNotContain("<atom:DataGrid");
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

    [Fact]
    public void BorderBeam_ShowCase_Api_And_Token_Tables_Document_Public_Contract()
    {
        var viewModel = new BorderBeamViewModel(null!);

        viewModel.EnsureApiRows();
        viewModel.EnsureDesignTokenRows();

        viewModel.ApiRows.ShouldNotBeNull();
        viewModel.ApiRows!.Select(row => row.Property).ShouldContain("Color");
        viewModel.ApiRows.Select(row => row.Property).ShouldContain("ColorStops");
        viewModel.ApiRows.Select(row => row.Property).ShouldContain("Outset");
        viewModel.ApiRows.Select(row => row.Property).ShouldContain("BorderThickness");
        viewModel.ApiRows.Select(row => row.Property).ShouldContain("CornerRadius");
        viewModel.ApiRows.Select(row => row.Property).ShouldContain("IsMotionEnabled");
        viewModel.ApiRows.Select(row => row.Property).ShouldContain("Duration");
        viewModel.ApiRows.Select(row => row.Property).ShouldContain("BeamSize");

        viewModel.DesignTokenRows.ShouldNotBeNull();
        viewModel.DesignTokenRows!.Select(row => row.Token).ShouldContain("BeamSize");
        viewModel.DesignTokenRows.Select(row => row.Token).ShouldContain("BeamOpacity");
        viewModel.DesignTokenRows.Select(row => row.Token).ShouldContain("MotionDuration");
        viewModel.DesignTokenRows.Select(row => row.Token).ShouldContain("MaxVisibleStopPercent");
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
