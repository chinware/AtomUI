using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace AtomUIGallery.ShowCases.Skeleton;

public partial class SkeletonShowCase : GalleryReactiveUserControl<SkeletonViewModel>
{
    public const string LanguageId = nameof(SkeletonShowCase);
    private const string ExamplesScenario    = "Examples";
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly Dictionary<string, Control> _lazyScenarioContentCache = new(StringComparer.Ordinal);

    public SkeletonShowCase()
    {
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
            ApiScenario         => new SkeletonApiDataGrid(),
            DesignTokenScenario => new SkeletonDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Skeleton scenario: {scenario}")
        };
    }

    private void HandleSizeTypeChanged(object? sender, OptionCheckedChangedEventArgs e)
    {
        if (DataContext is SkeletonViewModel viewModel)
        {
            if (e.Index == 0)
            {
                viewModel.SkeletonButtonAndInputSizeType = CustomizableSizeType.Middle;
            }
            else if (e.Index == 1)
            {
                viewModel.SkeletonButtonAndInputSizeType = CustomizableSizeType.Large;
            }
            else if (e.Index == 2)
            {
                viewModel.SkeletonButtonAndInputSizeType = CustomizableSizeType.Small;
            }
        }
    }

    private void HandleButtonShapeChanged(object? sender, OptionCheckedChangedEventArgs e)
    {
        if (DataContext is SkeletonViewModel viewModel)
        {
            if (e.Index == 0)
            {
                viewModel.SkeletonButtonShape = SkeletonButtonShape.Square;
            }
            else if (e.Index == 1)
            {
                viewModel.SkeletonButtonShape = SkeletonButtonShape.Round;
            }
            else if (e.Index == 2)
            {
                viewModel.SkeletonButtonShape = SkeletonButtonShape.Circle;
            }
        }
    }

    private void HandleButtonAvatarChanged(object? sender, OptionCheckedChangedEventArgs e)
    {
        if (DataContext is SkeletonViewModel viewModel)
        {
            if (e.Index == 0)
            {
                viewModel.SkeletonAvatarShape = AvatarShape.Square;
            }
            else if (e.Index == 1)
            {
                viewModel.SkeletonAvatarShape = AvatarShape.Circle;
            }
        }
    }

    private void HandleLoadingButtonClicked(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SkeletonViewModel viewModel)
        {
            viewModel.SkeletonLoading = !viewModel.SkeletonLoading;
            DispatcherTimer.RunOnce(() =>
            {
                viewModel.SkeletonLoading = !viewModel.SkeletonLoading;
            }, TimeSpan.FromSeconds(3));
        }
    }
}
