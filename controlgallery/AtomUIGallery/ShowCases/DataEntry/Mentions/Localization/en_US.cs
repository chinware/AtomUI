using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Mentions;

[LanguageProvider(LanguageCode.en_US, MentionsShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The simplest use.";
    public const string SizeTypeTitle = "Mentions size";
    public const string SizeTypeDescription = "Mentions supports large, middle, small, and Custom sizes with local height and font overrides.";
    public const string VariantsTitle = "Variants";
    public const string VariantsDescription = "Variants of Mentions, there are four variants: outlined filled borderless and underlined.";
    public const string AsynchronousLoadingTitle = "Asynchronous loading";
    public const string AsynchronousLoadingDescription = "async.";
    public const string CustomizeTriggerTokenTitle = "Customize Trigger Token";
    public const string CustomizeTriggerTokenDescription = "Customize Trigger Token by prefix props. Default to @, array also supported.";
    public const string DisabledOrReadOnlyTitle = "disabled or readOnly";
    public const string DisabledOrReadOnlyDescription = "Configure disabled and readOnly.";
    public const string PlacementTitle = "Placement";
    public const string PlacementDescription = "Change the suggestions placement.";
    public const string StatusTitle = "Status";
    public const string StatusDescription = "Add status to Mentions with status, which could be error or warning.";
    public const string AutoSizeTitle = "autoSize";
    public const string AutoSizeDescription = "Height autoSize.";
    public const string WithClearIconTitle = "With clear icon";
    public const string WithClearIconDescription = "Customize clear button.";
    public const string P2PlaceholderTextLarge = "Large";
    public const string P2PlaceholderTextMiddle = "Middle";
    public const string P2PlaceholderTextSmall = "Small";
    public const string P2PlaceholderTextCustom = "Custom";
    public const string P2PlaceholderTextOutlined = "Outlined";
    public const string P2PlaceholderTextFilled = "Filled";
    public const string P2PlaceholderTextBorderless = "Borderless";
    public const string P2PlaceholderTextUnderlined = "Underlined";
    public const string P2PlaceholderTextInputToMentionPeopleToMentionTag = "input @ to mention people, # to mention tag";
    public const string P2PlaceholderTextThisIsDisabledMentions = "this is disabled Mentions";
    public const string P2PlaceholderTextThisIsReadonlyMentions = "this is readOnly Mentions";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Mention people, tags or custom entities while typing.";
    public const string PageDescription =
        "Mentions provides trigger-based candidate popups, async option loading, custom trigger tokens, input variants, placement, status and auto-size scenarios.";
    public const string ComponentCategory = "Data Entry";
    public const string ComponentStatusStable = "Stable";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertyValue = "Current text value of the mentions input.";
    public const string ApiPropertyDefaultValue = "Initial text value applied when the control is created.";
    public const string ApiPropertyOptionsSource = "Static option source used for mention suggestions.";
    public const string ApiPropertyOptionsAsyncLoader = "Asynchronous loader that provides options from the current mention context.";
    public const string ApiPropertyOptionTemplate = "Template used to render each suggestion option.";
    public const string ApiPropertyTriggerPrefix = "Trigger tokens that open the candidate list.";
    public const string ApiPropertySplit = "Text inserted after a selected mention option.";
    public const string ApiPropertyIsAllowClear = "Shows a clear affordance when the input has content.";
    public const string ApiPropertyClearIcon = "Custom icon used by the clear affordance.";
    public const string ApiPropertyStyleVariant = "Visual input variant such as outlined, filled, borderless or underlined.";
    public const string ApiPropertyStatus = "Validation status displayed by the mentions surface.";
    public const string ApiPropertyPlacement = "Preferred popup placement for the candidate list.";
    public const string ApiPropertyIsAutoSize = "Allows the input height to grow with its text content.";
    public const string ApiPropertyLines = "Initial visible text line count.";
    public const string ApiPropertyMinLines = "Minimum visible text line count used by auto-size.";
    public const string ApiPropertyMaxLines = "Maximum visible text line count used by auto-size.";
    public const string ApiPropertyDisplayCandidateCount = "Number of candidate rows used to calculate popup height.";
    public const string ApiPropertyIsReadOnly = "Prevents editing while preserving readable content.";
    public const string ApiPropertyAsyncLoadDebounce = "Delay before invoking the async option loader.";
    public const string ApiPropertyAsyncLoadTimeout = "Maximum wait time for asynchronous option loading.";
    public const string ApiPropertyShouldUseOverlayPopup = "Uses an overlay popup host when suggestions are opened.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNamePopupContentPadding = "Padding inside the candidate popup.";
    public const string TokenNameOptionHeight = "Height of each suggestion option.";
    public const string TokenNameMinPopupWidth = "Minimum width of the candidate popup.";
    public const string TokenScopeComponent = "Mentions";
    public const string TokenStatusStable = "Stable";

    protected override Type GetResourceKindType() => typeof(MentionsShowCaseLangResourceKind);
}
