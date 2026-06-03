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
    public const string P2PlaceholderTextBasicUsage = "Basic usage";
    public const string P2PlaceholderTextLarge = "Large";
    public const string P2PlaceholderTextMiddle = "Middle";
    public const string P2PlaceholderTextSmall = "Small";
    public const string P2TitleNormal = "Normal";
    public const string P2PlaceholderTextOutlined = "Outlined";
    public const string P2PlaceholderTextFilled = "Filled";
    public const string P2PlaceholderTextBorderless = "Borderless";
    public const string P2PlaceholderTextUnderlined = "Underlined";
    public const string P2TitleLeftaddonAndRightaddon = "LeftAddOn and RightAddOn";
    public const string P2TextMysite = "mysite";
    public const string P2PlaceholderTextInputWithClearIcon = "input with clear icon";
    public const string P2PlaceholderTextTextareaWithClearIcon = "textarea with clear icon";
    public const string P2PlaceholderTextInputPassword = "input password";
    public const string P2PlaceholderTextEnterYourUsername = "Enter your username";
    public const string P2PlaceholderTextError = "Error";
    public const string P2PlaceholderTextWarning = "Warning";
    public const string P2PlaceholderTextErrorWithPrefix = "Error with prefix";
    public const string P2PlaceholderTextWarningWithPrefix = "Warning with prefix";
    public const string P2TitleSearch = "Search";
    public const string P2PlaceholderTextInputSearchText = "input search text";
    public const string P2TitleFilled = "Filled";
    public const string P2TitleBorderless = "Borderless";
    public const string P2TitleUnderlined = "Underlined";
    public const string P2PlaceholderTextInputSearchLoadingDefault = "input search loading default";
    public const string P2PlaceholderTextInputSearchLoadingWithEnterbutton = "input search loading with enterButton";
    public const string P2PlaceholderTextMaxlengthIsN6 = "maxLength is 6";
    public const string P2PlaceholderTextDisabled = "disabled";
    public const string P2PlaceholderTextAutosizeHeightBasedOnContentLines = "Autosize height based on content lines";
    public const string P2PlaceholderTextAutosizeHeightWithMinimumAndMaximumNumberOf = "Autosize height with minimum and maximum number of lines";
    public const string P2PlaceholderTextCanResize = "can resize";
    public const string P2PlaceholderTextDisableResize = "disable resize";

    public const string P2SearchButtonTextSearch = "Search";

    public const string P2SearchButtonTextText = "Search now";

    protected override Type GetResourceKindType() => typeof(LineEditShowCaseLangResourceKind);
}
