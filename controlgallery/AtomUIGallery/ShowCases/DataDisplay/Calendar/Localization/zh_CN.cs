using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Calendar;

[LanguageProvider(LanguageCode.zh_CN, CalendarShowCase.LanguageId)]
internal partial class zh_CN : LanguageProvider
{
    public const string BasicTitle = "基础用法";
    public const string BasicDescription = "Calendar 的最简单用法。";

    protected override Type GetResourceKindType() => typeof(CalendarShowCaseLangResourceKind);
}
