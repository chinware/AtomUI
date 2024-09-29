using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class SegmentedToken : AbstractControlDesignToken
{
    public const string ID = "Segmented";

    public SegmentedToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 选项文本颜色
    /// </summary>
    public Color ItemColor { get; set; }

    /// <summary>
    /// 选项悬浮态文本颜色
    /// </summary>
    public Color ItemHoverColor { get; set; }

    /// <summary>
    /// 选项悬浮态背景颜色
    /// </summary>
    public Color ItemHoverBg { get; set; }

    /// <summary>
    /// 选项激活态背景颜色
    /// </summary>
    public Color ItemActiveBg { get; set; }

    /// <summary>
    /// 选项选中时背景颜色
    /// </summary>
    public Color ItemSelectedBg { get; set; }

    /// <summary>
    /// 选项选中时文字颜色
    /// </summary>
    public Color ItemSelectedColor { get; set; }

    /// <summary>
    /// Segmented 控件容器的 padding
    /// </summary>
    public Thickness TrackPadding { get; set; }

    /// <summary>
    /// 大尺寸选项最小高度
    /// </summary>
    public double ItemMinHeightLG { get; set; }

    /// <summary>
    /// 选项最小高度
    /// </summary>
    public double ItemMinHeight { get; set; }

    /// <summary>
    /// 小尺寸选项最小高度
    /// </summary>
    public double ItemMinHeightSM { get; set; }

    /// <summary>
    /// Segmented 控件容器背景色
    /// </summary>
    public Color TrackBg { get; set; }

    // 内部 token
    public Thickness SegmentedItemPadding { get; set; }
    public Thickness SegmentedItemPaddingSM { get; set; }
    public Thickness SegmentedItemContentMargin { get; set; }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        TrackPadding      = new Thickness(_globalToken.LineWidthBold);
        TrackBg           = _globalToken.ColorBgLayout;
        ItemColor         = _globalToken.ColorTextLabel;
        ItemHoverColor    = _globalToken.ColorText;
        ItemHoverBg       = _globalToken.ColorFillSecondary;
        ItemSelectedBg    = _globalToken.ColorBgElevated;
        ItemActiveBg      = _globalToken.ColorFill;
        ItemSelectedColor = _globalToken.ColorText;
        var lineWidth = _globalToken.LineWidth;
        SegmentedItemPadding = new Thickness(
            Math.Max(_globalToken.ControlPadding - lineWidth, 0),
            0,
            Math.Max(_globalToken.ControlPadding - lineWidth, 0),
            0);
        SegmentedItemPaddingSM = new Thickness(
            Math.Max(_globalToken.ControlPaddingSM - lineWidth, 0),
            0,
            Math.Max(_globalToken.ControlPaddingSM - lineWidth, 0),
            0);
        SegmentedItemContentMargin = new Thickness(_globalToken.PaddingXXS, 0, 0, 0);

        ItemMinHeightLG = _globalToken.ControlHeightLG - TrackPadding.Top - TrackPadding.Bottom;
        ItemMinHeight   = _globalToken.ControlHeight - TrackPadding.Top - TrackPadding.Bottom;
        ItemMinHeightSM = _globalToken.ControlHeightSM - TrackPadding.Top - TrackPadding.Bottom;
    }
}