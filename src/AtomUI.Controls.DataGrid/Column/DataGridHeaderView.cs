using System.Diagnostics;
using AtomUI.Data;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class DataGridHeaderView : ItemsControl
{
    internal DataGrid? OwningGrid { get; set; }
    
    #region 内部属性定义
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel()
        {
            Orientation = Orientation.Horizontal
        });
    
    #endregion

    static DataGridHeaderView()
    {
        ItemsPanelProperty.OverrideDefaultValue<DataGridHeaderView>(DefaultPanel);
    }
    
    protected override Control CreateContainerForItemOverride(
        object? item,
        int index,
        object? recycleKey)
    {
        Debug.Assert(OwningGrid != null);
        return new DataGridHeaderViewItem(OwningGrid);
    }
    
    protected override bool NeedsContainerOverride(
        object? item,
        int index,
        out object? recycleKey)
    {
        return NeedsContainer<DataGridHeaderViewItem>(item, out recycleKey);
    }
    
    protected override void ContainerForItemPreparedOverride(
        Control container,
        object? item,
        int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        Debug.Assert(OwningGrid is not null);
        if (container is DataGridHeaderViewItem headerViewItem)
        {
            headerViewItem.OwningHeaderView = this;
            BindUtils.RelayBind(OwningGrid, DataGrid.BorderThicknessProperty, headerViewItem, DataGridHeaderViewItem.BorderThicknessProperty);
        }
    }
}