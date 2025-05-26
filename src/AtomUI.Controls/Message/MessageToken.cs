using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class MessageToken : AbstractControlDesignToken
{
    public const string ID = "Message";

    /// <summary>
    /// 提示框背景色
    /// </summary>
    public Color ContentBg { get; set; }

    /// <summary>
    /// 提示框内边距
    /// </summary>
    public Thickness ContentPadding { get; set; }

    /// <summary>
    /// 提示框高度
    /// </summary>
    public double CardHeight { get; set; }

    /// <summary>
    /// 提醒框图标尺寸
    /// </summary>
    public double MessageIconSize { get; set; }

    /// <summary>
    /// 提醒框图标外边距
    /// </summary>
    public Thickness MessageIconMargin { get; set; }

    /// <summary>
    /// 提醒框上边缘外边距
    /// </summary>
    public Thickness MessageTopMargin { get; set; }

    public MessageToken()
        : base(ID)
    {
    }

    protected internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        ContentBg = SharedToken.ColorBgElevated;
        ContentPadding = new Thickness(
            (SharedToken.ControlHeightLG -
             SharedToken.FontSize * SharedToken.LineHeightRatio) / 2,
            SharedToken.PaddingXS);
        MessageIconMargin = new Thickness(0, 0, SharedToken.MarginXS, 0);
        MessageTopMargin  = new Thickness(SharedToken.Margin, SharedToken.Margin, SharedToken.Margin, 0);
        MessageIconSize   = SharedToken.FontSizeSM * SharedToken.LineHeightRatioSM;
    }
}