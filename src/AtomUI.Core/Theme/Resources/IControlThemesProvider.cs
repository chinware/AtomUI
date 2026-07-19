using Avalonia.Controls;

namespace AtomUI.Theme.Resources;

public interface IControlThemesProvider
{
    string Id { get; }
    IList<IResourceProvider> ControlThemes { get; }
}
