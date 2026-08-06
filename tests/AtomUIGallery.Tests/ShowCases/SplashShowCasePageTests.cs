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
        var navigationEn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/en-US.xlf");
        var navigationZhCn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/zh-CN.xlf");
        var navigationZhTw = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/Workspace/Localization/CaseNavigationLang/zh-TW.xlf");

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

        foreach (var localization in new[] { navigationEn, navigationZhCn, navigationZhTw })
        {
            localization.ContainsKey("Other_Splash").ShouldBeTrue();
        }
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
        var en = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/en-US.xlf");
        var zhCn = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/zh-CN.xlf");
        var zhTw = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/zh-TW.xlf");

        source.ShouldContain("<gallery:GalleryShowCaseHeader");
        source.ShouldContain("Status=\"{gallery:SplashShowCaseLangResource ComponentStatusPreview}\"");
        source.ShouldContain("StatusTagColor=\"processing\"");
        source.ShouldContain("IntroducedVersion=\"{gallery:SplashShowCaseLangResource ComponentIntroducedVersion}\"");
        source.ShouldNotContain("IntroducedVersionTagColor=");
        source.ShouldNotContain("IsIntroducedVersionTagBordered=");

        foreach (var localization in new[] { en, zhCn, zhTw })
        {
            localization["ComponentIntroducedVersion"].ShouldBe("v6.0.7");
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
        var en               = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/en-US.xlf");
        var zhCn             = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/zh-CN.xlf");
        var zhTw             = XliffTestDocument.Read(
            "controlgallery/AtomUIGallery/ShowCases/Other/Splash/Localization/zh-TW.xlf");

        var serviceDemo = ExtractShowCaseItemMarkup(pageSource, "SplashShowCaseLangResource WindowServiceTitle");
        serviceDemo.ShouldContain("SplashShowCaseLangResource WindowServiceDescription");
        serviceDemo.ShouldContain("P2ContentShowWindowSplash");
        serviceDemo.ShouldContain("Click=\"HandleShowWindowSplashButtonClick\"");

        codeBehindSource.ShouldContain("private bool _isWindowSplashRunning");
        codeBehindSource.ShouldContain("new GallerySplashService()");
        codeBehindSource.ShouldContain("private sealed class GallerySplashService : SplashService");
        codeBehindSource.ShouldContain("return new GallerySplashWindow();");
        codeBehindSource.ShouldNotContain("base.CreateWindow(");
        codeBehindSource.ShouldNotContain("window.Resources[");
        codeBehindSource.ShouldNotContain("ControlTokenResourceKey");
        codeBehindSource.ShouldNotContain("SplashTokens.Identity");
        codeBehindSource.ShouldNotContain("SharedTokenKind");
        codeBehindSource.ShouldNotContain("SplashTokenKind");
        codeBehindSource.ShouldNotContain("WindowSplashTitleForegroundResourceKey");
        codeBehindSource.ShouldNotContain("WindowSplashMessageForegroundResourceKey");
        codeBehindSource.ShouldNotContain("AddTemplateForegroundStyle");
        codeBehindSource.ShouldNotContain(".Template()");
        codeBehindSource.ShouldNotContain(".Name(partName)");
        codeBehindSource.ShouldNotContain("PART_TitleBlock");
        codeBehindSource.ShouldNotContain("PART_MessageBlock");
        codeBehindSource.ShouldNotContain("CreateWindowSplashSurfaceBrush");
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

        foreach (var localization in new[] { en, zhCn, zhTw })
        {
            localization.ContainsKey("WindowServiceTitle").ShouldBeTrue();
            localization.ContainsKey("WindowServiceDescription").ShouldBeTrue();
            localization.ContainsKey("P2ContentShowWindowSplash").ShouldBeTrue();
            localization.ContainsKey("P2WindowSplashMessageStarting").ShouldBeTrue();
            localization.ContainsKey("P2WindowSplashMessageComplete").ShouldBeTrue();
            localization.ContainsKey("P2WindowSplashFooter").ShouldBeTrue();
        }
    }

    [Fact]
    public void Splash_ShowCase_Dedicated_Child_Owns_Its_Custom_Splash_Styles()
    {
        var windowSource = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/Other/Splash/ShowCaseControls/GallerySplashWindow.axaml");
        var windowControlSource = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/Other/Splash/ShowCaseControls/GallerySplashWindow.axaml.cs");
        var splashSource = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/Other/Splash/ShowCaseControls/GalleryWindowSplash.axaml");
        var splashControlSource = ReadRepoFile(
            "controlgallery/AtomUIGallery/ShowCases/Other/Splash/ShowCaseControls/GalleryWindowSplash.axaml.cs");

        windowControlSource.ShouldContain("partial class GallerySplashWindow : SplashWindow");
        windowControlSource.ShouldNotContain("GalleryWindowSplash :");
        windowSource.ShouldContain("x:Class=\"AtomUIGallery.ShowCases.Splash.GallerySplashWindow\"");
        windowSource.ShouldContain("<local:GalleryWindowSplash />");
        windowSource.ShouldNotContain("ControlTheme");
        windowSource.ShouldNotContain("BasedOn");
        windowSource.ShouldNotContain("StaticResource");
        windowSource.ShouldNotContain("/template/");

        splashControlSource.ShouldContain("sealed partial class GalleryWindowSplash : SplashControl");
        splashControlSource.ShouldContain("StyleKeyOverride { get; } = typeof(SplashControl)");
        splashSource.ShouldContain("x:Class=\"AtomUIGallery.ShowCases.Splash.GalleryWindowSplash\"");
        splashSource.ShouldContain("Classes=\"gallery-window-splash\"");
        splashSource.ShouldContain("<atom:Splash.Background>");
        splashSource.ShouldContain("<atom:Splash.Styles>");
        splashSource.ShouldNotContain("ControlTheme");
        splashSource.ShouldNotContain("BasedOn");
        splashSource.ShouldNotContain("StaticResource");
        splashSource.ShouldNotContain("Border#PART_SurfaceLayout");
        splashSource.ShouldContain("atom|Splash.gallery-window-splash /template/ atom|TextBlock#PART_TitleBlock");
        splashSource.ShouldContain("atom|Splash.gallery-window-splash:loading /template/ atom|TextBlock#PART_MessageBlock");
        splashSource.ShouldContain("atom|Splash.gallery-window-splash /template/ atom|TextBlock#PART_SubtitleBlock");
        splashSource.ShouldContain("atom|Splash.gallery-window-splash /template/ atom|TextBlock#PART_DetailBlock");
        splashSource.ShouldContain("#0B1026");
        splashSource.ShouldContain("#1D39C4");
        splashSource.ShouldContain("#13C2C2");
        splashSource.ShouldContain("#F5F8FF");
        splashSource.ShouldContain("#D6E4FF");
        splashSource.ShouldNotContain("ControlTokenResourceKey");
        splashSource.ShouldNotContain("SplashTokenKind");
        splashSource.ShouldNotContain("gallery-window-splash:success");
        splashSource.ShouldNotContain("gallery-window-splash:error");
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
