using Avalonia.Media;

namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public partial class GlobalToken
{
    /// <summary>
    /// 纯白色
    /// 不随主题变化的纯白色
    /// </summary>
    public Color ColorWhite { get; set; } = Color.FromRgb(255, 255, 255);

    /// <summary>
    /// 浮层的背景蒙层颜色
    /// 浮层的背景蒙层颜色，用于遮罩浮层下面的内容，Modal、Drawer 等组件的蒙层使用的是该 token
    /// </summary>
    public Color ColorBgMask { get; set; }

    /// <summary>
    /// 纯黑色
    /// 不随主题变化的纯黑色
    /// </summary>
    public Color ColorBlack { get; set; } = Color.FromRgb(0, 0, 0);

    /// <summary>
    /// 选择背景色
    /// </summary>
    public Color SelectionBackground { get; set; }

    /// <summary>
    /// 选择前景色
    /// </summary>
    public Color SelectionForeground { get; set; }
}