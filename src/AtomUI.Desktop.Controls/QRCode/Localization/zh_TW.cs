using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.QRCodeLang;

[LanguageProvider(LanguageCode.zh_TW, QRCodeToken.ID)]
internal class zh_TW : LanguageProvider
{
    public zh_TW()
        : base(LanguageCode.zh_TW, QRCodeToken.ID)
    {
    }

    public const string Refresh = "點擊刷新";
    public const string Expired = "二維碼過期";
    public const string Scanned = "已掃描";
    
    protected override Type GetResourceKindType() => typeof(QRCodeLangResourceKind);
}