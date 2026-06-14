using System.Reactive.Disposables;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using TabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.Card;

public partial class CardShowCase : GalleryReactiveUserControl<CardViewModel>
{
    public const string LanguageId = nameof(CardShowCase);

    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public CardShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);

        this.WhenActivated(disposables =>
        {
            var application = Application.Current;
            if (application != null)
            {
                application.ActualThemeVariantChanged += HandleActualThemeVariantChanged;
                disposables.Add(Disposable.Create(() => application.ActualThemeVariantChanged -= HandleActualThemeVariantChanged ));
            }
        });
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ConfigureBorderlessBgFrame();
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
        ConfigureBorderlessBgFrame();
        _scenarioController.UpdateDataContext(DataContext);
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new CardApiDataGrid(),
            DesignTokenScenario => new CardDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Card scenario: {scenario}")
        };
    }

    private void HandleActualThemeVariantChanged(object? sender, EventArgs e)
    {
        ConfigureBorderlessBgFrame();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        ConfigureBorderlessBgFrame();
    }

    private void ConfigureBorderlessBgFrame()
    {
        var application = Application.Current;
        if (application != null)
        {
            if (DataContext is CardViewModel cardViewModel)
            {
                if (application.IsDarkThemeMode())
                {
                    cardViewModel.BorderlessFrameBg = new SolidColorBrush(Color.FromRgb(48, 48, 48));
                }
                else
                {
                    cardViewModel.BorderlessFrameBg = new SolidColorBrush(Color.FromRgb(240, 242, 245));
                }
            }
        }
    }
}
