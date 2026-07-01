using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Empty;

[LanguageProvider(LanguageCode.en_US, EmptyShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Simplest Usage.";
    public const string SizeTitle = "Size";
    public const string SizeDescription = "AtomUI supports three sizes of buttons: small, default and large.";
    public const string CustomizeTitle = "Customize";
    public const string CustomizeDescription = "Customize image source, image size, description and extra content.";
    public const string NoDescriptionTitle = "No description";
    public const string NoDescriptionDescription = "Simplest Usage with no description.";
    public const string P2DescriptionCustomizeDescription = "Customize Description";
    public const string P2ContentCreateNow = "Create Now";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Empty state placeholder for no data or unavailable content.";
    public const string PageDescription =
        "Use Empty to communicate that a container has no result, no configured content, or an optional recovery action.";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyPresetImage = "Uses a built-in empty-state illustration.";
    public const string ApiPropertyImagePath = "Loads an image asset by path; cannot be combined with ImageSource or PresetImage.";
    public const string ApiPropertyImageSource = "Uses an inline image source; cannot be combined with ImagePath or PresetImage.";
    public const string ApiPropertyDescription = "Text shown below the illustration.";
    public const string ApiPropertySizeType = "Controls illustration size and description spacing.";
    public const string ApiPropertyIsDescriptionVisible = "Shows or hides the description text.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameEmptyImgHeight = "Height of the large preset empty illustration.";
    public const string TokenNameEmptyImgHeightMD = "Height of the middle preset empty illustration.";
    public const string TokenNameEmptyImgHeightSM = "Height of the small preset empty illustration.";
    public const string TokenNameDescriptionMargin = "Description margin for the large size.";
    public const string TokenNameDescriptionMarginSM = "Description margin for middle and small sizes.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(EmptyShowCaseLangResourceKind);
}
