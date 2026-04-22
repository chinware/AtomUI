using System.Collections.Specialized;
using System.ComponentModel;
using AtomUI.Controls;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Reflection;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
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
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
            {
                var compactSpaceItems = new List<CompactSpaceItem>();
                foreach (var newItem in e.NewItems!.OfType<Control>())
                {
                    EnsureCompactSpaceItem(newItem);
                    var compactSpaceItem = new CompactSpaceItem()
                    {
                        Child = newItem
                    };
                    NotifyAddCompactSpaceItem(compactSpaceItem);
                    compactSpaceItems.Add(compactSpaceItem);
                }
                _contentLayout.Children.InsertRange(e.NewStartingIndex, compactSpaceItems);
                break;
            }

            case NotifyCollectionChangedAction.Move:
                _contentLayout.Children.MoveRange(e.OldStartingIndex, e.OldItems!.Count, e.NewStartingIndex);
                break;

            case NotifyCollectionChangedAction.Remove:
            {
                var oldItems             = new HashSet<Control>(e.OldItems!.OfType<Control>());
                var oldCompactSpaceItems = new List<CompactSpaceItem>();
                foreach (var oldItem in _contentLayout.Children)
                {
                    if (oldItem is CompactSpaceItem compactSpaceItem)
                    {
                        if (oldItems.Contains(compactSpaceItem.Child!))
                        {
                            oldCompactSpaceItems.Add(compactSpaceItem);
                        }
                    }
                }
                foreach (var compactSpaceItem in oldCompactSpaceItems)
                {
                    NotifyRemoveCompactSpaceItem(compactSpaceItem);
                }
                _contentLayout.Children.RemoveAll(oldCompactSpaceItems);
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

    private void NotifyAddCompactSpaceItem(CompactSpaceItem compactSpaceItem)
    {
        if (compactSpaceItem.Child != null)
        {
            var target                  = compactSpaceItem.Child;
            var targetCompactSpaceAware = target as ICompactSpaceAware;
            if (targetCompactSpaceAware != null && !targetCompactSpaceAware.IgnoreZIndexChange())
            {
                target.GotFocus       += HandleGotFocus;
                target.PointerEntered += HandlePointerEntered;
                target.PointerExited  += HandlePointerExited;
            }

            target.PropertyChanged += CompactSpaceItemChildPropertyChanged;
            SetItemSize(compactSpaceItem, GetItemSize(target));

            RegisterChildClassesChangedHandler(compactSpaceItem, target);
            if (targetCompactSpaceAware != null && targetCompactSpaceAware.IsAlwaysActiveZIndex())
            {
                compactSpaceItem.ZIndex = ACTIVE_ZINDEX;
            }

            if (target is ISizeTypeAware)
            {
                target[!SizeTypeProperty] = this[!SizeTypeProperty];
            }
            target.SetTemplatedParent(TemplatedParent);
        }
    }
    
    private void NotifyRemoveCompactSpaceItem(CompactSpaceItem compactSpaceItem)
    {
        if (compactSpaceItem.Child != null)
        {
            var target                  = compactSpaceItem.Child;
            var targetCompactSpaceAware = target as ICompactSpaceAware;
            if (targetCompactSpaceAware != null && !targetCompactSpaceAware.IgnoreZIndexChange())
            {
                target.GotFocus       -= HandleGotFocus;
                target.PointerEntered -= HandlePointerEntered;
                target.PointerExited  -= HandlePointerExited;
            }
            target.PropertyChanged -= CompactSpaceItemChildPropertyChanged;
        
            if (_childClassesChangedHandlers.TryGetValue(target, out var childClassesChangedHandler))
            {
                target.Classes.CollectionChanged -= childClassesChangedHandler;
                _childClassesChangedHandlers.Remove(target);
            }
            target.SetTemplatedParent(null);
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
            }
        }
    }
    
    #region 孩子的 ZIndex 处理方法

    private void HandleGotFocus(object? sender, GotFocusEventArgs e)
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
        if (sender is CompactSpaceItem compactSpaceItem && compactSpaceItem.Child is ICompactSpaceAware compactSpaceAware)
        {
            if (!IsEffectiveFocused(compactSpaceItem) && !compactSpaceAware.IsAlwaysActiveZIndex())
            {
                compactSpaceItem.ZIndex = NORMAL_ZINDEX;
            }
        }
    }

    private void HandleChildFocusWithinChanged(Control control, bool focused)
    {
        if (focused)
        {
            control.ZIndex = ACTIVE_ZINDEX;
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
    #endregion
    
    private protected virtual void InvalidateMeasureOnChildrenChanged()
    {
        InvalidateMeasure();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ClearChildClassesChangedHandlers();
        _contentLayout = e.NameScope.Get<Grid>(CompactSpaceThemeConstants.ContentLayoutPart);
        var compactSpaceItems = new List<CompactSpaceItem>();
        foreach (var child in Children)
        {
            EnsureCompactSpaceItem(child);
            var compactSpaceItem = new CompactSpaceItem()
            {
                Child = child,
            };
            NotifyAddCompactSpaceItem(compactSpaceItem);
            compactSpaceItems.Add(compactSpaceItem);
        }
        _contentLayout.Children.AddRange(compactSpaceItems);

        ConfigureSizeDefinitions();
        _childIndexChanged?.Invoke(this, ChildIndexChangedEventArgs.ChildIndexesReset);
        InvalidateMeasureOnChildrenChanged();
    }

    protected override Size MeasureCore(Size availableSize)
    {
        if (_contentLayout != null)
        {
            // 检查 filler 位置和数量
            var fillerCount   = 0;
            var childrenCount = _contentLayout.Children.Count;
            for (var i = 0; i < childrenCount; i++)
            {
                var child = _contentLayout.Children[i];
                if (child is CompactSpaceFiller)
                {
                    ++fillerCount;
                    if (i != fillerCount - 1)
                    {
                        throw new InvalidSpaceFillerUsageException("The CompactSpaceFiller is misplaced, it can only be the last child.");
                    }
                }
            }

            if (fillerCount > 1)
            {
                throw new InvalidSpaceFillerUsageException("There can only be one CompactSpaceFiller.");
            }
        }
        return base.MeasureCore(availableSize);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearChildClassesChangedHandlers();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ReRegisterChildClassesChangedHandlers();
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

    private void ReRegisterChildClassesChangedHandlers()
    {
        if (_contentLayout == null)
        {
            return;
        }

        foreach (var child in _contentLayout.Children)
        {
            if (child is CompactSpaceItem compactSpaceItem && compactSpaceItem.Child is { } target)
            {
                RegisterChildClassesChangedHandler(compactSpaceItem, target);
            }
        }
    }

    private void RegisterChildClassesChangedHandler(CompactSpaceItem compactSpaceItem, Control target)
    {
        if (_childClassesChangedHandlers.ContainsKey(target))
        {
            return;
        }

        NotifyCollectionChangedEventHandler handler = (_, _) =>
        {
            HandleChildFocusWithinChanged(compactSpaceItem, target.Classes.Contains(StdPseudoClass.FocusWithIn));
        };
        _childClassesChangedHandlers.Add(target, handler);
        target.Classes.CollectionChanged += handler;
    }

    private void ConfigureSizeDefinitions()
    {
        if (_contentLayout == null || _contentLayout.Children.Count == 0)
        {
            return;
        }
        var children = _contentLayout.Children;
        if (Orientation == Orientation.Horizontal)
        {
            var columnDefinitions = new ColumnDefinitions();
            for (var i = 0; i < children.Count; i++)
            {
                var child    = children[i];
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
        }
        else
        {
            var rowDefinitions = new RowDefinitions();
            for (var i = 0; i < children.Count; i++)
            {
                var child    = children[i];
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
            _contentLayout.RowDefinitions = rowDefinitions;
        }
        
        // 计算位置
        var realChildren = children.Where(child => child is CompactSpaceItem compactSpaceItem && compactSpaceItem.Child is not CompactSpaceFiller).ToList();
        if (realChildren.Count == 1)
        {
            if (realChildren[0] is ICompactSpaceAware compactSpaceAware)
            {
                compactSpaceAware.NotifyPositionChange(SpaceItemPosition.First | SpaceItemPosition.Last);
            }
        }
        else
        {
            for (var i = 0; i < realChildren.Count; i++)
            {
                var child = realChildren[i];
                if (child is CompactSpaceItem compactSpaceItem)
                {
                    compactSpaceItem.PositionIndex = i;
                }
                if (child is ICompactSpaceAware compactSpaceAware)
                {
                    compactSpaceAware.NotifyOrientationChange(Orientation);
                    if (i == 0)
                    {
                        compactSpaceAware.NotifyPositionChange(SpaceItemPosition.First);
                    }
                    else if (i == realChildren.Count - 1)
                    {
                        compactSpaceAware.NotifyPositionChange(SpaceItemPosition.Last);
                    }
                    else
                    {
                        compactSpaceAware.NotifyPositionChange(SpaceItemPosition.Middle);
                    }
                }
            }
        }
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