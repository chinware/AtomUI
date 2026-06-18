using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.BorderBeam;

public partial class BorderBeamShowCase : GalleryReactiveUserControl<BorderBeamViewModel>
{
    public const string LanguageId = nameof(BorderBeamShowCase);

    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public BorderBeamShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(
            ScenarioTabs,
            ScenarioContentHost,
            CreateScenarioContent,
            ExamplesContent);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _scenarioController.Detach();
        base.OnDetachedFromVisualTree(e);
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
            ApiScenario         => new BorderBeamApiDataGrid(),
            DesignTokenScenario => new BorderBeamDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown BorderBeam scenario: {scenario}")
        };
    }
}
