using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUIGallery.ShowCases.AutoComplete;

public partial class AutoCompleteShowCase : GalleryReactiveUserControl<AutoCompleteViewModel>
{
    public const string LanguageId = nameof(AutoCompleteShowCase);
    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public AutoCompleteShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;

        this.WhenActivated(disposables =>
        {
            if (DataContext is AutoCompleteViewModel viewModel)
            {
                viewModel.BasicOptionsAsyncLoader       = new BasicOptionsAsyncLoader();
                viewModel.CustomLabelOptionsAsyncLoader = new CustomLabelOptionsAsyncLoader();
                viewModel.SearchEditOptionsAsyncLoader  = new SearchEditOptionsAsyncLoader();
                InitFilterCaseOptions(viewModel);
                InitCityOptions(viewModel);

                Disposable.Create(() =>
                {
                    viewModel.BasicOptionsAsyncLoader       = null;
                    viewModel.CustomLabelOptionsAsyncLoader = null;
                    viewModel.SearchEditOptionsAsyncLoader  = null;
                    viewModel.FilterCaseOptions             = null;
                    viewModel.CityOptions                   = null;
                }).DisposeWith(disposables);
            }
        });
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
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
            ApiScenario         => new AutoCompleteApiDataGrid(),
            DesignTokenScenario => new AutoCompleteDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown AutoComplete scenario: {scenario}")
        };
    }

    private void InitFilterCaseOptions(AutoCompleteViewModel vm)
    {
        vm.FilterCaseOptions =
        [
            new AutoCompleteOption()
            {
                Header  = "Burns Bay Road",
                Content = "Burns Bay Road"
            },
            new AutoCompleteOption()
            {
                Header  = "Downing Street",
                Content = "Downing Street"
            },
            new AutoCompleteOption()
            {
                Header  = "Wall Street",
                Content = "Wall Street"
            }
        ];
    }

    private void InitCityOptions(AutoCompleteViewModel vm)
    {
        vm.CityOptions =
        [
            new CityAutoCompleteOption { Header = "Amsterdam", Content = "Amsterdam", Country = "NL", Population = 905234 },
            new CityAutoCompleteOption { Header = "Auckland",  Content = "Auckland",  Country = "NZ", Population = 1657200 },
            new CityAutoCompleteOption { Header = "Beijing",   Content = "Beijing",   Country = "CN", Population = 21893095 },
            new CityAutoCompleteOption { Header = "Berlin",    Content = "Berlin",    Country = "DE", Population = 3677472 },
            new CityAutoCompleteOption { Header = "Boston",    Content = "Boston",    Country = "US", Population = 654776 },
            new CityAutoCompleteOption { Header = "Jakarta",   Content = "Jakarta",   Country = "ID", Population = 10770487 },
            new CityAutoCompleteOption { Header = "Lisbon",    Content = "Lisbon",    Country = "PT", Population = 545923 }
        ];
    }
}
