using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Toolkits.GalleryBase.Controls;

[ControlDesignToken]
internal class ShowCaseItemToken : AbstractControlDesignToken
{

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

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        CardPadding       = new Thickness(EffectiveGlobalToken.SizeUnit * 5);
        CardCornerRadius  = EffectiveGlobalToken.BorderRadiusLG;
        CardShadow        = EffectiveGlobalToken.BoxShadowsTertiary;
        PreviewMargin     = new Thickness(0, 0, 0, EffectiveGlobalToken.SizeUnit * 8);
        BadgePreviewMargin = new Thickness(0, EffectiveGlobalToken.SizeUnit * 4, 0, EffectiveGlobalToken.SizeUnit * 8);
        DescriptionMargin = new Thickness(0, EffectiveGlobalToken.SizeUnit * 2 + 2, 0, 0);
        TitleFontWeight   = EffectiveGlobalToken.FontWeightStrong;
        DeferredPlaceholderHeight       = EffectiveGlobalToken.SizeUnit * 40;
        DeferredPlaceholderCornerRadius = EffectiveGlobalToken.BorderRadius;
    }

}
