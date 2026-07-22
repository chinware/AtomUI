using AtomUI.Theme.Language;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

[LanguageProvider(LanguageCode.en_US, ImagePreviewerShowCase.LanguageId)]
internal partial class en_US
{
    public const string ScenarioExamples = "Examples";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string PageSubtitle = "Preview single or multiple images in an overlay with zooming, moving, and switching controls.";
    public const string PageDescription = "ImagePreviewer displays an image cover that opens a preview surface. It supports fallback images, cover indexes, image groups, scaling limits, and dialog lifecycle events.";
    public const string BasicUsageTitle = "Basic Usage";
    public const string BasicUsageDescription = "Click the image to zoom in.";
    public const string RemoteImageLoadingTitle = "Remote image loading";
    public const string RemoteImageLoadingDescription = "Load an image directly from an HTTPS source URI.";
    public const string FaultTolerantTitle = "Fault tolerant";
    public const string FaultTolerantDescription = "Load failed to display image placeholder.";
    public const string TwentyRemoteImagesTitle = "20 remote images";
    public const string TwentyRemoteImagesDescription = "Load a fixed set of 20 public images with cover-only loading and neighbor preloading.";
    public const string PreviewFromOneImageTitle = "Preview from one image";
    public const string PreviewFromOneImageDescription = "Preview a collection from one image.";
    public const string CustomPreviewImageTitle = "Custom preview image";
    public const string CustomPreviewImageDescription = "Choose which source image is displayed as the cover.";
    public const string MultipleImagePreviewTitle = "Multiple image preview";
    public const string MultipleImagePreviewDescription = "Click the left and right switch buttons to preview multiple images.";
    public const string ApiEventDialogOpened = "Raised after the preview dialog opens.";
    public const string ApiEventDialogClosing = "Raised before the preview dialog closes.";
    public const string ApiEventDialogClosed = "Raised after the preview dialog closes.";

}
