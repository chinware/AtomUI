using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class ScrollBarToken : AbstractControlDesignToken
{
    public const string ID = "ScrollBar";

    /// <summary>
    /// 滚动条滑块的粗细
    /// </summary>
    public double ThumbThickness { get; set; }

    /// <summary>
    /// 滚动条的粗细
    /// </summary>
    public double ScrollBarThickness { get; set; }

    /// <summary>
    /// 滚动条滑块背景颜色
    /// </summary>
    public Color ThumbBg { get; set; }

    /// <summary>
    /// 滚动条滑块鼠标 hover 背景颜色
    /// </summary>
    public Color ThumbHoverBg { get; set; }
    
    /// <summary>
    /// 滚动条滑块的圆角大小
    /// </summary>
    public CornerRadius ThumbCornerRadius { get; set; }
    
    #region 内部 Token 定义
    /// <summary>
    /// 水平滚动条内间距
    /// </summary>
    public Thickness ContentHPadding { get; set; }
    
    /// <summary>
    /// 垂直滚动条内间距
    /// </summary>
    public Thickness ContentVPadding { get; set; }
    #endregion

    public ScrollBarToken()
        : base(ID)
    {
    }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        ThumbBg            = _globalToken.ColorBorderSecondary;
        ThumbHoverBg       = _globalToken.ColorBorder;
        ThumbThickness     = _globalToken.SizeXS;
        ScrollBarThickness = _globalToken.SizeSM;
        ThumbCornerRadius  = new CornerRadius(_globalToken.SizeXS / 2.0);
        ContentHPadding    = new Thickness(_globalToken.PaddingXXS, 0d);
        ContentVPadding    = new Thickness(0d, _globalToken.PaddingXXS);
    }
}