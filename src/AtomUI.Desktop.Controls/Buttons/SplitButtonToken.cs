using AtomUI.Theme.DesignTokens;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class SplitButtonToken : AbstractControlDesignToken
{
    public double GutterToFlyout { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        GutterToFlyout = EffectiveGlobalToken.UniformlyMarginXXS;
    }
}
