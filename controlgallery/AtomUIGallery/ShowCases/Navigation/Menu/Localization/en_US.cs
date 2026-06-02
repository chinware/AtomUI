using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Menu;

[LanguageProvider(LanguageCode.en_US, MenuShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Simplest Usage.";
    public const string MenuItemItemsSourceTitle = "Generate MenuItem by ItemsSource";
    public const string MenuItemItemsSourceDescription = "Generate structure based on ItemsSource and template.";
    public const string InlineNavMenuItemsSourceTitle = "Inline NavMenu by ItemsSource";
    public const string InlineNavMenuItemsSourceDescription = "Generate inline NavMenu structure based on ItemsSource and template.";
    public const string IconAndSubmenuTitle = "Icon and submenu";
    public const string IconAndSubmenuDescription = "With icon and submenu.";
    public const string ToggleTypeTitle = "Menu item with ToggleType";
    public const string ToggleTypeDescription = "Renders a checkbox or radio button on a menu.";
    public const string ScrollableTitle = "Scrollable menu";
    public const string ScrollableDescription = "When there are too many menu items, up and down scroll buttons will appear.";
    public const string ContextMenuTitle = "Context menu";
    public const string ContextMenuDescription = "Right click to bring up the context menu.";
    public const string ContextMenuItemsSourceTitle = "Context menu, Generate MenuItem by ItemsSource";
    public const string ContextMenuItemsSourceDescription = "Right click to bring up the context menu.";
    public const string MenuFlyoutTitle = "Menu Flyout";
    public const string MenuFlyoutDescription = "Right Click to show Context Flyout";
    public const string VerticalNavMenuTitle = "Vertical nav menu";
    public const string VerticalNavMenuDescription = "Submenus open as pop-ups.";
    public const string InlineMenuTitle = "Inline menu";
    public const string InlineMenuDescription = "Vertical menu with inline submenus.";
    public const string TopNavigationTitle = "Top Navigation";
    public const string TopNavigationDescription = "Horizontal top navigation menu.";
    public const string SwitchMenuTypeTitle = "Switch the menu type";
    public const string SwitchMenuTypeDescription = "Show the dynamic switching mode (between inline and vertical).";
    public const string DefaultOpenedPathsTitle = "Default Opened paths";
    public const string DefaultOpenedPathsDescription = "You can open or select the path by default.";
    public const string NavMenuItemItemsSourceTitle = "Generate NavMenuItem by ItemsSource";
    public const string NavMenuItemItemsSourceDescription = "Generate structure based on ItemsSource and template.";

    protected override Type GetResourceKindType() => typeof(MenuShowCaseLangResourceKind);
}
