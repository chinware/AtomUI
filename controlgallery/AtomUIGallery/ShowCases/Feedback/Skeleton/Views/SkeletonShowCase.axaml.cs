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
    private const string ApiScenario         = "Api";
    private const string DesignTokenScenario = "DesignToken";

    private readonly GalleryShowCaseScenarioController _scenarioController;

    public SkeletonShowCase()
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
