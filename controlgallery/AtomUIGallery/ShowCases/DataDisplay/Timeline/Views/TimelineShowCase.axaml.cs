using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Timeline;

public partial class TimelineShowCase : GalleryReactiveUserControl<TimelineViewModel>
{
    public const string LanguageId = nameof(TimelineShowCase);
    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public TimelineShowCase()
    {
        InitializeComponent();
        ScenarioTabs.SelectionChanged += HandleScenarioSelectionChanged;

        this.WhenActivated(disposables =>
        {
            ModeLeft.IsCheckedChanged      += ModeChecked;
            ModeRight.IsCheckedChanged     += ModeChecked;
            ModeAlternate.IsCheckedChanged += ModeChecked;
            ReverseButton.Click            += ReverseButtonClick;

            Disposable.Create(() =>
            {
                ModeLeft.IsCheckedChanged      -= ModeChecked;
                ModeRight.IsCheckedChanged     -= ModeChecked;
                ModeAlternate.IsCheckedChanged -= ModeChecked;
                ReverseButton.Click            -= ReverseButtonClick;
            }).DisposeWith(disposables);
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
            ApiScenario         => new TimelineApiDataGrid(),
            DesignTokenScenario => new TimelineDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Timeline scenario: {scenario}")
        };
    }

    private void ReverseButtonClick(object? sender, RoutedEventArgs e)
    {
        ReverseTimeline.IsReverse = !ReverseTimeline.IsReverse;
    }

    private void ModeChecked(object? sender, RoutedEventArgs e)
    {
        if (sender is AtomUIRadioButton radioButton)
        {
            if (radioButton == ModeLeft && ModeLeft.IsChecked == true)
            {
                LabelTimeline.Mode = TimelineMode.Left;
            }
            else if (radioButton == ModeRight && ModeRight.IsChecked == true)
            {
                LabelTimeline.Mode = TimelineMode.Right;
            }
            else if (radioButton == ModeAlternate && ModeAlternate.IsChecked == true)
            {
                LabelTimeline.Mode = TimelineMode.Alternate;
            }
        }
    }
}
