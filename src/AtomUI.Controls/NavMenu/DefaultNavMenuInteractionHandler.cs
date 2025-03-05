using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class DefaultNavMenuInteractionHandler : INavMenuInteractionHandler
{
    private IDisposable? _inputManagerSubscription;
    private IRenderRoot? _root;
    private IDisposable? _currentDelayRunDisposable;
    private bool _currentPressedIsValid = false;

    public DefaultNavMenuInteractionHandler()
        : this(AvaloniaLocator.Current.GetService<IInputManager>(), DefaultDelayRun)
    {
    }

    public DefaultNavMenuInteractionHandler(
        IInputManager? inputManager,
        Func<Action, TimeSpan, IDisposable> delayRun)
    {
        delayRun = delayRun ?? throw new ArgumentNullException(nameof(delayRun));
        
        InputManager   = inputManager;
        DelayRun       = delayRun;
    }

    public void Attach(NavMenuBase navMenu) => AttachCore(navMenu);
    public void Detach(NavMenuBase navMenu) => DetachCore(navMenu);

    protected Func<Action, TimeSpan, IDisposable> DelayRun { get; }

    protected IInputManager? InputManager { get; }

    internal INavMenu? NavMenu { get; private set; }

    public static TimeSpan MenuShowDelay { get; set; } = TimeSpan.FromMilliseconds(400);

    protected virtual void GotFocus(object? sender, GotFocusEventArgs e)
    {
    }

    protected virtual void LostFocus(object? sender, RoutedEventArgs e)
    {
    }

    protected virtual void PointerEntered(object? sender, RoutedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);
        if (item?.Parent == null)
        {
            return;
        }

        if (item.IsTopLevel)
        {
            if (item != item.Parent.SelectedItem &&
                item.Parent.SelectedItem?.IsSubMenuOpen == true)
            {
                item.Parent.SelectedItem.Close();
                if (item.HasSubMenu)
                {
                    item.Open();
                }
            }
        }
        else
        {
            if (item.HasSubMenu)
            {
                OpenWithDelay(item);
            }
            else if (item.Parent != null)
            {
                foreach (var sibling in item.Parent.SubItems)
                {
                    if (sibling.IsSubMenuOpen)
                    {
                        CloseWithDelay(sibling);
                    }
                }
            }
        }
    }

    protected virtual void PointerMoved(object? sender, PointerEventArgs e)
    {
        // HACK: #8179 needs to be addressed to correctly implement it in the PointerPressed method.
        var item = GetMenuItemCore(e.Source as Control) as NavMenuItem;

        if (item == null)
        {
            return;
        }

        var transformedBounds = item.GetTransformedBounds();
        if (transformedBounds == null)
        {
            return;
        }

        var point = e.GetCurrentPoint(null);

        if (point.Properties.IsLeftButtonPressed
            && transformedBounds.Value.Contains(point.Position) == false)
        {
            e.Pointer.Capture(null);
        }
    }

    protected virtual void PointerExited(object? sender, RoutedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);

        if (item?.Parent == null)
        {
            return;
        }

        if (item.Parent.SelectedItem == item)
        {
            if (!item.IsPointerOverSubMenu)
            {
                DelayRun(() =>
                {
                    if (!item.IsPointerOverSubMenu)
                    {
                        item.IsSubMenuOpen = false;
                    }
                }, MenuShowDelay);
            }
        }
    }

    protected virtual void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);
        if (item is null) 
        {
            return;
        }

        if (item is NavMenuItem navMenuItem)
        {
            if (!navMenuItem.PointInNavMenuItemHeader(e.GetCurrentPoint(navMenuItem).Position))
            {
                return;
            }

            _currentPressedIsValid = true;
            if (sender is Visual visual &&
                e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed)
            {
                if (navMenuItem.HasSubMenu)
                {
                    if (navMenuItem.IsSubMenuOpen)
                    {
                        // PointerPressed events may bubble from disabled items in sub-menus. In this case,
                        // keep the sub-navMenu open.
                        var popup = (e.Source as ILogical)?.FindLogicalAncestorOfType<Popup>();
                        if (navMenuItem.IsTopLevel && popup == null)
                        {
                            CloseMenu(navMenuItem);
                        }
                    }
                    else
                    {
                        navMenuItem.Open();
                    }
                }
                else
                {
                    if (NavMenu is NavMenu navMenu)
                    {
                        navMenu.ClearSelection();
                    }

                    navMenuItem.SelectItemRecursively();
                }
            
                e.Handled = true;
            }
        }
    }

    protected virtual void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);
        if (item is null || !_currentPressedIsValid) 
        {
            return;
        }

        _currentPressedIsValid = false;
        if (e.InitialPressMouseButton == MouseButton.Left && item?.HasSubMenu == false)
        {
            Click(item);
            e.Handled = true;
        }
    }

    protected virtual void MenuOpened(object? sender, RoutedEventArgs e)
    {
    }

    protected virtual void RawInput(RawInputEventArgs e)
    {
        var mouse = e as RawPointerEventArgs;

        if (mouse?.Type == RawPointerEventType.NonClientLeftButtonDown)
        {
            NavMenu?.Close();
        }
    }

    protected virtual void RootPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (NavMenu?.IsOpen == true)
        {
            if (e.Source is ILogical control && !NavMenu.IsLogicalAncestorOf(control))
            {
                NavMenu.Close();
            }
        }
    }

    protected virtual void WindowDeactivated(object? sender, EventArgs e)
    {
        NavMenu?.Close();
    }

    internal static NavMenuItem? GetMenuItem(StyledElement? item) => (NavMenuItem?)GetMenuItemCore(item);

    internal void AttachCore(INavMenu navMenu)
    {
        if (NavMenu != null)
        {
            throw new NotSupportedException("DefaultMenuInteractionHandler is already attached.");
        }

        NavMenu              =  navMenu;
        NavMenu.GotFocus        += GotFocus;
        NavMenu.LostFocus       += LostFocus;
        NavMenu.PointerPressed  += PointerPressed;
        NavMenu.PointerReleased += PointerReleased;
        NavMenu.AddHandler(NavMenuBase.OpenedEvent, MenuOpened);
        NavMenu.AddHandler(NavMenuItem.PointerEnteredItemEvent, PointerEntered);
        NavMenu.AddHandler(NavMenuItem.PointerExitedItemEvent, PointerExited);
        NavMenu.AddHandler(InputElement.PointerMovedEvent, PointerMoved);

        _root = NavMenu.VisualRoot;

        if (_root is InputElement inputRoot)
        {
            inputRoot.AddHandler(InputElement.PointerPressedEvent, RootPointerPressed, RoutingStrategies.Tunnel);
        }

        if (_root is WindowBase window)
        {
            window.Deactivated += WindowDeactivated;
        }

        if (_root is TopLevel tl && tl.PlatformImpl is ITopLevelImpl pimpl)
        {
            pimpl.LostFocus += TopLevelLostPlatformFocus;
        }

        _inputManagerSubscription = InputManager?.Process.Subscribe(RawInput);
    }

    internal void DetachCore(INavMenu navMenu)
    {
        if (NavMenu != navMenu)
        {
            throw new NotSupportedException("DefaultMenuInteractionHandler is not attached to the navMenu.");
        }

        NavMenu.GotFocus        -= GotFocus;
        NavMenu.LostFocus       -= LostFocus;
        NavMenu.PointerPressed  -= PointerPressed;
        NavMenu.PointerReleased -= PointerReleased;
        NavMenu.RemoveHandler(NavMenuBase.OpenedEvent, MenuOpened);
        NavMenu.RemoveHandler(NavMenuItem.PointerEnteredItemEvent, PointerEntered);
        NavMenu.RemoveHandler(NavMenuItem.PointerExitedItemEvent, PointerExited);
        NavMenu.RemoveHandler(InputElement.PointerMovedEvent, PointerMoved);

        if (_root is InputElement inputRoot)
        {
            inputRoot.RemoveHandler(InputElement.PointerPressedEvent, RootPointerPressed);
        }

        if (_root is WindowBase root)
        {
            root.Deactivated -= WindowDeactivated;
        }

        if (_root is TopLevel tl && tl.PlatformImpl != null)
        {
            tl.PlatformImpl.LostFocus -= TopLevelLostPlatformFocus;
        }

        _inputManagerSubscription?.Dispose();

        NavMenu  = null;
        _root = null;
    }

    internal void Click(INavMenuItem item)
    {
        item.RaiseClick();
        var navMenu = FindNavMenu(item);
        navMenu?.RaiseNavMenuItemClick(item);
        
        if (!item.StaysOpenOnClick)
        {
            CloseMenu(item);
        }
    }

    internal void CloseMenu(INavMenuItem item)
    {
        var navMenu = FindNavMenu(item);
        navMenu?.Close();
    }

    private static NavMenu? FindNavMenu(INavMenuItem item)
    {
        var       current = (INavMenuElement?)item;
        NavMenu? result  = null;

        while (current != null && !(current is NavMenu))
        {
            current = (current as INavMenuItem)?.Parent;
        }

        if (current is NavMenu navMenu)
        {
            result = navMenu;
        }
        return result;
    }

    internal void CloseWithDelay(INavMenuItem item)
    {
        void Execute()
        {
            if (item.Parent?.SelectedItem != item)
            {
                item.Close();
            }
        }

        DelayRun(Execute, MenuShowDelay);
    }
    
    internal void OpenWithDelay(INavMenuItem item)
    {
        void Execute()
        {
            item.Open();
        }
        _currentDelayRunDisposable?.Dispose();
        _currentDelayRunDisposable = DelayRun(Execute, MenuShowDelay);
    }

    internal static INavMenuItem? GetMenuItemCore(StyledElement? item)
    {
        while (true)
        {
            if (item == null)
            {
                return null;
            }

            if (item is INavMenuItem menuItem)
            {
                return menuItem;
            }
               
            item = item.Parent;
        }
    }

    private void TopLevelLostPlatformFocus()
    {
        NavMenu?.Close();
    }

    private static IDisposable DefaultDelayRun(Action action, TimeSpan timeSpan)
    {
        return DispatcherTimer.RunOnce(action, timeSpan);
    }
}