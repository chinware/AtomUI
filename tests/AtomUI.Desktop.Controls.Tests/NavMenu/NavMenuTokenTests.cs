using AtomUI.Theme.TokenSystem;
using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.NavMenu;

public class NavMenuTokenTests
{
    [Fact]
    public void Item_Height_Uses_Large_Control_Height_To_Match_AntDesign_Menu()
    {
        var sharedToken = new DesignToken
        {
            ControlHeight   = 32,
            ControlHeightLG = 40
        };
        var navMenuToken = new NavMenuToken();
        navMenuToken.AssignSharedToken(sharedToken);

        navMenuToken.CalculateTokenValues(isDarkMode: false);

        navMenuToken.ItemHeight.ShouldBe(sharedToken.ControlHeightLG);
    }

    [Fact]
    public void Vertical_Item_Gap_Uses_Collapsed_Block_Margin_To_Match_AntDesign_Menu()
    {
        var sharedToken = new DesignToken
        {
            UniformlyMarginXXS = 4
        };
        var navMenuToken = new NavMenuToken();
        navMenuToken.AssignSharedToken(sharedToken);

        navMenuToken.CalculateTokenValues(isDarkMode: false);

        navMenuToken.ItemContentMargin.ShouldBe(new Thickness(4, 0, 4, 4));
        navMenuToken.VerticalMenuContentPadding.ShouldBe(new Thickness(0, 4, 0, 0));
        navMenuToken.VerticalItemsPanelSpacing.ShouldBe(0);
        navMenuToken.VerticalChildItemsMargin.ShouldBe(new Thickness(0, 0, 0, 4));
    }
}
