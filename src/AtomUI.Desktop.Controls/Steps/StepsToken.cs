using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class StepsToken : AbstractControlDesignToken
{

    public StepsToken()

    {
    }
    
    /// <summary>
    /// 描述区域最大宽度
    /// Max width of description area
    /// </summary>
    public double DescriptionMaxWidth { get; set; }
    
    /// <summary>
    /// 自定义图标容器尺寸
    /// Size of custom icon container
    /// </summary>
    public double CustomIconSize { get; set; }
    
    /// <summary>
    /// 自定义图标大小
    /// Font size of custom icon
    /// </summary>
    public double CustomIconFontSize { get; set; }
    
    /// <summary>
    /// 图标容器尺寸
    /// Size of icon container
    /// </summary>
    public double IconContainerSize { get; set; }
    
    /// <summary>
    /// 图标大小
    /// Size of icon
    /// </summary>
    public double IconFontSize { get; set; }
    
    /// <summary>
    /// 点状步骤点大小
    /// Size of dot
    /// </summary>
    public double DotSize { get; set; }
    
    /// <summary>
    /// 点状步骤点当前大小
    /// Current size of dot
    /// </summary>
    public double DotCurrentSize { get; set; }
    
    /// <summary>
    /// 可跳转步骤条箭头颜色
    /// Color of arrow in nav
    /// </summary>
    public Color NavArrowColor { get; set; }
    
    /// <summary>
    /// 小号步骤条图标大小
    /// Size of small steps icon
    /// </summary>
    public double IconContainerSizeSM { get; set; }
    
    /// <summary>
    /// Label 水平排列的时候的外间距
    /// External spacing when Label is arranged horizontally
    /// </summary>
    public Thickness HorizontalHeaderMargin { get; set; }

    /// <summary>
    /// 子标题外间距
    /// External spacing of subtitle
    /// </summary>
    public Thickness SubHeaderMargin { get; set; }
    
    /// <summary>
    /// 垂直排列的时候 item 的间距
    /// </summary>
    public double VerticalItemSpacing { get; set; }
    
    /// <summary>
    /// 垂直排列描述的内间距
    /// </summary>
    public Thickness VerticalDescriptionPadding { get; set; }
    
    /// <summary>
    /// 垂直标签排列时候内容跟图标之间的外间距
    /// </summary>
    public Thickness VerticalLabelContentMargin { get; set; }
    
    /// <summary>
    /// 垂直导航类型箭头的外间距
    /// </summary>
    public Thickness VerticalNavArrowMargin { get; set; }
    
    /// <summary>
    /// 垂直导航类型箭头的外间距，小尺寸
    /// </summary>
    public Thickness VerticalNavArrowMarginSM { get; set; }

    /// <summary>
    /// 垂直导航类型 item 内边距
    /// </summary>
    public Thickness VerticalNavItemPadding { get; set; }
    
    /// <summary>
    /// 垂直导航类型内容和指示线的间距
    /// </summary>
    public double NavItemGutter { get; set; }

    #region 内部 Token

    public Color WaitIconColor { get; set; }
    public Color WaitIconBgColor { get; set; }
    public Color WaitIconBorderColor { get; set; }
    public Color FinishIconBgColor { get; set; }
    public Color FinishIconBorderColor { get; set; }
    
    public Color ProcessTailColor { get; set; }
    public Color ProcessIconColor { get; set; }
    public Color ProcessTitleColor { get; set; }
    public Color ProcessDescriptionColor { get; set; }
    public Color ProcessIconBgColor { get; set; }
    public Color ProcessIconBorderColor { get; set; }
    public Color ProcessDotColor { get; set; }
    public Color WaitTitleColor { get; set; }
    public Color WaitDescriptionColor { get; set; }
    public Color WaitTailColor { get; set; }
    public Color WaitDotColor { get; set; }
    public Color FinishIconColor { get; set; }
    public Color FinishTitleColor { get; set; }
    public Color FinishDescriptionColor { get; set; }
    public Color FinishTailColor { get; set; }
    public Color FinishDotColor { get; set; }
    public Color ErrorIconColor { get; set; }
    public Color ErrorTitleColor { get; set; }
    public Color ErrorDescriptionColor { get; set; }
    public Color ErrorTailColor { get; set; }
    public Color ErrorIconBgColor { get; set; }
    public Color ErrorIconBorderColor { get; set; }
    public Color ErrorDotColor { get; set; }
    public Color StepsNavActiveColor { get; set; }
    // Steps inline variable
    public double InlineDotSize { get; set; }
    public Thickness InlineHeaderMargin { get; set; }
    public Thickness InlineHeaderPadding { get; set; }
    public Thickness InlineItemPadding { get; set; }
    public Color InlineTitleColor { get; set; }
    public Color InlineTailColor { get; set; }
    public double DotLineThickness { get; set; }
    public Thickness ProgressFramePadding { get; set; }
    public Thickness ProgressFramePaddingSM { get; set; }
    public Color ProgressGrooveColor { get; set; }
    public Color ProgressColor { get; set; }
    
    #endregion

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        
        CustomIconSize      = EffectiveGlobalToken.ControlHeight;
        CustomIconFontSize  = EffectiveGlobalToken.ControlHeightSM;
        IconContainerSize   = EffectiveGlobalToken.ControlHeight;
        IconFontSize        = EffectiveGlobalToken.FontSize;
        IconContainerSizeSM = EffectiveGlobalToken.FontSizeHeading3;
        DotSize             = EffectiveGlobalToken.ControlHeight / 4;
        DotCurrentSize      = EffectiveGlobalToken.ControlHeightLG / 4;
        NavArrowColor       = EffectiveGlobalToken.ColorTextDisabled;
        DescriptionMaxWidth = 140;

        WaitIconColor         = EffectiveGlobalToken.ColorTextLabel;
        WaitIconBgColor       = EffectiveGlobalToken.ColorFillContent;
        WaitIconBorderColor   = Colors.Transparent;
        WaitTitleColor        = EffectiveGlobalToken.ColorTextDescription;
        WaitDescriptionColor  = EffectiveGlobalToken.ColorTextDescription;
        WaitTailColor         = EffectiveGlobalToken.ColorTextDisabled;
        WaitDotColor          = EffectiveGlobalToken.ColorTextDisabled;
        
        ProcessIconColor        = EffectiveGlobalToken.ColorTextLightSolid;
        ProcessTitleColor       = EffectiveGlobalToken.ColorText;
        ProcessDescriptionColor = EffectiveGlobalToken.ColorText;
        ProcessIconBgColor      = EffectiveGlobalToken.ColorPrimary;
        ProcessIconBorderColor  = EffectiveGlobalToken.ColorPrimary;
        ProcessDotColor         = EffectiveGlobalToken.ColorPrimary;
        ProcessTailColor        = EffectiveGlobalToken.ColorPrimary;

        ProgressGrooveColor = EffectiveGlobalToken.ColorSplit;
        ProgressColor       = EffectiveGlobalToken.ColorPrimary;
        
        FinishIconBgColor       = EffectiveGlobalToken.ControlItemBgActive;
        FinishIconBorderColor   = EffectiveGlobalToken.ControlItemBgActive;
        FinishIconColor         = EffectiveGlobalToken.ColorPrimary;
        FinishTitleColor        = EffectiveGlobalToken.ColorText;
        FinishDescriptionColor  = EffectiveGlobalToken.ColorTextDescription;
        FinishTailColor         = EffectiveGlobalToken.ColorPrimary;
        FinishDotColor          = EffectiveGlobalToken.ColorPrimary;
        
        ErrorIconColor          = EffectiveGlobalToken.ColorTextLightSolid;
        ErrorTitleColor         = EffectiveGlobalToken.ColorError;
        ErrorDescriptionColor   = EffectiveGlobalToken.ColorError;
        ErrorTailColor          = EffectiveGlobalToken.ColorError;
        ErrorIconBgColor        = EffectiveGlobalToken.ColorError;
        ErrorIconBorderColor    = EffectiveGlobalToken.ColorError;
        ErrorDotColor           = EffectiveGlobalToken.ColorError;
        StepsNavActiveColor     = EffectiveGlobalToken.ColorPrimary;
        // Steps inline variable
        InlineDotSize       = 6;
        InlineHeaderMargin  = new Thickness();
        InlineHeaderPadding = new Thickness();
        InlineTitleColor    = EffectiveGlobalToken.ColorTextSecondary;
        InlineTailColor     = EffectiveGlobalToken.ColorTextDisabled;
        var inlineItemHorizontalPadding = EffectiveGlobalToken.UniformlyPaddingXXS + EffectiveGlobalToken.UniformlyMarginXXS / 2;
        InlineItemPadding   = new Thickness(
            inlineItemHorizontalPadding,
            EffectiveGlobalToken.UniformlyPaddingXS + EffectiveGlobalToken.LineWidth,
            inlineItemHorizontalPadding,
            0);
        
        HorizontalHeaderMargin     = new Thickness(EffectiveGlobalToken.UniformlyMargin, 0);
        SubHeaderMargin            = new Thickness(EffectiveGlobalToken.UniformlyMarginXS, 0, 0, 0);
        VerticalItemSpacing        = EffectiveGlobalToken.UniformlyMarginXXS;
        VerticalDescriptionPadding = new Thickness();
        DotLineThickness           = EffectiveGlobalToken.LineWidth * 3;

        VerticalLabelContentMargin = new Thickness(0, EffectiveGlobalToken.UniformlyMarginSM, 0, 0);
        VerticalNavArrowMargin     = new Thickness();
        VerticalNavArrowMarginSM   = new Thickness();
        VerticalNavItemPadding     = new Thickness(EffectiveGlobalToken.UniformlyPaddingSM, EffectiveGlobalToken.UniformlyPadding);
        NavItemGutter              = EffectiveGlobalToken.Spacing;
        ProgressFramePadding       = new Thickness(EffectiveGlobalToken.LineWidthBold * 2);
        ProgressFramePaddingSM     = new Thickness(EffectiveGlobalToken.LineWidthBold * 2);
    }
    
}
