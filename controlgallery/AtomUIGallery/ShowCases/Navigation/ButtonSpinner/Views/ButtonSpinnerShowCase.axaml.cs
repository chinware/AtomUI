using Avalonia;
using Avalonia.Controls;
using TabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.ButtonSpinner;

public partial class ButtonSpinnerShowCase : GalleryReactiveUserControl<ButtonSpinnerViewModel>
{
    public const string LanguageId = nameof(ButtonSpinnerShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public ButtonSpinnerShowCase()
    {
        InitializeComponent();
        AddHandler(Spinner.SpinEvent, HandleSpin);
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

    private void HandleSpin(object? sender, SpinEventArgs args)
    {
        if (DataContext is ButtonSpinnerViewModel viewModel)
        {
            viewModel.HandleSpin(args.Source, args);
        }
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new ButtonSpinnerApiDataGrid(),
            DesignTokenScenario => new ButtonSpinnerDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown ButtonSpinner scenario: {scenario}")
        };
    }
}
