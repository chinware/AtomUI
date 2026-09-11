using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuInteractionHandlerTests
{
    static NavMenuInteractionHandlerTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Layout_Induced_Pointer_Exit_Does_Not_Schedule_Submenu_Close()
    {
        var fixture = CreateScrollableHorizontalMenuFixture();
        var delayedRuns = new List<ScheduledRun>();
        var handler = new DefaultNavMenuInteractionHandler(null, (action, _) =>
        {
            var run = new ScheduledRun(action);
            delayedRuns.Add(run);
            return run;
        });

        try
        {
            fixture.Window.Show();
            Dispatcher.UIThread.RunJobs();
            ReplaceInteractionHandler(fixture.Menu, handler);

            var item = fixture.Menu.ContainerFromItem(fixture.Submenu).ShouldBeOfType<NavMenuItem>();
            item.Open();
            Dispatcher.UIThread.RunJobs();
            item.IsSubMenuOpen.ShouldBeTrue();

            fixture.ScrollViewer.Offset = new Vector(0, 20);
            Dispatcher.UIThread.RunJobs();

            RaisePointerExited(item, fixture.Window, timestamp: 0);
            Dispatcher.UIThread.RunJobs();

            delayedRuns.Count.ShouldBe(0);
            item.IsSubMenuOpen.ShouldBeTrue();
        }
        finally
        {
            fixture.Window.Close();
        }
    }

    [Fact]
    public void Pointer_Move_Exit_Schedules_Submenu_Close()
    {
        var fixture = CreateScrollableHorizontalMenuFixture();
        var delayedRuns = new List<ScheduledRun>();
        var handler = new DefaultNavMenuInteractionHandler(null, (action, _) =>
        {
            var run = new ScheduledRun(action);
            delayedRuns.Add(run);
            return run;
        });

        try
        {
            fixture.Window.Show();
            Dispatcher.UIThread.RunJobs();
            ReplaceInteractionHandler(fixture.Menu, handler);

            var item = fixture.Menu.ContainerFromItem(fixture.Submenu).ShouldBeOfType<NavMenuItem>();
            item.Open();
            Dispatcher.UIThread.RunJobs();
            item.IsSubMenuOpen.ShouldBeTrue();

            RaisePointerExited(item, fixture.Window, timestamp: 42);
            Dispatcher.UIThread.RunJobs();

            delayedRuns.Count.ShouldBe(1);
            delayedRuns[0].Run();
            Dispatcher.UIThread.RunJobs();
            item.IsSubMenuOpen.ShouldBeFalse();
        }
        finally
        {
            fixture.Window.Close();
        }
    }

    [Fact]
    public void Removing_An_Item_Disposes_Its_Pending_Delayed_Open()
    {
        var fixture = CreateScrollableHorizontalMenuFixture();
        var delayedRuns = new List<ScheduledRun>();
        var handler = new DefaultNavMenuInteractionHandler(null, (action, _) =>
        {
            var run = new ScheduledRun(action);
            delayedRuns.Add(run);
            return run;
        });

        try
        {
            fixture.Window.Show();
            Dispatcher.UIThread.RunJobs();
            ReplaceInteractionHandler(fixture.Menu, handler);

            var item = fixture.Menu.ContainerFromItem(fixture.Submenu).ShouldBeOfType<NavMenuItem>();
            RaisePointerEntered(item, fixture.Window, timestamp: 42);
            Dispatcher.UIThread.RunJobs();
            delayedRuns.Count.ShouldBe(1);
            delayedRuns[0].IsDisposed.ShouldBeFalse();

            fixture.Menu.Items.Remove(fixture.Submenu);
            Dispatcher.UIThread.RunJobs();
            fixture.Window.UpdateLayout();

            delayedRuns[0].IsDisposed.ShouldBeTrue(
                "A delayed hover action must not retain or later act on a recycled container.");
        }
        finally
        {
            fixture.Window.Close();
        }
    }

    [Fact]
    public void Removing_An_Item_Disposes_Its_Pending_Delayed_Close()
    {
        var fixture = CreateScrollableHorizontalMenuFixture();
        var delayedRuns = new List<ScheduledRun>();
        var handler = new DefaultNavMenuInteractionHandler(null, (action, _) =>
        {
            var run = new ScheduledRun(action);
            delayedRuns.Add(run);
            return run;
        });

        try
        {
            fixture.Window.Show();
            Dispatcher.UIThread.RunJobs();
            ReplaceInteractionHandler(fixture.Menu, handler);

            var item = fixture.Menu.ContainerFromItem(fixture.Submenu).ShouldBeOfType<NavMenuItem>();
            item.Open();
            Dispatcher.UIThread.RunJobs();
            RaisePointerExited(item, fixture.Window, timestamp: 42);
            Dispatcher.UIThread.RunJobs();
            delayedRuns.Count.ShouldBe(1);
            delayedRuns[0].IsDisposed.ShouldBeFalse();

            fixture.Menu.Items.Remove(fixture.Submenu);
            Dispatcher.UIThread.RunJobs();
            fixture.Window.UpdateLayout();

            delayedRuns[0].IsDisposed.ShouldBeTrue(
                "A delayed close action must not retain or later act on a recycled container.");
        }
        finally
        {
            fixture.Window.Close();
        }
    }

    [Fact]
    public void Pointer_Enter_On_A_Grouped_Leaf_Closes_An_Open_Item_In_Another_Root_Group()
    {
        var openNode = CreateSubmenuNode("Open");
        var leaf = new NavMenuNode { Header = "Leaf" };
        var firstGroup = new NavMenuGroup { Header = "First group" };
        firstGroup.Entries.Add(openNode);
        var secondGroup = new NavMenuGroup { Header = "Second group" };
        secondGroup.Entries.Add(leaf);
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Vertical,
            IsMotionEnabled = false
        };
        menu.Items.Add(firstGroup);
        menu.Items.Add(secondGroup);
        var overlay = new VisualLayerManager { Child = menu };
        EnablePopupOverlayLayer(overlay);
        var window = new AvaloniaWindow { Width = 320, Height = 240, Content = overlay };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var openContainer = menu.ContainerFromItem(firstGroup)
                .ShouldBeOfType<NavMenuGroupItem>()
                .ContainerFromItem(openNode)
                .ShouldBeOfType<NavMenuItem>();
            var leafContainer = menu.ContainerFromItem(secondGroup)
                .ShouldBeOfType<NavMenuGroupItem>()
                .ContainerFromItem(leaf)
                .ShouldBeOfType<NavMenuItem>();

            openContainer.Open();
            Dispatcher.UIThread.RunJobs();
            openContainer.IsSubMenuOpen.ShouldBeTrue();

            RaisePointerEntered(leafContainer, window, timestamp: 42);
            RunDispatcherJobsUntil(() => !openContainer.IsSubMenuOpen);

            openContainer.IsSubMenuOpen.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Layout_Induced_Pointer_Enter_Does_Not_Schedule_Submenu_Open()
    {
        var fixture = CreateScrollableHorizontalMenuFixture();
        var delayedRuns = new List<ScheduledRun>();
        var handler = new DefaultNavMenuInteractionHandler(null, (action, _) =>
        {
            var run = new ScheduledRun(action);
            delayedRuns.Add(run);
            return run;
        });

        try
        {
            fixture.Window.Show();
            Dispatcher.UIThread.RunJobs();
            ReplaceInteractionHandler(fixture.Menu, handler);

            var item = fixture.Menu.ContainerFromItem(fixture.Submenu).ShouldBeOfType<NavMenuItem>();
            item.IsSubMenuOpen.ShouldBeFalse();

            fixture.ScrollViewer.Offset = new Vector(0, 20);
            Dispatcher.UIThread.RunJobs();

            RaisePointerEntered(item, fixture.Window, timestamp: 0);
            Dispatcher.UIThread.RunJobs();

            delayedRuns.Count.ShouldBe(0);
            item.IsSubMenuOpen.ShouldBeFalse();
        }
        finally
        {
            fixture.Window.Close();
        }
    }

    [Fact]
    public void Default_Handler_Detach_Disposes_Pending_Delayed_Open()
    {
        var scheduledRuns = new List<TrackingDisposable>();
        var handler = new DefaultNavMenuInteractionHandler(null, (_, _) =>
        {
            var disposable = new TrackingDisposable();
            scheduledRuns.Add(disposable);
            return disposable;
        });
        var menu = new AtomUI.Desktop.Controls.NavMenu();

        handler.AttachCore(menu);
        handler.OpenWithDelay(new NavMenuItem());

        scheduledRuns.Count.ShouldBe(1);
        scheduledRuns[0].IsDisposed.ShouldBeFalse();

        handler.DetachCore(menu);

        scheduledRuns[0].IsDisposed.ShouldBeTrue();
    }

    [Fact]
    public void Removing_The_Pressed_Item_Invalidates_It_Before_Pointer_Release()
    {
        var first = new NavMenuNode { Header = "First", ItemKey = "first" };
        var second = new NavMenuNode { Header = "Second", ItemKey = "second" };
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(first);
        menu.Items.Add(second);

        var clickedItems = new List<INavMenuItem>();
        menu.NavMenuItemClick += (_, args) => clickedItems.Add(args.NavMenuItem);
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 240,
            Content = menu
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var firstContainer = menu.ContainerFromItem(first).ShouldBeOfType<NavMenuItem>();
            var header = firstContainer.ItemHeader.ShouldNotBeNull();
            var point = header.TranslatePoint(
                new Point(header.Bounds.Width / 2, header.Bounds.Height / 2),
                window).ShouldNotBeNull();

            window.MouseMove(point);
            window.MouseDown(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            menu.Items.Remove(first);
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();

            window.MouseUp(point, MouseButton.Left);
            Dispatcher.UIThread.RunJobs();

            clickedItems.ShouldBeEmpty(
                "Releasing after the pressed container was recycled must not click the removed entry.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Inline_Accordion_Closes_A_Previously_Open_Top_Level_Item_Through_Groups()
    {
        var first = CreateSubmenuNode("First");
        var second = CreateSubmenuNode("Second");
        var firstGroup = new NavMenuGroup { Header = "First group" };
        firstGroup.Entries.Add(first);
        var secondGroup = new NavMenuGroup { Header = "Second group" };
        secondGroup.Entries.Add(second);
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsAccordionMode = true,
            IsMotionEnabled = false
        };
        menu.Items.Add(firstGroup);
        menu.Items.Add(new NavMenuDivider());
        menu.Items.Add(secondGroup);
        var window = new AvaloniaWindow { Width = 320, Height = 240, Content = menu };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var firstContainer = menu.ContainerFromItem(firstGroup)
                .ShouldBeOfType<NavMenuGroupItem>()
                .ContainerFromItem(first)
                .ShouldBeOfType<NavMenuItem>();
            var secondContainer = menu.ContainerFromItem(secondGroup)
                .ShouldBeOfType<NavMenuGroupItem>()
                .ContainerFromItem(second)
                .ShouldBeOfType<NavMenuItem>();

            firstContainer.Open();
            Dispatcher.UIThread.RunJobs();
            firstContainer.IsSubMenuOpen.ShouldBeTrue();

            secondContainer.Open();
            Dispatcher.UIThread.RunJobs();

            secondContainer.IsSubMenuOpen.ShouldBeTrue();
            firstContainer.IsSubMenuOpen.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Vertical_Grouped_Sibling_Submenus_Remain_Mutually_Exclusive()
    {
        var first = CreateSubmenuNode("First");
        var second = CreateSubmenuNode("Second");
        var group = new NavMenuGroup { Header = "Section" };
        group.Entries.Add(first);
        group.Entries.Add(second);
        var parent = new NavMenuNode { Header = "Parent" };
        parent.Entries.Add(group);
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Vertical,
            IsMotionEnabled = false
        };
        menu.Items.Add(parent);
        var overlay = new VisualLayerManager { Child = menu };
        EnablePopupOverlayLayer(overlay);
        var window = new AvaloniaWindow { Width = 320, Height = 240, Content = overlay };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var parentContainer = menu.ContainerFromItem(parent).ShouldBeOfType<NavMenuItem>();
            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();
            menu.ExecutePendingContainerLayout(parentContainer);
            var groupContainer = parentContainer.ContainerFromItem(group).ShouldBeOfType<NavMenuGroupItem>();
            var firstContainer = groupContainer.ContainerFromItem(first).ShouldBeOfType<NavMenuItem>();
            var secondContainer = groupContainer.ContainerFromItem(second).ShouldBeOfType<NavMenuItem>();

            firstContainer.Open();
            Dispatcher.UIThread.RunJobs();
            firstContainer.IsSubMenuOpen.ShouldBeTrue();

            secondContainer.Open();
            Dispatcher.UIThread.RunJobs();

            secondContainer.IsSubMenuOpen.ShouldBeTrue();
            firstContainer.IsSubMenuOpen.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Close_Recursively_Closes_Grouped_Descendant_Submenus()
    {
        var child = CreateSubmenuNode("Child");
        var group = new NavMenuGroup { Header = "Section" };
        group.Entries.Add(child);
        var parent = new NavMenuNode { Header = "Parent" };
        parent.Entries.Add(group);
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Inline,
            IsMotionEnabled = false
        };
        menu.Items.Add(parent);
        var visualLayerManager = new VisualLayerManager { Child = menu };
        EnablePopupOverlayLayer(visualLayerManager);
        var window = new AvaloniaWindow { Width = 320, Height = 240, Content = visualLayerManager };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var parentContainer = menu.ContainerFromItem(parent).ShouldBeOfType<NavMenuItem>();
            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();
            var groupContainer = parentContainer.ContainerFromItem(group).ShouldBeOfType<NavMenuGroupItem>();
            var childContainer = groupContainer.ContainerFromItem(child).ShouldBeOfType<NavMenuItem>();
            childContainer.Open();
            Dispatcher.UIThread.RunJobs();
            childContainer.IsSubMenuOpen.ShouldBeTrue();

            parentContainer.Close();
            RunDispatcherJobsUntil(() => !parentContainer.IsSubMenuOpen);

            parentContainer.IsSubMenuOpen.ShouldBeFalse();
            childContainer.IsSubMenuOpen.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void InlineCollapsed_TopLevel_Lost_Platform_Focus_Closes_Popups_Without_Clearing_Selection()
    {
        var leaf = new NavMenuNode { Header = "Leaf", ItemKey = "leaf" };
        var child = new NavMenuNode { Header = "Child", ItemKey = "child" };
        child.Children.Add(leaf);
        var parent = new NavMenuNode { Header = "Parent", ItemKey = "parent" };
        parent.Children.Add(child);
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode              = NavMenuMode.Inline,
            IsInlineCollapsed = true,
            IsMotionEnabled   = false,
            Width             = 240
        };
        menu.Items.Add(parent);
        var visualLayerManager = new VisualLayerManager { Child = menu };
        EnablePopupOverlayLayer(visualLayerManager);
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 240,
            Content = visualLayerManager
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var parentContainer = menu.ContainerFromItem(parent).ShouldBeOfType<NavMenuItem>();
            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();
            menu.ExecutePendingContainerLayout(parentContainer);

            var childContainer = parentContainer.ContainerFromItem(child).ShouldBeOfType<NavMenuItem>();
            childContainer.Open();
            Dispatcher.UIThread.RunJobs();
            menu.ExecutePendingContainerLayout(childContainer);

            var leafContainer = childContainer.ContainerFromItem(leaf).ShouldBeOfType<NavMenuItem>();
            menu.InteractionHandler.ShouldNotBeNull();
            menu.SelectNavMenuItem(leafContainer);
            Dispatcher.UIThread.RunJobs();

            menu.SelectedItem.ShouldBeSameAs(leaf);
            parentContainer.IsInSelectedPath.ShouldBeTrue();
            childContainer.IsInSelectedPath.ShouldBeTrue();

            var handler = menu.InteractionHandler.ShouldBeOfType<DefaultNavMenuInteractionHandler>();
            var lostFocusMethod = typeof(DefaultNavMenuInteractionHandler).GetMethod(
                "TopLevelLostPlatformFocus",
                BindingFlags.Instance | BindingFlags.NonPublic);
            lostFocusMethod.ShouldNotBeNull();
            lostFocusMethod.Invoke(handler, null);
            RunDispatcherJobsUntil(() => !parentContainer.IsSubMenuOpen);

            parentContainer.IsSubMenuOpen.ShouldBeFalse();
            menu.SelectedItem.ShouldBeSameAs(leaf);
            parentContainer.IsInSelectedPath.ShouldBeTrue();

            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();
            menu.ExecutePendingContainerLayout(parentContainer);

            var reopenedChildContainer = parentContainer.ContainerFromItem(child).ShouldBeOfType<NavMenuItem>();
            reopenedChildContainer.IsInSelectedPath.ShouldBeTrue();
            reopenedChildContainer.Open();
            Dispatcher.UIThread.RunJobs();
            menu.ExecutePendingContainerLayout(reopenedChildContainer);

            var reopenedLeafContainer = reopenedChildContainer.ContainerFromItem(leaf).ShouldBeOfType<NavMenuItem>();
            reopenedLeafContainer.IsSelected.ShouldBeTrue();
            parentContainer.IsInSelectedPath.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Public_Close_Clears_Selection_While_Internal_Popup_Close_Does_Not()
    {
        var leaf = new NavMenuNode { Header = "Leaf", ItemKey = "leaf" };
        var parent = new NavMenuNode { Header = "Parent", ItemKey = "parent" };
        parent.Children.Add(leaf);
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Vertical,
            IsMotionEnabled = false
        };
        menu.Items.Add(parent);
        var visualLayerManager = new VisualLayerManager { Child = menu };
        EnablePopupOverlayLayer(visualLayerManager);
        var window = new AvaloniaWindow { Width = 320, Height = 240, Content = visualLayerManager };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var parentContainer = menu.ContainerFromItem(parent).ShouldBeOfType<NavMenuItem>();
            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();
            var leafContainer = parentContainer.ContainerFromItem(leaf).ShouldBeOfType<NavMenuItem>();
            menu.InteractionHandler.ShouldNotBeNull();
            menu.SelectNavMenuItem(leafContainer);
            Dispatcher.UIThread.RunJobs();

            menu.Close();
            RunDispatcherJobsUntil(() => !parentContainer.IsSubMenuOpen);

            menu.SelectedItem.ShouldBeNull();
            parentContainer.IsInSelectedPath.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Mode_Change_Closes_Popup_Without_Clearing_Selection()
    {
        var leaf = new NavMenuNode { Header = "Leaf", ItemKey = "leaf" };
        var parent = new NavMenuNode { Header = "Parent", ItemKey = "parent" };
        parent.Children.Add(leaf);
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Vertical,
            IsMotionEnabled = false
        };
        menu.Items.Add(parent);
        var visualLayerManager = new VisualLayerManager { Child = menu };
        EnablePopupOverlayLayer(visualLayerManager);
        var window = new AvaloniaWindow { Width = 320, Height = 240, Content = visualLayerManager };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var parentContainer = menu.ContainerFromItem(parent).ShouldBeOfType<NavMenuItem>();
            parentContainer.Open();
            Dispatcher.UIThread.RunJobs();
            var leafContainer = parentContainer.ContainerFromItem(leaf).ShouldBeOfType<NavMenuItem>();
            menu.InteractionHandler.ShouldNotBeNull();
            menu.SelectNavMenuItem(leafContainer);
            Dispatcher.UIThread.RunJobs();

            menu.Mode = NavMenuMode.Horizontal;
            Dispatcher.UIThread.RunJobs();

            menu.SelectedItem.ShouldBeSameAs(leaf);
            var modeChangedParentContainer = menu.ContainerFromItem(parent).ShouldBeOfType<NavMenuItem>();
            modeChangedParentContainer.IsSubMenuOpen.ShouldBeFalse();
            modeChangedParentContainer.IsInSelectedPath.ShouldBeTrue();

            modeChangedParentContainer.Open();
            Dispatcher.UIThread.RunJobs();
            modeChangedParentContainer.ContainerFromItem(leaf)
                           .ShouldBeOfType<NavMenuItem>()
                           .IsSelected.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }

    private static MenuFixture CreateScrollableHorizontalMenuFixture()
    {
        var submenu = new NavMenuNode
        {
            Header = "Parent"
        };
        submenu.Children.Add(new NavMenuNode
        {
            Header = "Child"
        });
        var menu = new AtomUI.Desktop.Controls.NavMenu
        {
            Mode            = NavMenuMode.Horizontal,
            IsMotionEnabled = false
        };
        menu.Items.Add(submenu);

        var stackPanel = new StackPanel();
        stackPanel.Children.Add(new Border
        {
            Height = 80
        });
        stackPanel.Children.Add(menu);
        stackPanel.Children.Add(new Border
        {
            Height = 600
        });
        var scrollViewer = new ScrollViewer
        {
            Height  = 240,
            Content = stackPanel
        };
        var visualLayerManager = new VisualLayerManager
        {
            Child = scrollViewer
        };
        EnablePopupOverlayLayer(visualLayerManager);
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 240,
            Content = visualLayerManager
        };

        return new MenuFixture(window, scrollViewer, menu, submenu);
    }

    private static NavMenuNode CreateSubmenuNode(string header)
    {
        var node = new NavMenuNode { Header = header };
        node.Children.Add(new NavMenuNode { Header = $"{header} child" });
        return node;
    }

    private static void RunDispatcherJobsUntil(Func<bool> condition, int maxPasses = 128)
    {
        for (var i = 0; i < maxPasses && !condition(); i++)
        {
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static void ReplaceInteractionHandler(
        AtomUI.Desktop.Controls.NavMenu menu,
        DefaultNavMenuInteractionHandler handler)
    {
        menu.InteractionHandler.ShouldNotBeNull();
        menu.InteractionHandler!.Detach(menu);

        var property = typeof(AtomUI.Desktop.Controls.NavMenu).GetProperty(
            "InteractionHandler",
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(menu, handler);
        handler.AttachCore(menu);
    }

    private static void RaisePointerExited(NavMenuItem item, AvaloniaWindow window, ulong timestamp)
    {
        RaisePointerTransition(InputElement.PointerExitedEvent, item, window, timestamp);
    }

    private static void RaisePointerEntered(NavMenuItem item, AvaloniaWindow window, ulong timestamp)
    {
        RaisePointerTransition(InputElement.PointerEnteredEvent, item, window, timestamp);
    }

    private static void RaisePointerTransition(
        RoutedEvent routedEvent,
        NavMenuItem item,
        AvaloniaWindow window,
        ulong timestamp)
    {
        var pointer = new Avalonia.Input.Pointer(
            Avalonia.Input.Pointer.GetNextFreeId(),
            PointerType.Mouse,
            true);
        var position = item.TranslatePoint(
            new Point(item.Bounds.Width + 1, item.Bounds.Height + 1),
            window) ?? default;

        var args = new PointerEventArgs(
            routedEvent,
            item,
            pointer,
            window,
            position,
            timestamp,
            PointerPointProperties.None,
            KeyModifiers.None);

        item.RaiseEvent(args);
    }

    private sealed record MenuFixture(
        AvaloniaWindow Window,
        ScrollViewer ScrollViewer,
        AtomUI.Desktop.Controls.NavMenu Menu,
        NavMenuNode Submenu);

    private sealed class ScheduledRun : IDisposable
    {
        private readonly Action _action;

        public ScheduledRun(Action action)
        {
            _action = action;
        }

        public bool IsDisposed { get; private set; }

        public void Run()
        {
            if (!IsDisposed)
            {
                _action();
            }
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
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
