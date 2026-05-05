using AtomUI.Desktop.Controls.Localization;
using AtomUI.Theme.Language;

namespace AtomUI.Desktop.Controls.TourLang;

[LanguageProvider(LanguageCode.en_US, TourToken.ID)]
internal class en_US : LanguageProvider
{
    public const string Previous = "Previous";
    public const string Next = "Next";
    public const string Finish = "Finish";
    
    protected override Type GetResourceKindType() => typeof(TourLangResourceKind);
}