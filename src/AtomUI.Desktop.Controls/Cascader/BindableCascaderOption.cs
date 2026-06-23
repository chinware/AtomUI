using System.Collections.Specialized;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

[GenerateScopedResourceHost]
public partial class BindableCascaderOption : AvaloniaObject, ICascaderOption, ISelectTagTextProvider
{
    #region 公共属性定义

    public static readonly DirectProperty<BindableCascaderOption, EntityKey?> ItemKeyProperty =
        AvaloniaProperty.RegisterDirect<BindableCascaderOption, EntityKey?>(
            nameof(ItemKey),
            o => o.ItemKey,
            (o, v) => o.ItemKey = v);

    public static readonly DirectProperty<BindableCascaderOption, object?> HeaderProperty =
        AvaloniaProperty.RegisterDirect<BindableCascaderOption, object?>(
            nameof(Header),
            o => o.Header,
            (o, v) => o.Header = v);

    public static readonly DirectProperty<BindableCascaderOption, PathIcon?> IconProperty =
        AvaloniaProperty.RegisterDirect<BindableCascaderOption, PathIcon?>(
            nameof(Icon),
            o => o.Icon,
            (o, v) => o.Icon = v);

    public static readonly DirectProperty<BindableCascaderOption, bool> IsEnabledProperty =
        AvaloniaProperty.RegisterDirect<BindableCascaderOption, bool>(
            nameof(IsEnabled),
            o => o.IsEnabled,
            (o, v) => o.IsEnabled = v);

    public static readonly DirectProperty<BindableCascaderOption, bool?> IsCheckedProperty =
        AvaloniaProperty.RegisterDirect<BindableCascaderOption, bool?>(
            nameof(IsChecked),
            o => o.IsChecked,
            (o, v) => o.IsChecked = v);

    public static readonly DirectProperty<BindableCascaderOption, bool> IsExpandedProperty =
        AvaloniaProperty.RegisterDirect<BindableCascaderOption, bool>(
            nameof(IsExpanded),
            o => o.IsExpanded,
            (o, v) => o.IsExpanded = v);

    public static readonly DirectProperty<BindableCascaderOption, bool> IsCheckBoxEnabledProperty =
        AvaloniaProperty.RegisterDirect<BindableCascaderOption, bool>(
            nameof(IsCheckBoxEnabled),
            o => o.IsCheckBoxEnabled,
            (o, v) => o.IsCheckBoxEnabled = v);

    public static readonly DirectProperty<BindableCascaderOption, bool> IsLeafProperty =
        AvaloniaProperty.RegisterDirect<BindableCascaderOption, bool>(
            nameof(IsLeaf),
            o => o.IsLeaf,
            (o, v) => o.IsLeaf = v);

    public static readonly DirectProperty<BindableCascaderOption, object?> ValueProperty =
        AvaloniaProperty.RegisterDirect<BindableCascaderOption, object?>(
            nameof(Value),
            o => o.Value,
            (o, v) => o.Value = v);

    public static readonly DirectProperty<BindableCascaderOption, IList<ICascaderOption>> ChildrenProperty =
        AvaloniaProperty.RegisterDirect<BindableCascaderOption, IList<ICascaderOption>>(
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

    private bool _isExpanded;

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetAndRaise(IsExpandedProperty, ref _isExpanded, value);
    }

    private bool _isCheckBoxEnabled = true;

    public bool IsCheckBoxEnabled
    {
        get => _isCheckBoxEnabled;
        set => SetAndRaise(IsCheckBoxEnabledProperty, ref _isCheckBoxEnabled, value);
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

    private IList<ICascaderOption> _children = new AvaloniaList<ICascaderOption>();
    private readonly List<ICascaderOption> _attachedChildren = [];

    [Content]
    public IList<ICascaderOption> Children
    {
        get => _children;
        set => SetChildren(value);
    }

    #endregion

    public ITreeNode<ICascaderOption>? ParentNode { get; private set; }

    string? ISelectTagTextProvider.TagText => Header?.ToString();

    IEnumerable<ICascaderOption> ITreeNode<ICascaderOption>.Children => Children;

    public BindableCascaderOption()
    {
        AttachChildren(Children);
    }

    public void UpdateParentNode(ICascaderOption? parentNode)
    {
        ParentNode = parentNode;
    }

    private void SetChildren(IList<ICascaderOption>? children)
    {
        children ??= new AvaloniaList<ICascaderOption>();
        if (ReferenceEquals(_children, children))
        {
            return;
        }

        DetachChildren(_children);
        SetAndRaise(ChildrenProperty, ref _children, children);
        AttachChildren(_children);
    }

    private void AttachChildren(IList<ICascaderOption> children)
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

    private void DetachChildren(IList<ICascaderOption> children)
    {
        if (children is INotifyCollectionChanged notifyCollectionChanged)
        {
            notifyCollectionChanged.CollectionChanged -= HandleChildrenCollectionChanged;
        }

        DetachAttachedChildren();
    }

    private void AttachChild(ICascaderOption child)
    {
        child.UpdateParentNode(this);
        if (!_attachedChildren.Contains(child))
        {
            _attachedChildren.Add(child);
        }
    }

    private void DetachChild(ICascaderOption child)
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
                if (e.OldItems[i] is ICascaderOption option)
                {
                    DetachChild(option);
                }
            }
        }

        if (e.NewItems != null)
        {
            for (var i = 0; i < e.NewItems.Count; i++)
            {
                if (e.NewItems[i] is ICascaderOption option)
                {
                    AttachChild(option);
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
