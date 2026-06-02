using Avalonia.Controls;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Space;

public partial class SpaceShowCase : ReactiveUserControl<SpaceViewModel>
{
    public const string LanguageId = nameof(SpaceShowCase);

    private const string BasicScenario         = "Basic";
    private const string SizeScenario          = "Size";
    private const string AlignScenario         = "Align";
    private const string CompactFormScenario   = "CompactForm";
    private const string CompactButtonScenario = "CompactButton";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public SpaceShowCase()
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
            BasicScenario         => new SpaceBasicShowCase(),
            SizeScenario          => new SpaceSizeShowCase(),
            AlignScenario         => new SpaceAlignShowCase(),
            CompactFormScenario   => new SpaceCompactFormShowCase(),
            CompactButtonScenario => new SpaceCompactButtonShowCase(),
            _                     => throw new InvalidOperationException($"Unknown Space scenario: {scenario}")
        };
    }
}
