using Avalonia.Controls;
using Avalonia.Layout;
using AtomImagePreviewer = AtomUI.Desktop.Controls.ImagePreviewer;
using AtomImageGroupPreviewer = AtomUI.Desktop.Controls.ImageGroupPreviewer;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static readonly string[] ImagePreviewerDefaultImages =
    [
        GetImagePreviewerAssetUri("1.png")
    ];

    private static readonly string[] ImagePreviewerThreeImages =
    [
        GetImagePreviewerAssetUri("4.webp"),
        GetImagePreviewerAssetUri("5.webp"),
        GetImagePreviewerAssetUri("6.webp")
    ];

    private static readonly string[] ImagePreviewerTwoImages =
    [
        GetImagePreviewerAssetUri("2.svg"),
        GetImagePreviewerAssetUri("3.svg")
    ];

    private static readonly string ImagePreviewerFallbackImage = GetImagePreviewerAssetUri("Fallback.png");
    private static readonly string ImagePreviewerBlurImage = GetImagePreviewerAssetUri("Blur.png");

    private static IReadOnlyList<PerfScenario> CreateImagePreviewerScenarios()
    {
        return
        [
            new PerfScenario("ImagePreviewer.Basic", _ => CreateBasicImagePreviewer()),
            new PerfScenario("ImagePreviewer.Fallback", _ => CreateFallbackImagePreviewer()),
            new PerfScenario("ImagePreviewer.MultiSource", _ => CreateMultiSourceImagePreviewer()),
            new PerfScenario("ImagePreviewer.CustomCover", _ => CreateCustomCoverImagePreviewer()),
            new PerfScenario("ImageGroupPreviewer.TwoSvg", _ => CreateImageGroupPreviewer()),
            new PerfScenario("ImagePreviewer.GalleryShape", _ => CreateImagePreviewerGalleryShape())
        ];
    }

    private static AtomImagePreviewer CreateBasicImagePreviewer()
    {
        return new AtomImagePreviewer
        {
            Width       = 200,
            ItemsSource = ImagePreviewerDefaultImages
        };
    }

    private static AtomImagePreviewer CreateFallbackImagePreviewer()
    {
        return new AtomImagePreviewer
        {
            Width            = 200,
            FallbackImageSrc = ImagePreviewerFallbackImage
        };
    }

    private static AtomImagePreviewer CreateMultiSourceImagePreviewer()
    {
        return new AtomImagePreviewer
        {
            Width       = 200,
            ItemsSource = ImagePreviewerThreeImages
        };
    }

    private static AtomImagePreviewer CreateCustomCoverImagePreviewer()
    {
        return new AtomImagePreviewer
        {
            Width         = 200,
            ItemsSource   = ImagePreviewerDefaultImages,
            CoverImageSrc = ImagePreviewerBlurImage
        };
    }

    private static AtomImageGroupPreviewer CreateImageGroupPreviewer()
    {
        return new AtomImageGroupPreviewer
        {
            CoverWidth  = 200,
            CoverHeight = 200,
            ItemsSource = ImagePreviewerTwoImages
        };
    }

    private static Control CreateImagePreviewerGalleryShape()
    {
        return new StackPanel
        {
            Orientation = Orientation.Vertical,
            Spacing     = 20,
            Children =
            {
                CreateBasicImagePreviewer(),
                CreateFallbackImagePreviewer(),
                CreateMultiSourceImagePreviewer(),
                CreateCustomCoverImagePreviewer(),
                CreateImageGroupPreviewer()
            }
        };
    }

    private static string GetImagePreviewerAssetUri(string fileName)
    {
        var path = Path.GetFullPath(Path.Combine(
            "controlgallery",
            "AtomUIGallery",
            "Assets",
            "ImagePreviewerShowCase",
            fileName));
        return new Uri(path).AbsoluteUri;
    }
}
