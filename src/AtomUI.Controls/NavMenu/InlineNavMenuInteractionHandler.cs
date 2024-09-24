using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AtomUI.Controls;

internal class InlineNavMenuInteractionHandler : INavMenuInteractionHandler
{
    internal INavMenu? NavMenu { get; private set; }
    
    public void Attach(NavMenuBase navMenu) => AttachCore(navMenu);
    public void Detach(NavMenuBase navMenu) => DetachCore(navMenu);

    internal void AttachCore(INavMenu navMenu)
    {
        if (NavMenu != null)
        {
            throw new NotSupportedException("InlineNavMenuInteractionHandler is already attached.");
        }
        NavMenu                =  navMenu;
        NavMenu.PointerPressed += PointerPressed;
    }

    internal void DetachCore(INavMenu navMenu)
    {
        if (NavMenu != navMenu)
        {
            throw new NotSupportedException("InlineNavMenuInteractionHandler is not attached to the navMenu.");
        }
        NavMenu.PointerPressed -= PointerPressed;
        NavMenu                =  null;
    }
    
    protected virtual void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var item = GetMenuItemCore(e.Source as Control);
        
        if (sender is Visual visual &&
            e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed)
        {
            if (item?.HasSubMenu == true)
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
                if (NavMenu is NavMenu navMenu)
                {
                    navMenu.ClearSelection();
                }
                item?.SelectItemRecursively();
            }
            
            e.Handled = true;
        }
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
}