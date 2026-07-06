using AtomUI.Theme.Language;
using AtomUIGallery.Localization;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

[LanguageProvider(LanguageCode.en_US, ImagePreviewerShowCase.LanguageId)]
internal partial class en_US
{
    public const string ScenarioExamples = "Examples";
    public const string ScenarioApi = "API";
    public const string ScenarioDesignToken = "Design Token";
    public const string ComponentCategory = "Data Display";
    public const string ComponentStatusStable = "Stable";
    public const string PageSubtitle = "Preview single or multiple images in an overlay with zooming, moving, and switching controls.";
    public const string PageDescription = "ImagePreviewer displays an image cover that opens a preview surface. It supports fallback images, custom covers, image groups, scaling limits, and dialog lifecycle events.";
    public const string BasicUsageTitle = "Basic Usage";
    public const string BasicUsageDescription = "Click the image to zoom in.";
    public const string RemoteImageLoadingTitle = "Remote image loading";
    public const string RemoteImageLoadingDescription = "Load an image directly from an HTTPS source URI.";
    public const string FaultTolerantTitle = "Fault tolerant";
    public const string FaultTolerantDescription = "Load failed to display image placeholder.";
    public const string PreviewFromOneImageTitle = "Preview from one image";
    public const string PreviewFromOneImageDescription = "Preview a collection from one image.";
    public const string CustomPreviewImageTitle = "Custom preview image";
    public const string CustomPreviewImageDescription = "You can set different preview image.";
    public const string MultipleImagePreviewTitle = "Multiple image preview";
    public const string MultipleImagePreviewDescription = "Click the left and right switch buttons to preview multiple images.";
    public const string ApiColumnProperty = "Property";
    public const string ApiColumnDescription = "Description";
    public const string ApiColumnType = "Type";
    public const string ApiColumnDefault = "Default";
    public const string ApiPropertySourceUri = "Single image source URI used by ImagePreviewer.";
    public const string ApiPropertySourceUris = "Image source URI list used by ImagePreviewer and ImageGroupPreviewer.";
    public const string ApiPropertyFallbackSourceUri = "Fallback image source URI shown when the configured image cannot be loaded.";
    public const string ApiPropertyIsOpen = "Controls whether the preview overlay or dialog is open. Two-way binding is enabled by default.";
    public const string ApiPropertyCoverWidth = "Width of the image cover.";
    public const string ApiPropertyCoverHeight = "Height of the image cover.";
    public const string ApiPropertyCurrentIndex = "Current preview image index in a group. Two-way binding is enabled by default.";
    public const string ApiPropertyPreviewTitle = "Explicit title shown in the preview dialog title bar.";
    public const string ApiPropertyPreviewTitleIcon = "Optional icon shown before the preview dialog title.";
    public const string ApiPropertyPreviewTitleResolver = "Resolver used to derive the preview dialog title when PreviewTitle is empty.";
    public const string ApiPropertyCoverSourceUri = "Custom cover image source URI for ImagePreviewer.";
    public const string ApiPropertyLoadingContent = "Custom content displayed while an image is loading.";
    public const string ApiPropertyLoadingContentTemplate = "Template used to render custom loading content.";
    public const string ApiPropertyErrorContent = "Custom content displayed when image loading fails.";
    public const string ApiPropertyErrorContentTemplate = "Template used to render custom error content.";
    public const string ApiPropertyIsShowCoverMask = "Shows or hides the cover mask and preview indicator.";
    public const string ApiPropertyImageScaleStep = "Scale step applied by zoom-in and zoom-out actions.";
    public const string ApiPropertyImageMinScale = "Minimum image scale in the preview surface.";
    public const string ApiPropertyImageMaxScale = "Maximum image scale in the preview surface.";
    public const string ApiEventDialogOpened = "Raised after the preview dialog opens.";
    public const string ApiEventDialogClosing = "Raised before the preview dialog closes.";
    public const string ApiEventDialogClosed = "Raised after the preview dialog closes.";
    public const string TokenColumnToken = "Token";
    public const string TokenColumnDescription = "Description";
    public const string TokenColumnScope = "Scope";
    public const string TokenColumnStatus = "Status";
    public const string TokenNamePreviewOperationSize = "Size of preview operation icons.";
    public const string TokenNamePreviewOperationColor = "Color of preview operation icons.";
    public const string TokenNamePreviewOperationHoverColor = "Hover color of preview operation icons.";
    public const string TokenNameImagePreviewSwitchSize = "Size of image switch buttons.";
    public const string TokenNameMaskBgColor = "Mask background color over the cover.";
    public const string TokenNameDialogMinWidth = "Minimum width of the preview dialog.";
    public const string TokenNameDialogMinHeight = "Minimum height of the preview dialog.";
    public const string TokenNameCoverImageWidth = "Default cover image width.";
    public const string TokenScopeComponent = "Component";
    public const string TokenStatusStable = "Stable";

}
