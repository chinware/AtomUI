using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class DataGridHeaderView : ItemsControl
{
    internal DataGrid OwningGrid { get; set; }

    public DataGridHeaderView(DataGrid owningGrid)
    {
        OwningGrid = owningGrid;
    }
    
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
}