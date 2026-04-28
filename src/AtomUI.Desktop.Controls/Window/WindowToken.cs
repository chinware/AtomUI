using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class WindowToken : AbstractControlDesignToken
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
    
    public WindowToken()
        : base("Window")
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        DefaultBackground        = SharedToken.ColorBgContainer;
        DefaultForeground        = SharedToken.ColorText;
        CornerRadius             = new CornerRadius(12);
        SystemBarColor           = new SolidColorBrush(SharedToken.ColorBgContainer);
        TitleBarHeight           = SharedToken.ControlHeightLG;
        FrameShadows             = SharedToken.BoxShadowsSecondary;
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
        FullscreenHeaderFramePadding = new Thickness(SharedToken.UniformlyPaddingLG, 0);
    }
    
    protected override Type GetTokenKindType() => typeof(WindowTokenKind);
}