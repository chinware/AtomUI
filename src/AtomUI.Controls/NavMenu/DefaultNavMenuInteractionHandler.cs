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

namespace AtomUI.Controls.NavMenu;

public class DefaultNavMenuInteractionHandler : INavMenuInteractionHandler
{
    private readonly bool _isContextMenu;
    private IDisposable? _inputManagerSubscription;
    private IRenderRoot? _root;

    public DefaultNavMenuInteractionHandler(bool isContextMenu)
        : this(isContextMenu, AvaloniaLocator.Current.GetService<IInputManager>(), DefaultDelayRun)
    {
    }

    public DefaultNavMenuInteractionHandler(
        bool isContextMenu,
        IInputManager? inputManager,
        Action<Action, TimeSpan> delayRun)
    {
        delayRun = delayRun ?? throw new ArgumentNullException(nameof(delayRun));

        _isContextMenu = isContextMenu;
        InputManager   = inputManager;
        DelayRun       = delayRun;
    }

    public void Attach(NavMenuBase navMenu) => AttachCore(navMenu);
    public void Detach(NavMenuBase navMenu) => DetachCore(navMenu);

    protected Action<Action, TimeSpan> DelayRun { get; }

    protected IInputManager? InputManager { get; }

    internal INavMenu? NavMenu { get; private set; }

    public static TimeSpan MenuShowDelay { get; set; } = TimeSpan.FromMilliseconds(400);

    protected internal virtual void GotFocus(object? sender, GotFocusEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);

        if (item?.Parent != null)
        {
            item.SelectedItem = item;
        }
    }

    protected internal virtual void LostFocus(object? sender, RoutedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);

        if (item != null)
        {
            item.SelectedItem = null;
        }
    }

    protected internal virtual void KeyDown(object? sender, KeyEventArgs e)
    {
        KeyDown(GetMenuItemCore(e.Source as Control), e);
    }

    protected internal virtual void AccessKeyPressed(object? sender, RoutedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);

        if (item == null)
        {
            return;
        }

        if (item.HasSubMenu && item.IsEffectivelyEnabled)
        {
            Open(item, true);
        }
        else
        {
            Click(item);
        }

        e.Handled = true;
    }

    protected internal virtual void PointerEntered(object? sender, RoutedEventArgs e)
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
                SelectItemAndAncestors(item);
                if (item.HasSubMenu)
                    Open(item, false);
            }
            else
            {
                SelectItemAndAncestors(item);
            }
        }
        else
        {
            SelectItemAndAncestors(item);

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

    protected internal virtual void PointerMoved(object? sender, PointerEventArgs e)
    {
        // HACK: #8179 needs to be addressed to correctly implement it in the PointerPressed method.
        var item = GetMenuItemCore(e.Source as Control) as MenuItem;

        if (item == null)
            return;

        var transformedBounds = item.GetTransformedBounds();
        if (transformedBounds == null)
            return;

        var point = e.GetCurrentPoint(null);

        if (point.Properties.IsLeftButtonPressed
            && transformedBounds.Value.Contains(point.Position) == false)
        {
            e.Pointer.Capture(null);
        }
    }

    protected internal virtual void PointerExited(object? sender, RoutedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);

        if (item?.Parent == null)
        {
            return;
        }

        if (item.Parent.SelectedItem == item)
        {
            if (item.IsTopLevel)
            {
                if (!((INavMenu)item.Parent).IsOpen)
                {
                    item.Parent.SelectedItem = null;
                }
            }
            else if (!item.HasSubMenu)
            {
                item.Parent.SelectedItem = null;
            }
            else if (!item.IsPointerOverSubMenu)
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

    protected internal virtual void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);

        if (sender is Visual visual &&
            e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed && item?.HasSubMenu == true)
        {
            if (item.IsSubMenuOpen)
            {
                // PointerPressed events may bubble from disabled items in sub-menus. In this case,
                // keep the sub-navMenu open.
                var popup = (e.Source as ILogical)?.FindLogicalAncestorOfType<Popup>();
                if (item.IsTopLevel && popup == null)
                {
                    CloseMenu(item);
                }
            }
            else
            {
                // if (item.IsTopLevel && item.Parent is IMainMenu mainMenu)
                // {
                //     mainNavMenu.Open();
                // }

                Open(item, false);
            }

            e.Handled = true;
        }
    }

    protected internal virtual void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);

        if (e.InitialPressMouseButton == MouseButton.Left && item?.HasSubMenu == false)
        {
            Click(item);
            e.Handled = true;
        }
    }

    protected internal virtual void MenuOpened(object? sender, RoutedEventArgs e)
    {
        if (e.Source is NavMenu)
        {
            NavMenu?.MoveSelection(NavigationDirection.First, true);
        }
    }

    protected internal virtual void RawInput(RawInputEventArgs e)
    {
        var mouse = e as RawPointerEventArgs;

        if (mouse?.Type == RawPointerEventType.NonClientLeftButtonDown)
        {
            NavMenu?.Close();
        }
    }

    protected internal virtual void RootPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (NavMenu?.IsOpen == true)
        {
            if (e.Source is ILogical control && !NavMenu.IsLogicalAncestorOf(control))
            {
                NavMenu.Close();
            }
        }
    }

    protected internal virtual void WindowDeactivated(object? sender, EventArgs e)
    {
        NavMenu?.Close();
    }

    internal static MenuItem? GetMenuItem(StyledElement? item) => (MenuItem?)GetMenuItemCore(item);

    internal void AttachCore(INavMenu navMenu)
    {
        if (NavMenu != null)
        {
            throw new NotSupportedException("DefaultMenuInteractionHandler is already attached.");
        }

        NavMenu              =  navMenu;
        NavMenu.GotFocus        += GotFocus;
        NavMenu.LostFocus       += LostFocus;
        NavMenu.KeyDown         += KeyDown;
        NavMenu.PointerPressed  += PointerPressed;
        NavMenu.PointerReleased += PointerReleased;
        // NavMenu.AddHandler(AccessKeyHandler.AccessKeyPressedEvent, AccessKeyPressed);
        NavMenu.AddHandler(MenuBase.OpenedEvent, MenuOpened);
        NavMenu.AddHandler(MenuItem.PointerEnteredItemEvent, PointerEntered);
        NavMenu.AddHandler(MenuItem.PointerExitedItemEvent, PointerExited);
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
            pimpl.LostFocus += TopLevelLostPlatformFocus;

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
        NavMenu.KeyDown         -= KeyDown;
        NavMenu.PointerPressed  -= PointerPressed;
        NavMenu.PointerReleased -= PointerReleased;
        // NavMenu.RemoveHandler(AccessKeyHandler.AccessKeyPressedEvent, AccessKeyPressed);
        NavMenu.RemoveHandler(MenuBase.OpenedEvent, MenuOpened);
        NavMenu.RemoveHandler(MenuItem.PointerEnteredItemEvent, PointerEntered);
        NavMenu.RemoveHandler(MenuItem.PointerExitedItemEvent, PointerExited);
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
            tl.PlatformImpl.LostFocus -= TopLevelLostPlatformFocus;

        _inputManagerSubscription?.Dispose();

        NavMenu  = null;
        _root = null;
    }

    internal void Click(INavMenuItem item)
    {
        item.RaiseClick();

        if (!item.StaysOpenOnClick)
        {
            CloseMenu(item);
        }
    }

    internal void CloseMenu(INavMenuItem item)
    {
        var current = (INavMenuElement?)item;

        while (current != null && !(current is INavMenu))
        {
            current = (current as INavMenuItem)?.Parent;
        }

        current?.Close();
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

    internal void KeyDown(INavMenuItem? item, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
            case Key.Down:
            {
                if (item?.IsTopLevel == true && item.HasSubMenu)
                {
                    if (!item.IsSubMenuOpen)
                    {
                        Open(item, true);
                    }
                    else
                    {
                        item.MoveSelection(NavigationDirection.First, true);
                    }

                    e.Handled = true;
                }
                else
                {
                    goto default;
                }

                break;
            }

            case Key.Left:
            {
                if (item is { IsSubMenuOpen: true, SelectedItem: null })
                {
                    item.Close();
                }
                else if (item?.Parent is INavMenuItem { IsTopLevel: false, IsSubMenuOpen: true } parent)
                {
                    parent.Close();
                    parent.Focus();
                    e.Handled = true;
                }
                else
                {
                    goto default;
                }

                break;
            }

            case Key.Right:
            {
                if (item != null && !item.IsTopLevel && item.HasSubMenu)
                {
                    Open(item, true);
                    e.Handled = true;
                }
                else
                {
                    goto default;
                }

                break;
            }

            case Key.Enter:
            {
                if (item != null)
                {
                    if (!item.HasSubMenu)
                    {
                        Click(item);
                    }
                    else
                    {
                        Open(item, true);
                    }

                    e.Handled = true;
                }

                break;
            }

            case Key.Escape:
            {
                if (item?.Parent is INavMenuElement parent)
                {
                    parent.Close();
                    parent.Focus();
                }
                else
                {
                    NavMenu!.Close();
                }

                e.Handled = true;
                break;
            }

            default:
            {
                var direction = e.Key.ToNavigationDirection();

                if (direction?.IsDirectional() == true)
                {
                    if (item == null && _isContextMenu)
                    {
                        if (NavMenu!.MoveSelection(direction.Value, true) == true)
                        {
                            e.Handled = true;
                        }
                    }
                    else if (item?.Parent?.MoveSelection(direction.Value, true) == true)
                    {
                        // If the the parent is an IMenu which successfully moved its selection,
                        // and the current navMenu is open then close the current navMenu and open the
                        // new navMenu.
                        if (item.IsSubMenuOpen &&
                            item.Parent is INavMenu &&
                            item.Parent.SelectedItem is object &&
                            item.Parent.SelectedItem != item)
                        {
                            item.Close();
                            Open(item.Parent.SelectedItem, true);
                        }

                        e.Handled = true;
                    }
                }

                break;
            }
        }

        if (!e.Handled && item?.Parent is INavMenuItem parentItem)
        {
            KeyDown(parentItem, e);
        }
    }

    internal void Open(INavMenuItem item, bool selectFirst)
    {
        item.Open();

        if (selectFirst)
        {
            item.MoveSelection(NavigationDirection.First, true);
        }
    }

    internal void OpenWithDelay(INavMenuItem item)
    {
        void Execute()
        {
            if (item.Parent?.SelectedItem == item)
            {
                Open(item, false);
            }
        }

        DelayRun(Execute, MenuShowDelay);
    }

    internal void SelectItemAndAncestors(INavMenuItem item)
    {
        var current = (INavMenuItem?)item;

        while (current?.Parent != null)
        {
            current.Parent.SelectedItem = current;
            current                     = current.Parent as INavMenuItem;
        }
    }

    internal static INavMenuItem? GetMenuItemCore(StyledElement? item)
    {
        while (true)
        {
            if (item == null)
                return null;
            if (item is INavMenuItem menuItem)
                return menuItem;
            item = item.Parent;
        }
    }

    private void TopLevelLostPlatformFocus()
    {
        NavMenu?.Close();
    }

    private static void DefaultDelayRun(Action action, TimeSpan timeSpan)
    {
        DispatcherTimer.RunOnce(action, timeSpan);
    }
    
}