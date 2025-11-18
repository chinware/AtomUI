using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public class DataGridColumnGroupItem : AvaloniaObject,
                                       IDataGridColumnGroupItemInternal, 
                                       IDataGridColumnGroupChanged
{
    #region 公共属性定义

    public static readonly DirectProperty<DataGridColumnGroupItem, object?> HeaderProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnGroupItem, object?>(
            nameof(Header),
            o => o.Header,
            (o, v) => o.Header = v);
    
    public static readonly DirectProperty<DataGridColumnGroupItem, IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnGroupItem, IDataTemplate?>(
            nameof(HeaderTemplate),
            o => o.HeaderTemplate,
            (o, v) => o.HeaderTemplate = v);
    
    public static readonly StyledProperty<HorizontalAlignment> HorizontalAlignmentProperty =
        Layoutable.HorizontalAlignmentProperty.AddOwner<DataGridColumnGroupItem>();
    
    public static readonly StyledProperty<VerticalAlignment> VerticalAlignmentProperty =
        Layoutable.VerticalAlignmentProperty.AddOwner<DataGridColumnGroupItem>();

    public object? _header;
    public object? Header
    {
        get => _header;
        set => SetAndRaise(HeaderProperty, ref _header, value);
    }
    
    public IDataTemplate? _headerTemplate;
    public IDataTemplate? HeaderTemplate
    {
        get => _headerTemplate;
        set => SetAndRaise(HeaderTemplateProperty, ref _headerTemplate, value);
    }
    
    public HorizontalAlignment HorizontalAlignment
    {
        get => GetValue(HorizontalAlignmentProperty);
        set => SetValue(HorizontalAlignmentProperty, value);
    }
    
    public VerticalAlignment VerticalAlignment
    {
        get => GetValue(VerticalAlignmentProperty);
        set => SetValue(VerticalAlignmentProperty, value);
    }
    
    public IDataGridColumnGroupItem? GroupParent { get; set; }
    
    [Content]
    public ObservableCollection<IDataGridColumnGroupItem> GroupChildren { get; }
    #endregion

    #region 公共事件定义

    public event EventHandler<DataGridColumnGroupChangedArgs>? GroupChanged;
    public event EventHandler<PointerPressedEventArgs>? HeaderPointerPressed;
    public event EventHandler<PointerReleasedEventArgs>? HeaderPointerReleased;

    #endregion
    
    #region 内部属性定义
    
    DataGridHeaderViewItem? IDataGridColumnGroupItemInternal.GroupHeaderViewItem { get; set; }
    
    internal bool HasHeaderCell => _headerCell != null;

    internal DataGridColumnGroupHeader HeaderCell
    {
        get
        {
            _headerCell ??= CreateHeader();
            return _headerCell;
        }
    }
    
    protected internal DataGrid? OwningGrid
    {
        get;
        internal set;
    }

    internal bool IsFrozen { get; set; } = false;
    #endregion
    
    private DataGridColumnGroupHeader? _headerCell;
    private CompositeDisposable? _headerBindingDisposables;

    static DataGridColumnGroupItem()
    {
        HorizontalAlignmentProperty.OverrideDefaultValue<DataGridColumnGroupItem>(HorizontalAlignment.Left);
        VerticalAlignmentProperty.OverrideDefaultValue<DataGridColumnGroupItem>(VerticalAlignment.Center);
    }

    public DataGridColumnGroupItem()
    {
        GroupChildren                   =  new ObservableCollection<IDataGridColumnGroupItem>();
        GroupChildren.CollectionChanged += HandleCollectionChanged;
    }
    
    private void HandleCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is IDataGridColumnGroupItem groupItem)
                {
                    HandleColumnGroupChanged(groupItem, NotifyColumnGroupChangedType.Remove);
                    groupItem.GroupParent = null;
                }
            }
        }

        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is IDataGridColumnGroupItem groupItem)
                {
                    groupItem.GroupParent = this;
                    HandleColumnGroupChanged(groupItem, NotifyColumnGroupChangedType.Add);
                }
            }
        }
    }

    private void HandleColumnGroupChanged(IDataGridColumnGroupItem groupItem, NotifyColumnGroupChangedType changedType)
    {
        if (groupItem is DataGridColumn)
        {
            Debug.Assert(groupItem.GroupParent != null);
            var current = groupItem;
            while (current.GroupParent != null)
            {
                current = current.GroupParent;
            }
            GroupChanged?.Invoke(this, new DataGridColumnGroupChangedArgs(groupItem, changedType));
        }
    }
    
    internal virtual DataGridColumnGroupHeader CreateHeader()
    {
        var result = new DataGridColumnGroupHeader();
        result.OwningGroupItem = this;
        Debug.Assert(OwningGrid != null);
        _headerBindingDisposables?.Dispose();
        _headerBindingDisposables = new CompositeDisposable(5);
        _headerBindingDisposables.Add(BindUtils.RelayBind(this, HeaderProperty, result, DataGridColumnGroupHeader.HeaderProperty));
        _headerBindingDisposables.Add(BindUtils.RelayBind(this, HeaderTemplateProperty, result, DataGridColumnGroupHeader.HeaderTemplateProperty));
        _headerBindingDisposables.Add(BindUtils.RelayBind(this, HorizontalAlignmentProperty, result, DataGridColumnGroupHeader.HorizontalContentAlignmentProperty));
        _headerBindingDisposables.Add(BindUtils.RelayBind(this, VerticalAlignmentProperty, result, DataGridColumnGroupHeader.VerticalContentAlignmentProperty));
        _headerBindingDisposables.Add(BindUtils.RelayBind(OwningGrid, DataGrid.SizeTypeProperty, result, DataGridColumnGroupHeader.SizeTypeProperty));
        
        result.PointerPressed  += (s, e) => { HeaderPointerPressed?.Invoke(this, e); };
        result.PointerReleased += (s, e) => { HeaderPointerReleased?.Invoke(this, e); };
        return result;
    }
}