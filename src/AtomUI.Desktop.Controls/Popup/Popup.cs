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
    
    public static readonly StyledProperty<CustomPlacementCallback?> CustomPlacementCallbackProperty =
        AvaloniaProperty.Register<Popup, CustomPlacementCallback?>(nameof(CustomPlacementCallback));
    
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
    
    public CustomPlacementCallback? CustomPlacementCallback
    {
        get => GetValue(CustomPlacementCallbackProperty);
        set => SetValue(CustomPlacementCallbackProperty, value);
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
                var originalPlacement = Placement;
                CustomPopupPlacementCallback = HandleCustomPlacement;
                Placement = PlacementMode.Custom;
                if (originalPlacement == PlacementMode.Custom)
                {
                    this.HandlePositionChange();
                }
            }
            else
            {
                CustomPopupPlacementCallback = null;
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
        var shadowThickness    = FrameShadow.Thickness();
        var requestedPlacement = RequestedPlacement!.Value;
        var isUseOverlayHost   = ShouldUseOverlayLayer;
        var hOffset            = HorizontalOffset;
        var vOffset            = VerticalOffset;
        var marginToAnchor     = MarginToAnchor;

        PopupAnchor anchor;
        PopupGravity gravity;

        if (requestedPlacement == PlacementMode.Center)
        {
            // Center 不依赖具体的 PlacementTarget，直接从 Popup 自身向上找 TopLevel。
            var referenceControl = (PlacementTarget ?? Parent as Control) ?? this;
            var topLevel         = TopLevel.GetTopLevel(referenceControl);
            if (topLevel == null)
            {
                return;
            }

            if (isUseOverlayHost)
            {
                // Overlay 模式：popup 渲染在覆盖整个窗口的 OverlayLayer 中，坐标系为窗口客户区逻辑坐标。
                // 将 AnchorRectangle 设为整个客户区，使 Anchor=None / Gravity=None 的定位器
                // 自动将 popup 居中于窗口。
                placement.AnchorRectangle = new Rect(default, topLevel.ClientSize);
            }
            else
            {
                // Popup Root 模式：popup 是独立的 OS 窗口。
                // ManagedPopupPositioner 会将 AnchorRectangle（窗口客户区逻辑坐标）自动加上
                // parentGeometry.TopLeft 转换为屏幕坐标，flip 约束边界为 screen.WorkingArea。
                // 设为整个窗口客户区，popup 最终居中于父窗口所在屏幕区域内。
                placement.AnchorRectangle = new Rect(default, topLevel.ClientSize);
            }

            anchor  = PopupAnchor.None;
            gravity = PopupGravity.None;
        }
        else
        {
            var target = PlacementTarget ?? Parent as Control;
            if (target is null)
            {
                return;
            }

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
                if (provider.IsShowArrow() && PopupUtils.CanEnabledArrow(requestedPlacement))
                {
                    Child.Measure(Size.Infinity);
                    Child.Arrange(new Rect(Child.DesiredSize));
                    arrowIndicatorLayoutBounds = provider.GetArrowDecoratedBox().ArrowIndicatorLayoutBounds;

                    if (IsPointAtCenter)
                    {
                        var delta = PopupUtils.CalculatePointAtCenterDelta(
                            target, provider.GetArrowDecoratedBox(), requestedPlacement, anchor, gravity);
                        hOffset += delta.X;
                        vOffset += delta.Y;
                    }
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
            CustomPlacementCallback?.Invoke(placement, shadowThickness, marginToAnchor, isUseOverlayHost, flipX, flipY);
            return;
        }

        // Center 走到这里：AnchorRectangle 已设为整个窗口客户区，Anchor/Gravity=None 定位器自动居中。
        // 居中不存在翻转，直接设置 Offset 并通知 (false, false)。
        placement.Anchor  = anchor;
        placement.Gravity = gravity;
        placement.Offset  = new Point(hOffset, vOffset);
        NotifyFlipped(false, false);
        CustomPlacementCallback?.Invoke(placement, shadowThickness, marginToAnchor, isUseOverlayHost, false, false);
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
