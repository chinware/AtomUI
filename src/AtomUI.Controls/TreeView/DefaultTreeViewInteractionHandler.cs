using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.Rendering;
using Avalonia.Threading;

namespace AtomUI.Controls;

public class DefaultTreeViewInteractionHandler : ITreeViewInteractionHandler
{
    protected Action<Action, TimeSpan> DelayRun { get; }

    protected IInputManager? InputManager { get; }
    internal TreeView? Menu { get; private set; }
    
    private readonly bool _isContextMenu;
    private IDisposable? _inputManagerSubscription;
    private IRenderRoot? _root;
    private RadioButtonGroupManager? _groupManager;
    
    private static void DefaultDelayRun(Action action, TimeSpan timeSpan)
    {
        DispatcherTimer.RunOnce(action, timeSpan);
    }
    
    public static TimeSpan MenuShowDelay { get; set;} = TimeSpan.FromMilliseconds(400);
    
    public DefaultTreeViewInteractionHandler(bool isContextMenu)
        : this(isContextMenu, DefaultDelayRun)
    {
    }
    
    public DefaultTreeViewInteractionHandler(
        bool isContextMenu,
        Action<Action, TimeSpan> delayRun)
    {
        delayRun = delayRun ?? throw new ArgumentNullException(nameof(delayRun));

        _isContextMenu = isContextMenu;
        InputManager   = AvaloniaLocator.Current.GetService<IInputManager>()!;
        DelayRun       = delayRun;
    }

    public void Attach(TreeView treeView)
    {
        
    }

    public void Detach(TreeView treeView)
    {
        
    }
    
    protected internal virtual void GotFocus(object? sender, GotFocusEventArgs e)
    {
        var item = GetTreeViewItemCore(e.Source as Control);

        if (item?.Parent != null)
        {
           // item.SelectedItem = item;
        }
    }
    
    protected internal virtual void LostFocus(object? sender, RoutedEventArgs e)
    {
        var item = GetTreeViewItemCore(e.Source as Control);

        if (item != null)
        {
           // item.SelectedItem = null;
        }
    }
    
    protected internal virtual void KeyDown(object? sender, KeyEventArgs e)
    {
        KeyDown(GetTreeViewItemCore(e.Source as Control), e);
    }
    
    protected internal virtual void AccessKeyPressed(object? sender, RoutedEventArgs e)
    {
        var item = GetTreeViewItemCore(e.Source as Control);
        if (item is null)
        {
            return;
        }
        
        // if (e is AccessKeyEventArgs { IsMultiple: true })
        // {
        //     // in case we have multiple matches, only focus item and bail
        //     item.Focus(NavigationMethod.Tab);
        //     return;
        // }
        //     
        // if (item.HasSubMenu && item.IsEffectivelyEnabled)
        // {
        //     Open(item, true);
        // }
        // else
        // {
        //     Click(item);
        // }

        e.Handled = true;
    }
    
    internal static TreeViewItem? GetTreeViewItemCore(StyledElement? item)
    {
        while (true)
        {
            if (item == null)
            {
                return null;
            }

            if (item is TreeViewItem treeViewItem)
            {
                return treeViewItem;
            }
                
            item = item.Parent;
        }
    }
    
    protected internal virtual void PointerEntered(object? sender, RoutedEventArgs e)
    {
        var item = GetTreeViewItemCore(e.Source as Control);

        if (item?.Parent == null)
        {
            return;
        }

        // if (item.IsTopLevel)
        // {
        //     if (item != item.Parent.SelectedItem &&
        //         item.Parent.SelectedItem?.IsSubMenuOpen == true)
        //     {
        //         item.Parent.SelectedItem.Close();
        //         SelectItemAndAncestors(item);
        //         if (item.HasSubMenu)
        //             Open(item, false);
        //     }
        //     else
        //     {
        //         SelectItemAndAncestors(item);
        //     }
        // }
        // else
        // {
        //     SelectItemAndAncestors(item);
        //
        //     if (item.HasSubMenu)
        //     {
        //         OpenWithDelay(item);
        //     }
        //     else if (item.Parent != null)
        //     {
        //         foreach (var sibling in item.Parent.SubItems)
        //         {
        //             if (sibling.IsSubMenuOpen)
        //             {
        //                 CloseWithDelay(sibling);
        //             }
        //         }
        //     }
        // }
    }
    
    protected internal virtual void PointerMoved(object? sender, PointerEventArgs e)
    {
        // HACK: #8179 needs to be addressed to correctly implement it in the PointerPressed method.
        // var item = GetTreeViewItemCore(e.Source as Control) as MenuItem;
        //
        // if (item == null)
        //     return;
        //
        // var transformedBounds = item.GetTransformedBounds();
        // if (transformedBounds == null)
        //     return;
        //
        // var point = e.GetCurrentPoint(null);
        //
        // if (point.Properties.IsLeftButtonPressed
        //     && transformedBounds.Value.Contains(point.Position) == false)
        // {
        //     e.Pointer.Capture(null);
        // }
    }
    
    protected internal virtual void PointerExited(object? sender, RoutedEventArgs e)
    {
        // var item = GetMenuItemCore(e.Source as Control);
        //
        // if (item?.Parent == null)
        // {
        //     return;
        // }
        //
        // if (item.Parent.SelectedItem == item)
        // {
        //     if (item.IsTopLevel)
        //     {
        //         if (!((IMenu)item.Parent).IsOpen)
        //         {
        //             item.Parent.SelectedItem = null;
        //         }
        //     }
        //     else if (!item.HasSubMenu)
        //     {
        //         item.Parent.SelectedItem = null;
        //     }
        //     else if (!item.IsPointerOverSubMenu)
        //     {
        //         DelayRun(() =>
        //         {
        //             if (!item.IsPointerOverSubMenu)
        //             {
        //                 item.IsSubMenuOpen = false;
        //             }
        //         }, MenuShowDelay);
        //     }
        // }
    }
    
    protected internal virtual void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // var item = GetMenuItemCore(e.Source as Control);
        //
        // if (sender is Visual visual &&
        //     e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed && item?.HasSubMenu == true)
        // {
        //     if (item.IsSubMenuOpen)
        //     {
        //         // PointerPressed events may bubble from disabled items in sub-menus. In this case,
        //         // keep the sub-menu open.
        //         var popup = (e.Source as ILogical)?.FindLogicalAncestorOfType<Popup>();
        //         if (item.IsTopLevel && popup == null)
        //         {
        //             CloseMenu(item);
        //         }
        //     }
        //     else
        //     {
        //         if (item.IsTopLevel && item.Parent is IMainMenu mainMenu)
        //         {
        //             mainMenu.Open();
        //         }
        //
        //         Open(item, false);
        //     }
        //
        //     e.Handled = true;
        // }
    }

    protected internal virtual void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // var item = GetMenuItemCore(e.Source as Control);
        //
        // if (e.InitialPressMouseButton == MouseButton.Left && item?.HasSubMenu == false)
        // {
        //     Click(item);
        //     e.Handled = true;
        // }
    }
    
    protected internal virtual void MenuOpened(object? sender, RoutedEventArgs e)
    {
        // if (e.Source is Menu)
        // {
        //     Menu?.MoveSelection(NavigationDirection.First, true);
        // }
    }
    
    protected internal virtual void RawInput(RawInputEventArgs e)
    {
        // var mouse = e as RawPointerEventArgs;
        //
        // if (mouse?.Type == RawPointerEventType.NonClientLeftButtonDown)
        // {
        //     Menu?.Close();
        // }
    }
    
    protected internal virtual void RootPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // if (Menu?.IsOpen == true)
        // {
        //     if (e.Source is ILogical control && !Menu.IsLogicalAncestorOf(control))
        //     {
        //         Menu.Close();
        //     }
        // }
    }
    
    private static void AddTreeViewItemToRadioGroup(RadioButtonGroupManager manager, TreeViewItem element)
    {
        // Instead add menu item to the group on attached/detached + ensure checked stated on attached.
        if (element is IRadioButton button)
        {
            manager.Add(button);
        }

        for (var i = 0; i < element.ItemCount; i++)
        {
            var item = element.ContainerFromIndex(i);
            if (item is TreeViewItem treeViewItem)
            {
                AddTreeViewItemToRadioGroup(manager, treeViewItem);
            }
        }
    }
    
    private static void RemoveMenuItemFromRadioGroup(RadioButtonGroupManager manager, TreeViewItem element)
    {
        if (element is IRadioButton button)
        {
            manager.Remove(button, button.GroupName);
        }
        
        for (var i = 0; i < element.ItemCount; i++)
        {
            var item = element.ContainerFromIndex(i);
            if (item is TreeViewItem treeViewItem)
            {
                RemoveMenuItemFromRadioGroup(manager, treeViewItem);
            }
        }
    }
}