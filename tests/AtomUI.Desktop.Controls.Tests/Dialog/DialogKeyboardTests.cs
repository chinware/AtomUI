using AtomUI.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Dialog;

public class DialogKeyboardTests
{
    static DialogKeyboardTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Escape_Invokes_Cancel_When_No_Explicit_Escape_Button_Is_Set()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowDialog();
            try
            {
                var args = RaiseEscape(fixture.Window);
                WaitWithDispatcherPump(fixture.SessionTask);

                args.Handled.ShouldBeTrue();
                fixture.Dialog.Result.ShouldBe(DialogCode.Rejected);
                fixture.Dialog.IsOpen.ShouldBeFalse();
            }
            finally
            {
                fixture.Window.Close();
            }
        });
    }

    [Fact]
    public void Explicit_NoButton_Disables_Escape_Even_When_Cancel_Is_Visible()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowDialog(DialogStandardButton.NoButton);
            try
            {
                var args = RaiseEscape(fixture.Window);

                args.Handled.ShouldBeFalse();
                fixture.Dialog.IsOpen.ShouldBeTrue();
                fixture.SessionTask.IsCompleted.ShouldBeFalse();

                fixture.Dialog.Reject();
                WaitWithDispatcherPump(fixture.SessionTask);
            }
            finally
            {
                fixture.Window.Close();
            }
        });
    }

    private static DialogFixture ShowDialog(DialogStandardButton? escapeButton = null)
    {
        var placementTarget = new Border { Width = 80, Height = 32 };
        var root = new ScopeAwareOverlayLayerPanel
        {
            Width  = 480,
            Height = 360,
            Children = { placementTarget }
        };
        var window = new AtomUI.Desktop.Controls.Window
        {
            Width   = 480,
            Height  = 360,
            Content = root
        };
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            PlacementTarget       = placementTarget,
            Content               = "Confirm?",
            StandardButtons       = DialogStandardButton.Cancel | DialogStandardButton.Ok,
            DefaultStandardButton = DialogStandardButton.Ok,
            IsMotionEnabled       = false
        };
        if (escapeButton is { } value)
        {
            dialog.EscapeStandardButton = value;
        }

        window.Show();
        var sessionTask = dialog.OpenAsync();
        PumpUntil(() => window.GetVisualDescendants().OfType<DialogSurface>().Any());
        return new DialogFixture(window, dialog, sessionTask);
    }

    private static KeyEventArgs RaiseEscape(AtomUI.Desktop.Controls.Window window)
    {
        var surface = window.GetVisualDescendants().OfType<DialogSurface>().ShouldHaveSingleItem();
        var args = new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Source      = surface,
            Key         = Key.Escape
        };
        surface.RaiseEvent(args);
        return args;
    }

    private static void WaitWithDispatcherPump(Task task)
    {
        PumpUntil(() => task.IsCompleted);
        task.GetAwaiter().GetResult();
    }

    private static void PumpUntil(Func<bool> condition)
    {
        var timeoutAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
        while (!condition() && DateTimeOffset.UtcNow < timeoutAt)
        {
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(1);
        }

        condition().ShouldBeTrue();
    }

    private static void RunOnUIThread(Action action)
    {
        Dispatcher.UIThread.Invoke(action);
    }

    private sealed record DialogFixture(
        AtomUI.Desktop.Controls.Window Window,
        AtomUI.Desktop.Controls.Dialog Dialog,
        Task SessionTask);
}
