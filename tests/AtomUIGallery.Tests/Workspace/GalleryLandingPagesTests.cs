using System;
using System.IO;
using AtomUI.Controls;
using AtomUI.Toolkits.GalleryBase.Navigation;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Workspace;

public class GalleryLandingPagesTests
{
    [Fact]
    public void Navigation_Uses_Overview_Community_And_Components_As_Top_Level_Items()
    {
        var configuration = global::AtomUIGallery.AtomUIGalleryModule.CreateConfiguration();
        var topLevelKeys  = configuration.NavigationNodes.Select(node => node.Key.Value).ToArray();

        topLevelKeys.ShouldBe(["Overview", "Community", "Components"]);
        configuration.DefaultOpenKeys.ShouldContain(new EntityKey("Components"));
        configuration.NavigationNodes[0].IsRoute.ShouldBeTrue();
        configuration.NavigationNodes[1].IsRoute.ShouldBeTrue();
        configuration.NavigationNodes[2].IsRoute.ShouldBeFalse();
        Walk(configuration.NavigationNodes)
            .ShouldContain(node => node.Key == "General" && !node.IsRoute);
        Walk(configuration.NavigationNodes)
            .ShouldNotContain(node => node.Key == "AboutUs" || node.Key == "General_AboutUs");

        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml.cs");
        codeBehindSource.ShouldContain("ShowCaseNavMenu.DefaultOpenPaths");
        codeBehindSource.ShouldContain("GalleryNavigationMenuAdapter");
    }

    [Fact]
    public void Navigation_Default_Page_Is_Overview()
    {
        var configuration = global::AtomUIGallery.AtomUIGalleryModule.CreateConfiguration();
        var source        = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/ViewModels/CaseNavigationViewModel.cs");

        configuration.DefaultRoute.ShouldBe(new EntityKey("Overview"));
        source.ShouldContain("GalleryNavigationViewModel");
        source.ShouldNotContain("OverviewViewModel.ID");
        source.ShouldNotContain("AboutUsViewModel.ID");
    }

    [Fact]
    public void Navigation_Clicks_Only_Route_Showcase_Items()
    {
        var viewSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml.cs");
        var viewModelSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/ViewModels/CaseNavigationViewModel.cs");

        viewSource.ShouldContain("ViewModel.CanNavigateTo(key.Value)");
        viewModelSource.ShouldContain("GalleryNavigationViewModel");
        viewModelSource.ShouldNotContain("_showCaseViewModelFactories");
    }

    [Fact]
    public void Workspace_Uses_Layout_Background_Only_For_Content_Area()
    {
        var shellSource  = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/Shell/GalleryShellView.cs");
        var windowSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml");

        shellSource.ShouldContain("SharedTokenKind.ColorBgContainer");
        shellSource.ShouldContain("SharedTokenKind.ColorBorderSecondary");
        shellSource.ShouldContain("SharedTokenKind.ColorBgLayout");
        shellSource.ShouldContain("Grid.SetColumn(ContentHost, 1)");
        windowSource.ShouldContain("Name=\"ShellHost\"");
        windowSource.ShouldNotContain("Background=\"#");
        windowSource.ShouldNotContain("ColorBgContainer");
        windowSource.ShouldNotContain("ColorBgLayout");
    }

    [Fact]
    public void Overview_Page_Keeps_Product_Banner_And_Dynamic_NuGet_Install_Command()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Overview/Views/OverviewPage.axaml");

        source.ShouldContain("atomui-oss-release-banner.png");
        source.ShouldContain(".NET 8.0");
        source.ShouldContain(".NET 10");
        source.ShouldContain("dotnet add package AtomUI.Desktop.Controls --version");
        source.ShouldContain("{x:Static gallery:GalleryVersionInfo.Version}");
        source.ShouldContain("https://www.atomui.net/manual/AtomUIProgrammingBible/6.0");
        source.ShouldContain("https://www.atomui.net/reference/AtomUIOSS/v6.0.1");
        source.ShouldContain("OverviewPageLangResource LearningLinksTitle");
        source.ShouldContain("OverviewPageLangResource UserManualTitle");
        source.ShouldContain("OverviewPageLangResource ApiDocsTitle");
        source.ShouldContain("MaxWidth=\"820\"");
        source.ShouldContain("<StackPanel Width=\"820\"");
        source.ShouldContain("HorizontalAlignment=\"Center\"");
        source.ShouldNotContain("qinware-wechatpress.png");
        source.ShouldNotContain("atomui-wechat.png");
        source.ShouldNotContain("atomui-qq.png");

        var titleIndex = source.IndexOf("Text=\"{gallery:OverviewPageLangResource InstallTitle}\"", StringComparison.Ordinal);
        var dotNet8TagIndex = source.IndexOf("Text=\".NET 8.0\"", StringComparison.Ordinal);
        var dotNet10TagIndex = source.IndexOf("Text=\".NET 10\"", StringComparison.Ordinal);

        titleIndex.ShouldBeGreaterThanOrEqualTo(0);
        dotNet8TagIndex.ShouldBeGreaterThan(titleIndex);
        dotNet10TagIndex.ShouldBeGreaterThan(dotNet8TagIndex);
    }

    [Fact]
    public void Overview_Page_Uses_Shadow_Cards_For_Main_Content()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Overview/Views/OverviewPage.axaml");

        source.ShouldContain("BoxShadow=\"{atom:SharedTokenResource BoxShadowsTertiary}\"");
        source.ShouldNotContain("BorderBrush=\"{atom:SharedTokenResource ColorBorderSecondary}\"");
    }

    [Fact]
    public void Showcase_Items_Use_Elevated_Cards_Instead_Of_Bordered_Cards()
    {
        var source = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/ShowCaseItemTheme.axaml");

        source.ShouldContain("Background=\"{gallery:ShowCaseItemTokenSharedTokenResource ColorBgContainer}\"");
        source.ShouldContain("BoxShadow=\"{gallery:ShowCaseItemTokenResource CardShadow}\"");
        source.ShouldNotContain("Property=\"BorderBrush\" Value=\"{atom:SharedTokenResource ColorBorder}\"");
        source.ShouldNotContain("Property=\"BorderThickness\" Value=\"1\"");
    }

    [Fact]
    public void Community_Page_Holds_Company_Vision_Mission_Links_And_QR_Codes()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Community/Views/CommunityPage.axaml");

        source.ShouldContain("QinwareLogo");
        source.ShouldNotContain("<Svg Name=\"TLAIC\"");
        source.ShouldContain("<Svg Name=\"TLAICIncubationLogo\"");
        source.ShouldContain("VisionLabel");
        source.ShouldContain("MissionLabel");
        source.ShouldContain("https://www.atomui.net");
        source.ShouldContain("https://gitee.com/chinware/atomui");
        source.ShouldContain("https://github.com/chinware/atomui");
        source.ShouldContain("qinware-wechatpress.png");
        source.ShouldContain("atomui-telegram.png");
        source.ShouldContain("atomui-wechat.png");
        source.ShouldContain("atomui-qq.png");
        source.ShouldContain("TelegramGroup");
        source.ShouldNotContain("WeChatOfficialDescription");
        source.ShouldNotContain("TelegramGroupDescription");
        source.ShouldNotContain("WeChatGroupDescription");
        source.ShouldNotContain("QQGroupDescription");
        File.Exists(GetRepoFile("controlgallery/AtomUIGallery/Assets/atomui-telegram.png")).ShouldBeTrue();
        source.ShouldNotContain("AtomUIOSS-release-banner.png");
        source.ShouldNotContain("dotnet add package AtomUI.Desktop.Controls");
    }

    [Fact]
    public void Community_Page_Uses_Compact_About_Block_And_Shadow_QR_Cards()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Community/Views/CommunityPage.axaml");

        source.ShouldContain("CommunityPageLangResource AboutAtomUI");
        source.ShouldContain("Width=\"820\"");
        source.ShouldContain("BoxShadow=\"{atom:SharedTokenResource BoxShadowsTertiary}\"");
        source.ShouldContain("ColorBgContainer");
        source.ShouldNotContain("Style Selector=\"Svg#TLAIC\"");
        source.ShouldNotContain("Width=\"160\"");
        source.ShouldNotContain("<atom:GroupBox");
        source.ShouldNotContain("Foreground=\"Red\"");
        source.ShouldNotContain("Width=\"300\"");
        source.ShouldNotContain("Width=\"200\" />");
    }

    [Fact]
    public void Community_Page_Uses_Compact_Tlaic_Incubation_Block()
    {
        var source       = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Community/Views/CommunityPage.axaml");
        var zhCnResource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Community/Localization/zh_CN.cs");

        source.ShouldContain("CommunityPageLangResource IncubationEyebrow");
        source.ShouldContain("CommunityPageLangResource IncubationTitle");
        source.ShouldContain("CommunityPageLangResource IncubationDescription");
        source.ShouldContain("CommunityPageLangResource IncubationPointOpenSource");
        source.ShouldContain("CommunityPageLangResource IncubationPointEcosystem");
        source.ShouldContain("CommunityPageLangResource IncubationPointLicense");
        source.ShouldContain("<Svg Name=\"TLAICIncubationLogo\"");
        source.ShouldNotContain("atomui-oss-banner.png");
        source.IndexOf("CommunityPageLangResource IncubationEyebrow", StringComparison.Ordinal)
              .ShouldBeLessThan(source.IndexOf("CommunityPageLangResource WeChatOfficial", StringComparison.Ordinal));

        zhCnResource.ShouldContain("INCUBATION SUPPORT");
        zhCnResource.ShouldContain("AtomUI OSS 纳入通明湖中心开源孵化体系");
        zhCnResource.ShouldContain("开源孵化");
        zhCnResource.ShouldContain("生态共建");
        zhCnResource.ShouldContain("开放许可");
    }

    [Fact]
    public void Overview_And_Community_ViewModels_Are_Registered()
    {
        var configuration = global::AtomUIGallery.AtomUIGalleryModule.CreateConfiguration();
        var registerSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/ShowCaseRegister.cs");
        var navigationSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/ViewModels/CaseNavigationViewModel.cs");

        configuration.Routes.ContainsRoute("Overview").ShouldBeTrue();
        configuration.Routes.ContainsRoute("Community").ShouldBeTrue();
        registerSource.ShouldContain("AtomUIGalleryModule.RegisterViews(locator)");
        navigationSource.ShouldContain("AtomUIGalleryModule.GetConfiguration()");
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
