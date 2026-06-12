using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using AtomUIGallery.Controls.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUIGallery.Controls;

[ControlDesignToken]
internal class ShowCaseItemToken : AbstractControlDesignToken
{
    public const string ID = "ShowCaseItem";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    public Thickness CardPadding { get; set; }
    public CornerRadius CardCornerRadius { get; set; }
    public BoxShadows CardShadow { get; set; }
    public Thickness PreviewMargin { get; set; }
    public Thickness DescriptionMargin { get; set; }
    public FontWeight TitleFontWeight { get; set; }

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
        DescriptionMargin = new Thickness(0, SharedToken.SizeUnit * 2 + 2, 0, 0);
        TitleFontWeight   = SharedToken.FontWeightStrong;
    }

    protected override Type GetTokenKindType() => typeof(ShowCaseItemTokenKind);
}
