namespace AtomUI.Controls.Data;

public class DataGridFilterDescription
{
    public bool IsEnabled { get; set; }
    public int ColumnIndex { get; set; }
    public List<object>? FilterConditions { get; set; }
    public Func<object, object, bool>? OnFilter { get; set; }
}