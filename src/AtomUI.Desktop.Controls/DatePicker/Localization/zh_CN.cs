using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.DatePickerLang;

[LanguageProvider(LanguageCode.zh_CN, DatePickerToken.ID)]
internal class zh_CN : LanguageProvider
{
    public const string Today = "今天";
    public const string Now = "现在";
    
    protected override Type GetResourceKindType() => typeof(DatePickerLangResourceKind);
}