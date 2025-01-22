using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.MotionScene;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
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

public class Popup : AvaloniaPopup
{
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

    internal static readonly StyledProperty<bool> IsDetectMouseClickEnabledProperty =
        AvaloniaProperty.Register<Popup, bool>(nameof(IsDetectMouseClickEnabled), true);

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

    internal bool IsDetectMouseClickEnabled
    {
        get => GetValue(IsDetectMouseClickEnabledProperty);
        set => SetValue(IsDetectMouseClickEnabledProperty, value);
    }

    private bool _isFlipped;

    public bool IsFlipped
    {
        get => _isFlipped;
        private set => SetAndRaise(IsFlippedProperty, ref _isFlipped, value);
    }

    private PopupShadowLayer? _shadowLayer;
    private CompositeDisposable? _compositeDisposable;
    private IDisposable? _selfLightDismissDisposable;
    private bool _initialized;
    private ManagedPopupPositionerInfo? _managedPopupPositioner;
    private bool _isNeedFlip = true;
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
        Closed += HandleClosed;
        Opened += HandleOpened;
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        if (!_initialized)
        {
            TokenResourceBinder.CreateGlobalTokenBinding(this, MaskShadowsProperty,
                GlobalTokenResourceKey.BoxShadowsSecondary);
            TokenResourceBinder.CreateGlobalTokenBinding(this, MotionDurationProperty,
                GlobalTokenResourceKey.MotionDurationMid);
            _initialized = true;
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _compositeDisposable?.Dispose();
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
        VerticalOffset = offsetY;

        _compositeDisposable?.Dispose();
        _selfLightDismissDisposable?.Dispose();
        _firstDetected = true;
        _shadowLayer = null;
    }

    private void HandleOpened(object? sender, EventArgs? args)
    {
        var placementTarget = GetEffectivePlacementTarget();
        if (placementTarget is not null)
        {
            var toplevel = TopLevel.GetTopLevel(placementTarget);
            if (toplevel is null)
            {
                throw new InvalidOperationException(
                    "Unable to create shadow layer, top level for PlacementTarget is null.");
            }

            // 目前我们只支持 WindowBase Popup
            _managedPopupPositioner = new ManagedPopupPositionerInfo((toplevel as WindowBase)!);

            if (toplevel is null)
            {
                throw new InvalidOperationException(
                    "Unable to create shadow layer, top level for PlacementTarget is null.");
            }

            if (_isNeedFlip)
            {
                if (Placement != PlacementMode.Pointer && Placement != PlacementMode.Center)
                {
                    AdjustPopupHostPosition(placementTarget!);
                }
            }

            if (!_openAnimating)
            {
                CreateShadowLayer();
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

        var placementTarget = GetEffectivePlacementTarget();
        if (placementTarget is not null)
        {
            var popupRoot = Host as PopupRoot;
            var toplevel = TopLevel.GetTopLevel(popupRoot?.ParentTopLevel);
            if (toplevel is null)
            {
                throw new InvalidOperationException(
                    "Unable to create shadow layer, top level for PlacementTarget is null.");
            }

            _compositeDisposable = new CompositeDisposable();
            _shadowLayer = new PopupShadowLayer(toplevel);
            _compositeDisposable?.Add(BindUtils.RelayBind(this, MaskShadowsProperty, _shadowLayer!));
            _compositeDisposable?.Add(BindUtils.RelayBind(this, OpacityProperty, _shadowLayer!));
            _shadowLayer.AttachToTarget(this);
        }
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
        var screens = _managedPopupPositioner.Screens;
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
        offsetX += marginToAnchorOffset.X;
        offsetY += marginToAnchorOffset.Y;
        HorizontalOffset = offsetX;
        VerticalOffset = offsetY;

        var direction = PopupUtils.GetDirection(Placement);
        var topLevel = TopLevel.GetTopLevel(placementTarget)!;

        if (Placement != PlacementMode.Center && Placement != PlacementMode.Pointer)
        {
            // 计算是否 flip
            var parameters = new PopupPositionerParameters();
            var offset = new Point(HorizontalOffset, VerticalOffset);
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

            var scaling = _managedPopupPositioner!.Scaling;
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
                var flipPlacement = GetFlipPlacement(Placement);
                var flipAnchorAndGravity = PopupUtils.GetAnchorAndGravity(flipPlacement);
                var flipOffset = PopupUtils.CalculateMarginToAnchorOffset(flipPlacement, MarginToAnchor,
                    PlacementAnchor, PlacementGravity);

                Placement = flipPlacement;
                PlacementAnchor = flipAnchorAndGravity.Item1;
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

    protected static PlacementMode GetFlipPlacement(PlacementMode placement)
    {
        return placement switch
        {
            PlacementMode.Left => PlacementMode.Right,
            PlacementMode.LeftEdgeAlignedTop => PlacementMode.RightEdgeAlignedTop,
            PlacementMode.LeftEdgeAlignedBottom => PlacementMode.RightEdgeAlignedBottom,

            PlacementMode.Top => PlacementMode.Bottom,
            PlacementMode.TopEdgeAlignedLeft => PlacementMode.BottomEdgeAlignedLeft,
            PlacementMode.TopEdgeAlignedRight => PlacementMode.BottomEdgeAlignedRight,

            PlacementMode.Right => PlacementMode.Left,
            PlacementMode.RightEdgeAlignedTop => PlacementMode.LeftEdgeAlignedTop,
            PlacementMode.RightEdgeAlignedBottom => PlacementMode.LeftEdgeAlignedBottom,

            PlacementMode.Bottom => PlacementMode.Top,
            PlacementMode.BottomEdgeAlignedLeft => PlacementMode.TopEdgeAlignedLeft,
            PlacementMode.BottomEdgeAlignedRight => PlacementMode.TopEdgeAlignedRight,

            _ => throw new ArgumentOutOfRangeException(nameof(placement), placement, "Invalid value for PlacementMode")
        };
    }

    public void HideShadowLayer()
    {
        if (_shadowLayer is not null)
        {
            _shadowLayer.Opacity = 0;
        }
    }

    // TODO Review 需要评估这里等待的变成模式是否正确高效
    public void OpenAnimation(Action? opened = null)
    {
        // AbstractPopup is currently open
        if (IsOpen || _openAnimating || _closeAnimating)
        {
            return;
        }

        _openAnimating = true;

        var placementTarget = GetEffectivePlacementTarget();

        Open();

        var popupRoot = (Host as PopupRoot)!;
        // 获取 popup 的具体位置，这个就是非常准确的位置，还有大小
        // TODO 暂时只支持 WindowBase popup
        popupRoot.Hide();
        var popupOffset = popupRoot.PlatformImpl!.Position;
        var topLevel = TopLevel.GetTopLevel(placementTarget);
        var scaling = 1.0;
        if (topLevel is WindowBase windowBase)
        {
            scaling = windowBase.DesktopScaling;
        }

        var offset = new Point(popupOffset.X, popupOffset.Y);

        // 调度动画
        var motion = new ZoomBigInMotion(MotionDuration);

        var motionActor = new PopupMotionActor(MaskShadows, offset, scaling, Child ?? popupRoot);
        motionActor.SceneParent = topLevel;

        MotionInvoker.InvokeInPopupLayer(motionActor, motion, null, () =>
        {
            CreateShadowLayer();
            popupRoot.Show();
            if (opened is not null)
            {
                opened();
            }

            _openAnimating = false;
            if (RequestCloseWhereAnimationCompleted)
            {
                RequestCloseWhereAnimationCompleted = false;
                Dispatcher.UIThread.InvokeAsync(() => { CloseAnimation(); });
            }
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

        _closeAnimating = true;

        var motion = new ZoomBigOutMotion(MotionDuration);

        var popupRoot = (Host as PopupRoot)!;
        var popupOffset = popupRoot.PlatformImpl!.Position;
        var offset = new Point(popupOffset.X, popupOffset.Y);
        var placementTarget = GetEffectivePlacementTarget();
        var topLevel = TopLevel.GetTopLevel(placementTarget);

        var scaling = 1.0;

        if (topLevel is WindowBase windowBase)
        {
            scaling = windowBase.DesktopScaling;
        }

        var motionActor = new PopupMotionActor(MaskShadows, offset, scaling, Child ?? popupRoot);
        motionActor.SceneParent = topLevel;

        MotionInvoker.InvokeInPopupLayer(motionActor, motion, () =>
        {
            popupRoot.Opacity = 0;
            HideShadowLayer();
        }, () =>
        {
            _closeAnimating = false;
            _isNeedFlip = true;
            Close();
            if (closed is not null)
            {
                closed();
            }
        });
    }
}

internal class ManagedPopupPositionerInfo
{
    private readonly WindowBase _parent;

    public ManagedPopupPositionerInfo(WindowBase parent)
    {
        _parent = parent;
    }

    public IReadOnlyList<ManagedPopupPositionerScreenInfo> Screens =>
        _parent.Screens.All
            .Select(s => new ManagedPopupPositionerScreenInfo(s.Bounds.ToRect(1), s.WorkingArea.ToRect(1)))
            .ToArray();

    public Rect ParentClientAreaScreenGeometry
    {
        get
        {
            // Popup positioner operates with abstract coordinates, but in our case they are pixel ones
            var point = _parent.PointToScreen(default);
            var size = _parent.ClientSize * Scaling;
            return new Rect(point.X, point.Y, size.Width, size.Height);
        }
    }

    public double Scaling => _parent.DesktopScaling;
}

public class PopupFlippedEventArgs : EventArgs
{
    public bool Flipped { get; set; }

    public PopupFlippedEventArgs(bool flipped)
    {
        Flipped = flipped;
    }
}