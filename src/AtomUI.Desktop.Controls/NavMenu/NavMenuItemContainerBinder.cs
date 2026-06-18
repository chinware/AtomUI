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
