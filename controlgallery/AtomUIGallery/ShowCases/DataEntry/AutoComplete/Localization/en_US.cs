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

    protected override Type GetResourceKindType() => typeof(AutoCompleteShowCaseLangResourceKind);
}
