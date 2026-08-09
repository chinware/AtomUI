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

    [Fact]
    public void Code_Viewer_Overrides_AvaloniaEdit_SearchPanel_With_Local_AtomUI_Theme()
    {
        var viewerMarkup = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryCodeViewer.axaml"));
        var searchPanelTheme = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Toolkits.GalleryBase/Controls/GalleryCodeViewerSearchPanelTheme.axaml"));

        viewerMarkup.ShouldContain("Controls/GalleryCodeViewerSearchPanelTheme.axaml");
        viewerMarkup.ShouldNotContain("Controls/Themes/GalleryCodeViewerSearchPanelTheme.axaml");
        searchPanelTheme.ShouldContain("x:Key=\"{x:Type search:SearchPanel}\"");
        searchPanelTheme.ShouldContain("atom:LineEdit");
        searchPanelTheme.ShouldContain("atom:IconButton");
        searchPanelTheme.ShouldContain("atom:ToggleIconButton");
        searchPanelTheme.ShouldNotContain("Avalonia.Controls.Primitives.ToggleButton");
        searchPanelTheme.ShouldNotContain("BasedOn=\"{StaticResource {x:Type ToggleButton}}\"");
        searchPanelTheme.ShouldNotContain("ToolTip.Tip");
        searchPanelTheme.ShouldContain("SizeType=\"Middle\"");
        searchPanelTheme.ShouldContain("{atom:SharedTokenResource ControlHeight}");
        searchPanelTheme.ShouldContain("{atom:SharedTokenResource IconSize}");
        searchPanelTheme.ShouldNotContain("SizeType=\"Small\"");
        searchPanelTheme.ShouldNotContain("Width=\"28\"");
        searchPanelTheme.ShouldNotContain("Height=\"28\"");
        searchPanelTheme.ShouldNotContain("Width=\"24\"");
        searchPanelTheme.ShouldNotContain("Height=\"24\"");

        var innerRightContent = ExtractSearchPanelInnerRightContent(searchPanelTheme);
        innerRightContent.ShouldContain("{atom:SharedTokenResource ControlInteractiveSize}");
        innerRightContent.ShouldContain("{atom:SharedTokenResource IconSizeSM}");
        innerRightContent.ShouldNotContain("Width=\"{atom:SharedTokenResource ControlHeight}\"");
        innerRightContent.ShouldNotContain("Height=\"{atom:SharedTokenResource ControlHeight}\"");
    }

    [Fact]
    public void Code_Viewer_Does_Not_Keep_A_Permanent_LayoutUpdated_Scrollbar_Adjuster()
    {
        var viewerSource = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryCodeViewer.cs"));

        var requestMethodIndex = viewerSource.IndexOf(
            "private void RequestScrollBarInsetUpdate()",
            StringComparison.Ordinal);
        var subscriptionIndex = viewerSource.IndexOf(
            "_editor.LayoutUpdated += HandleEditorLayoutUpdated",
            StringComparison.Ordinal);
        var handlerIndex = viewerSource.IndexOf(
            "private void HandleEditorLayoutUpdated",
            StringComparison.Ordinal);
        var cancelInHandlerIndex = viewerSource.IndexOf(
            "CancelScrollBarInsetUpdate();",
            handlerIndex,
            StringComparison.Ordinal);
        var updateInHandlerIndex = viewerSource.IndexOf(
            "UpdateScrollBarInsets();",
            handlerIndex,
            StringComparison.Ordinal);

        requestMethodIndex.ShouldBeGreaterThanOrEqualTo(0);
        subscriptionIndex.ShouldBeGreaterThan(requestMethodIndex);
        cancelInHandlerIndex.ShouldBeLessThan(updateInHandlerIndex);
    }

    [Fact]
    public void Drawer_Content_Initializes_Code_Viewer_Source_Atomically()
    {
        var contentSource = File.ReadAllText(GetRepoFile("src/AtomUI.Toolkits.GalleryBase/Controls/GalleryShowCaseCodeDrawerContent.cs"));

        contentSource.ShouldContain("new GalleryCodeViewer(snippet.Text, snippet.Language)");
        contentSource.ShouldNotContain("CodeText = snippet.Text");
        contentSource.ShouldNotContain("Language = snippet.Language");
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

    private static string ExtractSearchPanelInnerRightContent(string searchPanelTheme)
    {
        const string startMarker = "<TextBox.InnerRightContent>";
        const string endMarker   = "</TextBox.InnerRightContent>";

        var start = searchPanelTheme.IndexOf(startMarker, StringComparison.Ordinal);
        start.ShouldBeGreaterThanOrEqualTo(0);

        var end = searchPanelTheme.IndexOf(endMarker, start, StringComparison.Ordinal);
        end.ShouldBeGreaterThan(start);

        return searchPanelTheme.Substring(start, end - start);
    }
}
