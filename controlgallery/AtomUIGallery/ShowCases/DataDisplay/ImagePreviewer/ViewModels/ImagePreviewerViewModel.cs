using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

public class ImagePreviewerViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "ImagePreviewer";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private IImagePreviewSource? _remoteImage;

    public IImagePreviewSource? RemoteImage
    {
        get => _remoteImage;
        set => this.RaiseAndSetIfChanged(ref _remoteImage, value);
    }

    private IList<IImagePreviewSource>? _defaultImages;

    public IList<IImagePreviewSource>? DefaultImages
    {
        get => _defaultImages;
        set => this.RaiseAndSetIfChanged(ref _defaultImages, value);
    }

    private IList<IImagePreviewSource>? _twoImages;

    public IList<IImagePreviewSource>? TwoImages
    {
        get => _twoImages;
        set => this.RaiseAndSetIfChanged(ref _twoImages, value);
    }

    private IList<IImagePreviewSource>? _threeImages;

    public IList<IImagePreviewSource>? ThreeImages
    {
        get => _threeImages;
        set => this.RaiseAndSetIfChanged(ref _threeImages, value);
    }

    private IList<IImagePreviewSource>? _twentyRemoteImages;

    public IList<IImagePreviewSource>? TwentyRemoteImages
    {
        get => _twentyRemoteImages;
        set => this.RaiseAndSetIfChanged(ref _twentyRemoteImages, value);
    }

    private IImagePreviewSource? _fallbackImage;

    public IImagePreviewSource? FallbackImage
    {
        get => _fallbackImage;
        set => this.RaiseAndSetIfChanged(ref _fallbackImage, value);
    }

    public ImagePreviewerViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void EnsurePreviewAssets()
    {
        RemoteImage = new UriImagePreviewSource("https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png");
        DefaultImages =
        [
            new UriImagePreviewSource("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png")
        ];
        ThreeImages =
        [
            new UriImagePreviewSource("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/4.webp"),
            new UriImagePreviewSource("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/5.webp"),
            new UriImagePreviewSource("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/6.webp")
        ];
        TwoImages =
        [
            new UriImagePreviewSource("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/2.svg"),
            new UriImagePreviewSource("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/3.svg"),
        ];
        TwentyRemoteImages =
        [
            new UriImagePreviewSource("https://picsum.photos/id/20/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/21/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/22/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/23/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/24/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/25/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/26/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/27/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/28/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/29/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/30/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/31/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/32/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/33/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/34/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/35/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/36/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/37/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/38/600/400"),
            new UriImagePreviewSource("https://picsum.photos/id/39/600/400")
        ];
        FallbackImage = new UriImagePreviewSource("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/Fallback.png");
    }

    public void ClearPreviewAssets()
    {
        RemoteImage        = null;
        DefaultImages      = null;
        ThreeImages        = null;
        TwoImages          = null;
        TwentyRemoteImages = null;
        FallbackImage      = null;
    }

}
