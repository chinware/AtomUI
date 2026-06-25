using System;
using System.IO;
using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Tests.SourceCode;

public class GallerySourceCodeDisplayLayoutTests
{
    [Fact]
    public void Drawer_Content_Uses_AtomUI_Tabs_And_Stretching_Code_Viewer()
    {
        var contentSource = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryShowCaseCodeDrawerContent.cs"));
        var viewerMarkup  = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryCodeViewer.axaml"));

        contentSource.ShouldContain("DesktopTabControl");
        contentSource.ShouldContain("DesktopTabItem");
        contentSource.ShouldContain("SelectedIndex = 0");
        contentSource.ShouldContain("HorizontalContentAlignment = HorizontalAlignment.Stretch");
        contentSource.ShouldContain("VerticalContentAlignment = VerticalAlignment.Stretch");
        contentSource.ShouldContain("ContentPadding = new Thickness(1)");
        contentSource.ShouldNotContain("MinHeight = 520");
        viewerMarkup.ShouldContain("Name=\"PART_Editor\"");
        viewerMarkup.ShouldContain("HorizontalAlignment=\"Stretch\"");
        viewerMarkup.ShouldContain("VerticalAlignment=\"Stretch\"");
    }

    [Fact]
    public void Drawer_Content_Uses_Source_Code_Specific_Spacing()
    {
        var contentSource = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryShowCaseCodeDrawerContent.cs"));
        var hostSource    = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryShowCaseCodeDrawerHost.cs"));

        hostSource.ShouldContain("ContentPadding = new Thickness(8, 1, 1, 1)");
        contentSource.ShouldNotContain("DrawerContentPaddingCompensation");
        contentSource.ShouldNotContain("new Thickness(-");
        contentSource.ShouldContain("HeaderStartEdgePadding = 0");
        contentSource.ShouldContain("HeaderEndEdgePadding = 0");
        contentSource.ShouldContain("TabAndContentGutter = 2");
        contentSource.ShouldNotContain("Content = new Border");
    }

    [Fact]
    public void Code_Viewer_Includes_AvaloniaEdit_TextEditor_Theme()
    {
        var viewerMarkup = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryCodeViewer.axaml"));

        viewerMarkup.ShouldContain("StyleInclude");
        viewerMarkup.ShouldContain("avares://AvaloniaEdit/Themes/Simple/AvaloniaEdit.xaml");
        viewerMarkup.ShouldNotContain("avares://AvaloniaEdit/Themes/Fluent/AvaloniaEdit.xaml");
        viewerMarkup.ShouldContain("x:Key=\"FontSizeNormal\"");
        viewerMarkup.ShouldContain("x:Key=\"ContentControlThemeFontFamily\"");
        viewerMarkup.ShouldContain("x:Key=\"ThemeBackgroundBrush\"");
        viewerMarkup.ShouldContain("x:Key=\"ThemeForegroundColor\"");
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
