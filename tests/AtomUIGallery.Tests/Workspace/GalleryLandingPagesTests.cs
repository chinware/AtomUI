using System;
using System.IO;
using System.Text.RegularExpressions;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Workspace;

public class GalleryLandingPagesTests
{
    [Fact]
    public void Navigation_Uses_Overview_Community_And_Components_As_Top_Level_Items()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml");
        var codeBehindSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml.cs");

        source.ShouldContain("Header=\"{gallery:CaseNavigationLangResource Overview}\"");
        source.ShouldContain("ItemKey=\"{x:Static viewmodels:OverviewViewModel.ID}\"");
        source.ShouldContain("Header=\"{gallery:CaseNavigationLangResource Community}\"");
        source.ShouldContain("ItemKey=\"{x:Static viewmodels:CommunityViewModel.ID}\"");
        source.ShouldContain("Header=\"{gallery:CaseNavigationLangResource Components}\"");
        source.ShouldContain("ItemKey=\"Components\"");
        source.ShouldContain("Icon=\"{antdicons:AntDesignIconProvider Kind=AppstoreOutlined}\"");
        Regex.IsMatch(
            source,
            "Header=\"\\{gallery:CaseNavigationLangResource General\\}\"\\s+Icon=\"\\{antdicons:AntDesignIconProvider Kind=ControlOutlined\\}\"").ShouldBeTrue();
        codeBehindSource.ShouldContain("ShowCaseNavMenu.DefaultOpenPaths");
        codeBehindSource.ShouldContain("new(\"Components\")");
        source.ShouldNotContain("Header=\"{gallery:CaseNavigationLangResource General_AboutUs}\"");
    }

    [Fact]
    public void Navigation_Default_Page_Is_Overview()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/ViewModels/CaseNavigationViewModel.cs");

        source.ShouldContain("DoNavigateTo(OverviewViewModel.ID)");
        source.ShouldNotContain("DoNavigateTo(AboutUsViewModel.ID)");
    }

    [Fact]
    public void Navigation_Clicks_Only_Route_Showcase_Items()
    {
        var viewSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml.cs");
        var viewModelSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/ViewModels/CaseNavigationViewModel.cs");

        viewSource.ShouldContain("ViewModel.CanNavigateTo(key.Value)");
        viewModelSource.ShouldContain("public bool CanNavigateTo(EntityKey showCaseId)");
        viewModelSource.ShouldContain("_showCaseViewModelFactories.ContainsKey(showCaseId)");
    }

    [Fact]
    public void Workspace_Uses_Layout_Background_Only_For_Content_Area()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml");

        source.ShouldContain("Background=\"{atom:SharedTokenResource ColorBgContainer}\"");
        source.ShouldContain("BorderBrush=\"{atom:SharedTokenResource ColorBorderSecondary}\"");
        source.ShouldContain("<Border Grid.Column=\"1\"");
        source.ShouldContain("Background=\"{atom:SharedTokenResource ColorBgLayout}\"");
        Regex.IsMatch(source, "<Border Grid\\.Column=\"0\"\\s+Background=").ShouldBeFalse();
        source.ShouldNotContain("Background=\"#");
    }

    [Fact]
    public void Overview_Page_Keeps_Product_Banner_And_Dynamic_NuGet_Install_Command()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Overview/Views/OverviewPage.axaml");

        source.ShouldContain("AtomUIOSS-release-banner.png");
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
        var source = ReadRepoFile("controlgallery/AtomUIGallery/Controls/ShowCaseItemTheme.axaml");

        source.ShouldContain("Background=\"{atom:SharedTokenResource ColorBgContainer}\"");
        source.ShouldContain("BoxShadow=\"{gallery:ShowCaseItemTokenResource CardShadow}\"");
        source.ShouldNotContain("Property=\"BorderBrush\" Value=\"{atom:SharedTokenResource ColorBorder}\"");
        source.ShouldNotContain("Property=\"BorderThickness\" Value=\"1\"");
    }

    [Fact]
    public void Community_Page_Holds_Company_Vision_Mission_Links_And_QR_Codes()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/General/Community/Views/CommunityPage.axaml");

        source.ShouldContain("QinwareLogo");
        source.ShouldContain("TLAIC");
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
        source.ShouldContain("TelegramGroupDescription");
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
        source.ShouldNotContain("<atom:GroupBox");
        source.ShouldNotContain("Foreground=\"Red\"");
        source.ShouldNotContain("Width=\"300\"");
        source.ShouldNotContain("Width=\"200\" />");
    }

    [Fact]
    public void Overview_And_Community_ViewModels_Are_Registered()
    {
        var registerSource = ReadRepoFile("controlgallery/AtomUIGallery/ShowCases/ShowCaseRegister.cs");
        var navigationSource = ReadRepoFile("controlgallery/AtomUIGallery/Workspace/ViewModels/CaseNavigationViewModel.cs");

        registerSource.ShouldContain("locator.Map<OverviewViewModel, OverviewPage>");
        registerSource.ShouldContain("locator.Map<CommunityViewModel, CommunityPage>");
        navigationSource.ShouldContain("_showCaseViewModelFactories.Add(OverviewViewModel.ID");
        navigationSource.ShouldContain("_showCaseViewModelFactories.Add(CommunityViewModel.ID");
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
