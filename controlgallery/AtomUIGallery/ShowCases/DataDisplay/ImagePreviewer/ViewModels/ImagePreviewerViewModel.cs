using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
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

    private IList<string>? _defaultImages;

    public IList<string>? DefaultImages
    {
        get => _defaultImages;
        set => this.RaiseAndSetIfChanged(ref _defaultImages, value);
    }

    private IList<string>? _twoImages;

    public IList<string>? TwoImages
    {
        get => _twoImages;
        set => this.RaiseAndSetIfChanged(ref _twoImages, value);
    }

    private IList<string>? _threeImages;

    public IList<string>? ThreeImages
    {
        get => _threeImages;
        set => this.RaiseAndSetIfChanged(ref _threeImages, value);
    }

    private string? _fallbackImage;

    public string? FallbackImage
    {
        get => _fallbackImage;
        set => this.RaiseAndSetIfChanged(ref _fallbackImage, value);
    }

    private string? _blurImage;

    public string? BlurImage
    {
        get => _blurImage;
        set => this.RaiseAndSetIfChanged(ref _blurImage, value);
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
        FallbackImage = "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/Fallback.png";
        BlurImage     = "avares://AtomUIGallery/Assets/ImagePreviewerShowCase/Blur.png";
    }

    public void ClearPreviewAssets()
    {
        DefaultImages = null;
        ThreeImages   = null;
        TwoImages     = null;
        FallbackImage = null;
        BlurImage     = null;
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new ImagePreviewerApiRow("ItemsSource", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyItemsSource), "IList<string>?", "cyan", "null"),
            new ImagePreviewerApiRow("FallbackImageSrc", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyFallbackImageSrc), "string?", "cyan", "null"),
            new ImagePreviewerApiRow("IsOpen", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyIsOpen), "bool", "green", "false"),
            new ImagePreviewerApiRow("CoverWidth", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverWidth), "double", "green", "NaN"),
            new ImagePreviewerApiRow("CoverHeight", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverHeight), "double", "green", "NaN"),
            new ImagePreviewerApiRow("CurrentIndex", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyCurrentIndex), "int", "green", "0"),
            new ImagePreviewerApiRow("CoverImageSrc", Lang(ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverImageSrc), "string?", "cyan", "null"),
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
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyItemsSource                 => en_US.ApiPropertyItemsSource,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyFallbackImageSrc            => en_US.ApiPropertyFallbackImageSrc,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyIsOpen                      => en_US.ApiPropertyIsOpen,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverWidth                  => en_US.ApiPropertyCoverWidth,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverHeight                 => en_US.ApiPropertyCoverHeight,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyCurrentIndex                => en_US.ApiPropertyCurrentIndex,
            ImagePreviewerShowCaseLangResourceKind.ApiPropertyCoverImageSrc               => en_US.ApiPropertyCoverImageSrc,
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
