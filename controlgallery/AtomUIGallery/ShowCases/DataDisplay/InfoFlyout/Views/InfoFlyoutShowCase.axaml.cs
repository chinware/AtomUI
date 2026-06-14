using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.InfoFlyout;

public partial class InfoFlyoutShowCase : GalleryReactiveUserControl<InfoFlyoutViewModel>
{
    public const string LanguageId = nameof(InfoFlyoutShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public InfoFlyoutShowCase()
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
            ApiScenario         => new InfoFlyoutApiDataGrid(),
            DesignTokenScenario => new InfoFlyoutDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown InfoFlyout scenario: {scenario}")
        };
    }

    private void HandleArrowSegmentedSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (DataContext is InfoFlyoutViewModel viewModel)
        {
            viewModel.HandleSelectionChanged(sender, args);
        }
    }
}
