using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class TextBoxToken : AbstractControlDesignToken
{
    public const string ID = "TextBox";

    public TextBoxToken()
        : base(ID)
    {
    }

    /// <summary>
    /// 默认边框色
    /// </summary>
    public Color BorderColor { get; set; }

    /// <summary>
    /// 默认边框厚度
    /// </summary>
    public Thickness BorderThickness { get; set; }

    /// <summary>
    /// 默认圆角
    /// </summary>
    public CornerRadius BorderRadius { get; set; }

    /// <summary>
    /// 大号圆角
    /// </summary>
    public CornerRadius BorderRadiusLG { get; set; }

    /// <summary>
    /// 小号圆角
    /// </summary>
    public CornerRadius BorderRadiusSM { get; set; }

    /// <summary>
    /// 默认内边距
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// 小号内边距
    /// </summary>
    public Thickness PaddingSM { get; set; }

    /// <summary>
    /// 大号内边距
    /// </summary>
    public Thickness PaddingLG { get; set; }

    /// <summary>
    /// 悬浮态边框色
    /// </summary>
    public Color HoverBorderColor { get; set; }

    /// <summary>
    /// 激活态边框色
    /// </summary>
    public Color ActiveBorderColor { get; set; }

    /// <summary>
    /// 激活态阴影
    /// </summary>
    public BoxShadows ActiveShadow { get; set; }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);

        var fontSize     = SharedToken.FontSize;
        var fontSizeLG   = SharedToken.FontSizeLG;
        var lineHeight   = SharedToken.RelativeLineHeight;
        var lineHeightLG = SharedToken.RelativeLineHeightLG;
        var lineWidth    = SharedToken.LineWidth;

        BorderColor     = SharedToken.ColorBorder;
        BorderThickness = SharedToken.BorderThickness;
        BorderRadius    = SharedToken.BorderRadius;
        BorderRadiusLG  = SharedToken.BorderRadiusLG;
        BorderRadiusSM  = SharedToken.BorderRadiusSM;
        Padding = new Thickness(SharedToken.UniformlyPaddingSM - lineWidth,
            Math.Round((SharedToken.ControlHeight - fontSize * lineHeight) / 2 * 10) / 10 - lineWidth);
        PaddingSM = new Thickness(SharedToken.ControlPaddingHorizontalSM - lineWidth,
            Math.Round((SharedToken.ControlHeightSM - fontSize * lineHeight) / 2 * 10) / 10 - lineWidth * 2);
        PaddingLG = new Thickness(SharedToken.ControlPaddingHorizontal - lineWidth,
            Math.Ceiling((SharedToken.ControlHeightLG - fontSizeLG * lineHeightLG) / 2 * 10) / 10 -
            lineWidth);
        HoverBorderColor  = SharedToken.ColorPrimaryHover;
        ActiveBorderColor = SharedToken.ColorPrimary;
        ActiveShadow = new BoxShadows(new BoxShadow
        {
            Spread = SharedToken.ControlOutlineWidth,
            Color  = SharedToken.ColorControlOutline
        });
    }

}
