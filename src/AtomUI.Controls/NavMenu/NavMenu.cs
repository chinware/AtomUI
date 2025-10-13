using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls.Primitives;
using AtomUI.Controls.Themes;
using AtomUI.Data;
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
using Avalonia.Styling;
using Avalonia.Threading;
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

[PseudoClasses(
    NavMenuPseudoClass.InlineMode,
    NavMenuPseudoClass.HorizontalMode,
    NavMenuPseudoClass.VerticalMode,
    NavMenuPseudoClass.DarkStyle,
    NavMenuPseudoClass.LightStyle)]
public class NavMenu : NavMenuBase
{
    #region 公共属性定义

    public static readonly StyledProperty<NavMenuMode> ModeProperty =
        AvaloniaProperty.Register<NavMenu, NavMenuMode>(nameof(Mode), NavMenuMode.Horizontal);

    public static readonly StyledProperty<bool> IsDarkStyleProperty =
        AvaloniaProperty.Register<NavMenu, bool>(nameof(IsDarkStyle), false);

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

    internal double HorizontalBorderThickness
    {
        get => GetValue(HorizontalBorderThicknessProperty);
        set => SetValue(HorizontalBorderThicknessProperty, value);
    }
    
    #endregion

    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel { Orientation = Orientation.Vertical });
    private ItemsPresenter? _menuItemsPresenter;
    private readonly Dictionary<NavMenuItem, CompositeDisposable> _itemsBindingDisposables = new();
    private IDisposable? _borderThicknessDisposable;

    static NavMenu()
    {
        ItemsPanelProperty.OverrideDefaultValue(typeof(NavMenu), DefaultPanel);
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue(
            typeof(NavMenu),
            KeyboardNavigationMode.Once);
        AutomationProperties.AccessibilityViewProperty.OverrideDefaultValue<NavMenu>(AccessibilityView.Control);
        AutomationProperties.ControlTypeOverrideProperty.OverrideDefaultValue<NavMenu>(AutomationControlType.Menu);
        AutoScrollToSelectedItemProperty.OverrideDefaultValue<NavMenu>(false);
    }
    
    public NavMenu()
    {
        UpdatePseudoClasses();
        Items.CollectionChanged  += HandleItemsCollectionChanged;
    }
    
    private void HandleItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Remove)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is NavMenuItem menuItem)
                    {
                        if (_itemsBindingDisposables.TryGetValue(menuItem, out var disposable))
                        {
                            disposable.Dispose();
                        }
                        _itemsBindingDisposables.Remove(menuItem);
                    }
                }
            }
        }
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

        if (change.Property == ModeProperty)
        {
            ConfigureControlTheme(true);
            ConfigureItemContainerTheme(true);
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == ModeProperty)
            {
                HandleModeChanged();
            }
        }
    }

    private void HandleModeChanged()
    {
        CloseChildItemsRecursively();
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

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        // Child menu items should not inherit the menu's ItemContainerTheme as that is specific
        // for top-level menu items.
        if ((container as NavMenuItem)?.ItemContainerTheme == ItemContainerTheme)
        {
            container.ClearValue(ItemContainerThemeProperty);
        }

        if (container is NavMenuItem navMenuItem)
        {
            var disposables = new CompositeDisposable(6);
      
            if (item != null && item is not Visual)
            {
                if (!navMenuItem.IsSet(NavMenuItem.HeaderProperty))
                {
                    navMenuItem.SetCurrentValue(NavMenuItem.HeaderProperty, item);
                }

                if (item is ITreeNode treeNode)
                {
                    if (!navMenuItem.IsSet(NavMenuItem.IconProperty))
                    {
                        navMenuItem.SetCurrentValue(NavMenuItem.IconProperty, treeNode.Icon);
                    }

                    if (navMenuItem.ItemKey == null)
                    {
                        navMenuItem.ItemKey = treeNode.ItemKey;
                    }
                }
             
            }
            
            if (ItemTemplate != null)
            {
                disposables.Add(BindUtils.RelayBind(this, ItemTemplateProperty, navMenuItem, NavMenuItem.HeaderTemplateProperty));
            }
            
            disposables.Add(BindUtils.RelayBind(this, ItemContainerThemeProperty, navMenuItem, ItemContainerThemeProperty));
            disposables.Add(BindUtils.RelayBind(this, ModeProperty, navMenuItem, NavMenuItem.ModeProperty));
            disposables.Add(BindUtils.RelayBind(this, IsDarkStyleProperty, navMenuItem, NavMenuItem.IsDarkStyleProperty));
            disposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, navMenuItem, NavMenuItem.IsMotionEnabledProperty));
            
            PrepareNavMenuItem(navMenuItem, item, index, disposables);
            
            if (_itemsBindingDisposables.TryGetValue(navMenuItem, out var oldDisposables))
            {
                oldDisposables.Dispose();
                _itemsBindingDisposables.Remove(navMenuItem);
            }
            _itemsBindingDisposables.Add(navMenuItem, disposables);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type NavMenuItem.");
        }
    }
    
    protected virtual void PrepareNavMenuItem(NavMenuItem navMenuItem, object? item, int index, CompositeDisposable compositeDisposable)
    {
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(NavMenuPseudoClass.HorizontalMode, Mode == NavMenuMode.Horizontal);
        PseudoClasses.Set(NavMenuPseudoClass.VerticalMode, Mode == NavMenuMode.Vertical);
        PseudoClasses.Set(NavMenuPseudoClass.InlineMode, Mode == NavMenuMode.Inline);
        PseudoClasses.Set(NavMenuPseudoClass.DarkStyle, IsDarkStyle);
        PseudoClasses.Set(NavMenuPseudoClass.LightStyle, !IsDarkStyle);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        ConfigureControlTheme(false);
        ConfigureItemContainerTheme(false);
        SetupInteractionHandler();
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _borderThicknessDisposable = TokenResourceBinder.CreateTokenBinding(this, HorizontalBorderThicknessProperty,
            SharedTokenKey.LineWidth,
            BindingPriority.Template,
            new RenderScaleAwareDoubleConfigure(this));
        InteractionHandler?.Attach(this);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _borderThicknessDisposable?.Dispose();
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
    
    private void ConfigureControlTheme(bool force)
    {
        string? resourceKey = null;
        if (Theme == null || force)
        {
            if (Mode == NavMenuMode.Horizontal)
            {
                resourceKey = NavMenuThemeConstants.HorizontalNavMenuThemeId;
            }
            else
            {
                resourceKey = NavMenuThemeConstants.VerticalNavMenuThemeId;
            }
            if (Application.Current != null)
            {
                if (Application.Current.TryFindResource(resourceKey, out var resource))
                {
                    if (resource is ControlTheme theme)
                    {
                        Theme = theme;
                    }
                }
            }
        }
    }
    
    private void ConfigureItemContainerTheme(bool force)
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
                resourceKey = NavMenuThemeConstants.HorizontalNavMenuItemThemeId;
            }

            if (Application.Current != null)
            {
                if (Application.Current.TryFindResource(resourceKey, out var resource))
                {
                    if (resource is ControlTheme theme)
                    {
                        ItemContainerTheme = theme;
                    }
                }
            }
        }
    }

    internal void ClearSelection()
    {
        for (var i = 0; i < ItemCount; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is NavMenuItem navMenuItem)
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
        
        for (var i = 0; i < item.ItemCount; i++)
        {
            var container = item.ContainerFromIndex(i);
            if (container is NavMenuItem navMenuItem)
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
        for (var i = 0; i < ItemCount; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is NavMenuItem navMenuItem)
            {
                navMenuItem.RegenerateContainers();
            }
        }
    }

    private void CloseInlineItems()
    {
        for (var i = 0; i < ItemCount; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is NavMenuItem navMenuItem)
            {
                CloseInlineItemRecursively(navMenuItem);
            }
        }
    }

    private void CloseInlineItemRecursively(NavMenuItem navMenuItem)
    {
        for (var i = 0; i < navMenuItem.ItemCount; i++)
        {
            var container = navMenuItem.ContainerFromIndex(i);
            if (container is NavMenuItem childNavMenuItem)
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
        HandleModeChanged();
    }

    private void ConfigureDefaultOpenedPaths()
    {
        if (DefaultOpenPaths != null)
        {
            foreach (var defaultOpenPath in DefaultOpenPaths)
            {
                var pathNodes = FindMenuItemByPath(defaultOpenPath);
                OpenMenuItemPaths(pathNodes);
            }
        }
    }

    private void ConfigureDefaultSelectedPath()
    {
        if (DefaultSelectedPath != null)
        {
            var pathNodes = FindMenuItemByPath(DefaultSelectedPath);
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var navMenuItems = await OpenMenuItemPathAsync(pathNodes);
                if (InteractionHandler is not null)
                {
                    List<bool> oldFocusables = [];
                    foreach (var item in navMenuItems)
                    {
                        oldFocusables.Add(item.Focusable);
                        item.Focusable = false;
                    }
                    try
                    {
                        InteractionHandler.Select(navMenuItems.Last());
                    }
                    finally
                    {
                        Dispatcher.UIThread.Post(() =>
                        {
                            for (var i = 0; i < oldFocusables.Count; i++)
                            {
                                var item = navMenuItems[i];
                                var oldFocusable = oldFocusables[i];
                                item.Focusable = oldFocusable;
                            }
                        });
                    }
                }
            });
        }
    }

    private IList<ITreeNode> FindMenuItemByPath(TreeNodePath treeNodePath)
    {
        if (treeNodePath.Length == 0)
        {
            return [];
        }
        var              segments   = treeNodePath.Segments;
        IList<ITreeNode> items      = Items.OfType<ITreeNode>().ToList();
        IList<ITreeNode> pathNodes = new List<ITreeNode>();
        foreach (var segment in segments)
        {
            bool childFound = false;
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item.ItemKey == segment)
                {
                    items      = item.Children;
                    childFound = true;
                    pathNodes.Add(item);
                    break;
                }
            }

            if (!childFound)
            {
                return [];
            }
        }
        return pathNodes;
    }

    private void OpenMenuItemPaths(IList<ITreeNode> pathNodes)
    {
        if (pathNodes.Count == 0)
        {
            return;
        }

        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            await OpenMenuItemPathAsync(pathNodes);
        });
    }

    private async Task<List<NavMenuItem>> OpenMenuItemPathAsync(IList<ITreeNode> pathNodes)
    {
        List<NavMenuItem> items = new List<NavMenuItem>();
        try
        {
            ItemsControl current = this;
            foreach (var pathNode in pathNodes)
            {
                var child = await GetNavMenuItemContainerAsync(pathNode, current);
            
                if (child != null)
                {
                    items.Add(child);
                    current               = child;
                    child.IsMotionEnabled = false;
                    child.IsSubMenuOpen   = true;
                }
            }
        }
        finally
        {
            foreach (var item in items)
            {
                item.IsMotionEnabled = true;
            }
        }

        return items;
    }

    private async Task<NavMenuItem?> GetNavMenuItemContainerAsync(ITreeNode childNode, ItemsControl current)
    {
        var cycleCount = 10;
        NavMenuItem? target = null;
        while (cycleCount > 0)
        {
            target = current.ContainerFromItem(childNode) as NavMenuItem;
            if (target == null)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(50));
            }
            else
            {
                break;
            }
            --cycleCount;
        }
        return target;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureDefaultOpenedPaths();
        ConfigureDefaultSelectedPath();
    }
}