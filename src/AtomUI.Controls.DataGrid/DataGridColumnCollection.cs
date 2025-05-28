using System.Collections.ObjectModel;

namespace AtomUI.Controls;

public class DataGridColumnCollection : ObservableCollection<DataGridColumn>
{
    public DataGridColumnCollection(DataGrid owningGrid)
    {
    }
}