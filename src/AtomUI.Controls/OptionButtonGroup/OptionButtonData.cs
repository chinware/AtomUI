using Avalonia.Controls;

namespace AtomUI.Controls;

interface IOptionButtonData : IHeadered
{
    PathIcon? Icon { get; }
    bool IsEnabled { get; }
}

public class OptionButtonData : IOptionButtonData
{
    public object? Header { get; init; }
    public PathIcon? Icon { get; init; }
    public bool IsEnabled { get; init; } = true;
}