namespace AtomUI.Desktop.Controls;

internal class InlineNavMenuInteractionHandler : NavMenuInteractionHandlerBase
{
    protected override void ActivateSubMenuItem(NavMenuItem menuItem)
    {
        if (menuItem.IsSubMenuOpen)
        {
            menuItem.Close();
        }
        else
        {
            Open(menuItem);
        }
    }

    internal void Open(INavMenuItem menuItem) => menuItem.Open();
}
