using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;

namespace AtomUI.Controls;

public class DataGridHeaderViewItem : HeaderedItemsControl
{
    #region 公共属性定义

    public static readonly DirectProperty<DataGridHeaderViewItem, int> LevelProperty =
        AvaloniaProperty.RegisterDirect<DataGridHeaderViewItem, int>(
            nameof(Level), o => o.Level);
    
    /// <summary>
    /// Gets the level/indentation of the item.
    /// </summary>
    public int Level
    {
        get => _level;
        private set => SetAndRaise(LevelProperty, ref _level, value);
    }
    private int _level;

    #endregion

    #region 内部属性定义
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel()
        {
            Orientation = Orientation.Horizontal
        });
    
    #endregion
    
    private DataGridHeaderView? _gridHeaderView;
    private Control? _header;
    private Control? _headerPresenter;
    
    
    static DataGridHeaderViewItem()
    {
        ItemsPanelProperty.OverrideDefaultValue<DataGridHeaderViewItem>(DefaultPanel);
    }
}