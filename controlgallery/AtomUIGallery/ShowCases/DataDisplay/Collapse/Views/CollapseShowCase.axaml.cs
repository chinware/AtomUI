using Avalonia.Controls;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Collapse;

public partial class CollapseShowCase : GalleryReactiveUserControl<CollapseViewModel>
{
    public const string LanguageId = nameof(CollapseShowCase);

    private const string BasicScenario      = "Basic";
    private const string AppearanceScenario = "Appearance";
    private const string BehaviorScenario   = "Behavior";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public CollapseShowCase()
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
            BasicScenario      => new CollapseBasicShowCase(),
            AppearanceScenario => new CollapseAppearanceShowCase(),
            BehaviorScenario   => new CollapseBehaviorShowCase(),
            _                  => throw new InvalidOperationException($"Unknown Collapse scenario: {scenario}")
        };
    }
}
