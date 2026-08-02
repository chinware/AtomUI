using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class CollapseToken : AbstractControlDesignToken
{
    
    public CollapseToken()

    {
    }

    /// <summary>
    /// 折叠面板头部内边距
    /// </summary>
    public Thickness HeaderPadding { get; set; }

    /// <summary>
    /// 折叠面板头部背景
    /// </summary>
    public Color HeaderBg { get; set; }

    /// <summary>
    /// 折叠面板内容内边距
    /// </summary>
    public Thickness ContentPadding { get; set; }

    /// <summary>
    /// 折叠面板内容背景
    /// </summary>
    public Color ContentBg { get; set; }

    #region 内部 Token 定义

    public Thickness CollapseHeaderPaddingSM { get; set; }
    public Thickness CollapseHeaderPaddingLG { get; set; }
    public Thickness CollapseContentPaddingSM { get; set; }
    public Thickness CollapseContentPaddingLG { get; set; }
    public CornerRadius CollapsePanelBorderRadius { get; set; }
    public Thickness LeftExpandButtonMarginSM { get; set; }
    public Thickness LeftExpandButtonMargin { get; set; }
    public Thickness LeftExpandButtonMarginLG { get; set; }
    public Thickness RightExpandButtonMarginSM { get; set; }
    public Thickness RightExpandButtonMargin { get; set; }
    public Thickness RightExpandButtonMarginLG { get; set; }

    #endregion

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        HeaderPadding             = new Thickness(EffectiveGlobalToken.UniformlyPadding, EffectiveGlobalToken.UniformlyPaddingSM);
        HeaderBg                  = EffectiveGlobalToken.ColorFillAlter;
        ContentPadding            = new Thickness(16, EffectiveGlobalToken.UniformlyPadding);
        CollapseContentPaddingSM  = EffectiveGlobalToken.PaddingSM;
        CollapseContentPaddingLG  = EffectiveGlobalToken.PaddingLG;
        ContentBg                 = EffectiveGlobalToken.ColorBgContainer;
        CollapseHeaderPaddingSM   = new Thickness(EffectiveGlobalToken.UniformlyPaddingSM, EffectiveGlobalToken.UniformlyPaddingXS);
        CollapseHeaderPaddingLG   = new Thickness(EffectiveGlobalToken.UniformlyPaddingLG, EffectiveGlobalToken.UniformlyPadding);
        CollapsePanelBorderRadius = EffectiveGlobalToken.BorderRadiusLG;
        LeftExpandButtonMarginSM  = new Thickness(0, 0, EffectiveGlobalToken.UniformlyMarginXXS, 0);
        LeftExpandButtonMargin    = new Thickness(0, 0, EffectiveGlobalToken.UniformlyMarginXS, 0);
        LeftExpandButtonMarginLG  = new Thickness(0, 0, EffectiveGlobalToken.UniformlyMarginSM, 0);
        RightExpandButtonMarginSM = new Thickness(EffectiveGlobalToken.UniformlyMarginXXS, 0, 0, 0);
        RightExpandButtonMargin   = new Thickness(EffectiveGlobalToken.UniformlyMarginXS, 0, 0, 0);
        RightExpandButtonMarginLG   = new Thickness(EffectiveGlobalToken.UniformlyMarginSM, 0, 0, 0);
    }
    
}