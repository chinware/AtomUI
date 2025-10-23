namespace AtomUI.Controls;

public interface ISelectOption
{
    string Header { get; }
}

public record SelectOption : ISelectOption
{
    public string Header { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public object? Value { get; set; }
}

public record SelectOptionGroup : ISelectOption
{
    public string Header { get; set; } = string.Empty;
}