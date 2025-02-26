using System.Diagnostics;
using AtomUI.Controls.Primitives;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.MotionScene;
using AtomUI.Reflection;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
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

namespace AtomUI.Controls;

using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

public class Popup : AvaloniaPopup,
                     IAnimationAwareControl
{
    #region 公共属性定义

    public event EventHandler<PopupFlippedEventArgs>? PositionFlipped;

    public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
        Border.BoxShadowProperty.AddOwner<Popup>();

    public static readonly StyledProperty<double> MarginToAnchorProperty =
        AvaloniaProperty.Register<Popup, double>(nameof(MarginToAnchor), 4);

    public static readonly StyledProperty<TimeSpan> MotionDurationProperty
        = AvaloniaProperty.Register<Popup, TimeSpan>(nameof(MotionDuration));

    public static readonly DirectProperty<Popup, bool> IsFlippedProperty =
        AvaloniaProperty.RegisterDirect<Popup, bool>(nameof(IsFlipped),
            o => o.IsFlipped,
            (o, v) => o.IsFlipped = v);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AvaloniaProperty.Register<Popup, bool>(nameof(IsMotionEnabled));

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AvaloniaProperty.Register<Popup, bool>(nameof(IsWaveAnimationEnabled));

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

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsDetectMouseClickEnabledProperty =
        AvaloniaProperty.Register<Popup, bool>(nameof(IsDetectMouseClickEnabled), true);

    internal bool IsDetectMouseClickEnabled
    {
        get => GetValue(IsDetectMouseClickEnabledProperty);
        set => SetValue(IsDetectMouseClickEnabledProperty, value);
    }

    Control IAnimationAwareControl.PropertyBindTarget => this;

    #endregion

    private PopupShadowLayer? _shadowLayer;

    private IDisposable? _selfLightDismissDisposable;

    private IManagedPopupPositionerPopup? _managedPopupPositionerX;
    private bool _isNeedDetectFlip = true;
    private bool _openAnimating;
    private bool _closeAnimating;

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
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
        Closed += HandleClosed;
        Opened += HandleOpened;
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        CreateShadowLayer();
        TokenResourceBinder.CreateTokenBinding(this, MaskShadowsProperty, SharedTokenKey.BoxShadowsSecondary);
        TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty, SharedTokenKey.MotionDurationFast);
    }

    protected Control? GetEffectivePlacementTarget()
    {
        return PlacementTarget ?? this.FindLogicalAncestorOfType<Control>();
    }

    private void HandleClosed(object? sender, EventArgs? args)
    {
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
        _firstDetected = true;
    }

    private void HandleOpened(object? sender, EventArgs? args)
    {
        if (Host is PopupRoot popupRoot)
        {
            var popupPositioner = popupRoot.PlatformImpl?.PopupPositioner;
            if (popupPositioner is ManagedPopupPositioner managedPopupPositioner)
            {
                if (managedPopupPositioner.TryGetField("_popup",
                        out IManagedPopupPositionerPopup? managedPopupPositionerPopup))
                {
                    _managedPopupPositionerX = managedPopupPositionerPopup;
                }
            }
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
                _shadowLayer?.Open();
            }

            // 如果没有启动，我们使用自己的处理函数，一般是为了增加我们自己的动画效果
            if (!IsLightDismissEnabled)
            {
                var inputManager = AvaloniaLocator.Current.GetService<IInputManager>()!;
                _selfLightDismissDisposable = inputManager.Process.Subscribe(HandleMouseClick);
            }
        }
    }

    private bool _firstDetected = true;

    private void HandleMouseClick(RawInputEventArgs args)
    {
        if (!IsOpen)
        {
            return;
        }

        if (IsDetectMouseClickEnabled)
        {
            if (args is RawPointerEventArgs pointerEventArgs)
            {
                if (pointerEventArgs.Type == RawPointerEventType.LeftButtonUp)
                {
                    if (_firstDetected)
                    {
                        _firstDetected = false;
                        return;
                    }

                    if (this is IPopupHostProvider popupHostProvider)
                    {
                        if (popupHostProvider.PopupHost != pointerEventArgs.Root)
                        {
                            CloseAnimation();
                        }
                    }
                }
            }
        }
    }

    private void CreateShadowLayer()
    {
        if (_shadowLayer is not null)
        {
            return;
        }

        _shadowLayer = new PopupShadowLayer(this);
        BindUtils.RelayBind(this, MaskShadowsProperty, _shadowLayer);
        BindUtils.RelayBind(this, OpacityProperty, _shadowLayer);
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
        if (_managedPopupPositionerX is null)
        {
            throw new InvalidOperationException("ManagedPopupPositioner is null");
        }

        var parentGeometry = _managedPopupPositionerX.ParentClientAreaScreenGeometry;
        var screens        = _managedPopupPositionerX.Screens;
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

            Debug.Assert(_managedPopupPositionerX != null);
            var scaling = _managedPopupPositionerX.Scaling;
            var anchorRect = new Rect(
                parameters.AnchorRectangle.TopLeft * scaling,
                parameters.AnchorRectangle.Size * scaling);
            anchorRect = anchorRect.Translate(_managedPopupPositionerX.ParentClientAreaScreenGeometry.TopLeft);

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

    // TODO Review 需要评估这里等待的变成模式是否正确高效
    public void OpenAnimation(Action? opened = null)
    {
        // AbstractPopup is currently open
        if (IsOpen || _openAnimating || _closeAnimating)
        {
            return;
        }

        if (!IsMotionEnabled)
        {
            Open();
            opened?.Invoke();
            return;
        }

        _openAnimating = true;
        var placementTarget = GetEffectivePlacementTarget();
        Open();
        var popupRoot = Host as PopupRoot;
        Debug.Assert(popupRoot != null);
      
        var popupOffset = popupRoot.PlatformImpl!.Position;
        var topLevel    = TopLevel.GetTopLevel(placementTarget);
        var scaling     = 1.0;
        if (topLevel is WindowBase windowBase)
        {
            scaling = windowBase.DesktopScaling;
        }

        var offset = new Point(popupOffset.X, popupOffset.Y);

        var maskShadowsThickness              = MaskShadows.Thickness();
        var motionActorOffset = new Point(offset.X - maskShadowsThickness.Left * scaling, offset.Y - maskShadowsThickness.Top * scaling);

        var motionTarget = Child ?? popupRoot;
        var motion       = new ZoomBigInMotion(MotionDuration);
        var motionActor = new PopupMotionActor(motionActorOffset,
            MotionGhostControlUtils.BuildMotionGhost(Child ?? popupRoot, MaskShadows),
            motionTarget.DesiredSize);
        
        // 获取 popup 的具体位置，这个就是非常准确的位置，还有大小
        // TODO 暂时只支持 WindowBase popup
        popupRoot.Hide();
        
        motionActor.SceneParent = topLevel;

        Dispatcher.UIThread.Post(() =>
        {
            MotionInvoker.InvokeInPopupLayer(motionActor, motion, null, () =>
            {
                _shadowLayer?.Open();
                popupRoot.Show();
                opened?.Invoke();
                _openAnimating = false;
                if (RequestCloseWhereAnimationCompleted)
                {
                    RequestCloseWhereAnimationCompleted = false;
                    Dispatcher.UIThread.InvokeAsync(() => { CloseAnimation(); });
                }
            });
        });
    }

    public void CloseAnimation(Action? closed = null)
    {
        if (_closeAnimating)
        {
            return;
        }

        if (_openAnimating)
        {
            RequestCloseWhereAnimationCompleted = true;
            return;
        }

        if (!IsOpen)
        {
            return;
        }

        if (!IsMotionEnabled)
        {
            _shadowLayer?.Close();
            _isNeedDetectFlip = true;
            Close();
            closed?.Invoke();
            return;
        }

        _closeAnimating = true;

        var motion = new ZoomBigOutMotion(MotionDuration);

        var popupRoot       = (Host as PopupRoot)!;
        var popupOffset     = popupRoot.PlatformImpl!.Position;
        var offset          = new Point(popupOffset.X, popupOffset.Y);
        var placementTarget = GetEffectivePlacementTarget();
        var topLevel        = TopLevel.GetTopLevel(placementTarget);

        var scaling = 1.0;

        if (topLevel is WindowBase windowBase)
        {
            scaling = windowBase.DesktopScaling;
        }
        
        var maskShadowsThickness              = MaskShadows.Thickness();
        var motionActorOffset = new Point(offset.X - maskShadowsThickness.Left * scaling, offset.Y - maskShadowsThickness.Top * scaling);
        
        var motionTarget = Child ?? popupRoot;
        var motionActor = new PopupMotionActor(motionActorOffset, MotionGhostControlUtils.BuildMotionGhost(Child ?? popupRoot, MaskShadows),
            motionTarget.DesiredSize);
        
        motionActor.SceneParent = topLevel;

        Dispatcher.UIThread.Post(() =>
        {
            MotionInvoker.InvokeInPopupLayer(motionActor, motion, () =>
            {
                popupRoot.Hide();
                _shadowLayer?.Close();
            }, () =>
            {
                _closeAnimating   = false;
                _isNeedDetectFlip = true;
                Close();
                closed?.Invoke();
            });
        });
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