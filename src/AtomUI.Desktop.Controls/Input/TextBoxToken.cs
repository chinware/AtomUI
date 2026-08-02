using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class TextBoxToken : AbstractControlDesignToken
{

    public TextBoxToken()

    {
    }

    /// <summary>
    /// 默认边框色
    /// </summary>
    public Color BorderColor { get; set; }

    /// <summary>
    /// 默认内边距
    /// </summary>
    public Thickness ContentPadding { get; set; }

    /// <summary>
    /// 小号内边距
    /// </summary>
    public Thickness ContentPaddingSM { get; set; }

    /// <summary>
    /// 大号内边距
    /// </summary>
    public Thickness ContentPaddingLG { get; set; }

    /// <summary>
    /// 悬浮态边框色
    /// </summary>
    public Color HoverBorderColor { get; set; }

    /// <summary>
    /// 激活态边框色
    /// </summary>
    public Color ActiveBorderColor { get; set; }

    /// <summary>
    /// 激活态阴影
    /// </summary>
    public BoxShadows ActiveShadow { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        var fontSize     = EffectiveGlobalToken.FontSize;
        var fontSizeLG   = EffectiveGlobalToken.FontSizeLG;
        var lineHeight   = EffectiveGlobalToken.RelativeLineHeight;
        var lineHeightLG = EffectiveGlobalToken.RelativeLineHeightLG;
        var lineWidth    = EffectiveGlobalToken.LineWidth;

        BorderColor     = EffectiveGlobalToken.ColorBorder;
        ContentPadding = new Thickness(EffectiveGlobalToken.UniformlyPaddingSM - lineWidth,
            Math.Round((EffectiveGlobalToken.ControlHeight - fontSize * lineHeight) / 2 * 10) / 10 - lineWidth);
        ContentPaddingSM = new Thickness(EffectiveGlobalToken.ControlPaddingHorizontalSM - lineWidth,
            Math.Round((EffectiveGlobalToken.ControlHeightSM - fontSize * lineHeight) / 2 * 10) / 10 - lineWidth * 2);
        ContentPaddingLG = new Thickness(EffectiveGlobalToken.ControlPaddingHorizontal - lineWidth,
            Math.Ceiling((EffectiveGlobalToken.ControlHeightLG - fontSizeLG * lineHeightLG) / 2 * 10) / 10 -
            lineWidth);
        HoverBorderColor  = EffectiveGlobalToken.ColorPrimaryHover;
        ActiveBorderColor = EffectiveGlobalToken.ColorPrimary;
        ActiveShadow = new BoxShadows(new BoxShadow
        {
            Spread = EffectiveGlobalToken.ControlOutlineWidth,
            Color  = EffectiveGlobalToken.ColorControlOutline
        });
    }

}
