using AtomUI.Theme.DesignTokens;
using Avalonia;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class GroupBoxToken : AbstractControlDesignToken
{
    
    public GroupBoxToken()

    {
    }

    /// <summary>
    /// 文本横向内间距 单位 em
    /// </summary>
    public double TextPaddingInline { get; set; }

    /// <summary>
    /// 文本与边缘距离的比例，取值 0 ～ 1
    /// </summary>
    public double OrientationMarginPercent { get; set; }

    /// <summary>
    /// 纵向分割线的横向外间距
    /// </summary>
    public double VerticalMarginInline { get; set; }

    /// <summary>
    /// 内容区域内间距
    /// </summary>
    public Thickness ContentPadding { get; set; }

    /// <summary>
    /// Header 容器外间距
    /// </summary>
    public Thickness HeaderContainerMargin { get; set; }

    /// <summary>
    /// 头部图标的外间距
    /// </summary>
    public Thickness HeaderIconMargin { get; set; }

    /// <summary>
    /// Header 内容区域内间距
    /// </summary>
    public Thickness HeaderContentPadding { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        TextPaddingInline        = 1.0;
        OrientationMarginPercent = 0.05;
        VerticalMarginInline     = EffectiveGlobalToken.UniformlyMarginXS;
        ContentPadding           = EffectiveGlobalToken.PaddingXS;
        HeaderContainerMargin = new Thickness(EffectiveGlobalToken.UniformlyMargin, EffectiveGlobalToken.UniformlyMarginXS,
            EffectiveGlobalToken.UniformlyMargin, 0);
        HeaderContentPadding = new Thickness(EffectiveGlobalToken.UniformlyPaddingXXS, 0);
        HeaderIconMargin     = new Thickness(0, 0, EffectiveGlobalToken.UniformlyMarginXXS, 0);
    }
    
}