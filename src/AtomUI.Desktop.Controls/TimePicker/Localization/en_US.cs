using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.TimePickerLang;

[LanguageProvider(LanguageCode.en_US, TimePickerToken.ID)]
internal class en_US : LanguageProvider
{
    public en_US()
        : base(LanguageCode.en_US, TimePickerToken.ID)
    {
    }

    public const string AMText = "AM";
    public const string PMText = "PM";
    public const string Now = "Now";
    
    protected override Type GetResourceKindType() => typeof(TimePickerLangResourceKind);
}