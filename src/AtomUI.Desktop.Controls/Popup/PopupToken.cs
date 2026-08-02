using AtomUI.Media;
using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class PopupToken : AbstractControlDesignToken
{
    public CornerRadius PopupCornerRadius { get; set; }

    public BoxShadows OverlayHostShadow { get; set; }

    public BoxShadows PopupRootShadow { get; set; }

    public double MarginToAnchor { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        OverlayHostShadow = EffectiveGlobalToken.BoxShadowsSecondary;
        PopupRootShadow = new BoxShadows(new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 1,
            Blur    = 6,
            Spread  = 0,
            Color   = ColorUtils.FromRgbF(0.14, 0, 0, 0)
        }, [new BoxShadow
        {
            OffsetX = 0,
            OffsetY = 0,
            Blur    = 4,
            Spread  = 0,
            Color   = ColorUtils.FromRgbF(0.10, 0, 0, 0)
        }]);
        PopupCornerRadius = EffectiveGlobalToken.BorderRadiusLG;
        MarginToAnchor = EffectiveGlobalToken.UniformlyMarginXXS;
    }
}
