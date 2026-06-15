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

namespace AtomUI.Desktop.Controls.Tests.Dialog;

public class DialogKeyboardTests
{
    static DialogKeyboardTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void ShowDialog_With_Cancel_Button_Rejects_With_Escape_By_Default()
    {
        var window = CreateWindow(new Control());
        var fallbackClickRequired = false;

        try
        {
            ScheduleKeyPressOpenDialogButton(
                window,
                Key.Escape,
                DialogStandardButton.Cancel,
                () => fallbackClickRequired = true);

            var result = AtomUI.Desktop.Controls.Dialog.ShowDialog(
                new AtomUI.Desktop.Controls.TextBlock { Text = "Confirm?" },
                options: new DialogOptions
                {
                    StandardButtons       = DialogStandardButton.Cancel | DialogStandardButton.Ok,
                    DefaultStandardButton = DialogStandardButton.Ok
                },
                topLevel: window);

            result.ShouldBe(DialogCode.Rejected);
            fallbackClickRequired.ShouldBeFalse(
                "a Dialog that renders a Cancel standard button should treat Escape as Cancel unless EscapeStandardButton is explicitly configured.");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ShowDialog_With_Explicit_No_Escape_Button_Does_Not_Infer_Cancel_Button()
    {
        var (window, overlayPanel) = CreateWindowWithOverlayPanel(new Control());
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            Content                 = new AtomUI.Desktop.Controls.TextBlock { Text = "Confirm?" },
            PlacementTarget         = overlayPanel,
            StandardButtons         = DialogStandardButton.Cancel | DialogStandardButton.Ok,
            DefaultStandardButton   = DialogStandardButton.Ok,
            EscapeStandardButton    = DialogStandardButton.NoButton,
            HorizontalStartupLocation = DialogHorizontalAnchor.Center,
            VerticalStartupLocation   = DialogVerticalAnchor.Center
        };
        var fallbackClickRequired = false;

        try
        {
            overlayPanel.Children.Add(dialog);
            ScheduleKeyPressOpenDialogButton(
                window,
                Key.Escape,
                DialogStandardButton.Cancel,
                () => fallbackClickRequired = true);

            var result = dialog.Open();

            result.ShouldBe(DialogCode.Rejected);
            fallbackClickRequired.ShouldBeTrue(
                "an explicitly configured NoButton escape mapping should keep Escape disabled even when a Cancel button exists.");
        }
        finally
        {
            overlayPanel.Children.Remove(dialog);
            window.Close();
        }
    }

    private static AvaloniaWindow CreateWindow(Control content)
    {
        return CreateWindowWithOverlayPanel(content).Window;
    }

    private static (AvaloniaWindow Window, ScopeAwareOverlayLayerPanel OverlayPanel) CreateWindowWithOverlayPanel(Control content)
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
        return (window, overlayPanel);
    }

    private static void ScheduleKeyPressOpenDialogButton(
        AvaloniaWindow window,
        Key key,
        DialogStandardButton fallbackButton,
        Action fallbackClickRequired,
        int attempt = 0)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (TryKeyPressOpenDialogButton(window, key, fallbackButton, fallbackClickRequired))
            {
                return;
            }

            attempt.ShouldBeLessThan(10, "Dialog button should become available while the synchronous dispatcher frame is running.");
            ScheduleKeyPressOpenDialogButton(window, key, fallbackButton, fallbackClickRequired, attempt + 1);
        });
    }

    private static bool TryKeyPressOpenDialogButton(
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

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }
}
