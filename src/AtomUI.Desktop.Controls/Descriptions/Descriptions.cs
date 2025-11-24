using System.Collections;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls.Themes;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

public record DescriptionsColumnInfo
{
    public int SmallColumns { get; init; } = 3;
    public int MediumColumns { get; init; } = 3;
    public int LargeColumns { get; init; } = 3;
    public int ExtraLargeColumns { get; init; } = 3;
    public int ExtraExtraLargeColumns { get; init; } = 3;

    public DescriptionsColumnInfo(int column)
    {
        SmallColumns           = column;
        MediumColumns          = column;
        LargeColumns           = column;
        ExtraLargeColumns      = column;
        ExtraExtraLargeColumns = column;
    }
}

public class Descriptions : TemplatedControl, 
                            IControlSharedTokenResourcesHost,
                            ISizeTypeAware
{
    #region 公共属性定义

    public static readonly StyledProperty<bool> IsBorderedProperty =
        AvaloniaProperty.Register<Descriptions, bool>(nameof(IsBordered));

    public static readonly StyledProperty<bool> IsShowColonProperty =
        AvaloniaProperty.Register<Descriptions, bool>(nameof(IsShowColon), true);

    public static readonly StyledProperty<DescriptionsColumnInfo> ColumnInfoProperty =
        AvaloniaProperty.Register<Descriptions, DescriptionsColumnInfo>(nameof(ColumnInfo),
            new DescriptionsColumnInfo(3));

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

    public DescriptionsColumnInfo ColumnInfo
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

    [Content] public DescriptionItems Items { get; set; } = new();
    
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
    
    string IControlSharedTokenResourcesHost.TokenId => DescriptionsToken.ID;
    Control IControlSharedTokenResourcesHost.HostControl => this;

    #endregion

    private Grid? _gridLayout;
    private MediaBreakPoint? _breakPoint;
    private int _effectiveColumns;
    private readonly Dictionary<object, CompositeDisposable> _itemsBindingDisposables = new();

    public Descriptions()
    {
        Items.CollectionChanged += HandleCollectionChanged;
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
                        AddDescriptionItems(e.NewItems.OfType<DescriptionItem>().ToList());
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems != null)
                    {
                        RemoveDescriptionItems(e.OldItems.OfType<DescriptionItem>().ToList());
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    throw new NotSupportedException();
            }
            DoLayoutChildren();
            InvalidateMeasure();
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (TopLevel.GetTopLevel(this) is Window window)
        {
            window.MediaBreakPointChanged += HandleMediaBreakChanged;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        if (TopLevel.GetTopLevel(this) is Window window)
        {
            window.MediaBreakPointChanged -= HandleMediaBreakChanged;
        }
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
        if (breakPoint == MediaBreakPoint.Small)
        {
            columns = ColumnInfo.SmallColumns;
        }
        else if (breakPoint == MediaBreakPoint.Medium)
        {
            columns = ColumnInfo.MediumColumns;
        }
        else if (breakPoint == MediaBreakPoint.Large)
        {
            columns = ColumnInfo.LargeColumns;
        }
        else if (breakPoint == MediaBreakPoint.ExtraLarge)
        {
            columns = ColumnInfo.ExtraLargeColumns;
        }
        else if (breakPoint == MediaBreakPoint.ExtraExtraLarge)
        {
            columns = ColumnInfo.ExtraExtraLargeColumns;
        }

        return columns;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _gridLayout = e.NameScope.Find<Grid>(DescriptionsThemeConstants.GridLayoutPart);
        AddDescriptionItems(Items.ToList());
        if (TopLevel.GetTopLevel(this) is Window window)
        {
            _breakPoint = window.MediaBreakPoint;
            var columns = GetColumnsForMediaBreak(_breakPoint.Value);
            UpdateGridColumns(columns);
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
                Items.AddRange(ItemsSource.OfType<DescriptionItem>().ToList());
            }
        }
        else if (change.Property == IsBorderedProperty)
        {
            DoLayoutChildren();
        }
        else if (change.Property == HeaderProperty ||
                 change.Property == ExtraProperty)
        {
            SetCurrentValue(IsHeaderLayoutVisibleProperty, Header != null || Extra != null);
        }
    }

    private void AddDescriptionItems(List<DescriptionItem> items)
    {
        if (_gridLayout != null)
        {
            foreach (DescriptionItem item in items)
            {
                if (IsBordered)
                {
                    var itemLabel   = new DescriptionBorderedItemLabel();
                    var itemContent = new DescriptionBorderedItemContent();
                    itemLabel.DataContext   = item;
                    itemContent.DataContext = item;
                    _gridLayout.Children.Add(itemLabel);
                    _gridLayout.Children.Add(itemContent);
                }
                else
                {
                    var disposables           = new CompositeDisposable(2);
                    var descriptionSimpleItem = new DescriptionSimpleItem();
                    disposables.Add(BindUtils.RelayBind(this, IsShowColonProperty, descriptionSimpleItem, DescriptionSimpleItem.IsColonVisibleProperty));
                    descriptionSimpleItem.Header = item.Label;
                    descriptionSimpleItem.Content = item.Content;
                    _gridLayout.Children.Add(descriptionSimpleItem);
                    if (_itemsBindingDisposables.TryGetValue(descriptionSimpleItem, out var oldDisposables))
                    {
                        oldDisposables.Dispose();
                        _itemsBindingDisposables.Remove(descriptionSimpleItem);
                    }
                    _itemsBindingDisposables.Add(descriptionSimpleItem, disposables);
                }
            }
        }
    }

    private void RemoveDescriptionItems(List<DescriptionItem> items)
    {
        if (_gridLayout != null)
        {
            foreach (DescriptionItem item in items)
            {
                var index = Items.IndexOf(item);
                if (index != -1)
                {
                    if (IsBordered)
                    {
                        if (_gridLayout.Children[index] is DescriptionBorderedItemLabel itemLabel)
                        {
                            _gridLayout.Children.Remove(itemLabel);
                            if (_itemsBindingDisposables.TryGetValue(itemLabel, out var disposable))
                            {
                                disposable.Dispose();
                                _itemsBindingDisposables.Remove(itemLabel);
                            }
                        }

                        if (_gridLayout.Children[index + 1] is DescriptionBorderedItemContent itemContent)
                        {
                            _gridLayout.Children.Remove(itemContent);
                            if (_itemsBindingDisposables.TryGetValue(itemContent, out var disposable))
                            {
                                disposable.Dispose();
                                _itemsBindingDisposables.Remove(itemContent);
                            }
                        }
                    }
                    else
                    {
                        if (_gridLayout.Children[index] is DescriptionSimpleItem simpleItem)
                        {
                            _gridLayout.Children.Remove(simpleItem);
                            if (_itemsBindingDisposables.TryGetValue(simpleItem, out var disposable))
                            {
                                disposable.Dispose();
                                _itemsBindingDisposables.Remove(simpleItem);
                            }
                        }
                    }
                }
            }
        }
    }

    private void UpdateGridColumns(int columnCount)
    {
        if (columnCount != _effectiveColumns)
        {
            _effectiveColumns = columnCount;
            DoLayoutChildren();
        }
    }

    private void DoLayoutChildren()
    {
        if (_gridLayout != null)
        {
            var row = 0;
            var column = 0;
            for (var i = 0; i < Items.Count; i++)
            {
                var item  = Items[i];
                var index = Items.IndexOf(item);
                if (index != -1)
                {
                    if (IsBordered)
                    {
                    }
                    else
                    {
                        if (_gridLayout.Children[index] is DescriptionSimpleItem simpleItem)
                        {
                            var itemSpan = Math.Max(1, Math.Min(_effectiveColumns - column, item.Span));
                            if (i == Items.Count - 1)
                            {
                                itemSpan = _effectiveColumns - column;
                            }
                            
                            Grid.SetRow(simpleItem, row);
                            Grid.SetColumn(simpleItem, column);
                            Grid.SetColumnSpan(simpleItem, itemSpan);
                            column += itemSpan;
                            if (column >= _effectiveColumns)
                            {
                                column = 0;
                                ++row;
                            }
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
            for (var i = 0; i <= row; i++)
            {
                rowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }
            _gridLayout.RowDefinitions = rowDefinitions;
        }
    }
}