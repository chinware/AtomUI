using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class TreeViewToken : AbstractControlDesignToken
{
    public const string ID = "TreeView";

    public TreeViewToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 节点标题高度
    /// </summary>
    public double TitleHeight { get; set; }

    /// <summary>
    /// 节点悬浮态背景色
    /// </summary>
    public Color NodeHoverBg { get; set; }

    /// <summary>
    /// 节点选中态背景色
    /// </summary>
    public Color NodeSelectedBg { get; set; }

    #region 内部 Token 定义

    /// <summary>
    /// 目录树节点选中文字颜色
    /// </summary>
    public Color DirectoryNodeSelectedColor { get; set; }

    /// <summary>
    /// 目录树节点选中背景色
    /// </summary>
    public Color DirectoryNodeSelectedBg { get; set; }

    /// <summary>
    /// 树节点的外边距
    /// </summary>
    public Thickness TreeItemMargin { get; set; }

    /// <summary>
    /// 树节点标题的内间距
    /// </summary>
    public Thickness TreeItemHeaderPadding { get; set; }

    /// <summary>
    /// 树节点展开按钮外边距
    /// </summary>
    public Thickness TreeNodeSwitcherMargin { get; set; }

    /// <summary>
    /// 树节点 Icon 边距
    /// </summary>
    public Thickness TreeNodeIconMargin { get; set; }

    /// <summary>
    /// 拖动指示器
    /// </summary>
    public double DragIndicatorLineWidth { get; set; }

    #endregion

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        TitleHeight    = _globalToken.ControlHeightSM;
        NodeHoverBg    = _globalToken.ControlItemBgHover;
        NodeSelectedBg = _globalToken.ControlItemBgActive;

        DirectoryNodeSelectedColor = _globalToken.ColorTextLightSolid;
        DirectoryNodeSelectedBg    = _globalToken.ColorPrimary;

        TreeItemMargin         = new Thickness(0, 0, 0, _globalToken.PaddingXS / 2);
        TreeItemHeaderPadding  = new Thickness(_globalToken.PaddingXS / 2, 0);
        TreeNodeSwitcherMargin = new Thickness(0, 0, _globalToken.PaddingXS / 2, 0);
        TreeNodeIconMargin     = new Thickness(_globalToken.PaddingXS / 2, 0, 0, 0);

        DragIndicatorLineWidth = _globalToken.LineWidthFocus;
    }
}