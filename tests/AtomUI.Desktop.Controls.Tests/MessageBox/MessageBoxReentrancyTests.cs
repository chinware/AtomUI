using System.Reflection;
using System.Diagnostics;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.MessageBox;

public class MessageBoxReentrancyTests
{
    static MessageBoxReentrancyTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void ShowMessageBox_Called_From_Button_Click_Does_Not_Reenter_Click_Handler()
    {
        var trigger = new AtomUI.Desktop.Controls.Button
        {
            Width   = 120,
            Height  = 32,
            Content = "Show"
        };
        var window = CreateWindow(trigger);

        var clickCount = 0;
        trigger.Click += (_, _) =>
        {
            clickCount++;
            ScheduleClickOpenMessageBoxButton(window);

            var result = AtomUI.Desktop.Controls.MessageBox.ShowMessageBox(
                new AtomUI.Desktop.Controls.TextBlock { Text = "Confirm?" },
                options: new MessageBoxOptions
                {
                    Title             = "Confirm",
                    IsCenterOnStartup = true,
                    Style             = MessageBoxStyle.Confirm
                },
                topLevel: window);

            result.ShouldBe(DialogCode.Accepted);
        };

        try
        {
            Click(trigger, window);
            Dispatcher.UIThread.RunJobs();

            clickCount.ShouldBe(1);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ShowMessageBox_Called_From_Button_Click_Accepts_Default_Button_With_Enter_Without_Reentering_Click_Handler()
    {
        var trigger = new AtomUI.Desktop.Controls.Button
        {
            Width   = 120,
            Height  = 32,
            Content = "Show"
        };
        var window = CreateWindow(trigger);

        var clickCount = 0;
        var fallbackClickRequired = false;
        object? result = null;
        trigger.Click += (_, _) =>
        {
            clickCount++;
            if (clickCount > 1)
            {
                return;
            }

            ScheduleKeyPressOpenMessageBoxButton(
                window,
                Key.Enter,
                DialogStandardButton.Ok,
                () => fallbackClickRequired = true);

            result = AtomUI.Desktop.Controls.MessageBox.ShowMessageBox(
                new AtomUI.Desktop.Controls.TextBlock { Text = "Confirm?" },
                options: new MessageBoxOptions
                {
                    Title             = "Confirm",
                    IsCenterOnStartup = true,
                    Style             = MessageBoxStyle.Confirm
                },
                topLevel: window);
        };

        try
        {
            Click(trigger, window);
            Dispatcher.UIThread.RunJobs();

            result.ShouldBe(DialogCode.Accepted);
            clickCount.ShouldBe(1);
            fallbackClickRequired.ShouldBeFalse(
                "Enter should activate the MessageBox default button without needing a mouse click fallback.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ShowMessageBox_Called_From_Button_Click_Rejects_Escape_Button_Without_Reentering_Click_Handler()
    {
        var trigger = new AtomUI.Desktop.Controls.Button
        {
            Width   = 120,
            Height  = 32,
            Content = "Show"
        };
        var window = CreateWindow(trigger);

        var clickCount = 0;
        var fallbackClickRequired = false;
        object? result = null;
        trigger.Click += (_, _) =>
        {
            clickCount++;
            if (clickCount > 1)
            {
                return;
            }

            ScheduleKeyPressOpenMessageBoxButton(
                window,
                Key.Escape,
                DialogStandardButton.Cancel,
                () => fallbackClickRequired = true);

            result = AtomUI.Desktop.Controls.MessageBox.ShowMessageBox(
                new AtomUI.Desktop.Controls.TextBlock { Text = "Confirm?" },
                options: new MessageBoxOptions
                {
                    Title             = "Confirm",
                    IsCenterOnStartup = true,
                    Style             = MessageBoxStyle.Confirm
                },
                topLevel: window);
        };

        try
        {
            Click(trigger, window);
            Dispatcher.UIThread.RunJobs();

            result.ShouldBe(DialogCode.Rejected);
            clickCount.ShouldBe(1);
            fallbackClickRequired.ShouldBeFalse(
                "Escape should activate the MessageBox cancel button without needing a mouse click fallback.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ShowMessageBox_Returns_When_Dialog_Is_Logically_Closed_Instead_Of_Waiting_For_Close_Animation()
    {
        var trigger = new AtomUI.Desktop.Controls.Button
        {
            Width   = 120,
            Height  = 32,
            Content = "Show"
        };
        var window = CreateWindow(trigger);

        TimeSpan showMessageBoxDuration = default;
        trigger.Click += (_, _) =>
        {
            ScheduleClickOpenMessageBoxButton(
                window,
                canClick: IsOverlayDialogHostAnimationReady,
                beforeClick: () => SetOverlayDialogHostAnimationDuration(window, TimeSpan.FromSeconds(1)));

            var stopwatch = Stopwatch.StartNew();
            AtomUI.Desktop.Controls.MessageBox.ShowMessageBox(
                new AtomUI.Desktop.Controls.TextBlock { Text = "Confirm?" },
                options: new MessageBoxOptions
                {
                    Title             = "Confirm",
                    IsCenterOnStartup = true,
                    Style             = MessageBoxStyle.Confirm
                },
                topLevel: window);
            stopwatch.Stop();
            showMessageBoxDuration = stopwatch.Elapsed;
        };

        try
        {
            Click(trigger, window);
            Dispatcher.UIThread.RunJobs();

            showMessageBoxDuration.ShouldBeLessThan(
                TimeSpan.FromMilliseconds(500),
                "synchronous MessageBox should return once OK logically closes the dialog; close animation cleanup can continue asynchronously.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ShowMessageBox_Called_From_Button_Pointer_Click_Accepts_First_Dialog_Mouse_Click()
    {
        var trigger = new AtomUI.Desktop.Controls.Button
        {
            Width   = 120,
            Height  = 32,
            Content = "Show"
        };
        var window = CreateWindow(trigger);

        var firstDialogMouseClickHandled = false;
        object? result = null;
        trigger.Click += (_, _) =>
        {
            ScheduleFirstMouseClickOpenMessageBoxButton(
                window,
                () => firstDialogMouseClickHandled = true);

            result = AtomUI.Desktop.Controls.MessageBox.ShowMessageBox(
                new AtomUI.Desktop.Controls.TextBlock { Text = "Confirm?" },
                options: new MessageBoxOptions
                {
                    Title             = "Confirm",
                    IsCenterOnStartup = true,
                    Style             = MessageBoxStyle.Confirm
                },
                topLevel: window);
        };

        try
        {
            Click(trigger, window);
            Dispatcher.UIThread.RunJobs();

            result.ShouldBe(DialogCode.Accepted);
            firstDialogMouseClickHandled.ShouldBeTrue(
                "a synchronous MessageBox opened from a pointer click must release the trigger's pointer capture before entering the nested dispatcher frame.");
        }
        finally
        {
            window.Close();
        }
    }


    [Fact]
    public void ShowMessageBox_Called_From_MenuItem_Click_Accepts_Default_Button_With_Enter_Without_Reentering_Click_Handler()
    {
        var aboutMenuItem = new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "About"
        };
        var helpMenuItem = new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "Help"
        };
        helpMenuItem.Items.Add(aboutMenuItem);

        var menu = new AtomUI.Desktop.Controls.Menu
        {
            Width = 320
        };
        menu.Items.Add(helpMenuItem);

        var window = CreateWindow(menu);
        var clickCount = 0;
        var fallbackClickRequired = false;
        object? result = null;

        aboutMenuItem.Click += (_, _) =>
        {
            clickCount++;
            if (clickCount > 1)
            {
                return;
            }

            ScheduleKeyPressOpenMessageBoxButton(
                window,
                Key.Enter,
                DialogStandardButton.Ok,
                () => fallbackClickRequired = true);

            result = AtomUI.Desktop.Controls.MessageBox.ShowMessageBox(
                new AtomUI.Desktop.Controls.TextBlock { Text = "About" },
                options: new MessageBoxOptions
                {
                    Title = "About",
                    Style = MessageBoxStyle.Information
                },
                topLevel: window);
        };

        try
        {
            menu.Open();
            helpMenuItem.IsSubMenuOpen = true;
            Dispatcher.UIThread.RunJobs();

            aboutMenuItem.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.MenuItem.ClickEvent, aboutMenuItem));
            Dispatcher.UIThread.RunJobs();

            result.ShouldBe(DialogCode.Accepted);
            clickCount.ShouldBe(1);
            fallbackClickRequired.ShouldBeFalse(
                "Enter should close the MessageBox and must not re-trigger the menu item while ShowMessageBox is nested in the click handler.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ShowMessageBox_Called_From_MenuItem_Click_Sees_Menu_Closed_Before_Handler_Blocks()
    {
        var aboutMenuItem = new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "About"
        };
        var helpMenuItem = new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "Help"
        };
        helpMenuItem.Items.Add(aboutMenuItem);

        var menu = new AtomUI.Desktop.Controls.Menu
        {
            Width = 320
        };
        menu.Items.Add(helpMenuItem);

        var window = CreateWindow(menu);
        var menuWasOpenWhenHandlerStarted = false;

        aboutMenuItem.Click += (_, _) =>
        {
            menuWasOpenWhenHandlerStarted = menu.IsOpen || helpMenuItem.IsSubMenuOpen;
            ScheduleClickOpenMessageBoxButton(window);

            AtomUI.Desktop.Controls.MessageBox.ShowMessageBox(
                new AtomUI.Desktop.Controls.TextBlock { Text = "About" },
                options: new MessageBoxOptions
                {
                    Title = "About",
                    Style = MessageBoxStyle.Information
                },
                topLevel: window);
        };

        try
        {
            menu.Open();
            helpMenuItem.IsSubMenuOpen = true;
            Dispatcher.UIThread.RunJobs();

            aboutMenuItem.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.MenuItem.ClickEvent, aboutMenuItem));

            menuWasOpenWhenHandlerStarted.ShouldBeFalse(
                "a synchronous MessageBox opened from a menu click must not block Avalonia's normal menu close path.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void MenuItem_Click_Preclose_Preserves_Bubbling_Handlers()
    {
        var aboutMenuItem = new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "About"
        };
        var helpMenuItem = new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "Help"
        };
        helpMenuItem.Items.Add(aboutMenuItem);

        var menu = new AtomUI.Desktop.Controls.Menu();
        menu.Items.Add(helpMenuItem);

        var window = CreateWindow(menu);
        var bubbledToParentMenuItem = false;
        var bubbledToMenu = false;
        helpMenuItem.AddHandler(
            Avalonia.Controls.MenuItem.ClickEvent,
            (_, _) => bubbledToParentMenuItem = true);
        menu.AddHandler(
            Avalonia.Controls.MenuItem.ClickEvent,
            (_, _) => bubbledToMenu = true);

        try
        {
            menu.Open();
            helpMenuItem.IsSubMenuOpen = true;
            Dispatcher.UIThread.RunJobs();

            aboutMenuItem.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.MenuItem.ClickEvent, aboutMenuItem));

            bubbledToParentMenuItem.ShouldBeTrue();
            bubbledToMenu.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void MenuItem_Click_With_StaysOpenOnClick_Does_Not_Preclose_Menu()
    {
        var aboutMenuItem = new AtomUI.Desktop.Controls.MenuItem
        {
            Header           = "About",
            StaysOpenOnClick = true
        };
        var helpMenuItem = new AtomUI.Desktop.Controls.MenuItem
        {
            Header = "Help"
        };
        helpMenuItem.Items.Add(aboutMenuItem);

        var menu = new AtomUI.Desktop.Controls.Menu();
        menu.Items.Add(helpMenuItem);

        var window = CreateWindow(menu);
        var menuWasOpenWhenHandlerStarted = false;
        aboutMenuItem.Click += (_, _) =>
        {
            menuWasOpenWhenHandlerStarted = menu.IsOpen || helpMenuItem.IsSubMenuOpen;
        };

        try
        {
            menu.Open();
            helpMenuItem.IsSubMenuOpen = true;
            Dispatcher.UIThread.RunJobs();

            aboutMenuItem.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.MenuItem.ClickEvent, aboutMenuItem));

            menuWasOpenWhenHandlerStarted.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    private static AvaloniaWindow CreateWindow(Control content)
    {
        var overlayPanel = new ScopeAwareOverlayLayerPanel
        {
            Width  = 320,
            Height = 240
        };
        overlayPanel.Children.Add(content);

        var visualLayerManager = new VisualLayerManager
        {
            Child = overlayPanel
        };
        EnablePopupOverlayLayer(visualLayerManager);

        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 240,
            Content = visualLayerManager
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static void Click(Control control, AvaloniaWindow window)
    {
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window);

        point.ShouldNotBeNull();
        window.MouseMove(point.Value);
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
    }

    private static void ScheduleClickOpenMessageBoxButton(
        AvaloniaWindow window,
        int attempt = 0,
        Func<AvaloniaWindow, bool>? canClick = null,
        Action? beforeClick = null)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (TryClickOpenMessageBoxButton(window, canClick, beforeClick))
            {
                return;
            }

            attempt.ShouldBeLessThan(10, "MessageBox button should become available while the synchronous dispatcher frame is running.");
            ScheduleClickOpenMessageBoxButton(window, attempt + 1, canClick, beforeClick);
        });
    }

    private static bool TryClickOpenMessageBoxButton(
        AvaloniaWindow window,
        Func<AvaloniaWindow, bool>? canClick,
        Action? beforeClick)
    {
        var acceptButton = window.GetVisualDescendants()
                                 .OfType<DialogButton>()
                                 .FirstOrDefault(x => x.StandardButtonType == DialogStandardButton.Ok);
        if (acceptButton is null || acceptButton.Bounds.Width <= 0 || acceptButton.Bounds.Height <= 0)
        {
            return false;
        }

        if (canClick is not null && !canClick(window))
        {
            return false;
        }

        beforeClick?.Invoke();
        acceptButton.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent, acceptButton));
        return true;
    }

    private static void ScheduleKeyPressOpenMessageBoxButton(
        AvaloniaWindow window,
        Key key,
        DialogStandardButton fallbackButton,
        Action fallbackClickRequired,
        int attempt = 0)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (TryKeyPressOpenMessageBoxButton(window, key, fallbackButton, fallbackClickRequired))
            {
                return;
            }

            attempt.ShouldBeLessThan(10, "MessageBox should become available while the synchronous dispatcher frame is running.");
            ScheduleKeyPressOpenMessageBoxButton(window, key, fallbackButton, fallbackClickRequired, attempt + 1);
        });
    }

    private static bool TryKeyPressOpenMessageBoxButton(
        AvaloniaWindow window,
        Key key,
        DialogStandardButton fallbackButton,
        Action fallbackClickRequired)
    {
        var fallbackDialogButton = window.GetVisualDescendants()
                                         .OfType<DialogButton>()
                                         .FirstOrDefault(x => x.StandardButtonType == fallbackButton);
        if (fallbackDialogButton is null || fallbackDialogButton.Bounds.Width <= 0 || fallbackDialogButton.Bounds.Height <= 0)
        {
            return false;
        }

        window.KeyPress(key, RawInputModifiers.None, ToPhysicalKey(key), null);
        Dispatcher.UIThread.Post(() =>
        {
            if (!fallbackDialogButton.IsAttachedToVisualTree())
            {
                return;
            }

            fallbackClickRequired();
            fallbackDialogButton.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent, fallbackDialogButton));
        });
        return true;
    }

    private static PhysicalKey ToPhysicalKey(Key key)
    {
        return key switch
        {
            Key.Enter  => PhysicalKey.Enter,
            Key.Escape => PhysicalKey.Escape,
            _          => PhysicalKey.None
        };
    }

    private static void ScheduleFirstMouseClickOpenMessageBoxButton(
        AvaloniaWindow window,
        Action firstDialogMouseClickHandled,
        int attempt = 0)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (TryFirstMouseClickOpenMessageBoxButton(window, firstDialogMouseClickHandled))
            {
                return;
            }

            attempt.ShouldBeLessThan(20, "MessageBox button should become hittable while the synchronous dispatcher frame is running.");
            DispatcherTimer.RunOnce(
                () => ScheduleFirstMouseClickOpenMessageBoxButton(window, firstDialogMouseClickHandled, attempt + 1),
                TimeSpan.FromMilliseconds(20));
        });
    }

    private static bool TryFirstMouseClickOpenMessageBoxButton(
        AvaloniaWindow window,
        Action firstDialogMouseClickHandled)
    {
        var acceptButton = window.GetVisualDescendants()
                                 .OfType<DialogButton>()
                                 .FirstOrDefault(x => x.StandardButtonType == DialogStandardButton.Ok);
        if (acceptButton is null || acceptButton.Bounds.Width <= 0 || acceptButton.Bounds.Height <= 0)
        {
            return false;
        }

        if (!IsCenterPointHitTestInside(acceptButton, window))
        {
            return false;
        }

        var handled = false;
        void HandleAcceptButtonClick(object? sender, RoutedEventArgs args)
        {
            handled = true;
            firstDialogMouseClickHandled();
        }

        acceptButton.Click += HandleAcceptButtonClick;
        Click(acceptButton, window);
        acceptButton.Click -= HandleAcceptButtonClick;

        if (!handled)
        {
            acceptButton.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent, acceptButton));
        }

        return true;
    }

    private static bool IsCenterPointHitTestInside(Control control, AvaloniaWindow window)
    {
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window);
        if (point is null || window.InputHitTest(point.Value) is not Visual hit)
        {
            return false;
        }

        return ReferenceEquals(hit, control) || control.IsVisualAncestorOf(hit);
    }

    private static bool IsOverlayDialogHostAnimationReady(Visual searchRoot)
    {
        var overlayDialogHost = searchRoot.GetVisualDescendants()
                                          .SingleOrDefault(x => x.GetType().Name == "OverlayDialogHost");
        if (overlayDialogHost is null)
        {
            return false;
        }

        var field = overlayDialogHost.GetType().GetField(
            "_animatedOverlayHost",
            BindingFlags.Instance | BindingFlags.NonPublic);

        field.ShouldNotBeNull();
        return field.GetValue(overlayDialogHost) is not null;
    }

    private static void SetOverlayDialogHostAnimationDuration(Visual searchRoot, TimeSpan duration)
    {
        var overlayDialogHost = searchRoot.GetVisualDescendants()
                                          .Single(x => x.GetType().Name == "OverlayDialogHost");
        var property = overlayDialogHost.GetType().GetProperty(
            "AnimationDuration",
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(overlayDialogHost, duration);
    }

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }
}
