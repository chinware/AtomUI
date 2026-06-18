using System.IO;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuThemeContractTests
{
    [Fact]
    public void NavMenu_Root_Background_Uses_Component_ItemBg_Token_Without_Right_Divider()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/NavMenuTheme.axaml");

        source.ShouldContain("<Setter Property=\"Background\" Value=\"{atom:NavMenuTokenResource ItemBg}\" />");
        source.ShouldContain("<Setter Property=\"Background\" Value=\"{atom:NavMenuTokenResource DarkMenuBg}\" />");
        source.ShouldContain("<Setter Property=\"BorderThickness\" Value=\"0\" />");
        source.ShouldContain("<Setter Property=\"IsVisible\" Value=\"False\" />");
        source.ShouldNotContain("<Setter Property=\"BorderThickness\" Value=\"0,0,1,0\" />");
        source.ShouldNotContain("<Setter Property=\"Background\" Value=\"Transparent\" />");
        source.ShouldNotContain("<Setter Property=\"Background\" Value=\"{atom:SharedTokenResource ColorBgContainer}\" />");
        source.ShouldNotContain("<Setter Property=\"Background\" Value=\"{atom:NavMenuTokenResource DarkMenuPopupBg}\" />");
    }

    [Fact]
    public void Inline_Submenu_Container_Uses_Submenu_Background_Tokens()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/NavMenuItemTheme.axaml");

        source.ShouldContain("Name=\"PART_ChildItemsFrame\"");
        source.ShouldContain("Value=\"{atom:NavMenuTokenResource SubMenuItemBg}\"");
        source.ShouldContain("Value=\"{atom:NavMenuTokenResource DarkSubMenuItemBg}\"");
    }

    [Fact]
    public void Item_Background_Control_Is_Public_And_Forwarded_To_Internal_Menu_Parts()
    {
        var navMenuSource       = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/NavMenu.cs");
        var navMenuItemSource   = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/NavMenuItem.cs");
        var headerSource        = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Header/BaseNavMenuItemHeader.cs");
        var navMenuThemeSource  = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/NavMenuItemTheme.axaml");

        navMenuSource.ShouldContain("public static readonly StyledProperty<bool> IsItemBackgroundEnabledProperty");
        navMenuSource.ShouldContain("AvaloniaProperty.Register<NavMenu, bool>(nameof(IsItemBackgroundEnabled), true)");
        navMenuSource.ShouldContain("public bool IsItemBackgroundEnabled");
        navMenuSource.ShouldContain("menuItem[!NavMenuItem.IsItemBackgroundEnabledProperty] = this[!IsItemBackgroundEnabledProperty];");

        navMenuItemSource.ShouldContain("internal static readonly StyledProperty<bool> IsItemBackgroundEnabledProperty");
        headerSource.ShouldContain("internal static readonly StyledProperty<bool> IsItemBackgroundEnabledProperty");
        navMenuThemeSource.ShouldContain("IsItemBackgroundEnabled=\"{TemplateBinding IsItemBackgroundEnabled}\"");
    }

    [Fact]
    public void Disabled_Item_Background_Mode_Gates_Item_And_Inline_Submenu_Backgrounds()
    {
        var headerThemeSource      = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/BaseNavMenuItemHeaderTheme.axaml");
        var horizontalThemeSource  = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/HorizontalNavMenuItemHeaderTheme.axaml");
        var navMenuItemThemeSource = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/NavMenuItemTheme.axaml");

        headerThemeSource.ShouldContain("Selector=\"^[IsItemBackgroundEnabled=True]\"");
        headerThemeSource.ShouldContain("Selector=\"^[IsItemBackgroundEnabled=False]\"");
        headerThemeSource.ShouldContain("<Setter Property=\"Background\" Value=\"Transparent\" />");

        horizontalThemeSource.ShouldContain("Selector=\"^[IsItemBackgroundEnabled=True]\"");
        horizontalThemeSource.ShouldContain("Selector=\"^[IsItemBackgroundEnabled=False]\"");

        navMenuItemThemeSource.ShouldContain("Selector=\"^[IsItemBackgroundEnabled=False]\"");
        navMenuItemThemeSource.ShouldContain("Selector=\"^ /template/ Border#PART_ChildItemsFrame\"");
        navMenuItemThemeSource.ShouldContain("<Setter Property=\"Background\" Value=\"Transparent\" />");
    }

    [Fact]
    public void Inline_And_Vertical_Item_Header_Backgrounds_Use_Content_Margin()
    {
        var inlineHeaderSource = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/InlineNavMenuItemHeaderTheme.axaml");
        var verticalHeaderSource = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/VerticalNavMenuItemHeaderTheme.axaml");

        inlineHeaderSource.ShouldContain("<Setter Property=\"Margin\" Value=\"{atom:NavMenuTokenResource ItemContentMargin}\" />");
        verticalHeaderSource.ShouldContain("<Setter Property=\"Margin\" Value=\"{atom:NavMenuTokenResource ItemContentMargin}\" />");
    }

    [Fact]
    public void Popup_Frame_Uses_Component_Popup_Background_Token()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/NavMenuItemTheme.axaml");

        source.ShouldContain("Value=\"{atom:NavMenuTokenResource MenuPopupBg}\"");
        source.ShouldNotContain("Value=\"{atom:SharedTokenResource ColorBgElevated}\"");
    }

    [Fact]
    public void Inline_And_Vertical_Pressed_State_Does_Not_Style_Header_From_Ancestor_NavMenuItem()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/NavMenuItemTheme.axaml");

        source.ShouldNotContain(":pressed:not(:selected):not(^[IsDarkStyle=True])");
        source.ShouldNotContain("Value=\"{atom:NavMenuTokenResource ItemActiveBg}\"");
    }

    [Fact]
    public void Submenu_Title_Hover_Uses_Foreground_And_Background_Tokens()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/BaseNavMenuItemHeaderTheme.axaml");

        source.ShouldContain("Value=\"{atom:NavMenuTokenResource ItemHoverColor}\"");
        source.ShouldContain("Value=\"{atom:NavMenuTokenResource ItemHoverBg}\"");
        source.ShouldContain("Value=\"{atom:NavMenuTokenResource DarkItemHoverColor}\"");
        source.ShouldContain("Value=\"{atom:NavMenuTokenResource DarkItemHoverBg}\"");
    }

    [Fact]
    public void NavMenu_Tokens_Keep_Root_And_Header_Background_Semantics_Separate()
    {
        var tokenSource = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/NavMenuToken.cs");
        var headerSource = ReadRepoFile("src/AtomUI.Desktop.Controls/NavMenu/Themes/BaseNavMenuItemHeaderTheme.axaml");

        tokenSource.ShouldContain("ItemBg                      = SharedToken.ColorBgContainer;");
        headerSource.ShouldContain("<Setter Property=\"Background\" Value=\"Transparent\" />");
    }

    private static string ReadRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
