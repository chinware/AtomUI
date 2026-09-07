using AtomUI.Media;
using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class ArrowDecoratedBoxToken : AbstractControlDesignToken
{

    /// <summary>
    /// 箭头三角形大小
    /// </summary>
    public double ArrowSize { get; set; }

    /// <summary>
    /// 默认的内边距
    /// </summary>
    public Thickness ContentPadding { get; set; }

    /// <summary>
    /// 箭头描边颜色
    /// </summary>
    public Color ArrowStrokeColor { get; set; }

    /// <summary>
    /// 箭头描边粗细
    /// </summary>
    public double ArrowStrokeThickness { get; set; }

    public ArrowDecoratedBoxToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        ArrowSize            = EffectiveGlobalToken.SizePopupArrow / 1.3;
        ContentPadding       = EffectiveGlobalToken.PaddingXS;
        ArrowStrokeColor     = ColorUtils.FromRgbF(0.07, 0, 0, 0);
        ArrowStrokeThickness = 1;
    }

}
