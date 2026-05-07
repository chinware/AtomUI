namespace AtomUI.Desktop.Controls;

public interface IDataGridColumnGroupChanged
{
    event EventHandler<DataGridColumnGroupChangedArgs> GroupChanged;
}