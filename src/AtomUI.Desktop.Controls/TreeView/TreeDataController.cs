using System.Collections;
using System.Collections.Specialized;
using AtomUI.Controls.AsyncLoad;
using Avalonia;

namespace AtomUI.Desktop.Controls;

internal readonly record struct TreeDropOperationResult(
    TreeViewItem DraggedViewItem,
    TreeViewItem? DroppedItem,
    int DropIndex);

internal sealed class TreeDataController
{
    private readonly TreeView _owner;
    private readonly TreeNodeIndex _nodeIndex = new();
    private readonly HashSet<ITreeItemNode> _movingNodes = new(ReferenceEqualityComparer<ITreeItemNode>.Instance);

    public TreeDataController(TreeView owner)
    {
        _owner = owner;
    }

    public void SetRootSource(IEnumerable? rootSource)
    {
        _nodeIndex.SetRootSource(rootSource);
    }

    public void ClearRootSource()
    {
        _nodeIndex.Clear();
    }

    public bool IsMovingNode(ITreeItemNode node)
    {
        return _movingNodes.Contains(node);
    }

    public bool TryMove(TreeViewItem sourceContainer, DropTargetInfo targetInfo, out TreeDropOperationResult result)
    {
        result = default;

        var sourceNode = _owner.ResolveTreeItemNode(sourceContainer);
        if (sourceNode is null ||
            !_nodeIndex.TryGetContext(sourceNode, out var sourceContext) ||
            !MutableTreeNodeCollection.TryCreate(sourceContext.Collection, out var sourceCollection) ||
            !sourceCollection.CanWrite)
        {
            return false;
        }

        if (!TryResolveTargetCollection(targetInfo, out var targetCollection, out var targetParentNode, out var droppedContainer))
        {
            return false;
        }

        if (!targetCollection.CanWrite ||
            targetParentNode is not null && _nodeIndex.IsSelfOrDescendantOf(targetParentNode, sourceNode))
        {
            return false;
        }

        var sourceIndex = sourceCollection.IndexOf(sourceNode);
        if (sourceIndex < 0)
        {
            return false;
        }

        var targetIndex = Math.Clamp(targetInfo.Index, 0, targetCollection.Count);
        var isSameCollection = ReferenceEquals(sourceCollection.Identity, targetCollection.Identity);
        if (isSameCollection && sourceIndex < targetIndex)
        {
            targetIndex--;
        }

        if (isSameCollection && sourceIndex == targetIndex)
        {
            return false;
        }

        var moveState = CaptureMovedNodeState(sourceNode);
        using (BeginMove(sourceNode))
        {
            sourceCollection.RemoveAt(sourceIndex);
            targetCollection.Insert(targetIndex, sourceNode);
            sourceNode.UpdateParentNode(targetParentNode);
            _nodeIndex.SyncMovedNode(
                sourceNode,
                sourceCollection.Identity,
                targetCollection.Identity,
                targetParentNode);
            RestoreMovedNodeState(sourceNode, moveState);
        }

        result = new TreeDropOperationResult(sourceContainer, droppedContainer, targetInfo.Index);
        return true;
    }

    public bool TryAppendChildren(ITreeItemNode parentNode, IReadOnlyList<ITreeItemNode> children)
    {
        if (children.Count == 0 ||
            !MutableTreeNodeCollection.TryCreate(parentNode.Children, out var collection) ||
            !collection.CanWrite)
        {
            return false;
        }

        for (var i = 0; i < children.Count; i++)
        {
            collection.Insert(collection.Count, children[i]);
            children[i].UpdateParentNode(parentNode);
        }

        _nodeIndex.SyncCollection(collection.Identity, parentNode);
        return true;
    }

    private bool TryResolveTargetCollection(
        DropTargetInfo targetInfo,
        out MutableTreeNodeCollection targetCollection,
        out ITreeItemNode? targetParentNode,
        out TreeViewItem? droppedContainer)
    {
        targetCollection = default;
        targetParentNode = null;
        droppedContainer = null;

        if (targetInfo.IsRoot)
        {
            return MutableTreeNodeCollection.TryCreate(_nodeIndex.RootSource, out targetCollection);
        }

        if (targetInfo.TargetTreeItem is null)
        {
            return false;
        }

        targetParentNode = _owner.ResolveTreeItemNode(targetInfo.TargetTreeItem);
        if (targetParentNode is null)
        {
            return false;
        }

        droppedContainer = targetInfo.TargetTreeItem;
        return MutableTreeNodeCollection.TryCreate(targetParentNode.Children, out targetCollection);
    }

    private IDisposable BeginMove(ITreeItemNode node)
    {
        _movingNodes.Add(node);
        return new MoveScope(this, node);
    }

    private MoveNodeState CaptureMovedNodeState(ITreeItemNode node)
    {
        return new MoveNodeState(
            ReferenceEquals(_owner.SelectedItem, node),
            _owner.SelectedItems.Contains(node),
            _owner.CheckedItems.Contains(node));
    }

    private void RestoreMovedNodeState(ITreeItemNode node, MoveNodeState moveState)
    {
        if ((moveState.IsSelectedItem || node.IsSelected) &&
            !ReferenceEquals(_owner.SelectedItem, node))
        {
            _owner.SetCurrentValue(TreeView.SelectedItemProperty, node);
        }

        if ((moveState.IsInSelectedItems || node.IsSelected) &&
            !_owner.SelectedItems.Contains(node))
        {
            _owner.SelectedItems.Add(node);
        }

        if ((moveState.IsInCheckedItems || node.IsChecked == true) &&
            !_owner.CheckedItems.Contains(node))
        {
            _owner.CheckedItems.Add(node);
        }
    }

    private readonly record struct MoveNodeState(
        bool IsSelectedItem,
        bool IsInSelectedItems,
        bool IsInCheckedItems);

    private sealed class MoveScope : IDisposable
    {
        private readonly TreeDataController _owner;
        private readonly ITreeItemNode _node;

        public MoveScope(TreeDataController owner, ITreeItemNode node)
        {
            _owner = owner;
            _node  = node;
        }

        public void Dispose()
        {
            _owner._movingNodes.Remove(_node);
        }
    }
}

internal readonly struct MutableTreeNodeCollection
{
    private readonly IList? _list;
    private readonly IList<ITreeItemNode>? _genericList;

    public object Identity { get; }

    public int Count => _genericList?.Count ?? _list?.Count ?? 0;

    public bool CanWrite { get; }

    private MutableTreeNodeCollection(object identity, IList list)
    {
        Identity     = identity;
        _list        = list;
        _genericList = null;
        CanWrite     = !list.IsReadOnly && !list.IsFixedSize;
    }

    private MutableTreeNodeCollection(object identity, IList<ITreeItemNode> list)
    {
        Identity     = identity;
        _list        = null;
        _genericList = list;
        CanWrite     = !list.IsReadOnly;
    }

    public static bool TryCreate(object? source, out MutableTreeNodeCollection collection)
    {
        switch (source)
        {
            case IList list:
                collection = new MutableTreeNodeCollection(source, list);
                return true;
            case IList<ITreeItemNode> genericList:
                collection = new MutableTreeNodeCollection(source, genericList);
                return true;
            default:
                collection = default;
                return false;
        }
    }

    public int IndexOf(ITreeItemNode node)
    {
        return _genericList?.IndexOf(node) ?? _list?.IndexOf(node) ?? -1;
    }

    public void RemoveAt(int index)
    {
        if (_genericList is not null)
        {
            _genericList.RemoveAt(index);
            return;
        }

        _list!.RemoveAt(index);
    }

    public void Insert(int index, ITreeItemNode node)
    {
        if (_genericList is not null)
        {
            _genericList.Insert(index, node);
            return;
        }

        _list!.Insert(index, node);
    }
}

internal readonly record struct TreeNodeIndexContext(
    ITreeItemNode Node,
    ITreeItemNode? ParentNode,
    object Collection,
    int Index);

internal sealed class TreeNodeIndex
{
    private readonly Dictionary<ITreeItemNode, TreeNodeIndexContext> _contexts =
        new(ReferenceEqualityComparer<ITreeItemNode>.Instance);

    private readonly Dictionary<object, CollectionSubscription> _collectionSubscriptions =
        new(ReferenceEqualityComparer<object>.Instance);

    private readonly HashSet<BindableTreeItemNode> _bindableNodes =
        new(ReferenceEqualityComparer<BindableTreeItemNode>.Instance);

    public IEnumerable? RootSource { get; private set; }

    public void SetRootSource(IEnumerable? rootSource)
    {
        Clear();
        RootSource = rootSource;
        if (RootSource is not null)
        {
            AttachCollection(RootSource, null);
        }
    }

    public bool TryGetContext(ITreeItemNode node, out TreeNodeIndexContext context)
    {
        return _contexts.TryGetValue(node, out context);
    }

    public bool IsSelfOrDescendantOf(ITreeItemNode candidate, ITreeItemNode ancestor)
    {
        var current = candidate;
        while (current is not null)
        {
            if (ReferenceEquals(current, ancestor))
            {
                return true;
            }

            current = current.ParentNode as ITreeItemNode;
        }

        return false;
    }

    public void SyncMovedNode(
        ITreeItemNode node,
        object sourceCollection,
        object targetCollection,
        ITreeItemNode? targetParentNode)
    {
        DetachNode(node);
        var targetIndex = IndexOf(targetCollection, node);
        if (targetIndex >= 0)
        {
            AttachNode(node, targetParentNode, targetCollection, targetIndex);
        }
        UpdateCollectionIndexes(sourceCollection, 0);
        if (!ReferenceEquals(sourceCollection, targetCollection))
        {
            UpdateCollectionIndexes(targetCollection, 0);
        }
    }

    public void SyncCollection(object collection, ITreeItemNode? parentNode)
    {
        AttachCollection(collection, parentNode);
        UpdateCollectionIndexes(collection, 0);
    }

    public void Clear()
    {
        foreach (var subscription in _collectionSubscriptions.Values.ToArray())
        {
            subscription.Dispose();
        }
        _collectionSubscriptions.Clear();

        foreach (var node in _bindableNodes.ToArray())
        {
            node.PropertyChanged -= HandleBindableNodePropertyChanged;
        }
        _bindableNodes.Clear();
        _contexts.Clear();
        RootSource = null;
    }

    private void AttachCollection(object collection, ITreeItemNode? parentNode)
    {
        if (collection is not IEnumerable items)
        {
            return;
        }

        if (collection is INotifyCollectionChanged notifyCollectionChanged &&
            !_collectionSubscriptions.ContainsKey(collection))
        {
            var subscription = new CollectionSubscription(this, collection, parentNode, notifyCollectionChanged);
            _collectionSubscriptions.Add(collection, subscription);
        }

        AttachItems(items, parentNode, collection);
    }

    private void AttachItems(IEnumerable items, ITreeItemNode? parentNode, object collection)
    {
        var index = 0;
        foreach (var item in items)
        {
            if (item is ITreeItemNode node)
            {
                AttachNode(node, parentNode, collection, index);
            }
            index++;
        }
    }

    private void AttachNode(ITreeItemNode node, ITreeItemNode? parentNode, object collection, int index)
    {
        _contexts[node] = new TreeNodeIndexContext(node, parentNode, collection, index);
        AttachBindableNode(node);
        AttachCollection(node.Children, node);
    }

    private void DetachCollection(object collection)
    {
        if (_collectionSubscriptions.Remove(collection, out var subscription))
        {
            subscription.Dispose();
        }

        if (collection is not IEnumerable items)
        {
            return;
        }

        foreach (var item in items.OfType<ITreeItemNode>().ToArray())
        {
            DetachNode(item);
        }
    }

    private void DetachNode(ITreeItemNode node)
    {
        if (!_contexts.Remove(node))
        {
            return;
        }

        DetachBindableNode(node);
        DetachCollection(node.Children);
    }

    private void AttachBindableNode(ITreeItemNode node)
    {
        if (node is not BindableTreeItemNode bindableNode ||
            !_bindableNodes.Add(bindableNode))
        {
            return;
        }

        bindableNode.PropertyChanged += HandleBindableNodePropertyChanged;
    }

    private void DetachBindableNode(ITreeItemNode node)
    {
        if (node is not BindableTreeItemNode bindableNode ||
            !_bindableNodes.Remove(bindableNode))
        {
            return;
        }

        bindableNode.PropertyChanged -= HandleBindableNodePropertyChanged;
    }

    private void HandleBindableNodePropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is not BindableTreeItemNode node ||
            e.Property != BindableTreeItemNode.ChildrenProperty ||
            !_contexts.TryGetValue(node, out _))
        {
            return;
        }

        var oldChildren = e.GetOldValue<IList<ITreeItemNode>>();
        if (oldChildren is not null)
        {
            DetachCollection(oldChildren);
        }

        AttachCollection(node.Children, node);
    }

    private void HandleCollectionChanged(CollectionSubscription subscription, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AttachNewItems(subscription, e.NewItems, e.NewStartingIndex);
                UpdateCollectionIndexes(subscription.Collection, e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Remove:
                DetachOldItems(e.OldItems);
                UpdateCollectionIndexes(subscription.Collection, e.OldStartingIndex);
                break;
            case NotifyCollectionChangedAction.Replace:
                DetachOldItems(e.OldItems);
                AttachNewItems(subscription, e.NewItems, e.NewStartingIndex);
                UpdateCollectionIndexes(subscription.Collection, e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Move:
                UpdateCollectionIndexes(subscription.Collection, Math.Min(e.OldStartingIndex, e.NewStartingIndex));
                break;
            case NotifyCollectionChangedAction.Reset:
                SetRootSource(RootSource);
                break;
        }
    }

    private void AttachNewItems(CollectionSubscription subscription, IList? items, int startIndex)
    {
        if (items is null)
        {
            return;
        }

        var index = Math.Max(startIndex, 0);
        for (var i = 0; i < items.Count; i++)
        {
            if (items[i] is ITreeItemNode node)
            {
                AttachNode(node, subscription.ParentNode, subscription.Collection, index + i);
            }
        }
    }

    private void DetachOldItems(IList? items)
    {
        if (items is null)
        {
            return;
        }

        for (var i = 0; i < items.Count; i++)
        {
            if (items[i] is ITreeItemNode node)
            {
                DetachNode(node);
            }
        }
    }

    private void UpdateCollectionIndexes(object collection, int startIndex)
    {
        if (collection is not IEnumerable items)
        {
            return;
        }

        var index = 0;
        var start = Math.Max(startIndex, 0);
        foreach (var item in items)
        {
            if (index >= start &&
                item is ITreeItemNode node &&
                _contexts.TryGetValue(node, out var context))
            {
                _contexts[node] = context with { Index = index };
            }
            index++;
        }
    }

    private static int IndexOf(object collection, ITreeItemNode node)
    {
        return collection switch
        {
            IList<ITreeItemNode> list => list.IndexOf(node),
            IList list               => list.IndexOf(node),
            IEnumerable items        => IndexOf(items, node),
            _                        => -1
        };
    }

    private static int IndexOf(IEnumerable items, ITreeItemNode node)
    {
        var index = 0;
        foreach (var item in items)
        {
            if (ReferenceEquals(item, node))
            {
                return index;
            }
            index++;
        }

        return -1;
    }

    private sealed class CollectionSubscription : IDisposable
    {
        private readonly TreeNodeIndex _owner;
        private readonly INotifyCollectionChanged _collectionChanged;

        public object Collection { get; }
        public ITreeItemNode? ParentNode { get; }

        public CollectionSubscription(
            TreeNodeIndex owner,
            object collection,
            ITreeItemNode? parentNode,
            INotifyCollectionChanged collectionChanged)
        {
            _owner             = owner;
            Collection         = collection;
            ParentNode         = parentNode;
            _collectionChanged = collectionChanged;
            _collectionChanged.CollectionChanged += HandleCollectionChanged;
        }

        public void Dispose()
        {
            _collectionChanged.CollectionChanged -= HandleCollectionChanged;
        }

        private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _owner.HandleCollectionChanged(this, e);
        }
    }
}
