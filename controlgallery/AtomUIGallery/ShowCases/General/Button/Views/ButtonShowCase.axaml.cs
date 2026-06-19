using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AtomUIGallery.ShowCases.Button;

public partial class ButtonShowCase : GalleryReactiveUserControl<ButtonViewModel>
{
    public const string LanguageId = nameof(ButtonShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;
    private ButtonViewModel? _viewModel;
    public ButtonShowCase()
    {
        this.WhenActivated(disposables =>
        {
            _viewModel = DataContext as ButtonViewModel;
        });
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
        _viewModel                   = DataContext as ButtonViewModel;
        _scenarioController.UpdateDataContext(DataContext);
    }

    public void HandleButtonSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (_viewModel != null)
        {
            var sizeType = args.Index switch
            {
                0 => CustomizableSizeType.Large,
                1 => CustomizableSizeType.Middle,
                2 => CustomizableSizeType.Small,
                3 => CustomizableSizeType.Custom,
                _ => _viewModel.ButtonSizeType
            };
            _viewModel.ButtonSizeType = sizeType;
        }
    }

    public void HandleLoadingBtnClick(object? sender, RoutedEventArgs args)
    {
        if (sender is AtomUIButton button)
        {
            button.IsLoading = true;
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                button.IsLoading = false;
            });
        }
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new ButtonApiDataGrid(),
            DesignTokenScenario => new ButtonDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Button scenario: {scenario}")
        };
    }
}
