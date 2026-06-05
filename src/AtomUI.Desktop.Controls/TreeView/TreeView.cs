using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Controls.Utils;
using AtomUI.MotionScene;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

using AvaloniaTreeView = Avalonia.Controls.TreeView;

public enum TreeItemHoverMode
{
    Default,
    Block,
    WholeLine
}

[Flags]
public enum TreeFilterStrategy
{
    None = 0,
    HighlightedMatch = 0x01,
    HighlightedWhole = 0x02,
    BoldedMatch = 0x04,
    ExpandPath = 0x08,
    HideUnMatched = 0x10,
    MatchedOnly = HighlightedMatch | BoldedMatch | ExpandPath | HideUnMatched,
    FullTree = HighlightedMatch | BoldedMatch | ExpandPath,
    All = MatchedOnly
}

[PseudoClasses(StdPseudoClass.Draggable)]
public partial class TreeView : AvaloniaTreeView, 
                                IMotionAwareControl,
                                IFormItemAware
{
    #region 公共属性定义
    public static readonly StyledProperty<bool> IsAutoExpandParentProperty =
        AvaloniaProperty.Register<TreeView, bool>(nameof(IsAutoExpandParent), true);
    
    public static readonly StyledProperty<bool> IsDraggableProperty =
        AvaloniaProperty.Register<TreeView, bool>(nameof(IsDraggable));

    public static readonly StyledProperty<bool> IsShowIconProperty =
        AvaloniaProperty.Register<TreeView, bool>(nameof(IsShowIcon));

    public static readonly StyledProperty<bool> IsShowLineProperty =
        AvaloniaProperty.Register<TreeView, bool>(nameof(IsShowLine));
    
    public static readonly StyledProperty<bool> IsDefaultExpandAllProperty =
        AvaloniaProperty.Register<TreeView, bool>(
            nameof(IsDefaultExpandAll));

    public static readonly StyledProperty<TreeItemHoverMode> NodeHoverModeProperty =
        AvaloniaProperty.Register<TreeView, TreeItemHoverMode>(nameof(NodeHoverMode), TreeItemHoverMode.Default);
    
    public static readonly StyledProperty<IconTemplate?> SwitcherExpandIconProperty =
        AvaloniaProperty.Register<TreeView, IconTemplate?>(nameof(SwitcherExpandIcon));

    public static readonly StyledProperty<IconTemplate?> SwitcherCollapseIconProperty =
        AvaloniaProperty.Register<TreeView, IconTemplate?>(nameof(SwitcherCollapseIcon));

    public static readonly StyledProperty<IconTemplate?> SwitcherRotationIconProperty =
        AvaloniaProperty.Register<TreeView, IconTemplate?>(nameof(SwitcherRotationIcon));

    public static readonly StyledProperty<IconTemplate?> SwitcherLoadingIconProperty =
        AvaloniaProperty.Register<TreeView, IconTemplate?>(nameof(SwitcherLoadingIcon));

    public static readonly StyledProperty<IconTemplate?> SwitcherLeafIconProperty =
        AvaloniaProperty.Register<TreeView, IconTemplate?>(nameof(SwitcherLeafIcon));

    public static readonly StyledProperty<bool> IsShowLeafIconProperty =
        AvaloniaProperty.Register<TreeView, bool>(nameof(IsShowLeafIcon));
    
    public static readonly StyledProperty<bool> IsSwitcherRotationProperty = 
        AvaloniaProperty.Register<TreeView, bool>(nameof(IsSwitcherRotation), true);
    
    public static readonly StyledProperty<bool> IsSelectableProperty =
        AvaloniaProperty.Register<TreeView, bool>(nameof(IsSelectable), true);

    public static readonly StyledProperty<bool> IsSelectOnRightClickProperty =
        AvaloniaProperty.Register<TreeView, bool>(nameof(IsSelectOnRightClick), true);

    public static readonly StyledProperty<bool> IsCheckStrictlyProperty = 
        AvaloniaProperty.Register<TreeView, bool>(nameof(IsCheckStrictly), false);

    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<TreeView>();
    
    public static readonly StyledProperty<AbstractMotion?> OpenMotionProperty = 
        Popup.OpenMotionProperty.AddOwner<TreeView>();
        
    public static readonly StyledProperty<AbstractMotion?> CloseMotionProperty = 
        Popup.CloseMotionProperty.AddOwner<TreeView>();
    
    public static readonly StyledProperty<ItemToggleType> ToggleTypeProperty =
        AvaloniaProperty.Register<TreeView, ItemToggleType>(nameof(ToggleType), ItemToggleType.None);
    
    public static readonly DirectProperty<TreeView, IList<TreeNodePath>?> DefaultCheckedPathsProperty =
        AvaloniaProperty.RegisterDirect<TreeView, IList<TreeNodePath>?>(
            nameof(DefaultCheckedPaths),
            o => o.DefaultCheckedPaths,
            (o, v) => o.DefaultCheckedPaths = v);
    
    public static readonly DirectProperty<TreeView, IList<TreeNodePath>?> DefaultSelectedPathsProperty =
        AvaloniaProperty.RegisterDirect<TreeView, IList<TreeNodePath>?>(
            nameof(DefaultSelectedPaths),
            o => o.DefaultSelectedPaths,
            (o, v) => o.DefaultSelectedPaths = v);
    
    public static readonly DirectProperty<TreeView, IList<TreeNodePath>?> DefaultExpandedPathsProperty =
        AvaloniaProperty.RegisterDirect<TreeView, IList<TreeNodePath>?>(
            nameof(DefaultExpandedPaths),
            o => o.DefaultExpandedPaths,
            (o, v) => o.DefaultExpandedPaths = v);
    
    public static readonly StyledProperty<ITreeItemNodeLoader?> DataLoaderProperty =
        AvaloniaProperty.Register<TreeView, ITreeItemNodeLoader?>(nameof(DataLoader));
    
    public static readonly StyledProperty<IValueFilter?> FilterProperty =
        AvaloniaProperty.Register<TreeView, IValueFilter?>(nameof(Filter));

    public static readonly StyledProperty<object?> FilterValueProperty =
        AvaloniaProperty.Register<TreeView, object?>(nameof(FilterValue));

    public static readonly StyledProperty<DefaultFilterValueSelector?> FilterValueSelectorProperty =
        AvaloniaProperty.Register<TreeView, DefaultFilterValueSelector?>(nameof(FilterValueSelector));
    
    public static readonly StyledProperty<TreeFilterStrategy> FilterStrategyProperty =
        AvaloniaProperty.Register<TreeView, TreeFilterStrategy>(nameof(FilterStrategy), TreeFilterStrategy.All);
    
    public static readonly DirectProperty<TreeView, int> FilterResultCountProperty =
        AvaloniaProperty.RegisterDirect<TreeView, int>(nameof(FilterResultCount),
            o => o.FilterResultCount);
    
    public static readonly StyledProperty<IBrush?> FilterHighlightForegroundProperty =
        AvaloniaProperty.Register<TreeView, IBrush?>(nameof(FilterHighlightForeground));
    
    public static readonly StyledProperty<object?> EmptyIndicatorProperty =
        AvaloniaProperty.Register<TreeView, object?>(nameof(EmptyIndicator));
    
    public static readonly StyledProperty<IDataTemplate?> EmptyIndicatorTemplateProperty =
        AvaloniaProperty.Register<TreeView, IDataTemplate?>(nameof(EmptyIndicatorTemplate));
    
    public static readonly StyledProperty<bool> IsShowEmptyIndicatorProperty =
        AvaloniaProperty.Register<TreeView, bool>(nameof(IsShowEmptyIndicator), true);
    
    public static readonly StyledProperty<Thickness> EmptyIndicatorPaddingProperty =
        AvaloniaProperty.Register<TreeView, Thickness>(nameof(EmptyIndicatorPadding));
    
    public bool IsAutoExpandParent
    {
        get => GetValue(IsAutoExpandParentProperty);
        set => SetValue(IsAutoExpandParentProperty, value);
    }

    public bool IsDraggable
    {
        get => GetValue(IsDraggableProperty);
        set => SetValue(IsDraggableProperty, value);
    }
    
    public bool IsShowIcon
    {
        get => GetValue(IsShowIconProperty);
        set => SetValue(IsShowIconProperty, value);
    }

    public bool IsShowLine
    {
        get => GetValue(IsShowLineProperty);
        set => SetValue(IsShowLineProperty, value);
    }
    
    public bool IsDefaultExpandAll
    {
        get => GetValue(IsDefaultExpandAllProperty);
        set => SetValue(IsDefaultExpandAllProperty, value);
    }

    public TreeItemHoverMode NodeHoverMode
    {
        get => GetValue(NodeHoverModeProperty);
        set => SetValue(NodeHoverModeProperty, value);
    }
    
    public IconTemplate? SwitcherExpandIcon
    {
        get => GetValue(SwitcherExpandIconProperty);
        set => SetValue(SwitcherExpandIconProperty, value);
    }

    public IconTemplate? SwitcherCollapseIcon
    {
        get => GetValue(SwitcherCollapseIconProperty);
        set => SetValue(SwitcherCollapseIconProperty, value);
    }

    public IconTemplate? SwitcherRotationIcon
    {
        get => GetValue(SwitcherRotationIconProperty);
        set => SetValue(SwitcherRotationIconProperty, value);
    }

    public IconTemplate? SwitcherLoadingIcon
    {
        get => GetValue(SwitcherLoadingIconProperty);
        set => SetValue(SwitcherLoadingIconProperty, value);
    }

    public IconTemplate? SwitcherLeafIcon
    {
        get => GetValue(SwitcherLeafIconProperty);
        set => SetValue(SwitcherLeafIconProperty, value);
    }

    public bool IsShowLeafIcon
    {
        get => GetValue(IsShowLeafIconProperty);
        set => SetValue(IsShowLeafIconProperty, value);
    }
    
    public bool IsSwitcherRotation
    {
        get => GetValue(IsSwitcherRotationProperty);
        set => SetValue(IsSwitcherRotationProperty, value);
    }
    
    public bool IsSelectable
    {
        get => GetValue(IsSelectableProperty);
        set => SetValue(IsSelectableProperty, value);
    }

    public bool IsSelectOnRightClick
    {
        get => GetValue(IsSelectOnRightClickProperty);
        set => SetValue(IsSelectOnRightClickProperty, value);
    }

    public bool IsCheckStrictly
    {
        get => GetValue(IsCheckStrictlyProperty);
        set => SetValue(IsCheckStrictlyProperty, value);
    }

    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public AbstractMotion? OpenMotion
    {
        get => GetValue(OpenMotionProperty);
        set => SetValue(OpenMotionProperty, value);
    }
    
    public AbstractMotion? CloseMotion
    {
        get => GetValue(CloseMotionProperty);
        set => SetValue(CloseMotionProperty, value);
    }
    
    public ItemToggleType ToggleType
    {
        get => GetValue(ToggleTypeProperty);
        set => SetValue(ToggleTypeProperty, value);
    }
    
    private IList<TreeNodePath>? _defaultCheckedPaths;
    
    public IList<TreeNodePath>? DefaultCheckedPaths
    {
        get => _defaultCheckedPaths;
        set => SetAndRaise(DefaultCheckedPathsProperty, ref _defaultCheckedPaths, value);
    }
    
    private IList<TreeNodePath>? _defaultSelectedPaths;
    
    public IList<TreeNodePath>? DefaultSelectedPaths
    {
        get => _defaultSelectedPaths;
        set => SetAndRaise(DefaultSelectedPathsProperty, ref _defaultSelectedPaths, value);
    }
    
    private IList<TreeNodePath>? _defaultExpandedPaths;
    
    public IList<TreeNodePath>? DefaultExpandedPaths
    {
        get => _defaultExpandedPaths;
        set => SetAndRaise(DefaultExpandedPathsProperty, ref _defaultExpandedPaths, value);
    }
    
    public ITreeItemNodeLoader? DataLoader
    {
        get => GetValue(DataLoaderProperty);
        set => SetValue(DataLoaderProperty, value);
    }
    
    public IValueFilter? Filter
    {
        get => GetValue(FilterProperty);
        set => SetValue(FilterProperty, value);
    }
    
    public object? FilterValue
    {
        get => GetValue(FilterValueProperty);
        set => SetValue(FilterValueProperty, value);
    }

    public DefaultFilterValueSelector? FilterValueSelector
    {
        get => GetValue(FilterValueSelectorProperty);
        set => SetValue(FilterValueSelectorProperty, value);
    }

    public TreeFilterStrategy FilterStrategy
    {
        get => GetValue(FilterStrategyProperty);
        set => SetValue(FilterStrategyProperty, value);
    }

    private int _filterResultCount;
    
    public int FilterResultCount
    {
        get => _filterResultCount;
        private set => SetAndRaise(FilterResultCountProperty, ref _filterResultCount, value);
    }
    
    [DependsOn(nameof(EmptyIndicatorTemplate))]
    public object? EmptyIndicator
    {
        get => GetValue(EmptyIndicatorProperty);
        set => SetValue(EmptyIndicatorProperty, value);
    }

    public IDataTemplate? EmptyIndicatorTemplate
    {
        get => GetValue(EmptyIndicatorTemplateProperty);
        set => SetValue(EmptyIndicatorTemplateProperty, value);
    }
    
    public bool IsShowEmptyIndicator
    {
        get => GetValue(IsShowEmptyIndicatorProperty);
        set => SetValue(IsShowEmptyIndicatorProperty, value);
    }
    
    public Thickness EmptyIndicatorPadding
    {
        get => GetValue(EmptyIndicatorPaddingProperty);
        set => SetValue(EmptyIndicatorPaddingProperty, value);
    }
    
    public IBrush? FilterHighlightForeground
    {
        get => GetValue(FilterHighlightForegroundProperty);
        set => SetValue(FilterHighlightForegroundProperty, value);
    }
    
    /// <summary>
    /// Gets or sets the selected items.
    /// </summary>
    [AllowNull]
    public IList CheckedItems
    {
        get
        {
            if (_checkedItems == null)
            {
                _checkedItems = new AvaloniaList<object>();
                SubscribeToCheckedItems();
            }

            return _checkedItems;
        }
        set
        {
            if (value?.IsReadOnly == true)
            {
                throw new NotSupportedException(
                    "Cannot use a fixed size or read-only collection as CheckedItems.");
            }

            UnsubscribeFromCheckedItems();
            _checkedItems = value ?? new AvaloniaList<object>();
            SubscribeToCheckedItems();
        }
    }

    #endregion

    #region 公共事件定义
    
    public event EventHandler<TreeViewCheckedItemsChangedEventArgs>? CheckedItemsChanged;
    public event EventHandler<TreeViewItemLoadedEventArgs>? TreeItemLoaded;
    public event EventHandler<TreeViewDragStartedEventArgs>? ItemDragStarted;
    public event EventHandler<TreeViewDragCompletedEventArgs>? ItemDragCompleted;
    public event EventHandler<TreeViewDragEnterEventArgs>? ItemDragEnter;
    public event EventHandler<TreeViewDragLeaveEventArgs>? ItemDragLeave;
    public event EventHandler<TreeViewDragOverEventArgs>? ItemDragOver;
    public event EventHandler<TreeViewDroppedEventArgs>? ItemDropped;
    public event EventHandler<TreeItemExpandedEventArgs>? ItemExpanded;
    public event EventHandler<TreeItemCollapsedEventArgs>? ItemCollapsed;
    public event EventHandler<TreeItemClickedEventArgs>? ItemClicked;
    public event EventHandler<TreeItemContextMenuEventArgs>? ItemContextMenuRequest;
    
    #endregion

    #region 内部属性定义
    
    internal static readonly StyledProperty<TimeSpan> MotionDurationProperty =
        MotionAwareControlProperty.MotionDurationProperty.AddOwner<TreeView>();
    
    internal static readonly DirectProperty<TreeView, bool> IsEffectiveEmptyVisibleProperty =
        AvaloniaProperty.RegisterDirect<TreeView, bool>(
            nameof(IsEffectiveEmptyVisible),
            o => o.IsEffectiveEmptyVisible,
            (o, v) => o.IsEffectiveEmptyVisible = v);
    
    internal TimeSpan MotionDuration
    {
        get => GetValue(MotionDurationProperty);
        set => SetValue(MotionDurationProperty, value);
    }
    
    private bool _isEffectiveEmptyVisible = false;
    internal bool IsEffectiveEmptyVisible
    {
        get => _isEffectiveEmptyVisible;
        set => SetAndRaise(IsEffectiveEmptyVisibleProperty, ref _isEffectiveEmptyVisible, value);
    }
    
    protected internal ITreeViewInteractionHandler InteractionHandler { get; }
    
    #endregion
    
    private static readonly IList Empty = Array.Empty<object>();
    private IList? _checkedItems;
    internal bool SyncingCheckedItems;
    
    internal bool IsExpandAllProcess { get; set; }
    internal bool IsCollapseAllProcess { get; set; }

    static TreeView()
    {
        ConfigureDragAndDrop();
        TreeViewItem.ExpandedEvent.AddClassHandler<TreeView>((treeView, args) => treeView.HandleTreeItemExpanded(args));
        TreeViewItem.CollapsedEvent.AddClassHandler<TreeView>((treeView, args) => treeView.HandleTreeItemCollapsed(args));
        TreeViewItem.ContextMenuRequestEvent.AddClassHandler<TreeView>((treeView, args) => treeView.HandleTreeItemContextMenuRequest(args));
        TreeViewItem.ClickEvent.AddClassHandler<TreeView>((treeView, args) => treeView.HandleTreeItemClicked(args));
        ConfigureFilter();

        SelectedItemProperty.Changed.AddClassHandler<TreeView>((treeView, args) => treeView.NotifyFormValueChanged(args.NewValue));
        SelectedItemsProperty.Changed.AddClassHandler<TreeView>((treeView, args) => treeView.NotifyFormValueChanged(args.NewValue));
    }

    public TreeView()
        : this(new DefaultTreeViewInteractionHandler(false))
    {
    }
    
    protected TreeView(ITreeViewInteractionHandler interactionHandler)
    {
        InteractionHandler = interactionHandler ?? throw new ArgumentNullException(nameof(interactionHandler));
        this.RegisterTokenResourceScope(TreeViewToken.ScopeProvider);
        Items.CollectionChanged           += HandleCollectionChanged;
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Filter ??= ValueFilterFactory.BuildFilter(ValueFilterMode.Contains);
        FilterValueSelector ??= DefaultTreeFilterValueSelector;
        ConfigureEmptyIndicator();
    }

    internal static readonly DefaultFilterValueSelector DefaultTreeFilterValueSelector = value =>
    {
        if (value is TreeViewItem treeViewItem)
        {
            if (treeViewItem.Header is ITreeItemNode treeItemData)
            {
                return treeItemData.Header?.ToString();
            }
            if (treeViewItem.Header is string header)
            {
                return header;
            }
            return treeViewItem.Header?.ToString();
        }
        return null;
    };
    
    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ConfigureEmptyIndicator();
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Remove:
            case NotifyCollectionChangedAction.Replace:
                var oldItems = e.OldItems!;
                for (var i = 0; i < oldItems.Count; i++)
                {
                    CheckedItems.Remove(oldItems[i]);
                }
                break;
            case NotifyCollectionChangedAction.Reset:
                CheckedItems.Clear();
                break;
        }
    }
    
    public void ExpandAll(bool? motionEnabled = null)
    {
        var originMotionEnabled = IsMotionEnabled;
        try
        {
            IsExpandAllProcess = true;
            if (motionEnabled.HasValue)
            {
                SetCurrentValue(IsMotionEnabledProperty, motionEnabled.Value);
            }

            foreach (var item in Items)
            {
                if (item != null)
                {
                    if (TreeContainerFromItem(item) is TreeViewItem treeItem)
                    {
                        ExpandSubTree(treeItem);
                    }
                }
            }
        }
        finally
        {
            IsExpandAllProcess = false;
            if (motionEnabled.HasValue)
            {
                SetCurrentValue(IsMotionEnabledProperty, originMotionEnabled);
            }
        }
    }

    public void CollapseAll(bool? motionEnabled = null)
    {
        var originMotionEnabled = IsMotionEnabled;
        try
        {
            IsCollapseAllProcess = true;
            if (motionEnabled.HasValue)
            {
                SetCurrentValue(IsMotionEnabledProperty, motionEnabled.Value);
            }

            for (var i = 0; i < ItemCount; i++)
            {
                if (ContainerFromIndex(i) is TreeViewItem treeItem)
                {
                    CollapseSubTree(treeItem);
                }
            }
        }
        finally
        {
            IsCollapseAllProcess = false;
            if (motionEnabled.HasValue)
            {
                SetCurrentValue(IsMotionEnabledProperty, originMotionEnabled);
            }
        }
    }

    public void CheckedSubTree(TreeViewItem viewItem)
    {
        if (!viewItem.IsEffectiveCheckable())
        {
            return;
        }
        
        var originIsMotionEnabled = IsMotionEnabled;
        try
        {
            SetCurrentValue(IsMotionEnabledProperty, false);
            var checkedItems = DoCheckedSubTree(viewItem);
            try
            {
                SyncingCheckedItems = true;
                foreach (var checkedItem in checkedItems)
                {
                    if (!CheckedItems.Contains(checkedItem))
                    {
                        CheckedItems.Add(checkedItem);
                    }
                }
            }
            finally
            {
                SyncingCheckedItems = false;
            }
        }
        finally
        {
            SetCurrentValue(IsMotionEnabledProperty, originIsMotionEnabled);
        }
    }

    protected virtual bool RecursiveCheckNodePredicate(TreeViewItem treeViewItem)
    {
        return true;
    }
    
    protected virtual bool RecursiveUnCheckNodePredicate(TreeViewItem treeViewItem)
    {
        return true;
    }

    private ISet<object> DoCheckedSubTree(TreeViewItem treeViewItem)
    {
        var expandedStates  = new Dictionary<TreeViewItem, bool>();

        // Phase 1: Expand entire subtree to realize all containers
        ExpandSubTreeForCheck(treeViewItem, expandedStates);
        var checkedItems = new HashSet<object>(GetSubTreeCheckResultCapacity(treeViewItem, expandedStates.Count));

        try
        {
            // Phase 2: Check all nodes (all containers are now realized)
            DoCheckedSubTreeCore(treeViewItem, checkedItems);

            // Phase 3: Update parent chain once after all children are checked
            if (!IsCheckStrictly)
            {
                var (checkedParentItems, _) = SetupParentNodeCheckedStatus(treeViewItem);
                checkedItems.UnionWith(checkedParentItems);
            }
        }
        finally
        {
            // Phase 4: Restore all expanded states
            RestoreExpandedStates(expandedStates);
        }

        return checkedItems;
    }

    private void DoCheckedSubTreeCore(TreeViewItem treeViewItem, HashSet<object> checkedItems)
    {
        if (RecursiveCheckNodePredicate(treeViewItem))
        {
            treeViewItem.SetCurrentValue(TreeViewItem.IsCheckedProperty, true);
            var treeItemData = TreeItemFromContainer(treeViewItem);
            Debug.Assert(treeItemData != null);
            checkedItems.Add(treeItemData);
        }

        foreach (var childItem in treeViewItem.Items)
        {
            if (childItem != null)
            {
                var container = TreeContainerFromItem(childItem);
                if (container is TreeViewItem childTreeViewItem && childTreeViewItem.IsEffectiveCheckable())
                {
                    DoCheckedSubTreeCore(childTreeViewItem, checkedItems);
                }
            }
        }
    }

    public void UnCheckedSubTree(TreeViewItem viewItem)
    {
        if (!viewItem.IsEffectiveCheckable())
        {
            return;
        }

        var originIsMotionEnabled = IsMotionEnabled;
        try
        {
            SetCurrentValue(IsMotionEnabledProperty, false);
            var unCheckedItems = DoUnCheckedSubTree(viewItem);
            try
            {
                SyncingCheckedItems = true;
                foreach (var unCheckedItem in unCheckedItems)
                {
                    CheckedItems.Remove(unCheckedItem);
                }

                var treeItemData = TreeItemFromContainer(viewItem);
                Debug.Assert(treeItemData != null);
                CheckedItems.Remove(treeItemData);
            }
            finally
            {
                SyncingCheckedItems = false;
            }
        }
        finally
        {
            SetCurrentValue(IsMotionEnabledProperty, originIsMotionEnabled);
        }
    }

    public ISet<object> DoUnCheckedSubTree(TreeViewItem treeViewItem)
    {
        var expandedStates = new Dictionary<TreeViewItem, bool>();

        // Phase 1: Expand entire subtree to realize all containers
        ExpandSubTreeForCheck(treeViewItem, expandedStates);
        var unCheckedItems = new HashSet<object>(GetSubTreeCheckResultCapacity(treeViewItem, expandedStates.Count));

        try
        {
            // Phase 2: Uncheck all nodes (all containers are now realized)
            DoUnCheckedSubTreeCore(treeViewItem, unCheckedItems);

            // Phase 3: Update parent chain once after all children are unchecked
            if (!IsCheckStrictly)
            {
                var (_, unCheckedParentItems) = SetupParentNodeCheckedStatus(treeViewItem);
                unCheckedItems.UnionWith(unCheckedParentItems);
            }
        }
        finally
        {
            // Phase 4: Restore all expanded states
            RestoreExpandedStates(expandedStates);
        }

        return unCheckedItems;
    }

    private int GetSubTreeCheckResultCapacity(TreeViewItem treeViewItem, int realizedSubTreeCount)
    {
        if (IsCheckStrictly)
        {
            return realizedSubTreeCount;
        }

        return realizedSubTreeCount + Math.Max(0, CountTreeViewItemPathDepth(treeViewItem) - 1);
    }

    private void DoUnCheckedSubTreeCore(TreeViewItem treeViewItem, HashSet<object> unCheckedItems)
    {
        if (treeViewItem.IsChecked == true && RecursiveUnCheckNodePredicate(treeViewItem))
        {
            var treeItemData = TreeItemFromContainer(treeViewItem);
            Debug.Assert(treeItemData != null);
            unCheckedItems.Add(treeItemData);
            treeViewItem.SetCurrentValue(TreeViewItem.IsCheckedProperty, false);
        }

        foreach (var childItem in treeViewItem.Items)
        {
            if (childItem != null)
            {
                var control = TreeContainerFromItem(childItem);
                if (control is TreeViewItem childTreeViewItem && childTreeViewItem.IsEffectiveCheckable())
                {
                    DoUnCheckedSubTreeCore(childTreeViewItem, unCheckedItems);
                }
            }
        }
    }

    private void ExpandSubTreeForCheck(TreeViewItem treeViewItem, Dictionary<TreeViewItem, bool> expandedStates)
    {
        var wasExpanded = treeViewItem.IsExpanded;
        expandedStates[treeViewItem] = wasExpanded;

        if (treeViewItem.Presenter?.Panel == null && !wasExpanded)
        {
            treeViewItem.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                topLevel.GetLayoutManager()?.ExecuteLayoutPass();
            }
        }

        foreach (var childItem in treeViewItem.Items)
        {
            if (childItem != null)
            {
                var container = TreeContainerFromItem(childItem);
                if (container is TreeViewItem childTreeViewItem)
                {
                    ExpandSubTreeForCheck(childTreeViewItem, expandedStates);
                }
            }
        }
    }

    private void RestoreExpandedStates(Dictionary<TreeViewItem, bool> expandedStates)
    {
        foreach (var (item, wasExpanded) in expandedStates)
        {
            item.SetCurrentValue(TreeViewItem.IsExpandedProperty, wasExpanded);
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Draggable, IsDraggable);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new TreeViewItem();
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<TreeViewItem>(item, out recycleKey);
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        if (container is TreeViewItem treeViewItem)
        {
            treeViewItem.OwnerTreeView = this;
            
            if (item != null && item is not Visual && item is ITreeItemNode treeViewItemData)
            {
                TreeViewItem.ApplyNodeData(treeViewItem, treeViewItemData);
            }
            
            if (ItemTemplate != null)
            {
                treeViewItem[!TreeViewItem.HeaderTemplateProperty] = this[!ItemTemplateProperty];
            }
            
            SetTreeViewItemIcon(treeViewItem, TreeViewItem.SwitcherExpandIconProperty, SwitcherExpandIcon);
            SetTreeViewItemIcon(treeViewItem, TreeViewItem.SwitcherCollapseIconProperty, SwitcherCollapseIcon);
            SetTreeViewItemIcon(treeViewItem, TreeViewItem.SwitcherRotationIconProperty, SwitcherRotationIcon);
            SetTreeViewItemIcon(treeViewItem, TreeViewItem.SwitcherLoadingIconProperty, SwitcherLoadingIcon);
            SetTreeViewItemIcon(treeViewItem, TreeViewItem.SwitcherLeafIconProperty, SwitcherLeafIcon);
            
            treeViewItem[!TreeViewItem.FilterStrategyProperty]           = this[!FilterStrategyProperty];
            treeViewItem[!TreeViewItem.IsMotionEnabledProperty]           = this[!IsMotionEnabledProperty];
            treeViewItem[!TreeViewItem.NodeHoverModeProperty]             = this[!NodeHoverModeProperty];
            treeViewItem[!TreeViewItem.IsShowLineProperty]                = this[!IsShowLineProperty];
            treeViewItem[!TreeViewItem.IsShowIconProperty]                = this[!IsShowIconProperty];
            treeViewItem[!TreeViewItem.IsShowLeafIconProperty]            = this[!IsShowLeafIconProperty];
            treeViewItem[!TreeViewItem.IsSwitcherRotationProperty]        = this[!IsSwitcherRotationProperty];
            treeViewItem[!TreeViewItem.ToggleTypeProperty]                = this[!ToggleTypeProperty];
            treeViewItem[!TreeViewItem.IsSelectableProperty]              = this[!IsSelectableProperty];
            treeViewItem[!TreeViewItem.FilterHighlightForegroundProperty] = this[!FilterHighlightForegroundProperty];
            treeViewItem[!TreeViewItem.HasTreeItemDataLoaderProperty]     = this[!HasTreeItemDataLoaderProperty];
            treeViewItem[!TreeViewItem.IsAutoExpandParentProperty]        = this[!IsAutoExpandParentProperty];
            
            PrepareTreeViewItem(treeViewItem, item, index);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(container), "The container type is incorrect, it must be type TreeItem.");
        }
    }

    private void SetTreeViewItemIcon(TreeViewItem treeViewItem, AvaloniaProperty iconProperty, IIconTemplate? iconTemplate)
    {
        if (iconTemplate == null)
        {
            treeViewItem.SetValue(iconProperty, null);
        }
        else
        {
            treeViewItem.SetValue(iconProperty, iconTemplate.Build());
        }
    }
    
    protected virtual void PrepareTreeViewItem(TreeViewItem treeViewItem, object? item, int index)
    {
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        InteractionHandler.Attach(this);
        UpdatePseudoClasses();
    }
    
    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        InteractionHandler.Detach(this);
        
        // 清理所有待处理的异步加载操作
        _asyncLoadCoordinator.CancelAll();
    }

    private TreeViewItem? GetTreeViewItemContainer(object childNode, ItemsControl current)
    {
        if (current.Presenter?.Panel == null)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel != null)
            {
                topLevel.GetLayoutManager()?.ExecuteLayoutPass();
            }
        }
        if (current.Presenter?.Panel is { } panel)
        {
            return current.ContainerFromItem(childNode) as TreeViewItem;
        }
        return null;
    }
    
    private void SubscribeToCheckedItems()
    {
        if (_checkedItems is INotifyCollectionChanged incc)
        {
            incc.CollectionChanged += HandleCheckedItemsCollectionChanged;
        }

        HandleCheckedItemsCollectionChanged(
            _checkedItems,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
    
    private void UnsubscribeFromCheckedItems()
    {
        if (_checkedItems is INotifyCollectionChanged incc)
        {
            incc.CollectionChanged -= HandleCheckedItemsCollectionChanged;
        }
    }
    
    private void HandleCheckedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        IList? added   = null;
        IList? removed = null;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:

            {
                if (e.NewItems != null)
                {
                    if (!SyncingCheckedItems)
                    {
                        CheckedItemsAdded(e.NewItems); 
                    }
                    added = e.NewItems;
                }
               
                break;
            }
            case NotifyCollectionChangedAction.Remove:
                if (!SyncingCheckedItems)
                {
                    if (e.OldItems != null)
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            MarkItemChecked(e.OldItems[i]!, false);
                        }
                    }
                }

                removed = e.OldItems;

                break;
            case NotifyCollectionChangedAction.Reset:
                if (!SyncingCheckedItems)
                {
                    foreach (var container in GetRealizedTreeContainers())
                    {
                        MarkContainerChecked(container, false);
                    }
                    if (e.NewItems?.Count > 0)
                    {
                        CheckedItemsAdded(e.NewItems);
                    }
                }

                if (e.NewItems?.Count > 0)
                {
                    added = new List<object>(CheckedItems.Count);
                    foreach (var item in CheckedItems)
                    {
                        added.Add(item);
                    }
                }

                break;
            case NotifyCollectionChangedAction.Replace:
            {
                if (!SyncingCheckedItems)
                {
                    if (e.OldItems != null)
                    {
                        for (var i = 0; i < e.OldItems.Count; i++)
                        {
                            MarkItemChecked(e.OldItems[i]!, false);
                        }
                    }

                    if (e.NewItems != null)
                    {
                        for (var i = 0; i < e.NewItems.Count; i++)
                        {
                            MarkItemChecked(e.NewItems[i]!, true);
                        }
                    }
                }
                
                added   = e.NewItems;
                removed = e.OldItems;
                break;
            }
        }
        if (added?.Count > 0 || removed?.Count > 0)
        {
            CheckedItemsChanged?.Invoke(this, new TreeViewCheckedItemsChangedEventArgs(
                removed ?? Empty,
                added ?? Empty));
        }
    }
    
    private void CheckedItemsAdded(IList items)
    {
        if (items.Count == 0)
        {
            return;
        }
        foreach (var item in items)
        {
            MarkItemChecked(item, true);
        }
    }
    
    private void MarkItemChecked(object item, bool isChecked)
    {
        var container = TreeContainerFromItem(item);
        if (container != null)
        {
            MarkContainerChecked(container, isChecked);
        }
    }
    
    private void MarkContainerChecked(Control container, bool isChecked)
    {
        container.SetCurrentValue(TreeViewItem.IsCheckedProperty, isChecked);
    }

    #region 默认展开选中

    private List<TreeViewItem>? ExpandTreeViewPath(TreeNodePath treeNodePath)
    {
        if (treeNodePath.Length == 0)
        {
            return null;
        }

        var originIsMotionEnabled = IsMotionEnabled;
        try
        {
            SetCurrentValue(IsMotionEnabledProperty, false);
            var   segments  = treeNodePath.Segments;
            IList items     = Items;
            var   pathNodes = new List<TreeViewItem>(segments.Count);
            foreach (var segment in segments)
            {
                bool childFound = false;
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item != null)
                    {
                        var treeViewItem = TreeContainerFromItem(item) as TreeViewItem;
                        if (treeViewItem == null)
                        {
                            return null;
                        }
                        if (IsPathSegmentMatched(treeViewItem, segment))
                        {
                            treeViewItem.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
                            if (treeViewItem.Presenter?.Panel == null)
                            {
                                var topLevel = TopLevel.GetTopLevel(this);
                                if (topLevel != null)
                                {
                                    topLevel.GetLayoutManager()?.ExecuteLayoutPass();
                                }
                            }
                            items      = treeViewItem.Items;
                            childFound = true;
                            pathNodes.Add(treeViewItem);
                            break;
                        }
                    }
                }

                if (!childFound)
                {
                    return null;
                }
            }

            return pathNodes;
        }
        finally
        {
            SetCurrentValue(IsMotionEnabledProperty, originIsMotionEnabled);
        }
    }

    private List<TreeViewItem>? CollapseTreeViewPath(TreeNodePath treeNodePath)
    {
        if (treeNodePath.Length == 0)
        {
            return null;
        }
        var originIsMotionEnabled = IsMotionEnabled;
        try
        {
            var   segments  = treeNodePath.Segments;
            IList items     = Items;
            var   pathNodes = new List<TreeViewItem>(segments.Count);
            foreach (var segment in segments)
            {
                bool childFound = false;
                for (var i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if (item != null)
                    {
                        var treeViewItem = TreeContainerFromItem(item) as TreeViewItem;
                        if (treeViewItem == null)
                        {
                            return null;
                        }
                        if (IsPathSegmentMatched(treeViewItem, segment))
                        {
                            treeViewItem.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
                            if (treeViewItem.Presenter?.Panel == null)
                            {
                                var topLevel = TopLevel.GetTopLevel(this);
                                if (topLevel != null)
                                {
                                    topLevel.GetLayoutManager()?.ExecuteLayoutPass();
                                }
                            }
                            items      = treeViewItem.Items;
                            childFound = true;
                            pathNodes.Add(treeViewItem);
                            break;
                        }
                    }
                }

                if (!childFound)
                {
                    return null;
                }
            }

            foreach (var treeViewItem in pathNodes)
            {
                treeViewItem.SetCurrentValue(TreeViewItem.IsExpandedProperty, false);
            }
            return pathNodes;
        }
        finally
        {
            SetCurrentValue(IsMotionEnabledProperty, originIsMotionEnabled);
        }
    }
    
    private List<TreeViewItem>? TraverTreeViewPath(TreeNodePath treeNodePath, Action<TreeViewItem, int>? action)
    {
        if (treeNodePath.Length == 0)
        {
            return null;
        }

        var originIsMotionEnabled = IsMotionEnabled;
        try
        {
            SetCurrentValue(IsMotionEnabledProperty, false);
            var   segments             = treeNodePath.Segments;
            IList items                = Items;
            var   pathNodes            = new List<TreeViewItem>(segments.Count);
            var   pathNodeExpandStatus = new List<bool>(segments.Count);
            try
            {
                for (int i = 0; i < segments.Count; i++)
                {
                    var  segment    = segments[i];
                    bool childFound = false;
                    for (var j = 0; j < items.Count; j++)
                    {
                        var item = items[j];
                        if (item != null)
                        {
                            var treeViewItem = TreeContainerFromItem(item) as TreeViewItem;
                            if (treeViewItem == null)
                            {
                                return null;
                            }

                            if (IsPathSegmentMatched(treeViewItem, segment))
                            {
                                pathNodeExpandStatus.Add(treeViewItem.IsExpanded);
                                treeViewItem.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
                                if (treeViewItem.Presenter?.Panel == null)
                                {
                                    var topLevel = TopLevel.GetTopLevel(this);
                                    if (topLevel != null)
                                    {
                                        topLevel.GetLayoutManager()?.ExecuteLayoutPass();
                                    }
                                }

                                items      = treeViewItem.Items;
                                childFound = true;
                                pathNodes.Add(treeViewItem);
                                action?.Invoke(treeViewItem, i);
                                break;
                            }
                        }
                    }

                    if (!childFound)
                    {
                        return null;
                    }
                }

                return pathNodes;
            }
            finally
            {
                for (var i = pathNodes.Count - 1; i >= 0; --i)
                {
                    var treeViewItem = pathNodes[i];
                    var expandStatus = pathNodeExpandStatus[i];
                    treeViewItem.SetCurrentValue(TreeViewItem.IsExpandedProperty, expandStatus);
                }
            }
        }
        finally
        {
            SetCurrentValue(IsMotionEnabledProperty, originIsMotionEnabled);
        }
    }

    private static bool IsPathSegmentMatched(TreeViewItem treeViewItem, string segment)
    {
        if (treeViewItem.ItemKey != null && treeViewItem.ItemKey.Value == segment)
        {
            return true;
        }
        return treeViewItem.Value?.ToString() == segment;
    }
    
    private (ISet<object>, ISet<object>) SetupParentNodeCheckedStatus(TreeViewItem viewItem)
    {
        var parent           = viewItem.Parent;
        var parentDepth      = Math.Max(0, CountTreeViewItemPathDepth(viewItem) - 1);
        var checkedParents   =  new HashSet<object>(parentDepth);
        var unCheckedParents =  new HashSet<object>(parentDepth);
        while (parent is TreeViewItem parentTreeItem && parentTreeItem.IsEnabled)
        {
            GetChildCheckStatus(parentTreeItem, out var isAllChecked, out var isAnyChecked);

            if (parentTreeItem.IsChecked == true && !isAllChecked)
            {
                var parentTreeItemData = TreeItemFromContainer(parentTreeItem);
                Debug.Assert(parentTreeItemData != null);
                unCheckedParents.Add(parentTreeItemData);
            }
            
            var originMotionEnabled = parentTreeItem.IsMotionEnabled;
            try
            {
                parentTreeItem.SetCurrentValue(TreeViewItem.IsMotionEnabledProperty, false);
                if (isAllChecked)
                {
                    parentTreeItem.SetCurrentValue(TreeViewItem.IsCheckedProperty, true);
                }
                else if (isAnyChecked)
                {
                    parentTreeItem.SetCurrentValue(TreeViewItem.IsCheckedProperty, null);
                }
                else
                {
                    parentTreeItem.SetCurrentValue(TreeViewItem.IsCheckedProperty, false);
                }
            }
            finally
            {
                parentTreeItem.SetCurrentValue(TreeViewItem.IsMotionEnabledProperty, originMotionEnabled);
            }
       
            if (parentTreeItem.IsChecked == true)
            {
                var parentTreeItemData = TreeItemFromContainer(parentTreeItem);
                Debug.Assert(parentTreeItemData != null);
                checkedParents.Add(parentTreeItemData);
            }
            parent = parent.Parent;
        }

        return (checkedParents, unCheckedParents);
    }

    private void ConfigureStateAfterItemsSourceChanged()
    {
        if (!IsLoaded)
        {
            return;
        }

        var selectedItemPath  = BuildNodeIdentityPath(SelectedItem as ITreeItemNode);
        var selectedItemPaths = BuildNodeIdentityPaths(SelectedItems);
        var checkedItemPaths  = BuildNodeIdentityPaths(CheckedItems);

        SetCurrentValue(SelectedItemProperty, null);
        SelectedItems.Clear();
        CheckedItems.Clear();

        var selectionRestored = false;
        if (selectedItemPath != null)
        {
            selectionRestored = TrySelectNodePath(selectedItemPath);
        }
        if (!selectionRestored && selectedItemPaths != null)
        {
            foreach (var path in selectedItemPaths)
            {
                selectionRestored |= TrySelectNodePath(path);
            }
        }
        if (!selectionRestored)
        {
            ConfigureDefaultSelectedPaths();
        }

        var checkedRestored = false;
        if (checkedItemPaths != null)
        {
            foreach (var path in checkedItemPaths)
            {
                checkedRestored |= TryCheckNodePath(path);
            }
        }
        if (!checkedRestored)
        {
            ConfigureDefaultCheckedPaths();
        }

        if (IsDefaultExpandAll)
        {
            ExpandAll(false);
        }
        else
        {
            ConfigureDefaultExpandedPaths();
        }
    }

    private static List<TreeNodePath>? BuildNodeIdentityPaths(IList? nodes)
    {
        if (nodes == null)
        {
            return null;
        }

        var paths = new List<TreeNodePath>(nodes.Count);
        foreach (var node in nodes)
        {
            if (node is ITreeItemNode treeItemNode)
            {
                var path = BuildNodeIdentityPath(treeItemNode);
                if (path != null)
                {
                    paths.Add(path);
                }
            }
        }
        return paths;
    }

    private static TreeNodePath? BuildNodeIdentityPath(ITreeItemNode? node)
    {
        if (node == null)
        {
            return null;
        }

        var depth    = CountTreeItemPathDepth(node);
        var segments = new string[depth];
        var current  = node;
        for (var i = segments.Length - 1; current != null; i--)
        {
            var segment = current.ItemKey?.ToString() ?? current.Value?.ToString();
            if (string.IsNullOrEmpty(segment))
            {
                return null;
            }

            segments[i] = segment;
            current     = current.ParentNode as ITreeItemNode;
        }

        return new TreeNodePath(segments);
    }

    private bool TrySelectNodePath(TreeNodePath path)
    {
        var selected = false;
        TraverTreeViewPath(path, (treeViewItem, i) =>
        {
            if (i == path.Length - 1)
            {
                var item = TreeItemFromContainer(treeViewItem);
                if (item != null)
                {
                    if (!SelectedItems.Contains(item))
                    {
                        SelectedItems.Add(item);
                    }
                    if (SelectionMode == SelectionMode.Single)
                    {
                        SetCurrentValue(SelectedItemProperty, item);
                    }
                    selected = true;
                }
            }
        });
        return selected;
    }

    private bool TryCheckNodePath(TreeNodePath path)
    {
        var isChecked = false;
        TraverTreeViewPath(path, (treeViewItem, i) =>
        {
            if (i == path.Length - 1)
            {
                treeViewItem.SetCurrentValue(TreeViewItem.IsCheckedProperty, true);
                isChecked = true;
            }
        });
        return isChecked;
    }

    private void GetChildCheckStatus(TreeViewItem parentTreeItem, out bool isAllChecked, out bool isAnyChecked)
    {
        isAllChecked = false;
        isAnyChecked = false;

        if (parentTreeItem.Items.Count == 0)
        {
            return;
        }

        isAllChecked = true;
        foreach (var childItem in parentTreeItem.Items)
        {
            var childSatisfiesAllChecked = false;
            if (childItem != null)
            {
                var container = TreeContainerFromItem(childItem);
                if (container is TreeViewItem treeViewItem)
                {
                    var isCheckable = treeViewItem.IsEffectiveCheckable();
                    childSatisfiesAllChecked = !isCheckable || treeViewItem.IsChecked == true;
                    if (isCheckable && treeViewItem.IsChecked != false)
                    {
                        isAnyChecked = true;
                    }
                }
            }

            if (!childSatisfiesAllChecked)
            {
                isAllChecked = false;
            }

            if (!isAllChecked && isAnyChecked)
            {
                break;
            }
        }
    }

    private void ConfigureDefaultCheckedPaths()
    {
        if (DefaultCheckedPaths != null)
        {
            foreach (var checkedPath in DefaultCheckedPaths)
            {
                TraverTreeViewPath(checkedPath, (treeViewItem, i) =>
                {
                    if (i == checkedPath.Length - 1)
                    {
                        treeViewItem.SetCurrentValue(TreeViewItem.IsCheckedProperty, true);
                    }
                });
            }
        }
    }
        
    private void ConfigureDefaultExpandedPaths()
    {
        if (DefaultExpandedPaths != null)
        {
            foreach (var path in DefaultExpandedPaths)
            {
                ExpandTreeViewPath(path);
            }
        }
    }
    
    private void ConfigureDefaultSelectedPaths()
    {
        if (IsSelectable)
        {
            if (SelectedItems.Count == 0 && SelectedItem == null)
            {
                if (DefaultSelectedPaths != null)
                {
                    foreach (var selectedPath in DefaultSelectedPaths)
                    {
                        TraverTreeViewPath(selectedPath, (treeViewItem, i) =>
                        {
                            if (i == selectedPath.Length - 1)
                            {
                                if (!SelectedItems.Contains(TreeItemFromContainer(treeViewItem)))
                                {
                                    SelectedItems.Add(TreeItemFromContainer(treeViewItem));
                                }
                            }
                        });
                    }
                }
            }
            else
            {
                if (SelectedItem != null)
                {
                    var paths = GetTreePathFromItem(SelectedItem);
                    SelectTreeItemByPath(paths);
                }
            }
        }
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        ConfigureDefaultSelectedPaths();
        ConfigureDefaultCheckedPaths();
        
        FilterTreeNode();
        
        if (IsDefaultExpandAll)
        {
            Dispatcher.Post(() => ExpandAll(false));
        }
        else
        {
            ConfigureDefaultExpandedPaths();
        }
    }

    #endregion
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == DefaultSelectedPathsProperty)
        {
            ConfigureDefaultSelectedPaths();
        }
        else if (change.Property == SwitcherRotationIconProperty)
        {
            HandleSwitcherRotationIconChanged();
        }
        else if (change.Property == SwitcherExpandIconProperty)
        {
            HandleSwitcherExpandIconChanged();
        }
        else if (change.Property == SwitcherCollapseIconProperty)
        {
            HandleSwitcherCollapseIconChanged();
        }
        else if (change.Property == SwitcherLoadingIconProperty)
        {
            HandleSwitcherLoadingIconChanged();
        }
        else if (change.Property == SwitcherLeafIconProperty)
        {
            HandleSwitcherLeafIconChanged();
        }
        else if (change.Property == DataLoaderProperty)
        {
            HasTreeItemDataLoader = DataLoader != null;
        }
        else if (change.Property == FilterProperty ||
                 change.Property == FilterStrategyProperty ||
                 change.Property == ItemsSourceProperty ||
                 change.Property == FilterValueProperty)
        {
            FilterTreeNode();
        }

        if (change.Property == IsShowEmptyIndicatorProperty ||
            change.Property == ItemsSourceProperty ||
            change.Property == FilterResultCountProperty ||
            change.Property == IsFilterModeProperty)
        {
            ConfigureEmptyIndicator();
        }
        else if (change.Property == IsSelectableProperty)
        {
            if (!IsSelectable)
            {
                SetCurrentValue(SelectedItemProperty, null);
                SelectedItems.Clear();
            }
        }

        if (change.Property == ItemsSourceProperty)
        {
            Dispatcher.Post(ConfigureStateAfterItemsSourceChanged);
        }
    }

    protected void HandleSwitcherRotationIconChanged()
    {
        for (var i = 0; i < ItemCount; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is TreeViewItem treeViewItem)
            {
                SetTreeViewItemIcon(treeViewItem, TreeViewItem.SwitcherRotationIconProperty, SwitcherRotationIcon);
            }
        }
    }
    
    protected void HandleSwitcherExpandIconChanged()
    {
        for (var i = 0; i < ItemCount; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is TreeViewItem treeViewItem)
            {
                SetTreeViewItemIcon(treeViewItem, TreeViewItem.SwitcherExpandIconProperty, SwitcherExpandIcon);
            }
        }
    }
    
    protected void HandleSwitcherCollapseIconChanged()
    {
        for (var i = 0; i < ItemCount; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is TreeViewItem treeViewItem)
            {
                SetTreeViewItemIcon(treeViewItem, TreeViewItem.SwitcherCollapseIconProperty, SwitcherCollapseIcon);
            }
        }
    }
    
    protected void HandleSwitcherLoadingIconChanged()
    {
        for (var i = 0; i < ItemCount; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is TreeViewItem treeViewItem)
            {
                SetTreeViewItemIcon(treeViewItem, TreeViewItem.SwitcherLoadingIconProperty, SwitcherLoadingIcon);
            }
        }
    }
    
    protected void HandleSwitcherLeafIconChanged()
    {
        for (var i = 0; i < ItemCount; i++)
        {
            var container = ContainerFromIndex(i);
            if (container is TreeViewItem treeViewItem)
            {
                SetTreeViewItemIcon(treeViewItem, TreeViewItem.SwitcherLeafIconProperty, SwitcherLeafIcon);
            }
        }
    }

    private void HandleTreeItemExpanded(RoutedEventArgs args)
    {
        if (args.Source is TreeViewItem item)
        {
            ItemExpanded?.Invoke(this, new TreeItemExpandedEventArgs(item));
        }
    }
    
    private void HandleTreeItemCollapsed(RoutedEventArgs args)
    {
        if (args.Source is TreeViewItem item)
        {
            ItemCollapsed?.Invoke(this, new TreeItemCollapsedEventArgs(item));
        }
    }

    private void HandleTreeItemContextMenuRequest(RoutedEventArgs args)
    {
        if (args.Source is TreeViewItem item)
        {
            ItemContextMenuRequest?.Invoke(this, new TreeItemContextMenuEventArgs(item));
        }
    }
    
    private void HandleTreeItemClicked(RoutedEventArgs args)
    {
        if (args.Source is TreeViewItem item)
        {
            NotifyTreeItemClicked(item);
            ItemClicked?.Invoke(this, new TreeItemClickedEventArgs(item));
        }
    }

    protected virtual void NotifyTreeItemClicked(TreeViewItem viewItem)
    {
    }
    
    protected virtual void ConfigureEmptyIndicator()
    {
        var isEmpty = false;
        if (IsFilterMode)
        {
            isEmpty = FilterResultCount == 0;
        }
        else
        {
            if (ItemsSource != null)
            {
                var enumerator = ItemsSource.GetEnumerator();
                isEmpty = !enumerator.MoveNext();
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            else
            {
                isEmpty = Items.Count == 0;
            }
        }
        IsEffectiveEmptyVisible = IsShowEmptyIndicator && isEmpty;
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        var container = DefaultTreeViewInteractionHandler.GetTreeViewItemCore(e.Source as Control);
        if (container is not null && e.Source is Visual source)
        {
            var point = e.GetCurrentPoint(source);
            if (point.Properties.IsLeftButtonPressed || point.Properties.IsRightButtonPressed)
            {
                if (IsSelectable)
                {
                    UpdateSelectionFromEvent(container, e);
                }
            }

            if (point.Properties.IsLeftButtonPressed && IsDraggable)
            {
                _lastPoint = e.GetPosition(this);
                e.PreventGestureRecognition();
            }
        }
    }

    public override bool UpdateSelectionFromEvent(Control container, RoutedEventArgs eventArgs)
    {
        if (!IsSelectOnRightClick &&
            eventArgs is PointerEventArgs pointerEvent &&
            pointerEvent.GetCurrentPoint(container).Properties.PointerUpdateKind is
                PointerUpdateKind.RightButtonPressed or PointerUpdateKind.RightButtonReleased)
        {
            return false;
        }

        return base.UpdateSelectionFromEvent(container, eventArgs);
    }

    private List<object> GetTreePathFromItem(object item)
    {
        List<object> paths;
        if (item is ITreeItemNode itemData)
        {
            var pathDepth = CountTreeItemPathDepth(itemData);
            paths = new List<object>(pathDepth);
            for (var i = 0; i < pathDepth; i++)
            {
                paths.Add(null!);
            }

            var current = itemData;
            for (var i = pathDepth - 1; current != null; i--)
            {
                paths[i] = current;
                current  = current.ParentNode as ITreeItemNode;
            }
        }
        else if (item is TreeViewItem treeViewItem)
        {
            var pathDepth = CountTreeViewItemPathDepth(treeViewItem);
            paths = new List<object>(pathDepth);
            for (var i = 0; i < pathDepth; i++)
            {
                paths.Add(null!);
            }

            var current = treeViewItem;
            for (var i = pathDepth - 1; current != null; i--)
            {
                paths[i] = current;
                current  = current.Parent as TreeViewItem;
            }
        }
        else
        {
            throw new ArgumentException("Invalid item type, Must ITreeItemNode or TreeItem.");
        }

        return paths;
    }

    private static int CountTreeItemPathDepth(ITreeItemNode itemData)
    {
        var count   = 0;
        var current = itemData;
        while (current != null)
        {
            count++;
            current = current.ParentNode as ITreeItemNode;
        }

        return count;
    }

    private void SelectTreeItemByPath(IList paths)
    {
        if (paths.Count == 0)
        {
            return;
        }
        ItemsControl current             = this;
        bool         originMotionEnabled = IsMotionEnabled;
        try
        {
            SetCurrentValue(IsMotionEnabledProperty, false);
            for (var i = 0; i < paths.Count; i++)
            {
                var pathNode = paths[i];
                if (pathNode != null)
                {
                    TreeViewItem? child          = null;
                    bool?         originExpanded = null;
                    try
                    {

                        if (current is TreeViewItem item)
                        {
                            originExpanded = item.IsExpanded;
                            item.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
                        }

                        child = GetTreeViewItemContainer(pathNode, current);
                    }
                    finally
                    {
                        if (current is TreeViewItem item)
                        {
                            if (originExpanded != null)
                            {
                                item.SetCurrentValue(TreeViewItem.IsExpandedProperty, originExpanded.Value);
                            }
                        }
                    }

                    if (child != null)
                    {
                        current = child;
                    }
                }
            }

            if (current is TreeViewItem treeViewItem)
            {
                var item = TreeItemFromContainer(treeViewItem);
                if (item != null && !SelectedItems.Contains(item))
                {
                    SelectedItems.Add(item);
                }
            }
        }
        finally
        {
            SetCurrentValue(IsMotionEnabledProperty, originMotionEnabled);
        }
    }
    
    #region 实现 FormItem 接口
    
    private EventHandler? _formValueChanged;
    event EventHandler? IFormItemAware.ValueChanged
    {
        add => _formValueChanged += value;
        remove => _formValueChanged -= value;
    }

    void IFormItemAware.SetFormValue(object? value) => NotifySetFormValue(value?.ToString());

    object? IFormItemAware.GetFormValue() => NotifyGetFormValue();
    void IFormItemAware.ClearFormValue() => NotifyClearFormValue();
    void IFormItemAware.NotifyValidateStatus(FormValidateStatus status) => NotifyValidateStatus(status);
    
    protected virtual void NotifyFormValueChanged(object? value)
    {
        _formValueChanged?.Invoke(this, EventArgs.Empty);
    }

    protected virtual void NotifySetFormValue(object? value)
    {
        if ((SelectionMode & SelectionMode.Multiple) == SelectionMode.Multiple)
        {
            SelectedItems = value as IList;
        }
        else
        {
            SelectedItem = value;
        }
    }

    protected virtual object? NotifyGetFormValue()
    {
        if ((SelectionMode & SelectionMode.Multiple) == SelectionMode.Multiple)
        {
            return SelectedItems;
        }
        return SelectedItem;
    }

    protected virtual void NotifyClearFormValue()
    {
        if ((SelectionMode & SelectionMode.Multiple) == SelectionMode.Multiple)
        {
            SelectedItems = null;
        }
        else
        {
            SelectedItem = null;
        }
    }

    protected virtual void NotifyValidateStatus(FormValidateStatus status)
    {
    }
    #endregion
}
