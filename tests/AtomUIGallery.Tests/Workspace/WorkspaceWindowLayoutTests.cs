using System;
using System.IO;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Workspace;

public class WorkspaceWindowLayoutTests
{
    [Fact]
    public void Sidebar_Brand_Area_Does_Not_Show_Desktop_Gallery_Text()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));
        var moduleSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/AtomUIGalleryModule.cs"));

        source.ShouldNotContain("Text=\"Desktop Gallery\"");
        source.ShouldNotContain("avares://AtomUIGallery/Assets/atomui-oss.svg");
        source.ShouldContain("ShellHost");
        moduleSource.ShouldContain("avares://AtomUIGallery/Assets/atomui-oss.svg");
        source.ShouldNotContain("avares://AtomUIGallery/Assets/atomui-red.svg");
    }

    [Fact]
    public void Sidebar_Navigation_Does_Not_Use_Fixed_Menu_Width()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml"));

        source.ShouldNotContain("Width=\"260\"");
        source.ShouldContain("HorizontalAlignment=\"Stretch\"");
    }

    [Fact]
    public void Sidebar_Does_Not_Show_Search_Box()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));

        source.ShouldContain("Name=\"ShellHost\"");
        source.ShouldNotContain("<atom:SearchEdit");
        source.ShouldNotContain("Search components...");
        source.ShouldNotContain("<workspaceviews:CaseNavigation Grid.Row=\"1\"");
        source.ShouldNotContain("<Border Grid.Row=\"2\"");
    }

    [Fact]
    public void Sidebar_Navigation_Does_Not_Reserve_Divider_Gap()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml"));

        source.ShouldNotContain("Margin=\"0,0,2,0\"");
    }

    [Fact]
    public void Workspace_Draws_Navigation_Content_Separator()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Shell/GalleryShellView.cs"));

        source.ShouldContain("Name             = \"WorkspaceNavigationSeparator\"");
        source.ShouldContain("Grid.SetColumn(_navigationSeparator, 1)");
        source.ShouldContain("Width            = 1");
        source.ShouldContain("HorizontalAlignment = HorizontalAlignment.Left");
        source.ShouldContain("SharedTokenKind.ColorBorderSecondary");
        source.ShouldContain("IsHitTestVisible = false");
        source.ShouldNotContain("BorderThickness=\"0,0,1,0\"");
    }

    [Fact]
    public void Sidebar_Navigation_Does_Not_Override_Selected_Background()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml"));

        source.ShouldNotContain("<UserControl.Styles>");
        source.ShouldNotContain("BaseNavMenuItemHeader[IsSelected=True]");
        source.ShouldNotContain("IsDarkStyle=True][IsSelected=True]");
        source.ShouldNotContain("Value=\"#");
    }

    [Fact]
    public void Sidebar_Navigation_Does_Not_Couple_Dark_Menu_Style_To_Global_Dark_Mode()
    {
        var viewSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml"));
        var codeBehindSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml.cs"));

        viewSource.ShouldNotContain("IsDarkStyle=\"True\"");
        codeBehindSource.ShouldNotContain("IThemeManager.IsDarkThemeModeProperty");
        codeBehindSource.ShouldNotContain("NavMenu.IsDarkStyleProperty");
        codeBehindSource.ShouldContain("ShowCaseNavMenu");
    }

    [Fact]
    public void Sidebar_Width_Is_Twenty_Pixels_Narrower()
    {
        var configuration = global::AtomUIGallery.AtomUIGalleryModule.CreateConfiguration();

        configuration.Shell.SidebarWidth.ShouldBe(280);
    }

    [Fact]
    public void Workspace_Window_Has_Minimum_Width_To_Protect_Main_Content()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));

        source.ShouldContain("MinWidth=\"520\"");
        source.ShouldNotContain("MinWidth=\"1040\"");
        source.ShouldNotContain("MinWidth=\"1200\"");
    }

    [Fact]
    public void Workspace_Window_Draws_TitleBar_Bottom_Separator_With_Secondary_Border_Color()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));

        source.ShouldContain("Name=\"TitleBarBottomSeparator\"");
        source.ShouldContain("Height=\"1\"");
        source.ShouldContain("Background=\"{atom:SharedTokenResource ColorBorderSecondary}\"");
        source.ShouldContain("IsHitTestVisible=\"False\"");
    }

    [Fact]
    public void Sidebar_Footer_Shows_Website_Gitee_And_Github_Links_With_Larger_Tighter_Icons()
    {
        var moduleSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/AtomUIGalleryModule.cs"));
        var shellSource = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Shell/GalleryShellView.cs"));

        moduleSource.ShouldContain("https://www.atomui.net");
        moduleSource.ShouldContain("AntDesignIconKind.GlobalOutlined");
        moduleSource.ShouldContain("https://gitee.com/chinware/AtomUI");
        moduleSource.ShouldContain("AntDesignIconKind.GiteeOutlined");
        moduleSource.ShouldContain("https://github.com/chinware/atomui");
        moduleSource.ShouldContain("AntDesignIconKind.GithubOutlined");
        shellSource.ShouldContain("Spacing     = 0");
        shellSource.ShouldContain("IconWidth   = 22");
        shellSource.ShouldContain("IconHeight  = 22");
        shellSource.ShouldNotContain("FontSize = 22");
    }

    [Fact]
    public void Sidebar_Footer_Shows_AtomUI_Version_As_Green_Tag()
    {
        var moduleSource = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/AtomUIGalleryModule.cs"));
        var shellSource = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Shell/GalleryShellView.cs"));

        shellSource.ShouldContain("new DesktopTag");
        shellSource.ShouldContain("TagColor            = \"Green\"");
        moduleSource.ShouldContain("GalleryVersionInfo.DisplayVersion");
        shellSource.ShouldNotContain("Text = \"v0.9.8\"");
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

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
