using AtomUI.IconPkg;

namespace AtomUI.Controls;

interface IOptionButtonData : IHeadered
{
    Icon? Icon { get; }
    bool IsEnabled { get; }
}

public class OptionButtonData : IOptionButtonData
{
    public object? Header { get; init; }
    public Icon? Icon { get; init; }
    public bool IsEnabled { get; init; }
}