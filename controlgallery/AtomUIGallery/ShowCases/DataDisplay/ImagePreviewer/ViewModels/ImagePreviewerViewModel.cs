using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

public class ImagePreviewerViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "ImagePreviewer";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private IList<ImagePreviewItem>? _remoteImages;

    public IList<ImagePreviewItem>? RemoteImages
    {
        get => _remoteImages;
        set => this.RaiseAndSetIfChanged(ref _remoteImages, value);
    }

    private IList<ImagePreviewItem>? _defaultImages;

    public IList<ImagePreviewItem>? DefaultImages
    {
        get => _defaultImages;
        set => this.RaiseAndSetIfChanged(ref _defaultImages, value);
    }

    private IList<ImagePreviewItem>? _twoImages;

    public IList<ImagePreviewItem>? TwoImages
    {
        get => _twoImages;
        set => this.RaiseAndSetIfChanged(ref _twoImages, value);
    }

    private IList<ImagePreviewItem>? _threeImages;

    public IList<ImagePreviewItem>? ThreeImages
    {
        get => _threeImages;
        set => this.RaiseAndSetIfChanged(ref _threeImages, value);
    }

    private IList<ImagePreviewItem>? _twentyRemoteImages;

    public IList<ImagePreviewItem>? TwentyRemoteImages
    {
        get => _twentyRemoteImages;
        set => this.RaiseAndSetIfChanged(ref _twentyRemoteImages, value);
    }

    private IList<ImagePreviewItem>? _fallbackImages;

    public IList<ImagePreviewItem>? FallbackImages
    {
        get => _fallbackImages;
        set => this.RaiseAndSetIfChanged(ref _fallbackImages, value);
    }

    public ImagePreviewerViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void EnsurePreviewAssets()
    {
        RemoteImages =
        [
            CreateItem("https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png")
        ];
        DefaultImages =
        [
            CreateItem("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png")
        ];
        ThreeImages =
        [
            CreateItem("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/4.webp"),
            CreateItem("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/5.webp"),
            CreateItem("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/6.webp")
        ];
        TwoImages =
        [
            CreateItem("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/2.svg"),
            CreateItem("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/3.svg"),
        ];
        TwentyRemoteImages =
        [
            CreateItem("https://picsum.photos/id/20/600/400"),
            CreateItem("https://picsum.photos/id/21/600/400"),
            CreateItem("https://picsum.photos/id/22/600/400"),
            CreateItem("https://picsum.photos/id/23/600/400"),
            CreateItem("https://picsum.photos/id/24/600/400"),
            CreateItem("https://picsum.photos/id/25/600/400"),
            CreateItem("https://picsum.photos/id/26/600/400"),
            CreateItem("https://picsum.photos/id/27/600/400"),
            CreateItem("https://picsum.photos/id/28/600/400"),
            CreateItem("https://picsum.photos/id/29/600/400"),
            CreateItem("https://picsum.photos/id/30/600/400"),
            CreateItem("https://picsum.photos/id/31/600/400"),
            CreateItem("https://picsum.photos/id/32/600/400"),
            CreateItem("https://picsum.photos/id/33/600/400"),
            CreateItem("https://picsum.photos/id/34/600/400"),
            CreateItem("https://picsum.photos/id/35/600/400"),
            CreateItem("https://picsum.photos/id/36/600/400"),
            CreateItem("https://picsum.photos/id/37/600/400"),
            CreateItem("https://picsum.photos/id/38/600/400"),
            CreateItem("https://picsum.photos/id/39/600/400")
        ];
        FallbackImages =
        [
            new ImagePreviewItem(ImageLoadSource.FromUri("https://example.invalid/missing-image.png"))
            {
                FallbackSource = ImageLoadSource.FromUri(
                    "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/Fallback.png")
            }
        ];
    }

    public void ClearPreviewAssets()
    {
        RemoteImages       = null;
        DefaultImages      = null;
        ThreeImages        = null;
        TwoImages          = null;
        TwentyRemoteImages = null;
        FallbackImages     = null;
    }

    private static ImagePreviewItem CreateItem(string source)
    {
        return new ImagePreviewItem(ImageLoadSource.FromUri(source));
    }
}
