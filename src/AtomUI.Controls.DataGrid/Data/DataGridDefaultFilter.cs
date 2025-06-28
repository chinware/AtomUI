namespace AtomUI.Controls.Data;

public class DataGridDefaultFilter
{
    public bool Filter(object value)
    {
        return true;
    }
    
    public static implicit operator Func<object, bool>(DataGridDefaultFilter filter)
    {
        return filter.Filter;
    }
}