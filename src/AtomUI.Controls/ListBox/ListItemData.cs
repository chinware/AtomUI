namespace AtomUI.Controls;

public interface IListBoxItemData
{
    bool IsEnabled { get; }
    bool IsSelected { get; }
    object? Content { get; }
}

public record ListItemData : IListBoxItemData
{
    public bool IsEnabled { get; init; } = true;
    public bool IsSelected { get; init; }
    public object? Content { get; init; }
}