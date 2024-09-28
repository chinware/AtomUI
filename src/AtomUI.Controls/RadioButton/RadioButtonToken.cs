using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class RadioButtonToken : AbstractControlDesignToken
{
    public const string ID = "RadioButton";

    /// <summary>
    /// 单选框大小，除去文字部分的
    /// </summary>
    public double RadioSize { get; set; }

    /// <summary>
    /// 单选框圆点大小
    /// </summary>
    public double DotSize { get; set; }

    /// <summary>
    /// 单选框圆点禁用颜色
    /// </summary>
    public Color DotColorDisabled { get; set; }

    /// 内部使用
    public Color RadioColor { get; set; }

    public Color RadioBgColor { get; set; }
    
    public double DotPadding { get; set; }
    
    public Thickness TextMargin { get; set; }

    public RadioButtonToken()
        : base(ID)
    {
    }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        var wireFrame        = _globalToken.Wireframe;
        var lineWidth        = _globalToken.LineWidth;
        var fontSizeLG       = _globalToken.FontSizeLG;
        var colorBgContainer = _globalToken.ColorBgContainer;
        var colorPrimary     = _globalToken.ColorPrimary;
        var colorWhite       = _globalToken.ColorWhite;

        var dotPadding = 4; // 魔术值，需要看有没有好办法消除
        var radioSize  = fontSizeLG;

        var radioDotSize = wireFrame
            ? radioSize - dotPadding * 2
            : radioSize - (dotPadding + lineWidth) * 2;

        DotPadding       = dotPadding;
        RadioSize        = radioSize;
        DotSize          = radioDotSize;
        DotColorDisabled = _globalToken.ColorTextDisabled;

        // internal
        RadioColor   = wireFrame ? colorPrimary : colorWhite;
        RadioBgColor = wireFrame ? colorBgContainer : colorPrimary;
        TextMargin   = new Thickness(_globalToken.MarginXXS, 0, 0, 0);
    }
}