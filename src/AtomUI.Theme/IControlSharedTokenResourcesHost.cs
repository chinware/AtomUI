using Avalonia.Controls;

namespace AtomUI.Theme;

public interface IControlSharedTokenResourcesHost
{
    Control HostControl { get; }
    string TokenId { get; }
    string? ResourceCatalog => null;
}