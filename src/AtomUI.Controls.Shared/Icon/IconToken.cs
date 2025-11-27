using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class IconToken : AbstractControlDesignToken
{
    public const string ID = "Icon";
    
    /// <summary>
    /// 次要的描边颜色
    /// </summary>
    public Color SecondaryStrokeColor { get; set; }
    
    /// <summary>
    /// 次要的填充颜色
    /// </summary>
    public Color SecondaryFillColor { get; set; }
    
    /// <summary>
    /// 备用颜色
    /// </summary>
    public Color FallbackColor { get; set; }
    
    /// <summary>
    /// 默认的描边笔宽度
    /// </summary>
    public double StrokeWidth { get; set; }
    
    /// <summary>
    /// 默认的描边的 LineCap
    /// </summary>
    public PenLineCap StrokeLineCap { get; set; }
    
    /// <summary>
    /// 默认的描边的 LineJoin
    /// </summary>
    public PenLineJoin StrokeLineJoin { get; set; }
    
    public IconToken()
        : base(ID)
    {
        SecondaryStrokeColor = Colors.White;
        SecondaryFillColor   = Color.Parse("#43CCF8");
        FallbackColor        = Colors.White;
        StrokeWidth          = 4;
        StrokeLineCap        = PenLineCap.Round;
        StrokeLineJoin       = PenLineJoin.Round;
    }
}