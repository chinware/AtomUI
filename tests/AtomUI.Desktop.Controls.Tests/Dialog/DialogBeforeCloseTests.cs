using System.Reflection;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Dialog;

public class DialogBeforeCloseTests
{
    static DialogBeforeCloseTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void BeforeCloseAsync_Returning_False_Keeps_Dialog_Open_And_Restores_Result()
    {
        var window = CreateWindow(new Control(), out var overlayPanel);
        DialogClosingContext? capturedContext = null;
        var callbackCount = 0;
        var dialog = CreateStaticDialog(
            overlayPanel,
            new DialogOptions
            {
                BeforeCloseAsync = context =>
                {
                    callbackCount++;
                    capturedContext = context;
                    return ValueTask.FromResult(false);
                }
            });

        overlayPanel.Children.Add(dialog);

        try
        {
            OpenNonModal(dialog);

            dialog.Accept();
            Dispatcher.UIThread.RunJobs();

            callbackCount.ShouldBe(1);
            dialog.IsOpen.ShouldBeTrue();
            dialog.Result.ShouldBeNull();
            capturedContext.ShouldNotBeNull();
            capturedContext.Dialog.ShouldBe(dialog);
            capturedContext.Result.ShouldBe(DialogCode.Accepted);
            capturedContext.DialogCode.ShouldBe(DialogCode.Accepted);
            capturedContext.Reason.ShouldBe(DialogCloseReason.Accepted);
            capturedContext.SourceButton.ShouldBeNull();
        }
        finally
        {
            overlayPanel.Children.Remove(dialog);
            window.Close();
        }
    }

    [Fact]
    public void BeforeCloseAsync_Returning_True_Allows_Dialog_To_Close()
    {
        var window = CreateWindow(new Control(), out var overlayPanel);
        var callbackCount = 0;
        var dialog = CreateStaticDialog(
            overlayPanel,
            new DialogOptions
            {
                BeforeCloseAsync = _ =>
                {
                    callbackCount++;
                    return ValueTask.FromResult(true);
                }
            });

        overlayPanel.Children.Add(dialog);

        try
        {
            OpenNonModal(dialog);

            dialog.Accept();
            Dispatcher.UIThread.RunJobs();

            callbackCount.ShouldBe(1);
            dialog.IsOpen.ShouldBeFalse();
            dialog.Result.ShouldBe(DialogCode.Accepted);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Close_Without_BeforeCloseAsync_Propagates_Synchronous_Finished_Exception()
    {
        var window = CreateWindow(new Control(), out var overlayPanel);
        var dialog = CreateStaticDialog(overlayPanel, new DialogOptions());
        var exception = new InvalidOperationException("finished failed");
        EventHandler<DialogFinishedEventArgs> handler = (_, _) => throw exception;
        dialog.Finished += handler;

        overlayPanel.Children.Add(dialog);

        try
        {
            OpenNonModal(dialog);

            var actual = Should.Throw<InvalidOperationException>(() => dialog.Accept());

            actual.ShouldBeSameAs(exception);
            dialog.IsOpen.ShouldBeTrue();
        }
        finally
        {
            dialog.Finished -= handler;
            overlayPanel.Children.Remove(dialog);
            window.Close();
        }
    }

    [Fact]
    public void ButtonClicked_Handled_Stops_Default_Close_And_Skips_BeforeCloseAsync()
    {
        var window = CreateWindow(new Control(), out var overlayPanel);
        var callbackCount = 0;
        var dialog = CreateStaticDialog(
            overlayPanel,
            new DialogOptions
            {
                StandardButtons = DialogStandardButton.Ok,
                BeforeCloseAsync = _ =>
                {
                    callbackCount++;
                    return ValueTask.FromResult(true);
                }
            });
        dialog.ButtonClicked += (_, args) => args.Handled = true;

        overlayPanel.Children.Add(dialog);

        try
        {
            OpenNonModal(dialog);

            ClickStandardButton(window, DialogStandardButton.Ok);
            Dispatcher.UIThread.RunJobs();

            callbackCount.ShouldBe(0);
            dialog.IsOpen.ShouldBeTrue();
            dialog.Result.ShouldBeNull();
        }
        finally
        {
            overlayPanel.Children.Remove(dialog);
            window.Close();
        }
    }

    [Fact]
    public void Closing_Cancel_Stops_Close_And_Skips_BeforeCloseAsync()
    {
        var window = CreateWindow(new Control(), out var overlayPanel);
        var callbackCount = 0;
        var dialog = CreateStaticDialog(
            overlayPanel,
            new DialogOptions
            {
                BeforeCloseAsync = _ =>
                {
                    callbackCount++;
                    return ValueTask.FromResult(true);
                }
            });
        dialog.Closing += (_, args) => args.Cancel = true;

        overlayPanel.Children.Add(dialog);

        try
        {
            OpenNonModal(dialog);

            dialog.Accept();
            Dispatcher.UIThread.RunJobs();

            callbackCount.ShouldBe(0);
            dialog.IsOpen.ShouldBeTrue();
            dialog.Result.ShouldBeNull();
        }
        finally
        {
            overlayPanel.Children.Remove(dialog);
            window.Close();
        }
    }

    [Fact]
    public void IsOpen_Close_Request_Restores_IsOpen_When_BeforeCloseAsync_Denies_Close()
    {
        var window = CreateWindow(new Control(), out var overlayPanel);
        var dialog = CreateStaticDialog(
            overlayPanel,
            new DialogOptions
            {
                BeforeCloseAsync = _ => ValueTask.FromResult(false)
            });

        overlayPanel.Children.Add(dialog);

        try
        {
            OpenNonModal(dialog);

            dialog.IsOpen = false;
            Dispatcher.UIThread.RunJobs();

            dialog.IsOpen.ShouldBeTrue();
            dialog.Result.ShouldBeNull();
        }
        finally
        {
            overlayPanel.Children.Remove(dialog);
            window.Close();
        }
    }

    [Fact]
    public void Pending_BeforeCloseAsync_Suppresses_Duplicate_Close_Requests()
    {
        var window = CreateWindow(new Control(), out var overlayPanel);
        var callbackCount = 0;
        var gate          = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        var dialog = CreateStaticDialog(
            overlayPanel,
            new DialogOptions
            {
                BeforeCloseAsync = _ =>
                {
                    callbackCount++;
                    return new ValueTask<bool>(gate.Task);
                }
            });

        overlayPanel.Children.Add(dialog);

        try
        {
            OpenNonModal(dialog);

            Dispatcher.UIThread.Post(dialog.Accept);
            Dispatcher.UIThread.RunJobs();
            Dispatcher.UIThread.Post(dialog.Accept);
            Dispatcher.UIThread.RunJobs();

            callbackCount.ShouldBe(1);
            dialog.IsOpen.ShouldBeTrue();

            gate.SetResult(true);
            RunDispatcherUntil(() => !dialog.IsOpen);

            callbackCount.ShouldBe(1);
            dialog.IsOpen.ShouldBeFalse();
            dialog.Result.ShouldBe(DialogCode.Accepted);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Host_Close_Request_Uses_HostCloseRequest_Reason()
    {
        var window = CreateWindow(new Control(), out var overlayPanel);
        DialogClosingContext? capturedContext = null;
        var dialog = CreateStaticDialog(
            overlayPanel,
            new DialogOptions
            {
                BeforeCloseAsync = context =>
                {
                    capturedContext = context;
                    return ValueTask.FromResult(false);
                }
            });

        overlayPanel.Children.Add(dialog);

        try
        {
            OpenNonModal(dialog);

            NotifyHostCloseRequest(dialog);
            Dispatcher.UIThread.RunJobs();

            dialog.IsOpen.ShouldBeTrue();
            capturedContext.ShouldNotBeNull();
            capturedContext.Reason.ShouldBe(DialogCloseReason.HostCloseRequest);
            capturedContext.DialogCode.ShouldBeNull();
        }
        finally
        {
            overlayPanel.Children.Remove(dialog);
            window.Close();
        }
    }

    [Fact]
    public void ShowDialogModal_Waits_Until_BeforeCloseAsync_Allows_Close()
    {
        var window = CreateWindow(new Control(), out _);
        var callbackCount = 0;
        var options = new DialogOptions
        {
            StandardButtons       = DialogStandardButton.Ok,
            DefaultStandardButton = DialogStandardButton.Ok,
            BeforeCloseAsync = _ =>
            {
                callbackCount++;
                if (callbackCount == 1)
                {
                    ScheduleClickStandardButton(window, DialogStandardButton.Ok);
                    return ValueTask.FromResult(false);
                }

                return ValueTask.FromResult(true);
            }
        };

        try
        {
            ScheduleClickStandardButton(window, DialogStandardButton.Ok);

            var result = AtomUI.Desktop.Controls.Dialog.ShowDialogModal(
                new AtomUI.Desktop.Controls.TextBlock { Text = "Dialog" },
                options: options,
                topLevel: window);

            result.ShouldBe(DialogCode.Accepted);
            callbackCount.ShouldBe(2);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ShowDialogModalAsync_Waits_Until_BeforeCloseAsync_Allows_Close()
    {
        var window = CreateWindow(new Control(), out _);
        var callbackCount = 0;
        var options = new DialogOptions
        {
            StandardButtons       = DialogStandardButton.Ok,
            DefaultStandardButton = DialogStandardButton.Ok,
            BeforeCloseAsync = _ =>
            {
                callbackCount++;
                if (callbackCount == 1)
                {
                    ScheduleClickStandardButton(window, DialogStandardButton.Ok);
                    return ValueTask.FromResult(false);
                }

                return ValueTask.FromResult(true);
            }
        };

        try
        {
            var resultTask = StartShowDialogModalAsync(window, options);
            ScheduleClickStandardButton(window, DialogStandardButton.Ok);

            var result = WaitWithDispatcherPump(resultTask);

            result.ShouldBe(DialogCode.Accepted);
            callbackCount.ShouldBe(2);
        }
        finally
        {
            window.Close();
        }
    }

    private static void OpenNonModal(AtomUI.Desktop.Controls.Dialog dialog)
    {
        var openTask = dialog.OpenAsync(TestContext.Current.CancellationToken);
        openTask.IsCompleted.ShouldBeTrue("non-modal Dialog.OpenAsync should finish after scheduling the opening motion.");
        Dispatcher.UIThread.RunJobs();
        dialog.IsOpen.ShouldBeTrue();
    }

    private static void ClickStandardButton(AvaloniaWindow window, DialogStandardButton standardButton)
    {
        var button = window.GetVisualDescendants()
                           .OfType<DialogButton>()
                           .Single(x => x.StandardButtonType == standardButton);
        button.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent, button));
    }

    private static void ScheduleClickStandardButton(
        AvaloniaWindow window,
        DialogStandardButton standardButton,
        int attempt = 0)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var button = window.GetVisualDescendants()
                               .OfType<DialogButton>()
                               .FirstOrDefault(x => x.StandardButtonType == standardButton);
            if (button is null || button.Bounds.Width <= 0 || button.Bounds.Height <= 0)
            {
                attempt.ShouldBeLessThan(10, "Dialog button should become available while the dispatcher frame is running.");
                ScheduleClickStandardButton(window, standardButton, attempt + 1);
                return;
            }

            SetOverlayDialogHostAnimationDuration(window, TimeSpan.Zero);
            button.RaiseEvent(new RoutedEventArgs(Avalonia.Controls.Button.ClickEvent, button));
        });
    }

    private static void SetOverlayDialogHostAnimationDuration(Visual searchRoot, TimeSpan duration)
    {
        var overlayDialogHost = searchRoot.GetVisualDescendants()
                                          .FirstOrDefault(x => x.GetType().Name == "OverlayDialogHost");
        if (overlayDialogHost is null)
        {
            return;
        }

        var property = overlayDialogHost.GetType().GetProperty(
            "AnimationDuration",
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(overlayDialogHost, duration);
    }

    private static void NotifyHostCloseRequest(AtomUI.Desktop.Controls.Dialog dialog)
    {
        var method = typeof(AtomUI.Desktop.Controls.Dialog).GetMethod(
            "NotifyDialogHostCloseRequest",
            BindingFlags.Instance | BindingFlags.NonPublic);

        method.ShouldNotBeNull();
        method.Invoke(dialog, null);
    }

    private static Task<object?> StartShowDialogModalAsync(AvaloniaWindow window, DialogOptions options)
    {
        Task<object?>? resultTask = null;
        Dispatcher.UIThread.Post(() =>
        {
            resultTask = AtomUI.Desktop.Controls.Dialog.ShowDialogModalAsync(
                new AtomUI.Desktop.Controls.TextBlock { Text = "Dialog" },
                options: options,
                topLevel: window,
                cancellationToken: TestContext.Current.CancellationToken);
        });

        var timeoutAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
        while (resultTask is null && DateTimeOffset.UtcNow < timeoutAt)
        {
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(1);
        }

        resultTask.ShouldNotBeNull("ShowDialogModalAsync should start on the UI dispatcher.");
        return resultTask;
    }

    private static T WaitWithDispatcherPump<T>(Task<T> task)
    {
        var timeoutAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
        while (!task.IsCompleted && DateTimeOffset.UtcNow < timeoutAt)
        {
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(1);
        }

        task.IsCompleted.ShouldBeTrue("the dialog task should complete after the second before-close approval.");
        return task.GetAwaiter().GetResult();
    }

    private static void RunDispatcherUntil(Func<bool> condition)
    {
        var timeoutAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
        while (!condition() && DateTimeOffset.UtcNow < timeoutAt)
        {
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(1);
        }

        condition().ShouldBeTrue("the expected dialog state should be reached while pumping dispatcher jobs.");
    }

    private static AtomUI.Desktop.Controls.Dialog CreateStaticDialog(
        Control placementTarget,
        DialogOptions options)
    {
        var createDialog = typeof(AtomUI.Desktop.Controls.Dialog).GetMethod(
            "CreateDialog",
            BindingFlags.Static | BindingFlags.NonPublic);

        createDialog.ShouldNotBeNull();
        var dialog = (AtomUI.Desktop.Controls.Dialog)createDialog.Invoke(
            null,
            [new AtomUI.Desktop.Controls.TextBlock { Text = "Dialog" }, null, options, placementTarget])!;
        dialog.IsModal = false;
        return dialog;
    }

    private static AvaloniaWindow CreateWindow(Control content, out ScopeAwareOverlayLayerPanel overlayPanel)
    {
        overlayPanel = new ScopeAwareOverlayLayerPanel
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

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }
}
