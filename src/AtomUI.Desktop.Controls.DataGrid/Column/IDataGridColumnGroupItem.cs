using System.Collections.ObjectModel;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

public interface IDataGridColumnGroupItem
{
    object? Header { get; set; }
    IDataTemplate? HeaderTemplate { get; set; }
    IDataGridColumnGroupItem? GroupParent { get; set; }
    ObservableCollection<IDataGridColumnGroupItem> GroupChildren { get; }
    event EventHandler<DataGridColumnGroupChangedArgs>? GroupChanged;
}

internal interface IDataGridColumnGroupItemInternal : IDataGridColumnGroupItem
{
    DataGridHeaderViewItem? GroupHeaderViewItem { get; internal set; }
}