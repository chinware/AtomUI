using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.SplitButton;

public partial class SplitButtonShowCase : GalleryReactiveUserControl<SplitButtonViewModel>
{
    public const string LanguageId = nameof(SplitButtonShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public SplitButtonShowCase()
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
            ApiScenario         => new SplitButtonApiDataGrid(),
            DesignTokenScenario => new SplitButtonDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown SplitButton scenario: {scenario}")
        };
    }
}
