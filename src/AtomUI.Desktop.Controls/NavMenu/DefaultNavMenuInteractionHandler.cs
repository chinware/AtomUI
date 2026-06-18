using AtomUI.Controls;
using AtomUI.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class DefaultNavMenuInteractionHandler : INavMenuInteractionHandler
{
    private IDisposable? _inputManagerSubscription;
    private TopLevel? _root;
    private IDisposable? _currentOpenDelayRunDisposable;
    private IDisposable? _currentCloseDelayRunDisposable;
    private bool _currentPressedIsValid;
    private NavMenuItem? _latestClickedItem;
    private IDisposable? _deactivationSubscription;
    private readonly NavMenuSelectionCoordinator _selectionCoordinator = new();

    public DefaultNavMenuInteractionHandler()
        : this(AvaloniaLocator.Current.GetService<IInputManager>(), DefaultDelayRun)
    {
    }

    public DefaultNavMenuInteractionHandler(IInputManager? inputManager, Func<Action, TimeSpan, IDisposable> delayRun)
    {
        delayRun = delayRun ?? throw new ArgumentNullException(nameof(delayRun));

        InputManager = inputManager;
        DelayRun     = delayRun;
    }

    public void Attach(NavMenu navMenu) => AttachCore(navMenu);
    public void Detach(NavMenu navMenu) => DetachCore(navMenu);

    protected Func<Action, TimeSpan, IDisposable> DelayRun { get; }

    protected IInputManager? InputManager { get; }

    internal INavMenu? Menu { get; private set; }

    public static TimeSpan MenuShowDelay { get; set; } = TimeSpan.FromMilliseconds(400);

    protected virtual void NotifyPointerEntered(object? sender, RoutedEventArgs e)
    {
        var menuItem = GetMenuItemCore(e.Source as Control) as INavMenuItem;
        if (menuItem?.Parent == null)
        {
            return;
        }

        DisposePendingDelayRuns();
        if (menuItem.HasSubMenu)
        {
            OpenWithDelay(menuItem);
        }
        else if (menuItem.Parent != null)
        {
            var siblings = menuItem.Parent.LogicalChildren;
            for (var i = 0; i < siblings.Count; i++)
            {
                if (siblings[i] is INavMenuItem sibling &&
                    sibling.IsSubMenuOpen)
                {
                    sibling.Close();
                }
            }
        }
    }

    protected virtual void NotifyPointerExited(object? sender, RoutedEventArgs e)
    {
        var menuItem = GetMenuItemCore(e.Source as Control) as INavMenuItem;

        if (menuItem?.Parent == null)
        {
            return;
        }

        DisposePendingDelayRuns();

        if (!menuItem.IsPointerOverSubMenu)
        {
            _currentCloseDelayRunDisposable = DelayRun(() =>
            {
                if (!menuItem.IsPointerOverSubMenu)
                {
                    menuItem.Close();
                }

                _currentCloseDelayRunDisposable = null;
            }, MenuShowDelay);
        }
    }

    protected virtual void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ResetPressState();

        var sourceControl = e.Source as Control;
        var menuItem      = GetMenuItemCore(sourceControl);
        if (menuItem is null || !menuItem.ItemHeader.IsVisualAncestorOf(sourceControl)) 
        {
            return;
        }
        
        _currentPressedIsValid = true;
        _latestClickedItem     = menuItem;
        if (sender is Visual visual &&
            e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed)
        {
            Select(menuItem);
            e.Handled = true;
        }
    }
    
    protected virtual void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_latestClickedItem is null || !_currentPressedIsValid) 
        {
            return;
        }

        try
        {
            if (e.InitialPressMouseButton == MouseButton.Left)
            {
                Click(_latestClickedItem);
                e.Handled = true;
            }
        }
        finally
        {
            ResetPressState();
        }
    }

    public void Select(NavMenuItem menuItem)
    {
        if (menuItem.HasSubMenu)
        {
            if (!menuItem.IsSubMenuOpen)
            {
                menuItem.Open();
            }
        }
        else
        {
            _selectionCoordinator.Select(Menu, menuItem);
        }
    }

    public void ClearSelection()
    {
        _selectionCoordinator.ClearSelection();
    }
    
    protected virtual void RawInput(RawInputEventArgs e)
    {
        var mouse = e as RawPointerEventArgs;

        if (mouse?.Type == RawPointerEventType.NonClientLeftButtonDown)
        {
            CloseAllTopLevelMenuItems();
        }
    }

    protected virtual void RootPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (HasOpenTopLevelItem() && !IsPointerWithinOpenTopLevelItem(e.Source))
        {
            CloseAllTopLevelMenuItems();
        }
    }

    protected virtual void WindowDeactivated(object? sender, EventArgs e)
    {
        CloseAllTopLevelMenuItems();
    }

    private void CloseAllTopLevelMenuItems()
    {
        if (Menu != null)
        {
            var children = Menu.LogicalChildren;
            for (var i = 0; i < children.Count; i++)
            {
                if (children[i] is INavMenuItem { IsSubMenuOpen: true } menuItem)
                {
                    menuItem.Close();
                }
            }
        }
    }

    private bool HasOpenTopLevelItem()
    {
        if (Menu == null)
        {
            return false;
        }

        var children = Menu.LogicalChildren;
        for (var i = 0; i < children.Count; i++)
        {
            if (children[i] is INavMenuItem { IsSubMenuOpen: true })
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPointerWithinOpenTopLevelItem(object? source)
    {
        if (Menu == null)
        {
            return false;
        }

        var sourceLogical = source as ILogical;
        var sourceItem    = GetMenuItemCore(source as StyledElement);
        var children      = Menu.LogicalChildren;
        for (var i = 0; i < children.Count; i++)
        {
            if (children[i] is not INavMenuItem { IsSubMenuOpen: true } openTopLevelItem)
            {
                continue;
            }

            if (ReferenceEquals(source, openTopLevelItem) ||
                sourceItem != null && ReferenceEquals(FindTopLevelMenuItem(sourceItem), openTopLevelItem) ||
                openTopLevelItem.IsPointerOverSubMenu)
            {
                return true;
            }

            if (sourceLogical != null && openTopLevelItem.IsLogicalAncestorOf(sourceLogical))
            {
                return true;
            }
        }

        return false;
    }

    internal void AttachCore(INavMenu navMenu)
    {
        if (Menu != null)
        {
            throw new NotSupportedException("DefaultMenuInteractionHandler is already attached.");
        }

        Menu                 =  navMenu;
        Menu.PointerPressed  += PointerPressed;
        Menu.PointerReleased += PointerReleased;

        Menu.AddHandler(NavMenuItem.PointerEnteredItemEvent, NotifyPointerEntered);
        Menu.AddHandler(NavMenuItem.PointerExitedItemEvent, NotifyPointerExited);

        if (Menu is Visual visual)
        {
            _root = TopLevel.GetTopLevel(visual);
        }

        if (_root is InputElement inputRoot)
        {
            inputRoot.AddHandler(InputElement.PointerPressedEvent, RootPointerPressed, RoutingStrategies.Tunnel);
        }

        _deactivationSubscription = TopLevelDeactivation.Subscribe(_root, WindowDeactivated);

        if (_root != null && _root.PlatformImpl != null)
        {
            _root.PlatformImpl.LostFocus += TopLevelLostPlatformFocus;
        }

        _inputManagerSubscription = InputManager?.Process.Subscribe(RawInput);
    }

    internal void DetachCore(INavMenu navMenu)
    {
        if (Menu != navMenu)
        {
            throw new NotSupportedException("DefaultMenuInteractionHandler is not attached to the navMenu.");
        }

        Menu.PointerPressed  -= PointerPressed;
        Menu.PointerReleased -= PointerReleased;
       
        Menu.RemoveHandler(NavMenuItem.PointerEnteredItemEvent, NotifyPointerEntered);
        Menu.RemoveHandler(NavMenuItem.PointerExitedItemEvent, NotifyPointerExited);

        if (_root is InputElement inputRoot)
        {
            inputRoot.RemoveHandler(InputElement.PointerPressedEvent, RootPointerPressed);
        }

        _deactivationSubscription?.Dispose();

        if (_root is TopLevel tl && tl.PlatformImpl != null)
        {
            tl.PlatformImpl.LostFocus -= TopLevelLostPlatformFocus;
        }

        _inputManagerSubscription?.Dispose();
        _inputManagerSubscription = null;

        DisposePendingDelayRuns();
        ResetPressState();

        Menu                = null;
        _root               = null;
        _deactivationSubscription = null;
        _selectionCoordinator.Reset();
    }
    
    internal void Click(INavMenuItem item)
    {
        (item as IClickableControl)?.RaiseClick();
        if (Menu is NavMenu navMenu) 
        {
            navMenu.RaiseNavMenuItemClick(item);
            if (!item.HasSubMenu && !item.StaysOpenOnClick)
            {
                var topLevelItem = FindTopLevelMenuItem(item);
                topLevelItem?.Close();
            }
        }
    }

    private static INavMenuItem? FindTopLevelMenuItem(INavMenuItem item)
    {
        if (item.IsTopLevel)
        {
            return item;
        }

        var current = item;
        while (current != null && !current.IsTopLevel)
        {
            current = current.Parent as INavMenuItem;
        }
        return current;
    }
    
    internal void OpenWithDelay(INavMenuItem item)
    {
        void Execute()
        {
            _currentOpenDelayRunDisposable = null;
            var parent = item.Parent as NavMenuItem;
            if (!item.IsTopLevel && parent?.Popup?.IsOpen == true)
            {
                item.Open();
            }
        }
        DisposePendingOpenDelayRun();
        _currentOpenDelayRunDisposable = DelayRun(Execute, MenuShowDelay);
    }

    private void DisposePendingDelayRuns()
    {
        DisposePendingOpenDelayRun();
        DisposePendingCloseDelayRun();
    }

    private void DisposePendingOpenDelayRun()
    {
        _currentOpenDelayRunDisposable?.Dispose();
        _currentOpenDelayRunDisposable = null;
    }

    private void DisposePendingCloseDelayRun()
    {
        _currentCloseDelayRunDisposable?.Dispose();
        _currentCloseDelayRunDisposable = null;
    }

    private void ResetPressState()
    {
        _currentPressedIsValid = false;
        _latestClickedItem     = null;
    }

    internal static NavMenuItem? GetMenuItemCore(StyledElement? item)
    {
        NavMenuItem? target  = null;
        var           current = item;
        while (current != null)
        {
            if (current is NavMenuItem menuItem)
            {
                target = menuItem;
                break;
            }
            current = current.Parent;
        }
        return target;
    }

    private void TopLevelLostPlatformFocus()
    {
        Menu?.Close();
    }

    private static IDisposable DefaultDelayRun(Action action, TimeSpan timeSpan)
    {
        return DispatcherTimer.RunOnce(action, timeSpan);
    }
}
