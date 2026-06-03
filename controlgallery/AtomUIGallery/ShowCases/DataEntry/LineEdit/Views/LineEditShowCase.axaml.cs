using Avalonia.Controls;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.LineEdit;

public partial class LineEditShowCase : ReactiveUserControl<LineEditViewModel>
{
    public const string LanguageId = nameof(LineEditShowCase);

    private const string BasicScenario    = "Basic";
    private const string StateScenario    = "State";
    private const string SearchScenario   = "Search";
    private const string TextAreaScenario = "TextArea";

    private readonly Dictionary<string, Control> _scenarioCache = new(StringComparer.Ordinal);

    public LineEditShowCase()
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
            BasicScenario    => new LineEditBasicShowCase(),
            StateScenario    => new LineEditStateShowCase(),
            SearchScenario   => new LineEditSearchShowCase(),
            TextAreaScenario => new LineEditTextAreaShowCase(),
            _                => throw new InvalidOperationException($"Unknown LineEdit scenario: {scenario}")
        };
    }
}
