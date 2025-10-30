namespace AtomUI.Controls;

public interface IListBoxItemData
{
    bool IsEnabled { get; }
    bool IsSelected { get; }
    object? Content { get; }
    string? Group { get; }
}

public record ListItemData : IListBoxItemData
{
    public bool IsEnabled { get; init; } = true;
    public bool IsSelected { get; init; }
    public object? Content { get; init; }
    public string? Group { get; init; }
}

public record ListGroupData
{
    public string Header { get; init; } = string.Empty;
}