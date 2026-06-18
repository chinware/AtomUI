using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUIGallery.ShowCases.Masonry;

public partial class MasonryShowCase : GalleryReactiveUserControl<MasonryViewModel>
{
    public const string LanguageId = nameof(MasonryShowCase);
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public MasonryShowCase()
    {
        InitializeComponent();
        _scenarioController = new GalleryShowCaseScenarioController(ScenarioTabs, ScenarioContentHost, CreateScenarioContent, ExamplesContent);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _scenarioController.Attach(DataContext);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _scenarioController.Detach();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        _scenarioController.UpdateDataContext(DataContext);
    }

    private void HandleDynamicMasonryLayoutChanged(object? sender, MasonryLayoutChangedEventArgs e)
    {
        ViewModel?.UpdateDynamicMasonryColumns(e.Items);
    }

    private void HandleRemoveDynamicMasonryItemClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Control { DataContext: MasonryDynamicItem item })
        {
            ViewModel?.RemoveDynamicMasonryItem(item.Key);
        }
    }

    private static Control CreateScenarioContent(string scenario)
    {
        return scenario switch
        {
            ApiScenario         => new MasonryApiDataGrid(),
            DesignTokenScenario => new MasonryDesignTokenDataGrid(),
            _                   => throw new InvalidOperationException($"Unknown Masonry scenario: {scenario}")
        };
    }
}
