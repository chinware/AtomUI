using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[ControlDesignToken]
internal class QRCodeToken : AbstractControlDesignToken
{
    public const string ID = "QRCode";
    public static readonly ControlTokenResourceScopeProvider ScopeProvider = new(ID);
    
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

    public override void CalculateTokenValues(bool isDarkMode)
    {
        base.CalculateTokenValues(isDarkMode);
        var colorBgContainer = SharedToken.ColorBgContainer;
        QRCodeTextColor           = SharedToken.ColorText;
        QRCodeMaskBackgroundColor = Color.FromArgb(244, colorBgContainer.R, colorBgContainer.G, colorBgContainer.B);
    }
    
    protected override Type GetTokenKindType() => typeof(QRCodeTokenKind);
}