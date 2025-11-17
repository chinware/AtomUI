using AtomUI.Theme.Language;

namespace AtomUI.Controls.QRCodeLang;

[LanguageProvider(LanguageCode.en_US, QRCodeToken.ID)]
internal class en_US : LanguageProvider
{
    public const string Refresh = "Refresh";
    public const string Expired = "QR code expired";
    public const string Scanned = "Scanned";
}