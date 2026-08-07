using AtomUI.Desktop.Controls;
using AtomUI.Icons.AntDesign;
using Avalonia.Controls;
using Avalonia.Layout;
using AtomTextBlock = AtomUI.Desktop.Controls.TextBlock;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static IReadOnlyList<PerfScenario> CreateStepsScenarios()
    {
        return
        [
            new PerfScenario("Steps.Basic.Items3", _ => CreateSteps()),
            new PerfScenario("Steps.Vertical.Items3", _ => CreateSteps(orientation: Orientation.Vertical, currentStep: 1)),
            new PerfScenario("Steps.Dot.Items4", _ => CreateSteps(count: 4, indicatorType: StepsItemIndicatorType.Dot, currentStep: 1)),
            new PerfScenario("Steps.Navigation.Items4", _ => CreateSteps(count: 4, style: StepsStyle.Navigation, isClickable: true)),
            new PerfScenario("Steps.Inline.Items3", _ => CreateSteps(style: StepsStyle.Inline, currentStep: 1)),
            new PerfScenario("Steps.Progress.Items3", _ => CreateSteps(currentStep: 1, progressValue: 60, isShowProgress: true)),
            new PerfScenario("Steps.Icon.Items4", _ => CreateIconSteps()),
            new PerfScenario("Steps.GalleryShape", _ => CreateStepsGalleryShape())
        ];
    }

    private static Steps CreateSteps(int count = 3,
                                     Orientation orientation = Orientation.Horizontal,
                                     StepsStyle style = StepsStyle.Default,
                                     StepsItemIndicatorType indicatorType = StepsItemIndicatorType.Default,
                                     int currentStep = 0,
                                     bool isClickable = false,
                                     bool isShowProgress = false,
                                     double progressValue = 0)
    {
        var steps = new Steps
        {
            Width               = orientation == Orientation.Horizontal ? 760 : 360,
            CurrentStep         = currentStep,
            Orientation         = orientation,
            Style               = style,
            ItemIndicatorType   = indicatorType,
            IsItemClickable     = isClickable,
            IsShowItemProgress  = isShowProgress,
            ProgressValue       = progressValue
        };

        for (var i = 0; i < count; i++)
        {
            steps.Items.Add(new StepsItem
            {
                Header      = $"Step {i + 1}",
                SubHeader   = i == 1 ? "Left 00:00:08" : null,
                Description = "This is a description."
            });
        }

        return steps;
    }

    private static Steps CreateIconSteps()
    {
        var steps = CreateSteps(count: 0);
        steps.Items.Add(new StepsItem { Header = "Login", Status = StepsItemStatus.Finish, Icon = new UserOutlined() });
        steps.Items.Add(new StepsItem { Header = "Verification", Status = StepsItemStatus.Finish, Icon = new SolutionOutlined() });
        steps.Items.Add(new StepsItem { Header = "Pay", Status = StepsItemStatus.Process, Icon = new LoadingOutlined() });
        steps.Items.Add(new StepsItem { Header = "Done", Status = StepsItemStatus.Wait, Icon = new SmileOutlined() });
        return steps;
    }

    private static Control CreateStepsGalleryShape()
    {
        var root = new StackPanel
        {
            Spacing = 12
        };

        root.Children.Add(CreateSteps());
        root.Children.Add(CreateSteps(currentStep: 1, orientation: Orientation.Vertical));
        root.Children.Add(CreateIconSteps());
        root.Children.Add(CreateSteps(count: 4, indicatorType: StepsItemIndicatorType.Dot, currentStep: 1));
        root.Children.Add(CreateSteps(count: 4, indicatorType: StepsItemIndicatorType.Dot, orientation: Orientation.Vertical, currentStep: 1));
        root.Children.Add(CreateSteps(count: 4, style: StepsStyle.Navigation, isClickable: true));
        root.Children.Add(CreateSteps(count: 4, style: StepsStyle.Navigation, orientation: Orientation.Vertical, isClickable: true));
        root.Children.Add(CreateSteps(currentStep: 1, progressValue: 60, isShowProgress: true));
        var verticalLabelSteps = CreateSteps(currentStep: 1, progressValue: 45, isShowProgress: true);
        verticalLabelSteps.LabelPlacement = Orientation.Vertical;
        root.Children.Add(verticalLabelSteps);
        root.Children.Add(CreateSteps(style: StepsStyle.Inline, currentStep: 1));
        root.Children.Add(new AtomTextBlock { Text = "Steps gallery shape sentinel" });

        return root;
    }
}
