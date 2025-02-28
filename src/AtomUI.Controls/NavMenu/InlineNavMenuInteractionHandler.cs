using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Controls;

internal class InlineNavMenuInteractionHandler : INavMenuInteractionHandler
{
    internal INavMenu? NavMenu { get; private set; }
    
    public void Attach(NavMenuBase navMenu) => AttachCore(navMenu);
    public void Detach(NavMenuBase navMenu) => DetachCore(navMenu);

    private bool _currentPressedIsValid = false;

    internal void AttachCore(INavMenu navMenu)
    {
        if (NavMenu != null)
        {
            throw new NotSupportedException("InlineNavMenuInteractionHandler is already attached.");
        }
        NavMenu                 =  navMenu;
        NavMenu.PointerPressed  += PointerPressed;
        NavMenu.PointerReleased += PointerReleased;
    }

    internal void DetachCore(INavMenu navMenu)
    {
        if (NavMenu != navMenu)
        {
            throw new NotSupportedException("InlineNavMenuInteractionHandler is not attached to the navMenu.");
        }
        NavMenu.PointerPressed  -= PointerPressed;
        NavMenu.PointerReleased -= PointerReleased;
        NavMenu                 =  null;
    }
    
    protected virtual void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);
        if (item is null || !item.PointInNavMenuItemHeader(e.GetCurrentPoint(item).Position)) 
        {
            return;
        }

        _currentPressedIsValid = true;
        
        if (sender is Visual visual &&
            e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed)
        {
            if (item.HasSubMenu)
            {
                if (item.IsSubMenuOpen)
                {
                    item.Close();
                }
                else
                {
                    Open(item);
                }
            }
            else
            {
                // 判断当前选中的是不是自己
                if (item.Parent is SelectingItemsControl selectingItemsControl && !ReferenceEquals(selectingItemsControl.SelectedItem, item))
                {
                    if (NavMenu is NavMenu navMenu)
                    {
                        navMenu.ClearSelection();
                    }
                    item.SelectItemRecursively();
                }
            }
            
            e.Handled = true;
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

        if (e.InitialPressMouseButton == MouseButton.Left && !item.HasSubMenu)
        {
            Click(item);
            e.Handled = true;
        }
    }
    
    internal void Click(INavMenuItem item)
    {
        item.RaiseClick();
        var navMenu = FindNavMenu(item);
        navMenu?.RaiseNavMenuItemClick(item);
    }
    
    internal void Open(INavMenuItem item)
    {
        item.Open();
    }
    
    internal static NavMenuItem? GetMenuItemCore(StyledElement? item)
    {
        while (true)
        {
            if (item == null)
            {
                return null;
            }

            if (item is NavMenuItem menuItem)
            {
                return menuItem;
            }
               
            item = item.Parent;
        }
    }
    
    private static NavMenu? FindNavMenu(INavMenuItem item)
    {
        var      current = (INavMenuElement?)item;
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
}