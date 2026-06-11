using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.DatePickerLang;

[LanguageProvider(LanguageCode.zh_TW, DatePickerToken.ID)]
internal class zh_TW : LanguageProvider
{
    public zh_TW()
        : base(LanguageCode.zh_TW, DatePickerToken.ID)
    {
    }

    public const string Today = "今天";
    public const string Now = "現在";
    
    protected override Type GetResourceKindType() => typeof(DatePickerLangResourceKind);
}