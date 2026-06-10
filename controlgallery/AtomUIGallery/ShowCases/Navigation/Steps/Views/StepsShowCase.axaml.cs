using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.Steps;

public partial class StepsShowCase : GalleryReactiveUserControl<StepsViewModel>
{
    public const string LanguageId = nameof(StepsShowCase);

    private const string BasicScenario        = "Basic";
    private const string InteractiveScenario  = "Interactive";
    private const string VerticalScenario     = "Vertical";
    private const string DotClickableScenario = "DotClickable";
    private const string NavigationScenario   = "Navigation";
    private const string ProgressScenario     = "Progress";
    private const string InlineScenario       = "Inline";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public StepsShowCase()
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
            BasicScenario        => new StepsBasicShowCase(),
            InteractiveScenario  => new StepsInteractiveShowCase(),
            VerticalScenario     => new StepsVerticalShowCase(),
            DotClickableScenario => new StepsDotClickableShowCase(),
            NavigationScenario   => new StepsNavigationShowCase(),
            ProgressScenario     => new StepsProgressShowCase(),
            InlineScenario       => new StepsInlineShowCase(),
            _                    => throw new InvalidOperationException($"Unknown Steps scenario: {scenario}")
        };
    }
}
