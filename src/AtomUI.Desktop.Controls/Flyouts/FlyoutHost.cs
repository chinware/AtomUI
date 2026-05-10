using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.MotionScene;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

using FlyoutControl = Flyout;

public enum FlyoutTriggerType
{
    Hover,
    Click,
    Focus,
}

public class FlyoutHost : ContentControl, IMotionAwareControl
{
    #region 公共属性定义
    public static readonly StyledProperty<BoxShadows> PopupRootShadowProperty =
        Popup.PopupRootShadowProperty.AddOwner<FlyoutHost>();
    
    public static readonly StyledProperty<BoxShadows> OverlayHostShadowProperty =
        Popup.OverlayHostShadowProperty.AddOwner<FlyoutHost>();
    
    public static readonly StyledProperty<Flyout?> FlyoutProperty =
        AvaloniaProperty.Register<FlyoutHost, Flyout?>(nameof(Flyout));

    public static readonly StyledProperty<FlyoutTriggerType> TriggerProperty =
        FlyoutStateHelper.TriggerTypeProperty.AddOwner<FlyoutHost>();
    
    public static readonly StyledProperty<bool> IsArrowVisibleProperty =
        ArrowDecoratedBox.IsArrowVisibleProperty.AddOwner<FlyoutHost>();
    
    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        FlyoutControl.IsPointAtCenterProperty.AddOwner<FlyoutHost>();

    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        Popup.PlacementProperty.AddOwner<FlyoutHost>();

    public static readonly StyledProperty<PopupAnchor> PlacementAnchorProperty =
        Popup.PlacementAnchorProperty.AddOwner<FlyoutHost>();

    public static readonly StyledProperty<PopupGravity> PlacementGravityProperty =
        Popup.PlacementGravityProperty.AddOwner<FlyoutHost>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<FlyoutHost>();
    
    public static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<FlyoutHost>();
    
    public static readonly StyledProperty<AbstractMotion?> OpenMotionProperty = 
        Popup.OpenMotionProperty.AddOwner<FlyoutHost>();
        
    public static readonly StyledProperty<AbstractMotion?> CloseMotionProperty = 
        Popup.CloseMotionProperty.AddOwner<FlyoutHost>();
    
    public static readonly StyledProperty<bool> ShouldUseOverlayPopupProperty =
        Flyout.ShouldUseOverlayPopupProperty.AddOwner<FlyoutHost>();

    /// <summary>
    /// 距离 anchor 的边距，根据垂直和水平进行设置
    /// 但是对某些组合无效，比如跟随鼠标的情况
    /// 还有些 anchor 和 gravity 的组合也没有用
    /// </summary>
    public static readonly StyledProperty<double> MarginToAnchorProperty =
        Popup.MarginToAnchorProperty.AddOwner<FlyoutHost>();

    public static readonly StyledProperty<int> MouseEnterDelayProperty =
        FlyoutStateHelper.MouseEnterDelayProperty.AddOwner<FlyoutHost>();

    public static readonly StyledProperty<int> MouseLeaveDelayProperty =
        FlyoutStateHelper.MouseLeaveDelayProperty.AddOwner<FlyoutHost>();
    
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

    public Flyout? Flyout
    {
        get => GetValue(FlyoutProperty);
        set => SetValue(FlyoutProperty, value);
    }

    public FlyoutTriggerType Trigger
    {
        get => GetValue(TriggerProperty);
        set => SetValue(TriggerProperty, value);
    }

    public bool IsArrowVisible
    {
        get => GetValue(IsArrowVisibleProperty);
        set => SetValue(IsArrowVisibleProperty, value);
    }

    public bool IsPointAtCenter
    {
        get => GetValue(IsPointAtCenterProperty);
        set => SetValue(IsPointAtCenterProperty, value);
    }

    public PlacementMode Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    public PopupGravity PlacementGravity
    {
        get => GetValue(PlacementGravityProperty);
        set => SetValue(PlacementGravityProperty, value);
    }

    public PopupAnchor PlacementAnchor
    {
        get => GetValue(PlacementAnchorProperty);
        set => SetValue(PlacementAnchorProperty, value);
    }

    public double MarginToAnchor
    {
        get => GetValue(MarginToAnchorProperty);
        set => SetValue(MarginToAnchorProperty, value);
    }

    public int MouseEnterDelay
    {
        get => GetValue(MouseEnterDelayProperty);
        set => SetValue(MouseEnterDelayProperty, value);
    }

    public int MouseLeaveDelay
    {
        get => GetValue(MouseLeaveDelayProperty);
        set => SetValue(MouseLeaveDelayProperty, value);
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

    public bool ShouldUseOverlayPopup
    {
        get => GetValue(ShouldUseOverlayPopupProperty);
        set => SetValue(ShouldUseOverlayPopupProperty, value);
    }
    #endregion
    
    private readonly FlyoutStateHelper _flyoutStateHelper;
    private CompositeDisposable? _flyoutDisposables;
    
    static FlyoutHost()
    {
        PlacementProperty.OverrideDefaultValue<FlyoutHost>(PlacementMode.Top);
    }
    
    public FlyoutHost()
    {
        this.RegisterTokenResourceScope(FlyoutHostToken.ScopeProvider);
        _flyoutStateHelper                                             = new FlyoutStateHelper();
        _flyoutStateHelper[!FlyoutStateHelper.AnchorTargetProperty]    = this[!ContentProperty];
        _flyoutStateHelper[!FlyoutStateHelper.FlyoutProperty]          = this[!FlyoutProperty];
        _flyoutStateHelper[!FlyoutStateHelper.MouseEnterDelayProperty] = this[!MouseEnterDelayProperty];
        _flyoutStateHelper[!FlyoutStateHelper.MouseLeaveDelayProperty] = this[!MouseLeaveDelayProperty];
        _flyoutStateHelper[!FlyoutStateHelper.TriggerTypeProperty]     = this[!TriggerProperty];
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        SetupFlyoutProperties();
        _flyoutStateHelper.NotifyAttachedToVisualTree();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _flyoutStateHelper.NotifyDetachedFromVisualTree();
        _flyoutDisposables?.Dispose();
        _flyoutDisposables = null;
    }

    protected virtual void SetupFlyoutProperties()
    {
        if (Flyout is not null)
        {
            _flyoutDisposables?.Dispose();
            _flyoutDisposables = new CompositeDisposable();
            _flyoutDisposables.Add(BindUtils.RelayBind(this, PlacementProperty, Flyout, FlyoutControl.RequestedPlacementProperty, (PlacementMode v) => (PlacementMode?)v));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, PlacementAnchorProperty, Flyout, FlyoutControl.PlacementAnchorProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, PlacementGravityProperty, Flyout, FlyoutControl.PlacementGravityProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, IsArrowVisibleProperty, Flyout, FlyoutControl.IsArrowVisibleProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, IsPointAtCenterProperty, Flyout, FlyoutControl.IsPointAtCenterProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, MarginToAnchorProperty, Flyout, FlyoutControl.MarginToAnchorProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, Flyout, FlyoutControl.IsMotionEnabledProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, MotionDurationProperty, Flyout, FlyoutControl.MotionDurationProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, OpenMotionProperty, Flyout, FlyoutControl.OpenMotionProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, CloseMotionProperty, Flyout, FlyoutControl.CloseMotionProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, PopupRootShadowProperty, Flyout, FlyoutControl.PopupRootShadowProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, OverlayHostShadowProperty, Flyout, FlyoutControl.OverlayHostShadowProperty));
            _flyoutDisposables.Add(BindUtils.RelayBind(this, ShouldUseOverlayPopupProperty, Flyout, FlyoutControl.ShouldUseOverlayPopupProperty));
            ConfigureMotion(Placement);
        }
    }

    private void ConfigureMotion(PlacementMode placement)
    {
        (OpenMotion, CloseMotion) = PopupUtils.CreateMotionForPlacement(placement);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (this.IsAttachedToLogicalTree())
        {
            if (change.Property == FlyoutProperty)
            {
                if (Flyout is not null)
                {
                    SetupFlyoutProperties();
                }
            }
            else if (change.Property == PlacementProperty)
            {
                ConfigureMotion(Placement);
            }
        }
    }

    public void ShowFlyout(bool immediately)
    {
        _flyoutStateHelper.ShowFlyout(immediately);
    }

    public void HideFlyout(bool immediately)
    {
        _flyoutStateHelper.HideFlyout(immediately);
    }
}