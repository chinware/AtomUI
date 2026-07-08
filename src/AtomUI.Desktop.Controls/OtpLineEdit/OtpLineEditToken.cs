using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class OtpLineEditToken : AbstractControlDesignToken
{
    public const string ID = "OtpLineEdit";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);

    public OtpLineEditToken()
        : base(ID)
    {
    }

    public double CellWidth { get; set; }

    public double CellWidthLG { get; set; }

    public double CellWidthSM { get; set; }

    public double CellGap { get; set; }

    public double CellGapLG { get; set; }

    public double CellGapSM { get; set; }

    public double SeparatorMarginInline { get; set; }

    public double SeparatorMarginInlineLG { get; set; }

    public double SeparatorMarginInlineSM { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        CellWidth               = SharedToken.ControlHeight;
        CellWidthLG             = SharedToken.ControlHeightLG;
        CellWidthSM             = SharedToken.ControlHeightSM;
        CellGap                 = SharedToken.UniformlyPaddingXXS;
        CellGapLG               = SharedToken.UniformlyPaddingXS;
        CellGapSM               = SharedToken.UniformlyPaddingXXS;
        SeparatorMarginInline   = SharedToken.UniformlyPaddingXXS;
        SeparatorMarginInlineLG = SharedToken.UniformlyPaddingXS;
        SeparatorMarginInlineSM = SharedToken.UniformlyPaddingXXS;
    }

    protected override Type GetTokenKindType() => typeof(OtpLineEditTokenKind);
}
