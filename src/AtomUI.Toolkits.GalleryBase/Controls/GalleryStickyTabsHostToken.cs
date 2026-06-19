using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using AtomUI.Toolkits.GalleryBase.Controls.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Toolkits.GalleryBase.Controls;

[ControlDesignToken]
internal class GalleryStickyTabsHostToken : AbstractControlDesignToken
{
    public const string ID = "GalleryStickyTabsHost";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    public Thickness StickyContentPadding { get; set; }
    public IBrush? StickyBackground { get; set; }
    public IBrush? StickyBorderBrush { get; set; }

    public GalleryStickyTabsHostToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        StickyContentPadding = new Thickness(SharedToken.SizeUnit * 7, 0);
        StickyBackground     = new SolidColorBrush(SharedToken.ColorBgLayout);
        StickyBorderBrush    = new SolidColorBrush(SharedToken.ColorBorderSecondary);
    }

    protected override Type GetTokenKindType() => typeof(GalleryStickyTabsHostTokenKind);
}
