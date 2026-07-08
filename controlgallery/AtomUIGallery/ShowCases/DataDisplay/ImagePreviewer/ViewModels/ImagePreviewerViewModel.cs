using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ImagePreviewer;

public class ImagePreviewerViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "ImagePreviewer";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ImageSourceUri? _remoteImage;

    public ImageSourceUri? RemoteImage
    {
        get => _remoteImage;
        set => this.RaiseAndSetIfChanged(ref _remoteImage, value);
    }

    private IList<ImageSourceUri>? _defaultImages;

    public IList<ImageSourceUri>? DefaultImages
    {
        get => _defaultImages;
        set => this.RaiseAndSetIfChanged(ref _defaultImages, value);
    }

    private IList<ImageSourceUri>? _twoImages;

    public IList<ImageSourceUri>? TwoImages
    {
        get => _twoImages;
        set => this.RaiseAndSetIfChanged(ref _twoImages, value);
    }

    private IList<ImageSourceUri>? _threeImages;

    public IList<ImageSourceUri>? ThreeImages
    {
        get => _threeImages;
        set => this.RaiseAndSetIfChanged(ref _threeImages, value);
    }

    private IList<ImageSourceUri>? _twentyRemoteImages;

    public IList<ImageSourceUri>? TwentyRemoteImages
    {
        get => _twentyRemoteImages;
        set => this.RaiseAndSetIfChanged(ref _twentyRemoteImages, value);
    }

    private ImageSourceUri? _fallbackImage;

    public ImageSourceUri? FallbackImage
    {
        get => _fallbackImage;
        set => this.RaiseAndSetIfChanged(ref _fallbackImage, value);
    }

    private ObservableCollection<ImagePreviewerApiRow>? _apiRows;
    private ObservableCollection<ImagePreviewerDesignTokenRow>? _designTokenRows;

    public ObservableCollection<ImagePreviewerApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<ImagePreviewerDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public ImagePreviewerViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void EnsurePreviewAssets()
    {
        RemoteImage = "https://zos.alipayobjects.com/rmsportal/jkjgkEfvpUPVyRjUImniVslZfWPnJuuZ.png";
        DefaultImages =
        [
            "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/1.png"
        ];
        ThreeImages =
        [
            "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/4.webp",
            "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/5.webp",
            "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/6.webp"
        ];
        TwoImages =
        [
            "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/2.svg",
            "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/3.svg",
        ];
        TwentyRemoteImages =
        [
            "https://picsum.photos/id/20/600/400",
            "https://picsum.photos/id/21/600/400",
            "https://picsum.photos/id/22/600/400",
            "https://picsum.photos/id/23/600/400",
            "https://picsum.photos/id/24/600/400",
            "https://picsum.photos/id/25/600/400",
            "https://picsum.photos/id/26/600/400",
            "https://picsum.photos/id/27/600/400",
            "https://picsum.photos/id/28/600/400",
            "https://picsum.photos/id/29/600/400",
            "https://picsum.photos/id/30/600/400",
            "https://picsum.photos/id/31/600/400",
            "https://picsum.photos/id/32/600/400",
            "https://picsum.photos/id/33/600/400",
            "https://picsum.photos/id/34/600/400",
            "https://picsum.photos/id/35/600/400",
            "https://picsum.photos/id/36/600/400",
            "https://picsum.photos/id/37/600/400",
            "https://picsum.photos/id/38/600/400",
            "https://picsum.photos/id/39/600/400"
        ];
        FallbackImage = "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/Fallback.png";
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

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new ImagePreviewerApiRow("SourceUri", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertySourceUri), "ImageSourceUri?", "cyan", "null"),
            new ImagePreviewerApiRow("SourceUris", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertySourceUris), "IList<ImageSourceUri>?", "cyan", "null"),
            new ImagePreviewerApiRow("FallbackSourceUri", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyFallbackSourceUri), "ImageSourceUri?", "cyan", "null"),
            new ImagePreviewerApiRow("IsOpen", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyIsOpen), "bool", "green", "false"),
            new ImagePreviewerApiRow("CoverWidth", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverWidth), "double", "green", "NaN"),
            new ImagePreviewerApiRow("CoverHeight", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverHeight), "double", "green", "NaN"),
            new ImagePreviewerApiRow("CurrentIndex", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyCurrentIndex), "int", "green", "0"),
            new ImagePreviewerApiRow("CoverIndex", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverIndex), "int", "green", "0"),
            new ImagePreviewerApiRow("MaxConcurrentLoads", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyMaxConcurrentLoads), "int", "green", "4"),
            new ImagePreviewerApiRow("PreloadCount", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyPreloadCount), "int", "green", "1"),
            new ImagePreviewerApiRow("PreviewTitle", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyPreviewTitle), "string?", "cyan", "null"),
            new ImagePreviewerApiRow("PreviewTitleIcon", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyPreviewTitleIcon), "PathIcon?", "cyan", "null"),
            new ImagePreviewerApiRow("PreviewTitleResolver", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyPreviewTitleResolver), "IImagePreviewTitleResolver?", "cyan", "DefaultImagePreviewTitleResolver.Instance"),
            new ImagePreviewerApiRow("LoadingContent", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyLoadingContent), "object?", "cyan", "null"),
            new ImagePreviewerApiRow("LoadingContentTemplate", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyLoadingContentTemplate), "IDataTemplate?", "cyan", "null"),
            new ImagePreviewerApiRow("ErrorContent", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyErrorContent), "object?", "cyan", "null"),
            new ImagePreviewerApiRow("ErrorContentTemplate", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyErrorContentTemplate), "IDataTemplate?", "cyan", "null"),
            new ImagePreviewerApiRow("IsShowCoverMask", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyIsShowCoverMask), "bool", "green", "true"),
            new ImagePreviewerApiRow("ImageScaleStep", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyImageScaleStep), "double", "green", "0.5"),
            new ImagePreviewerApiRow("ImageMinScale", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyImageMinScale), "double", "green", "1.0"),
            new ImagePreviewerApiRow("ImageMaxScale", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyImageMaxScale), "double", "green", "50.0"),
            new ImagePreviewerApiRow("DialogOpened", Lang(ImagePreviewerShowCaseLangResourceKind.ApiEventDialogOpened), "event EventHandler?", "purple", "-"),
            new ImagePreviewerApiRow("DialogClosing", Lang(ImagePreviewerShowCaseLangResourceKind.ApiEventDialogClosing), "event EventHandler<CancelEventArgs>?", "purple", "-"),
            new ImagePreviewerApiRow("DialogClosed", Lang(ImagePreviewerShowCaseLangResourceKind.ApiEventDialogClosed), "event EventHandler?", "purple", "-")
        ];
    }

    public void EnsureDesignTokenRows()
    {
        if (DesignTokenRows is not null)
        {
            return;
        }

        DesignTokenRows =
        [
            new ImagePreviewerDesignTokenRow("PreviewOperationSize", Lang(ImagePreviewerShowCaseLangResourceKind.TokenNamePreviewOperationSize), Lang(ImagePreviewerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ImagePreviewerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ImagePreviewerDesignTokenRow("PreviewOperationColor", Lang(ImagePreviewerShowCaseLangResourceKind.TokenNamePreviewOperationColor), Lang(ImagePreviewerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ImagePreviewerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ImagePreviewerDesignTokenRow("PreviewOperationHoverColor", Lang(ImagePreviewerShowCaseLangResourceKind.TokenNamePreviewOperationHoverColor), Lang(ImagePreviewerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ImagePreviewerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ImagePreviewerDesignTokenRow("ImagePreviewSwitchSize", Lang(ImagePreviewerShowCaseLangResourceKind.TokenNameImagePreviewSwitchSize), Lang(ImagePreviewerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ImagePreviewerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ImagePreviewerDesignTokenRow("MaskBgColor", Lang(ImagePreviewerShowCaseLangResourceKind.TokenNameMaskBgColor), Lang(ImagePreviewerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ImagePreviewerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ImagePreviewerDesignTokenRow("DialogMinWidth", Lang(ImagePreviewerShowCaseLangResourceKind.TokenNameDialogMinWidth), Lang(ImagePreviewerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ImagePreviewerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ImagePreviewerDesignTokenRow("DialogMinHeight", Lang(ImagePreviewerShowCaseLangResourceKind.TokenNameDialogMinHeight), Lang(ImagePreviewerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ImagePreviewerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ImagePreviewerDesignTokenRow("CoverImageWidth", Lang(ImagePreviewerShowCaseLangResourceKind.TokenNameCoverImageWidth), Lang(ImagePreviewerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ImagePreviewerShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(ImagePreviewerShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(ImagePreviewerShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            ImagePreviewerShowCaseLangResourceKind.RemoteImageLoadingTitle              => en_US.RemoteImageLoadingTitle,
            ImagePreviewerShowCaseLangResourceKind.RemoteImageLoadingDescription        => en_US.RemoteImageLoadingDescription,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertySourceUri                   => en_US.ApiPropertySourceUri,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertySourceUris                  => en_US.ApiPropertySourceUris,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyFallbackSourceUri           => en_US.ApiPropertyFallbackSourceUri,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyIsOpen                      => en_US.ApiPropertyIsOpen,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverWidth                  => en_US.ApiPropertyCoverWidth,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverHeight                 => en_US.ApiPropertyCoverHeight,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyCurrentIndex                => en_US.ApiPropertyCurrentIndex,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverIndex                  => en_US.ApiPropertyCoverIndex,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyMaxConcurrentLoads          => en_US.ApiPropertyMaxConcurrentLoads,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyPreloadCount                => en_US.ApiPropertyPreloadCount,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyPreviewTitle                => en_US.ApiPropertyPreviewTitle,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyPreviewTitleIcon            => en_US.ApiPropertyPreviewTitleIcon,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyPreviewTitleResolver        => en_US.ApiPropertyPreviewTitleResolver,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyLoadingContent              => en_US.ApiPropertyLoadingContent,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyLoadingContentTemplate      => en_US.ApiPropertyLoadingContentTemplate,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyErrorContent                => en_US.ApiPropertyErrorContent,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyErrorContentTemplate        => en_US.ApiPropertyErrorContentTemplate,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyIsShowCoverMask             => en_US.ApiPropertyIsShowCoverMask,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyImageScaleStep              => en_US.ApiPropertyImageScaleStep,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyImageMinScale               => en_US.ApiPropertyImageMinScale,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyImageMaxScale               => en_US.ApiPropertyImageMaxScale,
            ImagePreviewerShowCaseLangResourceKind.ApiEventDialogOpened                   => en_US.ApiEventDialogOpened,
            ImagePreviewerShowCaseLangResourceKind.ApiEventDialogClosing                  => en_US.ApiEventDialogClosing,
            ImagePreviewerShowCaseLangResourceKind.ApiEventDialogClosed                   => en_US.ApiEventDialogClosed,
            ImagePreviewerShowCaseLangResourceKind.TokenNamePreviewOperationSize          => en_US.TokenNamePreviewOperationSize,
            ImagePreviewerShowCaseLangResourceKind.TokenNamePreviewOperationColor         => en_US.TokenNamePreviewOperationColor,
            ImagePreviewerShowCaseLangResourceKind.TokenNamePreviewOperationHoverColor    => en_US.TokenNamePreviewOperationHoverColor,
            ImagePreviewerShowCaseLangResourceKind.TokenNameImagePreviewSwitchSize        => en_US.TokenNameImagePreviewSwitchSize,
            ImagePreviewerShowCaseLangResourceKind.TokenNameMaskBgColor                   => en_US.TokenNameMaskBgColor,
            ImagePreviewerShowCaseLangResourceKind.TokenNameDialogMinWidth                => en_US.TokenNameDialogMinWidth,
            ImagePreviewerShowCaseLangResourceKind.TokenNameDialogMinHeight               => en_US.TokenNameDialogMinHeight,
            ImagePreviewerShowCaseLangResourceKind.TokenNameCoverImageWidth               => en_US.TokenNameCoverImageWidth,
            ImagePreviewerShowCaseLangResourceKind.TokenScopeComponent                    => en_US.TokenScopeComponent,
            ImagePreviewerShowCaseLangResourceKind.TokenStatusStable                      => en_US.TokenStatusStable,
            _                                                                             => kind.ToString()
        };
    }
}

public sealed record ImagePreviewerApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record ImagePreviewerDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
