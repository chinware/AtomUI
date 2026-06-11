using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Calendar;

[LanguageProvider(LanguageCode.zh_TW, CalendarShowCase.LanguageId)]
internal partial class zh_TW : LanguageProvider
{
    public const string BasicTitle = "基礎用法";
    public const string BasicDescription = "Calendar 的最簡單用法。";

    protected override Type GetResourceKindType() => typeof(CalendarShowCaseLangResourceKind);
}

