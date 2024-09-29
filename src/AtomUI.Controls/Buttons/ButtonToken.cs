using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class ButtonToken : AbstractControlDesignToken
{
    public const string ID = "Button";

    /// <summary>
    /// 文字字重
    /// </summary>
    public int FontWeight { get; set; }

    /// <summary>
    /// 默认按钮阴影
    /// </summary>
    public BoxShadow DefaultShadow { get; set; }

    /// <summary>
    /// 主要按钮阴影
    /// </summary>
    public BoxShadow PrimaryShadow { get; set; }

    /// <summary>
    /// 危险按钮阴影
    /// </summary>
    public BoxShadow DangerShadow { get; set; }

    /// <summary>
    /// 主要按钮文本颜色
    /// </summary>
    public Color PrimaryColor { get; set; }

    /// <summary>
    /// 默认按钮文本颜色
    /// </summary>
    public Color DefaultColor { get; set; }

    /// <summary>
    /// 默认按钮背景色
    /// </summary>
    public Color DefaultBg { get; set; }

    /// <summary>
    /// 默认按钮边框颜色
    /// </summary>
    public Color DefaultBorderColor { get; set; }

    /// <summary>
    /// 默认的禁用边框颜色
    /// </summary>
    public Color DefaultBorderColorDisabled { get; set; }

    /// <summary>
    /// 危险按钮文本颜色
    /// </summary>
    public Color DangerColor { get; set; }

    /// <summary>
    /// 默认按钮悬浮态背景色
    /// </summary>
    public Color DefaultHoverBg { get; set; }

    /// <summary>
    /// 默认按钮悬浮态文本颜色
    /// </summary>
    public Color DefaultHoverColor { get; set; }

    /// <summary>
    /// 默认按钮悬浮态边框颜色
    /// </summary>
    public Color DefaultHoverBorderColor { get; set; }

    /// <summary>
    /// 默认按钮激活态背景色
    /// </summary>
    public Color DefaultActiveBg { get; set; }

    /// <summary>
    /// 默认按钮激活态文字颜色
    /// </summary>
    public Color DefaultActiveColor { get; set; }

    /// <summary>
    /// 默认按钮激活态边框颜色
    /// </summary>
    public Color DefaultActiveBorderColor { get; set; }

    /// <summary>
    /// 禁用状态边框颜色
    /// </summary>
    public Color BorderColorDisabled { get; set; }

    /// <summary>
    /// 默认幽灵按钮文本颜色
    /// </summary>
    public Color DefaultGhostColor { get; set; }

    /// <summary>
    /// 幽灵按钮背景色
    /// </summary>
    public Color GhostBg { get; set; }

    /// <summary>
    /// 默认幽灵按钮边框颜色
    /// </summary>
    public Color DefaultGhostBorderColor { get; set; }

    /// <summary>
    /// 按钮内间距
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// 大号按钮内间距
    /// </summary>
    public Thickness PaddingLG { get; set; }

    /// <summary>
    /// 小号按钮内间距
    /// </summary>
    public Thickness PaddingSM { get; set; }

    /// <summary>
    /// 按钮右边一个额外的区域对右侧的小号外边距
    /// </summary>
    public Thickness ExtraContentMarginSM { get; set; }

    /// <summary>
    /// 按钮右边一个额外的区域对右侧的外边距
    /// </summary>
    public Thickness ExtraContentMargin { get; set; }

    /// <summary>
    /// 按钮右边一个额外的区域对右侧的大号外边距
    /// </summary>
    public Thickness ExtraContentMarginLG { get; set; }

    /// <summary>
    /// 圆形按钮内间距
    /// </summary>
    public Thickness CirclePadding { get; set; }

    /// <summary>
    /// 只有图标的按钮图标尺寸
    /// </summary>
    public double OnlyIconSize { get; set; }

    /// <summary>
    /// 大号只有图标的按钮图标尺寸
    /// </summary>
    public double OnlyIconSizeLG { get; set; }

    /// <summary>
    /// 小号只有图标的按钮图标尺寸
    /// </summary>
    public double OnlyIconSizeSM { get; set; }

    /// <summary>
    /// 图标的按钮图标尺寸
    /// </summary>
    public double IconSize { get; set; }

    /// <summary>
    /// 只有图标的按钮图标尺寸
    /// </summary>
    public double IconSizeLG { get; set; }

    /// <summary>
    /// 只有图标的按钮图标尺寸
    /// </summary>
    public double IconSizeSM { get; set; }

    /// <summary>
    /// 完成 Icon 外边距
    /// </summary>
    public Thickness IconMargin { get; set; }

    /// <summary>
    /// 按钮组边框颜色
    /// </summary>
    public Color GroupBorderColor { get; set; }

    /// <summary>
    /// 链接按钮悬浮态背景色
    /// </summary>
    public Color LinkHoverBg { get; set; }

    /// <summary>
    /// 文本按钮悬浮态背景色
    /// </summary>
    public Color TextHoverBg { get; set; }

    /// <summary>
    /// 按钮内容字体大小
    /// </summary>
    public double ContentFontSize { get; set; } = double.NaN;

    /// <summary>
    /// 大号按钮内容字体大小
    /// </summary>
    public double ContentFontSizeLG { get; set; } = double.NaN;

    /// <summary>
    /// 小号按钮内容字体大小
    /// </summary>
    public double ContentFontSizeSM { get; set; } = double.NaN;

    /// <summary>
    /// 按钮内容字体行高
    /// </summary>
    public double ContentLineHeight { get; set; } = double.NaN;

    /// <summary>
    /// 大号按钮内容字体行高
    /// </summary>
    public double ContentLineHeightLG { get; set; } = double.NaN;

    /// <summary>
    /// 小号按钮内容字体行高
    /// </summary>
    public double ContentLineHeightSM { get; set; } = double.NaN;

    #region 内部 Token 定义

    /// <summary>
    /// IconOnly 按钮内间距
    /// </summary>
    public Thickness IconOnyPadding { get; set; }

    /// <summary>
    /// IconOnly 大号按钮内间距
    /// </summary>
    public Thickness IconOnyPaddingLG { get; set; }

    /// <summary>
    /// IconOnly 小号按钮内间距
    /// </summary>
    public Thickness IconOnyPaddingSM { get; set; }

    #endregion

    public ButtonToken()
        : base(ID)
    {
    }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        var fontSize   = _globalToken.FontSize;
        var fontSizeLG = _globalToken.FontSizeLG;

        ContentFontSize   = !double.IsNaN(ContentFontSize) ? ContentFontSize : fontSize;
        ContentFontSizeSM = !double.IsNaN(ContentFontSizeSM) ? ContentFontSizeSM : fontSize;
        ContentFontSizeLG = !double.IsNaN(ContentFontSizeLG) ? ContentFontSizeLG : fontSizeLG;
        ContentLineHeight = !double.IsNaN(ContentLineHeight)
            ? ContentLineHeight
            : CalculatorUtils.CalculateLineHeight(ContentFontSize) * ContentFontSize;
        ContentLineHeightSM = !double.IsNaN(ContentLineHeightSM)
            ? ContentLineHeightSM
            : CalculatorUtils.CalculateLineHeight(ContentFontSizeSM) * ContentFontSizeSM;
        ContentLineHeightLG = !double.IsNaN(ContentLineHeightLG)
            ? ContentLineHeightLG
            : CalculatorUtils.CalculateLineHeight(ContentFontSizeLG) * ContentFontSizeLG;

        var controlOutlineWidth = _globalToken.ControlOutlineWidth;
        FontWeight = 400;
        DefaultShadow = new BoxShadow
        {
            OffsetX = 0,
            OffsetY = controlOutlineWidth / 2,
            Blur    = 3,
            Spread  = 0,
            Color   = _globalToken.ColorControlOutline
        };

        PrimaryShadow = new BoxShadow
        {
            OffsetX = 0,
            OffsetY = controlOutlineWidth,
            Blur    = 3,
            Spread  = 0,
            Color   = _globalToken.ColorControlOutline
        };

        DangerShadow = new BoxShadow
        {
            OffsetX = 0,
            OffsetY = controlOutlineWidth,
            Blur    = 3,
            Spread  = 0,
            Color   = _globalToken.ColorErrorOutline
        };
        
        var lineWidth         = _globalToken.LineWidth;

        PrimaryColor            = _globalToken.ColorTextLightSolid;
        DangerColor             = _globalToken.ColorTextLightSolid;
        BorderColorDisabled     = _globalToken.ColorBorder;
        DefaultGhostColor       = _globalToken.ColorBgContainer;
        GhostBg                 = Colors.Transparent;
        DefaultGhostBorderColor = _globalToken.ColorBgContainer;

        GroupBorderColor           = _globalToken.ColorPrimaryHover;
        LinkHoverBg                = Colors.Transparent;
        TextHoverBg                = _globalToken.ColorBgTextHover;
        DefaultColor               = _globalToken.ColorText;
        DefaultBg                  = _globalToken.ColorBgContainer;
        DefaultBorderColor         = _globalToken.ColorBorder;
        DefaultBorderColorDisabled = _globalToken.ColorBorder;
        DefaultHoverBg             = _globalToken.ColorBgContainer;
        DefaultHoverColor          = _globalToken.ColorPrimaryHover;
        DefaultHoverBorderColor    = _globalToken.ColorPrimaryHover;
        DefaultActiveBg            = _globalToken.ColorBgContainer;
        DefaultActiveColor         = _globalToken.ColorPrimaryActive;
        DefaultActiveBorderColor   = _globalToken.ColorPrimaryActive;

        var controlHeightSM = _globalToken.ControlHeightSM;
        var controlHeight   = _globalToken.ControlHeight;
        var controlHeightLG = _globalToken.ControlHeightLG;

        PaddingSM = new Thickness(8 - _globalToken.LineWidth,
            Math.Max((controlHeightSM - ContentLineHeightSM) / 2 - lineWidth, 0));
        Padding = new Thickness(_globalToken.PaddingContentHorizontal - lineWidth,
            Math.Max((controlHeight - ContentLineHeight) / 2 - lineWidth, 0));
        PaddingLG = new Thickness(_globalToken.PaddingContentHorizontal - lineWidth,
            Math.Max((controlHeightLG - ContentLineHeightLG) / 2 - lineWidth, 0));

        ExtraContentMarginSM = new Thickness(PaddingSM.Left / 2, 0, 0, 0);
        ExtraContentMargin   = new Thickness(Padding.Left / 2, 0, 0, 0);
        ExtraContentMarginLG = new Thickness(PaddingLG.Left / 2, 0, 0, 0);

        CirclePadding  = new Thickness(PaddingSM.Left / 2);
        OnlyIconSizeSM = _globalToken.IconSize;
        OnlyIconSize   = _globalToken.IconSizeLG;
        OnlyIconSizeLG = _globalToken.IconSizeLG;

        IconSizeSM = _globalToken.IconSizeSM;
        IconSize   = _globalToken.IconSize;
        IconSizeLG = _globalToken.IconSize;

        IconMargin = new Thickness(0, 0, _globalToken.PaddingXXS, 0);

        IconOnyPadding   = new Thickness(Math.Max((controlHeight - ContentLineHeight) / 2 - lineWidth, 0));
        IconOnyPaddingLG = new Thickness(Math.Max((controlHeightLG - ContentLineHeightLG) / 2 - lineWidth, 0));
        IconOnyPaddingSM = new Thickness(Math.Max((controlHeightSM - ContentLineHeightSM) / 2 - lineWidth, 0));
    }
}