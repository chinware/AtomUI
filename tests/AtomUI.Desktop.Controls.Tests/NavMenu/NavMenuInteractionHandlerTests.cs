using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
