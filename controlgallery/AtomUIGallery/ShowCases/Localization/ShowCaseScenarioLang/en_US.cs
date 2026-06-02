using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using AtomUIGallery.ShowCases;

namespace AtomUIGallery.ShowCases.Localization.ShowCaseScenarioLang;

[LanguageProvider(LanguageCode.en_US, ShowCaseScenario.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string Basic = "Basic";
    public const string Layout = "Layout";
    public const string States = "States";
    public const string Validation = "Validation";
    public const string Dynamic = "Dynamic";
    public const string Presets = "Presets";
    public const string Controls = "Controls";
    public const string Interaction = "Interaction";
    public const string Filtering = "Filtering";
    public const string Structure = "Structure";
    public const string Fixed = "Fixed";
    public const string Drag = "Drag";
    public const string Editing = "Editing";
    public const string Paging = "Paging";
    public const string Size = "Size";
    public const string Align = "Align";
    public const string CompactForm = "Compact Form";
    public const string CompactButton = "Compact Button";

    protected override Type GetResourceKindType() => typeof(ShowCaseScenarioLangResourceKind);
}
