using AtomUI.IconPkg;

namespace AtomUI.Desktop.Controls;

public interface ITabItemData : IHeadered
{
    Icon? Icon { get; }
    Icon? CloseIcon { get; }
    bool IsEnabled { get; }
    bool IsClosable { get; }
    bool IsAutoHideCloseButton { get; }
}

public class TabItemData : ITabItemData
{
    public object? Header { get; init; }
    public Icon? Icon { get; init; }
    public Icon? CloseIcon { get; init; }
    public bool IsEnabled { get; init; } = true;
    public bool IsClosable { get; init; }
    public bool IsAutoHideCloseButton { get; init; }
}