using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

[LanguageProvider(LanguageCode.en_US, ImagePreviewerShowCase.LanguageId)]
internal class en_US : LanguageProvider
{
    public const string BasicUsageTitle = "Basic Usage";
    public const string BasicUsageDescription = "Click the image to zoom in.";
    public const string FaultTolerantTitle = "Fault tolerant";
    public const string FaultTolerantDescription = "Load failed to display image placeholder.";
    public const string PreviewFromOneImageTitle = "Preview from one image";
    public const string PreviewFromOneImageDescription = "Preview a collection from one image.";
    public const string CustomPreviewImageTitle = "Custom preview image";
    public const string CustomPreviewImageDescription = "You can set different preview image.";
    public const string MultipleImagePreviewTitle = "Multiple image preview";
    public const string MultipleImagePreviewDescription = "Click the left and right switch buttons to preview multiple images.";

    protected override Type GetResourceKindType() => typeof(ImagePreviewerShowCaseLangResourceKind);
}
