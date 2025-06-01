using System.Collections.ObjectModel;
using Avalonia.Controls.Templates;

namespace AtomUI.Controls;

public interface IDataGridColumnGroupItem
{
    object? Header { get; set; }
    IDataTemplate? HeaderTemplate { get; set; }
    IDataGridColumnGroupItem? Parent { get; set; }
    ObservableCollection<IDataGridColumnGroupItem> Children { get; }
    event EventHandler<DataGridColumnGroupChangedArgs>? GroupChanged;
}