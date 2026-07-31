using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIContextMenu = AtomUI.Desktop.Controls.ContextMenu;
using AtomUIMenu = AtomUI.Desktop.Controls.Menu;
using AtomUIMenuItem = AtomUI.Desktop.Controls.MenuItem;
using AtomUIScrollViewer = AtomUI.Desktop.Controls.ScrollViewer;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Menu;

public class MenuPopupScrollModeTests
{
    static MenuPopupScrollModeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Menu_Family_Defaults_Popup_Scroll_To_Enabled()
    {
        new AtomUIMenu().IsScrollEnabled.ShouldBeTrue();
        new AtomUIContextMenu().IsScrollEnabled.ShouldBeTrue();
        new AtomUIMenuItem().IsScrollEnabled.ShouldBeTrue();
        new MenuFlyout().IsScrollEnabled.ShouldBeTrue();
        new MenuFlyoutPresenter().IsScrollEnabled.ShouldBeTrue();
    }

    [Fact]
    public void MenuItem_Inherits_Scroll_Mode_From_Menu_And_Allows_Local_Override()
    {
        var menu  = new AtomUIMenu { IsScrollEnabled = false };
        var child = new AtomUIMenuItem();

        menu.Items.Add(child);

        child.IsScrollEnabled.ShouldBeFalse();

        child.IsScrollEnabled = true;

        child.IsScrollEnabled.ShouldBeTrue();
    }

    [Fact]
    public void MenuItem_Inherits_Scroll_Mode_From_Parent_MenuItem()
    {
        var parent = new AtomUIMenuItem { IsScrollEnabled = false };
        var child  = new AtomUIMenuItem();

        parent.Items.Add(child);

        child.IsScrollEnabled.ShouldBeFalse();
    }

    [Fact]
    public void ContextMenu_Item_Inherits_Scroll_Mode_From_ContextMenu()
    {
        var contextMenu = new AtomUIContextMenu { IsScrollEnabled = false };
        var child       = new AtomUIMenuItem();

        contextMenu.Items.Add(child);

        child.IsScrollEnabled.ShouldBeFalse();
    }

    [Fact]
    public void MenuFlyout_Relays_Scroll_Mode_To_Presenter()
    {
        var flyout = new TestMenuFlyout { IsScrollEnabled = false };

        var presenter = flyout.CreatePresenterForTest();

        presenter.IsScrollEnabled.ShouldBeFalse();
    }

    [Fact]
    public void ContextMenu_MaxPopupHeight_Uses_DisplayPageSize_Only_When_Scroll_Is_Enabled()
    {
        var contextMenu = new AtomUIContextMenu
        {
            ItemHeight      = 20,
            DisplayPageSize = 8,
            Padding         = new Thickness(1, 2, 3, 4)
        };

        contextMenu.MaxPopupHeight.ShouldBe(166);

        contextMenu.IsScrollEnabled = false;

        contextMenu.MaxPopupHeight.ShouldBe(double.PositiveInfinity);
    }

    [Fact]
    public void MenuItem_MaxPopupHeight_Uses_DisplayPageSize_Only_When_Scroll_Is_Enabled()
    {
        var menuItem = new AtomUIMenuItem
        {
            ItemHeight      = 20,
            DisplayPageSize = 8,
            PopupPadding    = new Thickness(1, 2, 3, 4)
        };

        menuItem.MaxPopupHeight.ShouldBe(166);

        menuItem.IsScrollEnabled = false;

        menuItem.MaxPopupHeight.ShouldBe(double.PositiveInfinity);
    }

    [Fact]
    public void MenuFlyoutPresenter_MaxPopupHeight_Uses_DisplayPageSize_Only_When_Scroll_Is_Enabled()
    {
        var presenter = new MenuFlyoutPresenter
        {
            ItemHeight      = 20,
            DisplayPageSize = 8,
            Padding         = new Thickness(1, 2, 3, 4)
        };

        presenter.MaxPopupHeight.ShouldBe(166);

        presenter.IsScrollEnabled = false;

        presenter.MaxPopupHeight.ShouldBe(double.PositiveInfinity);
    }

    [Fact]
    public void MenuPopupScrollHost_Creates_ScrollViewer_Only_When_Scroll_Is_Enabled()
    {
        var enabledHost = new MenuPopupScrollHost
        {
            IsScrollEnabled = true,
            Content         = new TextBlock { Text = "Item" }
        };

        ShowInWindow(enabledHost, () =>
        {
            enabledHost.GetVisualDescendants()
                       .OfType<AtomUIScrollViewer>()
                       .Count()
                       .ShouldBe(1);
        });

        var disabledHost = new MenuPopupScrollHost
        {
            IsScrollEnabled = false,
            Content         = new TextBlock { Text = "Item" }
        };

        ShowInWindow(disabledHost, () =>
        {
            disabledHost.GetVisualDescendants()
                        .OfType<AtomUIScrollViewer>()
                        .ShouldBeEmpty();
        });
    }

    [Fact]
    public void Popup_Templates_Route_ItemsPresenter_Through_MenuPopupScrollHost()
    {
        AssertPopupTemplateUsesScrollHost("src/AtomUI.Desktop.Controls/Menu/Themes/ContextMenuTheme.axaml");
        AssertPopupTemplateUsesScrollHost("src/AtomUI.Desktop.Controls/Menu/Themes/MenuItemTheme.axaml");
        AssertPopupTemplateUsesScrollHost("src/AtomUI.Desktop.Controls/Menu/Themes/TopLevelMenuItemTheme.axaml");
        AssertPopupTemplateUsesScrollHost("src/AtomUI.Desktop.Controls/Flyouts/Themes/MenuFlyoutPresenterTheme.axaml");
    }

    [Fact]
    public void MenuPopupScrollHost_Theme_Owns_The_Only_Popup_ScrollViewer_Branch()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/Menu/Themes/MenuPopupScrollHostTheme.axaml");

        source.ShouldContain("<atom:ScrollViewer");
        source.ShouldContain("IsScrollChainingEnabled=\"False\"");
        source.ShouldContain("IsMotionEnabled=\"{TemplateBinding IsMotionEnabled}\"");
        source.ShouldContain("IsLiteMode=\"{TemplateBinding atom:ScrollViewer.IsLiteMode}\"");
        source.ShouldContain("AllowAutoHide=\"{TemplateBinding AllowAutoHide}\"");
        source.ShouldContain("^[IsScrollEnabled=False]");
        source.ShouldNotContain("<ItemsPresenter");
    }

    private static void AssertPopupTemplateUsesScrollHost(string relativePath)
    {
        var source = ReadRepoFile(relativePath);

        source.ShouldContain("<atom:MenuPopupScrollHost");
        source.ShouldContain("IsScrollEnabled=\"{TemplateBinding IsScrollEnabled}\"");
        source.ShouldContain("IsMotionEnabled=\"{TemplateBinding IsMotionEnabled}\"");
        source.ShouldContain("Name=\"PART_ItemsPresenter\"");
        source.ShouldNotContain("<atom:ScrollViewer");
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 240,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            content.ApplyTemplate();
            Dispatcher.UIThread.RunJobs();

            assertion();
        }
        finally
        {
            window.Close();
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

    private sealed class TestMenuFlyout : MenuFlyout
    {
        public MenuFlyoutPresenter CreatePresenterForTest()
        {
            return (MenuFlyoutPresenter)CreatePresenter();
        }
    }
}
