using System.Diagnostics;
using AtomUI.Theme.Data;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Theme.Utils;

public static class ControlSharedTokenResourcesHostExtensions
{
    public static void RegisterResources(this IControlSharedTokenResourcesHost host)
    {
        host.HostControl.AttachedToLogicalTree += HandleAttachedToLogicalTree;
    }

    private static void HandleAttachedToLogicalTree(object? sender, LogicalTreeAttachmentEventArgs args)
    {
        // 对于控件就添加一次
        if (sender is Control control)
        {
            var themeVariant = TokenFinderUtils.FindThemeVariant(control);
            if (control.Resources.ThemeDictionaries.ContainsKey(themeVariant))
            {
                return;
            }

            var resourceDictionary = new ResourceDictionary();
            // 虽然能指定，一般就是指定自己
            var tokenResourceHost = control as IControlSharedTokenResourcesHost;
            Debug.Assert(tokenResourceHost != null);
            var controlToken = TokenFinderUtils.FindControlToken(tokenResourceHost.HostControl,
                tokenResourceHost.TokenId, tokenResourceHost.ResourceCatalog);
            Debug.Assert(controlToken != null);
            foreach (var entry in controlToken.GetSharedResourceDeltaDictionary())
            {
                resourceDictionary.Add(entry.Key, entry.Value);
            }

            control.Resources.ThemeDictionaries.Add(themeVariant, resourceDictionary);
        }
    }
}