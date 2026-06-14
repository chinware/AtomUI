using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.QRCode;

public partial class QRCodeShowCase : GalleryReactiveUserControl<QRCodeViewModel>
{
    public const string LanguageId = nameof(QRCodeShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public QRCodeShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);

        this.WhenActivated(disposables =>
        {
            if (DataContext is QRCodeViewModel viewModel)
            {
                viewModel.EccLevels = new List<string>
                {
                    nameof(QRCodeEccLevel.L),
                    nameof(QRCodeEccLevel.M),
                    nameof(QRCodeEccLevel.Q),
                    nameof(QRCodeEccLevel.H)
                };
                Disposable.Create(() =>
                {
                    viewModel.EccLevels = null;
                }).DisposeWith(disposables);
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
            ApiScenario         => new QRCodeApiDataGrid(),
            DesignTokenScenario => new QRCodeDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown QRCode scenario: {scenario}")
        };
    }
}
