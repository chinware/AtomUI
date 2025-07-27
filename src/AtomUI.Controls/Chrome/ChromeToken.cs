using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class ChromeToken : AbstractControlDesignToken
{
    public const string ID = "Chrome";
    
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
    /// 标题按钮的大小
    /// </summary>
    public double CaptionButtonSize {  get; set; }
    
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
    /// 窗口未激活状态下的颜色
    /// </summary>
    public Color InactiveBgColor { get; set; }
    
    /// <summary>
    /// 窗口未激活状态下鼠标划过的颜色
    /// </summary>
    public Color InactiveHoverBgColor { get; set; }
    
    public ChromeToken()
        : base(ID)
    {
    }

    protected internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        CloseHoverBackgroundColor   = SharedToken.ColorErrorTextActive;
        ClosePressedBackgroundColor = SharedToken.ColorErrorTextHover;
        ForegroundColor             = SharedToken.ColorTextSecondary;
        HoverBackgroundColor        = SharedToken.ColorBgTextHover;
        PressedBackgroundColor      = SharedToken.ColorBgTextActive;
        LogoAndTitleSpacing         = SharedToken.SizeUnit * 3;
        TitleBarPadding             = SharedToken.PaddingXS;
        CaptionButtonSize           = SharedToken.IconSizeSM;

        ActiveColor   = SharedToken.ColorText;
        InactiveColor = SharedToken.ColorTextQuaternary;

        ActiveBgColor      = SharedToken.ColorFillTertiary;
        ActiveHoverBgColor = SharedToken.ColorFillSecondary;
        ActivePressedBgColor = SharedToken.ColorFill;

        InactiveBgColor      = SharedToken.ColorFillQuaternary;
        InactiveHoverBgColor = SharedToken.ColorFillTertiary;
    }
}