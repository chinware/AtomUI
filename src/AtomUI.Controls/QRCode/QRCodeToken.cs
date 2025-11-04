using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

[ControlDesignToken]
internal class QRCodeToken : AbstractControlDesignToken
{
    public const string ID = "QRCode";

    /// <summary>
    /// 按钮内间距
    /// </summary>
    public Thickness Border { get; set; }

    public Color QRCodeMaskBackgroundColor { get; set; }

    public QRCodeToken()
        : base(ID)
    {
    }

    public override void CalculateTokenValues()
    {
        base.CalculateTokenValues();
        Border = new Thickness(SharedToken.LineWidth);
        var colorBgContainer = SharedToken.ColorBgContainer;
        QRCodeMaskBackgroundColor = Color.FromArgb(244, colorBgContainer.R, colorBgContainer.G, colorBgContainer.B);
    }
}