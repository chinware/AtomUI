using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.NumberUpDown;

[LanguageProvider(LanguageCode.en_US, NumberUpDownShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioBasic = "Basic";
    public const string ScenarioRange = "Range";
    public const string ScenarioStyle = "Style";
    public const string ScenarioAddon = "Add-ons";

    public const string BasicUsageTitle = "Basic usage";
    public const string BasicUsageDescription = "Numeric-only NumberUpDown.";
    public const string StringModeTitle = "String mode (high precision)";
    public const string StringModeDescription = "Keep a high-precision value as a string.";
    public const string KeyboardBehaviorTitle = "Keyboard behavior";
    public const string KeyboardBehaviorDescription = "Disable keyboard spin with the Keyboard property.";
    public const string MouseWheelBehaviorTitle = "Mouse wheel behavior";
    public const string MouseWheelBehaviorDescription = "Scroll the mouse wheel while the input is focused to increment or decrement by Increment.";
    public const string MinMaxTitle = "Min / Max";
    public const string MinMaxDescription = "Restrict the input value range.";
    public const string DecimalStepTitle = "Step (decimal)";
    public const string DecimalStepDescription = "Use decimal steps with Increment.";
    public const string SizesTitle = "Three sizes of NumberUpDown";
    public const string SizesDescription = "There are three sizes of an Input box: large (40px), default (32px) and small (24px).";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Variants of NumberUpDown.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Variants of NumberUpDown of disabled style.";
    public const string PrePostTabTitle = "Pre / Post tab";
    public const string PrePostTabDescription = "Using pre and post tabs example.";
    public const string WithClearIconTitle = "With clear icon";
    public const string WithClearIconDescription = "Input box with the remove icon, click the icon to delete everything.";
    public const string PrefixAndSuffixTitle = "prefix and suffix";
    public const string PrefixAndSuffixDescription = "Add a prefix or suffix icons inside input.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to Input with status, which could be error or warning.";
    public const string P2PlaceholderTextInputWeight = "Input weight";
    public const string P2PlaceholderTextKeyboardDisabled = "Keyboard disabled";
    public const string P2PlaceholderTextFocusAndScrollWheel = "Focus and scroll wheel";
    public const string P2PlaceholderTextInputWithClearIcon = "input with clear icon";
    public const string P2PlaceholderTextEnterYourValue = "Enter your value";
    public const string P2PlaceholderTextError = "Error";
    public const string P2PlaceholderTextWarning = "Warning";
    public const string P2PlaceholderTextErrorWithPrefix = "Error with prefix";
    public const string P2PlaceholderTextWarningWithPrefix = "Warning with prefix";
    public const string P2TextRawValuePrefix = "Raw: ";

    public const string P2ContentKeyboardEnabled = "Keyboard enabled";

    protected override Type GetResourceKindType() => typeof(NumberUpDownShowCaseLangResourceKind);
}
