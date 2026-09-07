using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class TextAreaToken : AbstractControlDesignToken
{
    
    public TextAreaToken()

    {
    }
    
    /// <summary>
    /// Resize 指示器颜色
    /// </summary>
    public Color ResizeIndicatorLineColor { get; set; }
    
    /// <summary>
    /// Resize 指示器大小
    /// </summary>
    public double ResizeHandleSize { get; set; }
    
    /// <summary>
    /// 输入框内边距
    /// </summary>
    public Thickness RightAddOnPadding { get; set; }

    /// <summary>
    /// 小号输入框内边距
    /// </summary>
    public Thickness RightAddOnPaddingSM { get; set; }

    /// <summary>
    /// 大号输入框内边距
    /// </summary>
    public Thickness RightAddOnPaddingLG { get; set; }

    /// <summary>
    /// 左侧内部附加内容与输入内容之间的间距。
    /// </summary>
    public Thickness LeftInnerAddOnMargin { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        ResizeIndicatorLineColor = EffectiveGlobalToken.ColorTextDescription;
        ResizeHandleSize         = EffectiveGlobalToken.SizeXS;
        
        var lineWidth    = EffectiveGlobalToken.LineWidth;
        RightAddOnPadding   = new Thickness(0, 0, EffectiveGlobalToken.UniformlyPaddingSM - lineWidth, 0);
        RightAddOnPaddingSM = new Thickness(0, 0, EffectiveGlobalToken.ControlPaddingHorizontalSM - lineWidth, 0);
        RightAddOnPaddingLG = new Thickness(0, 0, EffectiveGlobalToken.ControlPaddingHorizontal - lineWidth, 0);
        LeftInnerAddOnMargin = new Thickness(0, 0, EffectiveGlobalToken.UniformlyMarginXXS, 0);
    }
    
}
