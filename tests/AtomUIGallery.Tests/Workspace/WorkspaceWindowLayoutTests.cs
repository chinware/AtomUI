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

        source.ShouldNotContain("Text=\"Desktop Gallery\"");
        source.ShouldContain("avares://AtomUIGallery/Assets/atomui-oss.svg");
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

        source.ShouldContain("<Grid RowDefinitions=\"Auto,*,Auto\">");
        source.ShouldNotContain("<atom:SearchEdit");
        source.ShouldNotContain("Search components...");
        source.ShouldContain("<workspaceviews:CaseNavigation Grid.Row=\"1\"");
        source.ShouldContain("<Border Grid.Row=\"2\"");
    }

    [Fact]
    public void Sidebar_Navigation_Keeps_Two_Pixel_Gap_Between_Scrollbar_And_Divider()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/CaseNavigation.axaml"));

        source.ShouldContain("Margin=\"0,0,2,0\"");
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
    public void Sidebar_Width_Is_Twenty_Pixels_Narrower()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));

        source.ShouldContain("ColumnDefinitions=\"280,*\"");
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
        source.ShouldContain("Grid.ColumnSpan=\"2\"");
        source.ShouldContain("Height=\"1\"");
        source.ShouldContain("Background=\"{atom:SharedTokenResource ColorBorderSecondary}\"");
        source.ShouldContain("IsHitTestVisible=\"False\"");
    }

    [Fact]
    public void Sidebar_Footer_Shows_Website_Gitee_And_Github_Links_With_Larger_Tighter_Icons()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));

        source.ShouldContain("NavigateUri=\"https://www.atomui.net\"");
        source.ShouldContain("Kind=GlobalOutlined");
        source.ShouldContain("NavigateUri=\"https://gitee.com/chinware/AtomUI\"");
        source.ShouldContain("Kind=GiteeOutlined");
        source.ShouldContain("NavigateUri=\"https://github.com/chinware/atomui\"");
        source.ShouldContain("Kind=GithubOutlined");
        source.ShouldContain("Spacing=\"0\"");
        source.ShouldContain("IconWidth=\"22\"");
        source.ShouldContain("IconHeight=\"22\"");
        source.ShouldNotContain("FontSize=\"22\"");
    }

    [Fact]
    public void Sidebar_Footer_Shows_AtomUI_Version_As_Green_Tag()
    {
        var source = File.ReadAllText(GetRepoFile("controlgallery/AtomUIGallery/Workspace/Views/WorkspaceWindow.axaml"));

        source.ShouldContain("<atom:Tag Grid.Column=\"1\"");
        source.ShouldContain("TagColor=\"Green\"");
        source.ShouldContain("Text=\"{x:Static gallery:GalleryVersionInfo.DisplayVersion}\"");
        source.ShouldNotContain("Text=\"v0.9.8\"");
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
