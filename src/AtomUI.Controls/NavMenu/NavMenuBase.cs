using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Rendering;

namespace AtomUI.Controls;

public abstract class NavMenuBase : SelectingItemsControl, 
                                    IFocusScope, 
                                    INavMenu,
                                    IMotionAwareControl,
                                    IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    
    public static readonly DirectProperty<NavMenuBase, bool> IsOpenProperty =
        AvaloniaProperty.RegisterDirect<NavMenuBase, bool>(
            nameof(IsOpen),
            o => o.IsOpen);
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<NavMenuBase>();
    
    public static readonly StyledProperty<bool> IsAccordionModeProperty =
        AvaloniaProperty.Register<NavMenu, bool>(nameof(IsAccordionMode), false);

    private bool _isOpen;
    
    public bool IsOpen
    {
        get => _isOpen;
        protected set => SetAndRaise(IsOpenProperty, ref _isOpen, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsAccordionMode
    {
        get => GetValue(IsAccordionModeProperty);
        set => SetValue(IsAccordionModeProperty, value);
    }
    
    #endregion

    #region 公共事件定义

    /// <summary>
    /// Defines the <see cref="Opened"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> OpenedEvent =
        RoutedEvent.Register<NavMenuBase, RoutedEventArgs>(nameof(Opened), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when a <see cref="NavMenu"/> is opened.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Opened
    {
        add => AddHandler(OpenedEvent, value);
        remove => RemoveHandler(OpenedEvent, value);
    }

    /// <summary>
    /// Defines the <see cref="Closed"/> event.
    /// </summary>
    public static readonly RoutedEvent<RoutedEventArgs> ClosedEvent =
        RoutedEvent.Register<NavMenuBase, RoutedEventArgs>(nameof(Closed), RoutingStrategies.Bubble);

    /// <summary>
    /// Occurs when a <see cref="NavMenu"/> is closed.
    /// </summary>
    public event EventHandler<RoutedEventArgs>? Closed
    {
        add => AddHandler(ClosedEvent, value);
        remove => RemoveHandler(ClosedEvent, value);
    }

    #endregion
    
    #region 内部属性定义
    
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => NavMenuToken.ID;

    #endregion

    INavMenuInteractionHandler? INavMenu.InteractionHandler => InteractionHandler;
    
    IRenderRoot? INavMenu.VisualRoot => VisualRoot;

    INavMenuItem? INavMenuElement.SelectedItem
    {
        get
        {
            var index = SelectedIndex;
            return (index != -1) ? (INavMenuItem?)ContainerFromIndex(index) : null;
        }

        set => SelectedIndex = value is Control c ? IndexFromContainer(c) : -1;
    }

    IEnumerable<INavMenuItem> INavMenuElement.SubItems => LogicalChildren.OfType<INavMenuItem>();

    /// <summary>
    /// Gets the interaction handler for the menu.
    /// </summary>
    protected internal INavMenuInteractionHandler? InteractionHandler { get; protected set; }

    static NavMenuBase()
    {
        NavMenuItem.SubmenuOpenedEvent.AddClassHandler<NavMenuBase>((x, e) => x.OnSubmenuOpened(e));
    }
    
    public NavMenuBase()
    {
        this.RegisterResources();
    }

    /// <summary>
    /// Closes the menu.
    /// </summary>
    public abstract void Close();

    /// <summary>
    /// Opens the menu.
    /// </summary>
    public abstract void Open();

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new NavMenuItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        if (item is NavMenuItem or Separator)
        {
            recycleKey = null;
            return false;
        }

        recycleKey = DefaultRecycleKey;
        return true;
    }

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        // Don't handle here: let the interaction handler handle it.
    }

    /// <summary>
    /// Called when a submenu opens somewhere in the menu.
    /// </summary>
    /// <param name="e">The event args.</param>
    protected virtual void OnSubmenuOpened(RoutedEventArgs e)
    {
        if (IsAccordionMode)
        {
            if (e.Source is NavMenuItem menuItem && menuItem.Parent == this)
            {
                foreach (var child in this.GetLogicalChildren().OfType<NavMenuItem>())
                {
                    if (child != menuItem && child.IsSubMenuOpen)
                    {
                        child.IsSubMenuOpen = false;
                    }
                }
            }
        }
        IsOpen = true;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsAccordionModeProperty)
        {
            if (change.GetNewValue<bool>())
            {
                foreach (var child in this.GetLogicalChildren().OfType<NavMenuItem>())
                {
                    child.IsSubMenuOpen = false;
                }
            }
        }
    }
}