using AtomUI.Controls.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Rendering;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class DefaultTreeViewInteractionHandler : ITreeViewInteractionHandler
{
    protected IInputManager? InputManager { get; }
    internal TreeView? TreeView { get; private set; }

    private readonly bool _isFloatingTreeView;
    private IDisposable? _inputManagerSubscription;
    private IRenderRoot? _root;
    private RadioButtonGroupManager? _groupManager;


    public DefaultTreeViewInteractionHandler(bool isFloatingTreeView)
    {
        _isFloatingTreeView = isFloatingTreeView;
        InputManager        = AvaloniaLocator.Current.GetService<IInputManager>()!;
    }

    public void Attach(TreeView treeView)
    {
        TreeView                 =  treeView;
        TreeView.PointerPressed  += PointerPressed;
        TreeView.PointerReleased += PointerReleased;
        _root                    =  TreeView.GetVisualRoot();
        if (_root is not null)
        {
            _groupManager = RadioButtonGroupManager.GetOrCreateForRoot(_root);
            for (var i = 0; i < TreeView.ItemCount; i++)
            {
                if (TreeView.ContainerFromIndex(i) is TreeViewItem item)
                {
                    AddTreeViewItemToRadioGroup(_groupManager, item);
                }
            }
        }
        if (_root is InputElement inputRoot)
        {
            inputRoot.AddHandler(InputElement.PointerPressedEvent, RootPointerPressed, RoutingStrategies.Tunnel);
        }
        _inputManagerSubscription = InputManager?.Process.Subscribe(RawInput);
    }

    public void Detach(TreeView treeView)
    {
        if (TreeView != treeView)
        {
            throw new NotSupportedException("DefaultTreeViewInteractionHandler is not attached to the TreeView.");
        }
        TreeView.PointerPressed  -= PointerPressed;
        TreeView.PointerReleased -= PointerReleased;
        
        if (_root is not null && _groupManager is { } oldManager)
        {
            _groupManager = null;
            for (var i = 0; i < TreeView.ItemCount; i++)
            {
                if (TreeView.ContainerFromIndex(i) is TreeViewItem item)
                {
                    RemoveTreeViewItemToRadioGroup(oldManager, item);
                }
            }
        }
        if (_root is InputElement inputRoot)
        {
            inputRoot.RemoveHandler(InputElement.PointerPressedEvent, RootPointerPressed);
        }

        _inputManagerSubscription?.Dispose();

        TreeView  = null;
        _root = null;
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

    protected internal virtual void PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var item = GetTreeViewItemCore(e.Source as Control);

        if (sender is Visual visual &&
            e.GetCurrentPoint(visual).Properties.IsLeftButtonPressed && item?.ItemCount > 0)
        {
            e.Handled = true;
        }
    }

    protected internal virtual void PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var item = GetTreeViewItemCore(e.Source as Control);

        if (e.InitialPressMouseButton == MouseButton.Left && item?.ItemCount == 0)
        {
            Click(item);
            e.Handled = true;
        }
    }

    internal void Click(TreeViewItem item)
    {
        if (item.ItemCount == 0)
        {
            if (item.ToggleType == ItemToggleType.CheckBox)
            {
                if (item.IsChecked != true)
                {
                    item.IsChecked = false;
                }
                else
                {
                    item.IsChecked = true;
                }
            }
            else if (item.ToggleType == ItemToggleType.Radio && item.IsChecked != true)
            {
                item.IsChecked = true;
            }
        }

        item.RaiseClick();
    }

    protected internal virtual void RawInput(RawInputEventArgs e)
    {
        var mouse = e as RawPointerEventArgs;

        if (mouse?.Type == RawPointerEventType.NonClientLeftButtonDown && _isFloatingTreeView)
        {
            if (TreeView is FloatableTreeView floatableTreeView)
            {
                floatableTreeView.Close();
            }
        }
    }

    protected internal virtual void RootPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (TreeView is FloatableTreeView floatableTreeView)
        {
            if (floatableTreeView.IsOpen)
            {
                if (e.Source is ILogical control && !floatableTreeView.IsLogicalAncestorOf(control))
                {
                    floatableTreeView.Close();
                } 
            }
        }
    }

    internal void OnCheckedChanged(TreeViewItem item)
    {
        if (item is IRadioButton radioButton)
        {
            _groupManager?.OnCheckedChanged(radioButton);
        }
    }

    internal void OnGroupOrTypeChanged(IRadioButton button, string? oldGroupName)
    {
        if (!string.IsNullOrEmpty(oldGroupName))
        {
            _groupManager?.Remove(button, oldGroupName);
        }

        if (!string.IsNullOrEmpty(button.GroupName))
        {
            _groupManager?.Add(button);
        }
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

    private static void RemoveTreeViewItemToRadioGroup(RadioButtonGroupManager manager, TreeViewItem element)
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
                RemoveTreeViewItemToRadioGroup(manager, treeViewItem);
            }
        }
    }
}