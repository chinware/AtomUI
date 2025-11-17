using Avalonia.Controls;
using Avalonia.Metadata;

namespace AtomUI.Theme;

public class ControlThemesProvider : IControlThemesProvider
{
    private ResourceDictionary _resourceDictionary;
    
    public string Id { get; protected set; } = string.Empty;

    public ControlThemesProvider()
    {
        _resourceDictionary = new ResourceDictionary();
    }
    
    [Content]
    public IList<IResourceProvider> ControlThemes => _resourceDictionary.MergedDictionaries;
}