using AtomUI.Theme.Language;

namespace AtomUI.Controls.QRCodeLang;

[LanguageProvider(LanguageCode.zh_CN, QRCodeToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string Refresh = "点击刷新";
    public const string Expired = "二维码过期";
    public const string Scanned = "已扫描";
}