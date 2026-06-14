using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Descriptions;

public partial class DescriptionsShowCase : GalleryReactiveUserControl<DescriptionsViewModel>
{
    public const string LanguageId = nameof(DescriptionsShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public DescriptionsShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);

        this.WhenActivated(disposables =>
        {
            if (DataContext is DescriptionsViewModel viewModel)
            {
                viewModel.DescriptionsSizeType = SizeType.Large;
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
            ApiScenario         => new DescriptionsApiDataGrid(),
            DesignTokenScenario => new DescriptionsDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Descriptions scenario: {scenario}")
        };
    }

    private void SizeTypeCheckChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is AtomUIRadioButton { IsChecked: true, Tag: SizeType sizeType })
        {
            if (DataContext is DescriptionsViewModel viewModel)
            {
                viewModel.DescriptionsSizeType = sizeType;
            }
        }
    }
}
