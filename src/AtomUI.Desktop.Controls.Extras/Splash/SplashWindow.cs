using Avalonia;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

using AvaloniaWindow = Avalonia.Controls.Window;

public class SplashWindow : AvaloniaWindow
{
    #region 公共属性定义

    public static readonly StyledProperty<Splash?> SplashProperty =
        AvaloniaProperty.Register<SplashWindow, Splash?>(nameof(Splash));

    public static readonly StyledProperty<TimeSpan> MinimumShowDurationProperty =
        AvaloniaProperty.Register<SplashWindow, TimeSpan>(
            nameof(MinimumShowDuration),
            TimeSpan.FromMilliseconds(500));

    public static readonly StyledProperty<TimeSpan> CloseDelayProperty =
        AvaloniaProperty.Register<SplashWindow, TimeSpan>(
            nameof(CloseDelay),
            TimeSpan.Zero);

    public static readonly StyledProperty<TimeSpan> FadeOutDurationProperty =
        AvaloniaProperty.Register<SplashWindow, TimeSpan>(
            nameof(FadeOutDuration),
            TimeSpan.FromMilliseconds(180));

    public static readonly DirectProperty<SplashWindow, bool> IsCloseRequestedProperty =
        AvaloniaProperty.RegisterDirect<SplashWindow, bool>(
            nameof(IsCloseRequested),
            o => o.IsCloseRequested);

    public Splash? Splash
    {
        get => GetValue(SplashProperty);
        set => SetValue(SplashProperty, value);
    }

    public TimeSpan MinimumShowDuration
    {
        get => GetValue(MinimumShowDurationProperty);
        set => SetValue(MinimumShowDurationProperty, value);
    }

    public TimeSpan CloseDelay
    {
        get => GetValue(CloseDelayProperty);
        set => SetValue(CloseDelayProperty, value);
    }

    public TimeSpan FadeOutDuration
    {
        get => GetValue(FadeOutDurationProperty);
        set => SetValue(FadeOutDurationProperty, value);
    }

    private bool _isCloseRequested;

    public bool IsCloseRequested
    {
        get => _isCloseRequested;
        private set => SetAndRaise(IsCloseRequestedProperty, ref _isCloseRequested, value);
    }

    #endregion

    private DateTimeOffset? _shownAt;
    private bool _hasShown;
    private Task? _closeTask;

    public SplashWindow()
    {
    }

    protected override Type StyleKeyOverride { get; } = typeof(SplashWindow);

    public override void Show()
    {
        _shownAt = DateTimeOffset.UtcNow;
        _hasShown = true;
        base.Show();
    }

    public Task CloseAsync(CancellationToken cancellationToken = default)
    {
        _closeTask ??= CloseCoreAsync(cancellationToken);
        return _closeTask;
    }

    private async Task CloseCoreAsync(CancellationToken cancellationToken)
    {
        IsCloseRequested = true;

        await DelayForCloseScheduleAsync(cancellationToken);

        var splash = Splash;
        if (FadeOutDuration > TimeSpan.Zero && splash?.IsMotionEnabled == true)
        {
            await RunOnUiThreadAsync(() => splash.Opacity = 0, cancellationToken);
            await Task.Delay(FadeOutDuration, cancellationToken);
        }

        if (!_hasShown)
        {
            return;
        }

        await RunOnUiThreadAsync(Close, cancellationToken);
    }

    private async Task DelayForCloseScheduleAsync(CancellationToken cancellationToken)
    {
        if (_shownAt.HasValue && MinimumShowDuration > TimeSpan.Zero)
        {
            var elapsed   = DateTimeOffset.UtcNow - _shownAt.Value;
            var remaining = MinimumShowDuration - elapsed;
            if (remaining > TimeSpan.Zero)
            {
                await Task.Delay(remaining, cancellationToken);
            }
        }

        if (CloseDelay > TimeSpan.Zero)
        {
            await Task.Delay(CloseDelay, cancellationToken);
        }
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
}
