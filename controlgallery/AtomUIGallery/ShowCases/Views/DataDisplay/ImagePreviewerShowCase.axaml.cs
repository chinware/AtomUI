using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using AtomUIGallery.ShowCases.ViewModels;
using ReactiveUI;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Views;

public partial class ImagePreviewerShowCase : ReactiveUserControl<ImagePreviewerViewModel>
{
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

                this.OneWayBind(viewModel, vm => vm.DefaultImages, v => v.BasicPreviewer.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.FallbackImage, v => v.FaultTolerantPreviewer.FallbackImageSrc)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.ThreeImages, v => v.FromOnImagePreviewer.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.DefaultImages, v => v.CustomImagePreviewer.ItemsSource)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.BlurImage, v => v.CustomImagePreviewer.CoverImageSrc)
                    .DisposeWith(disposables);
                this.OneWayBind(viewModel, vm => vm.TwoImages, v => v.MultiImagesPreviewer.ItemsSource)
                    .DisposeWith(disposables);

                Disposable.Create(() =>
                {
                    viewModel.DefaultImages = null;
                    viewModel.ThreeImages   = null;
                    viewModel.TwoImages     = null;
                    viewModel.FallbackImage = null;
                    viewModel.BlurImage     = null;
                });
            }
        });
        InitializeComponent();
    }
}
