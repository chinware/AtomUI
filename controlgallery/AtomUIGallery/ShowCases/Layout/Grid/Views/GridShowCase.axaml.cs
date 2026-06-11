using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Grid;

public partial class GridShowCase : GalleryReactiveUserControl<GridViewModel>
{
    public const string LanguageId = nameof(GridShowCase);

    private const string BasicScenario     = "Basic";
    private const string SpacingScenario   = "Spacing";
    private const string AlignmentScenario = "Alignment";
    private const string OrderScenario     = "Order";
    private const string ColInfoScenario   = "ColInfo";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public GridShowCase()
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
            BasicScenario     => new GridBasicShowCase(),
            SpacingScenario   => new GridSpacingShowCase(),
            AlignmentScenario => new GridAlignmentShowCase(),
            OrderScenario     => new GridOrderShowCase(),
            ColInfoScenario   => new GridColInfoShowCase(),
            _                 => throw new InvalidOperationException($"Unknown Grid scenario: {scenario}")
        };
    }
}
