using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls.Primitives;

[ControlDesignToken]
internal class IndicatorScrollViewerToken : AbstractControlDesignToken
{
    public const string ID = "IndicatorScrollViewer";
    
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
    /// 滚动条滑块的圆角大小
    /// </summary>
    public CornerRadius ThumbCornerRadius { get; set; }
    
    public IndicatorScrollViewerToken()
        : base(ID)
    {
    }

    public override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        ThumbBg            = SharedToken.ColorBorder;
        ThumbThickness     = SharedToken.SizeXS;
        ScrollBarThickness = SharedToken.LineWidthBold;
        ThumbCornerRadius  = new CornerRadius(SharedToken.LineWidthBold / 2.0);
    }
}