using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal static class NavMenuSemanticNavigator
{
    public static IEnumerable<NavMenuItem> EnumerateDirectItems(ItemsControl owner)
    {
        for (var i = 0; i < owner.ItemCount; i++)
        {
            switch (owner.ContainerFromIndex(i))
            {
                case NavMenuItem item:
                    yield return item;
                    break;

                case NavMenuGroupItem group:
                    foreach (var groupedItem in EnumerateDirectItems(group))
                    {
                        yield return groupedItem;
                    }
                    break;
            }
        }
    }

    public static NavMenuItem? FindDirectItem(ItemsControl owner, INavMenuNode node)
    {
        if (owner.ContainerFromItem(node) is NavMenuItem directItem)
        {
            return directItem;
        }

        for (var i = 0; i < owner.ItemCount; i++)
        {
            if (owner.ContainerFromIndex(i) is NavMenuGroupItem group &&
                FindDirectItem(group, node) is { } groupedItem)
            {
                return groupedItem;
            }
        }

        return null;
    }

    public static void EnsureGroupContainers(ItemsControl owner, Action executeLayoutPass)
    {
        var preparedGroups = new HashSet<NavMenuGroupItem>();
        while (PrepareNewGroupPresenters(owner, preparedGroups))
        {
            executeLayoutPass();
        }
    }

    private static bool PrepareNewGroupPresenters(
        ItemsControl owner,
        ISet<NavMenuGroupItem> preparedGroups)
    {
        var preparedAny = false;
        for (var i = 0; i < owner.ItemCount; i++)
        {
            if (owner.ContainerFromIndex(i) is not NavMenuGroupItem group)
            {
                continue;
            }

            if (preparedGroups.Add(group))
            {
                group.ApplyTemplate();
                group.Presenter?.ApplyTemplate();
                preparedAny = true;
            }

            preparedAny |= PrepareNewGroupPresenters(group, preparedGroups);
        }

        return preparedAny;
    }
}
