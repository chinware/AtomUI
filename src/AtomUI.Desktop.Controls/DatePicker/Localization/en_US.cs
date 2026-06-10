using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.DatePickerLang;

[LanguageProvider(LanguageCode.en_US, DatePickerToken.ID)]
internal class en_US : LanguageProvider
{
    public en_US()
        : base(LanguageCode.en_US, DatePickerToken.ID)
    {
    }

    public const string Today = "Today";
    public const string Now = "Now";
    
    protected override Type GetResourceKindType() => typeof(DatePickerLangResourceKind);
}