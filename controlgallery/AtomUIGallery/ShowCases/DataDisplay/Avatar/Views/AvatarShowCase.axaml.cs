using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Avatar;

public partial class AvatarShowCase : GalleryReactiveUserControl<AvatarViewModel>
{
    public const string LanguageId = nameof(AvatarShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public AvatarShowCase()
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
            ApiScenario         => new AvatarApiDataGrid(),
            DesignTokenScenario => new AvatarDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Avatar scenario: {scenario}")
        };
    }
}
