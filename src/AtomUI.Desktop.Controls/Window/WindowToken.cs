using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal sealed class WindowToken : AbstractControlDesignToken
{

    /// <summary>
    /// 窗口默认的背景色
    /// </summary>
    public Color DefaultBackground { get; set; }

    /// <summary>
    /// 窗口默认的前景色
    /// </summary>
    public Color DefaultForeground { get; set; }
    
    /// <summary>
    /// 窗口圆角，后期可能
    /// </summary>
    public CornerRadius CornerRadius { get; set; }
    
    /// <summary>
    /// 标题栏高度
    /// </summary>
    public double TitleBarHeight { get; set; }
    
    public SolidColorBrush? SystemBarColor { get; set; }
    
    /// <summary>
    /// 主窗体阴影
    /// </summary>
    public BoxShadows FrameShadows { get; set; }
    
    /// <summary>
    /// 全屏弹出层的阴影
    /// </summary>
    public BoxShadows FullscreenPopoverShadows { get; set; }
    
    /// <summary>
    /// 全屏弹出层内容内间距
    /// </summary>
    public Thickness FullscreenHeaderFramePadding { get; set; }
    
    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        DefaultBackground        = EffectiveGlobalToken.ColorBgContainer;
        DefaultForeground        = EffectiveGlobalToken.ColorText;
        CornerRadius             = new CornerRadius(12);
        SystemBarColor           = new SolidColorBrush(EffectiveGlobalToken.ColorBgContainer);
        // 窗口装饰语义不属于密度算法作用域：紧凑算法会把 ControlHeightLG / SizeLG 拉小，
        // 连带 TitleBarHeight 和全屏弹层 padding 一起缩，因此使用跨平台稳定值切开 compact 链路。
        TitleBarHeight               = 40;
        FullscreenHeaderFramePadding = new Thickness(24, 0);
        FrameShadows             = EffectiveGlobalToken.BoxShadowsSecondary;
        FullscreenPopoverShadows = new BoxShadows(
            new BoxShadow
            {
                OffsetX = 0, OffsetY = 6, Blur = 16, Spread = 0, Color
                    = Color.FromArgb(20, 0, 0, 0)
            },
            [
                new BoxShadow
                {
                    OffsetX = 0, OffsetY = 3, Blur = 6, Spread = -4,
                    Color   = Color.FromArgb(31, 0, 0, 0)
                },
                new BoxShadow
                {
                    OffsetX = 0, OffsetY = 9, Blur = 28, Spread = 8,
                    Color   = Color.FromArgb(13, 0, 0, 0)
                },
            ]);
    }

}
