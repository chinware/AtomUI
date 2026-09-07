using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class ToolTipToken : AbstractControlDesignToken
{
    
    public ToolTipToken()

    {
    }

    /// <summary>
    /// tooltip 的最大宽度，超过了就换行
    /// </summary>
    public double ToolTipMaxWidth { get; set; }

    /// <summary>
    /// ToolTip 默认的前景色
    /// </summary>
    public Color ToolTipColor { get; set; }

    /// <summary>
    /// ToolTip 默认的背景色
    /// </summary>
    public Color ToolTipBackground { get; set; }

    /// <summary>
    /// ToolTip 默认的圆角
    /// </summary>
    public CornerRadius ToolTipCornerRadius { get; set; }

    /// <summary>
    /// ToolTip 默认的内间距
    /// </summary>
    public Thickness ContentPadding { get; set; }
    
    /// <summary>
    /// 动画时长
    /// </summary>
    public TimeSpan MotionDuration { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        ToolTipMaxWidth   = 250;
        ToolTipColor      = EffectiveGlobalToken.ColorTextLightSolid;
        ToolTipBackground = EffectiveGlobalToken.ColorBgSpotlight;
        ToolTipCornerRadius = new CornerRadius(4);
        ContentPadding = new Thickness(EffectiveGlobalToken.UniformlyPaddingSM, EffectiveGlobalToken.UniformlyPaddingSM / 2 + 2);
        MotionDuration = EffectiveGlobalToken.MotionDurationMid;
    }
    
}
