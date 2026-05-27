using Avalonia.Metadata;

namespace AtomUI.Desktop.Controls;

public class DataGridFilterItem
{
    private List<DataGridFilterItem>? _children;

    public string Text { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    
    [Content]
    public List<DataGridFilterItem> Children
    {
        get => _children ??= new List<DataGridFilterItem>();
        set => _children = value;
    }

    internal bool HasChildren => _children is { Count: > 0 };
}
