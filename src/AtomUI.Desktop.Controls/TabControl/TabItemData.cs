using AtomUI.Controls;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

public interface ITabItemData : IHeadered
{
    PathIcon? Icon { get; }
    PathIcon? CloseIcon { get; }
    bool IsEnabled { get; }
    bool IsClosable { get; }
    bool IsAutoHideCloseButton { get; }
}

public class TabItemData : ITabItemData
{
    public object? Header { get; init; }
    public PathIcon? Icon { get; init; }
    public PathIcon? CloseIcon { get; init; }
    public bool IsEnabled { get; init; } = true;
    public bool IsClosable { get; init; }
    public bool IsAutoHideCloseButton { get; init; }
}