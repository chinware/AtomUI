using System.Diagnostics;
using Avalonia.Controls;

namespace AtomUI.Theme;

public class ControlTokenResourceRegister
{
    private Control _target;
    private string _tokenId;
    private string? _resourceCatalog;
    private IResourceDictionary? _resourceDictionary;
    
    public ControlTokenResourceRegister(Control target, string tokenId, string? resourceCatalog = null)
    {
        _target          = target;
        _tokenId         = tokenId;
        _resourceCatalog = resourceCatalog;
    }

    public void RegisterResources()
    {
        var theme = ThemeManager.Current.ActivatedTheme;
        if (theme == null)
        {
            return;
        }

        if (_resourceDictionary != null)
        {
            _target.Resources.MergedDictionaries.Remove(_resourceDictionary);
        }

        var controlToken = theme.GetControlToken(_tokenId, _resourceCatalog);
        Debug.Assert(controlToken != null);
        _resourceDictionary = new ResourceDictionary();
        foreach (var entry in controlToken.GetSharedResourceDeltaDictionary())
        {
            _resourceDictionary.Add(entry.Key, entry.Value);
        }
     
        _target.Resources.MergedDictionaries.Add(_resourceDictionary);
    }
}