using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Toolkits.GalleryBase.Controls;

[ControlDesignToken]
internal class ShowCaseItemToken : AbstractControlDesignToken
{
    public const string ID = "ShowCaseItem";

    public Thickness CardPadding { get; set; }
    public CornerRadius CardCornerRadius { get; set; }
    public BoxShadows CardShadow { get; set; }
    public Thickness PreviewMargin { get; set; }
    public Thickness BadgePreviewMargin { get; set; }
    public Thickness DescriptionMargin { get; set; }
    public FontWeight TitleFontWeight { get; set; }
    public double DeferredPlaceholderHeight { get; set; }
    public CornerRadius DeferredPlaceholderCornerRadius { get; set; }

    public ShowCaseItemToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        CardPadding       = new Thickness(SharedToken.SizeUnit * 5);
        CardCornerRadius  = SharedToken.BorderRadiusLG;
        CardShadow        = SharedToken.BoxShadowsTertiary;
        PreviewMargin     = new Thickness(0, 0, 0, SharedToken.SizeUnit * 8);
        BadgePreviewMargin = new Thickness(0, SharedToken.SizeUnit * 4, 0, SharedToken.SizeUnit * 8);
        DescriptionMargin = new Thickness(0, SharedToken.SizeUnit * 2 + 2, 0, 0);
        TitleFontWeight   = SharedToken.FontWeightStrong;
        DeferredPlaceholderHeight       = SharedToken.SizeUnit * 40;
        DeferredPlaceholderCornerRadius = SharedToken.BorderRadius;
    }

}
