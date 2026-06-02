using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.Steps;

[LanguageProvider(LanguageCode.en_US, StepsShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
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

    protected override Type GetResourceKindType() => typeof(StepsShowCaseLangResourceKind);
}
