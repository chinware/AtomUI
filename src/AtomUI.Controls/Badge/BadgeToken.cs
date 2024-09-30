using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class BadgeToken : AbstractControlDesignToken
{
    public const string ID = "Badge";

    public BadgeToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 徽标高度
    /// </summary>
    public double IndicatorHeight { get; set; }

    /// <summary>
    /// 小号徽标高度
    /// </summary>
    public double IndicatorHeightSM { get; set; }

    /// <summary>
    /// 点状徽标尺寸
    /// </summary>
    public double DotSize { get; set; }

    /// <summary>
    /// 徽标文本尺寸
    /// </summary>
    public double TextFontSize { get; set; }

    /// <summary>
    /// 小号徽标文本尺寸
    /// </summary>
    public double TextFontSizeSM { get; set; }

    /// <summary>
    /// 徽标文本粗细
    /// </summary>
    public FontWeight TextFontWeight { get; set; }

    /// <summary>
    /// 状态徽标尺寸
    /// </summary>
    public double StatusSize { get; set; }

    #region 内部使用的 Token

    public double BadgeFontHeight { get; set; }
    public Color BadgeTextColor { get; set; }
    public Color BadgeColor { get; set; }
    public Color BadgeColorHover { get; set; }
    public double BadgeShadowSize { get; set; }
    public Color BadgeShadowColor { get; set; }
    public TimeSpan BadgeProcessingDuration { get; set; }
    public Point BadgeRibbonOffset { get; set; }
    public Transform? BadgeRibbonCornerTransform { get; set; }
    public int BadgeRibbonCornerDarkenAmount { get; set; }
    public Thickness BadgeRibbonTextPadding { get; set; }
    public Thickness DotBadgeLabelMargin { get; set; }

    #endregion

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        var lineWidth = _globalToken.LineWidth;
        IndicatorHeight   = Math.Round(_globalToken.FontSize * _globalToken.LineHeight) - 2 * lineWidth;
        IndicatorHeightSM = _globalToken.FontSize;
        DotSize           = _globalToken.FontSizeSM / 2;
        TextFontSize      = _globalToken.FontSizeSM;
        TextFontSizeSM    = _globalToken.FontSizeSM;
        TextFontWeight    = FontWeight.Normal;
        StatusSize        = _globalToken.FontSizeSM / 2;

        // 设置内部 token
        BadgeFontHeight         = _globalToken.FontHeight;
        BadgeShadowSize         = lineWidth;
        BadgeTextColor          = _globalToken.ColorBgContainer;
        BadgeColor              = _globalToken.ColorError;
        BadgeColorHover         = _globalToken.ColorErrorHover;
        BadgeShadowColor        = _globalToken.ColorBorderBg;
        BadgeProcessingDuration = TimeSpan.FromMilliseconds(1200);
        BadgeRibbonOffset       = new Point(_globalToken.MarginXS, _globalToken.MarginXS);

        BadgeRibbonCornerTransform    = new ScaleTransform(1, 0.75);
        BadgeRibbonCornerDarkenAmount = 15;
        BadgeRibbonTextPadding        = new Thickness(_globalToken.PaddingXS, 0);
        DotBadgeLabelMargin           = new Thickness(_globalToken.MarginXS, 0, 0, 0);
    }
}