using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Media;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class WindowTitleBarToken : AbstractControlDesignToken
{
    public const string ID = "WindowTitleBar";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    
    /// <summary>
    /// Hover 的背景色
    /// </summary>
    public Color HoverBackgroundColor { get; set; }

    /// <summary>
    /// 鼠标按下的背景色
    /// </summary>
    public Color PressedBackgroundColor { get; set; }

    /// <summary>
    /// 关闭按钮的背景颜色
    /// </summary>
    public Color CloseHoverBackgroundColor { get; set; }

    /// <summary>
    /// 关闭按钮鼠标按下的背景颜色
    /// </summary>
    public Color ClosePressedBackgroundColor { get; set; }

    /// <summary>
    /// 按钮的前景色
    /// </summary>
    public Color ForegroundColor { get; set; }
    
    /// <summary>
    /// 应用程序 Logo 和 标题之间的间距
    /// </summary>
    public double LogoAndTitleSpacing { get; set; }
    
    /// <summary>
    /// 应用程序标题栏
    /// </summary>
    public Thickness TitleBarPadding { get; set; }
    
    /// <summary>
    /// 窗口标题默认字体
    /// </summary>
    public double TitleFontSize { get; set; }
    
    /// <summary>
    /// 窗口标题默认字体粗细
    /// </summary>
    public FontWeight TitleFontWeight { get; set; }
    
    /// <summary>
    /// 标题按钮的大小
    /// </summary>
    public double CaptionButtonIconSize {  get; set; }
    
    /// <summary>
    /// 应用程序图标大小
    /// </summary>
    public double LogoSize { get; set; }
    
    /// <summary>
    /// 窗口激活状态下的颜色
    /// </summary>
    public Color ActiveColor { get; set; }
    
    /// <summary>
    /// 窗口未激活状态下的颜色
    /// </summary>
    public Color InactiveColor { get; set; }
    
    /// <summary>
    /// 窗口激活状态下的颜色
    /// </summary>
    public Color ActiveBgColor { get; set; }
    
    /// <summary>
    /// 窗口激活状态下鼠标划过的颜色
    /// </summary>
    public Color ActiveHoverBgColor { get; set; }
    
    /// <summary>
    /// 窗口激活状态下鼠标按下的颜色
    /// </summary>
    public Color ActivePressedBgColor { get; set; }
    
    /// <summary>
    /// Windows 窗口关闭按钮鼠标 hover 颜色
    /// </summary>
    public Color WindowsCloseButtonHoverColor { get; set; }
    
    /// <summary>
    /// Windows 窗口关闭按钮鼠标 hover 背景色
    /// </summary>
    public Color WindowsCloseButtonHoverBgColor { get; set; }
    
    /// <summary>
    /// Windows 窗口关闭按钮鼠标按下背景色
    /// </summary>
    public Color WindowsCloseButtonPressedBgColor { get; set; }
    
    /// <summary>
    /// 窗口未激活状态下的颜色
    /// </summary>
    public Color InactiveBgColor { get; set; }
    
    /// <summary>
    /// 窗口未激活状态下鼠标划过的颜色
    /// </summary>
    public Color InactiveHoverBgColor { get; set; }
    
    /// <summary>
    /// 标题按钮的内间距
    /// </summary>
    public Thickness CaptionButtonPadding { get; set; }
    
    /// <summary>
    /// 标题按钮间距
    /// </summary>
    public double CaptionGroupSpacing { get; set; }
    
    /// <summary>
    /// 标题高度
    /// </summary>
    public double Height { get; set; }

    /// <summary>
    /// 全屏状态下 Caption 按钮的尺寸（与密度算法解耦，取代直引 SharedToken.SizeLG）
    /// </summary>
    public double FullscreenCaptionButtonSize { get; set; }

    /// <summary>
    /// 标题栏水平布局分隔（Linux 模板 DockPanel 用，取代直引 SharedToken.SpacingXS）
    /// </summary>
    public double HeaderHorizontalSpacing { get; set; }
    
    public WindowTitleBarToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        CloseHoverBackgroundColor   = SharedToken.ColorErrorTextActive;
        ClosePressedBackgroundColor = SharedToken.ColorErrorTextHover;
        ForegroundColor             = SharedToken.ColorTextSecondary;
        HoverBackgroundColor        = SharedToken.ColorBgTextHover;
        PressedBackgroundColor      = SharedToken.ColorBgTextActive;
        LogoAndTitleSpacing         = SharedToken.SizeUnit * 2;
        TitleBarPadding             = new Thickness(LogoAndTitleSpacing * 1.8, LogoAndTitleSpacing);
        CaptionButtonIconSize       = SharedToken.IconSizeSM;
        LogoSize                    = SharedToken.SizeUnit * 4;
        
        ActiveColor   = SharedToken.ColorTextSecondary;
        InactiveColor = SharedToken.ColorTextQuaternary;

        ActiveBgColor        = SharedToken.ColorFillTertiary;
        ActiveHoverBgColor   = SharedToken.ColorFillSecondary;
        ActivePressedBgColor = SharedToken.ColorFill;

        InactiveBgColor      = SharedToken.ColorFillQuaternary;
        InactiveHoverBgColor = SharedToken.ColorFillTertiary;
        
        CaptionButtonPadding = new Thickness(SharedToken.SizeUnit * 2);
        CaptionGroupSpacing  = SharedToken.SizeUnit * 2;

        // 窗口装饰语义不走密度算法：紧凑模式下 ControlHeightLG / SizeLG / SpacingXS 都会被拉小，
        // 标题栏 / 全屏按钮 / DockPanel 间距不应跟着缩。按平台分支写死，三平台当前同值。
        Height                      = GetPlatformTitleBarHeight();
        FullscreenCaptionButtonSize = GetPlatformFullscreenCaptionButtonSize();
        HeaderHorizontalSpacing     = GetPlatformHeaderHorizontalSpacing();

        WindowsCloseButtonHoverBgColor   = Color.FromRgb(244, 67, 54); // #F44336
        WindowsCloseButtonPressedBgColor = Color.FromArgb(190, 244, 67, 54);
        WindowsCloseButtonHoverColor     = ColorUtils.OnBackground(Colors.White, WindowsCloseButtonHoverBgColor);

        TitleFontSize = 13;
        TitleFontWeight = FontWeight.Bold;
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

    private static double GetPlatformFullscreenCaptionButtonSize()
    {
        if (OperatingSystem.IsWindows())
        {
            return 24;
        }
        if (OperatingSystem.IsMacOS())
        {
            return 24;
        }
        // Linux / 其他
        return 24;
    }

    private static double GetPlatformHeaderHorizontalSpacing()
    {
        if (OperatingSystem.IsWindows())
        {
            return 8;
        }
        if (OperatingSystem.IsMacOS())
        {
            return 8;
        }
        // Linux / 其他
        return 8;
    }
    
    protected override Type GetTokenKindType() => typeof(WindowTitleBarTokenKind);
}