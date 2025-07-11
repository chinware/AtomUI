﻿using System.Reactive.Disposables;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class NavMenuItemClickEventArgs : RoutedEventArgs
{
    public NavMenuItemClickEventArgs(RoutedEvent routedEvent, INavMenuItem navMenuItem)
        : base(routedEvent)
    {
        NavMenuItem = navMenuItem;
    }

    public INavMenuItem NavMenuItem { get; }
}

[PseudoClasses(InlineModePC, HorizontalModePC, VerticalModePC)]
public class NavMenu : NavMenuBase,
                       IResourceBindingManager
{
    public const string InlineModePC = ":inline-mode";
    public const string HorizontalModePC = ":horizontal-mode";
    public const string VerticalModePC = ":vertical-mode";
    public const string DarkStylePC = ":dark";
    public const string LightStylePC = ":light";

    #region 公共属性定义

    public static readonly StyledProperty<NavMenuMode> ModeProperty =
        AvaloniaProperty.Register<NavMenu, NavMenuMode>(nameof(Mode), NavMenuMode.Horizontal);

    public static readonly StyledProperty<bool> IsDarkStyleProperty =
        AvaloniaProperty.Register<NavMenu, bool>(nameof(IsDarkStyle), false);

    public static readonly StyledProperty<double> ActiveBarWidthProperty =
        AvaloniaProperty.Register<NavMenu, double>(nameof(ActiveBarWidth), 1.0d,
            coerce: (o, v) => Math.Max(Math.Min(v, 1.0), 0.0));

    public static readonly StyledProperty<double> ActiveBarHeightProperty =
        AvaloniaProperty.Register<NavMenu, double>(nameof(ActiveBarHeight));

    public NavMenuMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public bool IsDarkStyle
    {
        get => GetValue(IsDarkStyleProperty);
        set => SetValue(IsDarkStyleProperty, value);
    }

    public double ActiveBarWidth
    {
        get => GetValue(ActiveBarWidthProperty);
        set => SetValue(ActiveBarWidthProperty, value);
    }

    public double ActiveBarHeight
    {
        get => GetValue(ActiveBarHeightProperty);
        set => SetValue(ActiveBarHeightProperty, value);
    }

    #endregion

    #region 公共事件定义

    public static readonly RoutedEvent<NavMenuItemClickEventArgs> NavMenuItemClickEvent =
        RoutedEvent.Register<NavMenu, NavMenuItemClickEventArgs>(
            nameof(NavMenuItemClick),
            RoutingStrategies.Bubble);

    public event EventHandler<NavMenuItemClickEventArgs>? NavMenuItemClick
    {
        add => AddHandler(NavMenuItemClickEvent, value);
        remove => RemoveHandler(NavMenuItemClickEvent, value);
    }

    #endregion

    #region 内部属性定义

    internal static readonly StyledProperty<double> HorizontalBorderThicknessProperty =
        AvaloniaProperty.Register<NavMenuItem, double>(nameof(HorizontalBorderThickness));

    public double HorizontalBorderThickness
    {
        get => GetValue(HorizontalBorderThicknessProperty);
        set => SetValue(HorizontalBorderThicknessProperty, value);
    }

    #endregion

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel { Orientation = Orientation.Vertical });

    static NavMenu()
    {
        ItemsPanelProperty.OverrideDefaultValue(typeof(NavMenu), DefaultPanel);
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(
            typeof(NavMenu),
            KeyboardNavigationMode.Once);
        AutomationProperties.AccessibilityViewProperty.OverrideDefaultValue<NavMenu>(AccessibilityView.Control);
        AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<NavMenu>(AutomationControlType.Menu);
    }

    private CompositeDisposable? _resourceBindingsDisposable;
    private ItemsPresenter? _menuItemsPresenter;

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable
    {
        get => _resourceBindingsDisposable;
        set => _resourceBindingsDisposable = value;
    }

    public NavMenu()
    {
        UpdatePseudoClasses();
    }

    public override void Close()
    {
        if (!IsOpen)
        {
            return;
        }

        foreach (var i in ((INavMenu)this).SubItems)
        {
            i.Close();
        }

        IsOpen        = false;
        SelectedIndex = -1;

        RaiseEvent(new RoutedEventArgs
        {
            RoutedEvent = ClosedEvent,
            Source      = this,
        });
    }

    public override void Open()
    {
        if (IsOpen)
        {
            return;
        }

        IsOpen = true;

        RaiseEvent(new RoutedEventArgs
        {
            RoutedEvent = OpenedEvent,
            Source      = this,
        });
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsDarkStyleProperty ||
            change.Property == ModeProperty)
        {
            UpdatePseudoClasses();
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == ModeProperty)
            {
                SetupControlTheme();
                HandleModeChanged();
            }
        }
    }

    private void SetupControlTheme()
    {
        if (Mode == NavMenuMode.Horizontal)
        {
            this.AddResourceBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, NavMenuThemeConstants.HorizontalNavMenuThemeId));
        }
        else
        {
            this.AddResourceBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, NavMenuThemeConstants.VerticalNavMenuThemeId));
        }
    }

    private void HandleModeChanged()
    {
        CloseChildItemsRecursively();
        SetupItemContainerTheme(true);
        RegenerateContainersRecursively();
        SetupMenuItemsPresenter();
        SetupInteractionHandler(true);
    }

    private void SetupMenuItemsPresenter()
    {
        if (_menuItemsPresenter?.Panel is StackPanel stackPanel)
        {
            if (Mode == NavMenuMode.Horizontal)
            {
                stackPanel.Orientation = Orientation.Horizontal;
            }
            else
            {
                stackPanel.Orientation = Orientation.Vertical;
            }
        }
    }

    protected override void PrepareContainerForItemOverride(Control element, object? item, int index)
    {
        base.PrepareContainerForItemOverride(element, item, index);
        // Child menu items should not inherit the menu's ItemContainerTheme as that is specific
        // for top-level menu items.
        if ((element as NavMenuItem)?.ItemContainerTheme == ItemContainerTheme)
        {
            element.ClearValue(ItemContainerThemeProperty);
        }

        if (element is NavMenuItem navMenuItem)
        {
            if (Mode == NavMenuMode.Horizontal)
            {
                BindUtils.RelayBind(this, ActiveBarHeightProperty, navMenuItem, NavMenuItem.ActiveBarHeightProperty);
                BindUtils.RelayBind(this, ActiveBarWidthProperty, navMenuItem, NavMenuItem.ActiveBarWidthProperty);
            }
            else
            {
                BindUtils.RelayBind(this, ItemContainerThemeProperty, navMenuItem, ItemContainerThemeProperty);
            }

            BindUtils.RelayBind(this, ModeProperty, navMenuItem, NavMenuItem.ModeProperty);
            BindUtils.RelayBind(this, IsDarkStyleProperty, navMenuItem, NavMenuItem.IsDarkStyleProperty);
            BindUtils.RelayBind(this, IsMotionEnabledProperty, navMenuItem, NavMenuItem.IsMotionEnabledProperty);
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(HorizontalModePC, Mode == NavMenuMode.Horizontal);
        PseudoClasses.Set(VerticalModePC, Mode == NavMenuMode.Vertical);
        PseudoClasses.Set(InlineModePC, Mode == NavMenuMode.Inline);
        PseudoClasses.Set(DarkStylePC, IsDarkStyle);
        PseudoClasses.Set(LightStylePC, !IsDarkStyle);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        _resourceBindingsDisposable = new CompositeDisposable();
        SetupControlTheme();
        SetupItemContainerTheme();
        base.OnAttachedToLogicalTree(e);
        
        SetupInteractionHandler();
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, HorizontalBorderThicknessProperty,
            SharedTokenKey.LineWidth,
            BindingPriority.Template,
            new RenderScaleAwareDoubleConfigure(this)));
        InteractionHandler?.Attach(this);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        InteractionHandler?.Detach(this);
    }

    private void SetupInteractionHandler(bool needMount = false)
    {
        if (needMount)
        {
            InteractionHandler?.Detach(this);
        }

        if (Mode == NavMenuMode.Inline)
        {
            InteractionHandler = new InlineNavMenuInteractionHandler();
        }
        else
        {
            InteractionHandler = new DefaultNavMenuInteractionHandler();
        }

        if (needMount)
        {
            InteractionHandler?.Attach(this);
        }
    }

    private void SetupItemContainerTheme(bool force = false)
    {
        if (ItemContainerTheme is null || force)
        {
            string resourceKey;
            if (Mode == NavMenuMode.Vertical)
            {
                resourceKey = NavMenuThemeConstants.VerticalNavMenuItemThemeId;
            }
            else if (Mode == NavMenuMode.Inline)
            {
                resourceKey = NavMenuThemeConstants.InlineNavMenuItemThemeId;
            }
            else
            {
                resourceKey = NavMenuThemeConstants.TopLevelHorizontalNavMenuItemThemeId;
            }

            this.AddResourceBindingDisposable(
                TokenResourceBinder.CreateTokenBinding(this, ItemContainerThemeProperty, resourceKey));
        }
    }

    internal void ClearSelection()
    {
        foreach (var item in Items)
        {
            if (item is NavMenuItem navMenuItem)
            {
                ClearSelectionRecursively(navMenuItem);
            }
        }
    }

    internal static void ClearSelectionRecursively(NavMenuItem item, bool skipSelf = false)
    {
        if (!skipSelf)
        {
            item.IsSelected = false;
        }

        foreach (var childItem in item.Items)
        {
            if (childItem is NavMenuItem navMenuItem)
            {
                ClearSelectionRecursively(navMenuItem);
            }
        }
    }

    private void CloseChildItemsRecursively()
    {
        CloseInlineItems();
        foreach (var i in ((INavMenu)this).SubItems)
        {
            i.Close();
        }
    }

    private void RegenerateContainersRecursively()
    {
        foreach (var item in Items)
        {
            if (item is NavMenuItem navMenuItem)
            {
                navMenuItem.RegenerateContainers();
            }
        }
    }

    private void CloseInlineItems()
    {
        foreach (var item in Items)
        {
            if (item is NavMenuItem navMenuItem)
            {
                CloseInlineItemRecursively(navMenuItem);
            }
        }
    }

    private void CloseInlineItemRecursively(NavMenuItem navMenuItem)
    {
        foreach (var item in navMenuItem.Items)
        {
            if (item is NavMenuItem childNavMenuItem)
            {
                CloseInlineItemRecursively(childNavMenuItem);
            }
        }

        navMenuItem.CloseInlineItem(true);
        navMenuItem.IsSubMenuOpen = false;
    }

    internal void RaiseNavMenuItemClick(INavMenuItem navMenuItem)
    {
        RaiseEvent(new NavMenuItemClickEventArgs(NavMenuItemClickEvent, navMenuItem));
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _menuItemsPresenter = e.NameScope.Find<ItemsPresenter>(NavMenuThemeConstants.ItemsPresenterPart);
        SetupMenuItemsPresenter();
    }
}