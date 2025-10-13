using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Threading;

namespace AtomUI.Controls;

using NavMenuControl = NavMenu;

internal class DefaultNavMenuInteractionHandler : INavMenuInteractionHandler
{
    private IDisposable? _inputManagerSubscription;
    private IRenderRoot? _root;
    private IDisposable? _currentOpenDelayRunDisposable;
    private IDisposable? _currentCloseDelayRunDisposable;
    private bool _currentPressedIsValid = false;
    internal StyledElement? LatestSelectedItem = null;

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

        _currentOpenDelayRunDisposable?.Dispose();
        _currentCloseDelayRunDisposable?.Dispose();
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
                    sibling.Close();
                }
            }
        }
    }

    protected virtual void PointerExited(object? sender, RoutedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);

        if (item?.Parent == null)
        {
            return;
        }

        _currentOpenDelayRunDisposable?.Dispose();
        _currentCloseDelayRunDisposable?.Dispose();

        if (!item.IsPointerOverSubMenu)
        {
            _currentCloseDelayRunDisposable = DelayRun(() =>
            {
                if (!item.IsPointerOverSubMenu)
                {
                    item.Close();
                }
            }, MenuShowDelay);
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
                Select(navMenuItem);
                e.Handled = true;
            }
        }
    }

    public void Select(NavMenuItem navMenuItem)
    {
        if (navMenuItem.HasSubMenu)
        {
            if (!navMenuItem.IsSubMenuOpen)
            {
                navMenuItem.Open();
            }
        }
        else
        {
            // 判断当前选中的是不是自己
            if (!ReferenceEquals(LatestSelectedItem, navMenuItem))
            {
                if (LatestSelectedItem != null)
                {
                    var ancestorInfo = HasCommonAncestor(LatestSelectedItem, navMenuItem);
                    if (!ancestorInfo.Item1)
                    {
                        if (NavMenu is NavMenu navMenu)
                        {
                            navMenu.ClearSelection();
                        }
                    }
                    else
                    {
                        if (ancestorInfo.Item2 is NavMenuItem neededClearAncestor)
                        {
                            NavMenuControl.ClearSelectionRecursively(neededClearAncestor, true);
                        }
                    }
                }
                navMenuItem.SelectItemRecursively();
                LatestSelectedItem = navMenuItem;
            }
        }
    }

    private IList<StyledElement> CollectAncestors(StyledElement control)
    {
        var            ancestors = new List<StyledElement>();
        StyledElement? current   = control.Parent;
        while (current != null && (current is NavMenuItem || control is NavMenu))
        {
            ancestors.Add(current);
            current = current.Parent;
        }

        return ancestors;
    }
    
    private (bool, StyledElement?) HasCommonAncestor(StyledElement lhs, StyledElement rhs)
    {
        var lhsAncestors = CollectAncestors(lhs);
        var rhsAncestors = CollectAncestors(rhs);
        var hasOverlaps  = lhsAncestors.ToHashSet().Overlaps(rhsAncestors.ToHashSet());
        if (!hasOverlaps)
        {
            return (false, null);
        }
        // 找共同的祖先
        StyledElement? commonAncestor = null;
        for (var i = 0; i < lhsAncestors.Count; i++)
        {
            var lhsAncestor = lhsAncestors[i];
            for (var j = 0; j < rhsAncestors.Count; j++)
            {
                var rhsAncestor = rhsAncestors[j];
                if (ReferenceEquals(lhsAncestor, rhsAncestor))
                {
                    commonAncestor = lhsAncestor;
                    break;
                }
            }

            if (commonAncestor != null)
            {
                break;
            }
        }
        Debug.Assert(commonAncestor != null);
        return (true, commonAncestor);
    }

    protected virtual void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);
        if (item is null || !_currentPressedIsValid) 
        {
            return;
        }

        _currentPressedIsValid = false;
        if (e.InitialPressMouseButton == MouseButton.Left && item.HasSubMenu == false)
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
    
    internal void OpenWithDelay(INavMenuItem item)
    {
        void Execute()
        {
            var parent = item.Parent as NavMenuItem;
            if (!item.IsTopLevel && parent?.Popup?.Host != null)
            {
                item.Open();
            }
        }
        _currentOpenDelayRunDisposable?.Dispose();
        _currentOpenDelayRunDisposable = DelayRun(Execute, MenuShowDelay);
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