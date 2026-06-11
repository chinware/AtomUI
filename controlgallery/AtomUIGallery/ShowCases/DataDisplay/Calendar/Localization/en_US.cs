using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Calendar;

[LanguageProvider(LanguageCode.en_US, CalendarShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest usage for Calendar.";

    protected override Type GetResourceKindType() => typeof(CalendarShowCaseLangResourceKind);
}
