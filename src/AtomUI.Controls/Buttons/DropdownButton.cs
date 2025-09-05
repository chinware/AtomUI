﻿using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Diagnostics;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Primitives.PopupPositioning;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Styling;

namespace AtomUI.Controls;

public class DropdownButton : Button
{
    #region 公共属性定义

    public static readonly StyledProperty<MenuFlyout?> DropdownFlyoutProperty =
        AvaloniaProperty.Register<DropdownButton, MenuFlyout?>(nameof(DropdownFlyout));

    public static readonly StyledProperty<FlyoutTriggerType> TriggerTypeProperty =
        FlyoutStateHelper.TriggerTypeProperty.AddOwner<DropdownButton>();

    public static readonly StyledProperty<bool> IsShowArrowProperty =
        ArrowDecoratedBox.IsShowArrowProperty.AddOwner<DropdownButton>();

    public static readonly StyledProperty<bool> IsPointAtCenterProperty =
        AtomUI.Controls.Flyout.IsPointAtCenterProperty.AddOwner<DropdownButton>();

    public static readonly StyledProperty<PlacementMode> PlacementProperty =
        Avalonia.Controls.Primitives.Popup.PlacementProperty.AddOwner<DropdownButton>();

    public static readonly StyledProperty<PopupAnchor> PlacementAnchorProperty =
        Avalonia.Controls.Primitives.Popup.PlacementAnchorProperty.AddOwner<DropdownButton>();

    public static readonly StyledProperty<PopupGravity> PlacementGravityProperty =
        Avalonia.Controls.Primitives.Popup.PlacementGravityProperty.AddOwner<DropdownButton>();

    public static readonly StyledProperty<double> MarginToAnchorProperty =
        Popup.MarginToAnchorProperty.AddOwner<DropdownButton>();

    public static readonly StyledProperty<int> MouseEnterDelayProperty =
        FlyoutStateHelper.MouseEnterDelayProperty.AddOwner<DropdownButton>();

    public static readonly StyledProperty<int> MouseLeaveDelayProperty =
        FlyoutStateHelper.MouseLeaveDelayProperty.AddOwner<DropdownButton>();

    public static readonly StyledProperty<bool> IsShowIndicatorProperty =
        AvaloniaProperty.Register<DropdownButton, bool>(nameof(IsShowIndicator), true);

    public static readonly RoutedEvent<FlyoutMenuItemClickedEventArgs> MenuItemClickedEvent =
        RoutedEvent.Register<DropdownButton, FlyoutMenuItemClickedEventArgs>(
            nameof(MenuItemClicked),
            RoutingStrategies.Bubble);

    public MenuFlyout? DropdownFlyout
    {
        get => GetValue(DropdownFlyoutProperty);
        set => SetValue(DropdownFlyoutProperty, value);
    }

    public FlyoutTriggerType TriggerType
    {
        get => GetValue(TriggerTypeProperty);
        set => SetValue(TriggerTypeProperty, value);
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

    public bool IsShowIndicator
    {
        get => GetValue(IsShowIndicatorProperty);
        set => SetValue(IsShowIndicatorProperty, value);
    }

    public event EventHandler<FlyoutMenuItemClickedEventArgs>? MenuItemClicked
    {
        add => AddHandler(MenuItemClickedEvent, value);
        remove => RemoveHandler(MenuItemClickedEvent, value);
    }

    #endregion
    
    private MenuFlyoutPresenter? _menuFlyoutPresenter;
    private readonly FlyoutStateHelper _flyoutStateHelper;
    private CompositeDisposable? _flyoutHelperBindingDisposables;
    private CompositeDisposable? _flyoutBindingDisposables;
    
    static DropdownButton()
    {
        PlacementProperty.OverrideDefaultValue<DropdownButton>(PlacementMode.BottomEdgeAlignedLeft);
        IsShowArrowProperty.OverrideDefaultValue<DropdownButton>(false);
    }

    public DropdownButton()
    {
        _flyoutStateHelper = new FlyoutStateHelper
        {
            AnchorTarget = this
        };
        _flyoutStateHelper.ClickHideFlyoutPredicate = ClickHideFlyoutPredicate;
    }
    
    protected override void ConfigureControlThemeBindings(bool force)
    {
        if (!ThemeConfigured || force)
        {
            string? resourceKey = null;
            if (ButtonType == ButtonType.Default)
            {
                resourceKey = DefaultDropdownButtonTheme.ID;
            }
            else if (ButtonType == ButtonType.Primary)
            {
                resourceKey = PrimaryDropdownButtonTheme.ID;
            }
            else if (ButtonType == ButtonType.Text)
            {
                resourceKey = TextDropdownButtonTheme.ID;
            }
            else if (ButtonType == ButtonType.Link)
            {
                resourceKey = LinkDropdownButtonTheme.ID;
            }

            resourceKey ??= DefaultDropdownButtonTheme.ID;

            if (AtomApplication.Current != null)
            {
                if (AtomApplication.Current.TryFindResource(resourceKey, out var resource))
                {
                    if (resource is ControlTheme theme)
                    {
                        Theme = theme;
                    }
                }
            }
        }
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        _flyoutHelperBindingDisposables?.Dispose();
        _flyoutHelperBindingDisposables = new CompositeDisposable(4);
        _flyoutHelperBindingDisposables.Add(BindUtils.RelayBind(this, DropdownFlyoutProperty, _flyoutStateHelper, FlyoutStateHelper.FlyoutProperty));
        _flyoutHelperBindingDisposables.Add(BindUtils.RelayBind(this, MouseEnterDelayProperty, _flyoutStateHelper,
            FlyoutStateHelper.MouseEnterDelayProperty));
        _flyoutHelperBindingDisposables.Add(BindUtils.RelayBind(this, MouseLeaveDelayProperty, _flyoutStateHelper,
            FlyoutStateHelper.MouseLeaveDelayProperty));
        _flyoutHelperBindingDisposables.Add(BindUtils.RelayBind(this, TriggerTypeProperty, _flyoutStateHelper, FlyoutStateHelper.TriggerTypeProperty));
        SetupFlyoutProperties();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        _flyoutHelperBindingDisposables?.Dispose();
        _flyoutBindingDisposables?.Dispose();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _flyoutStateHelper.NotifyDetachedFromVisualTree();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _flyoutStateHelper.NotifyAttachedToVisualTree();
    }

    private void SetupFlyoutProperties()
    {
        if (DropdownFlyout is not null)
        {
            _flyoutBindingDisposables?.Dispose();
            _flyoutBindingDisposables = new CompositeDisposable(7);
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, PlacementProperty, DropdownFlyout));
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, PlacementAnchorProperty, DropdownFlyout));
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, PlacementGravityProperty, DropdownFlyout));
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsShowArrowProperty, DropdownFlyout));
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsPointAtCenterProperty, DropdownFlyout));
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, MarginToAnchorProperty, DropdownFlyout));
            _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, DropdownFlyout));
        }
    }
    
    private bool ClickHideFlyoutPredicate(IPopupHostProvider hostProvider, RawPointerEventArgs args)
    {
        if (hostProvider.PopupHost != args.Root)
        {
            if (TriggerType == FlyoutTriggerType.Click)
            {
                return true;
            }
            // 只有 TriggerType 为 Hover 的时候会判断
            var secondaryButtonOrigin = this.TranslatePoint(new Point(0, 0), TopLevel.GetTopLevel(this)!);
            var secondaryBounds = secondaryButtonOrigin.HasValue ? new Rect(secondaryButtonOrigin.Value, Bounds.Size) : new Rect();
            if (!secondaryBounds.Contains(args.Position))
            {
                return true;
            }
        }
        return false;
    }

    private void HandleFlyoutOpened(object? sender, EventArgs e)
    {
        if (DropdownFlyout is IPopupHostProvider popupHostProvider)
        {
            var host = popupHostProvider.PopupHost;
            if (host is PopupRoot popupRoot)
            {
                if (popupRoot.Parent is Popup popup)
                {
                    if (popup.Child is MenuFlyoutPresenter menuFlyoutPresenter)
                    {
                        _menuFlyoutPresenter                =  menuFlyoutPresenter;
                        menuFlyoutPresenter.MenuItemClicked += HandleMenuItemClicked;
                    }
                }
            }
        }
    }

    private void HandleFlyoutClosed(object? sender, EventArgs e)
    {
        if (_menuFlyoutPresenter is not null)
        {
            _menuFlyoutPresenter.MenuItemClicked -= HandleMenuItemClicked;
            _menuFlyoutPresenter                 =  null;
        }
    }

    private void HandleMenuItemClicked(object? sender, FlyoutMenuItemClickedEventArgs args)
    {
        var eventArgs = new FlyoutMenuItemClickedEventArgs(MenuItemClickedEvent, args.Item);
        RaiseEvent(eventArgs);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        RightExtraContent = new Border();
        if (DropdownFlyout is not null)
        {
            DropdownFlyout.Opened                    += HandleFlyoutOpened;
            DropdownFlyout.Closed                    += HandleFlyoutClosed;
            DropdownFlyout.IsDetectMouseClickEnabled =  false;
        }
    }
}