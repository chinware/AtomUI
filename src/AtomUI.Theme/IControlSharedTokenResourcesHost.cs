using Avalonia.Controls;

namespace AtomUI.Theme;

internal interface IControlSharedTokenResourcesHost
{
    Control HostControl { get; }
    string TokenId { get; }
    string? ResourceCatalog => null;
}