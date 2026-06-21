using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

[LanguageProvider(LanguageCode.en_US, ToggleSwitchShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "Data Entry";
    public const string ComponentStatusStable = ".NET 10";
    public const string PageSubtitle = "Switch between two mutually exclusive states with optional text, icons, loading, and size variants.";
    public const string PageDescription = "ToggleSwitch is used for immediate on/off decisions. It supports disabled state, loading state, custom on/off content, icon content, size variants, motion, and wave feedback.";
    public const string InfoNamespaceLabel = "Namespace:";
    public const string InfoPackageLabel = "Package:";
    public const string InfoBaseClassLabel = "Base class:";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyIsChecked = "Current checked state inherited from ToggleButton.";
    public const string ApiPropertyGrooveBackground = "Overrides the switch groove background brush.";
    public const string ApiPropertyOnContent = "Content shown when the switch is checked.";
    public const string ApiPropertyOnContentTemplate = "Template used to render OnContent.";
    public const string ApiPropertyOffContent = "Content shown when the switch is unchecked.";
    public const string ApiPropertyOffContentTemplate = "Template used to render OffContent.";
    public const string ApiPropertySizeType = "Controls Large, Middle, Small, or Custom switch size.";
    public const string ApiPropertyIsLoading = "Shows a loading indicator and pending interaction state.";
    public const string ApiPropertyIsMotionEnabled = "Enables or disables switch motion effects.";
    public const string ApiPropertyIsWaveSpiritEnabled = "Enables or disables wave feedback.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNameTrackHeight = "Default switch track height.";
    public const string TokenNameTrackHeightSM = "Small switch track height.";
    public const string TokenNameTrackMinWidth = "Default minimum switch track width.";
    public const string TokenNameTrackMinWidthSM = "Small minimum switch track width.";
    public const string TokenNameTrackPadding = "Padding inside the switch track.";
    public const string TokenNameHandleBg = "Switch handle background color.";
    public const string TokenNameHandleShadow = "Switch handle shadow.";
    public const string TokenNameHandleSize = "Default switch handle size.";
    public const string TokenNameHandleSizeSM = "Small switch handle size.";
    public const string TokenNameInnerMinMargin = "Minimum inner margin for default content layout.";
    public const string TokenNameInnerMaxMargin = "Maximum inner margin for default content layout.";
    public const string TokenNameInnerMinMarginSM = "Minimum inner margin for small content layout.";
    public const string TokenNameInnerMaxMarginSM = "Maximum inner margin for small content layout.";
    public const string TokenNameIconSize = "Default content icon size.";
    public const string TokenNameIconSizeSM = "Small content icon size.";
    public const string TokenNameSwitchColor = "Active switch color.";
    public const string TokenNameSwitchDisabledOpacity = "Opacity used by disabled switch state.";
    public const string TokenNameExtraInfoFontSize = "Default font size for on/off text.";
    public const string TokenNameExtraInfoFontSizeSM = "Small font size for on/off text.";
    public const string TokenNameLoadingAnimationDuration = "Duration of the loading indicator animation.";
    public const string TokenNameOffStateLoadIndicatorColor = "Loading indicator color while the switch is off.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic usage.";
    public const string DisabledTitle = "Disabled";
    public const string DisabledDescription = "Disabled state of Switch.";
    public const string TextAndIconTitle = "Text and icon";
    public const string TextAndIconDescription = "With text and icon.";
    public const string TwoSizesTitle = "Sizes";
    public const string TwoSizesDescription = "SizeType controls preset and Custom switch size modes.";
    public const string LoadingTitle = "Loading";
    public const string LoadingDescription = "Mark a pending state of switch.";
    public const string P2ContentToggleDisabled = "toggle disabled";
    public const string P2ContentToggleLoading = "toggle loading";
    public const string P2ContentCustom = "Custom";

    public const string P2OnContentOn = "On";

    public const string P2OffContentOff = "Off";

    public const string P2OnContentText = "Open";

    public const string P2OffContentText = "Close";

    protected override Type GetResourceKindType() => typeof(ToggleSwitchShowCaseLangResourceKind);
}
