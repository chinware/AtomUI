using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.FlexPanel;

public partial class FlexPanelShowCase : GalleryReactiveUserControl<FlexPanelViewModel>
{
    public const string LanguageId = nameof(FlexPanelShowCase);

    private const string BasicScenario       = "Basic";
    private const string AlignmentScenario   = "Alignment";
    private const string ItemScenario        = "Item";
    private const string CombinationScenario = "Combination";
    private const string PlaygroundScenario  = "Playground";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public FlexPanelShowCase()
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
            BasicScenario       => new FlexPanelBasicShowCase(),
            AlignmentScenario   => new FlexPanelAlignmentShowCase(),
            ItemScenario        => new FlexPanelItemShowCase(),
            CombinationScenario => new FlexPanelCombinationShowCase(),
            PlaygroundScenario  => new FlexPanelPlaygroundShowCase(),
            _                   => throw new InvalidOperationException($"Unknown FlexPanel scenario: {scenario}")
        };
    }
}
