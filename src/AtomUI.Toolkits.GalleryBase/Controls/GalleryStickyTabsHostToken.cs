using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Toolkits.GalleryBase.Controls;

[ControlDesignToken]
internal sealed class GalleryStickyTabsHostToken : AbstractControlDesignToken
{

    public Thickness StickyContentPadding { get; set; }
    public IBrush? StickyBackground { get; set; }
    public IBrush? StickyBorderBrush { get; set; }

    public GalleryStickyTabsHostToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        StickyContentPadding = new Thickness(EffectiveGlobalToken.SizeUnit * 7, 0);
        StickyBackground     = new SolidColorBrush(EffectiveGlobalToken.ColorBgLayout);
        StickyBorderBrush    = new SolidColorBrush(EffectiveGlobalToken.ColorBorderSecondary);
    }

}
