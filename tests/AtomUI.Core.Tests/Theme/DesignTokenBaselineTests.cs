using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class DesignTokenBaselineTests
{
    [Fact]
    public void DefaultCalculator_Derives_Stable_Primary_And_Control_Size_Tokens()
    {
        var token = new DesignToken();
        var calculator = new DefaultThemeVariantCalculator();

        calculator.Calculate(token);
        token.CalculateAliasTokenValues();

        token.ColorPrimary.ShouldBe(Color.Parse("#1677ff"));
        token.ControlHeight.ShouldBe(32);
        token.BorderRadius.ShouldBe(new CornerRadius(6));
    }
}
