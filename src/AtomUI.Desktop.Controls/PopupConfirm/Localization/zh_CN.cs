using AtomUI.Theme.Language;

namespace AtomUI.Controls.PopupConfirmLang;

[LanguageProvider(LanguageCode.zh_CN, PopupConfirmToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string Ok = "确定";
    public const string Cancel = "取消";
}