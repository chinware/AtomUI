using AtomUI.Media;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class TagToken : AbstractControlDesignToken
{
    public const string ID = "Tag";

    /// <summary>
    /// 默认背景色
    /// </summary>
    public Color DefaultBg { get; set; }

    /// <summary>
    /// 默认文字颜色
    /// </summary>
    public Color DefaultColor { get; set; }

    public double TagFontSize { get; set; }
    public double TagLineHeight { get; set; }
    public double TagIconSize { get; set; }
    public double TagCloseIconSize { get; set; }
    public Thickness TagPadding { get; set; }
    public double TagTextPaddingInline { get; set; }
    public Color TagBorderlessBg { get; set; }

    public TagToken()
        : base(ID)
    {
    }

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        TagFontSize      = _globalToken.FontSizeSM;
        TagLineHeight    = _globalToken.LineHeightSM * TagFontSize;
        TagCloseIconSize = _globalToken.IconSizeXS;
        TagIconSize      = _globalToken.FontSizeIcon;
        TagPadding       = new Thickness(8, 0); // Fixed padding.
        // TODO 这个地方需要看看
        DefaultBg            = ColorUtils.OnBackground(_globalToken.ColorFillQuaternary, _globalToken.ColorBgContainer);
        TagBorderlessBg      = DefaultBg;
        DefaultColor         = _globalToken.ColorText;
        TagTextPaddingInline = _globalToken.PaddingXXS;
    }
}