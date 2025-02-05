using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class ScrollBarToken : AbstractControlDesignToken
{
    public const string ID = "ScrollBar";
 
    /// <summary>
    /// 滚动条的粗细
    /// </summary>
    public double ThumbThickness { get; set; }
    
    /// <summary>
    /// 滚动条滑块背景颜色
    /// </summary>
    public Color ThumbBg { get; set; }
    
    /// <summary>
    /// 滚动条滑块鼠标 hover 背景颜色
    /// </summary>
    public Color ThumbHoverBg { get; set; }

    /// <summary>
    /// 滚动条滑块激活状态的透明度
    /// </summary>
    public double ThumbActiveOpacity { get; set; } = 1.0;
    
    public ScrollBarToken()
        : base(ID)
    {
    }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        ThumbBg        = _globalToken.ColorBorderSecondary;
        ThumbHoverBg   = _globalToken.ColorBorder;
        ThumbThickness = _globalToken.SizeXS;
    }
}