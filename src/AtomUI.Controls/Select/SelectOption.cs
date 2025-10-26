namespace AtomUI.Controls;

public interface ISelectOption
{
    string Header { get; }
}

public record SelectOption : ISelectOption
{
    public string Header { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public object? Value { get; set; }
    public bool IsSelected { get; set; } = false;
}

public record SelectOptionGroup : ISelectOption
{
    public string Header { get; set; } = string.Empty;
}