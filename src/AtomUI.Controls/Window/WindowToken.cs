using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
public class WindowToken : AbstractControlDesignToken
{
    public const string ID = "Window";

    /// <summary>
    /// 窗口默认的背景色
    /// </summary>
    public Color DefaultBackground { get; set; }

    /// <summary>
    /// 窗口默认的前景色
    /// </summary>
    public Color DefaultForeground { get; set; }
    
    public SolidColorBrush? SystemBarColor { get; set; }

    public WindowToken()
        : base("Window")
    {
    }

    protected internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        DefaultBackground = SharedToken.ColorBgContainer;
        DefaultForeground = SharedToken.ColorText;
        SystemBarColor    = new SolidColorBrush(SharedToken.ColorBgContainer);
    }
}