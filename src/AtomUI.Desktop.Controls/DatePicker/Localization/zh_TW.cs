using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.DatePickerLang;

[LanguageProvider(LanguageCode.zh_TW, DatePickerToken.ID)]
internal class zh_TW : LanguageProvider
{
    public const string Today = "今天";
    public const string Now = "現在";
    
    protected override Type GetResourceKindType() => typeof(DatePickerLangResourceKind);
}