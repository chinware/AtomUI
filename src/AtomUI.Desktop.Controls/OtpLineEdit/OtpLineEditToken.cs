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

    /// <summary>
    /// 分隔符字形水平光学补偿（相对 FontSize 的比例）。
    /// 星号等分隔符的墨迹相对字符 advance 偏左，按字号缩放右移校正。
    /// </summary>
    public double SeparatorGlyphCompensationRatioX { get; set; }

    /// <summary>
    /// 分隔符字形垂直光学补偿（相对 FontSize 的比例）。
    /// 星号等分隔符的墨迹绘制在行盒上半区，按字号缩放下沉校正。
    /// </summary>
    public double SeparatorGlyphCompensationRatioY { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        CellWidth               = EffectiveGlobalToken.ControlHeight;
        CellWidthLG             = EffectiveGlobalToken.ControlHeightLG;
        CellWidthSM             = EffectiveGlobalToken.ControlHeightSM;
        CellGap                 = EffectiveGlobalToken.UniformlyPaddingXXS;
        // 14px 默认输入字号下约 1.5px / 3px 的设计校准值，随字号等比缩放
        SeparatorGlyphCompensationRatioX = 0.10;
        SeparatorGlyphCompensationRatioY = 0.21;
    }

}
