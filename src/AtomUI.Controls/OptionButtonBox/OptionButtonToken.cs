using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
public class OptionButtonToken : AbstractControlDesignToken
{
    public const string ID = "OptionButton";

    /// <summary>
    /// 单选框按钮背景色
    /// </summary>
    public Color ButtonBackground { get; set; }

    /// <summary>
    /// 单选框按钮选中背景色
    /// </summary>
    public Color ButtonCheckedBackground { get; set; }

    /// <summary>
    /// 单选框按钮文本颜色
    /// </summary>
    public Color ButtonColor { get; set; }

    /// <summary>
    /// 单选框按钮内间距
    /// </summary>
    public Thickness ButtonPadding { get; set; }

    /// <summary>
    /// 单选框按钮选中并禁用时的背景色
    /// </summary>
    public Color ButtonCheckedBgDisabled { get; set; }

    /// <summary>
    /// 单选框按钮选中并禁用时的文本颜色
    /// </summary>
    public Color ButtonCheckedColorDisabled { get; set; }

    /// <summary>
    /// 单选框实色按钮选中时的文本颜色
    /// </summary>
    public Color ButtonSolidCheckedColor { get; set; }

    /// <summary>
    /// 单选框实色按钮选中时的背景色
    /// </summary>
    public Color ButtonSolidCheckedBackground { get; set; }

    /// <summary>
    /// 单选框实色按钮选中时的悬浮态背景色
    /// </summary>
    public Color ButtonSolidCheckedHoverBackground { get; set; }

    /// <summary>
    /// 单选框实色按钮选中时的激活态背景色
    /// </summary>
    public Color ButtonSolidCheckedActiveBackground { get; set; }

    /// <summary>
    /// 按钮内容字体大小
    /// </summary>
    public double ContentFontSize { get; set; } = -1;

    /// <summary>
    /// 大号按钮内容字体大小
    /// </summary>
    public double ContentFontSizeLG { get; set; } = -1;

    /// <summary>
    /// 小号按钮内容字体大小
    /// </summary>
    public double ContentFontSizeSM { get; set; } = -1;

    /// <summary>
    /// 按钮内容字体行高
    /// </summary>
    public double ContentLineHeight { get; set; } = -1;

    /// <summary>
    /// 大号按钮内容字体行高
    /// </summary>
    public double ContentLineHeightLG { get; set; } = -1;

    /// <summary>
    /// 小号按钮内容字体行高
    /// </summary>
    public double ContentLineHeightSM { get; set; } = -1;

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

    public OptionButtonToken()
        : base(ID)
    {
    }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        ButtonSolidCheckedColor            = _globalToken.ColorTextLightSolid;
        ButtonSolidCheckedBackground       = _globalToken.ColorPrimary;
        ButtonSolidCheckedHoverBackground  = _globalToken.ColorPrimaryHover;
        ButtonSolidCheckedActiveBackground = _globalToken.ColorPrimaryActive;
        ButtonBackground                   = _globalToken.ColorBgContainer;
        ButtonCheckedBackground            = _globalToken.ColorBgContainer;
        ButtonColor                        = _globalToken.ColorText;
        ButtonCheckedBgDisabled            = _globalToken.ControlItemBgActiveDisabled;
        ButtonCheckedColorDisabled         = _globalToken.ColorTextDisabled;
        ButtonPadding                      = new Thickness(_globalToken.Padding, 0);

        var fontSize   = _globalToken.FontSize;
        var fontSizeLG = _globalToken.FontSizeLG;

        ContentFontSize   = !MathUtils.AreClose(ContentFontSize, -1) ? ContentFontSize : fontSize;
        ContentFontSizeSM = !MathUtils.AreClose(ContentFontSizeSM, -1) ? ContentFontSizeSM : fontSize;
        ContentFontSizeLG = !MathUtils.AreClose(ContentFontSizeLG, -1) ? ContentFontSizeLG : fontSizeLG;
        ContentLineHeight = !MathUtils.AreClose(ContentLineHeight, -1)
            ? ContentLineHeight
            : CalculatorUtils.CalculateLineHeight(ContentFontSize);
        ContentLineHeightSM = !MathUtils.AreClose(ContentLineHeightSM, -1)
            ? ContentLineHeightSM
            : CalculatorUtils.CalculateLineHeight(ContentFontSizeSM);
        ContentLineHeightLG = !MathUtils.AreClose(ContentLineHeightLG, -1)
            ? ContentLineHeightLG
            : CalculatorUtils.CalculateLineHeight(ContentFontSizeLG);

        var controlHeight   = _globalToken.ControlHeight;
        var controlHeightSM = _globalToken.ControlHeightSM;
        var controlHeightLG = _globalToken.ControlHeightLG;
        var lineWidth       = _globalToken.LineWidth;

        Padding = new Thickness(_globalToken.PaddingContentHorizontal - lineWidth,
            Math.Max((controlHeight - ContentFontSize * ContentLineHeight) / 2 - lineWidth, 0));
        PaddingLG = new Thickness(_globalToken.PaddingContentHorizontal - lineWidth,
            Math.Max((controlHeightSM - ContentFontSizeSM * ContentLineHeightSM) / 2 - lineWidth, 0));
        PaddingSM = new Thickness(8 - _globalToken.LineWidth,
            Math.Max((controlHeightLG - controlHeightLG * controlHeightLG) / 2 - lineWidth, 0));
    }
}