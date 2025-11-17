using Avalonia.Controls;

namespace AtomUI.Theme;

public interface IControlThemesProvider
{
    string Id { get; }
    IList<IResourceProvider> ControlThemes { get; }
}