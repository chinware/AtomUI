using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public class DataGridFilterItem
{
    public string Text { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    
    [Content]
    public List<DataGridFilterItem> Children { get; set; } = new List<DataGridFilterItem>();
}