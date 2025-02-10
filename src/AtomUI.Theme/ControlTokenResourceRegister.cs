using System.Diagnostics;
using AtomUI.Theme.Data;
using AtomUI.Theme.TokenSystem;
using Avalonia.Controls;
using Avalonia.Styling;

namespace AtomUI.Theme;

public class ControlTokenResourceRegister
{
    private Control _target;
    private string _tokenId;
    private string? _resourceCatalog;
    private ResourceDictionary? _resourceDictionary;
    
    public ControlTokenResourceRegister(Control target, string tokenId, string? resourceCatalog = null)
    {
        _target          = target;
        _tokenId         = tokenId;
        _resourceCatalog = resourceCatalog;
    }

    public void RegisterResources()
    {
        if (_resourceDictionary != null)
        {
            _target.Resources.MergedDictionaries.Remove(_resourceDictionary);
        }

        var controlToken = TokenFinderUtils.FindControlToken(_target, _tokenId, _resourceCatalog);
        Debug.Assert(controlToken != null);
        _resourceDictionary = new ResourceDictionary();
        foreach (var entry in controlToken.GetSharedResourceDeltaDictionary())
        {
            _resourceDictionary.Add(entry.Key, entry.Value);
        }

        var themeVariant = TokenFinderUtils.FindThemeVariant(_target);
        _target.Resources.ThemeDictionaries.Add(themeVariant, _resourceDictionary);
    }

    private IControlDesignToken? FindControlToken()
    {
        IControlDesignToken? token = null;
        var current = _target;
        while (current != null)
        {
            if (current is IThemeConfigProvider configProvider)
            {
                token = configProvider.GetControlToken(_tokenId, _resourceCatalog);
                if (token != null)
                {
                    break;
                }
            }
            current = (current as IStyleHost).StylingParent as Control;
        }
        
        return token;
    }
}