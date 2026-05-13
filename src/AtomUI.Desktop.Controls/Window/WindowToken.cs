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
        // 窗口装饰语义不属于密度算法作用域：紧凑算法会把 ControlHeightLG / SizeLG 拉小，
        // 连带 TitleBarHeight 和全屏弹层 padding 一起缩——这里按平台分支写死，切开与
        // compact 的链路。当前三平台同值，保留分支便于后续按平台独立调整。
        TitleBarHeight               = GetPlatformTitleBarHeight();
        FullscreenHeaderFramePadding = GetPlatformFullscreenHeaderPadding();
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
    }

    private static double GetPlatformTitleBarHeight()
    {
        if (OperatingSystem.IsWindows())
        {
            return 40;
        }
        if (OperatingSystem.IsMacOS())
        {
            return 40;
        }
        // Linux / 其他
        return 40;
    }

    private static Thickness GetPlatformFullscreenHeaderPadding()
    {
        if (OperatingSystem.IsWindows())
        {
            return new Thickness(24, 0);
        }
        if (OperatingSystem.IsMacOS())
        {
            return new Thickness(24, 0);
        }
        // Linux / 其他
        return new Thickness(24, 0);
    }
    
    protected override Type GetTokenKindType() => typeof(WindowTokenKind);
}