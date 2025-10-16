using Avalonia.Media;

namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public partial class DesignToken
{
    /// <summary>
    /// 警戒色的浅色背景颜色
    /// 1 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorWarningBg { get; set; }

    /// <summary>
    /// 警戒色的浅色背景色悬浮态
    /// 2 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorWarningBgHover { get; set; }

    /// <summary>
    /// 警戒色的描边色
    /// 3 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorWarningBorder { get; set; }

    /// <summary>
    /// 警戒色的描边色悬浮态
    /// 4 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorWarningBorderHover { get; set; }

    /// <summary>
    /// 警戒色的深色悬浮态
    /// 5 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorWarningHover { get; set; }

    /// <summary>
    /// 警戒色
    /// 最常用的警戒色，例如 Notification、 Alert等警告类组件或 Input 输入类等组件会使用该颜色
    /// 6 号色
    /// 在 Seed 文件中定义
    /// </summary>

    /// <summary>
    /// 警戒色的深色激活态
    /// 7 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorWarningActive { get; set; }

    /// <summary>
    /// 警戒色的文本悬浮态
    /// 8 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorWarningTextHover { get; set; }

    /// <summary>
    /// 警戒色的文本默认态
    /// 9 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorWarningText { get; set; }

    /// <summary>
    /// 警戒色的文本激活态
    /// 10 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorWarningTextActive { get; set; }
}