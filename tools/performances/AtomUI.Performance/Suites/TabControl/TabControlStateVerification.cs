using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using AvaloniaButton = Avalonia.Controls.Button;
using AtomMenuFlyout = AtomUI.Desktop.Controls.MenuFlyout;
using AtomTabControl = AtomUI.Desktop.Controls.TabControl;
using AtomTabItem = AtomUI.Desktop.Controls.TabItem;
using AtomTabStrip = AtomUI.Desktop.Controls.TabStrip;
using AtomTabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static bool RunTabControlStateVerification()
    {
        var failures = new List<string>();
        VerifyTabItemLazySlots(failures);
        VerifyTabItemCloseBehavior(failures);
        VerifyTabStripItemLazySlots(failures);
        VerifyTabStripItemCloseBehavior(failures);
        VerifyCardAddButtonDoesNotDuplicateRetemplateHandlers(failures);
        VerifyTabControlOverflowFlyoutCleanup(failures);
        VerifyTabControlOverflowMenuReopensAfterVisualReattach(failures);
        VerifyTabStripOverflowMenuReopensAfterVisualReattach(failures);

        if (failures.Count == 0)
        {
            Console.WriteLine("TabControl state verification passed.");
            return true;
        }

        Console.Error.WriteLine("TabControl state verification failed:");
        foreach (var failure in failures)
        {
            Console.Error.WriteLine($"- {failure}");
        }
        return false;
    }

    private static void VerifyTabItemLazySlots(ICollection<string> failures)
    {
        var tabItem = new AtomTabItem
        {
            Header = "Tab"
        };

        using var realized = RealizeControl(tabItem);
        Expect(FindVisualByName<IconPresenter>(tabItem, "ItemIconPresenter") == null,
            "Default TabItem without Icon should not create ItemIconPresenter.",
            failures);
        Expect(FindVisualByName<IconButton>(tabItem, "PART_ItemCloseButton") == null,
            "Default non-closable TabItem should not create PART_ItemCloseButton.",
            failures);

        var icon = new AppleOutlined();
        tabItem.Icon = icon;
        RefreshLayout(realized.Window);
        var iconPresenter = FindVisualByName<IconPresenter>(tabItem, "ItemIconPresenter");
        Expect(iconPresenter != null,
            "TabItem should create ItemIconPresenter when Icon is assigned.",
            failures);
        Expect(ReferenceEquals(iconPresenter?.Icon, icon),
            "TabItem ItemIconPresenter should mirror the assigned Icon.",
            failures);

        tabItem.Icon = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconPresenter>(tabItem, "ItemIconPresenter") == null,
            "TabItem should remove ItemIconPresenter when Icon is cleared.",
            failures);
        Expect(icon.GetVisualParent() == null,
            "Removed TabItem icon should not keep a visual parent.",
            failures);

        tabItem.IsClosable = true;
        tabItem.IsAutoHideCloseButton = false;
        RefreshLayout(realized.Window);
        var closeButton = FindVisualByName<IconButton>(tabItem, "PART_ItemCloseButton");
        Expect(closeButton != null,
            "TabItem should create PART_ItemCloseButton when IsClosable is true.",
            failures);
        Expect(closeButton?.Icon != null,
            "Closable TabItem should provide the default close icon.",
            failures);
        Expect(closeButton?.Opacity == 1.0,
            $"Non-auto-hidden TabItem close button should be opaque, actual {closeButton?.Opacity:0.###}.",
            failures);

        tabItem.IsClosable = false;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconButton>(tabItem, "PART_ItemCloseButton") == null,
            "TabItem should remove PART_ItemCloseButton when IsClosable is false.",
            failures);
        Expect(closeButton?.GetVisualParent() == null,
            "Removed TabItem close button should not keep a visual parent.",
            failures);
    }

    private static void VerifyTabItemCloseBehavior(ICollection<string> failures)
    {
        var tabControl = new AtomTabControl
        {
            IsTabClosable = true
        };
        tabControl.Items.Add(new AtomTabItem { Header = "Tab 1", IsClosable = true });
        tabControl.Items.Add(new AtomTabItem { Header = "Tab 2", IsClosable = true });
        var closedCount = 0;
        tabControl.Closed += (_, _) => closedCount++;

        using var realized = RealizeControl(tabControl);
        var firstItem = tabControl.Items[0] as AtomTabItem;
        var closeButton = firstItem is null
            ? null
            : FindVisualByName<IconButton>(firstItem, "PART_ItemCloseButton");
        closeButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent, closeButton));
        RefreshLayout(realized.Window);

        Expect(tabControl.Items.Count == 1,
            $"TabItem close button should remove one tab, actual count {tabControl.Items.Count}.",
            failures);
        Expect(closedCount == 1,
            $"TabItem close button should raise one Closed event, actual {closedCount}.",
            failures);
    }

    private static void VerifyTabStripItemLazySlots(ICollection<string> failures)
    {
        var tabStripItem = new AtomTabStripItem
        {
            Content = "Tab"
        };

        using var realized = RealizeControl(tabStripItem);
        Expect(FindVisualByName<IconPresenter>(tabStripItem, "ItemIconPresenter") == null,
            "Default TabStripItem without Icon should not create ItemIconPresenter.",
            failures);
        Expect(FindVisualByName<IconButton>(tabStripItem, "PART_ItemCloseButton") == null,
            "Default non-closable TabStripItem should not create PART_ItemCloseButton.",
            failures);

        var icon = new AndroidOutlined();
        tabStripItem.Icon = icon;
        RefreshLayout(realized.Window);
        var iconPresenter = FindVisualByName<IconPresenter>(tabStripItem, "ItemIconPresenter");
        Expect(iconPresenter != null,
            "TabStripItem should create ItemIconPresenter when Icon is assigned.",
            failures);
        Expect(ReferenceEquals(iconPresenter?.Icon, icon),
            "TabStripItem ItemIconPresenter should mirror the assigned Icon.",
            failures);

        tabStripItem.Icon = null;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconPresenter>(tabStripItem, "ItemIconPresenter") == null,
            "TabStripItem should remove ItemIconPresenter when Icon is cleared.",
            failures);
        Expect(icon.GetVisualParent() == null,
            "Removed TabStripItem icon should not keep a visual parent.",
            failures);

        tabStripItem.IsClosable = true;
        tabStripItem.IsAutoHideCloseButton = false;
        RefreshLayout(realized.Window);
        var closeButton = FindVisualByName<IconButton>(tabStripItem, "PART_ItemCloseButton");
        Expect(closeButton != null,
            "TabStripItem should create PART_ItemCloseButton when IsClosable is true.",
            failures);
        Expect(closeButton?.Icon != null,
            "Closable TabStripItem should provide the default close icon.",
            failures);

        tabStripItem.IsClosable = false;
        RefreshLayout(realized.Window);
        Expect(FindVisualByName<IconButton>(tabStripItem, "PART_ItemCloseButton") == null,
            "TabStripItem should remove PART_ItemCloseButton when IsClosable is false.",
            failures);
        Expect(closeButton?.GetVisualParent() == null,
            "Removed TabStripItem close button should not keep a visual parent.",
            failures);
    }

    private static void VerifyTabStripItemCloseBehavior(ICollection<string> failures)
    {
        var tabStrip = new AtomTabStrip
        {
            IsTabClosable = true
        };
        tabStrip.Items.Add(new AtomTabStripItem { Content = "Tab 1", IsClosable = true });
        tabStrip.Items.Add(new AtomTabStripItem { Content = "Tab 2", IsClosable = true });
        var closedCount = 0;
        tabStrip.Closed += (_, _) => closedCount++;

        using var realized = RealizeControl(tabStrip);
        var firstItem = tabStrip.Items[0] as AtomTabStripItem;
        var closeButton = firstItem is null
            ? null
            : FindVisualByName<IconButton>(firstItem, "PART_ItemCloseButton");
        closeButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent, closeButton));
        RefreshLayout(realized.Window);

        Expect(tabStrip.Items.Count == 1,
            $"TabStripItem close button should remove one tab, actual count {tabStrip.Items.Count}.",
            failures);
        Expect(closedCount == 1,
            $"TabStripItem close button should raise one Closed event, actual {closedCount}.",
            failures);
    }

    private static void VerifyCardAddButtonDoesNotDuplicateRetemplateHandlers(ICollection<string> failures)
    {
        var tabControl = new CardTabControl
        {
            IsShowAddTabButton = true
        };
        tabControl.Items.Add(new AtomTabItem { Header = "Tab 1" });
        var tabControlAddCount = 0;
        tabControl.AddTabRequest += (_, _) => tabControlAddCount++;

        using (var realized = RealizeControl(tabControl))
        {
            tabControl.ApplyTemplate();
            RefreshLayout(realized.Window);
            var addButton = FindVisualByName<IconButton>(tabControl, "PART_AddTabButton");
            addButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent, addButton));
            Expect(tabControlAddCount == 1,
                $"CardTabControl add button should raise one AddTabRequest after re-template, actual {tabControlAddCount}.",
                failures);
        }

        var tabStrip = new CardTabStrip
        {
            IsShowAddTabButton = true
        };
        tabStrip.Items.Add(new AtomTabStripItem { Content = "Tab 1" });
        var tabStripAddCount = 0;
        tabStrip.AddTabRequest += (_, _) => tabStripAddCount++;

        using (var realized = RealizeControl(tabStrip))
        {
            tabStrip.ApplyTemplate();
            RefreshLayout(realized.Window);
            var addButton = FindVisualByName<IconButton>(tabStrip, "PART_AddTabButton");
            addButton?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent, addButton));
            Expect(tabStripAddCount == 1,
                $"CardTabStrip add button should raise one AddTabRequest after re-template, actual {tabStripAddCount}.",
                failures);
        }
    }

    private static void VerifyTabControlOverflowFlyoutCleanup(ICollection<string> failures)
    {
        var tabControl = new AtomTabControl
        {
            Width = 260
        };
        for (var i = 0; i < 16; i++)
        {
            tabControl.Items.Add(new AtomTabItem
            {
                Header = $"Long Tab Header {i + 1}",
                Content = $"Content {i + 1}"
            });
        }

        using var realized = RealizeControl(tabControl);
        var scrollViewer = FindVisualByName<TabControlScrollViewer>(tabControl, "PART_TabsContainer");
        var menuIndicator = scrollViewer is null
            ? null
            : FindVisualByName<IconButton>(scrollViewer, "PART_ScrollMenuIndicator");
        Expect(scrollViewer != null,
            "Overflow TabControl should realize PART_TabsContainer.",
            failures);
        Expect(menuIndicator?.IsVisible == true,
            "Overflow TabControl should show the scroll menu indicator.",
            failures);

        var flyout = new AtomUI.Desktop.Controls.MenuFlyout();
        var disposable = new TrackingDisposable();
        if (scrollViewer is not null)
        {
            SetPrivateField(scrollViewer,
                "AtomUI.Desktop.Controls.BaseTabScrollViewer",
                "MenuFlyout",
                flyout);
            SetPrivateField(scrollViewer,
                "AtomUI.Desktop.Controls.TabControlScrollViewer",
                "_flyoutBindingDisposable",
                disposable);
        }

        if (scrollViewer is not null)
        {
            InvokePrivateMethod(scrollViewer,
                "AtomUI.Desktop.Controls.TabControlScrollViewer",
                "HandleMenuFlyoutClosed",
                flyout,
                EventArgs.Empty);
        }

        Expect(scrollViewer is null ||
               GetPrivateField(scrollViewer, "AtomUI.Desktop.Controls.TabControlScrollViewer", "_flyoutBindingDisposable") == null,
            "Closed TabControl MenuFlyout should dispose its motion binding.",
            failures);
        Expect(disposable.IsDisposed,
            "Closed TabControl MenuFlyout should dispose the previous binding disposable.",
            failures);
        Expect(scrollViewer is null ||
               GetPrivateField(scrollViewer, "AtomUI.Desktop.Controls.BaseTabScrollViewer", "MenuFlyout") == null,
            "Closed TabControl MenuFlyout should clear the retained flyout field.",
            failures);
    }

    private static void VerifyTabControlOverflowMenuReopensAfterVisualReattach(ICollection<string> failures)
    {
        var innerTabControl = CreateOverflowTabControl();
        var outerTabControl = new AtomTabControl();
        outerTabControl.Items.Add(new AtomTabItem
        {
            Header  = "TabControl",
            Content = innerTabControl
        });
        outerTabControl.Items.Add(new AtomTabItem
        {
            Header  = "Other",
            Content = new Avalonia.Controls.TextBlock { Text = "Other content" }
        });

        using var realized = RealizeControlInVisualLayerManager(outerTabControl);
        var firstFlyout = OpenTabControlOverflowMenu(innerTabControl,
            realized.Window,
            failures,
            "initial");

        outerTabControl.SelectedIndex = 1;
        RefreshLayout(realized.Window);

        Expect(firstFlyout?.IsOpen == false,
            "Open TabControl overflow flyout should close when its control is detached by a parent tab switch.",
            failures);

        outerTabControl.SelectedIndex = 0;
        RefreshLayout(realized.Window);

        var secondFlyout = OpenTabControlOverflowMenu(innerTabControl,
            realized.Window,
            failures,
            "reattached");

        Expect(secondFlyout is not null && !ReferenceEquals(firstFlyout, secondFlyout),
            "Reattached TabControl overflow menu should open a fresh flyout after parent tab switch.",
            failures);

        outerTabControl.SelectedIndex = 1;
        RefreshLayout(realized.Window);
    }

    private static void VerifyTabStripOverflowMenuReopensAfterVisualReattach(ICollection<string> failures)
    {
        var innerTabStrip = CreateOverflowTabStrip();
        var outerTabControl = new AtomTabControl();
        outerTabControl.Items.Add(new AtomTabItem
        {
            Header  = "TabStrip",
            Content = innerTabStrip
        });
        outerTabControl.Items.Add(new AtomTabItem
        {
            Header  = "Other",
            Content = new Avalonia.Controls.TextBlock { Text = "Other content" }
        });

        using var realized = RealizeControlInVisualLayerManager(outerTabControl);
        var firstFlyout = OpenTabStripOverflowMenu(innerTabStrip,
            realized.Window,
            failures,
            "initial");

        outerTabControl.SelectedIndex = 1;
        RefreshLayout(realized.Window);

        Expect(firstFlyout?.IsOpen == false,
            "Open TabStrip overflow flyout should close when its control is detached by a parent tab switch.",
            failures);

        outerTabControl.SelectedIndex = 0;
        RefreshLayout(realized.Window);

        var secondFlyout = OpenTabStripOverflowMenu(innerTabStrip,
            realized.Window,
            failures,
            "reattached");

        Expect(secondFlyout is not null && !ReferenceEquals(firstFlyout, secondFlyout),
            "Reattached TabStrip overflow menu should open a fresh flyout after parent tab switch.",
            failures);

        outerTabControl.SelectedIndex = 1;
        RefreshLayout(realized.Window);
    }

    private static AtomTabControl CreateOverflowTabControl()
    {
        var tabControl = new AtomTabControl
        {
            Width = 260
        };
        for (var i = 0; i < 16; i++)
        {
            tabControl.Items.Add(new AtomTabItem
            {
                Header  = $"Long Tab Header {i + 1}",
                Content = $"Content {i + 1}"
            });
        }

        return tabControl;
    }

    private static AtomTabStrip CreateOverflowTabStrip()
    {
        var tabStrip = new AtomTabStrip
        {
            Width = 260
        };
        for (var i = 0; i < 16; i++)
        {
            tabStrip.Items.Add(new AtomTabStripItem
            {
                Content = $"Long Tab Header {i + 1}"
            });
        }

        return tabStrip;
    }

    private static AtomMenuFlyout? OpenTabControlOverflowMenu(AtomTabControl tabControl,
                                                              Avalonia.Controls.Window window,
                                                              ICollection<string> failures,
                                                              string phase)
    {
        var scrollViewer  = FindVisualByName<TabControlScrollViewer>(tabControl, "PART_TabsContainer");
        var menuIndicator = scrollViewer is null
            ? null
            : FindVisualByName<IconButton>(scrollViewer, "PART_ScrollMenuIndicator");
        Expect(scrollViewer != null,
            $"Overflow TabControl should realize PART_TabsContainer during {phase} open.",
            failures);
        Expect(menuIndicator?.IsVisible == true,
            $"Overflow TabControl should show the scroll menu indicator during {phase} open.",
            failures);

        try
        {
            menuIndicator?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent, menuIndicator));
        }
        catch (InvalidOperationException exception) when (IsMissingPopupHostException(exception))
        {
            // The headless verification window may not provide a popup host. Creating the flyout still
            // proves the click handler was invoked, which is the lifecycle path being verified here.
        }
        catch (Exception exception)
        {
            failures.Add(
                $"Overflow TabControl menu threw unexpected exception during {phase} click: {exception.GetType().Name}: {exception.Message}");
        }
        finally
        {
            RefreshLayout(window);
        }

        var flyout = scrollViewer is null
            ? null
            : GetPrivateField(scrollViewer,
                "AtomUI.Desktop.Controls.BaseTabScrollViewer",
                "MenuFlyout") as AtomMenuFlyout;
        Expect(flyout is not null,
            $"Overflow TabControl menu should create a flyout during {phase} click.",
            failures);

        return flyout;
    }

    private static AtomMenuFlyout? OpenTabStripOverflowMenu(AtomTabStrip tabStrip,
                                                            Avalonia.Controls.Window window,
                                                            ICollection<string> failures,
                                                            string phase)
    {
        var scrollViewer  = FindVisualByName<TabStripScrollViewer>(tabStrip, "PART_TabsContainer");
        var menuIndicator = scrollViewer is null
            ? null
            : FindVisualByName<IconButton>(scrollViewer, "PART_ScrollMenuIndicator");
        Expect(scrollViewer != null,
            $"Overflow TabStrip should realize PART_TabsContainer during {phase} open.",
            failures);
        Expect(menuIndicator?.IsVisible == true,
            $"Overflow TabStrip should show the scroll menu indicator during {phase} open.",
            failures);

        try
        {
            menuIndicator?.RaiseEvent(new RoutedEventArgs(AvaloniaButton.ClickEvent, menuIndicator));
        }
        catch (InvalidOperationException exception) when (IsMissingPopupHostException(exception))
        {
            // The headless verification window may not provide a popup host. Creating the flyout still
            // proves the click handler was invoked, which is the lifecycle path being verified here.
        }
        catch (Exception exception)
        {
            failures.Add(
                $"Overflow TabStrip menu threw unexpected exception during {phase} click: {exception.GetType().Name}: {exception.Message}");
        }
        finally
        {
            RefreshLayout(window);
        }

        var flyout = scrollViewer is null
            ? null
            : GetPrivateField(scrollViewer,
                "AtomUI.Desktop.Controls.BaseTabScrollViewer",
                "MenuFlyout") as AtomMenuFlyout;
        Expect(flyout is not null,
            $"Overflow TabStrip menu should create a flyout during {phase} click.",
            failures);

        return flyout;
    }

    private static bool IsMissingPopupHostException(InvalidOperationException exception)
    {
        return exception.Message.Contains("IPopupImpl") ||
               exception.Message.Contains("overlay layer");
    }

    private sealed class TrackingDisposable : IDisposable
    {
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}
