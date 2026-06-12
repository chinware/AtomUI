using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AtomUIGallery.ShowCases.Button;

public partial class ButtonShowCase : GalleryReactiveUserControl<ButtonViewModel>
{
    public const string LanguageId = nameof(ButtonShowCase);
    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);
    private ButtonViewModel? _viewModel;
    public ButtonShowCase()
    {
        this.WhenActivated(disposables =>
        {
            _viewModel = DataContext as ButtonViewModel;
        });
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;
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
        _viewModel                   = DataContext as ButtonViewModel;
        ExamplesContent.DataContext  = DataContext;
        foreach (var content in _lazyScenarioContentCache.Values)
        {
            content.DataContext = DataContext;
        }
        EnsureSelectedScenarioContent();
    }

    public void HandleButtonSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (_viewModel != null)
        {
            if (args.Index == 0)
            {
                _viewModel.ButtonSizeType = SizeType.Large;
            }
            else if (args.Index == 1)
            {
                _viewModel.ButtonSizeType = SizeType.Middle;
            }
            else
            {
                _viewModel.ButtonSizeType = SizeType.Small;
            }
        }
    }

    public void HandleLoadingBtnClick(object? sender, RoutedEventArgs args)
    {
        if (sender is AtomUIButton button)
        {
            button.IsLoading = true;
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                button.IsLoading = false;
            });
        }
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
            ApiScenario         => new ButtonApiDataGrid(),
            DesignTokenScenario => new ButtonDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Button scenario: {scenario}")
        };
    }
}
