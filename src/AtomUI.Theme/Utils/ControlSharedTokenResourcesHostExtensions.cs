using System.Diagnostics;
using AtomUI.Theme.Data;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Theme.Utils;

internal static class ControlSharedTokenResourcesHostExtensions
{
    public static void RegisterResources(this IControlSharedTokenResourcesHost host)
    {
        host.HostControl.AttachedToLogicalTree += (object? sender, LogicalTreeAttachmentEventArgs args) =>
        {
            if (sender is Control control)
            {
                var resourceDictionary = new ResourceDictionary();
                var controlToken = TokenFinderUtils.FindControlToken(host.HostControl, host.TokenId, host.ResourceCatalog);
                Debug.Assert(controlToken != null);
                foreach (var entry in controlToken.GetSharedResourceDeltaDictionary())
                {
                    resourceDictionary.Add(entry.Key, entry.Value);
                }
        
                var themeVariant = TokenFinderUtils.FindThemeVariant(control);
                control.Resources.ThemeDictionaries.Add(themeVariant, resourceDictionary);
            }
        };
    }
}