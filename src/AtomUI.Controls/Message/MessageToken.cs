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

    internal override void CalculateFromAlias()
    {
        base.CalculateFromAlias();
        ContentBg = _globalToken.ColorBgElevated;
        ContentPadding = new Thickness(
            (_globalToken.ControlHeightLG -
             _globalToken.FontSize * _globalToken.LineHeight) / 2,
            _globalToken.PaddingXS);
        MessageIconMargin = new Thickness(0, 0, _globalToken.MarginXS, 0);
        MessageTopMargin  = new Thickness(_globalToken.Margin, _globalToken.Margin, _globalToken.Margin, 0);
        MessageIconSize   = _globalToken.FontSizeSM * _globalToken.LineHeightSM;
    }
}