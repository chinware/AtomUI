
using Avalonia;
using Avalonia.Controls;
using TabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.DropdownButton;

public partial class DropdownButtonShowCase : GalleryReactiveUserControl<DropdownButtonViewModel>
{
    public const string LanguageId = nameof(DropdownButtonShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public DropdownButtonShowCase()
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
            ApiScenario         => new DropdownButtonApiDataGrid(),
            DesignTokenScenario => new DropdownButtonDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown DropdownButton scenario: {scenario}")
        };
    }
}
