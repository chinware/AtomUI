using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.FloatButton;

public partial class FloatButtonShowCase : GalleryReactiveUserControl<FloatButtonViewModel>
{
    public const string LanguageId = nameof(FloatButtonShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public FloatButtonShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
        this.WhenActivated(disposables =>
        {
            if (DataContext is FloatButtonViewModel vm)
            {
                vm.IsOpened = true;
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
            ApiScenario         => new FloatButtonApiDataGrid(),
            DesignTokenScenario => new FloatButtonDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown FloatButton scenario: {scenario}")
        };
    }
}
