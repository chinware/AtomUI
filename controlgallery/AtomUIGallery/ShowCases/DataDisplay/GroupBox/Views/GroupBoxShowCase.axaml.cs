
using Avalonia;
using Avalonia.Controls;
using AtomUI.Desktop.Controls;

namespace AtomUIGallery.ShowCases.GroupBox;

public partial class GroupBoxShowCase : GalleryReactiveUserControl<GroupBoxViewModel>
{
    public const string LanguageId = nameof(GroupBoxShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public GroupBoxShowCase()
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
            ApiScenario         => new GroupBoxApiDataGrid(),
            DesignTokenScenario => new GroupBoxDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown GroupBox scenario: {scenario}")
        };
    }
}
