using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Toolkits.GalleryBase.Controls;

[ControlDesignToken]
internal class GalleryWindowTitleBarToken : AbstractControlDesignToken
{

    public FontWeight MenuFontWeight { get; set; }
    public Thickness MenuMargin { get; set; }

    public GalleryWindowTitleBarToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        MenuFontWeight = FontWeight.Normal;
        MenuMargin     = new Thickness(0, 0, EffectiveGlobalToken.SizeUnit * 2, 0);
    }

}
