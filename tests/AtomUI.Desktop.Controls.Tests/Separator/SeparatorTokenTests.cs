using AtomUI.Theme.TokenSystem;
using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Separator;

public class SeparatorTokenTests
{
    [Fact]
    public void Horizontal_Separator_Without_Title_Uses_Minimal_Block_Margin()
    {
        var sharedToken = new DesignToken
        {
            UniformlyMarginXS = 8,
            UniformlyMargin   = 16,
            UniformlyMarginLG = 24
        };
        var separatorToken = new SeparatorToken();
        separatorToken.AssignSharedToken(sharedToken);

        separatorToken.CalculateTokenValues(isDarkMode: false);

        separatorToken.HorizontalMarginBlockSM.ShouldBe(new Thickness(0, 1));
        separatorToken.HorizontalMarginBlock.ShouldBe(new Thickness(0, 1));
        separatorToken.HorizontalMarginBlockLG.ShouldBe(new Thickness(0, 1));
        separatorToken.HorizontalWithTextGutterMargin.ShouldBe(new Thickness(0, sharedToken.UniformlyMargin));
    }
}
