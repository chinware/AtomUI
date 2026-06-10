using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.ProgressBar;

public partial class ProgressBarShowCase : GalleryReactiveUserControl<ProgressBarViewModel>
{
    public const string LanguageId = nameof(ProgressBarShowCase);

    private const string BasicScenario    = "Basic";
    private const string AdvancedScenario = "Advanced";
    private const string LayoutScenario   = "Layout";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public ProgressBarShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
        EnsureSelectedScenarioContent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
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
            BasicScenario    => new ProgressBarBasicShowCase(),
            AdvancedScenario => new ProgressBarAdvancedShowCase(),
            LayoutScenario   => new ProgressBarLayoutShowCase(),
            _                => throw new InvalidOperationException($"Unknown ProgressBar scenario: {scenario}")
        };
    }
}
