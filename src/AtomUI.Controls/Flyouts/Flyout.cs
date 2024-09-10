using System.ComponentModel;
using System.Reactive.Disposables;
using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using PopupControl = Popup;

public class Flyout : PopupFlyoutBase
{
    private static readonly DirectProperty<Flyout, TimeSpan> MotionDurationTokenProperty
        = AvaloniaProperty.RegisterDirect<Flyout, TimeSpan>(nameof(_motionDuration),
            o => o._motionDuration,
            (o, v) => o._motionDuration = v);

    private Classes? _classes;

    protected CompositeDisposable? _compositeDisposable;

    private TimeSpan _motionDuration;

    static Flyout()
    {
        IsShowArrowProperty.OverrideDefaultValue<Flyout>(false);
    }

    public Flyout()
    {
        TokenResourceBinder.CreateGlobalTokenBinding(this, MotionDurationTokenProperty,
            GlobalTokenResourceKey.MotionDurationMid);
        TokenResourceBinder.CreateGlobalTokenBinding(this, MaskShadowsProperty,
            GlobalTokenResourceKey.BoxShadowsSecondary);
    }

    /// <summary>
    /// Gets the Classes collection to apply to the FlyoutPresenter this Flyout is hosting
    /// </summary>
    public Classes FlyoutPresenterClasses => _classes ??= new Classes();

    private void HandlePopupPropertyChanged(AvaloniaPropertyChangedEventArgs args)
    {
        SetupArrowPosition(Popup);
    }

    protected void SetupArrowPosition(Popup popup, FlyoutPresenter? flyoutPresenter = null)
    {
        if (flyoutPresenter is null)
        {
            var child = popup.Child;
            if (child is FlyoutPresenter childPresenter)
            {
                flyoutPresenter = childPresenter;
            }
        }

        var placement = Placement;
        var anchor    = PlacementAnchor;
        var gravity   = PlacementGravity;

        if (flyoutPresenter is not null)
        {
            var arrowPosition = PopupUtils.CalculateArrowPosition(placement, anchor, gravity);
            if (arrowPosition.HasValue)
            {
                flyoutPresenter.ArrowPosition = arrowPosition.Value;
            }
        }
    }

    protected override Control CreatePresenter()
    {
        var presenter = new FlyoutPresenter
        {
            [!ContentControl.ContentProperty] = this[!ContentProperty]
        };
        BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, presenter, IsShowArrowProperty);
        CalculateShowArrowEffective();
        SetupArrowPosition(Popup, presenter);
        return presenter;
    }

    protected internal override void NotifyPopupCreated(Popup popup)
    {
        base.NotifyPopupCreated(popup);
        BindUtils.RelayBind(this, PlacementProperty, popup, Avalonia.Controls.Primitives.Popup.PlacementProperty);
        BindUtils.RelayBind(this, PlacementAnchorProperty, popup,
            Avalonia.Controls.Primitives.Popup.PlacementAnchorProperty);
        BindUtils.RelayBind(this, PlacementGravityProperty, popup,
            Avalonia.Controls.Primitives.Popup.PlacementGravityProperty);
        BindUtils.RelayBind(this, MaskShadowsProperty, popup, Popup.MaskShadowsProperty);
        SetupArrowPosition(popup);
    }

    protected override void OnOpening(CancelEventArgs args)
    {
        _compositeDisposable = new CompositeDisposable();
        if (Popup.Child is { } presenter)
        {
            if (_classes != null)
            {
                SetPresenterClasses(presenter, FlyoutPresenterClasses);
            }

            if (FlyoutPresenterTheme is { } theme)
            {
                presenter.SetValue(StyledElement.ThemeProperty, theme);
            }
        }

        base.OnOpening(args);
        if (!args.Cancel)
        {
            _compositeDisposable.Add(PopupControl.IsFlippedProperty.Changed.Subscribe(HandlePopupPropertyChanged));
        }
    }

    protected override void OnClosed()
    {
        base.OnClosed();
        _compositeDisposable?.Dispose();
    }

    private Point CalculatePopupPositionDelta(Control anchorTarget,
                                              Control? flyoutPresenter,
                                              PlacementMode placement,
                                              PopupAnchor? anchor = null,
                                              PopupGravity? gravity = null)
    {
        var offsetX = 0d;
        var offsetY = 0d;
        if (IsShowArrow && IsPointAtCenter)
        {
            if (PopupUtils.CanEnabledArrow(placement, anchor, gravity))
            {
                if (flyoutPresenter is ArrowDecoratedBox arrowDecoratedBox)
                {
                    var arrowVertexPoint = arrowDecoratedBox.ArrowVertexPoint;

                    var anchorSize = anchorTarget.Bounds.Size;
                    var centerX    = anchorSize.Width / 2;
                    var centerY    = anchorSize.Height / 2;

                    // 这里计算不需要全局坐标
                    if (placement == PlacementMode.TopEdgeAlignedLeft ||
                        placement == PlacementMode.BottomEdgeAlignedLeft)
                    {
                        offsetX += centerX - arrowVertexPoint.Item1;
                    }
                    else if (placement == PlacementMode.TopEdgeAlignedRight ||
                             placement == PlacementMode.BottomEdgeAlignedRight)
                    {
                        offsetX -= centerX - arrowVertexPoint.Item2;
                    }
                    else if (placement == PlacementMode.RightEdgeAlignedTop ||
                             placement == PlacementMode.LeftEdgeAlignedTop)
                    {
                        offsetY += centerY - arrowVertexPoint.Item1;
                    }
                    else if (placement == PlacementMode.RightEdgeAlignedBottom ||
                             placement == PlacementMode.LeftEdgeAlignedBottom)
                    {
                        offsetY -= centerY - arrowVertexPoint.Item2;
                    }
                }
            }
        }

        return new Point(offsetX, offsetY);
    }

    // 因为在某些 placement 下箭头是不能显示的
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == IsShowArrowProperty ||
            e.Property == PlacementProperty ||
            e.Property == PlacementAnchorProperty ||
            e.Property == PlacementGravityProperty)
        {
            CalculateShowArrowEffective();
        }

        if (e.Property == PlacementProperty)
        {
            SetupArrowPosition(Popup);
        }
    }

    protected void CalculateShowArrowEffective()
    {
        if (IsShowArrow == false)
        {
            IsShowArrowEffective = false;
        }
        else
        {
            IsShowArrowEffective = PopupUtils.CanEnabledArrow(Placement, PlacementAnchor, PlacementGravity);
        }
    }

    protected internal override void NotifyPositionPopup(bool showAtPointer)
    {
        if (Popup.Child!.DesiredSize == default)
        {
            LayoutHelper.MeasureChild(Popup.Child, Size.Infinity, new Thickness());
        }

        Popup.PlacementAnchor  = PlacementAnchor;
        Popup.PlacementGravity = PlacementGravity;

        if (showAtPointer)
        {
            Popup.Placement = PlacementMode.Pointer;
        }
        else
        {
            Popup.Placement                     = Placement;
            Popup.PlacementConstraintAdjustment = PlacementConstraintAdjustment;
        }

        var pointAtCenterOffset =
            CalculatePopupPositionDelta(Target!, Popup.Child, Popup.Placement, Popup.PlacementAnchor,
                Popup.PlacementGravity);

        var offsetX = HorizontalOffset;
        var offsetY = VerticalOffset;
        if (IsPointAtCenter)
        {
            offsetX += pointAtCenterOffset.X;
            offsetY += pointAtCenterOffset.Y;
        }

        // 更新弹出信息是否指向中点
        Popup.HorizontalOffset = offsetX;
        Popup.VerticalOffset   = offsetY;
    }

    protected override bool ShowAtCore(Control placementTarget, bool showAtPointer = false)
    {
        if (IsOpen)
        {
            return false;
        }

        if (!PrepareShowPopup(placementTarget, showAtPointer))
        {
            return false;
        }

        IsOpen = true;
        Dispatcher.UIThread.Post(() =>
        {
            Popup.OpenAnimation();
            HandlePopupOpened(placementTarget);
        });
        return true;
    }

    protected override bool HideCore(bool canCancel = true)
    {
        if (!IsOpen)
        {
            return false;
        }

        if (canCancel)
        {
            if (CancelClosing())
            {
                return false;
            }
        }

        if (Popup.PlacementTarget?.GetVisualRoot() is null)
        {
            return base.HideCore(false);
        }

        IsOpen = false;
        Dispatcher.UIThread.Post(() => { Popup.CloseAnimation(HandlePopupClosed); });
        return true;
    }

    private bool CancelClosing()
    {
        var eventArgs = new CancelEventArgs();
        OnClosing(eventArgs);
        return eventArgs.Cancel;
    }

    #region 公共属性定义

    /// <summary>
    /// 是否显示指示箭头
    /// </summary>
    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<PopupFlyoutBase>();

    public static readonly StyledProperty<BoxShadows> MaskShadowsProperty =
        Border.BoxShadowProperty.AddOwner<Flyout>();

    /// <summary>
    /// 箭头是否始终指向中心
    /// </summary>
    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        AvaloniaProperty.Register<Flyout, bool>(nameof(IsPointAtCenter));

    /// <summary>
    /// Defines the <see cref="Content" /> property
    /// </summary>
    public static readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<Flyout, object>(nameof(Content));

    /// <summary>
    /// Defines the <see cref="FlyoutPresenterTheme" /> property.
    /// </summary>
    public static readonly StyledProperty<ControlTheme?> FlyoutPresenterThemeProperty =
        AvaloniaProperty.Register<Flyout, ControlTheme?>(nameof(FlyoutPresenterTheme));

    public bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
    }

    public bool IsPointAtCenter
    {
        get => GetValue(IsPointAtCenterProperty);
        set => SetValue(IsPointAtCenterProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="ControlTheme" /> that is applied to the container element generated for the flyout
    /// presenter.
    /// </summary>
    public ControlTheme? FlyoutPresenterTheme
    {
        get => GetValue(FlyoutPresenterThemeProperty);
        set => SetValue(FlyoutPresenterThemeProperty, value);
    }

    /// <summary>
    /// Gets or sets the content to display in this flyout
    /// </summary>
    [Content]
    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public BoxShadows MaskShadows
    {
        get => GetValue(MaskShadowsProperty);
        set => SetValue(MaskShadowsProperty, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<bool> IsShowArrowEffectiveProperty =
        AvaloniaProperty.Register<Flyout, bool>(nameof(IsShowArrowEffective));

    /// <summary>
    /// 是否实际显示箭头
    /// </summary>
    internal bool IsShowArrowEffective
    {
        get => GetValue(IsShowArrowEffectiveProperty);
        set => SetValue(IsShowArrowEffectiveProperty, value);
    }

    #endregion
}