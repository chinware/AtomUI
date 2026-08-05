namespace AtomUI.Desktop.Controls;

internal static class NavMenuEntryValidation
{
    public static INavMenuEntry Validate(object? item, object owner, int index)
    {
        switch (item)
        {
            case INavMenuNode node:
                return node;

            case NavMenuGroup group:
                return group;

            case NavMenuDivider divider:
                return divider;

            case INavMenuEntry entry:
                throw new InvalidOperationException(
                    $"{owner.GetType().Name} contains an unsupported NavMenu entry at index {index}. " +
                    $"Expected {nameof(INavMenuNode)}, {nameof(NavMenuGroup)}, or {nameof(NavMenuDivider)}, " +
                    $"actual {entry.GetType().Name}.");
        }

        var actualType = item?.GetType().Name ?? "<null>";
        throw new InvalidOperationException(
            $"{owner.GetType().Name} contains an invalid NavMenu entry at index {index}. " +
            $"Expected {nameof(INavMenuEntry)}, actual {actualType}.");
    }
}
