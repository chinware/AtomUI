using System.Reactive.Disposables;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Card;

public partial class CardShowCase : GalleryReactiveUserControl<CardViewModel>
{
    public const string LanguageId = nameof(CardShowCase);

    private const string BasicScenario    = "Basic";
    private const string LayoutScenario   = "Layout";
    private const string AdvancedScenario = "Advanced";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public CardShowCase()
    {
        this.WhenActivated(disposables =>
        {
            var application = Application.Current;
            if (application != null)
            {
                application.ActualThemeVariantChanged += HandleActualThemeVariantChanged;
                disposables.Add(Disposable.Create(() => application.ActualThemeVariantChanged -= HandleActualThemeVariantChanged ));
            }
        });
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
        EnsureSelectedScenarioContent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        ConfigureBorderlessBgFrame();
        foreach (var content in _scenarioCache.Values)
        {
            content.DataContext = DataContext;
        }
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not AtomUI.Desktop.Controls.TabItem tabItem ||
            tabItem.Tag is not string scenario)
        {
            return;
        }

        if (!_scenarioCache.TryGetValue(scenario, out var content))
        {
            content             = CreateScenarioContent(scenario);
            content.DataContext = DataContext;
            _scenarioCache.Add(scenario, content);
        }

        if (tabItem.Content != content)
        {
            tabItem.Content = content;
        }
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            BasicScenario    => new CardBasicShowCase(),
            LayoutScenario   => new CardLayoutShowCase(),
            AdvancedScenario => new CardAdvancedShowCase(),
            _                => throw new InvalidOperationException($"Unknown Card scenario: {scenario}")
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
