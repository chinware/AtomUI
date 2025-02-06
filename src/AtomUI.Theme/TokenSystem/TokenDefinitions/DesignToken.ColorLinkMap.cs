using Avalonia.Media;

namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public partial class DesignToken
{
    /// <summary>
    /// 超链接颜色
    /// 控制超链接的颜色。
    /// 在 Seed 文件中定义
    /// </summary>

    /// <summary>
    /// 超链接悬浮颜色
    /// 控制超链接悬浮时的颜色。
    /// </summary>
    public Color ColorLinkHover { get; set; }

    /// <summary>
    /// 超链接激活颜色
    /// 控制超链接被点击时的颜色。
    /// </summary>
    public Color ColorLinkActive { get; set; }
}