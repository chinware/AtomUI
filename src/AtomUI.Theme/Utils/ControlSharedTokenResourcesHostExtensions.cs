using System.Diagnostics;
using AtomUI.Theme.Data;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;

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
            // 虽然能指定，一般就是指定自己
            var tokenResourceHost = control as IControlSharedTokenResourcesHost;
            Debug.Assert(tokenResourceHost != null);
            var controlToken = TokenFinderUtils.FindControlToken(tokenResourceHost.HostControl,
                tokenResourceHost.TokenId, tokenResourceHost.ResourceCatalog);
            Debug.Assert(controlToken != null);
            var delta = controlToken.GetSharedResourceDeltaDictionary();
            if (delta.Count > 0)
            {
                var resourceDictionary = new ResourceDictionary();
                foreach (var entry in controlToken.GetSharedResourceDeltaDictionary())
                {
                    if (entry.Value is Color color)
                    {
                        resourceDictionary[entry.Key] = new SolidColorBrush(color);
                    }
                    else
                    {
                        resourceDictionary[entry.Key] = entry.Value;
                    }
                    
                }
                control.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
    }
}