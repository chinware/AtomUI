using Avalonia.Media;

namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public partial class GlobalToken
{
    /// <summary>
    /// 信息色的浅色背景颜色
    /// 1 号色
    /// </summary>
    public Color ColorInfoBg { get; set; }

    /// <summary>
    /// 信息色的浅色背景色悬浮态
    /// 2 号色
    /// </summary>
    public Color ColorInfoBgHover { get; set; }

    /// <summary>
    /// 信息色的描边色
    /// 3 号色
    /// </summary>
    public Color ColorInfoBorder { get; set; }

    /// <summary>
    /// 信息色的描边色悬浮态
    /// 4 号色
    /// </summary>
    public Color ColorInfoBorderHover { get; set; }

    /// <summary>
    /// 信息色的深色悬浮态
    /// 5 号色
    /// </summary>
    public Color ColorInfoHover { get; set; }

    /// <summary>
    /// 信息色
    /// 6 号色
    /// 在 Seed 文件中定义
    /// </summary>

    /// <summary>
    /// 信息色的深色激活态
    /// 7 号色
    /// </summary>
    public Color ColorInfoActive { get; set; }

    /// <summary>
    /// 信息色的文本悬浮态
    /// 8 号色
    /// </summary>
    public Color ColorInfoTextHover { get; set; }

    /// <summary>
    /// 信息色的文本默认态
    /// 9 号色
    /// </summary>
    public Color ColorInfoText { get; set; }

    /// <summary>
    /// 信息色的文本激活态
    /// 10 号色
    /// </summary>
    public Color ColorInfoTextActive { get; set; }
}