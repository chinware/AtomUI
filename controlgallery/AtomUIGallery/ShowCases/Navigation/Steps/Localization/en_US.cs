using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Steps;

[LanguageProvider(LanguageCode.en_US, StepsShowCase.LanguageId)]
internal partial class en_US
{
    public const string ScenarioBasic = "Basic";
    public const string ScenarioInteractive = "Interactive";
    public const string ScenarioVertical = "Vertical";
    public const string ScenarioDotClickable = "Dot & Clickable";
    public const string ScenarioNavigation = "Navigation";
    public const string ScenarioProgress = "Progress";
    public const string ScenarioInline = "Inline";
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string PageSubtitle = "Guide users through ordered tasks and process states.";
    public const string PageDescription = "Steps display a sequence of tasks, progress, navigation states, and optional step content for workflows that need clear stage awareness.";
    public const string ComponentCategory = "Navigation";
    public const string ComponentStatusStable = "Stable";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string ApiPropertyCurrentStep = "Sets or binds the current active step index.";
    public const string ApiPropertyInitialStep = "Sets the initial active step when the control is first configured.";
    public const string ApiPropertyProgressValue = "Progress percentage used by the current step when item progress is visible.";
    public const string ApiPropertyCurrentStepStatus = "Status applied to the current step.";
    public const string ApiPropertyOrientation = "Controls horizontal or vertical step layout.";
    public const string ApiPropertyLabelPlacement = "Controls whether labels are arranged horizontally or vertically.";
    public const string ApiPropertySizeType = "Controls the visual size of the steps.";
    public const string ApiPropertyItemIndicatorType = "Controls the indicator style, such as default or dot.";
    public const string ApiPropertyStyle = "Controls the visual style, including navigation and inline modes.";
    public const string ApiPropertyIsItemClickable = "Allows users to select a step item directly.";
    public const string ApiPropertyIsShowItemProgress = "Shows progress inside the current item indicator.";
    public const string ApiPropertyContentTemplate = "Template used for current step content.";
    public const string ApiPropertyCurrentContent = "Current selected step content exposed by Steps.";
    public const string ApiPropertyStepsItemSubHeader = "Secondary header content displayed near the step title.";
    public const string ApiPropertyStepsItemDescription = "Description content displayed below the step title.";
    public const string ApiPropertyStepsItemIcon = "Custom icon displayed in the step indicator.";
    public const string ApiPropertyStepsItemStatus = "Explicit status for an individual step item.";
    public const string TokenNameDescriptionMaxWidth = "Maximum width of the step description area.";
    public const string TokenNameIconSize = "Default step indicator container size.";
    public const string TokenNameIconFontSize = "Default step indicator icon font size.";
    public const string TokenNameIconSizeSM = "Small step indicator size.";
    public const string TokenNameDotSize = "Dot indicator size.";
    public const string TokenNameDotCurrentSize = "Current dot indicator size.";
    public const string TokenNameDotLineThickness = "Thickness of the dot connector line.";
    public const string TokenNameHorizontalHeaderMargin = "Header margin for horizontal steps.";
    public const string TokenNameVerticalItemSpacing = "Spacing between vertical step items.";
    public const string TokenNameVerticalDescriptionPadding = "Padding used by vertical step descriptions.";
    public const string TokenNameStepsNavActiveColor = "Active color used by navigation steps.";
    public const string TokenNameStepsProgressSize = "Size used by item progress in steps.";
    public const string TokenNameInlineDotSize = "Dot size used by inline steps.";
    public const string TokenNameInlineItemPadding = "Padding used by inline step items.";
    public const string TokenNameProcessIconBgColor = "Background color for the process step indicator.";
    public const string TokenNameFinishTailColor = "Connector color for finished steps.";
    public const string TokenNameErrorIconBgColor = "Background color for the error step indicator.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";
    public const string BasicTitle = "Basic";
    public const string BasicDescription = "The most basic step bar.";
    public const string MiniVersionTitle = "Mini version";
    public const string MiniVersionDescription = "By setting like this: SizeType to Small, you can get a mini version.";
    public const string WithIconTitle = "With icon";
    public const string WithIconDescription = "You can use your own custom icons by setting the property icon for items.";
    public const string SwitchStepTitle = "Switch Step";
    public const string SwitchStepDescription = "Cooperate with the content and buttons, to represent the progress of a process.";
    public const string VerticalTitle = "Vertical";
    public const string VerticalDescription = "A simple step bar in the vertical direction.";
    public const string VerticalMiniVersionTitle = "Vertical mini version";
    public const string VerticalMiniVersionDescription = "A simple mini version step bar in the vertical direction.";
    public const string ErrorStatusTitle = "Error status";
    public const string ErrorStatusDescription = "By using status of Steps, you can specify the state for current step.";
    public const string DotStyleTitle = "Dot Style";
    public const string DotStyleDescription = "Steps with progress dot style.";
    public const string DotStyleVerticalTitle = "Dot Style Vertical";
    public const string DotStyleVerticalDescription = "Steps with progress dot style vertical.";
    public const string ClickableTitle = "Clickable";
    public const string ClickableDescription = "Setting IsItemClickable=true makes Steps clickable.";
    public const string NavigationStepsTitle = "Navigation Steps";
    public const string NavigationStepsDescription = "Navigation steps.";
    public const string StepsWithProgressTitle = "Steps with progress";
    public const string StepsWithProgressDescription = "Steps with progress.";
    public const string LabelPlacementTitle = "Label Placement";
    public const string LabelPlacementDescription = "Set labelPlacement to vertical.";
    public const string InlineStepsTitle = "Inline Steps";
    public const string InlineStepsDescription = "Inline type steps, suitable for displaying the process and current state of the object in the list content scene.";
    public const string P2DescriptionThisIsADescription = "This is a description.";
    public const string P2HeaderFinished = "Finished";
    public const string P2HeaderInProgress = "In Progress";
    public const string P2HeaderWaiting = "Waiting";
    public const string P2HeaderLogin = "Login";
    public const string P2HeaderVerification = "Verification";
    public const string P2HeaderPay = "Pay";
    public const string P2HeaderDone = "Done";
    public const string P2HeaderFirst = "First";
    public const string P2HeaderSecond = "Second";
    public const string P2HeaderThird = "Third";
    public const string P2HeaderStepN1 = "Step 1";
    public const string P2HeaderStepN2 = "Step 2";
    public const string P2HeaderStepN3 = "Step 3";
    public const string P2HeaderStepN4 = "Step 4";
    public const string P2HeaderFinishN1 = "finish 1";
    public const string P2HeaderFinishN2 = "finish 2";
    public const string P2HeaderCurrentProcess = "current process";
    public const string P2HeaderWait = "wait";
    public const string P2SubHeaderLeftTime = "Left 00:00:08";
    public const string P2SubHeaderWaitingForLongTime = "waiting for longlong time";
    public const string P2TextAntDesignTitleN1 = "Ant Design Title 1";
    public const string P2TextAntDesignADesignLanguageForBackgroundApplications = "Ant Design, a design language for background applications, is refined by Ant UED Team";
    public const string P2TextAntDesignTitleN2 = "Ant Design Title 2";
    public const string P2TextAntDesignTitleN3 = "Ant Design Title 3";
    public const string P2TextAntDesignTitleN4 = "Ant Design Title 4";

    public const string P2ContentFirstContent = "First-content";

    public const string P2ContentSecondContent = "Second-content";

    public const string P2ContentLastContent = "Last-content";

    public const string P2ContentNext = "Next";

    public const string P2ContentPrevious = "Previous";

    public const string P2ContentDone = "Done";

}
