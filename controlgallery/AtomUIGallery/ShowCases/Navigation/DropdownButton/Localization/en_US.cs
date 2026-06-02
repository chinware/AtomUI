using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.DropdownButton;

[LanguageProvider(LanguageCode.en_US, DropdownButtonShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic dropdown menu.";
    public const string ButtonTypesTitle = "Button Types";
    public const string ButtonTypesDescription = "Support centralized button type.";
    public const string ArrowTitle = "Arrow";
    public const string ArrowDescription = "You could display an arrow.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "Support 6 placements.";

    protected override Type GetResourceKindType() => typeof(DropdownButtonShowCaseLangResourceKind);
}
