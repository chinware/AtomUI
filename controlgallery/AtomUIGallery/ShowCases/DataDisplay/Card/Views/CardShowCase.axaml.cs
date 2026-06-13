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

    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public CardShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;

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
        EnsureSelectedScenarioContent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearLazyScenarioContent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        ConfigureBorderlessBgFrame();
        ExamplesContent.DataContext = DataContext;
        foreach (var content in _lazyScenarioContentCache.Values)
        {
            content.DataContext = DataContext;
        }
        EnsureSelectedScenarioContent();
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not TabStripItem tabStripItem ||
            tabStripItem.Tag is not string scenario)
        {
            return;
        }

        var content = ResolveScenarioContent(scenario);
        if (!ReferenceEquals(ScenarioContentHost.Content, content))
        {
            ScenarioContentHost.Content = content;
        }
    }

    private void ClearLazyScenarioContent()
    {
        if (ScenarioContentHost.Content is not null &&
            !ReferenceEquals(ScenarioContentHost.Content, ExamplesContent))
        {
            ScenarioContentHost.Content = null;
        }
        _lazyScenarioContentCache.Clear();
    }

    private Control ResolveScenarioContent(string scenario)
    {
        if (scenario == ExamplesScenario)
        {
            ExamplesContent.DataContext = DataContext;
            return ExamplesContent;
        }

        if (!_lazyScenarioContentCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _lazyScenarioContentCache.Add(scenario, content);
        }

        return content;
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
