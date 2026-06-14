using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

public partial class ImagePreviewerShowCase : GalleryReactiveUserControl<ImagePreviewerViewModel>
{
    public const string LanguageId = nameof(ImagePreviewerShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;
    private ImagePreviewerViewModel? _activeViewModel;

    public ImagePreviewerShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        EnsurePreviewAssets();
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _scenarioController.Detach();
        ClearPreviewAssets();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        EnsurePreviewAssets();
        _scenarioController.UpdateDataContext(DataContext);
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new ImagePreviewerApiDataGrid(),
            DesignTokenScenario => new ImagePreviewerDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown ImagePreviewer scenario: {scenario}")
        };
    }

    private void EnsurePreviewAssets()
    {
        if (_activeViewModel is not null &&
            !ReferenceEquals(_activeViewModel, DataContext))
        {
            _activeViewModel.ClearPreviewAssets();
            _activeViewModel = null;
        }

        if (DataContext is ImagePreviewerViewModel viewModel)
        {
            _activeViewModel = viewModel;
            viewModel.EnsurePreviewAssets();
        }
    }

    private void ClearPreviewAssets()
    {
        _activeViewModel?.ClearPreviewAssets();
        _activeViewModel = null;
    }
}
