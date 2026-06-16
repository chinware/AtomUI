using System;
using System.IO;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Menu;

public class MenuTitleBarPopupTests
{
    [Fact]
    public void Linux_Csd_TitleBar_MenuItem_Uses_HostWindow_As_Popup_PlacementTarget()
    {
        var menuItemSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Menu/MenuItem.cs"));
        var titleBarSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBar.cs"));

        titleBarSource.ShouldContain("internal Window? HostWindow => _window;");

        menuItemSource.ShouldContain("private Popup? _popup;");
        menuItemSource.ShouldContain("OperatingSystem.IsLinux()");
        menuItemSource.ShouldContain("!IsTopLevel");
        menuItemSource.ShouldContain("TopLevel.GetTopLevel(this) is not null");
        menuItemSource.ShouldContain("FindLogicalAncestorOfType<WindowTitleBar>()");
        menuItemSource.ShouldContain("FindAncestorOfType<WindowTitleBar>()");
        menuItemSource.ShouldContain("window is null || !window.IsCsdEnabled");
        menuItemSource.ShouldContain("_popup.PlacementTarget = window;");
        menuItemSource.ShouldContain("_popup.PlacementRect = new Rect(position.Value, Bounds.Size);");
        menuItemSource.ShouldContain("ConfigureLinuxTitleBarPopupPlacement();");
        menuItemSource.IndexOf("ConfigureLinuxTitleBarPopupPlacement();", StringComparison.Ordinal)
                      .ShouldBeLessThan(menuItemSource.IndexOf("base.OnPropertyChanged(change);",
                          StringComparison.Ordinal));
    }

    [Fact]
    public void Linux_Csd_TitleBar_Menu_Uses_HostWindow_As_DismissRoot()
    {
        var menuSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Menu/Menu.cs"));

        menuSource.ShouldContain("private Window? _linuxTitleBarDismissRoot;");
        menuSource.ShouldContain("ConfigureLinuxTitleBarDismissRoot();");
        menuSource.ShouldContain("ResolveLinuxTitleBarDismissRoot");
        menuSource.ShouldContain("OperatingSystem.IsLinux()");
        menuSource.ShouldContain("TopLevel.GetTopLevel(this) is not null");
        menuSource.ShouldContain("FindLogicalAncestorOfType<WindowTitleBar>()");
        menuSource.ShouldContain("FindAncestorOfType<WindowTitleBar>()");
        menuSource.ShouldContain("window is { IsCsdEnabled: true }");
        menuSource.ShouldContain("InputElement.PointerPressedEvent");
        menuSource.ShouldContain("RoutingStrategies.Tunnel");
        menuSource.ShouldContain("!this.IsLogicalAncestorOf(control)");
        menuSource.ShouldContain("Close();");
        menuSource.ShouldContain("hostWindow.Deactivated += HandleLinuxTitleBarDismissRootDeactivated;");
        menuSource.ShouldContain("DetachLinuxTitleBarDismissRoot();");
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
