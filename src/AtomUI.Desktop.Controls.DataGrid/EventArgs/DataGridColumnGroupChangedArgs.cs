namespace AtomUI.Desktop.Controls;

public class DataGridColumnGroupChangedArgs : EventArgs
{
    public IDataGridColumnGroupItem GroupItem { get; }
    public NotifyColumnGroupChangedType ChangedType { get; }

    public DataGridColumnGroupChangedArgs(IDataGridColumnGroupItem item, NotifyColumnGroupChangedType changedType)
    {
        GroupItem   = item;   
        ChangedType = changedType;
    }
}

public enum NotifyColumnGroupChangedType
{
    Add,
    Remove,
}