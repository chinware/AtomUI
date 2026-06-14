
using Avalonia;
using Avalonia.Controls;
using AtomUI.Desktop.Controls;

namespace AtomUIGallery.ShowCases.Empty;

public partial class EmptyShowCase : GalleryReactiveUserControl<EmptyViewModel>
{
    public const string LanguageId = nameof(EmptyShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public EmptyShowCase()
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
            ApiScenario         => new EmptyApiDataGrid(),
            DesignTokenScenario => new EmptyDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Empty scenario: {scenario}")
        };
    }
}
