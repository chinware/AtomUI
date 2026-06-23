using System.Collections.Specialized;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

[GenerateScopedResourceHost]
public partial class BindableTreeItemNode : AvaloniaObject, ITreeItemNode, ISelectTagTextProvider
{
    #region 公共属性定义

    public static readonly DirectProperty<BindableTreeItemNode, EntityKey?> ItemKeyProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, EntityKey?>(
            nameof(ItemKey),
            o => o.ItemKey,
            (o, v) => o.ItemKey = v);

    public static readonly DirectProperty<BindableTreeItemNode, object?> HeaderProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, object?>(
            nameof(Header),
            o => o.Header,
            (o, v) => o.Header = v);

    public static readonly DirectProperty<BindableTreeItemNode, PathIcon?> IconProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, PathIcon?>(
            nameof(Icon),
            o => o.Icon,
            (o, v) => o.Icon = v);

    public static readonly DirectProperty<BindableTreeItemNode, bool> IsEnabledProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, bool>(
            nameof(IsEnabled),
            o => o.IsEnabled,
            (o, v) => o.IsEnabled = v);

    public static readonly DirectProperty<BindableTreeItemNode, bool?> IsCheckedProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, bool?>(
            nameof(IsChecked),
            o => o.IsChecked,
            (o, v) => o.IsChecked = v);

    public static readonly DirectProperty<BindableTreeItemNode, bool> IsSelectedProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, bool>(
            nameof(IsSelected),
            o => o.IsSelected,
            (o, v) => o.IsSelected = v);

    public static readonly DirectProperty<BindableTreeItemNode, bool> IsExpandedProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, bool>(
            nameof(IsExpanded),
            o => o.IsExpanded,
            (o, v) => o.IsExpanded = v);

    public static readonly DirectProperty<BindableTreeItemNode, bool> IsIndicatorEnabledProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, bool>(
            nameof(IsIndicatorEnabled),
            o => o.IsIndicatorEnabled,
            (o, v) => o.IsIndicatorEnabled = v);

    public static readonly DirectProperty<BindableTreeItemNode, string?> GroupNameProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, string?>(
            nameof(GroupName),
            o => o.GroupName,
            (o, v) => o.GroupName = v);

    public static readonly DirectProperty<BindableTreeItemNode, bool> IsLeafProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, bool>(
            nameof(IsLeaf),
            o => o.IsLeaf,
            (o, v) => o.IsLeaf = v);

    public static readonly DirectProperty<BindableTreeItemNode, object?> ValueProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, object?>(
            nameof(Value),
            o => o.Value,
            (o, v) => o.Value = v);

    public static readonly DirectProperty<BindableTreeItemNode, IList<ITreeItemNode>> ChildrenProperty =
        AvaloniaProperty.RegisterDirect<BindableTreeItemNode, IList<ITreeItemNode>>(
            nameof(Children),
            o => o.Children,
            (o, v) => o.Children = v);

    private EntityKey? _itemKey;

    public EntityKey? ItemKey
    {
        get => _itemKey;
        set => SetAndRaise(ItemKeyProperty, ref _itemKey, value);
    }

    private object? _header;

    public object? Header
    {
        get => _header;
        set => SetAndRaise(HeaderProperty, ref _header, value);
    }

    private PathIcon? _icon;

    public PathIcon? Icon
    {
        get => _icon;
        set => SetAndRaise(IconProperty, ref _icon, value);
    }

    private bool _isEnabled = true;

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetAndRaise(IsEnabledProperty, ref _isEnabled, value);
    }

    private bool? _isChecked = false;

    public bool? IsChecked
    {
        get => _isChecked;
        set => SetAndRaise(IsCheckedProperty, ref _isChecked, value);
    }

    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set => SetAndRaise(IsSelectedProperty, ref _isSelected, value);
    }

    private bool _isExpanded;

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetAndRaise(IsExpandedProperty, ref _isExpanded, value);
    }

    private bool _isIndicatorEnabled = true;

    public bool IsIndicatorEnabled
    {
        get => _isIndicatorEnabled;
        set => SetAndRaise(IsIndicatorEnabledProperty, ref _isIndicatorEnabled, value);
    }

    private string? _groupName;

    public string? GroupName
    {
        get => _groupName;
        set => SetAndRaise(GroupNameProperty, ref _groupName, value);
    }

    private bool _isLeaf;

    public bool IsLeaf
    {
        get => _isLeaf;
        set => SetAndRaise(IsLeafProperty, ref _isLeaf, value);
    }

    private object? _value;

    public object? Value
    {
        get => _value;
        set => SetAndRaise(ValueProperty, ref _value, value);
    }

    private IList<ITreeItemNode> _children = new AvaloniaList<ITreeItemNode>();
    private readonly List<ITreeItemNode> _attachedChildren = [];

    [Content]
    public IList<ITreeItemNode> Children
    {
        get => _children;
        set => SetChildren(value);
    }

    #endregion

    public ITreeNode<ITreeItemNode>? ParentNode { get; private set; }

    string? ISelectTagTextProvider.TagText => Header?.ToString();

    IEnumerable<ITreeItemNode> ITreeNode<ITreeItemNode>.Children => Children;

    public BindableTreeItemNode()
    {
        AttachChildren(Children);
    }

    public void UpdateParentNode(ITreeItemNode? parentNode)
    {
        ParentNode = parentNode;
    }

    private void SetChildren(IList<ITreeItemNode>? children)
    {
        children ??= new AvaloniaList<ITreeItemNode>();
        if (ReferenceEquals(_children, children))
        {
            return;
        }

        DetachChildren(_children);
        SetAndRaise(ChildrenProperty, ref _children, children);
        AttachChildren(_children);
    }

    private void AttachChildren(IList<ITreeItemNode> children)
    {
        if (children is INotifyCollectionChanged notifyCollectionChanged)
        {
            notifyCollectionChanged.CollectionChanged += HandleChildrenCollectionChanged;
        }

        for (var i = 0; i < children.Count; i++)
        {
            AttachChild(children[i]);
        }
    }

    private void DetachChildren(IList<ITreeItemNode> children)
    {
        if (children is INotifyCollectionChanged notifyCollectionChanged)
        {
            notifyCollectionChanged.CollectionChanged -= HandleChildrenCollectionChanged;
        }

        DetachAttachedChildren();
    }

    private void AttachChild(ITreeItemNode child)
    {
        child.UpdateParentNode(this);
        if (!_attachedChildren.Contains(child))
        {
            _attachedChildren.Add(child);
        }
    }

    private void DetachChild(ITreeItemNode child)
    {
        if (ReferenceEquals(child.ParentNode, this))
        {
            child.UpdateParentNode(null);
        }

        _attachedChildren.Remove(child);
    }

    private void DetachAttachedChildren()
    {
        for (var i = 0; i < _attachedChildren.Count; i++)
        {
            if (ReferenceEquals(_attachedChildren[i].ParentNode, this))
            {
                _attachedChildren[i].UpdateParentNode(null);
            }
        }

        _attachedChildren.Clear();
    }

    private void HandleChildrenCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            for (var i = 0; i < e.OldItems.Count; i++)
            {
                if (e.OldItems[i] is ITreeItemNode treeNode)
                {
                    DetachChild(treeNode);
                }
            }
        }

        if (e.NewItems != null)
        {
            for (var i = 0; i < e.NewItems.Count; i++)
            {
                if (e.NewItems[i] is ITreeItemNode treeNode)
                {
                    AttachChild(treeNode);
                }
            }
        }

        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            DetachAttachedChildren();
            for (var i = 0; i < Children.Count; i++)
            {
                AttachChild(Children[i]);
            }
        }
    }
}
