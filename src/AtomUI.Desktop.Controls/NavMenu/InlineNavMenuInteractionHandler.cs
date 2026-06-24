namespace AtomUI.Desktop.Controls;

internal class InlineNavMenuInteractionHandler : NavMenuInteractionHandlerBase
{
    public override void Select(NavMenuItem menuItem)
    {
        if (menuItem.HasSubMenu)
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
        else
        {
            (Menu as NavMenu)?.SelectNavMenuItem(menuItem);
        }
    }

    internal void Open(INavMenuItem menuItem) => menuItem.Open();
}
