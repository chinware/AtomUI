using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUI.Desktop.Controls;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

public partial class ImagePreviewerShowCase : GalleryReactiveUserControl<ImagePreviewerViewModel>
{
    public const string LanguageId = nameof(ImagePreviewerShowCase);

    public ImagePreviewerShowCase()
    {
        this.WhenActivated(disposables =>
        {
            if (DataContext is ImagePreviewerViewModel viewModel)
            {
                viewModel.DefaultImages = [
                    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png"
                ];
                viewModel.ThreeImages = [
                    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/4.webp",
                    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/5.webp",
                    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/6.webp"
                ];
                viewModel.TwoImages = [
                    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/2.svg",
                    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/3.svg",
                ];
                viewModel.FallbackImage = "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/Fallback.png";
                viewModel.BlurImage = "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/Blur.png";

                GalleryBindingUtils.OneWay(viewModel, nameof(ImagePreviewerViewModel.DefaultImages),
                                           vm => vm.DefaultImages, BasicPreviewer,
                                           AbstractImagePreviewer.ItemsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(ImagePreviewerViewModel.FallbackImage),
                                           vm => vm.FallbackImage, FaultTolerantPreviewer,
                                           AbstractImagePreviewer.FallbackImageSrcProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(ImagePreviewerViewModel.ThreeImages),
                                           vm => vm.ThreeImages, FromOnImagePreviewer,
                                           AbstractImagePreviewer.ItemsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(ImagePreviewerViewModel.DefaultImages),
                                           vm => vm.DefaultImages, CustomImagePreviewer,
                                           AbstractImagePreviewer.ItemsSourceProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(ImagePreviewerViewModel.BlurImage),
                                           vm => vm.BlurImage, CustomImagePreviewer,
                                           AtomUI.Desktop.Controls.ImagePreviewer.CoverImageSrcProperty)
                                   .DisposeWith(disposables);
                GalleryBindingUtils.OneWay(viewModel, nameof(ImagePreviewerViewModel.TwoImages),
                                           vm => vm.TwoImages, MultiImagesPreviewer,
                                           AbstractImagePreviewer.ItemsSourceProperty)
                                   .DisposeWith(disposables);

                Disposable.Create(() =>
                {
                    viewModel.DefaultImages = null;
                    viewModel.ThreeImages   = null;
                    viewModel.TwoImages     = null;
                    viewModel.FallbackImage = null;
                    viewModel.BlurImage     = null;
                }).DisposeWith(disposables);
            }
        });
        InitializeComponent();
    }
}
