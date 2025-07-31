﻿using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.MotionScene;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

public class Popup : AvaloniaPopup,
                     IWaveSpiritAwareControl,
                     IResourceBindingManager
{
    #region 公共属性定义

    public event EventHandler<PopupFlippedEventArgs>? PositionFlipped;

    public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
        AvaloniaProperty.Register<Popup, BoxShadows>(nameof(MaskShadows));

    public static readonly StyledProperty<double> MarginToAnchorProperty =
        AvaloniaProperty.Register<Popup, double>(nameof(MarginToAnchor), 4);

    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<Popup>();
    
    public static readonly StyledProperty<AbstractMotion?> OpenMotionProperty = 
        AvaloniaProperty.Register<Popup, AbstractMotion?>(nameof(OpenMotion));
        
    public static readonly StyledProperty<AbstractMotion?> CloseMotionProperty = 
        AvaloniaProperty.Register<Popup, AbstractMotion?>(nameof(CloseMotion));

    public static readonly DirectProperty<Popup, bool> IsFlippedProperty =
        AvaloniaProperty.RegisterDirect<Popup, bool>(nameof(IsFlipped),
            o => o.IsFlipped,
            (o, v) => o.IsFlipped = v);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Popup>();

    public static readonly StyledProperty<bool> IsWaveSpiritEnabledProperty = 
        WaveSpiritAwareControlProperty.IsWaveSpiritEnabledProperty.AddOwner<Popup>();
    
    public static readonly StyledProperty<bool> IsMotionAwareOpenProperty =
        AvaloniaProperty.Register<Popup, bool>(nameof (IsMotionAwareOpen));
    
    public static readonly StyledProperty<bool> ConfigureBlankMaskWhenMotionAwareOpenProperty =
        AvaloniaProperty.Register<Popup, bool>(nameof (ConfigureBlankMaskWhenMotionAwareOpen), false);

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
    
    public bool IsMotionAwareOpen
    {
        get => GetValue(IsMotionAwareOpenProperty);
        set => SetValue(IsMotionAwareOpenProperty, value);
    }
    
    /// <summary>
    /// When the animation is turned on and off, whether to not make a complete mask after the animation ends, including the content
    /// </summary>
    public bool ConfigureBlankMaskWhenMotionAwareOpen
    {
        get => GetValue(ConfigureBlankMaskWhenMotionAwareOpenProperty);
        set => SetValue(ConfigureBlankMaskWhenMotionAwareOpenProperty, value);
    }

    #endregion
    
    public Func<IPopupHostProvider, RawPointerEventArgs, bool>? ClickHidePredicate;

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsDetectMouseClickEnabledProperty =
        AvaloniaProperty.Register<Popup, bool>(nameof(IsDetectMouseClickEnabled), true);

    internal bool IsDetectMouseClickEnabled
    {
        get => GetValue(IsDetectMouseClickEnabledProperty);
        set => SetValue(IsDetectMouseClickEnabledProperty, value);
    }

    Control IMotionAwareControl.PropertyBindTarget => this;

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }

    #endregion

    internal BaseMotionActor? MotionActor;
    private CompositeDisposable? _resourceBindingsDisposable;
    private PopupBuddyLayer? _buddyLayer;
    private IDisposable? _selfLightDismissDisposable;
    private IManagedPopupPositionerPopup? _managedPopupPositioner;
    private bool _isNeedDetectFlip = true;
    private bool _ignoreIsOpenChanged;

    // 在翻转之后或者恢复正常，会有属性的变动，在变动之后捕捉动画需要等一个事件循环，保证布局已经生效
    private bool _isNeedWaitFlipSync;
    private bool _openAnimating;
    private bool _closeAnimating;
    private bool _motionAwareOpened;

    // 当鼠标移走了，但是打开动画还没完成，我们需要记录下来这个信号
    internal bool RequestCloseWhereAnimationCompleted { get; set; }

    static Popup()
    {
        AffectsMeasure<Popup>(PlacementProperty);
        AffectsMeasure<Popup>(PlacementAnchorProperty);
        AffectsMeasure<Popup>(PlacementGravityProperty);
        IsLightDismissEnabledProperty.OverrideDefaultValue<Popup>(false);
    }

    public Popup()
    {
        this.BindWaveSpiritProperties();
        Closed                         += HandleClosed;
        Opened                         += HandleOpened;
        if (this is IPopupHostProvider popupHostProvider)
        {
            if (popupHostProvider.PopupHost != null)
            {
                HandlePopupHostChanged(popupHostProvider.PopupHost);
            }
            else
            {
                popupHostProvider.PopupHostChanged += HandlePopupHostChanged; 
            }
        }
    }

    private void HandlePopupHostChanged(IPopupHost? popupHost)
    {
        if (popupHost is PopupRoot popupRoot)
        {
            MotionActor = popupRoot.FindDescendantOfType<MotionActor>();
        }
    }
    
    // Popup 好像不加入视觉树，所以我们放在逻辑树
    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(
            TokenResourceBinder.CreateTokenBinding(this, MaskShadowsProperty, SharedTokenKey.BoxShadowsSecondary));
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty,
            SharedTokenKey.MotionDurationMid));
    }
    
    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
    
    protected Control? GetEffectivePlacementTarget()
    {
        return PlacementTarget ?? this.FindLogicalAncestorOfType<Control>();
    }

    private void HandleClosed(object? sender, EventArgs? args)
    {
        _buddyLayer?.Detach();
        var offsetX = HorizontalOffset;
        var offsetY = VerticalOffset;
        // 还原位移
        var marginToAnchorOffset =
            PopupUtils.CalculateMarginToAnchorOffset(Placement, MarginToAnchor, PlacementAnchor, PlacementGravity);
        offsetX -= marginToAnchorOffset.X;
        offsetY -= marginToAnchorOffset.Y;

        HorizontalOffset = offsetX;
        VerticalOffset   = offsetY;

        _selfLightDismissDisposable?.Dispose();
        if (IgnoreFirstDetected)
        {
            _firstDetected = true;
        }
    }

    private void HandleOpened(object? sender, EventArgs? args)
    {
        if (Host is PopupRoot popupRoot)
        {
            var popupPositioner = popupRoot.PlatformImpl?.PopupPositioner;
            if (popupPositioner is ManagedPopupPositioner managedPopupPositioner)
            {
                _managedPopupPositioner = managedPopupPositioner.GetManagedPopupPositionerPopup();
            }
#if DEBUG
            if (popupRoot is TopLevel topLevel)
            {
                topLevel.AttachDevTools();
            }
#endif
        }

        var placementTarget = GetEffectivePlacementTarget();
        if (placementTarget is not null)
        {
            if (_isNeedDetectFlip)
            {
                if (Placement != PlacementMode.Pointer && Placement != PlacementMode.Center)
                {
                    AdjustPopupHostPosition(placementTarget);
                }
            }

            if (!_openAnimating)
            {
                CreateBuddyLayer();
                _buddyLayer?.Attach();
            }

            // 如果没有启动，我们使用自己的处理函数，一般是为了增加我们自己的动画效果
            if (!IsLightDismissEnabled)
            {
                var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
                _selfLightDismissDisposable = inputManager.Process.Subscribe(HandleMouseClick);
            }
        }
    }
    
    // 正常的菜单项点击的时候第一次是需要忽略点击的探测的，不然按钮会被关闭
    // 这个可以优化动态探测 Popup 的 parent
    internal bool IgnoreFirstDetected { get; set; } = true;
    private bool _firstDetected = true;

    private void HandleMouseClick(RawInputEventArgs args)
    {
        if (!IsMotionAwareOpen)
        {
            return;
        }

        if (IsDetectMouseClickEnabled)
        {
            if (args is RawPointerEventArgs pointerEventArgs)
            {
                if (pointerEventArgs.Type == RawPointerEventType.LeftButtonUp)
                {
                    if ((IgnoreFirstDetected && _firstDetected) || _openAnimating)
                    {
                        if (IgnoreFirstDetected && _firstDetected)
                        {
                            _firstDetected = false;
                        }
                        return;
                    }
                    
                    if (this is IPopupHostProvider popupHostProvider)
                    {
                        if (ClickHidePredicate is not null)
                        {
                            if (ClickHidePredicate(popupHostProvider, pointerEventArgs))
                            {
                                MotionAwareClose();
                            }
                        }
                        else
                        {
                            if (popupHostProvider.PopupHost != pointerEventArgs.Root)
                            {
                                MotionAwareClose();
                            }
                        }
                    }
                }
            }
        }
    }
    
    private void CreateBuddyLayer()
    {
        if (_buddyLayer != null)
        {
            _buddyLayer.Detach();
        }
        
        var topLevel = TopLevel.GetTopLevel(PlacementTarget ?? Parent as Visual);
        Debug.Assert(topLevel is not null);
        _buddyLayer         = new PopupBuddyLayer(this, topLevel);
        _buddyLayer.Topmost = true;
        BindUtils.RelayBind(this, MaskShadowsProperty, _buddyLayer, PopupBuddyLayer.MaskShadowsProperty);
    }

    internal (bool, bool) CalculateFlipInfo(Size translatedSize, Rect anchorRect, PopupAnchor anchor,
                                            PopupGravity gravity,
                                            Point offset)
    {
        var bounds = GetBounds(anchorRect);
        return CalculateFlipInfo(bounds, translatedSize, anchorRect, anchor, gravity, offset);
    }

    internal static (bool, bool) CalculateFlipInfo(Rect bounds, Size translatedSize, Rect anchorRect,
                                                   PopupAnchor anchor,
                                                   PopupGravity gravity,
                                                   Point offset)
    {
        var result = (false, false);

        bool FitsInBounds(Rect rc, PopupAnchor edge = PopupAnchor.AllMask)
        {
            if ((edge.HasFlag(PopupAnchor.Left) && rc.X < bounds.X) ||
                (edge.HasFlag(PopupAnchor.Top) && rc.Y < bounds.Y) ||
                (edge.HasFlag(PopupAnchor.Right) && rc.Right > bounds.Right) ||
                (edge.HasFlag(PopupAnchor.Bottom) && rc.Bottom > bounds.Bottom))
            {
                return false;
            }

            return true;
        }

        Rect GetUnconstrained(PopupAnchor a, PopupGravity g)
        {
            return new Rect(PopupUtils.Gravitate(PopupUtils.GetAnchorPoint(anchorRect, a), translatedSize, g) + offset,
                translatedSize);
        }

        var geo = GetUnconstrained(anchor, gravity);
        // If flipping geometry and anchor is allowed and helps, use the flipped one,
        // otherwise leave it as is
        if (!FitsInBounds(geo, PopupAnchor.HorizontalMask))
        {
            result.Item1 = true;
        }

        if (!FitsInBounds(geo, PopupAnchor.VerticalMask))
        {
            result.Item2 = true;
        }

        return result;
    }

    private Rect GetBounds(Rect anchorRect)
    {
        // 暂时只支持窗口的方式
        if (_managedPopupPositioner is null)
        {
            throw new InvalidOperationException("ManagedPopupPositioner is null");
        }

        var parentGeometry = _managedPopupPositioner.ParentClientAreaScreenGeometry;
        var screens        = _managedPopupPositioner.Screens;
        return GetBounds(anchorRect, parentGeometry, screens);
    }

    private static Rect GetBounds(Rect anchorRect, Rect parentGeometry,
                                  IReadOnlyList<ManagedPopupPositionerScreenInfo> screens)
    {
        var targetScreen = screens.FirstOrDefault(s => s.Bounds.ContainsExclusive(anchorRect.TopLeft))
                           ?? screens.FirstOrDefault(s => s.Bounds.Intersects(anchorRect))
                           ?? screens.FirstOrDefault(s => s.Bounds.ContainsExclusive(parentGeometry.TopLeft))
                           ?? screens.FirstOrDefault(s => s.Bounds.Intersects(parentGeometry))
                           ?? screens.FirstOrDefault();

        if (targetScreen != null &&
            targetScreen.WorkingArea.Width == 0 && targetScreen.WorkingArea.Height == 0)
        {
            return targetScreen.Bounds;
        }

        return targetScreen?.WorkingArea
               ?? new Rect(0, 0, double.MaxValue, double.MaxValue);
    }

    /// <summary>
    /// 在这里处理翻转问题
    /// </summary>
    internal void AdjustPopupHostPosition(Control placementTarget)
    {
        var offsetX = HorizontalOffset;
        var offsetY = VerticalOffset;
        var marginToAnchorOffset =
            PopupUtils.CalculateMarginToAnchorOffset(Placement, MarginToAnchor, PlacementAnchor, PlacementGravity);
        offsetX          += marginToAnchorOffset.X;
        offsetY          += marginToAnchorOffset.Y;
        HorizontalOffset =  offsetX;
        VerticalOffset   =  offsetY;

        var direction = PopupUtils.GetDirection(Placement);
        var topLevel  = TopLevel.GetTopLevel(placementTarget)!;

        if (Placement != PlacementMode.Center && Placement != PlacementMode.Pointer)
        {
            // 计算是否 flip
            var parameters = new PopupPositionerParameters();
            var offset     = new Point(HorizontalOffset, VerticalOffset);
            parameters.ConfigurePosition(topLevel,
                placementTarget,
                Placement,
                offset,
                PlacementAnchor,
                PlacementGravity,
                PopupPositionerConstraintAdjustment.All,
                null,
                FlowDirection);

            Size popupSize;
            // Popup.Child can't be null here, it was set in ShowAtCore.
            if (Child!.DesiredSize == default)
            {
                // Popup may not have been shown yet. Measure content
                popupSize = LayoutHelper.MeasureChild(Child, Size.Infinity, new Thickness());
            }
            else
            {
                popupSize = Child.DesiredSize;
            }

            Debug.Assert(_managedPopupPositioner != null);
            var scaling = _managedPopupPositioner.Scaling;
            var anchorRect = new Rect(
                parameters.AnchorRectangle.TopLeft * scaling,
                parameters.AnchorRectangle.Size * scaling);
            anchorRect = anchorRect.Translate(_managedPopupPositioner.ParentClientAreaScreenGeometry.TopLeft);

            var flipInfo = CalculateFlipInfo(popupSize * scaling,
                anchorRect,
                parameters.Anchor,
                parameters.Gravity,
                offset * scaling);

            if (flipInfo.Item1 || flipInfo.Item2)
            {
                var flipPlacement        = GetFlipPlacement(Placement, flipInfo.Item1, flipInfo.Item2);
                var flipAnchorAndGravity = PopupUtils.GetAnchorAndGravity(flipPlacement);
                var flipOffset = PopupUtils.CalculateMarginToAnchorOffset(flipPlacement,
                    MarginToAnchor,
                    PlacementAnchor,
                    PlacementGravity);

                Placement        = flipPlacement;
                PlacementAnchor  = flipAnchorAndGravity.Item1;
                PlacementGravity = flipAnchorAndGravity.Item2;

                // 这里有个问题，目前需要重新看看，就是 X 轴 和 Y 轴会不会同时被反转呢？

                if (direction == Direction.Top || direction == Direction.Bottom)
                {
                    VerticalOffset = flipOffset.Y;
                }
                else
                {
                    HorizontalOffset = flipOffset.X;
                }

                IsFlipped = true;
            }
            else
            {
                IsFlipped = false;
            }

            PositionFlipped?.Invoke(this, new PopupFlippedEventArgs(IsFlipped));
        }
    }

    protected static PlacementMode GetFlipPlacement(PlacementMode placement, bool isHorizontalFlipped,
                                                    bool isVerticalFlipped)
    {
        return placement switch
        {
            PlacementMode.Left => isHorizontalFlipped ? PlacementMode.Right : PlacementMode.Left,
            PlacementMode.LeftEdgeAlignedTop => isHorizontalFlipped
                ? PlacementMode.RightEdgeAlignedTop
                : PlacementMode.LeftEdgeAlignedTop,
            PlacementMode.LeftEdgeAlignedBottom => isHorizontalFlipped
                ? PlacementMode.RightEdgeAlignedBottom
                : PlacementMode.LeftEdgeAlignedBottom,

            PlacementMode.Top => isVerticalFlipped ? PlacementMode.Bottom : PlacementMode.Top,
            PlacementMode.TopEdgeAlignedLeft => isVerticalFlipped
                ? PlacementMode.BottomEdgeAlignedLeft
                : PlacementMode.TopEdgeAlignedLeft,
            PlacementMode.TopEdgeAlignedRight => isVerticalFlipped
                ? PlacementMode.BottomEdgeAlignedRight
                : PlacementMode.TopEdgeAlignedRight,

            PlacementMode.Right => isHorizontalFlipped ? PlacementMode.Left : PlacementMode.Right,
            PlacementMode.RightEdgeAlignedTop => isHorizontalFlipped
                ? PlacementMode.LeftEdgeAlignedTop
                : PlacementMode.RightEdgeAlignedTop,
            PlacementMode.RightEdgeAlignedBottom => isHorizontalFlipped
                ? PlacementMode.LeftEdgeAlignedBottom
                : PlacementMode.RightEdgeAlignedBottom,

            PlacementMode.Bottom => isVerticalFlipped ? PlacementMode.Top : PlacementMode.Bottom,
            PlacementMode.BottomEdgeAlignedLeft => isVerticalFlipped
                ? PlacementMode.TopEdgeAlignedLeft
                : PlacementMode.BottomEdgeAlignedLeft,
            PlacementMode.BottomEdgeAlignedRight => isVerticalFlipped
                ? PlacementMode.TopEdgeAlignedRight
                : PlacementMode.BottomEdgeAlignedRight,

            _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, "Invalid value for PlacementMode")
        };
    }

    public void MotionAwareOpen(Action? opened = null)
    {
        // AbstractPopup is currently open
        if (_motionAwareOpened || _openAnimating || _closeAnimating)
        {
            return;
        }
        
        if (!IsMotionEnabled)
        {
            Open();
            _buddyLayer?.Show();
            opened?.Invoke();
            _motionAwareOpened = true;
            using (BeginIgnoringIsOpen())
            {
                SetCurrentValue(IsMotionAwareOpenProperty, true);
            }
            return;
        }
        
        if (_isNeedWaitFlipSync)
        {
            Dispatcher.UIThread.Post(() =>
            {
                ShowBuddyWithMotion(opened);
            });
            return;
        }
        
        ShowBuddyWithMotion(opened);
    }

    private void ShowBuddyWithMotion(Action? opened = null)
    {
        Open();
        Debug.Assert(MotionActor != null);
        _buddyLayer?.Show();

        var motion       = OpenMotion ?? new ZoomBigInMotion();
        if (MotionDuration != TimeSpan.Zero)
        {
            motion.Duration = MotionDuration;
        }

        _openAnimating = true;
        using (BeginIgnoringIsOpen())
        {
            SetCurrentValue(IsMotionAwareOpenProperty, true);
        }
        motion.Run(MotionActor, null, () =>
        {
            opened?.Invoke();
            _isNeedWaitFlipSync = false;
            _openAnimating      = false;
            _motionAwareOpened  = true;

            if (RequestCloseWhereAnimationCompleted)
            {
                RequestCloseWhereAnimationCompleted = false;
                Dispatcher.UIThread.Post(() => MotionAwareClose());
            }
        });
    }

    public void MotionAwareClose(Action? closed = null)
    {
        var popupRoot = Host as PopupRoot;
        if ((!_motionAwareOpened && !_openAnimating) || _closeAnimating)
        {
            return;
        }
        
        if (_openAnimating)
        {
            RequestCloseWhereAnimationCompleted = true;
            return;
        }
        Debug.Assert(MotionActor != null);
        if (!IsMotionEnabled || popupRoot == null)
        {
            _isNeedDetectFlip = true;
            Close();
            closed?.Invoke();
            _motionAwareOpened = false;
            using (BeginIgnoringIsOpen())
            {
                SetCurrentValue(IsMotionAwareOpenProperty, false);
            }
            return;
        }
        
        using (BeginIgnoringIsOpen())
        {
            SetCurrentValue(IsMotionAwareOpenProperty, false);
        }
        
        var motion       = CloseMotion ?? new ZoomBigOutMotion();
        if (MotionDuration != TimeSpan.Zero)
        {
            motion.Duration = MotionDuration;
        }
        _closeAnimating    = true;
        motion.Run(MotionActor, null, () =>
        {
            _buddyLayer?.Hide();
            Close();
            closed?.Invoke();
            _closeAnimating    = false;
            _motionAwareOpened = false;
            _isNeedDetectFlip  = true;
        });
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsFlippedProperty)
        {
            _isNeedWaitFlipSync = true;
        }
        else if (change.Property == IsMotionAwareOpenProperty)
        {
            if (!_ignoreIsOpenChanged)
            {
                if (change.GetNewValue<bool>())
                {
                    MotionAwareOpen();
                }
                else
                {
                    MotionAwareClose();
                }
            }
        }
    }
    
    private IgnoreIsOpenScope BeginIgnoringIsOpen() => new IgnoreIsOpenScope(this);
    
    private readonly struct IgnoreIsOpenScope : IDisposable
    {
        private readonly Popup _owner;

        public IgnoreIsOpenScope(Popup owner)
        {
            _owner                      = owner;
            _owner._ignoreIsOpenChanged = true;
        }

        public void Dispose() => _owner._ignoreIsOpenChanged = false;
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
