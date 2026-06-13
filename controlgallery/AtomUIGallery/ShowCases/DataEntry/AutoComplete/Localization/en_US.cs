using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.AutoComplete;

[LanguageProvider(LanguageCode.en_US, AutoCompleteShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic Usage";
    public const string BasicUsageDescription = "Basic usage, set data source of autocomplete with options property.";
    public const string CustomizedTitle = "Customized";
    public const string CustomizedDescription = "You could set custom Option label.";
    public const string CustomOptionRenderingTitle = "Custom option rendering";
    public const string CustomOptionRenderingDescription = "Use OptionTemplate to render rich option content with multiple fields, badges, and multi-line layout.";
    public const string LookupPatternsUncertainCategoryTitle = "Lookup-Patterns - Uncertain Category";
    public const string LookupPatternsUncertainCategoryDescription = "Demonstration of Lookup Patterns: Uncertain Category.";
    public const string TextAreaAutoCompletionTitle = "TextArea type auto-completion";
    public const string TextAreaAutoCompletionDescription = "You can use the TextArea type for autocomplete.";
    public const string NonCaseSensitiveTitle = "Non-case-sensitive AutoComplete";
    public const string NonCaseSensitiveDescription = "A non-case-sensitive AutoComplete.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to AutoComplete with status, which could be error or warning.";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "There are outlined, filled, borderless, and underlined variants to choose from.";
    public const string CustomizeClearButtonTitle = "Customize clear button";
    public const string CustomizeClearButtonDescription = "Customize clear button.";
    public const string P2PlaceholderTextInputHere = "input here";
    public const string P2PlaceholderTextTryAOrB = "try 'a' or 'b'";
    public const string P2TextResults = "results";
    public const string P2PlaceholderTextTryToTypeB = "try to type `b`";
    public const string P2PlaceholderTextOutline = "Outline";
    public const string P2PlaceholderTextFilled = "Filled";
    public const string P2PlaceholderTextBorderless = "Borderless";
    public const string P2PlaceholderTextUnderlined = "Underlined";
    public const string P2PlaceholderTextUnclearable = "UnClearable";
    public const string P2PlaceholderTextCustomizedClearIcon = "Customized clear icon";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Type ahead and select suggested values from local or asynchronous options.";
    public const string PageDescription =
        "AutoComplete combines text input with candidate filtering, async loading, custom option rendering, status feedback and specialized search or text area variants.";
    public const string ComponentCategory = "Data Entry";
    public const string ComponentStatusStable = "Stable";
    public const string InfoNamespaceLabel = "Namespace";
    public const string InfoPackageLabel = "Package";
    public const string InfoBaseClassLabel = "Base class";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyValue = "Current text value of the autocomplete input.";
    public const string ApiPropertyOptionsSource = "Static option source used for candidate suggestions.";
    public const string ApiPropertyOptionsAsyncLoader = "Asynchronous loader that provides options from the current input context.";
    public const string ApiPropertyOptionTemplate = "Template used to render each suggestion option.";
    public const string ApiPropertyFilter = "Filter used to decide which options remain visible.";
    public const string ApiPropertyFilterValueSelector = "Selects the option value passed to the filter.";
    public const string ApiPropertyIsAllowClear = "Shows a clear affordance when the input has content.";
    public const string ApiPropertyClearIcon = "Custom icon used by the clear affordance.";
    public const string ApiPropertyStyleVariant = "Visual input variant such as outlined, filled, borderless or underlined.";
    public const string ApiPropertyStatus = "Validation status displayed by the input surface.";
    public const string ApiPropertyPlacement = "Preferred popup placement for the candidate list.";
    public const string ApiPropertyMinimumPrefixLength = "Minimum input length before autocomplete suggestions can open.";
    public const string ApiPropertyDisplayCandidateCount = "Number of candidate rows used to calculate popup height.";
    public const string ApiPropertyMaxDropDownHeight = "Maximum candidate popup height.";
    public const string ApiPropertyIsPopupMatchSelectWidth = "Matches the popup width to the input width.";
    public const string ApiPropertyShouldUseOverlayPopup = "Uses an overlay popup host when suggestions are opened.";
    public const string ApiPropertyAutoCompleteSearchButtonStyle = "Button style used by AutoCompleteSearchEdit.";
    public const string ApiPropertyAutoCompleteTextAreaLines = "Visible text lines used by AutoCompleteTextArea.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNamePopupContentPadding = "Padding inside the candidate popup.";
    public const string TokenNameOptionHeight = "Height of each suggestion option.";
    public const string TokenNameMinPopupWidth = "Minimum width of the candidate popup.";
    public const string TokenNameMaxPopupWidth = "Maximum popup width when it does not follow the input width.";
    public const string TokenScopeComponent = "AutoComplete";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(AutoCompleteShowCaseLangResourceKind);
}
