
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Statistic;

public partial class StatisticShowCase : GalleryReactiveUserControl<StatisticViewModel>
{
    public const string LanguageId = nameof(StatisticShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public StatisticShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);

        this.WhenActivated(disposables =>
        {
            if (DataContext is StatisticViewModel viewModel)
            {
                viewModel.Deadline = DateTime.Now.Add(TimeSpan.FromSeconds(60 * 60 * 24 * 2 + 30));
                viewModel.Before = DateTime.Now.Subtract(TimeSpan.FromSeconds(60 * 60 * 24 * 2 + 30));
                viewModel.TenSecondsLater = DateTime.Now.AddSeconds(10);
            }
        });
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
            ApiScenario         => new StatisticApiDataGrid(),
            DesignTokenScenario => new StatisticDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Statistic scenario: {scenario}")
        };
    }
}
