using System.ComponentModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.MotionScene;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

using PopupControl = Popup;
using AvaloniaPopup = Avalonia.Controls.Primitives.Popup;

public class Flyout : PopupFlyoutBase, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<BoxShadows> PopupRootShadowProperty =
        PopupControl.PopupRootShadowProperty.AddOwner<Flyout>();
    
    public static readonly StyledProperty<BoxShadows> OverlayHostShadowProperty =
        PopupControl.OverlayHostShadowProperty.AddOwner<Flyout>();
    
    /// <summary>
    /// 是否显示指示箭头
    /// </summary>
    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<Flyout>();
    
    public static readonly StyledProperty<double> MarginToAnchorProperty =
        PopupControl.MarginToAnchorProperty.AddOwner<Flyout>();

    /// <summary>
    /// 箭头是否始终指向中心
    /// </summary>
    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        AvaloniaProperty.Register<Flyout, bool>(nameof(IsPointAtCenter));
    
    public static readonly StyledProperty<object> ContentProperty =
        AvaloniaProperty.Register<Flyout, object>(nameof(Content));
    
    public static readonly StyledProperty<AbstractMotion?> OpenMotionProperty = 
        PopupControl.OpenMotionProperty.AddOwner<Flyout>();
        
    public static readonly StyledProperty<AbstractMotion?> CloseMotionProperty = 
        PopupControl.CloseMotionProperty.AddOwner<Flyout>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<Flyout>();
    
    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<Flyout>();
    
    public static readonly StyledProperty<bool> ShouldUseOverlayLayerProperty =
        PopupControl.ShouldUseOverlayLayerProperty.AddOwner<Flyout>();
    
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

    public bool IsShowArrow
    {
        get => GetValue(IsShowArrowProperty);
        set => SetValue(IsShowArrowProperty, value);
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
    
    [Content]
    public object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
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

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }

    public TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }
    
    public bool ShouldUseOverlayLayer
    {
        get => GetValue(ShouldUseOverlayLayerProperty);
        set => SetValue(ShouldUseOverlayLayerProperty, value);
    }

    #endregion

    #region 内部属性定义
    
    internal static readonly DirectProperty<Flyout, bool> IsShowArrowEffectiveProperty =
        AvaloniaProperty.RegisterDirect<Flyout, bool>(nameof(IsShowArrowEffective),
            o => o.IsShowArrowEffective,
            (o, v) => o.IsShowArrowEffective = v);
    
    internal static readonly DirectProperty<Flyout, bool> IsPopupFlippedProperty =
        AvaloniaProperty.RegisterDirect<Flyout, bool>(nameof(IsPopupFlipped),
            o => o.IsPopupFlipped,
            (o, v) => o.IsPopupFlipped = v);
    
    internal static readonly StyledProperty<ArrowPosition> ArrowPositionProperty =
        ArrowDecoratedBox.ArrowPositionProperty.AddOwner<Flyout>();
    
    private bool _isShowArrowEffective;

    internal bool IsShowArrowEffective
    {
        get => _isShowArrowEffective;
        private set => SetAndRaise(IsShowArrowEffectiveProperty, ref _isShowArrowEffective, value);
    }
    
    private bool _isPopupFlipped;

    internal bool IsPopupFlipped
    {
        get => _isPopupFlipped;
        private set => SetAndRaise(IsPopupFlippedProperty, ref _isPopupFlipped, value);
    }
    
    internal ArrowPosition ArrowPosition
    {
        get => GetValue(ArrowPositionProperty);
        set => SetValue(ArrowPositionProperty, value);
    }

    #endregion

    static Flyout()
    {
        IsShowArrowProperty.OverrideDefaultValue<Flyout>(false);
    }

    public Flyout()
    {
        TokenResourceBinder.CreateTokenBinding(this, PopupRootShadowProperty, FlyoutHostTokenKind.PopupRootShadow);
        TokenResourceBinder.CreateTokenBinding(this, OverlayHostShadowProperty, FlyoutHostTokenKind.OverlayHostShadow);
        TokenResourceBinder.CreateTokenBinding(this, MotionDurationProperty, SharedTokenKind.MotionDurationMid);
        this.SetPopupLazy(new Lazy<AvaloniaPopup>(CreatePopup));
    }
    
    private Popup CreatePopup()
    {
        var popup = new PopupControl
        {
            WindowManagerAddShadowHint = false,
            IsLightDismissEnabled      = false,
        };

        popup[!PopupControl.RequestedPlacementProperty]    = this[!PlacementProperty];
        popup[!PopupControl.PlacementAnchorProperty]       = this[!PlacementAnchorProperty];
        popup[!PopupControl.PlacementGravityProperty]      = this[!PlacementGravityProperty];
        popup[!PopupControl.PopupRootShadowProperty]       = this[!PopupRootShadowProperty];
        popup[!PopupControl.OverlayHostShadowProperty]     = this[!OverlayHostShadowProperty];
        popup[!PopupControl.MotionDurationProperty]        = this[!MotionDurationProperty];
        popup[!PopupControl.OpenMotionProperty]            = this[!OpenMotionProperty];
        popup[!PopupControl.CloseMotionProperty]           = this[!CloseMotionProperty];
        popup[!PopupControl.IsMotionEnabledProperty]       = this[!IsMotionEnabledProperty];
        popup[!PopupControl.MarginToAnchorProperty]        = this[!MarginToAnchorProperty];
        popup[!PopupControl.ShouldUseOverlayLayerProperty] = this[!ShouldUseOverlayLayerProperty];
        this[!IsPopupFlippedProperty]                      = popup[!PopupControl.IsFlippedProperty];

        popup.Opened += this.OnPopupOpened;
        popup.Closed += this.OnPopupClosed;
        popup.AddClosingEventHandler(HandlePopupClosing);
        popup.KeyUp += this.OnPlacementTargetOrPopupKeyUp;
        return popup;
    }

    private void HandlePopupClosing(object? sender, CancelEventArgs e)
    {
        if (!e.Cancel)
        {
            this.OnPopupClosing(sender, e);
        }
    }

    protected override bool HideCore(bool canCancel = true)
    {
        if (canCancel && IsMotionEnabled && CloseMotion is not null && Popup.IsOpen)
        {
            Popup.IsOpen = false;
            return true;
        }

        return base.HideCore(canCancel);
    }

    protected override Control CreatePresenter()
    {
        var presenter = new FlyoutPresenter();
        presenter[!FlyoutPresenter.ContentProperty]         = this[!ContentProperty];
        presenter[!FlyoutPresenter.IsMotionEnabledProperty] = this[!IsMotionEnabledProperty];
        presenter[!FlyoutPresenter.IsShowArrowProperty]     = this[!IsShowArrowEffectiveProperty];
        presenter[!FlyoutPresenter.ArrowPositionProperty]   = this[!ArrowPositionProperty];
        ConfigureShowArrowEffective();
        ConfigureArrowPosition();
        return presenter;
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsShowArrowProperty ||
            change.Property == PlacementProperty ||
            change.Property == PlacementAnchorProperty ||
            change.Property == PlacementGravityProperty)
        {
            ConfigureShowArrowEffective();
        }

        if (change.Property == PlacementProperty ||
            change.Property == IsPopupFlippedProperty)
        {
            ConfigureArrowPosition();
        }
    }

    protected void ConfigureShowArrowEffective()
    {
        if (!IsShowArrow)
        {
            SetCurrentValue(IsShowArrowEffectiveProperty, false);
        }
        else
        {
            SetCurrentValue(IsShowArrowEffectiveProperty, PopupUtils.CanEnabledArrow(Placement, PlacementAnchor, PlacementGravity));
        }
    }

    protected void ConfigureArrowPosition()
    {
        var arrowPosition = PopupUtils.CalculateArrowPosition(Placement, PlacementAnchor, PlacementGravity);
        if (arrowPosition.HasValue)
        {
            if (IsPopupFlipped)
            {
                arrowPosition = ArrowPositionUtils.FlipArrowPosition(arrowPosition.Value);
            }
            SetCurrentValue(ArrowPositionProperty, arrowPosition);
        }
    }
}