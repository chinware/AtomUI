using AtomUI.Theme.DesignTokens;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class OtpLineEditToken : AbstractControlDesignToken
{

    public OtpLineEditToken()

    {
    }

    public double CellWidth { get; set; }

    public double CellWidthLG { get; set; }

    public double CellWidthSM { get; set; }

    public double CellGap { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        CellWidth               = EffectiveGlobalToken.ControlHeight;
        CellWidthLG             = EffectiveGlobalToken.ControlHeightLG;
        CellWidthSM             = EffectiveGlobalToken.ControlHeightSM;
        CellGap                 = EffectiveGlobalToken.UniformlyPaddingXXS;
    }

}
