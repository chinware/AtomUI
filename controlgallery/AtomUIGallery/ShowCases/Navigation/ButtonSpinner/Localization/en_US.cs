using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ButtonSpinner;

[LanguageProvider(LanguageCode.en_US, ButtonSpinnerShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ComponentCategory = "Navigation";
    public const string ComponentStatusStable = "Stable";
    public const string PageSubtitle = "A compact spinner input with button handles.";
    public const string PageDescription = "ButtonSpinner combines an input-like content area with increment and decrement handles, and supports add-ons, prefixes, suffixes, statuses, sizes, and visual variants.";
    public const string InfoNamespaceLabel = "Namespace";
    public const string InfoPackageLabel = "Package";
    public const string InfoBaseClassLabel = "Base";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";

    public const string BasicTitle = "Basic";
    public const string BasicDescription = "Basic button spinner.";
    public const string ThreeSizesTitle = "ButtonSpinner sizes";
    public const string ThreeSizesDescription = "ButtonSpinner supports large (40px), default (32px), small (24px), and Custom with local size values.";
    public const string P2LabelSizeTypeLarge = "Large";
    public const string P2LabelSizeTypeMiddle = "Middle";
    public const string P2LabelSizeTypeSmall = "Small";
    public const string P2LabelSizeTypeCustom = "Custom";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Variants of Input.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Variants of disabled Input.";
    public const string PrePostTabTitle = "Pre / Post tab";
    public const string PrePostTabDescription = "Using pre and post tabs example.";
    public const string PrefixSuffixTitle = "prefix and suffix";
    public const string PrefixSuffixDescription = "Add a prefix or suffix icons inside input.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to Input with status, which could be error or warning.";

    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyIsSpinEnabled = "Enables pointer, keyboard, and wheel spin actions.";
    public const string ApiPropertyIsButtonSpinnerVisible = "Controls whether the spinner handle is visible.";
    public const string ApiPropertyButtonSpinnerLocation = "Places the spinner handle on the left or right side.";
    public const string ApiPropertyLeftAddOn = "Content displayed outside the input area on the left.";
    public const string ApiPropertyRightAddOn = "Content displayed outside the input area on the right.";
    public const string ApiPropertyInnerLeftContent = "Content displayed inside the input area before the value.";
    public const string ApiPropertyInnerRightContent = "Content displayed inside the input area after the value.";
    public const string ApiPropertySizeType = "Controls the button spinner size.";
    public const string ApiPropertyStyleVariant = "Controls outlined, filled, and borderless visual variants.";
    public const string ApiPropertyStatus = "Applies validation status styling.";
    public const string ApiPropertyIsButtonSpinnerFloatable = "Allows the spinner handle to float over content until interaction.";
    public const string ApiPropertyIsMotionEnabled = "Enables control motion when the theme allows it.";
    public const string ApiPropertySpinnerHandleWidth = "Overrides the spinner handle width.";
    public const string ApiPropertySpin = "Raised when the user requests an increase or decrease spin action.";

    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameControlWidth = "Default control width.";
    public const string TokenNameHandleWidth = "Spinner handle width.";
    public const string TokenNameHandleIconSize = "Spinner handle icon size.";
    public const string TokenNameHandleBg = "Spinner handle background color.";
    public const string TokenNameHandleActiveBg = "Spinner handle active background color.";
    public const string TokenNameHandleHoverColor = "Spinner handle hover foreground color.";
    public const string TokenNameHandleBorderColor = "Spinner handle border color.";
    public const string TokenNameFilledHandleBg = "Spinner handle background for filled variant.";
    public const string TokenNameInputFontSize = "Default input font size inherited from LineEdit.";
    public const string TokenNameInputFontSizeLG = "Large input font size inherited from LineEdit.";
    public const string TokenNameInputFontSizeSM = "Small input font size inherited from LineEdit.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(ButtonSpinnerShowCaseLangResourceKind);
}
