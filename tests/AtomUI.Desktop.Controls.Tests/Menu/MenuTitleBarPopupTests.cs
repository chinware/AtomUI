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
        var supportSource  = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Popup/LinuxCsdPopupSupport.cs"));
        var titleBarSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/WindowTitleBar/WindowTitleBar.cs"));

        titleBarSource.ShouldContain("internal Window? HostWindow => _window;");

        menuItemSource.ShouldContain("private Popup? _popup;");
        menuItemSource.ShouldContain("private IDisposable? _linuxCsdPopupPlacementTracker;");
        menuItemSource.ShouldContain("LinuxCsdPopupSupport.ConfigurePopupPlacement");
        menuItemSource.ShouldContain("LinuxCsdPopupSupport.ClearPopupPlacement");
        menuItemSource.ShouldContain("LinuxCsdPopupSupport.UpdatePopupPlacementTracker");
        menuItemSource.ShouldContain("LinuxCsdPopupSupport.ClearPopupPlacementTracker");
        menuItemSource.ShouldContain("IsTopLevel && IsSubMenuOpen");
        menuItemSource.ShouldNotContain("OperatingSystem.IsLinux()");
        menuItemSource.ShouldNotContain("FindLogicalAncestorOfType<WindowTitleBar>()");
        menuItemSource.ShouldNotContain("PlacementTarget =");
        menuItemSource.ShouldNotContain("PlacementRect =");
        menuItemSource.IndexOf("ConfigureLinuxCsdPopupPlacement();", StringComparison.Ordinal)
                      .ShouldBeLessThan(menuItemSource.IndexOf("base.OnPropertyChanged(change);",
                          StringComparison.Ordinal));

        supportSource.ShouldContain("ConfigurePopupPlacement(Control anchor, Popup? popup");
        supportSource.ShouldContain("UpdatePopupPlacementTracker(");
        supportSource.ShouldContain("ClearPopupPlacementTracker(ref IDisposable? current)");
        supportSource.ShouldContain("PopupPlacementRegistration");
        supportSource.ShouldContain("_anchor.GetObservable(Visual.BoundsProperty)");
        supportSource.ShouldContain("_anchor.LayoutUpdated += HandleAnchorLayoutUpdated;");
        supportSource.ShouldContain("if (popup.PlacementRect != placementRect)");
        supportSource.ShouldNotContain("MenuItem");
        supportSource.ShouldNotContain("Menu menu");
        supportSource.ShouldContain("popup.PlacementTarget = hostWindow;");
        supportSource.ShouldContain("popup.PlacementRect = placementRect;");
        supportSource.ShouldContain("new Rect(position.Value, anchor.Bounds.Size)");
    }

    [Fact]
    public void Linux_Csd_TitleBar_Menu_Uses_HostWindow_As_DismissRoot()
    {
        var menuSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Menu/Menu.cs"));
        var supportSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Popup/LinuxCsdPopupSupport.cs"));
        var windowSource = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Window.cs"));

        menuSource.ShouldContain("private IDisposable? _linuxCsdPopupDismissRoot;");
        menuSource.ShouldContain("ConfigureLinuxCsdPopupDismissRoot();");
        menuSource.ShouldContain("LinuxCsdPopupSupport.UpdateDismissRoot");
        menuSource.ShouldContain("LinuxCsdPopupSupport.ClearDismissRoot");
        menuSource.ShouldContain("RelayLinuxCsdPopupClickToHostWindow");
        menuSource.ShouldContain("SyncLinuxCsdRadioGroup");
        menuSource.ShouldContain("MenuItem.ClickEvent");
        menuSource.ShouldContain("MenuItem.IsCheckStateChangedEvent");
        menuSource.ShouldContain("hostWindow.RaiseRoutedEventFromOverlay((MenuItem)e.Source, relayedArgs);");
        menuSource.ShouldContain("UncheckSiblingRadioItems");
        menuSource.ShouldContain("UncheckNamedRadioGroup");
        menuSource.ShouldContain("SetCurrentValue(MenuItem.IsCheckedProperty, false);");
        menuSource.ShouldContain("IsInteractionInsideMenu");
        menuSource.ShouldContain("current = current.Parent;");
        menuSource.ShouldNotContain("OperatingSystem.IsLinux()");
        menuSource.ShouldNotContain("InputElement.PointerPressedEvent");
        menuSource.ShouldNotContain("RoutingStrategies.Tunnel");
        menuSource.ShouldNotContain("FindLogicalAncestorOfType<WindowTitleBar>()");

        supportSource.ShouldContain("OperatingSystem.IsLinux()");
        supportSource.ShouldContain("TopLevel.GetTopLevel(control) is not null");
        supportSource.ShouldContain("FindLogicalAncestorOfType<WindowTitleBar>()");
        supportSource.ShouldContain("FindAncestorOfType<WindowTitleBar>()");
        supportSource.ShouldContain("window is not { IsCsdEnabled: true }");
        supportSource.ShouldContain("TryResolveLinuxCsdHostWindow(Control control, out Window hostWindow)");
        supportSource.ShouldContain("Func<bool> isOpen");
        supportSource.ShouldContain("Action dismiss");
        supportSource.ShouldContain("Func<ILogical, bool> isInside");
        supportSource.ShouldContain("InputElement.PointerPressedEvent");
        supportSource.ShouldContain("RoutingStrategies.Tunnel");
        supportSource.ShouldContain("!_isInside(control)");
        supportSource.ShouldContain("_dismiss();");
        supportSource.ShouldContain("_root.Deactivated += HandleRootDeactivated;");

        windowSource.ShouldContain("RaiseRoutedEventFromOverlay(Interactive source, RoutedEventArgs args)");
        windowSource.ShouldContain("using var route = BuildEventRoute(args.RoutedEvent);");
        windowSource.ShouldContain("route.RaiseEvent(source, args);");
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
