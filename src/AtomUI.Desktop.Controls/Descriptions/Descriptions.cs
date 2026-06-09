using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public class Descriptions : TemplatedControl, ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsBorderedProperty =
        AvaloniaProperty.Register<Descriptions, bool>(nameof(IsBordered));

    public static readonly StyledProperty<bool> IsShowColonProperty =
        AvaloniaProperty.Register<Descriptions, bool>(nameof(IsShowColon), true);

    public static readonly StyledProperty<DescriptionsMediaBreakInfo> ColumnInfoProperty =
        AvaloniaProperty.Register<Descriptions, DescriptionsMediaBreakInfo>(nameof(ColumnInfo),
            new DescriptionsMediaBreakInfo(3));

    public static readonly StyledProperty<object?> ExtraProperty =
        AvaloniaProperty.Register<Descriptions, object?>(nameof(Extra));

    public static readonly StyledProperty<IDataTemplate?> ExtraTemplateProperty =
        AvaloniaProperty.Register<Descriptions, IDataTemplate?>(nameof(ExtraTemplate));

    public static readonly StyledProperty<Orientation> LayoutProperty =
        AvaloniaProperty.Register<Descriptions, Orientation>(nameof(Layout));

    public static readonly StyledProperty<SizeType> SizeTypeProperty =
        SizeTypeControlProperty.SizeTypeProperty.AddOwner<Descriptions>();

    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<Descriptions, object?>(nameof(Header));

    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<Descriptions, IDataTemplate?>(nameof(HeaderTemplate));

    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<Descriptions, IEnumerable?>(nameof(ItemsSource));

    public bool IsBordered
    {
        get => GetValue(IsBorderedProperty);
        set => SetValue(IsBorderedProperty, value);
    }

    public bool IsShowColon
    {
        get => GetValue(IsShowColonProperty);
        set => SetValue(IsShowColonProperty, value);
    }

    public DescriptionsMediaBreakInfo ColumnInfo
    {
        get => GetValue(ColumnInfoProperty);
        set => SetValue(ColumnInfoProperty, value);
    }

    [DependsOn(nameof(ExtraTemplate))]
    public object? Extra
    {
        get => GetValue(ExtraProperty);
        set => SetValue(ExtraProperty, value);
    }

    public IDataTemplate? ExtraTemplate
    {
        get => GetValue(ExtraTemplateProperty);
        set => SetValue(ExtraTemplateProperty, value);
    }

    public Orientation Layout
    {
        get => GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    public SizeType SizeType
    {
        get => GetValue(SizeTypeProperty);
        set => SetValue(SizeTypeProperty, value);
    }

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private DescriptionItems _items = new();

    [Content]
    public DescriptionItems Items
    {
        get => _items;
        set
        {
            if (ReferenceEquals(_items, value))
            {
                return;
            }

            _items.CollectionChanged -= HandleCollectionChanged;
            _items = value ?? new DescriptionItems();
            _items.CollectionChanged += HandleCollectionChanged;
            if (_gridLayout != null && this.IsAttachedToVisualTree())
            {
                _gridLayout.Children.Clear();
                AddDescriptionItems((IEnumerable<DescriptionItem>)_items);
                DoLayoutChildren();
                InvalidateMeasure();
            }
        }
    }

    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<Descriptions, bool> IsHeaderLayoutVisibleProperty =
        AvaloniaProperty.RegisterDirect<Descriptions, bool>(nameof(IsHeaderLayoutVisible),
            o => o.IsHeaderLayoutVisible,
            (o, v) => o.IsHeaderLayoutVisible = v);

    private bool _isHeaderLayoutVisible;

    internal bool IsHeaderLayoutVisible
    {
        get => _isHeaderLayoutVisible;
        set => SetAndRaise(IsHeaderLayoutVisibleProperty, ref _isHeaderLayoutVisible, value);
    }

    #endregion

    private Grid? _gridLayout;
    private MediaBreakPoint? _breakPoint;
    private int _effectiveColumns;
    private IMediaBreakAwareControl? _mediaOwner;

    static Descriptions()
    {
        SizeTypeProperty.OverrideDefaultValue<Descriptions>(SizeType.Large);
    }

    public Descriptions()
    {
        this.RegisterTokenResourceScope(DescriptionsToken.ScopeProvider);
        _items.CollectionChanged += HandleCollectionChanged;
    }
    
    protected virtual void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_gridLayout != null && this.IsAttachedToVisualTree())
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        AddDescriptionItems(e.NewItems, e.NewStartingIndex);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        RemoveDescriptionItems(e.OldItems, e.OldStartingIndex);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    throw new NotSupportedException();
                case NotifyCollectionChangedAction.Reset:
                    _gridLayout.Children.Clear();
                    break;
            }

            DoLayoutChildren();
            InvalidateMeasure();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (MediaQueryHost.FindOwner(this) is { } mediaOwner)
        {
            _mediaOwner = mediaOwner;
            _breakPoint = mediaOwner.MediaBreakPoint;
            mediaOwner.MediaBreakPointChanged += HandleMediaBreakChanged;
            var columns = GetColumnsForMediaBreak(_breakPoint.Value);
            UpdateGridColumns(columns, true);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (_mediaOwner != null)
        {
            _mediaOwner.MediaBreakPointChanged -= HandleMediaBreakChanged;
        }

        _mediaOwner = null;
    }

    private void HandleMediaBreakChanged(object? sender, MediaBreakPointChangedEventArgs args)
    {
        _breakPoint = args.MediaBreakPoint;
        if (_breakPoint != null)
        {
            var columns = GetColumnsForMediaBreak(_breakPoint.Value);
            UpdateGridColumns(columns, true);
        }
    }

    private int GetColumnsForMediaBreak(MediaBreakPoint breakPoint)
    {
        var columns = 1;
        if (breakPoint == MediaBreakPoint.ExtraSmall)
        {
            columns = ColumnInfo.ExtraSmall;
        }
        else if (breakPoint == MediaBreakPoint.Small)
        {
            columns = ColumnInfo.Small;
        }
        else if (breakPoint == MediaBreakPoint.Medium)
        {
            columns = ColumnInfo.Medium;
        }
        else if (breakPoint == MediaBreakPoint.Large)
        {
            columns = ColumnInfo.Large;
        }
        else if (breakPoint == MediaBreakPoint.ExtraLarge)
        {
            columns = ColumnInfo.ExtraLarge;
        }
        else if (breakPoint == MediaBreakPoint.ExtraExtraLarge)
        {
            columns = ColumnInfo.ExtraExtraLarge;
        }

        return columns;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _gridLayout = e.NameScope.Find<Grid>("PART_GridLayout");
        AddDescriptionItems((IEnumerable<DescriptionItem>)Items);
        if (MediaQueryHost.FindOwner(this) is { } mediaOwner)
        {
            _breakPoint = mediaOwner.MediaBreakPoint;
            var columns = GetColumnsForMediaBreak(_breakPoint.Value);
            UpdateGridColumns(columns, true);
        }
        else
        {
            DoLayoutChildren();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ItemsSourceProperty)
        {
            if (ItemsSource != null)
            {
                Items.Clear();
                AddItemsSourceItems(ItemsSource);
            }
        }
        else if (change.Property == IsBorderedProperty)
        {
            HandleBorderedChanged();
        }
        else if (change.Property == HeaderProperty ||
                 change.Property == ExtraProperty)
        {
            SetCurrentValue(IsHeaderLayoutVisibleProperty, Header != null || Extra != null);
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == LayoutProperty)
            {
                HandleLayoutChanged();
            }
        }
    }

    private void AddDescriptionItems(IEnumerable<DescriptionItem> items)
    {
        if (_gridLayout != null)
        {
            foreach (DescriptionItem item in items)
            {
                AddDescriptionItem(item);
            }
        }
    }

    private void AddDescriptionItems(IList items, int startingIndex)
    {
        if (_gridLayout != null)
        {
            for (var i = 0; i < items.Count; i++)
            {
                AddDescriptionItem((DescriptionItem)items[i]!, GetGridChildIndex(startingIndex + i));
            }
        }
    }

    private void AddItemsSourceItems(IEnumerable source)
    {
        var items = CollectItemsSourceItems(source);
        if (items is not null)
        {
            Items.AddRange(items);
        }
    }

    private static List<DescriptionItem>? CollectItemsSourceItems(IEnumerable source)
    {
        List<DescriptionItem>? items = null;
        if (source is IList list)
        {
            for (var i = 0; i < list.Count; ++i)
            {
                if (list[i] is DescriptionItem item)
                {
                    items ??= new List<DescriptionItem>(list.Count - i);
                    items.Add(item);
                }
            }

            return items;
        }

        if (source is IReadOnlyList<DescriptionItem> readOnlyList)
        {
            for (var i = 0; i < readOnlyList.Count; ++i)
            {
                if (readOnlyList[i] is DescriptionItem item)
                {
                    items ??= new List<DescriptionItem>(readOnlyList.Count - i);
                    items.Add(item);
                }
            }

            return items;
        }

        foreach (var value in source)
        {
            if (value is DescriptionItem item)
            {
                items ??= new List<DescriptionItem>();
                items.Add(item);
            }
        }

        return items;
    }

    private void RemoveDescriptionItems(IList items, int startingIndex)
    {
        if (_gridLayout != null)
        {
            var removeIndex = GetGridChildIndex(startingIndex);
            var removeCount = GetGridChildCount(items.Count);
            for (var i = 0; i < removeCount && removeIndex < _gridLayout.Children.Count; i++)
            {
                _gridLayout.Children.RemoveAt(removeIndex);
            }
        }
    }

    private void AddDescriptionItem(DescriptionItem item, int? gridChildIndex = null)
    {
        if (_gridLayout == null)
        {
            return;
        }

        if (Layout == Orientation.Horizontal)
        {
            if (IsBordered)
            {
                var itemLabel   = new DescriptionBorderedItemLabel();
                var itemContent = new DescriptionBorderedItemContent();
                itemLabel[!SizeTypeProperty] = this[!SizeTypeProperty];
                itemContent[!SizeTypeProperty] = this[!SizeTypeProperty];
                itemLabel.Content   = item.Label;
                itemContent.Content = item.Content;
                AddGridChild(itemLabel, gridChildIndex);
                AddGridChild(itemContent, gridChildIndex.HasValue ? gridChildIndex.Value + 1 : null);
            }
            else
            {
                var disposables            = new CompositeDisposable(2);
                var descriptionDefaultItem = new DescriptionDefaultItem();
                descriptionDefaultItem.Layout = Orientation.Horizontal;
                disposables.Add(BindUtils.RelayBind(this, IsShowColonProperty, descriptionDefaultItem,
                    DescriptionDefaultItem.IsColonVisibleProperty));
                descriptionDefaultItem.Header  = item.Label;
                descriptionDefaultItem.Content = item.Content;
                AddGridChild(descriptionDefaultItem, gridChildIndex);
            }
        }
        else
        {
            var descriptionDefaultItem = new DescriptionDefaultItem();
            descriptionDefaultItem.Layout                                          = Orientation.Vertical;
            descriptionDefaultItem[!DescriptionDefaultItem.IsColonVisibleProperty] = this[!IsShowColonProperty];
            descriptionDefaultItem[!DescriptionDefaultItem.IsBorderedProperty]     = this[!IsBorderedProperty];
            descriptionDefaultItem.Header                                          = item.Label;
            descriptionDefaultItem.Content                                         = item.Content;
            AddGridChild(descriptionDefaultItem, gridChildIndex);
        }
    }

    private void AddGridChild(Control child, int? gridChildIndex)
    {
        if (_gridLayout == null)
        {
            return;
        }

        if (gridChildIndex.HasValue && gridChildIndex.Value <= _gridLayout.Children.Count)
        {
            _gridLayout.Children.Insert(gridChildIndex.Value, child);
        }
        else
        {
            _gridLayout.Children.Add(child);
        }
    }

    private int GetGridChildIndex(int itemIndex)
    {
        return Layout == Orientation.Horizontal && IsBordered ? itemIndex * 2 : itemIndex;
    }

    private int GetGridChildCount(int itemCount)
    {
        return Layout == Orientation.Horizontal && IsBordered ? itemCount * 2 : itemCount;
    }

    private void UpdateGridColumns(int columnCount, bool forceLayout = false)
    {
        var effectiveColumns = columnCount;
        if (Layout == Orientation.Horizontal && IsBordered)
        {
            effectiveColumns = columnCount * 2;
        }

        if (effectiveColumns != _effectiveColumns)
        {
            _effectiveColumns = effectiveColumns;
            forceLayout       = true;
        }

        if (forceLayout)
        {
            DoLayoutChildren();
        }
    }

    private void HandleBorderedChanged()
    {
        if (Layout == Orientation.Horizontal)
        {
            if (_breakPoint == null)
            {
                if (MediaQueryHost.FindOwner(this) is { } mediaOwner)
                {
                    _breakPoint = mediaOwner.MediaBreakPoint;
                }
            }

            if (_breakPoint == null)
            {
                return;
            }

            var columns = GetColumnsForMediaBreak(_breakPoint.Value);
            UpdateGridColumns(columns);
        }
    }

    private void DoLayoutChildren()
    {
        if (_gridLayout != null)
        {
            EnsureEffectiveColumns();
            var row    = 0;
            var column = 0;
            for (var i = 0; i < Items.Count; i++)
            {
                var item  = Items[i];
                var index = Items.IndexOf(item);
                if (index != -1)
                {
                    if (Layout == Orientation.Horizontal)
                    {
                        if (IsBordered)
                        {
                            index *= 2;
                            if (_gridLayout.Children[index] is DescriptionBorderedItemLabel itemLabel)
                            {
                                Grid.SetRow(itemLabel, row);
                                Grid.SetColumn(itemLabel, column);
                                column += 1;
                            }

                            if (_gridLayout.Children[index + 1] is DescriptionBorderedItemContent itemContent)
                            {
                                var itemSpan = Math.Max(1,
                                    Math.Min(_effectiveColumns - column, GetItemSpan(item.Span) * 2 - 1));
                                if (i == Items.Count - 1 || item.IsFilled)
                                {
                                    itemSpan = _effectiveColumns - column;
                                }
                                
                                Grid.SetRow(itemContent, row);
                                Grid.SetColumn(itemContent, column);
                                Grid.SetColumnSpan(itemContent, itemSpan);
                                column += itemSpan;
                                if (column >= _effectiveColumns)
                                {
                                    column                   = 0;
                                    itemContent.IsLastColumn = true;
                                    ++row;
                                }
                                else
                                {
                                    itemContent.IsLastColumn = false;
                                }
                            }
                        }
                        else
                        {
                            if (_gridLayout.Children[index] is DescriptionDefaultItem defaultItem)
                            {
                                var itemSpan = Math.Max(1,
                                    Math.Min(_effectiveColumns - column, GetItemSpan(item.Span)));
                                if (i == Items.Count - 1 || item.IsFilled)
                                {
                                    itemSpan = _effectiveColumns - column;
                                }

                                Grid.SetRow(defaultItem, row);
                                Grid.SetColumn(defaultItem, column);
                                Grid.SetColumnSpan(defaultItem, itemSpan);
                                column += itemSpan;
                                if (column >= _effectiveColumns)
                                {
                                    column = 0;
                                    ++row;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (_gridLayout.Children[index] is DescriptionDefaultItem defaultItem)
                        {
                            var itemSpan = Math.Max(1,
                                Math.Min(_effectiveColumns - column, GetItemSpan(item.Span)));
                            if (i == Items.Count - 1 || item.IsFilled)
                            {
                                itemSpan = _effectiveColumns - column;
                            }

                            Grid.SetRow(defaultItem, row);
                            Grid.SetColumn(defaultItem, column);
                            Grid.SetColumnSpan(defaultItem, itemSpan);
                            column += itemSpan;
                            if (column >= _effectiveColumns)
                            {
                                column                   = 0;
                                defaultItem.IsLastColumn = true;
                                ++row;
                            }
                            else
                            {
                                defaultItem.IsLastColumn = false;
                            }
                        }
                    }
                }
            }
            
            // 寻找最后一行
            for (var i = 0; i < Items.Count; i++)
            {
                var item  = Items[i];
                var index = Items.IndexOf(item);
                if (index != -1)
                {
                    if (Layout == Orientation.Horizontal)
                    {
                        if (IsBordered)
                        {
                            index *= 2;
                            if (_gridLayout.Children[index] is DescriptionBorderedItemLabel itemLabel)
                            {
                                itemLabel.IsLastRow = Grid.GetRow(itemLabel) == row - 1;
                            }

                            if (_gridLayout.Children[index + 1] is DescriptionBorderedItemContent itemContent)
                            {
                                itemContent.IsLastRow = Grid.GetRow(itemContent) == row - 1;
                            }
                        }
                    }
                    else
                    {
                        if (_gridLayout.Children[index] is DescriptionDefaultItem defaultItem)
                        {
                            defaultItem.IsLastRow = Grid.GetRow(defaultItem) == row - 1;
                        }
                    }
                }
            }

            _gridLayout.ColumnDefinitions.Clear();
            var columnDefinitions = new ColumnDefinitions();
            for (var i = 0; i < _effectiveColumns; i++)
            {
                columnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            }

            _gridLayout.ColumnDefinitions = columnDefinitions;
            _gridLayout.RowDefinitions.Clear();
            var rowDefinitions = new RowDefinitions();
            for (var i = 0; i < row; i++)
            {
                rowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }

            _gridLayout.RowDefinitions = rowDefinitions;
        }
    }

    private void EnsureEffectiveColumns()
    {
        if (_effectiveColumns > 0)
        {
            return;
        }

        var breakPoint = _breakPoint;
        if (breakPoint == null && _mediaOwner != null)
        {
            breakPoint = _mediaOwner.MediaBreakPoint;
        }
        if (breakPoint == null && MediaQueryHost.FindOwner(this) is { } mediaOwner)
        {
            breakPoint = mediaOwner.MediaBreakPoint;
        }

        breakPoint   ??= MediaBreakPoint.ExtraExtraLarge;
        _breakPoint   = breakPoint;
        var columnCount = GetColumnsForMediaBreak(breakPoint.Value);
        if (Layout == Orientation.Horizontal)
        {
            _effectiveColumns = IsBordered ? columnCount * 2 : columnCount;
        }
        else
        {
            _effectiveColumns = columnCount;
        }
    }

    private int GetItemSpan(DescriptionsMediaBreakInfo breakInfo)
    {
        Debug.Assert(_breakPoint != null);
        return _breakPoint switch
        {
            MediaBreakPoint.ExtraSmall => breakInfo.ExtraSmall,
            MediaBreakPoint.Small => breakInfo.Small,
            MediaBreakPoint.Medium => breakInfo.Medium,
            MediaBreakPoint.Large => breakInfo.Large,
            MediaBreakPoint.ExtraLarge => breakInfo.ExtraLarge,
            _ => breakInfo.ExtraExtraLarge
        };
    }

    private void HandleLayoutChanged()
    {
        _gridLayout?.Children.Clear();
        AddDescriptionItems((IEnumerable<DescriptionItem>)Items);
        DoLayoutChildren();
    }
}
