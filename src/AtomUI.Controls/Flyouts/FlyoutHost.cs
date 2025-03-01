﻿using System.Reactive.Disposables;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace AtomUI.Controls;

using FlyoutControl = Flyout;

public enum FlyoutTriggerType
{
    Hover,
    Click
}

public class FlyoutHost : Control,
                          IAnimationAwareControl,
                          ITokenResourceConsumer
{
    #region 公共属性定义

    public static readonly StyledProperty<Control?> AnchorTargetProperty =
        AvaloniaProperty.Register<FlyoutHost, Control?>(nameof(AnchorTarget));

    /// <summary>
    /// Defines the <see cref="Flyout" /> property
    /// </summary>
    public static readonly StyledProperty<PopupFlyoutBase?> FlyoutProperty =
        AvaloniaProperty.Register<FlyoutHost, PopupFlyoutBase?>(nameof(Flyout));

    /// <summary>
    /// 触发方式
    /// </summary>
    public static readonly StyledProperty<FlyoutTriggerType> TriggerProperty =
        FlyoutStateHelper.TriggerTypeProperty.AddOwner<FlyoutHost>();

    /// <summary>
    /// 是否显示指示箭头
    /// </summary>
    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<FlyoutHost>();

    /// <summary>
    /// 箭头是否始终指向中心
    /// </summary>
    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        FlyoutControl.IsPointAtCenterProperty.AddOwner<FlyoutHost>();

    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        Popup.PlacementProperty.AddOwner<FlyoutHost>();

    public static readonly StyledProperty<PopupAnchor> PlacementAnchorProperty =
        Popup.PlacementAnchorProperty.AddOwner<FlyoutHost>();

    public static readonly StyledProperty<PopupGravity> PlacementGravityProperty =
        Popup.PlacementGravityProperty.AddOwner<FlyoutHost>();
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty
        = AnimationAwareControlProperty.IsMotionEnabledProperty.AddOwner<FlyoutHost>();

    public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
        = AnimationAwareControlProperty.IsWaveAnimationEnabledProperty.AddOwner<FlyoutHost>();

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

    /// <summary>
    /// 装饰的目标控件
    /// </summary>
    [Content]
    public Control? AnchorTarget
    {
        get => GetValue(AnchorTargetProperty);
        set => SetValue(AnchorTargetProperty, value);
    }

    public PopupFlyoutBase? Flyout
    {
        get => GetValue(FlyoutProperty);
        set => SetValue(FlyoutProperty, value);
    }

    public FlyoutTriggerType Trigger
    {
        get => GetValue(TriggerProperty);
        set => SetValue(TriggerProperty, value);
    }

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

    public bool IsWaveAnimationEnabled
    {
        get => GetValue(IsWaveAnimationEnabledProperty);
        set => SetValue(IsWaveAnimationEnabledProperty, value);
    }

    #endregion

    #region 内部属性定义

    Control IAnimationAwareControl.PropertyBindTarget => this;
    CompositeDisposable? ITokenResourceConsumer.TokenBindingsDisposable => _tokenBindingsDisposable;

    #endregion

    private CompositeDisposable? _tokenBindingsDisposable;
    private readonly FlyoutStateHelper _flyoutStateHelper;
    
    static FlyoutHost()
    {
        PlacementProperty.OverrideDefaultValue<FlyoutHost>(PlacementMode.Top);
    }
    
    public FlyoutHost()
    {
        _flyoutStateHelper = new FlyoutStateHelper();
        this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _tokenBindingsDisposable = new CompositeDisposable();
        this.AddTokenBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, MarginToAnchorProperty, SharedTokenKey.MarginXXS));
        
        BindUtils.RelayBind(this, AnchorTargetProperty, _flyoutStateHelper, FlyoutStateHelper.AnchorTargetProperty);
        BindUtils.RelayBind(this, FlyoutProperty, _flyoutStateHelper, FlyoutStateHelper.FlyoutProperty);
        BindUtils.RelayBind(this, MouseEnterDelayProperty, _flyoutStateHelper,
            FlyoutStateHelper.MouseEnterDelayProperty);
        BindUtils.RelayBind(this, MouseLeaveDelayProperty, _flyoutStateHelper,
            FlyoutStateHelper.MouseLeaveDelayProperty);
        BindUtils.RelayBind(this, TriggerProperty, _flyoutStateHelper, FlyoutStateHelper.TriggerTypeProperty);
        if (AnchorTarget is not null)
        {
            AnchorTarget.SetLogicalParent(this);
            VisualChildren.Add(AnchorTarget);
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _flyoutStateHelper.NotifyAttachedToVisualTree();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _flyoutStateHelper.NotifyDetachedFromVisualTree();
    }

    protected virtual void SetupFlyoutProperties()
    {
        if (Flyout is not null)
        {
            BindUtils.RelayBind(this, PlacementProperty, Flyout, FlyoutControl.PlacementProperty);
            BindUtils.RelayBind(this, PlacementAnchorProperty, Flyout, FlyoutControl.PlacementAnchorProperty);
            BindUtils.RelayBind(this, PlacementGravityProperty, Flyout, FlyoutControl.PlacementGravityProperty);
            BindUtils.RelayBind(this, IsShowArrowProperty, Flyout, FlyoutControl.IsShowArrowProperty);
            BindUtils.RelayBind(this, IsPointAtCenterProperty, Flyout, FlyoutControl.IsPointAtCenterProperty);
            BindUtils.RelayBind(this, MarginToAnchorProperty, Flyout, PopupFlyoutBase.MarginToAnchorProperty);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, Flyout, PopupFlyoutBase.IsMotionEnabledProperty);
            Flyout.IsDetectMouseClickEnabled = false;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == FlyoutProperty)
        {
            if (Flyout is not null)
            {
                SetupFlyoutProperties();
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