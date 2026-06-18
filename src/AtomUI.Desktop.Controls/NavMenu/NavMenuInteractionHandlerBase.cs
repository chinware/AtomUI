using AtomUI.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

internal abstract class NavMenuInteractionHandlerBase : INavMenuInteractionHandler
{
    private bool _currentPressedIsValid;
    private NavMenuItem? _latestClickedItem;

    protected NavMenuSelectionCoordinator SelectionCoordinator { get; } = new();

    internal INavMenu? Menu { get; private set; }

    public void Attach(NavMenu navMenu) => AttachCore(navMenu);

    public void Detach(NavMenu navMenu) => DetachCore(navMenu);

    public abstract void Select(NavMenuItem menuItem);

    public void ClearSelection()
    {
        SelectionCoordinator.ClearSelection();
    }

    internal void AttachCore(INavMenu navMenu)
    {
        if (Menu != null)
        {
            throw new NotSupportedException("NavMenu interaction handler is already attached.");
        }

        Menu                 =  navMenu;
        Menu.PointerPressed  += PointerPressed;
        Menu.PointerReleased += PointerReleased;
        OnAttached(navMenu);
    }

    internal void DetachCore(INavMenu navMenu)
    {
        if (Menu != navMenu)
        {
            throw new NotSupportedException("NavMenu interaction handler is not attached to the navMenu.");
        }

        Menu.PointerPressed  -= PointerPressed;
        Menu.PointerReleased -= PointerReleased;
        OnDetached(navMenu);

        Menu = null;
        ResetPressState();
        SelectionCoordinator.Reset();
    }

    protected virtual void OnAttached(INavMenu navMenu)
    {
    }

    protected virtual void OnDetached(INavMenu navMenu)
    {
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

    protected virtual void Click(INavMenuItem item)
    {
        (item as IClickableControl)?.RaiseClick();
        if (Menu is NavMenu navMenu)
        {
            navMenu.RaiseNavMenuItemClick(item);
        }
    }

    protected static INavMenuItem? FindTopLevelMenuItem(INavMenuItem item)
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

    protected static NavMenuItem? GetMenuItemCore(StyledElement? item)
    {
        NavMenuItem? target  = null;
        var          current = item;
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

    private void ResetPressState()
    {
        _currentPressedIsValid = false;
        _latestClickedItem     = null;
    }
}
