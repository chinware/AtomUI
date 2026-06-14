using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Theme.Language;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ScenarioTabStripItem = AtomUI.Desktop.Controls.TabStripItem;

namespace AtomUIGallery.ShowCases.Steps;

public partial class StepsShowCase : GalleryReactiveUserControl<StepsViewModel>
{
    public const string LanguageId = nameof(StepsShowCase);

    public static readonly StyledProperty<double[]> DashedArrayProperty =
        AvaloniaProperty.Register<StepsShowCase, double[]>(nameof(DashedArray));

    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public double[] DashedArray
    {
        get => GetValue(DashedArrayProperty);
        set => SetValue(DashedArrayProperty, value);
    }

    public StepsShowCase()
    {
        InitializeComponent();
        DashedArray = [4d, 3d];
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;

        this.WhenActivated(disposables =>
        {
            ResetInteractiveState();

            var themeManager = Application.Current?.GetThemeManager();
            if (themeManager != null)
            {
                EventHandler<LanguageVariantChangedEventArgs> handler = (_, _) => RefreshInteractiveButtonText();
                themeManager.LanguageVariantChanged += handler;
                Disposable.Create(() => themeManager.LanguageVariantChanged -= handler)
                          .DisposeWith(disposables);
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

        ResetInteractiveState();
        EnsureSelectedScenarioContent();
    }

    public void HandleNextButtonClick(object? sender, RoutedEventArgs args)
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.MoveToNextInteractiveStep();
        }
    }

    public void HandlePreviousButtonClick(object? sender, RoutedEventArgs args)
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.MoveToPreviousInteractiveStep();
        }
    }

    private void HandleScenarioSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        EnsureSelectedScenarioContent();
    }

    private void EnsureSelectedScenarioContent()
    {
        if (ScenarioTabs.SelectedItem is not ScenarioTabStripItem tabStripItem ||
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
            ApiScenario         => new StepsApiDataGrid(),
            DesignTokenScenario => new StepsDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Steps scenario: {scenario}")
        };
    }

    private void ResetInteractiveState()
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.ResetInteractiveStep();
        }
    }

    private void RefreshInteractiveButtonText()
    {
        if (DataContext is StepsViewModel viewModel)
        {
            viewModel.RefreshInteractiveButtonText();
        }
    }
}
