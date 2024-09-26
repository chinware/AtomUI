using AtomUI.Data;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Automation;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;

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
public class NavMenu : NavMenuBase
{
    public const string InlineModePC = ":inline-mode";
    public const string HorizontalModePC = ":horizontal-mode";
    public const string VerticalModePC = ":vertical-mode";
    public const string DarkStylePC = ":dark";
    public const string LightStylePC = ":light";
    
    #region 公共属性定义

    /// <summary>
    /// Defines the <see cref="Mode"/> property.
    /// </summary>
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
        
        if (VisualRoot is not null)
        {
            if (change.Property == ModeProperty)
            {
                HandleModeChanged();
            }
            UpdatePseudoClasses();
        }
    }

    private void HandleModeChanged()
    {
        CloseChildItemsRecursively();
        SetupItemContainerTheme(true);
        RegenerateContainersRecursively();
        SetupInteractionHandler(true);
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
            BindUtils.RelayBind(this, ModeProperty, navMenuItem, NavMenuItem.ModeProperty);
            BindUtils.RelayBind(this, IsDarkStyleProperty, navMenuItem, NavMenuItem.IsDarkStyleProperty);
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
        base.OnAttachedToLogicalTree(e);
        SetupItemContainerTheme();
        SetupInteractionHandler();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        InteractionHandler?.Attach(this);
        TokenResourceBinder.CreateGlobalTokenBinding(this, HorizontalBorderThicknessProperty, GlobalTokenResourceKey.LineWidth,
            BindingPriority.Template,
            new RenderScaleAwareDoubleConfigure(this));
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        InteractionHandler?.Detach(this);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        TokenResourceBinder.CreateTokenBinding(this, ActiveBarWidthProperty, NavMenuTokenResourceKey.ActiveBarWidth);
        TokenResourceBinder.CreateTokenBinding(this, ActiveBarHeightProperty, NavMenuTokenResourceKey.ActiveBarHeight);
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
            var resourceKey = string.Empty; 
            if (Mode == NavMenuMode.Vertical)
            {
                resourceKey = VerticalNavMenuItemTheme.ID;
            }
            else if (Mode == NavMenuMode.Inline)
            {
                resourceKey = InlineNavMenuItemTheme.ID;
            }
            else
            {
                resourceKey = TopLevelHorizontalNavMenuItemTheme.ID;
            }
            TokenResourceBinder.CreateGlobalResourceBinding(this, ItemContainerThemeProperty, resourceKey);
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

    private void ClearSelectionRecursively(NavMenuItem item)
    {
        item.IsSelected = false;
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
        navMenuItem.CloseInlineItem();
        navMenuItem.IsSubMenuOpen = false;
    }
    
    internal void RaiseNavMenuItemClick(INavMenuItem navMenuItem)
    {
        RaiseEvent(new NavMenuItemClickEventArgs(NavMenuItemClickEvent, navMenuItem));
    }
}