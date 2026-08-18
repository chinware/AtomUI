using System.Reactive.Disposables;
using AtomUI.Data;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal static class NavMenuItemContainerBinder
{
    public static void BindNode(
        NavMenuItem menuItem,
        object? item,
        IResourceHost resourceHost,
        CompositeDisposable disposables)
    {
        if (item is not INavMenuNode menuNode)
        {
            return;
        }

        if (menuNode is NavMenuNode navMenuNode)
        {
            disposables.Add(navMenuNode.AttachResourceHost(resourceHost));
            disposables.Add(BindUtils.RelayBind(
                navMenuNode,
                NavMenuNode.CommandProperty,
                menuItem,
                NavMenuItem.CommandProperty));
            disposables.Add(BindUtils.RelayBind(
                navMenuNode,
                NavMenuNode.CommandParameterProperty,
                menuItem,
                NavMenuItem.CommandParameterProperty));
            disposables.Add(BindUtils.RelayBind(
                navMenuNode,
                NavMenuNode.HeaderProperty,
                menuItem,
                NavMenuItem.NodeHeaderProperty));
            disposables.Add(BindUtils.RelayBind(
                navMenuNode,
                NavMenuNode.TooltipProperty,
                menuItem,
                NavMenuItem.TooltipProperty));
            disposables.Add(BindUtils.RelayBind(
                navMenuNode,
                NavMenuNode.IsTooltipEnabledProperty,
                menuItem,
                NavMenuItem.IsTooltipEnabledProperty));
        }
        else
        {
            disposables.Add(BindUtils.RelayBind(
                menuNode,
                nameof(INavMenuNode.Command),
                node => node.Command,
                menuItem,
                NavMenuItem.CommandProperty));
            disposables.Add(BindUtils.RelayBind(
                menuNode,
                nameof(INavMenuNode.CommandParameter),
                node => node.CommandParameter,
                menuItem,
                NavMenuItem.CommandParameterProperty));
            disposables.Add(BindUtils.RelayBind(
                menuNode,
                nameof(INavMenuNode.Header),
                node => node.Header,
                menuItem,
                NavMenuItem.NodeHeaderProperty));
            disposables.Add(BindUtils.RelayBind(
                menuNode,
                nameof(INavMenuNode.Tooltip),
                node => node.Tooltip,
                menuItem,
                NavMenuItem.TooltipProperty));
            disposables.Add(BindUtils.RelayBind(
                menuNode,
                nameof(INavMenuNode.IsTooltipEnabled),
                node => node.IsTooltipEnabled,
                menuItem,
                NavMenuItem.IsTooltipEnabledProperty));
        }

        menuItem.SetCurrentValue(NavMenuItem.HeaderProperty, menuNode);
        disposables.Add(BindUtils.RelayBind(menuNode, nameof(INavMenuNode.Icon),
            node => node.Icon, menuItem, NavMenuItem.IconProperty));
        disposables.Add(BindUtils.RelayBind(menuNode, nameof(INavMenuNode.IsEnabled),
            node => node.IsEnabled, menuItem, NavMenuItem.IsEnabledProperty));
        menuItem.ItemKey = menuNode.ItemKey;
    }

    public static bool TryBindNodeHeaderTemplate(
        NavMenuItem menuItem,
        object? item,
        CompositeDisposable disposables)
    {
        if (item is not INavMenuNode { HeaderTemplate: not null } menuNode)
        {
            return false;
        }

        disposables.Add(BindUtils.RelayBind(menuNode, nameof(INavMenuNode.HeaderTemplate),
            node => node.HeaderTemplate, menuItem, NavMenuItem.HeaderTemplateProperty));
        return true;
    }
}
