namespace AtomUI.Controls;

public interface IDataGridColumnGroupChanged
{
    event EventHandler<DataGridColumnGroupChangedArgs> GroupChanged;
}