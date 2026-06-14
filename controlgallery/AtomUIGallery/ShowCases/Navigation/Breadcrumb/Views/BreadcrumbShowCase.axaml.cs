using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Breadcrumb;

public partial class BreadcrumbShowCase : GalleryReactiveUserControl<BreadcrumbViewModel>
{
    public const string LanguageId = nameof(BreadcrumbShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private WindowMessageManager? _messageManager;
    private readonly GalleryShowCaseScenarioController _scenarioController;

    public BreadcrumbShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
        this.WhenActivated(disposables =>
        {
            if (DataContext is BreadcrumbViewModel viewModel)
            {
                viewModel.BreadcrumbItems = [
                    new BreadcrumbItemData()
                    {
                        Separator = ":",
                        Content = "Location"
                    },
                    new BreadcrumbItemData()
                    {
                        NavigateContext = "#",
                        Content = "Application Center"
                    },
                    new BreadcrumbItemData()
                    {
                        NavigateContext = "#",
                        Content         = "Application List"
                    },
                    new BreadcrumbItemData()
                    {
                        Content         = "An Application"
                    }
                ];

                Disposable.Create(() =>
                {
                    viewModel.BreadcrumbItems = null;
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
        _messageManager?.Dispose();
        _messageManager = null;
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
            ApiScenario         => new BreadcrumbApiDataGrid(),
            DesignTokenScenario => new BreadcrumbDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Breadcrumb scenario: {scenario}")
        };
    }

    private void HandleNavigateRequest(object? sender, BreadcrumbNavigateEventArgs eventArgs)
    {
        GetMessageManager()?.Show(new AtomUIMessage(
            $"Navigate context: {eventArgs.BreadcrumbItem.NavigateContext}"
        ));
    }

    private WindowMessageManager? GetMessageManager()
    {
        if (_messageManager is not null)
        {
            return _messageManager;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return null;
        }

        _messageManager = new WindowMessageManager(topLevel)
        {
            MaxItems = 10
        };
        return _messageManager;
    }
}
