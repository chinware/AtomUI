using Avalonia.Media;

namespace AtomUI.Theme.TokenSystem;

[GlobalDesignToken]
public partial class DesignToken
{
    /// <summary>
    /// 错误色的浅色背景颜色
    /// 1 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorErrorBg { get; set; }

    /// <summary>
    /// 错误色的浅色背景色悬浮态
    /// 2 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorErrorBgHover { get; set; }

    /// <summary>
    /// 错误色的浅色背景色激活态
    /// 3 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorErrorBgActive { get; set; }

    /// <summary>
    /// 错误色的描边色
    /// 3 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorErrorBorder { get; set; }

    /// <summary>
    /// 错误色的描边色悬浮态
    /// 4 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorErrorBorderHover { get; set; }

    /// <summary>
    /// 错误色的深色悬浮态
    /// 5 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorErrorHover { get; set; }

    /// <summary>
    /// 错误色
    /// 6 号色
    /// 在 Seed 文件中定义
    /// </summary>

    /// <summary>
    /// 错误色的深色激活态
    /// 7 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorErrorActive { get; set; }

    /// <summary>
    /// 错误色的文本悬浮态
    /// 8 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorErrorTextHover { get; set; }

    /// <summary>
    /// 错误色的文本默认态
    /// 9 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorErrorText { get; set; }

    /// <summary>
    /// 错误色的文本激活态
    /// 10 号色
    /// </summary>
    [DesignTokenKind(DesignTokenKind.Map)]
    public Color ColorErrorTextActive { get; set; }
}