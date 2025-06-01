using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class DataGridColumnGroupItem : AvaloniaObject, IDataGridColumnGroupItem, IDataGridColumnGroupChanged
{
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
    
    public IDataGridColumnGroupItem? Parent { get; set; }
    
    [Content]
    public ObservableCollection<IDataGridColumnGroupItem> Children { get; }
    
    public event EventHandler<DataGridColumnGroupChangedArgs>? GroupChanged;

    public DataGridColumnGroupItem()
    {
        Children                   =  new ObservableCollection<IDataGridColumnGroupItem>();
        Children.CollectionChanged += HandleCollectionChanged;
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
                    groupItem.Parent = null;
                }
            }
        }

        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is IDataGridColumnGroupItem groupItem)
                {
                    groupItem.Parent = this;
                    HandleColumnGroupChanged(groupItem, NotifyColumnGroupChangedType.Add);
                }
            }
        }
    }

    private void HandleColumnGroupChanged(IDataGridColumnGroupItem groupItem, NotifyColumnGroupChangedType changedType)
    {
        if (groupItem is DataGridColumn)
        {
            Debug.Assert(groupItem.Parent != null);
            var current = groupItem;
            while (current.Parent != null)
            {
                current = current.Parent;
            }
            GroupChanged?.Invoke(this, new DataGridColumnGroupChangedArgs(groupItem, changedType));
        }
    }
}