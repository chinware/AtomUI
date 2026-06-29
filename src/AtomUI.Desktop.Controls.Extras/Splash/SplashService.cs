using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

public class SplashService : ISplashService
{
    public SplashWindow? CurrentWindow { get; private set; }

    public async Task<Splash> ShowAsync(
        SplashOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await CloseAsync(cancellationToken);
        options ??= new SplashOptions();

        return await RunOnUiThreadAsync(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var window = CreateWindow(null);
            ConfigureWindow(window, options);
            CurrentWindow = window;
            window.Closed += HandleWindowClosed;
            var ownerWindow = ResolveOwnerWindow(window);
            ShowWindow(window, ownerWindow);
            return window.Splash ?? throw new InvalidOperationException("SplashWindow.Splash must be configured before showing.");
        }, cancellationToken);
    }

    public Task SetMessageAsync(
        string? message,
        string? detail = null,
        CancellationToken cancellationToken = default)
    {
        return RunOnUiThreadAsync(() => CurrentWindow?.Splash?.SetMessage(message, detail), cancellationToken);
    }

    public Task SetProgressAsync(
        double? progress,
        string? message = null,
        string? detail = null,
        CancellationToken cancellationToken = default)
    {
        return RunOnUiThreadAsync(() => CurrentWindow?.Splash?.SetProgress(progress, message, detail), cancellationToken);
    }

    public Task SetStatusAsync(
        SplashStatus status,
        string? message = null,
        string? detail = null,
        CancellationToken cancellationToken = default)
    {
        return RunOnUiThreadAsync(() => CurrentWindow?.Splash?.SetStatus(status, message, detail), cancellationToken);
    }

    public Task SetErrorAsync(
        string message,
        string? detail = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(message);
        return RunOnUiThreadAsync(() => CurrentWindow?.Splash?.SetError(message, detail), cancellationToken);
    }

    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        var window = CurrentWindow;
        if (window is null)
        {
            return;
        }

        await window.CloseAsync(cancellationToken);

        if (ReferenceEquals(CurrentWindow, window))
        {
            ClearCurrentWindow(window);
        }
    }

    protected virtual SplashWindow CreateWindow(SplashOptions? options)
    {
        return new SplashWindow();
    }

    protected virtual void ConfigureWindow(SplashWindow window, SplashOptions options)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(options);

        var splash = window.Splash ?? new Splash();

        options.ApplyTo(splash);
        splash.Width     = options.Width;
        splash.MinHeight = options.MinHeight;

        window.Splash = splash;
        window.MinHeight = options.MinHeight;
        window.Topmost   = options.Topmost;

        window.MinimumShowDuration = options.MinimumShowDuration;
        window.CloseDelay          = options.CloseDelay;
        window.FadeOutDuration     = options.FadeOutDuration;
    }

    private void HandleWindowClosed(object? sender, EventArgs e)
    {
        if (sender is SplashWindow window && ReferenceEquals(CurrentWindow, window))
        {
            ClearCurrentWindow(window);
        }
    }

    private void ClearCurrentWindow(SplashWindow window)
    {
        window.Closed -= HandleWindowClosed;
        CurrentWindow = null;
    }

    private static AvaloniaWindow? ResolveOwnerWindow(SplashWindow window)
    {
        if (window.Owner is AvaloniaWindow existingOwner &&
            IsValidOwnerWindow(existingOwner, window))
        {
            return existingOwner;
        }

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime &&
            desktopLifetime.MainWindow is {} mainWindow &&
            IsValidOwnerWindow(mainWindow, window))
        {
            return mainWindow;
        }

        return null;
    }

    private static bool IsValidOwnerWindow(AvaloniaWindow ownerWindow, SplashWindow window)
    {
        return !ReferenceEquals(ownerWindow, window) && ownerWindow.IsVisible;
    }

    private static void ShowWindow(SplashWindow window, AvaloniaWindow? ownerWindow)
    {
        if (ownerWindow is not null)
        {
            window.Show(ownerWindow);
            return;
        }

        window.Show();
    }

    private static Task RunOnUiThreadAsync(Action action, CancellationToken cancellationToken)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            cancellationToken.ThrowIfCancellationRequested();
            action();
            return Task.CompletedTask;
        }

        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                completion.TrySetCanceled(cancellationToken);
                return;
            }

            try
            {
                action();
                completion.TrySetResult();
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        });
        return completion.Task;
    }

    private static Task<T> RunOnUiThreadAsync<T>(Func<T> action, CancellationToken cancellationToken)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Task.FromResult(action());
        }

        var completion = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        Dispatcher.UIThread.Post(() =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                completion.TrySetCanceled(cancellationToken);
                return;
            }

            try
            {
                completion.TrySetResult(action());
            }
            catch (Exception exception)
            {
                completion.TrySetException(exception);
            }
        });
        return completion.Task;
    }
}
