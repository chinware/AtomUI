using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class TourToken : AbstractControlDesignToken
{
    public const string ID = "Tour";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    
    /// <summary>
    /// 关闭按钮尺寸
    /// Close button size
    /// </summary>
    public double CloseBtnSize { get; set; }
    
    /// <summary>
    /// Primary 模式上一步按钮背景色
    /// Background color of previous button in primary type
    /// </summary>
    public Color PrimaryPrevBtnBg { get; set; }
    
    /// <summary>
    /// Primary 模式下一步按钮悬浮背景色
    /// Hover background color of next button in primary type
    /// </summary>
    public Color PrimaryNextBtnHoverBg { get; set; }
    
    /// <summary>
    /// Tour 内容视图的最小宽度
    /// </summary>
    public double TourViewMinWidth { get; set; }
    
    /// <summary>
    /// Tour 内容视图的最小高度
    /// </summary>
    public double TourViewMinHeight { get; set; }
    
    /// <summary>
    /// 标题字体颜色
    /// Font color of title
    /// </summary>
    public Color HeaderColor { get; set; }

    #region 内部使用
    
    public double IndicatorSize { get; set; }
    public double PopupMarginToAnchor { get; set; }
    public CornerRadius TourBorderRadius { get; set; }
    
    #endregion
    
    public TourToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        CloseBtnSize          = SharedToken.FontSize * SharedToken.RelativeLineHeight;
        PrimaryPrevBtnBg      = SharedToken.ColorTextLightSolid.SetAlphaF(0.15);
        PrimaryNextBtnHoverBg = ColorUtils.OnBackground(SharedToken.ColorBgTextHover, SharedToken.ColorWhite);
        IndicatorSize         = 6;
        TourBorderRadius      = SharedToken.BorderRadiusLG;
        TourViewMinWidth      = 200;
        TourViewMinHeight     = 120;
        HeaderColor           = SharedToken.ColorTextHeading;
        PopupMarginToAnchor   = SharedToken.SpacingXXS;
    }
    
    protected override Type GetTokenKindType() => typeof(TourTokenKind);
}