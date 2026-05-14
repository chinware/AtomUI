using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AtomUI.Controls;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

[Flags]
internal enum SpaceItemPosition
{
    First  = 0x01,
    Middle = 0x02,
    Last   = 0x04,
}

public class CompactSpace : TemplatedControl,
                            ISizeTypeAware,
                            IChildIndexProvider,
                            INavigableContainer
{
    internal const int ACTIVE_ZINDEX = 1000;
    internal const int NORMAL_ZINDEX = 0;
    
    #region 公共属性定义

    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<CompactSpace, Orientation>(nameof(Orientation), defaultValue: Orientation.Horizontal);
    
    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<CompactSpace>();
    
    public static readonly AttachedProperty<CompactSpaceSize> ItemSizeProperty =
        AvaloniaProperty.RegisterAttached<CompactSpace, Control, CompactSpaceSize>(
            "ItemSize",
            defaultValue: CompactSpaceSize.Auto);
    
    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }
    
    [Content]
    public AvaloniaList<Control> Children { get; } = new();
    
    #endregion
    
    event EventHandler<ChildIndexChangedEventArgs>? IChildIndexProvider.ChildIndexChanged
    {
        add
        {
            if (_childIndexChanged is null)
            {
                Children.PropertyChanged += HandleChildrenPropertyChanged;
            }
            _childIndexChanged += value;
        }

        remove
        {
            _childIndexChanged -= value;
            if (_childIndexChanged is null)
            {
                Children.PropertyChanged -= HandleChildrenPropertyChanged;
            }
        }
    }

    #region 内部属性定义
    private EventHandler<ChildIndexChangedEventArgs>? _childIndexChanged;
    #endregion

    private Grid? _contentLayout;
    private readonly Dictionary<object, NotifyCollectionChangedEventHandler> _childClassesChangedHandlers = new();
    private readonly Dictionary<Control, IDisposable> _childSizeTypeBindings = new();
    private readonly HashSet<Control> _childPropertyChangedTargets = new();
    private readonly HashSet<Control> _childInteractionTargets = new();
    private int? _sizeDefinitionsSignature;

    static CompactSpace()
    {
        OrientationProperty.Changed.AddClassHandler<CompactSpace>((space, _) => space.ConfigureSizeDefinitions());
        TemplatedParentProperty.Changed.AddClassHandler<CompactSpace>((space, args) => space.HandleTemplatedParentChanged());
    }
    
    public CompactSpace()
    {
        this.RegisterTokenResourceScope(SpaceToken.ScopeProvider);
        Children.CollectionChanged += HandleChildrenChanged;
    }
    
    public static void SetItemSize(Control element, CompactSpaceSize size)
    {
        element.SetValue(ItemSizeProperty, size);
    }

    public static CompactSpaceSize GetItemSize(Control element)
    {
        return element.GetValue(ItemSizeProperty);
    }
    
    protected virtual void HandleChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_contentLayout == null)
        {
            return;
        }
        ValidateFillerUsage();
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            {
                var layoutChildren = new List<Control>();
                foreach (var newItem in e.NewItems!.OfType<Control>())
                {
                    EnsureCompactSpaceItem(newItem);
                    layoutChildren.Add(CreateLayoutChild(newItem));
                }
                _contentLayout.Children.InsertRange(e.NewStartingIndex, layoutChildren);
                break;
            }

            case NotifyCollectionChangedAction.Move:
                _contentLayout.Children.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
                break;

            case NotifyCollectionChangedAction.Remove:
            {
                var oldItems                = new HashSet<Control>(e.OldItems!.OfType<Control>());
                var oldLayoutChildren       = new List<Control>();
                var oldCompactSpaceWrappers = new List<CompactSpaceItem>();
                foreach (var layoutChild in _contentLayout.Children)
                {
                    if (layoutChild is CompactSpaceItem compactSpaceItem)
                    {
                        if (oldItems.Contains(compactSpaceItem.Child!))
                        {
                            oldCompactSpaceWrappers.Add(compactSpaceItem);
                            oldLayoutChildren.Add(compactSpaceItem);
                        }
                    }
                    else if (oldItems.Contains(layoutChild))
                    {
                        layoutChild.SetTemplatedParent(null);
                        oldLayoutChildren.Add(layoutChild);
                    }
                }
                foreach (var compactSpaceItem in oldCompactSpaceWrappers)
                {
                    NotifyRemoveCompactSpaceItem(compactSpaceItem);
                }
                _contentLayout.Children.RemoveAll(oldLayoutChildren);
                break;
            }

            case NotifyCollectionChangedAction.Replace:
            case NotifyCollectionChangedAction.Reset:
                throw new NotSupportedException();
        }
        
        ConfigureSizeDefinitions();
        _childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.ChildIndexesReset);
        InvalidateMeasureOnChildrenChanged();
    }

    private static void EnsureCompactSpaceItem(object item)
    {
        if (item is not ICompactSpaceAware)
        {
            throw new ArgumentException($"{item.GetType().FullName} is not ICompactSpaceAware.");
        }
    }

    private Control CreateLayoutChild(Control child)
    {
        if (child is CompactSpaceFiller)
        {
            child.SetTemplatedParent(TemplatedParent);
            return child;
        }

        var compactSpaceItem = new CompactSpaceItem
        {
            Child = child
        };
        NotifyAddCompactSpaceItem(compactSpaceItem);
        return compactSpaceItem;
    }

    private void NotifyAddCompactSpaceItem(CompactSpaceItem compactSpaceItem)
    {
        if (compactSpaceItem.Child != null)
        {
            var target                  = compactSpaceItem.Child;
            var targetCompactSpaceAware = target as ICompactSpaceAware;

            if (_childPropertyChangedTargets.Add(target))
            {
                target.PropertyChanged += CompactSpaceItemChildPropertyChanged;
            }
            SetItemSize(compactSpaceItem, GetItemSize(target));

            if (targetCompactSpaceAware != null && targetCompactSpaceAware.IsAlwaysActiveZIndex())
            {
                compactSpaceItem.ZIndex = ACTIVE_ZINDEX;
            }

            if (target is ISizeTypeAware)
            {
                if (_childSizeTypeBindings.Remove(target, out var existingSizeTypeBinding))
                {
                    existingSizeTypeBinding.Dispose();
                }
                _childSizeTypeBindings[target] = target.Bind(SizeTypeProperty, this.GetObservable(SizeTypeProperty), BindingPriority.LocalValue);
            }
            target.SetTemplatedParent(TemplatedParent);
        }
    }
    
    private void NotifyRemoveCompactSpaceItem(CompactSpaceItem compactSpaceItem)
    {
        if (compactSpaceItem.Child != null)
        {
            var target                  = compactSpaceItem.Child;
            DetachInteractionHandlers(target);
            if (_childPropertyChangedTargets.Remove(target))
            {
                target.PropertyChanged -= CompactSpaceItemChildPropertyChanged;
            }
        
            if (_childClassesChangedHandlers.TryGetValue(target, out var childClassesChangedHandler))
            {
                target.Classes.CollectionChanged -= childClassesChangedHandler;
                _childClassesChangedHandlers.Remove(target);
            }
            if (target is ICompactSpaceAware targetCompactSpaceAware)
            {
                targetCompactSpaceAware.NotifyPositionChange(null);
            }
            if (target is ISizeTypeAware)
            {
                if (_childSizeTypeBindings.Remove(target, out var sizeTypeBinding))
                {
                    sizeTypeBinding.Dispose();
                }
            }
            target.SetTemplatedParent(null);
            compactSpaceItem.Child = null;
        }
        
    }

    private void CompactSpaceItemChildPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (sender is Control currentControl && sender is ICompactSpaceAware)
        {
            var compactSpaceItem = currentControl.FindAncestorOfType<CompactSpaceItem>();
            if (compactSpaceItem != null && e.Property == ItemSizeProperty && e.NewValue is CompactSpaceSize compactSpaceSize)
            {
                SetItemSize(compactSpaceItem, compactSpaceSize);
                ConfigureSizeDefinitions();
                InvalidateMeasureOnChildrenChanged();
            }
        }
    }
    
    #region 孩子的 ZIndex 处理方法

    private void HandleGotFocus(object? sender, FocusChangedEventArgs e)
    {
        if (sender is Control currentControl && _contentLayout != null)
        {
            foreach (var child in _contentLayout.Children)
            {
                if (child is CompactSpaceItem compactSpaceItem && compactSpaceItem.Child is ICompactSpaceAware compactSpaceAware)
                {
                    if (compactSpaceItem.Child == currentControl)
                    {
                        compactSpaceItem.ZIndex = ACTIVE_ZINDEX;
                    }
                    else if (!compactSpaceAware.IsAlwaysActiveZIndex())
                    {
                        compactSpaceItem.ZIndex = NORMAL_ZINDEX;
                    }
                }
            }
        }
    }

    private void HandlePointerEntered(object? sender, PointerEventArgs e)
    {
        if (sender is Control currentControl && _contentLayout != null)
        {
            foreach (var child in _contentLayout.Children)
            {
                if (child is CompactSpaceItem compactSpaceItem && compactSpaceItem.Child is ICompactSpaceAware compactSpaceAware)
                {
                    if (compactSpaceItem.Child == currentControl)
                    {
                        compactSpaceItem.ZIndex = ACTIVE_ZINDEX;
                    }
                    else  if (!IsEffectiveFocused(compactSpaceItem) && !compactSpaceAware.IsAlwaysActiveZIndex())
                    {
                        compactSpaceItem.ZIndex = NORMAL_ZINDEX;
                    }
                }
            }
        }
    }

    private void HandlePointerExited(object? sender, PointerEventArgs e)
    {
        if (sender is Control currentControl &&
            TryFindCompactSpaceItem(currentControl, out var compactSpaceItem) &&
            compactSpaceItem.Child is ICompactSpaceAware compactSpaceAware)
        {
            if (!IsEffectiveFocused(compactSpaceItem) && !compactSpaceAware.IsAlwaysActiveZIndex())
            {
                compactSpaceItem.ZIndex = NORMAL_ZINDEX;
            }
        }
    }

    private void HandleChildFocusWithinChanged(CompactSpaceItem compactSpaceItem, Control target, bool focused)
    {
        if (focused)
        {
            compactSpaceItem.ZIndex = ACTIVE_ZINDEX;
        }
        else if (!target.IsPointerOver &&
                 compactSpaceItem.Child is ICompactSpaceAware compactSpaceAware &&
                 !compactSpaceAware.IsAlwaysActiveZIndex())
        {
            compactSpaceItem.ZIndex = NORMAL_ZINDEX;
        }
    }

    private bool IsEffectiveFocused(CompactSpaceItem control)
    {
        if (control.Child == null)
        {
            return false;
        }
        var target = control.Child;
        return target.Classes.Contains(StdPseudoClass.FocusWithIn) || target.IsFocused;
    }

    private bool TryFindCompactSpaceItem(Control target, out CompactSpaceItem compactSpaceItem)
    {
        if (_contentLayout != null)
        {
            foreach (var child in _contentLayout.Children)
            {
                if (child is CompactSpaceItem currentItem && currentItem.Child == target)
                {
                    compactSpaceItem = currentItem;
                    return true;
                }
            }
        }

        compactSpaceItem = null!;
        return false;
    }
    #endregion
    
    private protected virtual void InvalidateMeasureOnChildrenChanged()
    {
        InvalidateMeasure();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        DetachCompactSpaceItems();
        _contentLayout = e.NameScope.Get<Grid>("PART_ContentLayout");
        ValidateFillerUsage();
        var layoutChildren = new List<Control>();
        foreach (var child in Children)
        {
            EnsureCompactSpaceItem(child);
            layoutChildren.Add(CreateLayoutChild(child));
        }
        _contentLayout.Children.AddRange(layoutChildren);

        ConfigureSizeDefinitions();
        _childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.ChildIndexesReset);
        InvalidateMeasureOnChildrenChanged();
    }

    protected override Size MeasureCore(Size availableSize)
    {
        return base.MeasureCore(availableSize);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearChildClassesChangedHandlers();
        DetachAllInteractionHandlers();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateInteractionTracking();
    }

    private void ClearChildClassesChangedHandlers()
    {
        foreach (var (target, handler) in _childClassesChangedHandlers)
        {
            if (target is Control control)
            {
                control.Classes.CollectionChanged -= handler;
            }
        }
        _childClassesChangedHandlers.Clear();
    }

    private void RegisterChildClassesChangedHandler(CompactSpaceItem compactSpaceItem, Control target)
    {
        if (_childClassesChangedHandlers.ContainsKey(target))
        {
            return;
        }

        NotifyCollectionChangedEventHandler handler = (_, _) =>
        {
            HandleChildFocusWithinChanged(compactSpaceItem, target, target.Classes.Contains(StdPseudoClass.FocusWithIn));
        };
        _childClassesChangedHandlers.Add(target, handler);
        target.Classes.CollectionChanged += handler;
    }

    private void UnregisterChildClassesChangedHandler(Control target)
    {
        if (_childClassesChangedHandlers.TryGetValue(target, out var childClassesChangedHandler))
        {
            target.Classes.CollectionChanged -= childClassesChangedHandler;
            _childClassesChangedHandlers.Remove(target);
        }
    }

    private void AttachInteractionHandlers(Control target)
    {
        if (_childInteractionTargets.Add(target))
        {
            target.GotFocus       += HandleGotFocus;
            target.PointerEntered += HandlePointerEntered;
            target.PointerExited  += HandlePointerExited;
        }
    }

    private void DetachInteractionHandlers(Control target)
    {
        if (_childInteractionTargets.Remove(target))
        {
            target.GotFocus       -= HandleGotFocus;
            target.PointerEntered -= HandlePointerEntered;
            target.PointerExited  -= HandlePointerExited;
        }
    }

    private void DetachAllInteractionHandlers()
    {
        foreach (var target in _childInteractionTargets.ToList())
        {
            target.GotFocus       -= HandleGotFocus;
            target.PointerEntered -= HandlePointerEntered;
            target.PointerExited  -= HandlePointerExited;
        }
        _childInteractionTargets.Clear();
    }

    private void DetachAllPropertyChangedHandlers()
    {
        foreach (var target in _childPropertyChangedTargets.ToList())
        {
            target.PropertyChanged -= CompactSpaceItemChildPropertyChanged;
        }
        _childPropertyChangedTargets.Clear();
    }

    private void DetachAllSizeTypeBindings()
    {
        foreach (var binding in _childSizeTypeBindings.Values)
        {
            binding.Dispose();
        }
        _childSizeTypeBindings.Clear();
    }

    private void UpdateInteractionTracking()
    {
        if (_contentLayout == null)
        {
            DetachAllInteractionHandlers();
            ClearChildClassesChangedHandlers();
            return;
        }

        var realItemCount = CountRealCompactItems(_contentLayout.Children);
        foreach (var child in _contentLayout.Children)
        {
            if (child is not CompactSpaceItem compactSpaceItem || compactSpaceItem.Child is not { } target)
            {
                continue;
            }

            var compactSpaceAware = target as ICompactSpaceAware;
            if (compactSpaceAware?.IsAlwaysActiveZIndex() == true)
            {
                compactSpaceItem.ZIndex = ACTIVE_ZINDEX;
            }
            else if (compactSpaceAware == null || compactSpaceAware.IgnoreZIndexChange() || realItemCount <= 1)
            {
                compactSpaceItem.ZIndex = NORMAL_ZINDEX;
            }

            if (ShouldTrackZIndex(target, realItemCount))
            {
                AttachInteractionHandlers(target);
                RegisterChildClassesChangedHandler(compactSpaceItem, target);
            }
            else
            {
                DetachInteractionHandlers(target);
                UnregisterChildClassesChangedHandler(target);
            }
        }
    }

    private static bool ShouldTrackZIndex(Control target, int realItemCount)
    {
        if (realItemCount <= 1)
        {
            return false;
        }
        if (target is not ICompactSpaceAware compactSpaceAware)
        {
            return false;
        }
        return !compactSpaceAware.IgnoreZIndexChange() && !compactSpaceAware.IsAlwaysActiveZIndex();
    }

    private static bool IsRealCompactItem(Control child)
    {
        return child is CompactSpaceItem { Child: not null } compactSpaceItem &&
               compactSpaceItem.Child is not CompactSpaceFiller;
    }

    private static int CountRealCompactItems(IReadOnlyList<Control> children)
    {
        var count = 0;
        for (var i = 0; i < children.Count; i++)
        {
            if (IsRealCompactItem(children[i]))
            {
                count++;
            }
        }
        return count;
    }

    private void ConfigureSizeDefinitions()
    {
        if (_contentLayout == null)
        {
            return;
        }
        ValidateFillerUsage();
        var children = _contentLayout.Children;
        if (children.Count == 0)
        {
            _sizeDefinitionsSignature = null;
            UpdateInteractionTracking();
            return;
        }

        var signature = CreateSizeDefinitionsSignature(children);
        if (_sizeDefinitionsSignature != signature)
        {
            if (Orientation == Orientation.Horizontal)
            {
                var columnDefinitions = new ColumnDefinitions();
                for (var i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    Grid.SetColumn(child, i);
                    var itemSize = GetItemSize(child);
                    if (itemSize.IsAbsolute)
                    {
                        columnDefinitions.Add(new ColumnDefinition(itemSize.Value, GridUnitType.Pixel));
                    }
                    else if (itemSize.IsAuto)
                    {
                        columnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
                    }
                    else
                    {
                        columnDefinitions.Add(new ColumnDefinition(itemSize.Value, GridUnitType.Star));
                    }
                }
                _contentLayout.ColumnDefinitions = columnDefinitions;
                _contentLayout.RowDefinitions    = new RowDefinitions();
            }
            else
            {
                var rowDefinitions = new RowDefinitions();
                for (var i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    Grid.SetRow(child, i);
                    var itemSize = GetItemSize(child);
                    if (itemSize.IsAbsolute)
                    {
                        rowDefinitions.Add(new RowDefinition(itemSize.Value, GridUnitType.Pixel));
                    }
                    else if (itemSize.IsAuto)
                    {
                        rowDefinitions.Add(new RowDefinition(GridLength.Auto));
                    }
                    else
                    {
                        rowDefinitions.Add(new RowDefinition(itemSize.Value, GridUnitType.Star));
                    }
                }
                _contentLayout.RowDefinitions    = rowDefinitions;
                _contentLayout.ColumnDefinitions = new ColumnDefinitions();
            }

            _sizeDefinitionsSignature = signature;
        }

        ConfigureItemPositions(children);
        UpdateInteractionTracking();
    }

    private int CreateSizeDefinitionsSignature(IReadOnlyList<Control> children)
    {
        var hash = new HashCode();
        hash.Add(Orientation);
        hash.Add(children.Count);
        for (var i = 0; i < children.Count; i++)
        {
            var child = children[i];
            hash.Add(RuntimeHelpers.GetHashCode(child));
            hash.Add(GetItemSize(child));
        }
        return hash.ToHashCode();
    }

    private void ConfigureItemPositions(IReadOnlyList<Control> children)
    {
        var realChildrenCount = CountRealCompactItems(children);
        if (realChildrenCount == 0)
        {
            return;
        }

        var realIndex = 0;
        for (var i = 0; i < children.Count; i++)
        {
            if (children[i] is not CompactSpaceItem compactSpaceItem ||
                compactSpaceItem.Child is CompactSpaceFiller ||
                compactSpaceItem.Child is null)
            {
                continue;
            }

            if (compactSpaceItem.PositionIndex != realIndex)
            {
                compactSpaceItem.PositionIndex = realIndex;
            }

            var position = realChildrenCount == 1
                ? SpaceItemPosition.First | SpaceItemPosition.Last
                : realIndex == 0
                    ? SpaceItemPosition.First
                    : realIndex == realChildrenCount - 1
                        ? SpaceItemPosition.Last
                        : SpaceItemPosition.Middle;

            ((ICompactSpaceAware)compactSpaceItem).NotifyOrientationChange(Orientation);
            ((ICompactSpaceAware)compactSpaceItem).NotifyPositionChange(position);
            realIndex++;
        }
    }

    private void ValidateFillerUsage()
    {
        var fillerCount = 0;
        for (var i = 0; i < Children.Count; i++)
        {
            if (Children[i] is not CompactSpaceFiller)
            {
                continue;
            }

            fillerCount++;
            if (fillerCount > 1)
            {
                throw new InvalidSpaceFillerUsageException("There can only be one CompactSpaceFiller.");
            }
            if (i != Children.Count - 1)
            {
                throw new InvalidSpaceFillerUsageException("The CompactSpaceFiller is misplaced, it can only be the last child.");
            }
        }
    }

    private void DetachCompactSpaceItems()
    {
        if (_contentLayout == null)
        {
            ClearChildClassesChangedHandlers();
            DetachAllInteractionHandlers();
            DetachAllPropertyChangedHandlers();
            DetachAllSizeTypeBindings();
            _sizeDefinitionsSignature = null;
            return;
        }

        foreach (var layoutChild in _contentLayout.Children)
        {
            if (layoutChild is CompactSpaceItem compactSpaceItem)
            {
                NotifyRemoveCompactSpaceItem(compactSpaceItem);
            }
            else
            {
                layoutChild.SetTemplatedParent(null);
            }
        }
        _contentLayout.Children.Clear();
        ClearChildClassesChangedHandlers();
        DetachAllInteractionHandlers();
        DetachAllPropertyChangedHandlers();
        DetachAllSizeTypeBindings();
        _sizeDefinitionsSignature = null;
    }
    
    private void HandleChildrenPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Children.Count) || e.PropertyName is null)
        {
            _childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.TotalCountChanged);
        }
    }
    
    int IChildIndexProvider.GetChildIndex(ILogical child)
    {
        return child is Control control ? Children.IndexOf(control) : -1;
    }
    
    bool IChildIndexProvider.TryGetTotalCount(out int count)
    {
        count = Children.Count;
        return true;
    }
    
    IInputElement? INavigableContainer.GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
    {
        var  orientation = Orientation;
        var  children    = Children;
        bool horiz       = orientation == Orientation.Horizontal;
        int  index       = from is not null ? Children.IndexOf((Control)from) : -1;

        switch (direction)
        {
            case NavigationDirection.First:
                index = 0;
                break;
            case NavigationDirection.Last:
                index = children.Count - 1;
                break;
            case NavigationDirection.Next:
                ++index;
                break;
            case NavigationDirection.Previous:
                --index;
                break;
            case NavigationDirection.Left:
                index = horiz ? index - 1 : -1;
                break;
            case NavigationDirection.Right:
                index = horiz ? index + 1 : -1;
                break;
            case NavigationDirection.Up:
                index = horiz ? -1 : index - 1;
                break;
            case NavigationDirection.Down:
                index = horiz ? -1 : index + 1;
                break;
        }

        if (index >= 0 && index < children.Count)
        {
            return children[index];
        }
        return null;
    }

    internal static CornerRadius CalculateEffectiveCornerRadius(CornerRadius cornerRadius, 
                                                                bool isUsedInCompactSpace,
                                                                SpaceItemPosition? compactSpaceItemPosition,
                                                                Orientation compactSpaceOrientation)
    {
        var topLeft     = cornerRadius.TopLeft;
        var topRight    = cornerRadius.TopRight;
        var bottomRight = cornerRadius.BottomRight;
        var bottomLeft  = cornerRadius.BottomLeft;
        if (isUsedInCompactSpace &&
            compactSpaceItemPosition.HasValue &&
            (!compactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.First) || !compactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Last)))
        {
            if (compactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.First))
            {
                if (compactSpaceOrientation == Orientation.Horizontal)
                {
                    topRight    = 0;
                    bottomRight = 0;
                }
                else
                {
                    bottomLeft  = 0;
                    bottomRight = 0;
                }
            }
            else if (compactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Middle))
            {
                topRight    = 0;
                topLeft     = 0;
                bottomLeft  = 0;
                bottomRight = 0;
            }
            else if (compactSpaceItemPosition.Value.HasFlag(SpaceItemPosition.Last))
            {
                if (compactSpaceOrientation == Orientation.Horizontal)
                {
                    topLeft    = 0;
                    bottomLeft = 0;
                }
                else
                {
                    topLeft  = 0;
                    topRight = 0;
                }
            }
        }
        return new CornerRadius(topLeft, topRight, bottomRight, bottomLeft);
    }

    private void HandleTemplatedParentChanged()
    {
        foreach (var child in Children)
        {
            child.SetTemplatedParent(TemplatedParent);
        }
    }
}
