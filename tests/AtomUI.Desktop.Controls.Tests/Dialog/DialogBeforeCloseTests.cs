using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Dialog;

public class DialogBeforeCloseTests
{
    static DialogBeforeCloseTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void BeforeCloseAsync_Veto_Restores_IsOpen_And_Does_Not_Commit_Result()
    {
        RunOnUIThread(() =>
        {
            DialogClosingContext? capturedContext = null;
            var fixture = ShowDialog(context =>
            {
                capturedContext = context;
                return ValueTask.FromResult(false);
            });

            try
            {
                fixture.Dialog.Accept();
                Dispatcher.UIThread.RunJobs();

                fixture.Dialog.IsOpen.ShouldBeTrue();
                fixture.Dialog.Result.ShouldBeNull();
                fixture.SessionTask.IsCompleted.ShouldBeFalse();
                capturedContext.ShouldNotBeNull();
                capturedContext.Result.ShouldBe(DialogCode.Accepted);
                capturedContext.Reason.ShouldBe(DialogCloseReason.Accepted);

                fixture.Dialog.BeforeCloseAsync = _ => ValueTask.FromResult(true);
                fixture.Dialog.Accept();
                WaitWithDispatcherPump(fixture.SessionTask);
            }
            finally
            {
                fixture.Window.Close();
            }
        });
    }

    [Fact]
    public void IsOpen_Close_Veto_Restores_The_Declarative_Open_Intent()
    {
        RunOnUIThread(() =>
        {
            var fixture = ShowDialog(_ => ValueTask.FromResult(false));

            try
            {
                fixture.Dialog.IsOpen = false;
                PumpUntil(() => fixture.Dialog.IsOpen);

                fixture.SessionTask.IsCompleted.ShouldBeFalse();
                fixture.Dialog.Result.ShouldBeNull();

                fixture.Dialog.BeforeCloseAsync = _ => ValueTask.FromResult(true);
                fixture.Dialog.IsOpen = false;
                WaitWithDispatcherPump(fixture.SessionTask);
            }
            finally
            {
                fixture.Window.Close();
            }
        });
    }

    [Fact]
    public void ButtonClicked_Handled_Stops_Default_Close_And_Close_Policy()
    {
        RunOnUIThread(() =>
        {
            var policyCount = 0;
            var fixture = ShowDialog(_ =>
            {
                policyCount++;
                return ValueTask.FromResult(true);
            });
            EventHandler<DialogButtonClickedEventArgs> handler = (_, args) => args.Handled = true;
            fixture.Dialog.ButtonClicked += handler;

            try
            {
                FindStandardButton(fixture.Window, DialogStandardButton.Ok)
                    .RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent));

                policyCount.ShouldBe(0);
                fixture.Dialog.IsOpen.ShouldBeTrue();
                fixture.Dialog.Result.ShouldBeNull();

                fixture.Dialog.ButtonClicked -= handler;
                fixture.Dialog.Accept();
                WaitWithDispatcherPump(fixture.SessionTask);
            }
            finally
            {
                fixture.Window.Close();
            }
        });
    }

    [Fact]
    public void Mask_Close_Request_Uses_HostCloseRequest_Reason()
    {
        RunOnUIThread(() =>
        {
            DialogClosingContext? capturedContext = null;
            var fixture = ShowDialog(context =>
            {
                capturedContext = context;
                return ValueTask.FromResult(false);
            }, isModal: true);

            try
            {
                var mask = fixture.Window.GetVisualDescendants()
                                         .OfType<OverlayDialogMask>()
                                         .ShouldHaveSingleItem();
                RaisePointerPressed(mask);
                Dispatcher.UIThread.RunJobs();

                fixture.Dialog.IsOpen.ShouldBeTrue();
                capturedContext.ShouldNotBeNull();
                capturedContext.Reason.ShouldBe(DialogCloseReason.HostCloseRequest);
                capturedContext.DialogCode.ShouldBeNull();

                fixture.Dialog.BeforeCloseAsync = _ => ValueTask.FromResult(true);
                fixture.Dialog.Done();
                WaitWithDispatcherPump(fixture.SessionTask);
            }
            finally
            {
                fixture.Window.Close();
            }
        });
    }

    [Fact]
    public void Static_Async_Dialog_Waits_Until_Close_Policy_Allows_Close()
    {
        RunOnUIThread(() =>
        {
            var window = CreateWindow(new Border(), out _);
            var attempt = 0;
            var resultTask = AtomUI.Desktop.Controls.Dialog.ShowDialogModalAsync(
                new AtomUI.Desktop.Controls.TextBlock { Text = "Dialog" },
                options: new DialogOptions
                {
                    StandardButtons  = DialogStandardButton.Ok,
                    IsMotionEnabled  = false,
                    BeforeCloseAsync = _ => ValueTask.FromResult(++attempt > 1)
                },
                topLevel: window);

            try
            {
                PumpUntil(() => FindStandardButtonOrNull(window, DialogStandardButton.Ok) is not null);
                FindStandardButton(window, DialogStandardButton.Ok)
                    .RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent));
                Dispatcher.UIThread.RunJobs();

                resultTask.IsCompleted.ShouldBeFalse();
                attempt.ShouldBe(1);

                FindStandardButton(window, DialogStandardButton.Ok)
                    .RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent));

                WaitWithDispatcherPump(resultTask).ShouldBe(DialogCode.Accepted);
                attempt.ShouldBe(2);
            }
            finally
            {
                window.Close();
            }
        });
    }

    private static DialogFixture ShowDialog(
        Func<DialogClosingContext, ValueTask<bool>> beforeClose,
        bool isModal = false)
    {
        var window = CreateWindow(new Border(), out var root);
        var placementTarget = root.Children.OfType<Border>().Single();
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            PlacementTarget  = placementTarget,
            Content          = "Dialog",
            StandardButtons  = DialogStandardButton.Ok | DialogStandardButton.Cancel,
            IsModal          = isModal,
            IsMotionEnabled  = false,
            BeforeCloseAsync = beforeClose
        };
        root.Children.Add(dialog);
        var sessionTask = dialog.OpenAsync();
        PumpUntil(() => window.GetVisualDescendants().OfType<DialogSurface>().Any());
        return new DialogFixture(window, dialog, sessionTask);
    }

    private static AtomUI.Desktop.Controls.DialogButton FindStandardButton(
        Visual root,
        DialogStandardButton standardButton)
    {
        return FindStandardButtonOrNull(root, standardButton).ShouldNotBeNull();
    }

    private static AtomUI.Desktop.Controls.DialogButton? FindStandardButtonOrNull(
        Visual root,
        DialogStandardButton standardButton)
    {
        return root.GetVisualDescendants()
                   .OfType<AtomUI.Desktop.Controls.DialogButton>()
                   .FirstOrDefault(button => button.StandardButtonType == standardButton);
    }

    private static void RaisePointerPressed(Control source)
    {
        source.RaiseEvent(new PointerPressedEventArgs(
            source,
            new Pointer(Pointer.GetNextFreeId(), PointerType.Mouse, true),
            source,
            default,
            0,
            new PointerPointProperties(RawInputModifiers.LeftMouseButton, PointerUpdateKind.LeftButtonPressed),
            KeyModifiers.None));
    }

    private static AtomUI.Desktop.Controls.Window CreateWindow(
        Control content,
        out ScopeAwareOverlayLayerPanel root)
    {
        root = new ScopeAwareOverlayLayerPanel
        {
            Width  = 480,
            Height = 360,
            Children = { content }
        };
        var window = new AtomUI.Desktop.Controls.Window
        {
            Width   = 480,
            Height  = 360,
            Content = root
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static void WaitWithDispatcherPump(Task task)
    {
        PumpUntil(() => task.IsCompleted);
        task.GetAwaiter().GetResult();
    }

    private static T WaitWithDispatcherPump<T>(Task<T> task)
    {
        PumpUntil(() => task.IsCompleted);
        return task.GetAwaiter().GetResult();
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
