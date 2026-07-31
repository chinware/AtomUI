using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Toolkits.GalleryBase.Controls;

[ControlDesignToken]
internal class GalleryWindowTitleBarToken : AbstractControlDesignToken
{
    public const string ID = "GalleryWindowTitleBar";

    public FontWeight MenuFontWeight { get; set; }
    public Thickness MenuMargin { get; set; }

    public GalleryWindowTitleBarToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        MenuFontWeight = FontWeight.Normal;
        MenuMargin     = new Thickness(0, 0, SharedToken.SizeUnit * 2, 0);
    }

}
