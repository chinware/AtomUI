using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class QRCodeToken : AbstractControlDesignToken
{
    public const string ID = "QRCode";
    
    /// <summary>
    /// QRCode 文字颜色
    /// Text color of QRCode
    /// </summary>
    public Color QRCodeTextColor { get; set; }

    /// <summary>
    /// QRCode 遮罩背景颜色
    /// Mask background color of QRCode
    /// </summary>
    public Color QRCodeMaskBackgroundColor { get; set; }

    public QRCodeToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues()
    {
        base.CalculateTokenValues();
        var colorBgContainer = SharedToken.ColorBgContainer;
        QRCodeTextColor           = SharedToken.ColorText;
        QRCodeMaskBackgroundColor = Color.FromArgb(244, colorBgContainer.R, colorBgContainer.G, colorBgContainer.B);
    }
}