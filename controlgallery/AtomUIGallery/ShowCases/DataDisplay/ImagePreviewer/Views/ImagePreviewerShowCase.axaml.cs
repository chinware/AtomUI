using Avalonia;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

public partial class ImagePreviewerShowCase : GalleryReactiveUserControl<ImagePreviewerViewModel>
{
    public const string LanguageId = nameof(ImagePreviewerShowCase);

    private ImagePreviewerViewModel? _activeViewModel;

    public ImagePreviewerShowCase()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        EnsurePreviewAssets();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ClearPreviewAssets();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        EnsurePreviewAssets();
    }

    private void EnsurePreviewAssets()
    {
        if (ReferenceEquals(_activeViewModel, DataContext))
        {
            return;
        }

        if (_activeViewModel is not null &&
            !ReferenceEquals(_activeViewModel, DataContext))
        {
            _activeViewModel.ClearPreviewAssets();
            _activeViewModel = null;
        }

        if (DataContext is ImagePreviewerViewModel viewModel)
        {
            _activeViewModel = viewModel;
            viewModel.EnsurePreviewAssets();
        }
    }

    private void ClearPreviewAssets()
    {
        _activeViewModel?.ClearPreviewAssets();
        _activeViewModel = null;
    }
}
