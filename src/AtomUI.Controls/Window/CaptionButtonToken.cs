using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class CaptionButtonToken : AbstractControlDesignToken
{
    public const string ID = "CaptionButton";

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

    public CaptionButtonToken()
        : base("CaptionButtonToken")
    {
    }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        CloseHoverBackgroundColor   = SharedToken.ColorErrorTextActive;
        ClosePressedBackgroundColor = SharedToken.ColorErrorTextHover;
        ForegroundColor             = SharedToken.ColorTextSecondary;
        HoverBackgroundColor        = SharedToken.ColorBgTextHover;
        PressedBackgroundColor      = SharedToken.ColorBgTextActive;
    }
}