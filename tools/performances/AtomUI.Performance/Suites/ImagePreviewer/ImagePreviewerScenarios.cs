using Avalonia.Controls;
using Avalonia.Layout;
using AtomImagePreviewer = AtomUI.Desktop.Controls.ImagePreviewer;
using AtomImageGroupPreviewer = AtomUI.Desktop.Controls.ImageGroupPreviewer;
using AtomImagePreviewSource = AtomUI.Desktop.Controls.IImagePreviewSource;
using AtomUriImagePreviewSource = AtomUI.Desktop.Controls.UriImagePreviewSource;

namespace AtomUI.Performance;

internal static partial class Program
{
    private static readonly IList<AtomImagePreviewSource> ImagePreviewerDefaultImages =
    [
        CreateImagePreviewerSource("1.png")
    ];

    private static readonly IList<AtomImagePreviewSource> ImagePreviewerThreeImages =
    [
        CreateImagePreviewerSource("4.webp"),
        CreateImagePreviewerSource("5.webp"),
        CreateImagePreviewerSource("6.webp")
    ];

    private static readonly IList<AtomImagePreviewSource> ImagePreviewerTwoImages =
    [
        CreateImagePreviewerSource("2.svg"),
        CreateImagePreviewerSource("3.svg")
    ];

    private static readonly AtomImagePreviewSource ImagePreviewerFallbackImage = CreateImagePreviewerSource("Fallback.png");
    private static readonly AtomImagePreviewSource ImagePreviewerBlurImage = CreateImagePreviewerSource("Blur.png");

    private static IReadOnlyList<PerfScenario> CreateImagePreviewerScenarios()
    {
        return
        [
            new PerfScenario("ImagePreviewer.Basic", _ => CreateBasicImagePreviewer()),
            new PerfScenario("ImagePreviewer.Fallback", _ => CreateFallbackImagePreviewer()),
            new PerfScenario("ImagePreviewer.MultiSource", _ => CreateMultiSourceImagePreviewer()),
            new PerfScenario("ImagePreviewer.SingleSource", _ => CreateSingleSourceImagePreviewer()),
            new PerfScenario("ImageGroupPreviewer.TwoSvg", _ => CreateImageGroupPreviewer()),
            new PerfScenario("ImagePreviewer.GalleryShape", _ => CreateImagePreviewerGalleryShape())
        ];
    }

    private static AtomImagePreviewer CreateBasicImagePreviewer()
    {
        return new AtomImagePreviewer
        {
            Width       = 200,
            Source      = ImagePreviewerDefaultImages[0]
        };
    }

    private static AtomImagePreviewer CreateFallbackImagePreviewer()
    {
        return new AtomImagePreviewer
        {
            Width          = 200,
            FallbackSource = ImagePreviewerFallbackImage
        };
    }

    private static AtomImagePreviewer CreateMultiSourceImagePreviewer()
    {
        return new AtomImagePreviewer
        {
            Width       = 200,
            Sources     = ImagePreviewerThreeImages
        };
    }

    private static AtomImagePreviewer CreateSingleSourceImagePreviewer()
    {
        return new AtomImagePreviewer
        {
            Width  = 200,
            Source = ImagePreviewerBlurImage
        };
    }

    private static AtomImageGroupPreviewer CreateImageGroupPreviewer()
    {
        return new AtomImageGroupPreviewer
        {
            CoverWidth  = 200,
            CoverHeight = 200,
            Sources     = ImagePreviewerTwoImages
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
                CreateSingleSourceImagePreviewer(),
                CreateImageGroupPreviewer()
            }
        };
    }

    private static AtomImagePreviewSource CreateImagePreviewerSource(string fileName)
    {
        var path = Path.GetFullPath(Path.Combine(
            "controlgallery",
            "AtomUIGallery",
            "Assets",
            "ImagePreviewerShowCase",
            fileName));
        return new AtomUriImagePreviewSource(new Uri(path));
    }
}
