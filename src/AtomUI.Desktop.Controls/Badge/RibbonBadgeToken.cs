using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class RibbonBadgeToken : AbstractControlDesignToken
{
    public Point Offset { get; set; }

    public ImmutableTransform? CornerTransform { get; set; }

    public int CornerDarkenAmount { get; set; }

    public Thickness TextPadding { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        Offset             = new Point(EffectiveGlobalToken.UniformlyMarginXS, EffectiveGlobalToken.UniformlyMarginXS);
        CornerTransform    = new ScaleTransform(1, 0.75).ToImmutable();
        CornerDarkenAmount = 15;
        TextPadding        = new Thickness(EffectiveGlobalToken.UniformlyPaddingXS, 0);
    }
}
