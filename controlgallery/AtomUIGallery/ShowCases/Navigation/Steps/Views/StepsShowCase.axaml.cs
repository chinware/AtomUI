using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ScenarioTabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.Steps;

public partial class StepsShowCase : GalleryReactiveUserControl<StepsViewModel>
{
    public const string LanguageId = nameof(StepsShowCase);

    public static readonly StyledProperty<double[]> DashedArrayProperty =
        AvaloniaProperty.Register<StepsShowCase, double[]>(nameof(DashedArray));

    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public double[] DashedArray
    {
        get => GetValue(DashedArrayProperty);
        set => SetValue(DashedArrayProperty, value);
    }

    public StepsShowCase()
    {
        InitializeComponent();
        DashedArray = [4d, 3d];
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);

        this.WhenActivated(disposables =>
        {
            ResetInteractiveState();

            var themeManager = Application.Current?.GetThemeManager();
            if (themeManager != null)
            {
                EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshInteractiveButtonText();
                themeManager.LanguageVariantChanged += handler;
                Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                          .DisposeWith(disposables);
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

        ResetInteractiveState();
        _scenarioController.UpdateDataContext(DataContext);
    }

    public void HandleNextButtonClick(object? sender, RoutedEventArgs args)
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.MoveToNextInteractiveStep();
        }
    }

    public void HandlePreviousButtonClick(object? sender, RoutedEventArgs args)
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.MoveToPreviousInteractiveStep();
        }
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new StepsApiDataGrid(),
            DesignTokenScenario => new StepsDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Steps scenario: {scenario}")
        };
    }

    private void ResetInteractiveState()
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.ResetInteractiveStep();
        }
    }

    private void RefreshInteractiveButtonText()
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.RefreshInteractiveButtonText();
        }
    }
}
