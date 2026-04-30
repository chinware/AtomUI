using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using AtomUIGallery.Workspace.Views;

namespace AtomUIGallery.Workspace.Localization.CaseNavigationLang;

[LanguageProvider(LanguageCode.en_US, CaseNavigation.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string General = "General";
    public const string Layout = "Layout";
    public const string Navigation = "Navigation";
    public const string DataEntry = "Data Entry";
    public const string DataDisplay = "Data Display";
    public const string Feedback = "Feedback";

    protected override Type GetResourceKindType() => typeof(CaseNavigationLangResourceKind);
}
