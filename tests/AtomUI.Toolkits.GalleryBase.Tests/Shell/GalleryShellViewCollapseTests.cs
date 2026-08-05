using AtomUI.Desktop.Controls;
using AtomUI.Toolkits.GalleryBase.Configuration;
using AtomUI.Toolkits.GalleryBase.Shell;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ReactiveUI;
using ReactiveUI.Avalonia;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Toolkits.GalleryBase.Tests.Shell;

public class GalleryShellViewCollapseTests
{
    [Fact]
    public void Collapsible_Navigation_Host_Follows_NavMenu_Effective_Width()
    {
        AvaloniaTestApp.EnsureInitialized();
        var navigationView = new TestCollapsibleNavigationView();
        using var shell = new GalleryShellView(CreateConfiguration(), navigationView, new RoutingState());
        var window = ShowInWindow(shell);

        try
        {
            var sidebar = FindNamedBorder(shell, "WorkspaceSidebar");

            navigationView.SidebarNavMenu.Width.ShouldBe(280);
            sidebar.Width.ShouldBe(280);

            navigationView.SidebarNavMenu.IsInlineCollapsed = true;
            Dispatcher.UIThread.RunJobs();

            navigationView.SidebarNavMenu.Width.ShouldBe(64);
            sidebar.Width.ShouldBe(64);

            navigationView.SidebarNavMenu.IsInlineCollapsed = false;
            Dispatcher.UIThread.RunJobs();

            navigationView.SidebarNavMenu.Width.ShouldBe(280);
            sidebar.Width.ShouldBe(280);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Plain_Navigation_View_Keeps_Configured_Fixed_Sidebar_Width()
    {
        AvaloniaTestApp.EnsureInitialized();
        using var shell = new GalleryShellView(CreateConfiguration(), new Border(), new RoutingState());
        var window = ShowInWindow(shell);

        try
        {
            var sidebar = FindNamedBorder(shell, "WorkspaceSidebar");
            var rootLayout = FindNamedGrid(shell, "WorkspaceRootLayout");

            sidebar.Width.ShouldBe(double.NaN);
            rootLayout.ColumnDefinitions[0].Width.ShouldBe(new GridLength(280));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Dispose_Releases_Collapsible_Navigation_Width_Binding()
    {
        AvaloniaTestApp.EnsureInitialized();
        var navigationView = new TestCollapsibleNavigationView();
        var shell = new GalleryShellView(CreateConfiguration(), navigationView, new RoutingState());
        var window = ShowInWindow(shell);

        try
        {
            var sidebar = FindNamedBorder(shell, "WorkspaceSidebar");

            sidebar.Width.ShouldBe(280);
            shell.Dispose();
            double.IsNaN(sidebar.Width).ShouldBeTrue();

            navigationView.SidebarNavMenu.IsInlineCollapsed = true;
            Dispatcher.UIThread.RunJobs();

            double.IsNaN(sidebar.Width).ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Brand_Header_Action_And_Footer_Follow_The_Effective_Inline_Collapsed_State()
    {
        AvaloniaTestApp.EnsureInitialized();
        var navigationView = new TestCollapsibleNavigationView();
        using var shell = new GalleryShellView(CreateConfiguration(), navigationView, new RoutingState());
        var window = ShowInWindow(shell);

        try
        {
            var brandHeader = FindNamedGrid(shell, "WorkspaceBrandHost");
            var brandContent = FindNamedBorder(shell, "WorkspaceBrandContent");
            var actionHost = FindNamedBorder(shell, "WorkspaceSidebarHeaderActionHost");
            var footer = FindNamedBorder(shell, "WorkspaceSidebarFooter");

            actionHost.Child.ShouldBeSameAs(navigationView.SidebarHeaderAction);
            brandHeader.Margin.ShouldBe(new Thickness(0, 20, 0, 16));
            brandContent.IsVisible.ShouldBeTrue();
            actionHost.IsVisible.ShouldBeTrue();
            actionHost.HorizontalAlignment.ShouldBe(HorizontalAlignment.Right);
            actionHost.Margin.ShouldBe(new Thickness(0, 0, 4, 0));

            navigationView.SidebarNavMenu.IsInlineCollapsed = true;
            Dispatcher.UIThread.RunJobs();

            brandHeader.IsVisible.ShouldBeTrue();
            brandHeader.Margin.ShouldBe(new Thickness(0, 20, 0, 0));
            brandContent.IsVisible.ShouldBeFalse();
            actionHost.IsVisible.ShouldBeTrue();
            actionHost.HorizontalAlignment.ShouldBe(HorizontalAlignment.Center);
            actionHost.Margin.ShouldBe(default);
            footer.IsVisible.ShouldBeFalse();

            navigationView.SidebarNavMenu.Mode = NavMenuMode.Vertical;
            Dispatcher.UIThread.RunJobs();

            brandContent.IsVisible.ShouldBeTrue();
            actionHost.HorizontalAlignment.ShouldBe(HorizontalAlignment.Right);
            actionHost.Margin.ShouldBe(new Thickness(0, 0, 4, 0));
            footer.IsVisible.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    private static AvaloniaWindow ShowInWindow(Control content)
    {
        var window = new AvaloniaWindow
        {
            Width = 1000,
            Height = 700,
            Content = content
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static Border FindNamedBorder(Control root, string name)
    {
        return root.GetVisualDescendants()
                   .OfType<Border>()
                   .Single(control => control.Name == name);
    }

    private static Grid FindNamedGrid(Control root, string name)
    {
        return FindNamedControl<Grid>(root, name);
    }

    private static TControl FindNamedControl<TControl>(Control root, string name)
        where TControl : Control
    {
        return root.GetVisualDescendants()
                   .OfType<TControl>()
                   .Single(control => control.Name == name);
    }

    private static GalleryBaseConfiguration CreateConfiguration()
    {
        var options = new GalleryBaseOptions();
        options.Branding.VersionText = "v1";
        options.Navigation.DefaultRoute = "Overview";
        options.Navigation.AddPage("Overview", "Overview");
        options.Routes.Map(
            "Overview",
            screen => new TestRouteViewModel(screen),
            () => new TestRouteView());
        return options.BuildConfiguration();
    }

    private sealed class TestCollapsibleNavigationView : UserControl, IGallerySidebarNavMenuHost
    {
        public NavMenu SidebarNavMenu { get; } = new()
        {
            InlineCollapsedWidth = 64,
            IsMotionEnabled = false
        };

        public Control? SidebarHeaderAction { get; } = new Border
        {
            Width  = 40,
            Height = 40
        };

        public TestCollapsibleNavigationView()
        {
            Content = SidebarNavMenu;
        }
    }

    private sealed class TestRouteViewModel(IScreen hostScreen) : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "Overview";

        public IScreen HostScreen { get; } = hostScreen;
    }

    private sealed class TestRouteView : ReactiveUserControl<TestRouteViewModel>
    {
    }
}
