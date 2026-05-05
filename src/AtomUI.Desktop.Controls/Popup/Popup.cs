using System.ComponentModel;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Media;
using AtomUI.MotionScene;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;

namespace AtomUI.Desktop.Controls;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

public class Popup : AvaloniaPopup, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<BoxShadows> PopupRootShadowProperty =
        AvaloniaProperty.Register<Popup, BoxShadows>(nameof(PopupRootShadow));
    
    public static readonly StyledProperty<BoxShadows> OverlayHostShadowProperty =
        AvaloniaProperty.Register<Popup, BoxShadows>(nameof(OverlayHostShadow));

    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<Popup>();

    public static readonly DirectProperty<Popup, bool> IsHorizontalFlippedProperty =
        AvaloniaProperty.RegisterDirect<Popup, bool>(nameof(IsHorizontalFlipped),
            o => o.IsHorizontalFlipped,
            (o, v) => o.IsHorizontalFlipped = v);
    
    public static readonly DirectProperty<Popup, bool> IsVerticalFlippedProperty =
        AvaloniaProperty.RegisterDirect<Popup, bool>(nameof(IsVerticalFlipped),
            o => o.IsVerticalFlipped,
            (o, v) => o.IsVerticalFlipped = v);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Popup>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty =
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<Popup>();

    public static readonly StyledProperty<AbstractMotion?> OpenMotionProperty =
        AvaloniaProperty.Register<Popup, AbstractMotion?>(nameof(OpenMotion));

    public static readonly StyledProperty<AbstractMotion?> CloseMotionProperty =
        AvaloniaProperty.Register<Popup, AbstractMotion?>(nameof(CloseMotion));

    public static readonly StyledProperty<PlacementMode?> RequestedPlacementProperty =
        AvaloniaProperty.Register<Popup, PlacementMode?>(nameof(RequestedPlacement));

    public static readonly StyledProperty<double> MarginToAnchorProperty =
        AvaloniaProperty.Register<Popup, double>(nameof(MarginToAnchor));

    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        AvaloniaProperty.Register<Popup, bool>(nameof(IsPointAtCenter));
    
    public BoxShadows PopupRootShadow
    {
        get => GetValue(PopupRootShadowProperty);
        set => SetValue(PopupRootShadowProperty, value);
    }
    
    public BoxShadows OverlayHostShadow
    {
        get => GetValue(OverlayHostShadowProperty);
        set => SetValue(OverlayHostShadowProperty, value);
    }

    public TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }

    private bool _isHorizontalFlipped;

    public bool IsHorizontalFlipped
    {
        get => _isHorizontalFlipped;
        private set => SetAndRaise(IsHorizontalFlippedProperty, ref _isHorizontalFlipped, value);
    }
    
    private bool _isVerticalFlipped;

    public bool IsVerticalFlipped
    {
        get => _isVerticalFlipped;
        private set => SetAndRaise(IsVerticalFlippedProperty, ref _isVerticalFlipped, value);
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

    public PlacementMode? RequestedPlacement
    {
        get => GetValue(RequestedPlacementProperty);
        set => SetValue(RequestedPlacementProperty, value);
    }

    public double MarginToAnchor
    {
        get => GetValue(MarginToAnchorProperty);
        set => SetValue(MarginToAnchorProperty, value);
    }

    public bool IsPointAtCenter
    {
        get => GetValue(IsPointAtCenterProperty);
        set => SetValue(IsPointAtCenterProperty, value);
    }
    #endregion

    #region 公共事件定义

    public event EventHandler<PopupFlippedEventArgs>? PositionFlipped;

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<BoxShadows> FrameShadowProperty =
        AvaloniaProperty.Register<Popup, BoxShadows>(nameof(FrameShadow));    

    internal BoxShadows FrameShadow
    {
        get => GetValue(FrameShadowProperty);
        set => SetValue(FrameShadowProperty, value);
    }
    #endregion

    #region 动画相关字段

    private bool _isClosingAnimating;
    private bool _isPlayingCloseMotion;
    private CancellationTokenSource? _motionCts;
    private PopupMotionActor? _motionActor;
    private bool _isRedirectingPlacement;

    #endregion

    public Popup()
    {
        this.ConfigureMotionBindingStyle();
        TokenResourceBinder.CreateTokenBinding(this, PopupRootShadowProperty, PopupHostTokenKind.PopupRootShadow);
        TokenResourceBinder.CreateTokenBinding(this, OverlayHostShadowProperty, PopupHostTokenKind.OverlayHostShadow);
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

        _isPlayingCloseMotion = true;
        _motionCts = new CancellationTokenSource();

        var motion = CloseMotion!;
        motion.Duration = MotionDuration;

        try
        {
            await motion.RunAsync(actor, cancellationToken: _motionCts.Token);
        }
        catch (OperationCanceledException)
        {
            _isPlayingCloseMotion = false;
            return;
        }

        _isPlayingCloseMotion = false;
        _isClosingAnimating = true;
        Dispatcher.UIThread.Post(Close);
    }

    #endregion

    internal bool IsPlayingCloseMotion => _isPlayingCloseMotion;

    internal void CancelCloseAnimation()
    {
        if (!_isPlayingCloseMotion)
        {
            return;
        }

        _motionCts?.Cancel();
        if (_motionActor is not null)
        {
            _motionActor.Opacity = 1.0d;
        }

        IsOpen = true;
    }

    internal void NotifyFlipped(bool horizontalFlipped,  bool verticalFlipped)
    {
        IsHorizontalFlipped = horizontalFlipped;
        IsVerticalFlipped   = verticalFlipped;
        PositionFlipped?.Invoke(this, new PopupFlippedEventArgs(horizontalFlipped, verticalFlipped));
    }

    internal void NotifyMotionActorReady(PopupMotionActor actor)
    {
        _motionActor = actor;
    }

    #region 自定义定位逻辑

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == RequestedPlacementProperty)
        {
            var requestedPlacement = change.GetNewValue<PlacementMode?>();
            if (requestedPlacement is not null)
            {
                CustomPopupPlacementCallback = HandleCustomPlacement;
                Placement = PlacementMode.Custom;
            }
            else
            {
                CustomPopupPlacementCallback = null;
            }
        }
        else if (change.Property == PlacementProperty && !_isRedirectingPlacement)
        {
            var placement = change.GetNewValue<PlacementMode>();
            if (placement != PlacementMode.Custom)
            {
                _isRedirectingPlacement = true;
                RequestedPlacement = placement;
                CustomPopupPlacementCallback = HandleCustomPlacement;
                Placement = PlacementMode.Custom;
                _isRedirectingPlacement = false;
            }
        }
        else if (change.Property == PopupRootShadowProperty ||
                 change.Property == OverlayHostShadowProperty ||
                 change.Property == IsUsingOverlayLayerProperty)
        {
            ConfigureFrameShadow();
        }
    }

    internal void HandleCustomPlacement(CustomPopupPlacement placement)
    {
        var target = PlacementTarget ?? Parent as Control;
        if (target is null)
        {
            return;
        }

        var shadowThickness    = FrameShadow.Thickness();
        var requestedPlacement = RequestedPlacement!.Value;
        var isUseOverlayHost   = ShouldUseOverlayLayer;
        var hOffset            = HorizontalOffset;
        var vOffset            = VerticalOffset;
        var marginToAnchor     = MarginToAnchor;

        PopupAnchor anchor;
        PopupGravity gravity;

        if (requestedPlacement == PlacementMode.Pointer)
        {
            var topLevel = TopLevel.GetTopLevel(target);
            if (topLevel == null)
            {
                return;
            }

            var position = topLevel.PointToClient(topLevel.GetLastPointerPosition() ?? default);
            placement.AnchorRectangle = new Rect(position, new Size(1, 1));
            anchor  = PopupAnchor.TopLeft;
            gravity = PopupGravity.BottomRight;
        }
        else
        {
            (anchor, gravity) = PopupUtils.GetAnchorAndGravity(requestedPlacement);
        }

        var arrowIndicatorLayoutBounds = default(Rect);
        if (Child is IArrowAwareShadowMaskInfoProvider provider)
        {
            // If ArrowIndicatorLayoutBounds is not initialized yet, force a layout pass
            if (provider.IsShowArrow())
            {
                Child.Measure(Size.Infinity);
                Child.Arrange(new Rect(Child.DesiredSize));
                arrowIndicatorLayoutBounds = provider.GetArrowDecoratedBox().ArrowIndicatorLayoutBounds;
            }

            if (IsPointAtCenter && provider.IsShowArrow())
            {
                var delta = PopupUtils.CalculatePointAtCenterDelta(
                    target, provider.GetArrowDecoratedBox(), requestedPlacement, anchor, gravity);
                hOffset += delta.X;
                vOffset += delta.Y;
            }
        }

        var (flipX, flipY) = PopupUtils.ApplyCustomPlacement(
            placement,
            requestedPlacement,
            isUseOverlayHost,
            hOffset,
            vOffset,
            marginToAnchor,
            shadowThickness,
            anchor,
            gravity,
            arrowIndicatorLayoutBounds);

        NotifyFlipped(flipX, flipY);
    }

    #endregion

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        ConfigureFrameShadow();
    }

    private void ConfigureFrameShadow()
    {
        if (IsUsingOverlayLayer)
        {
            FrameShadow = OverlayHostShadow;
        }
        else
        {
            FrameShadow = PopupRootShadow;
        }
    }
}

public class PopupFlippedEventArgs : EventArgs
{
    public bool HorizontalFlipped { get; set; }
    public bool VerticalFlipped { get; set; }

    public PopupFlippedEventArgs(bool horizontalFlipped, bool verticalFlipped)
    {
        HorizontalFlipped = horizontalFlipped;
        VerticalFlipped = verticalFlipped;
    }
}
