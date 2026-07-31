using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Workspace;

public class BrowserGalleryConventionsTests
{
    [Fact]
    public void Browser_Gallery_Uses_Desktop_Workspace_Navigation_And_Routing()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery.Browser/BrowserGalleryView.cs");

        source.ShouldContain("GalleryBrowserShellView");
        source.ShouldContain("new WorkspaceWindowViewModel()");
        source.ShouldContain("using AtomUIGallery.Workspace.ViewModels;");
        source.ShouldContain("using AtomUIGallery.Workspace.Views;");
        source.ShouldContain("new CaseNavigation");
        source.ShouldContain("ViewModel = viewModel.CaseNavigation");
        source.ShouldNotContain("new ColumnDefinitions(\"280,*\")");
        source.ShouldNotContain("ColorBgLayout");
        source.ShouldNotContain("ColorBgContainer");
    }

    [Fact]
    public void Browser_Gallery_Does_Not_Keep_Legacy_Header_Page_Factory_Or_Preload_Warmup()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery.Browser/BrowserGalleryView.cs");

        source.ShouldNotContain("AtomUI Browser Gallery");
        source.ShouldNotContain("BrowserGalleryPageKind");
        source.ShouldNotContain("CreatePage(");
        source.ShouldNotContain("s_pagesToPreload");
        source.ShouldNotContain("DispatcherTimer");
        source.ShouldNotContain("BeginPageWarmup");
        source.ShouldNotContain("CompletePageWarmup");
        source.ShouldNotContain("CancelPageWarmup");
        source.ShouldNotContain("_contentHost.Children.Remove");
        source.ShouldNotContain("AboutUsPage");
        source.ShouldNotContain("AboutUsViewModel");
    }

    [Fact]
    public void Browser_Gallery_Delegates_Shell_Branding_And_Overlay_To_GalleryBase()
    {
        var source = ReadRepoFile("controlgallery/AtomUIGallery.Browser/BrowserGalleryView.cs");
        var galleryBaseSource = ReadRepoFile("src/AtomUI.Toolkits.GalleryBase/Shell/GalleryBrowserShellView.cs");

        source.ShouldNotContain("avares://AtomUIGallery/Assets/atomui-oss.svg");
        source.ShouldNotContain("https://www.atomui.net");
        source.ShouldNotContain("https://gitee.com/chinware/AtomUI");
        source.ShouldNotContain("https://github.com/chinware/atomui");
        source.ShouldNotContain("ConfigureOverlayLayers");
        source.ShouldNotContain("BindingFlags.Instance | BindingFlags.NonPublic");

        galleryBaseSource.ShouldContain("ConfigureOverlayLayers");
        galleryBaseSource.ShouldContain("EnableBrowserMediaBreakpoints");
        galleryBaseSource.ShouldContain("IMediaBreakAwareControl");
        galleryBaseSource.ShouldContain("VisualLayerManager");
    }

    private static string ReadRepoFile(string relativePath)
    {
        var path = Path.Combine(GetRepoRoot(), relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
    }

    private static string GetRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "controlgallery/AtomUIGallery.Browser");
            if (Directory.Exists(candidate))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        return AppContext.BaseDirectory;
    }
}
