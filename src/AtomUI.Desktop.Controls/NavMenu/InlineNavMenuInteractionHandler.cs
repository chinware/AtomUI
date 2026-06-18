using AtomUI.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal class InlineNavMenuInteractionHandler : INavMenuInteractionHandler
{
     internal INavMenu? Menu { get; private set; }
    
    public void Attach(NavMenu navMenu) => AttachCore(navMenu);
    public void Detach(NavMenu navMenu) => DetachCore(navMenu);

    private bool _currentPressedIsValid;
    private NavMenuItem? _latestClickedItem;
    private readonly NavMenuSelectionCoordinator _selectionCoordinator = new();

    internal void AttachCore(INavMenu navMenu)
    {
        if (Menu != null)
        {
            throw new NotSupportedException("InlineNavMenuInteractionHandler is already attached.");
        }
        Menu                 =  navMenu;
        Menu.PointerPressed  += PointerPressed;
        Menu.PointerReleased += PointerReleased;
    }

    internal void DetachCore(INavMenu navMenu)
    {
        if (Menu != navMenu)
        {
            throw new NotSupportedException("InlineNavMenuInteractionHandler is not attached to the navMenu.");
        }
        Menu.PointerPressed  -= PointerPressed;
        Menu.PointerReleased -= PointerReleased;
        Menu                 =  null;
        ResetPressState();
        _selectionCoordinator.Reset();
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
    
    internal void Click(INavMenuItem item)
    {
        if (item is IClickableControl clickableControl)
        {
            clickableControl.RaiseClick();
        }
        if (Menu is NavMenu navMenu)
        {
            navMenu.RaiseNavMenuItemClick(item);
        }
    }
    
    public void Select(NavMenuItem menuItem)
    {
        if (menuItem.HasSubMenu)
        {
            if (menuItem.IsSubMenuOpen)
            {
                menuItem.Close();
            }
            else
            {
                Open(menuItem);
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

    internal void Open(INavMenuItem menuItem) => menuItem.Open();

    private void ResetPressState()
    {
        _currentPressedIsValid = false;
        _latestClickedItem     = null;
    }
    
    internal static NavMenuItem? GetMenuItemCore(StyledElement? item)
    {
        NavMenuItem? target = null;
        var current = item;
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
}
