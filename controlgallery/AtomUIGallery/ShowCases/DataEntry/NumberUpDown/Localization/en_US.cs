using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.NumberUpDown;

[LanguageProvider(LanguageCode.en_US, NumberUpDownShowCase.LanguageId)]
internal partial class en_US
{
    public const string ScenarioBasic = "Basic";
    public const string ScenarioRange = "Range";
    public const string ScenarioStyle = "Style";
    public const string ScenarioAddon = "Add-ons";

    public const string BasicUsageTitle = "Basic usage";
    public const string BasicUsageDescription = "Numeric-only NumberUpDown.";
    public const string SpinnerModeTitle = "Spinner";
    public const string SpinnerModeDescription = "Numeric spinner.";
    public const string HideHandleTitle = "Hide handle";
    public const string HideHandleDescription = "Hide the spin handle with ShowButtonSpinner=\"False\".";
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
    public const string SizesTitle = "Sizes of NumberUpDown";
    public const string SizesDescription = "NumberUpDown supports large (40px), default (32px), small (24px) and custom size.";
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
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Enter and adjust numeric values with keyboard, wheel and step controls.";
    public const string PageDescription =
        "NumberUpDown combines numeric input, precision string mode, min/max constraints, decimal steps, input variants, add-ons, clear affordances and validation status.";
    public const string ComponentCategory = "Data Entry";
    public const string ComponentStatusStable = "Stable";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyValue = "Current numeric value.";
    public const string ApiPropertyMinimum = "Lowest accepted numeric value.";
    public const string ApiPropertyMaximum = "Highest accepted numeric value.";
    public const string ApiPropertyIncrement = "Amount added or subtracted by spin buttons, keyboard or wheel actions.";
    public const string ApiPropertyFormatString = "Format string used to display the numeric value.";
    public const string ApiPropertyMode = "Display mode of the control, either the default input mode or the inline spinner mode.";
    public const string ApiPropertyIsStringMode = "Keeps high-precision input as text while preserving numeric editing behavior.";
    public const string ApiPropertyStringValue = "String value used by high-precision string mode.";
    public const string ApiPropertyIsKeyboardEnabled = "Allows keyboard spin shortcuts such as Up, Down, PageUp and PageDown.";
    public const string ApiPropertyIsAllowClear = "Shows a clear affordance when the input has content.";
    public const string ApiPropertyClearIcon = "Custom icon used by the clear affordance.";
    public const string ApiPropertySizeType = "Input size variant.";
    public const string ApiPropertyStyleVariant = "Visual input variant such as outlined, filled or borderless.";
    public const string ApiPropertyStatus = "Validation status displayed by the input surface.";
    public const string ApiPropertyLeftAddOn = "Content attached before the input frame.";
    public const string ApiPropertyRightAddOn = "Content attached after the input frame.";
    public const string ApiPropertyInnerLeftContent = "Content rendered inside the input on the left.";
    public const string ApiPropertyInnerRightContent = "Content rendered inside the input on the right.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameControlWidth = "Default width of the numeric input.";
    public const string TokenNameHandleWidth = "Width of the spin button area.";
    public const string TokenNameHandleIconSize = "Icon size used by the spin buttons.";
    public const string TokenNameHandleBg = "Background color of the spin button area.";
    public const string TokenNameHandleActiveBg = "Active background color of the spin button area.";
    public const string TokenNameHandleHoverColor = "Foreground color used by the spin buttons on hover.";
    public const string TokenNameHandleBorderColor = "Border color around the spin button area.";
    public const string TokenNameFilledHandleBg = "Spin button background used by the filled variant.";
    public const string TokenScopeComponent = "NumericUpDown";
    public const string TokenStatusStable = "Stable";

}
