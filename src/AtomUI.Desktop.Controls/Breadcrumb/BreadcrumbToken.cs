using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class BreadcrumbToken : AbstractControlDesignToken
{

    /// <summary>
    /// 面包屑项文字颜色
    /// </summary>
    public Color ItemColor { get; set; }

    /// <summary>
    /// 最后一项文字颜色
    /// </summary>
    public Color LastItemColor { get; set; }

    /// <summary>
    /// 链接文字颜色
    /// </summary>
    public Color LinkColor { get; set; }

    /// <summary>
    /// 链接文字悬浮颜色
    /// </summary>
    public Color LinkHoverColor { get; set; }

    /// <summary>
    /// 链接文字悬浮背景颜色
    /// </summary>
    public Color LinkHoverBgColor { get; set; }

    /// <summary>
    /// BreadcrumbItem外面Border的Padding
    /// </summary>
    public Thickness BreadcrumbItemContentPadding { get; set; }

    /// <summary>
    /// 分隔符颜色
    /// </summary>
    public Color SeparatorColor { get; set; }

    /// <summary>
    /// 分隔符外间距
    /// </summary>
    public Thickness SeparatorMargin { get; set; }

    public BreadcrumbToken()

    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        ItemColor                   = EffectiveGlobalToken.ColorTextDescription;
        LastItemColor               = EffectiveGlobalToken.ColorText;
        LinkColor                   = EffectiveGlobalToken.ColorTextDescription;
        LinkHoverColor              = EffectiveGlobalToken.ColorText;
        LinkHoverBgColor            = EffectiveGlobalToken.ColorBgTextHover;
        SeparatorColor              = EffectiveGlobalToken.ColorTextDescription;
        SeparatorMargin             = new Thickness(EffectiveGlobalToken.UniformlyMarginXXS, 0);
        BreadcrumbItemContentPadding = new Thickness(EffectiveGlobalToken.UniformlyPaddingXXS, EffectiveGlobalToken.UniformlyPaddingXXS,
            EffectiveGlobalToken.UniformlyPaddingXXS, EffectiveGlobalToken.UniformlyPaddingXXS);
    }
    
}
