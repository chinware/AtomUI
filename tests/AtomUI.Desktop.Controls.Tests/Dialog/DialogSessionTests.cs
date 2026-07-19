using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Shouldly;
using Xunit;

using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Dialog;

public class DialogSessionTests
{
    static DialogSessionTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Duplicate_Close_Requests_Commit_Only_The_First_Result()
    {
        var dialog = new AtomUI.Desktop.Controls.Dialog();
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        presenter.Shown.Task.IsCompleted.ShouldBeTrue();

        WaitWithDispatcherPump(session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Accepted, null)).AsTask());
        WaitWithDispatcherPump(session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Rejected, DialogCloseReason.Rejected, null)).AsTask());

        WaitWithDispatcherPump(runTask).ShouldBe(DialogCode.Accepted);
        presenter.CloseCount.ShouldBe(1);
        session.State.ShouldBe(DialogSessionState.Closed);
    }

    [Fact]
    public void Vetoed_Close_Returns_To_Open_Until_A_Forced_Close_Arrives()
    {
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            BeforeCloseAsync = _ => ValueTask.FromResult(false)
        };
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        presenter.Shown.Task.IsCompleted.ShouldBeTrue();

        WaitWithDispatcherPump(session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Accepted, null)).AsTask());

        session.State.ShouldBe(DialogSessionState.Open);
        presenter.CloseCount.ShouldBe(0);
        runTask.IsCompleted.ShouldBeFalse();

        WaitWithDispatcherPump(session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask());

        WaitWithDispatcherPump(runTask).ShouldBeNull();
        presenter.CloseCount.ShouldBe(1);
        session.State.ShouldBe(DialogSessionState.Closed);
    }

    [Fact]
    public void Presenter_Close_Failure_Tears_Down_Before_It_Faults_Completion()
    {
        var exception = new InvalidOperationException("close failed");
        var dialog = new AtomUI.Desktop.Controls.Dialog();
        var presenter = new FakeDialogPresenter
        {
            CloseException = exception
        };
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        var closeTask = session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Accepted, null)).AsTask();

        Should.Throw<InvalidOperationException>(() => WaitWithDispatcherPump(closeTask))
              .ShouldBeSameAs(exception);
        Should.Throw<InvalidOperationException>(() => WaitWithDispatcherPump(runTask))
              .ShouldBeSameAs(exception);
        session.State.ShouldBe(DialogSessionState.Closed);
        presenter.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public void Cancellation_Callback_Failure_Does_Not_Interrupt_Teardown()
    {
        var expectedException = new InvalidOperationException("cancellation callback failed");
        var dialog = new AtomUI.Desktop.Controls.Dialog();
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        using var registration = presenter.ShowCancellationToken.Register(() => throw expectedException);
        var closeTask = session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask();

        Should.Throw<AggregateException>(() => WaitWithDispatcherPump(closeTask))
              .InnerExceptions.ShouldContain(expectedException);
        Should.Throw<AggregateException>(() => WaitWithDispatcherPump(runTask))
              .InnerExceptions.ShouldContain(expectedException);
        session.State.ShouldBe(DialogSessionState.Closed);
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public void Close_Policy_Failure_Returns_To_Open_Without_Tearing_Down()
    {
        var exception = new InvalidOperationException("policy failed");
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            BeforeCloseAsync = _ => ValueTask.FromException<bool>(exception)
        };
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        var closeTask = session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Accepted, null)).AsTask();

        Should.Throw<InvalidOperationException>(() => WaitWithDispatcherPump(closeTask))
              .ShouldBeSameAs(exception);
        session.State.ShouldBe(DialogSessionState.Open);
        presenter.CloseCount.ShouldBe(0);
        presenter.DisposeCount.ShouldBe(0);
        runTask.IsCompleted.ShouldBeFalse();

        WaitWithDispatcherPump(session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask());
        WaitWithDispatcherPump(runTask).ShouldBeNull();
    }

    [Fact]
    public void Forced_Close_Cancels_A_Pending_Policy_And_Commits_Once()
    {
        var policyStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var policyRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var policyToken = CancellationToken.None;
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            BeforeCloseAsync = async context =>
            {
                policyToken = context.CancellationToken;
                policyStarted.TrySetResult();
                await policyRelease.Task;
                return false;
            }
        };
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        var pendingCloseTask = session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Accepted, null)).AsTask();
        WaitWithDispatcherPump(policyStarted.Task);

        WaitWithDispatcherPump(session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask());
        var policyWasCanceled = policyToken.IsCancellationRequested;
        policyRelease.TrySetResult();
        WaitWithDispatcherPump(pendingCloseTask);

        policyWasCanceled.ShouldBeTrue();
        WaitWithDispatcherPump(runTask).ShouldBeNull();
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
        session.State.ShouldBe(DialogSessionState.Closed);
    }

    [Fact]
    public void Cancellation_While_Open_Tears_Down_Before_Completing_As_Canceled()
    {
        using var cancellationSource = new CancellationTokenSource();
        var closingCount = 0;
        var closedCount = 0;
        var beforeCloseCount = 0;
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            BeforeCloseAsync = _ =>
            {
                beforeCloseCount++;
                return ValueTask.FromResult(false);
            }
        };
        dialog.Closing += (_, e) =>
        {
            closingCount++;
            e.Cancel = true;
        };
        dialog.Closed += (_, _) => closedCount++;
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, cancellationSource.Token);

        var runTask = session.RunAsync();
        session.State.ShouldBe(DialogSessionState.Open);

        cancellationSource.Cancel();

        Should.Throw<OperationCanceledException>(() => WaitWithDispatcherPump(runTask));
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
        session.State.ShouldBe(DialogSessionState.Closed);
        closingCount.ShouldBe(1);
        beforeCloseCount.ShouldBe(0);
        closedCount.ShouldBe(1);
    }

    [Fact]
    public void Cancellation_While_Close_Policy_Is_Pending_Consumes_The_Policy_Cancellation()
    {
        using var cancellationSource = new CancellationTokenSource();
        var policyStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            BeforeCloseAsync = async context =>
            {
                policyStarted.TrySetResult();
                await Task.Delay(Timeout.InfiniteTimeSpan, context.CancellationToken);
                return true;
            }
        };
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, cancellationSource.Token);

        var runTask = session.RunAsync();
        var closeTask = session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Accepted, null)).AsTask();
        WaitWithDispatcherPump(policyStarted.Task);

        cancellationSource.Cancel();

        WaitWithDispatcherPump(closeTask);
        Should.Throw<OperationCanceledException>(() => WaitWithDispatcherPump(runTask));
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
        session.State.ShouldBe(DialogSessionState.Closed);
    }

    [Fact]
    public void Cancellation_While_Opening_Tears_Down_Before_Completing_As_Canceled()
    {
        using var cancellationSource = new CancellationTokenSource();
        var showRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var dialog = new AtomUI.Desktop.Controls.Dialog();
        var presenter = new FakeDialogPresenter
        {
            ShowTask = showRelease.Task
        };
        var session = new DialogSession(dialog, presenter, cancellationSource.Token);

        var runTask = session.RunAsync();
        presenter.Shown.Task.IsCompleted.ShouldBeTrue();
        session.State.ShouldBe(DialogSessionState.Opening);

        cancellationSource.Cancel();

        Should.Throw<OperationCanceledException>(() => WaitWithDispatcherPump(runTask));
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
        session.State.ShouldBe(DialogSessionState.Closed);
    }

    [Fact]
    public void Presenter_Show_Failure_Tears_Down_Before_Faulting_Completion()
    {
        var exception = new InvalidOperationException("show failed");
        var closingCount = 0;
        var closedCount = 0;
        var dialog = new AtomUI.Desktop.Controls.Dialog();
        dialog.Closing += (_, _) => closingCount++;
        dialog.Closed += (_, _) => closedCount++;
        var presenter = new FakeDialogPresenter
        {
            ShowTask = Task.FromException(exception)
        };
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();

        Should.Throw<InvalidOperationException>(() => WaitWithDispatcherPump(runTask))
              .ShouldBeSameAs(exception);
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
        session.State.ShouldBe(DialogSessionState.Closed);
        closingCount.ShouldBe(1);
        closedCount.ShouldBe(1);
    }

    [Fact]
    public void Reentrant_Forced_Close_During_Result_Notification_Does_Not_Commit_Again()
    {
        var acceptedCount = 0;
        var finishedCount = 0;
        var closedCount = 0;
        Task? reentrantCloseTask = null;
        DialogSession? session = null;
        var dialog = new AtomUI.Desktop.Controls.Dialog();
        dialog.Accepted += (_, _) =>
        {
            acceptedCount++;
            reentrantCloseTask = session!.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask();
        };
        dialog.Finished += (_, _) => finishedCount++;
        dialog.Closed += (_, _) => closedCount++;
        var presenter = new FakeDialogPresenter();
        session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        WaitWithDispatcherPump(session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Accepted, null)).AsTask());
        WaitWithDispatcherPump(reentrantCloseTask.ShouldNotBeNull());

        WaitWithDispatcherPump(runTask).ShouldBe(DialogCode.Accepted);
        acceptedCount.ShouldBe(1);
        finishedCount.ShouldBe(1);
        closedCount.ShouldBe(1);
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public void Presenter_Close_Request_Is_Routed_Through_The_Session_Once()
    {
        var policyCallCount = 0;
        var closeReason = DialogCloseReason.Programmatic;
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            BeforeCloseAsync = context =>
            {
                policyCallCount++;
                closeReason = context.Reason;
                return ValueTask.FromResult(true);
            }
        };
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        presenter.RequestClose(DialogCloseReason.HostCloseRequest);
        presenter.RequestClose(DialogCloseReason.HostCloseRequest);

        WaitWithDispatcherPump(runTask).ShouldBeNull();
        closeReason.ShouldBe(DialogCloseReason.HostCloseRequest);
        policyCallCount.ShouldBe(1);
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public void Presenter_Close_Request_Preserves_Result_And_Source_Button()
    {
        var sourceButton = new DialogButton { Role = DialogButtonRole.AcceptRole };
        DialogClosingContext? closingContext = null;
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            BeforeCloseAsync = context =>
            {
                closingContext = context;
                return ValueTask.FromResult(true);
            }
        };
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        presenter.RequestClose(
            DialogCloseReason.Accepted,
            DialogCode.Accepted,
            sourceButton);

        WaitWithDispatcherPump(runTask).ShouldBe(DialogCode.Accepted);
        closingContext.ShouldNotBeNull();
        closingContext.Result.ShouldBe(DialogCode.Accepted);
        closingContext.SourceButton.ShouldBeSameAs(sourceButton);
    }

    [Fact]
    public void Forced_Close_While_Opening_Cancels_Show_And_Tears_Down()
    {
        var showRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var dialog = new AtomUI.Desktop.Controls.Dialog();
        var presenter = new FakeDialogPresenter
        {
            ShowTask = showRelease.Task
        };
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        session.State.ShouldBe(DialogSessionState.Opening);

        WaitWithDispatcherPump(session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask());
        var firstForceClosedSession = session.State == DialogSessionState.Closed;
        var showWasCanceled = presenter.ShowCancellationToken.IsCancellationRequested;

        showRelease.TrySetResult();
        Avalonia.Threading.Dispatcher.UIThread.RunJobs();
        WaitWithDispatcherPump(session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask());
        WaitWithDispatcherPump(runTask).ShouldBeNull();

        firstForceClosedSession.ShouldBeTrue();
        showWasCanceled.ShouldBeTrue();
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public void Canceling_A_Pending_Close_Revokes_The_Policy_Commit()
    {
        var policyStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var policyRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var policyToken = CancellationToken.None;
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            BeforeCloseAsync = async context =>
            {
                policyToken = context.CancellationToken;
                policyStarted.TrySetResult();
                await policyRelease.Task;
                return true;
            }
        };
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        var pendingCloseTask = session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Accepted, null)).AsTask();
        WaitWithDispatcherPump(policyStarted.Task);

        session.CancelPendingClose();
        var policyWasCanceled = policyToken.IsCancellationRequested;
        policyRelease.TrySetResult();
        WaitWithDispatcherPump(pendingCloseTask);
        var remainedOpen = session.State == DialogSessionState.Open;
        var closeCount = presenter.CloseCount;

        if (!runTask.IsCompleted)
        {
            WaitWithDispatcherPump(session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask());
        }
        WaitWithDispatcherPump(runTask);

        policyWasCanceled.ShouldBeTrue();
        remainedOpen.ShouldBeTrue();
        closeCount.ShouldBe(0);
    }

    [Fact]
    public void Canceling_A_Pending_Close_Consumes_The_Policy_Cancellation()
    {
        var policyStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            BeforeCloseAsync = async context =>
            {
                policyStarted.TrySetResult();
                await Task.Delay(Timeout.InfiniteTimeSpan, context.CancellationToken);
                return true;
            }
        };
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        var pendingCloseTask = session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Accepted, null)).AsTask();
        WaitWithDispatcherPump(policyStarted.Task);

        session.CancelPendingClose();

        WaitWithDispatcherPump(pendingCloseTask);
        session.State.ShouldBe(DialogSessionState.Open);
        presenter.CloseCount.ShouldBe(0);

        WaitWithDispatcherPump(session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask());
        WaitWithDispatcherPump(runTask);
    }

    [Fact]
    public void Confirm_Loading_Blocks_Presenter_Close_But_Not_Programmatic_Close()
    {
        var policyCallCount = 0;
        var dialog = new AtomUI.Desktop.Controls.Dialog
        {
            IsConfirmLoading = true,
            BeforeCloseAsync = _ =>
            {
                policyCallCount++;
                return ValueTask.FromResult(true);
            }
        };
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        presenter.RequestClose(DialogCloseReason.Accepted, DialogCode.Accepted);
        Avalonia.Threading.Dispatcher.UIThread.RunJobs();

        session.State.ShouldBe(DialogSessionState.Open);
        policyCallCount.ShouldBe(0);
        presenter.CloseCount.ShouldBe(0);

        WaitWithDispatcherPump(session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Programmatic, null)).AsTask());
        WaitWithDispatcherPump(runTask).ShouldBe(DialogCode.Accepted);
        policyCallCount.ShouldBe(1);
        presenter.CloseCount.ShouldBe(1);
    }

    [Fact]
    public void Session_Disposal_Does_Not_Dispose_An_Already_Closed_Presenter_Again()
    {
        var dialog = new AtomUI.Desktop.Controls.Dialog();
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        WaitWithDispatcherPump(session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask());
        WaitWithDispatcherPump(runTask);

        WaitWithDispatcherPump(session.DisposeAsync().AsTask());
        WaitWithDispatcherPump(session.DisposeAsync().AsTask());

        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public void Closing_A_Session_Restores_The_Focus_Captured_Before_Show()
    {
        RunOnUIThread(() =>
        {
            var originalFocus = new Button { Focusable = true };
            var presenterFocus = new Border { Focusable = true };
            var placementTarget = new Border { Focusable = true };
            var window = new AvaloniaWindow
            {
                Content = new StackPanel
                {
                    Children = { originalFocus, presenterFocus, placementTarget }
                }
            };
            var presenter = new FakeDialogPresenter(presenterFocus);
            var session = new DialogSession(
                new AtomUI.Desktop.Controls.Dialog(),
                presenter,
                CancellationToken.None,
                placementTarget,
                window);

            try
            {
                window.Show();
                originalFocus.Focus().ShouldBeTrue();

                var runTask = session.RunAsync();
                window.FocusManager.GetFocusedElement().ShouldBeSameAs(presenterFocus);

                WaitWithDispatcherPump(session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask());
                WaitWithDispatcherPump(runTask);

                window.FocusManager.GetFocusedElement().ShouldBeSameAs(originalFocus);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [Fact]
    public void Closing_A_Session_Uses_Placement_Target_When_Previous_Focus_Is_Detached()
    {
        RunOnUIThread(() =>
        {
            var originalFocus = new Button { Focusable = true };
            var presenterFocus = new Border { Focusable = true };
            var placementTarget = new Border { Focusable = true };
            var content = new StackPanel
            {
                Children = { originalFocus, presenterFocus, placementTarget }
            };
            var window = new AvaloniaWindow { Content = content };
            var presenter = new FakeDialogPresenter(presenterFocus);
            var session = new DialogSession(
                new AtomUI.Desktop.Controls.Dialog(),
                presenter,
                CancellationToken.None,
                placementTarget,
                window);

            try
            {
                window.Show();
                originalFocus.Focus().ShouldBeTrue();

                var runTask = session.RunAsync();
                content.Children.Remove(originalFocus);
                WaitWithDispatcherPump(session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask());
                WaitWithDispatcherPump(runTask);

                window.FocusManager.GetFocusedElement().ShouldBeSameAs(placementTarget);
            }
            finally
            {
                window.Close();
            }
        });
    }

    [Fact]
    public void PostCommit_Event_Failure_Does_Not_Skip_Later_Notifications_Or_Teardown()
    {
        var firstException = new InvalidOperationException("accepted failed");
        var finishedCount = 0;
        var closedCount = 0;
        var dialog = new AtomUI.Desktop.Controls.Dialog();
        dialog.Accepted += (_, _) => throw firstException;
        dialog.Finished += (_, _) => finishedCount++;
        dialog.Closed += (_, _) => closedCount++;
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        var closeTask = session.RequestCloseAsync(
            new DialogCloseRequest(DialogCode.Accepted, DialogCloseReason.Accepted, null)).AsTask();

        Should.Throw<InvalidOperationException>(() => WaitWithDispatcherPump(closeTask))
              .ShouldBeSameAs(firstException);
        Should.Throw<InvalidOperationException>(() => WaitWithDispatcherPump(runTask))
              .ShouldBeSameAs(firstException);
        finishedCount.ShouldBe(1);
        closedCount.ShouldBe(1);
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
        session.State.ShouldBe(DialogSessionState.Closed);
    }

    [Fact]
    public void Forced_Close_Preserves_The_First_Lifecycle_Failure()
    {
        var closingException = new InvalidOperationException("closing failed");
        var finishedException = new InvalidOperationException("finished failed");
        var closedCount = 0;
        var dialog = new AtomUI.Desktop.Controls.Dialog();
        dialog.Closing += (_, _) => throw closingException;
        dialog.Finished += (_, _) => throw finishedException;
        dialog.Closed += (_, _) => closedCount++;
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();
        var closeTask = session.ForceCloseAsync(DialogCloseReason.OwnerClosed).AsTask();

        Should.Throw<InvalidOperationException>(() => WaitWithDispatcherPump(closeTask))
              .ShouldBeSameAs(closingException);
        Should.Throw<InvalidOperationException>(() => WaitWithDispatcherPump(runTask))
              .ShouldBeSameAs(closingException);
        closedCount.ShouldBe(1);
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
        session.State.ShouldBe(DialogSessionState.Closed);
    }

    [Fact]
    public void Opened_Event_Failure_Tears_Down_And_Faults_The_Session()
    {
        var exception = new InvalidOperationException("opened failed");
        var closingCount = 0;
        var closedCount = 0;
        var dialog = new AtomUI.Desktop.Controls.Dialog();
        dialog.Opened += (_, _) => throw exception;
        dialog.Closing += (_, _) => closingCount++;
        dialog.Closed += (_, _) => closedCount++;
        var presenter = new FakeDialogPresenter();
        var session = new DialogSession(dialog, presenter, CancellationToken.None);

        var runTask = session.RunAsync();

        Should.Throw<InvalidOperationException>(() => WaitWithDispatcherPump(runTask))
              .ShouldBeSameAs(exception);
        session.State.ShouldBe(DialogSessionState.Closed);
        presenter.CloseCount.ShouldBe(1);
        presenter.DisposeCount.ShouldBe(1);
        closingCount.ShouldBe(1);
        closedCount.ShouldBe(1);
        Should.Throw<InvalidOperationException>(() => WaitWithDispatcherPump(session.Completion))
              .ShouldBeSameAs(exception);
    }

    private static void WaitWithDispatcherPump(Task task)
    {
        WaitWithDispatcherPumpCore(task);
        task.GetAwaiter().GetResult();
    }

    private static T WaitWithDispatcherPump<T>(Task<T> task)
    {
        WaitWithDispatcherPumpCore(task);
        return task.GetAwaiter().GetResult();
    }

    private static void WaitWithDispatcherPumpCore(Task task)
    {
        var timeoutAt = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(5);
        while (!task.IsCompleted && DateTimeOffset.UtcNow < timeoutAt)
        {
            Avalonia.Threading.Dispatcher.UIThread.RunJobs();
            Thread.Sleep(1);
        }

        task.IsCompleted.ShouldBeTrue("the dialog session operation should complete while pumping dispatcher jobs.");
    }

    private static void RunOnUIThread(Action action)
    {
        Dispatcher.UIThread.Invoke(action);
    }

    private sealed class FakeDialogPresenter : IDialogPresenter
    {
        public event EventHandler<DialogPresenterCloseRequestedEventArgs>? CloseRequested;

        public IInputElement FocusScope { get; }

        public TaskCompletionSource Shown { get; } =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public int CloseCount { get; private set; }
        public int DisposeCount { get; private set; }
        public Exception? CloseException { get; init; }
        public Task? ShowTask { get; init; }
        public CancellationToken ShowCancellationToken { get; private set; }

        public FakeDialogPresenter(IInputElement? focusScope = null)
        {
            FocusScope = focusScope ?? new Avalonia.Controls.Control();
        }

        public ValueTask ShowAsync(CancellationToken cancellationToken)
        {
            ShowCancellationToken = cancellationToken;
            Shown.TrySetResult();
            return ShowTask is null
                ? ValueTask.CompletedTask
                : new ValueTask(ShowTask.WaitAsync(cancellationToken));
        }

        public ValueTask CloseAsync()
        {
            CloseCount++;
            if (CloseException is not null)
            {
                return ValueTask.FromException(CloseException);
            }

            return ValueTask.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            DisposeCount++;
            return ValueTask.CompletedTask;
        }

        public void RequestClose(
            DialogCloseReason reason,
            object? result = null,
            DialogButton? sourceButton = null)
        {
            CloseRequested?.Invoke(this, new DialogPresenterCloseRequestedEventArgs(reason, result, sourceButton));
        }
    }
}
