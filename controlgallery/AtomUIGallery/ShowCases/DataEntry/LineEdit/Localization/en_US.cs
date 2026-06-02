using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.LineEdit;

[LanguageProvider(LanguageCode.en_US, LineEditShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic usage";
    public const string BasicUsageDescription = "Basic usage example.";
    public const string InputSizesTitle = "Three sizes of Input";
    public const string InputSizesDescription = "There are three sizes of an Input box: large (40px), default (32px) and small (24px).";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Variants of Input.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Variants of Input disabled style.";
    public const string PrePostTabTitle = "Pre / Post tab";
    public const string PrePostTabDescription = "Using pre and post tabs example.";
    public const string WithClearIconTitle = "With clear icon";
    public const string WithClearIconDescription = "Input box with the remove icon, click the icon to delete everything.";
    public const string PasswordBoxTitle = "Password box";
    public const string PasswordBoxDescription = "Input type of password.";
    public const string PrefixAndSuffixTitle = "prefix and suffix";
    public const string PrefixAndSuffixDescription = "Add a prefix or suffix icons inside input.";
    public const string InputStatusTitle = "Status";
    public const string InputStatusDescription = "Add status to Input with status, which could be error or warning.";
    public const string SearchBoxTitle = "Search box";
    public const string SearchBoxDescription = "Example of creating a search box by grouping a standard input with a search button.";
    public const string DisabledSearchBoxTitle = "Disabled search box";
    public const string DisabledSearchBoxDescription = "Example of creating a search box by grouping a standard input with a search button.";
    public const string SearchBoxWithLoadingTitle = "Search box with loading";
    public const string SearchBoxWithLoadingDescription = "Search loading when onSearch.";
    public const string TextAreaTitle = "TextArea";
    public const string TextAreaDescription = "For multi-line input.";
    public const string AutoSizeTextAreaTitle = "Autosizing the height to fit the content";
    public const string AutoSizeTextAreaDescription = "autoSize prop for a textarea type of Input makes the height to automatically adjust based on the content. An option object can be provided to autoSize to specify the minimum and maximum number of lines the textarea will automatically adjust.";
    public const string CharacterCountingTitle = "With character counting";
    public const string CharacterCountingDescription = "Show character counting.";
    public const string TextAreaStatusTitle = "Status";
    public const string TextAreaStatusDescription = "Add status to TextArea with status, which could be error or warning.";

    protected override Type GetResourceKindType() => typeof(LineEditShowCaseLangResourceKind);
}
