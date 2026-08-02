using AtomUI.Theme.DesignTokens;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class OtpLineEditToken : AbstractControlDesignToken
{

    public OtpLineEditToken()

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

        CellWidth               = EffectiveGlobalToken.ControlHeight;
        CellWidthLG             = EffectiveGlobalToken.ControlHeightLG;
        CellWidthSM             = EffectiveGlobalToken.ControlHeightSM;
        CellGap                 = EffectiveGlobalToken.UniformlyPaddingXXS;
        CellGapLG               = EffectiveGlobalToken.UniformlyPaddingXS;
        CellGapSM               = EffectiveGlobalToken.UniformlyPaddingXXS;
        SeparatorMarginInline   = EffectiveGlobalToken.UniformlyPaddingXXS;
        SeparatorMarginInlineLG = EffectiveGlobalToken.UniformlyPaddingXS;
        SeparatorMarginInlineSM = EffectiveGlobalToken.UniformlyPaddingXXS;
    }

}
