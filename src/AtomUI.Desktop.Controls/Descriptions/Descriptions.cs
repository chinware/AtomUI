using System.Collections;
using System.Collections.Specialized;
using AtomUI.Controls;
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
            RebuildDescriptionItems();
            InvalidateMeasure();
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
    private Window? _attachedWindow;
    private bool _isSyncingItemsSource;

    static Descriptions()
    {
        SizeTypeProperty.OverrideDefaultValue<Descriptions>(SizeType.Large);
    }

    public Descriptions()
    {
        this.RegisterTokenResourceScope(DescriptionsToken.ScopeProvider);
        Items.CollectionChanged += HandleCollectionChanged;
    }
    
    protected virtual void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_isSyncingItemsSource)
        {
            return;
        }

        if (_gridLayout != null && this.IsAttachedToVisualTree())
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewItems != null)
                    {
                        InsertDescriptionItems(
                            e.NewItems.OfType<DescriptionItem>().ToList(),
                            e.NewStartingIndex);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        RemoveDescriptionItems(e.OldItems.Count, e.OldStartingIndex);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    RebuildDescriptionItems();
                    break;
            }

            DoLayoutChildren();
            InvalidateMeasure();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (e.RootVisual is Window rootWindow)
        {
            UpdateWindowSubscription(rootWindow);
            _breakPoint = rootWindow.MediaBreakPoint;
            UpdateGridColumns(GetColumnsForMediaBreak(_breakPoint.Value));
        }
        else if (TopLevel.GetTopLevel(this) is Window window)
        {
            UpdateWindowSubscription(window);
            _breakPoint = window.MediaBreakPoint;
            UpdateGridColumns(GetColumnsForMediaBreak(_breakPoint.Value));
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearWindowSubscription();
    }

    private void UpdateWindowSubscription(Window window)
    {
        if (ReferenceEquals(_attachedWindow, window))
        {
            return;
        }

        ClearWindowSubscription();
        _attachedWindow = window;
        window.MediaBreakPointChanged += HandleMediaBreakChanged;
    }

    private void ClearWindowSubscription()
    {
        if (_attachedWindow != null)
        {
            _attachedWindow.MediaBreakPointChanged -= HandleMediaBreakChanged;
        }

        _attachedWindow = null;
    }

    private void HandleMediaBreakChanged(object? sender, MediaBreakPointChangedEventArgs args)
    {
        _breakPoint = args.MediaBreakPoint;
        if (_breakPoint != null)
        {
            var columns = GetColumnsForMediaBreak(_breakPoint.Value);
            UpdateGridColumns(columns);
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
        ConfigureEffectiveColumns(GetCurrentColumnCount());
        RebuildDescriptionItems();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ItemsSourceProperty)
        {
            if (ItemsSource != null)
            {
                try
                {
                    _isSyncingItemsSource = true;
                    Items.Clear();
                    Items.AddRange(ItemsSource.OfType<DescriptionItem>().ToList());
                }
                finally
                {
                    _isSyncingItemsSource = false;
                }

                RebuildDescriptionItems();
                InvalidateMeasure();
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

    private void InsertDescriptionItems(IReadOnlyList<DescriptionItem> items, int itemIndex)
    {
        if (_gridLayout == null || items.Count == 0)
        {
            return;
        }

        var visualIndex = itemIndex < 0
            ? _gridLayout.Children.Count
            : Math.Min(_gridLayout.Children.Count, itemIndex * GetVisualsPerItem());

        foreach (var item in items)
        {
            foreach (var control in CreateDescriptionItemControls(item))
            {
                _gridLayout.Children.Insert(visualIndex++, control);
            }
        }
    }

    private IEnumerable<Control> CreateDescriptionItemControls(DescriptionItem item)
    {
        if (Layout == Orientation.Horizontal && IsBordered)
        {
            var itemLabel = new DescriptionBorderedItemLabel
            {
                Content = item.Label
            };
            var itemContent = new DescriptionBorderedItemContent
            {
                Content = item.Content
            };
            itemLabel[!SizeTypeProperty]   = this[!SizeTypeProperty];
            itemContent[!SizeTypeProperty] = this[!SizeTypeProperty];
            yield return itemLabel;
            yield return itemContent;
            yield break;
        }

        var descriptionDefaultItem = new DescriptionDefaultItem
        {
            Layout  = Layout,
            Header  = item.Label,
            Content = item.Content
        };
        descriptionDefaultItem[!DescriptionDefaultItem.IsColonVisibleProperty] = this[!IsShowColonProperty];
        descriptionDefaultItem[!DescriptionDefaultItem.IsBorderedProperty]     = this[!IsBorderedProperty];
        yield return descriptionDefaultItem;
    }

    private void RemoveDescriptionItems(int itemCount, int itemIndex)
    {
        if (_gridLayout == null || itemCount <= 0 || itemIndex < 0)
        {
            return;
        }

        var visualIndex  = itemIndex * GetVisualsPerItem();
        var visualsCount = itemCount * GetVisualsPerItem();
        for (var i = 0; i < visualsCount && visualIndex < _gridLayout.Children.Count; i++)
        {
            _gridLayout.Children.RemoveAt(visualIndex);
        }
    }

    private void RebuildDescriptionItems()
    {
        if (_gridLayout == null)
        {
            return;
        }

        ConfigureEffectiveColumns(GetCurrentColumnCount());
        _gridLayout.Children.Clear();
        InsertDescriptionItems(Items.ToList(), 0);
        DoLayoutChildren();
    }

    private void UpdateGridColumns(int columnCount)
    {
        var oldColumns = _effectiveColumns;
        ConfigureEffectiveColumns(columnCount);
        if (_effectiveColumns != oldColumns)
        {
            DoLayoutChildren();
        }
    }

    private void ConfigureEffectiveColumns(int columnCount)
    {
        var safeColumnCount = Math.Max(1, columnCount);
        _effectiveColumns = Layout == Orientation.Horizontal && IsBordered
            ? safeColumnCount * 2
            : safeColumnCount;
    }

    private void HandleBorderedChanged()
    {
        RebuildDescriptionItems();
        InvalidateMeasure();
    }

    private void DoLayoutChildren()
    {
        if (_gridLayout != null && _effectiveColumns > 0)
        {
            var row    = 0;
            var column = 0;
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (Layout == Orientation.Horizontal)
                {
                    if (IsBordered)
                    {
                        var index = i * 2;
                        if (_gridLayout.Children[index] is DescriptionBorderedItemLabel itemLabel)
                        {
                            Grid.SetRow(itemLabel, row);
                            Grid.SetColumn(itemLabel, column);
                            itemLabel.IsLastColumn = false;
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
                    else if (_gridLayout.Children[i] is DescriptionDefaultItem defaultItem)
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
                else if (_gridLayout.Children[i] is DescriptionDefaultItem defaultItem)
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

            var lastRow = row - 1;
            for (var i = 0; i < Items.Count; i++)
            {
                if (Layout == Orientation.Horizontal)
                {
                    if (IsBordered)
                    {
                        var index = i * 2;
                        if (_gridLayout.Children[index] is DescriptionBorderedItemLabel itemLabel)
                        {
                            itemLabel.IsLastRow = Grid.GetRow(itemLabel) == lastRow;
                        }

                        if (_gridLayout.Children[index + 1] is DescriptionBorderedItemContent itemContent)
                        {
                            itemContent.IsLastRow = Grid.GetRow(itemContent) == lastRow;
                        }
                    }
                }
                else if (_gridLayout.Children[i] is DescriptionDefaultItem defaultItem)
                {
                    defaultItem.IsLastRow = Grid.GetRow(defaultItem) == lastRow;
                }
            }

            EnsureColumnDefinitions(_effectiveColumns);
            EnsureRowDefinitions(row);
        }
    }

    private int GetItemSpan(DescriptionsMediaBreakInfo breakInfo)
    {
        var breakPoint = _breakPoint ?? MediaBreakPoint.ExtraExtraLarge;
        return breakPoint switch
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
        RebuildDescriptionItems();
        InvalidateMeasure();
    }

    private int GetVisualsPerItem()
    {
        return Layout == Orientation.Horizontal && IsBordered ? 2 : 1;
    }

    private int GetCurrentColumnCount()
    {
        if (TopLevel.GetTopLevel(this) is Window window)
        {
            UpdateWindowSubscription(window);
            _breakPoint = window.MediaBreakPoint;
        }

        return GetColumnsForMediaBreak(_breakPoint ?? MediaBreakPoint.ExtraExtraLarge);
    }

    private void EnsureColumnDefinitions(int count)
    {
        if (_gridLayout == null)
        {
            return;
        }

        while (_gridLayout.ColumnDefinitions.Count < count)
        {
            _gridLayout.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        }

        while (_gridLayout.ColumnDefinitions.Count > count)
        {
            _gridLayout.ColumnDefinitions.RemoveAt(_gridLayout.ColumnDefinitions.Count - 1);
        }
    }

    private void EnsureRowDefinitions(int count)
    {
        if (_gridLayout == null)
        {
            return;
        }

        while (_gridLayout.RowDefinitions.Count < count)
        {
            _gridLayout.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        while (_gridLayout.RowDefinitions.Count > count)
        {
            _gridLayout.RowDefinitions.RemoveAt(_gridLayout.RowDefinitions.Count - 1);
        }
    }
}
