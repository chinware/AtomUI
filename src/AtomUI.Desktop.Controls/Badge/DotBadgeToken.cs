using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class DotBadgeToken : AbstractControlDesignToken
{
    public double DotSize { get; set; }

    public Color DotColor { get; set; }

    public double ShadowSize { get; set; }

    public Color ShadowColor { get; set; }

    public Thickness LabelMargin { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        DotSize     = EffectiveGlobalToken.FontSizeSM / 2;
        DotColor    = EffectiveGlobalToken.ColorError;
        ShadowSize  = EffectiveGlobalToken.LineWidth;
        ShadowColor = EffectiveGlobalToken.ColorBorderBg;
        LabelMargin = new Thickness(EffectiveGlobalToken.UniformlyMarginXS, 0, 0, 0);
    }
}
