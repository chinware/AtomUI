using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ProgressBar;

[LanguageProvider(LanguageCode.en_US, ProgressBarShowCase.LanguageId)]
internal partial class en_US : LanguageProvider
{
    public const string ScenarioBasic = "Basic";
    public const string ScenarioAdvanced = "Advanced";
    public const string ScenarioLayout = "Layout";
    public const string ProgressBarTitle = "Progress bar";
    public const string ProgressBarDescription = "A standard progress bar.";
    public const string CircularProgressBarTitle = "Circular progress bar";
    public const string CircularProgressBarDescription = "A circular progress bar.";
    public const string MiniSizeTitle = "Mini size progress bar";
    public const string MiniSizeDescription = "Appropriate for a narrow area.";
    public const string SmallerCircularProgressBarTitle = "A smaller circular progress bar.";
    public const string SmallerCircularProgressBarDescription = "A smaller circular progress bar.";
    public const string DynamicTitle = "Dynamic";
    public const string DynamicDescription = "A dynamic progress bar is better.";
    public const string CustomTextFormatTitle = "Custom text format";
    public const string CustomTextFormatDescription = "You can set a custom text by setting the format prop.";
    public const string DashboardTitle = "Dashboard";
    public const string DashboardDescription = "By setting type=dashboard, you can get a dashboard style of progress easily. Modify gapDegree to set the degree of gap.";
    public const string SuccessSegmentTitle = "Progress bar with success segment";
    public const string SuccessSegmentDescription = "Show several parts of progress with different status.";
    public const string StrokeLinecapTitle = "Stroke Linecap";
    public const string StrokeLinecapDescription = "By setting strokeLinecap='butt', you can change the linecaps from round to butt, see stroke-linecap for more information.";
    public const string CustomLineGradientTitle = "Custom line gradient";
    public const string CustomLineGradientDescription = "Gradient encapsulation, circle and dashboard will ignore strokeLinecap when setting gradient.";
    public const string StepsTitle = "Progress bar with steps";
    public const string StepsDescription = "A progress bar with steps.";
    public const string CircularStepsTitle = "Circular progress bar whit steps";
    public const string CircularStepsDescription = "A circular progress bar that support steps and color segments, default gap is 2px.";
    public const string PercentPositionTitle = "Change progress value position";
    public const string PercentPositionDescription = "Change the position of the progress value, you can use percentPosition to adjust it so that the progress bar value is inside, outside or at the bottom of the progress bar.";
    public const string StepsPercentPositionTitle = "Change progress value position for StepsProgressBar";
    public const string StepsPercentPositionDescription = "Change the position of the progress value, you can use percentPosition to adjust it so that the progress bar value is inside, outside or at the bottom of the progress bar.";
    public const string VerticalLinearTitle = "Vertical progress bar";
    public const string VerticalLinearDescription = "Ordinary linear progress bar, supports position specification of additional areas";
    public const string VerticalStepsTitle = "Vertical progress bar";
    public const string VerticalStepsDescription = "Ordinary step progress bar, supports position specification of additional areas";
    public const string ToggleDisabledStatusTitle = "toggle disabled status";
    public const string ToggleDisabledStatusDescription = "The progress bar is in the disabled state and uses the disabled style.";
    public const string P2ContentSub = "Sub";
    public const string P2ContentAdd = "Add";

    protected override Type GetResourceKindType() => typeof(ProgressBarShowCaseLangResourceKind);
}
