using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace AtomUI.Controls;

using NavMenuControl = NavMenu;

internal class InlineNavMenuInteractionHandler : INavMenuInteractionHandler
{
    internal INavMenu? NavMenu { get; private set; }
    
    public void Attach(NavMenuBase navMenu) => AttachCore(navMenu);
    public void Detach(NavMenuBase navMenu) => DetachCore(navMenu);

    private bool _currentPressedIsValid = false;
    private StyledElement? _latestSelectedItem = null;

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
                if (!ReferenceEquals(_latestSelectedItem, item))
                {
                    if (_latestSelectedItem != null)
                    {
                        var ancestorInfo = HasCommonAncestor(_latestSelectedItem, item);
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
                    
                    item.SelectItemRecursively();
                }
                _latestSelectedItem = item;
            }
            
            e.Handled = true;
        }
    }

    private (bool, StyledElement?) HasCommonAncestor(StyledElement lhs, StyledElement rhs)
    {
        var lhsAncestors = CollectAncestors(lhs);
        var rhsAncestors = CollectAncestors(rhs);
        var hasOverlaps = lhsAncestors.ToHashSet().Overlaps(rhsAncestors.ToHashSet());
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
                if (object.ReferenceEquals(lhsAncestor, rhsAncestor))
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