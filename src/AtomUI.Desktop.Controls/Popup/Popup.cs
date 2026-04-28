using System.ComponentModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.MotionScene;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

public class Popup : AvaloniaPopup, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
        AvaloniaProperty.Register<Popup, BoxShadows>(nameof(MaskShadows));

    public static readonly StyledProperty<double> MarginToAnchorProperty =
        AvaloniaProperty.Register<Popup, double>(nameof(MarginToAnchor));

    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<Popup>();

    public static readonly DirectProperty<Popup, bool> IsFlippedProperty =
        AvaloniaProperty.RegisterDirect<Popup, bool>(nameof(IsFlipped),
            o => o.IsFlipped,
            (o, v) => o.IsFlipped = v);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Popup>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<Popup>();

    public static readonly StyledProperty<AbstractMotion?> OpenMotionProperty =
        AvaloniaProperty.Register<Popup, AbstractMotion?>(nameof(OpenMotion));

    public static readonly StyledProperty<AbstractMotion?> CloseMotionProperty =
        AvaloniaProperty.Register<Popup, AbstractMotion?>(nameof(CloseMotion));
    
    public BoxShadows MaskShadows
    {
        get => GetValue(MaskShadowsProperty);
        set => SetValue(MaskShadowsProperty, value);
    }

    public double MarginToAnchor
    {
        get => GetValue(MarginToAnchorProperty);
        set => SetValue(MarginToAnchorProperty, value);
    }

    public TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    private bool _isFlipped;

    public bool IsFlipped
    {
        get => _isFlipped;
        private set => SetAndRaise(IsFlippedProperty, ref _isFlipped, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public bool IsWaveSpiritEnabled
    {
        get => GetValue(IsWaveSpiritEnabledProperty);
        set => SetValue(IsWaveSpiritEnabledProperty, value);
    }

    public AbstractMotion? OpenMotion
    {
        get => GetValue(OpenMotionProperty);
        set => SetValue(OpenMotionProperty, value);
    }

    public AbstractMotion? CloseMotion
    {
        get => GetValue(CloseMotionProperty);
        set => SetValue(CloseMotionProperty, value);
    }
    #endregion

    #region 公共事件定义

    public event EventHandler<PopupFlippedEventArgs>? PositionFlipped;

    #endregion

    #region 动画相关字段

    private bool _isClosingAnimating;
    private CancellationTokenSource? _motionCts;
    private PopupMotionActor? _motionActor;

    #endregion

    public Popup()
    {
        this.ConfigureMotionBindingStyle();
        TokenResourceBinder.CreateTokenBinding(this, MaskShadowsProperty, SharedTokenKind.BoxShadowsSecondary);
        TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty, SharedTokenKind.MotionDurationMid);

        this.AddClosingEventHandler(HandlePopupClosing);
        Opened += HandlePopupOpened;
    }

    #region 动画逻辑

    private void HandlePopupOpened(object? sender, EventArgs e)
    {
        if (!IsMotionEnabled || OpenMotion is null || _motionActor is null)
        {
            return;
        }
        
        _motionCts?.Cancel();
        _motionCts = new CancellationTokenSource();

        var motion = OpenMotion;
        motion.Duration      = MotionDuration;
        _motionActor.Opacity = 0.0d;
        Dispatcher.UIThread.InvokeAsync(() => PlayMotionAsync(motion, _motionActor, _motionCts.Token));
    }

    private void HandlePopupClosing(object? sender, CancelEventArgs e)
    {
        if (_isClosingAnimating)
        {
            _isClosingAnimating = false;
            _motionActor        = null;
            return;
        }

        if (!IsMotionEnabled || CloseMotion is null || _motionActor is null)
        {
            return;
        }

        e.Cancel = true;
        Dispatcher.UIThread.InvokeAsync(() => PlayCloseMotionAndCloseAsync(_motionActor));
    }

    private async Task PlayMotionAsync(AbstractMotion motion, BaseMotionActor actor, CancellationToken cancellationToken)
    {
        try
        {
            await motion.RunAsync(actor, cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // 动画被取消（快速开关场景），忽略
        }
    }

    private async Task PlayCloseMotionAndCloseAsync(BaseMotionActor actor)
    {
        if (_motionCts != null)
        {
           await _motionCts.CancelAsync();
        }
    
        _motionCts = new CancellationTokenSource();

        var motion = CloseMotion!;
        motion.Duration = MotionDuration;

        try
        {
            await motion.RunAsync(actor, cancellationToken: _motionCts.Token);
        }
        catch (OperationCanceledException)
        {
            // 动画被取消，仍然需要关闭
        }

        _isClosingAnimating = true;
        Dispatcher.UIThread.Post(Close);
    }

    #endregion

    internal void NotifyFlipped(bool flipped)
    {
        PositionFlipped?.Invoke(this, new PopupFlippedEventArgs(flipped));
    }

    internal void NotifyMotionActorReady(PopupMotionActor actor)
    {
        _motionActor = actor;
    }
}

public class PopupFlippedEventArgs : EventArgs
{
    public bool Flipped { get; set; }

    public PopupFlippedEventArgs(bool flipped)
    {
        Flipped = flipped;
    }
}
