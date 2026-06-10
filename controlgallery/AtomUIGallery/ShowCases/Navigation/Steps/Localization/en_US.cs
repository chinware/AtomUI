using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Steps;

[LanguageProvider(LanguageCode.en_US, StepsShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioBasic = "Basic";
    public const string ScenarioInteractive = "Interactive";
    public const string ScenarioVertical = "Vertical";
    public const string ScenarioDotClickable = "Dot & Clickable";
    public const string ScenarioNavigation = "Navigation";
    public const string ScenarioProgress = "Progress";
    public const string ScenarioInline = "Inline";
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

    protected override Type GetResourceKindType() => typeof(StepsShowCaseLangResourceKind);
}
