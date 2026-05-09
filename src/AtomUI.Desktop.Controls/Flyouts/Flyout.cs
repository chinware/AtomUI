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
using Avalonia.Data;
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
    /// 如果要启用 AtomUI 的自定义定位功能（MarginToAnchor、箭头、翻转等），
    /// 请使用该属性而不是 <see cref="PopupFlyoutBase.Placement"/>。
    /// 为 null 时 Flyout 走 Avalonia 默认定位逻辑。
    /// </summary>
    public static readonly StyledProperty<PlacementMode?> RequestedPlacementProperty =
        PopupControl.RequestedPlacementProperty.AddOwner<Flyout>();

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
    
    public static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
        AvaloniaProperty.Register<Flyout, bool>(nameof(ShouldUseOverlayPopup), true);

    public static readonly StyledProperty<bool> IsLightDismissEnabledProperty =
        AvaloniaPopup.IsLightDismissEnabledProperty.AddOwner<Flyout>();

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

    public PlacementMode? RequestedPlacement
    {
        get => GetValue(RequestedPlacementProperty);
        set => SetValue(RequestedPlacementProperty, value);
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
    
    public bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }

    public bool IsLightDismissEnabled
    {
        get => GetValue(IsLightDismissEnabledProperty);
        set => SetValue(IsLightDismissEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义
    
    internal static readonly DirectProperty<Flyout, bool> IsShowArrowEffectiveProperty =
        AvaloniaProperty.RegisterDirect<Flyout, bool>(nameof(IsShowArrowEffective),
            o => o.IsShowArrowEffective,
            (o, v) => o.IsShowArrowEffective = v);
    
    internal static readonly DirectProperty<Flyout, bool> IsPopupHorizontalFlippedProperty =
        AvaloniaProperty.RegisterDirect<Flyout, bool>(nameof(IsPopupHorizontalFlipped),
            o => o.IsPopupHorizontalFlipped,
            (o, v) => o.IsPopupHorizontalFlipped = v);
    
    internal static readonly DirectProperty<Flyout, bool> IsPopupVerticalFlippedProperty =
        AvaloniaProperty.RegisterDirect<Flyout, bool>(nameof(IsPopupVerticalFlipped),
            o => o.IsPopupVerticalFlipped,
            (o, v) => o.IsPopupVerticalFlipped = v);
    
    internal static readonly StyledProperty<ArrowPosition> ArrowPositionProperty =
        ArrowDecoratedBox.ArrowPositionProperty.AddOwner<Flyout>();
    
    private bool _isShowArrowEffective;

    internal bool IsShowArrowEffective
    {
        get => _isShowArrowEffective;
        private set => SetAndRaise(IsShowArrowEffectiveProperty, ref _isShowArrowEffective, value);
    }
    
    private bool _isPopupHorizontalFlipped;

    internal bool IsPopupHorizontalFlipped
    {
        get => _isPopupHorizontalFlipped;
        private set => SetAndRaise(IsPopupHorizontalFlippedProperty, ref _isPopupHorizontalFlipped, value);
    }
    
    private bool _isPopupVerticalFlipped;

    internal bool IsPopupVerticalFlipped
    {
        get => _isPopupVerticalFlipped;
        private set => SetAndRaise(IsPopupVerticalFlippedProperty, ref _isPopupVerticalFlipped, value);
    }
    
    internal ArrowPosition ArrowPosition
    {
        get => GetValue(ArrowPositionProperty);
        set => SetValue(ArrowPositionProperty, value);
    }

    #endregion
    
    private object? _pointerHorizontalOffsetTokenKey;
    private object? _pointerVerticalOffsetTokenKey;
    private IDisposable? _pointerHorizontalOffsetBinding;
    private IDisposable? _pointerVerticalOffsetBinding;

    static Flyout()
    {
        IsShowArrowProperty.OverrideDefaultValue<Flyout>(false);
        IsLightDismissEnabledProperty.OverrideDefaultValue<Flyout>(true);
    }

    public Flyout()
    {
        TokenResourceBinder.CreateGlobalTokenBinding(this, PopupRootShadowProperty, FlyoutHostTokenKind.PopupRootShadow);
        TokenResourceBinder.CreateGlobalTokenBinding(this, OverlayHostShadowProperty, FlyoutHostTokenKind.OverlayHostShadow);
        TokenResourceBinder.CreateGlobalTokenBinding(this, MotionDurationProperty, SharedTokenKind.MotionDurationMid);
        this.SetPopupLazy(new Lazy<AvaloniaPopup>(CreatePopup));
    }
    
    private Popup CreatePopup()
    {
        var popup = new PopupControl
        {
            WindowManagerAddShadowHint = false,
        };

        popup[!PopupControl.RequestedPlacementProperty]     = this[!RequestedPlacementProperty];
        popup[!PopupControl.PlacementAnchorProperty]        = this[!PlacementAnchorProperty];
        popup[!PopupControl.PlacementGravityProperty]       = this[!PlacementGravityProperty];
        popup[!PopupControl.PopupRootShadowProperty]        = this[!PopupRootShadowProperty];
        popup[!PopupControl.OverlayHostShadowProperty]      = this[!OverlayHostShadowProperty];
        popup[!PopupControl.MotionDurationProperty]         = this[!MotionDurationProperty];
        popup[!PopupControl.OpenMotionProperty]             = this[!OpenMotionProperty];
        popup[!PopupControl.CloseMotionProperty]            = this[!CloseMotionProperty];
        popup[!PopupControl.IsMotionEnabledProperty]        = this[!IsMotionEnabledProperty];
        popup[!PopupControl.MarginToAnchorProperty]         = this[!MarginToAnchorProperty];
        popup[!PopupControl.IsPointAtCenterProperty]        = this[!IsPointAtCenterProperty];
        popup[!PopupControl.ShouldUseOverlayLayerProperty]  = this[!ShouldUseOverlayPopupProperty];
        popup[!AvaloniaPopup.IsLightDismissEnabledProperty] = this[!IsLightDismissEnabledProperty];
        this[!IsPopupHorizontalFlippedProperty]             = popup[!PopupControl.IsHorizontalFlippedProperty];
        this[!IsPopupVerticalFlippedProperty]               = popup[!PopupControl.IsVerticalFlippedProperty];

        popup.Opened += this.OnPopupOpened;
        popup.Closed += this.OnPopupClosed;
        popup.AddClosingEventHandler(HandlePopupClosing);
        popup.KeyUp += this.OnPlacementTargetOrPopupKeyUp;
        popup.PropertyChanged += HandlePopupPropertyChanged;
        return popup;
    }

    /// <remarks>
    /// <see cref="PopupFlyoutBase.ShowAtCore"/> 内部的 <c>PositionPopup</c> 会在每次打开时把
    /// <see cref="AvaloniaPopup.Placement"/> 和 <see cref="AvaloniaPopup.CustomPopupPlacementCallback"/>
    /// 强制覆盖为 Flyout 侧的值，导致 AtomUI 自定义定位链路失效。这里在 <see cref="RequestedPlacement"/>
    /// 非空时把 Popup 的相关属性重新拉回 Custom 模式。
    /// </remarks>
    private void HandlePopupPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is not PopupControl popup || !RequestedPlacement.HasValue)
        {
            return;
        }

        if (e.Property == AvaloniaPopup.PlacementProperty)
        {
            if (e.GetNewValue<PlacementMode>() != PlacementMode.Custom)
            {
                popup.Placement = PlacementMode.Custom;
            }
        }
        else if (e.Property == AvaloniaPopup.CustomPopupPlacementCallbackProperty)
        {
            if (e.GetNewValue<CustomPopupPlacementCallback?>() != popup.HandleCustomPlacement)
            {
                popup.CustomPopupPlacementCallback = popup.HandleCustomPlacement;
            }
        }
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsShowArrowProperty ||
            change.Property == PlacementProperty ||
            change.Property == RequestedPlacementProperty ||
            change.Property == PlacementAnchorProperty ||
            change.Property == PlacementGravityProperty)
        {
            ConfigureShowArrowEffective();
        }

        if (change.Property == PlacementProperty ||
            change.Property == RequestedPlacementProperty ||
            change.Property == IsPopupHorizontalFlippedProperty ||
            change.Property == IsPopupVerticalFlippedProperty)
        {
            ConfigureArrowPosition();
        }

        if (change.Property == RequestedPlacementProperty)
        {
            ConfigurePointerPlacementOffsets();
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
            var placement = RequestedPlacement ?? Placement;
            SetCurrentValue(IsShowArrowEffectiveProperty, PopupUtils.CanEnabledArrow(placement, PlacementAnchor, PlacementGravity));
        }
    }

    protected void ConfigureArrowPosition()
    {
        var placement     = RequestedPlacement ?? Placement;
        var arrowPosition = PopupUtils.CalculateArrowPosition(placement, PlacementAnchor, PlacementGravity);
        if (arrowPosition.HasValue)
        {
            arrowPosition = ArrowPositionUtils.FlipArrowPosition(arrowPosition.Value, IsPopupHorizontalFlipped, IsPopupVerticalFlipped);
            SetCurrentValue(ArrowPositionProperty, arrowPosition);
        }
    }

    /// <summary>
    /// Bind <see cref="PopupFlyoutBase.HorizontalOffsetProperty"/> / <see cref="PopupFlyoutBase.VerticalOffsetProperty"/>
    /// to the given tokens only while <see cref="RequestedPlacement"/> is <see cref="PlacementMode.Pointer"/>.
    /// For non-pointer placements the offsets stay at their default values so edge-aligned placements
    /// (e.g. <c>BottomEdgeAlignedRight</c>) aren't shifted off anchor.
    /// </summary>
    protected void BindPointerPlacementOffsets(object horizontalOffsetTokenKey, object verticalOffsetTokenKey)
    {
        _pointerHorizontalOffsetTokenKey = horizontalOffsetTokenKey;
        _pointerVerticalOffsetTokenKey   = verticalOffsetTokenKey;
        ConfigurePointerPlacementOffsets();
    }

    private void ConfigurePointerPlacementOffsets()
    {
        _pointerHorizontalOffsetBinding?.Dispose();
        _pointerVerticalOffsetBinding?.Dispose();
        _pointerHorizontalOffsetBinding = null;
        _pointerVerticalOffsetBinding   = null;

        if (RequestedPlacement == PlacementMode.Pointer &&
            _pointerHorizontalOffsetTokenKey != null &&
            _pointerVerticalOffsetTokenKey != null)
        {
            _pointerHorizontalOffsetBinding = TokenResourceBinder.CreateGlobalResourceBinding(
                this, HorizontalOffsetProperty, _pointerHorizontalOffsetTokenKey, BindingPriority.Style);
            _pointerVerticalOffsetBinding = TokenResourceBinder.CreateGlobalResourceBinding(
                this, VerticalOffsetProperty, _pointerVerticalOffsetTokenKey, BindingPriority.Style);
        }
    }
}