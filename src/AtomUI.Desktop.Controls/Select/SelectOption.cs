namespace AtomUI.Controls;

public interface ISelectOption
{
    string Header { get; }
}

public record SelectOption : ISelectOption
{
    public string Header { get; init; } = string.Empty;
    public bool IsEnabled { get; init; } = true;
    public object? Value { get; init; }
    public string? Group { get; init; }
    public bool IsDynamicAdded { get; init; } = false;
}
