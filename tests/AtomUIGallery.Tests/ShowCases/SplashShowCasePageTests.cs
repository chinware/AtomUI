using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AtomUI.Toolkits.GalleryBase.Localization;
using AtomUI.Toolkits.GalleryBase.Navigation;
using AtomUIGallery.ShowCases.Splash;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class SplashShowCasePageTests
{
    [Fact]
    public void Splash_ShowCase_Is_Registered_Under_Other_Category()
    {
        var configuration      = global::AtomUIGallery.AtomUIGalleryModule.CreateConfiguration();
        var galleryProject     = ReadRepoFile("controlgallery/AtomUIGallery/AtomUIGallery.csproj");
        var assemblyInfoSource = ReadRepoFile("controlgallery/AtomUIGallery/Properties/AssemblyInfo.cs");
        var navigationEnSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/en_US.cs");
        var navigationZhCnSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/zh_CN.cs");
        var navigationZhTwSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/zh_TW.cs");

        var otherNode = Walk(configuration.NavigationNodes)
            .First(node => node.Key == "Other");
        var splashNode = Walk(configuration.NavigationNodes)
            .First(node => node.Key == SplashViewModel.ID);

        otherNode.IsRoute.ShouldBeFalse();
        otherNode.Header.ShouldBeAssignableTo<IGalleryLocalizedText>();
        otherNode.Icon.ShouldNotBeNull();
        splashNode.IsRoute.ShouldBeTrue();
        splashNode.Header.ShouldBeAssignableTo<IGalleryLocalizedText>();
        configuration.Routes.ContainsRoute(SplashViewModel.ID).ShouldBeTrue();
        galleryProject.ShouldContain("AtomUI.Desktop.Controls.Extras");
        assemblyInfoSource.ShouldContain("AtomUIGallery.ShowCases.Splash");

        navigationEnSource.ShouldContain("public const string Other_Splash");
        navigationZhCnSource.ShouldContain("public const string Other_Splash");
        navigationZhTwSource.ShouldContain("public const string Other_Splash");
    }

    [Fact]
    public void Splash_ShowCase_Uses_Document_Layout_With_Examples()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/Splash/Views/SplashShowCase.axaml");

        source.ShouldContain("SplashShowCaseLangResource PageSubtitle");
        source.ShouldContain("SplashShowCaseLangResource PageDescription");
        source.ShouldContain("SplashShowCaseLangResource ComponentCategory");
        source.ShouldContain("SplashShowCaseLangResource ComponentStatusPreview");
        source.ShouldContain("SplashShowCaseLangResource ComponentIntroducedVersion");
        source.ShouldNotContain("SplashShowCaseLangResource ScenarioExamples");
        source.ShouldNotContain("SplashShowCaseLangResource ScenarioApi");
        source.ShouldNotContain("SplashShowCaseLangResource ScenarioDesignToken");
        source.ShouldContain("<gallery:GalleryStickyTabsHost");
        source.ShouldContain("<gallery:GalleryShowCaseHeader");
        source.ShouldContain("Title=\"Splash\"");
        source.ShouldContain("Category=\"{gallery:SplashShowCaseLangResource ComponentCategory}\"");
        source.ShouldContain("Status=\"{gallery:SplashShowCaseLangResource ComponentStatusPreview}\"");
        source.ShouldContain("StatusTagColor=\"processing\"");
        source.ShouldContain("IntroducedVersion=\"{gallery:SplashShowCaseLangResource ComponentIntroducedVersion}\"");
        source.ShouldContain("Subtitle=\"{gallery:SplashShowCaseLangResource PageSubtitle}\"");
        source.ShouldContain("Description=\"{gallery:SplashShowCaseLangResource PageDescription}\"");
        source.ShouldContain("Namespace=\"AtomUI.Desktop.Controls\"");
        source.ShouldContain("Package=\"AtomUI.Desktop.Controls.Extras\"");
        source.ShouldContain("BaseClass=\"ContentControl\"");
        source.ShouldContain("MetadataValueWidth=\"240\"");
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
    public void Splash_ShowCase_Header_Centers_Title_Tags_And_Adds_Blue_Introduced_Version_Tag()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/Splash/Views/SplashShowCase.axaml");
        var enSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/en_US.cs");
        var zhCnSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/zh_CN.cs");
        var zhTwSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/zh_TW.cs");

        source.ShouldContain("<gallery:GalleryShowCaseHeader");
        source.ShouldContain("Status=\"{gallery:SplashShowCaseLangResource ComponentStatusPreview}\"");
        source.ShouldContain("StatusTagColor=\"processing\"");
        source.ShouldContain("IntroducedVersion=\"{gallery:SplashShowCaseLangResource ComponentIntroducedVersion}\"");
        source.ShouldNotContain("IntroducedVersionTagColor=");
        source.ShouldNotContain("IsIntroducedVersionTagBordered=");

        foreach (var localizationSource in new[] { enSource, zhCnSource, zhTwSource })
        {
            localizationSource.ShouldContain("public const string ComponentIntroducedVersion = \"v6.0.7\";");
        }
    }

    [Fact]
    public void Splash_ShowCase_Demos_Cover_Visual_States_Without_Showing_Window()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/Splash/Views/SplashShowCase.axaml");

        var basicDemo = ExtractShowCaseItemMarkup(source, "SplashShowCaseLangResource BasicTitle");
        basicDemo.ShouldContain("<atom:Splash");
        basicDemo.ShouldContain("Title=\"AtomUI\"");
        basicDemo.ShouldContain("IsIndeterminate=\"True\"");
        basicDemo.ShouldContain("Logo=\"{Binding BasicLogo}\"");
        basicDemo.ShouldNotContain("Splash.ShowAsync");

        var progressDemo = ExtractShowCaseItemMarkup(source, "SplashShowCaseLangResource DeterminateTitle");
        progressDemo.ShouldContain("Progress=\"{Binding ProgressValue}\"");
        progressDemo.ShouldContain("IsIndeterminate=\"False\"");
        progressDemo.ShouldContain("Message=\"{gallery:SplashShowCaseLangResource P2MessageLoadingModules}\"");

        var statusDemo = ExtractShowCaseItemMarkup(source, "SplashShowCaseLangResource StatusTitle");
        statusDemo.ShouldContain("Status=\"Success\"");
        statusDemo.ShouldContain("Status=\"Error\"");
        statusDemo.ShouldContain("Footer=\"{gallery:SplashShowCaseLangResource P2FooterStaticPreview}\"");

        var composedDemo = ExtractShowCaseItemMarkup(source, "SplashShowCaseLangResource ComposedTitle");
        composedDemo.ShouldContain("<atom:Tag");
        composedDemo.ShouldContain("LogoTemplate");
        composedDemo.ShouldContain("FooterTemplate");
    }

    [Fact]
    public void Splash_ShowCase_Window_Service_Demo_Runs_Fake_Loading_And_Closes()
    {
        var pageSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/Splash/Views/SplashShowCase.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/Splash/Views/SplashShowCase.axaml.cs");
        var enSource         = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/en_US.cs");
        var zhCnSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/zh_CN.cs");
        var zhTwSource       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/zh_TW.cs");

        var serviceDemo = ExtractShowCaseItemMarkup(pageSource, "SplashShowCaseLangResource WindowServiceTitle");
        serviceDemo.ShouldContain("SplashShowCaseLangResource WindowServiceDescription");
        serviceDemo.ShouldContain("P2ContentShowWindowSplash");
        serviceDemo.ShouldContain("Click=\"HandleShowWindowSplashButtonClick\"");

        codeBehindSource.ShouldContain("private bool _isWindowSplashRunning");
        codeBehindSource.ShouldContain("new GallerySplashService()");
        codeBehindSource.ShouldContain("private sealed class GallerySplashService : SplashService");
        codeBehindSource.ShouldContain("SplashTokenKind.SurfaceBackground");
        codeBehindSource.ShouldNotContain("ControlSharedTokenResourceKey");
        codeBehindSource.ShouldNotContain("SharedTokenKind.ColorTextHeading");
        codeBehindSource.ShouldNotContain("SharedTokenKind.ColorText");
        codeBehindSource.ShouldContain("SplashTokenKind.SubtleForeground");
        codeBehindSource.ShouldContain("window.Resources[SplashTokenKind.SurfaceBackground]");
        codeBehindSource.ShouldContain("AddTemplateForegroundStyle(window, \"PART_TitleBlock\", WindowSplashTitleBrush)");
        codeBehindSource.ShouldContain("AddTemplateForegroundStyle(window, \"PART_MessageBlock\", WindowSplashPrimaryTextBrush)");
        codeBehindSource.ShouldContain(".Template()");
        codeBehindSource.ShouldContain(".Name(partName)");
        codeBehindSource.ShouldNotContain("window.Resources[SharedTokenKind.ColorTextHeading]");
        codeBehindSource.ShouldNotContain("window.Resources[SharedTokenKind.ColorText]");
        codeBehindSource.ShouldNotContain("window.Splash.Resources");
        codeBehindSource.ShouldContain("#0B1026");
        codeBehindSource.ShouldContain("#1D39C4");
        codeBehindSource.ShouldContain("#13C2C2");
        codeBehindSource.ShouldContain("await splashService.ShowAsync(new SplashOptions");
        codeBehindSource.ShouldContain("private const double WindowSplashWidth = 560");
        codeBehindSource.ShouldContain("private const double WindowSplashMinHeight = 360");
        codeBehindSource.ShouldContain("Width               = WindowSplashWidth");
        codeBehindSource.ShouldContain("MinHeight           = WindowSplashMinHeight");
        codeBehindSource.ShouldContain("Logo                = WindowSplashLogo");
        codeBehindSource.ShouldContain("LogoTemplate        = CreateWindowSplashLogoTemplate()");
        codeBehindSource.ShouldNotContain("Content             = WindowSplashStages");
        codeBehindSource.ShouldNotContain("ContentTemplate     = CreateWindowSplashStagesTemplate()");
        codeBehindSource.ShouldNotContain("CreateWindowSplashStagesTemplate");
        codeBehindSource.ShouldNotContain("CreateWindowSplashStage");
        codeBehindSource.ShouldContain("FooterTemplate      = CreateWindowSplashFooterTemplate()");
        codeBehindSource.ShouldContain("LinearGradientBrush");
        codeBehindSource.ShouldContain("TimeSpan.FromSeconds(5)");
        codeBehindSource.ShouldContain("await Task.Delay(TimeSpan.FromSeconds(1)");
        codeBehindSource.ShouldContain("await splashService.SetProgressAsync");
        codeBehindSource.ShouldContain("await splashService.SetStatusAsync(SplashStatus.Success");
        codeBehindSource.ShouldContain("await splashService.CloseAsync()");

        foreach (var source in new[] { enSource, zhCnSource, zhTwSource })
        {
            source.ShouldContain("WindowServiceTitle");
            source.ShouldContain("WindowServiceDescription");
            source.ShouldContain("P2ContentShowWindowSplash");
            source.ShouldContain("P2WindowSplashMessageStarting");
            source.ShouldContain("P2WindowSplashMessageComplete");
            source.ShouldContain("P2WindowSplashFooter");
        }
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
        const string headerStartMarker = "Text=\"Splash\"";
        const string headerEndMarker = "SplashShowCaseLangResource PageSubtitle";

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
