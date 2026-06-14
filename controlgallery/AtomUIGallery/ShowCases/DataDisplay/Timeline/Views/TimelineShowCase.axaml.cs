using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Timeline;

public partial class TimelineShowCase : GalleryReactiveUserControl<TimelineViewModel>
{
    public const string LanguageId = nameof(TimelineShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public TimelineShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _scenarioController.Detach();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        _scenarioController.UpdateDataContext(DataContext);
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new TimelineApiDataGrid(),
            DesignTokenScenario => new TimelineDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Timeline scenario: {scenario}")
        };
    }

    private void ReverseButtonClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is TimelineViewModel viewModel)
        {
            viewModel.ReverseTimelineIsReverse = !viewModel.ReverseTimelineIsReverse;
        }
    }

    private void ModeChecked(object? sender, RoutedEventArgs e)
    {
        if (sender is AtomUIRadioButton { IsChecked: true, Tag: TimelineMode mode } &&
            DataContext is TimelineViewModel viewModel)
        {
            viewModel.SelectedTimelineMode = mode;
        }
    }
}
