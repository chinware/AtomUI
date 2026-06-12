using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Skeleton;

public class SkeletonViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Skeleton";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ObservableCollection<SkeletonApiRow>? _apiRows;
    private ObservableCollection<SkeletonDesignTokenRow>? _designTokenRows;

    public ObservableCollection<SkeletonApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<SkeletonDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private bool _isSkeletonActive;

    public bool IsSkeletonActive
    {
        get => _isSkeletonActive;
        set => this.RaiseAndSetIfChanged(ref _isSkeletonActive, value);
    }

    private bool _isSkeletonBlock;

    public bool IsSkeletonBlock
    {
        get => _isSkeletonBlock;
        set => this.RaiseAndSetIfChanged(ref _isSkeletonBlock, value);
    }

    private CustomizableSizeType _skeletonButtonAndInputSizeType;

    public CustomizableSizeType SkeletonButtonAndInputSizeType
    {
        get => _skeletonButtonAndInputSizeType;
        set => this.RaiseAndSetIfChanged(ref _skeletonButtonAndInputSizeType, value);
    }

    private SkeletonButtonShape _skeletonButtonShape;

    public SkeletonButtonShape SkeletonButtonShape
    {
        get => _skeletonButtonShape;
        set => this.RaiseAndSetIfChanged(ref _skeletonButtonShape, value);
    }

    private AvatarShape _skeletonAvatarShape;

    public AvatarShape SkeletonAvatarShape
    {
        get => _skeletonAvatarShape;
        set => this.RaiseAndSetIfChanged(ref _skeletonAvatarShape, value);
    }

    private bool _skeletonLoading;

    public bool SkeletonLoading
    {
        get => _skeletonLoading;
        set => this.RaiseAndSetIfChanged(ref _skeletonLoading, value);
    }

    public SkeletonViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new SkeletonApiRow("Skeleton.IsLoading", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyIsLoading), "bool", "purple", "false"),
            new SkeletonApiRow("Skeleton.IsShowAvatar", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyIsShowAvatar), "bool", "purple", "false"),
            new SkeletonApiRow("Skeleton.IsShowParagraph", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyIsShowParagraph), "bool", "purple", "true"),
            new SkeletonApiRow("Skeleton.IsShowTitle", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyIsShowTitle), "bool", "purple", "true"),
            new SkeletonApiRow("Skeleton.IsRound", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyIsRound), "bool", "purple", "false"),
            new SkeletonApiRow("Skeleton.TitleWidth", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyTitleWidth), "Dimension", "cyan", "50%"),
            new SkeletonApiRow("Skeleton.ParagraphRows", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyParagraphRows), "int", "cyan", "2"),
            new SkeletonApiRow("Skeleton.ParagraphLastLineWidth", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyParagraphLastLineWidth), "Dimension", "cyan", "61%"),
            new SkeletonApiRow("Skeleton.ParagraphLineWidths", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyParagraphLineWidths), "List<Dimension>?", "cyan", "null"),
            new SkeletonApiRow("Skeleton.AvatarShape", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyAvatarShape), "AvatarShape", "cyan", "Circle"),
            new SkeletonApiRow("Skeleton.AvatarSizeType", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyAvatarSizeType), "CustomizableSizeType", "cyan", "Middle"),
            new SkeletonApiRow("Skeleton.AvatarSize", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyAvatarSize), "double", "cyan", "NaN"),
            new SkeletonApiRow("Skeleton.Content", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyContent), "object?", "cyan", "null"),
            new SkeletonApiRow("Skeleton.ContentTemplate", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyContentTemplate), "IDataTemplate?", "cyan", "null"),
            new SkeletonApiRow("AbstractSkeleton.IsActive", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyIsActive), "bool", "purple", "false"),
            new SkeletonApiRow("AbstractSkeleton.MotionDuration", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyMotionDuration), "TimeSpan", "cyan", "token"),
            new SkeletonApiRow("AbstractSkeleton.MotionEasingCurve", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyMotionEasingCurve), "Easing?", "cyan", "null"),
            new SkeletonApiRow("SkeletonElement.SizeType", Lang(SkeletonShowCaseLangResourceKind.ApiPropertySizeType), "CustomizableSizeType", "cyan", "Middle"),
            new SkeletonApiRow("SkeletonElement.IsBlock", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyIsBlock), "bool", "purple", "false"),
            new SkeletonApiRow("SkeletonButton.Shape", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyButtonShape), "SkeletonButtonShape", "cyan", "Square"),
            new SkeletonApiRow("SkeletonAvatar.Shape", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyAvatarElementShape), "AvatarShape", "cyan", "Circle"),
            new SkeletonApiRow("SkeletonAvatar.Size", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyAvatarElementSize), "double", "cyan", "NaN"),
            new SkeletonApiRow("SkeletonNode.Content", Lang(SkeletonShowCaseLangResourceKind.ApiPropertyNodeContent), "object?", "cyan", "null")
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
            new SkeletonDesignTokenRow("GradientFromColor", Lang(SkeletonShowCaseLangResourceKind.TokenNameGradientFromColor), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("GradientToColor", Lang(SkeletonShowCaseLangResourceKind.TokenNameGradientToColor), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("TitleHeight", Lang(SkeletonShowCaseLangResourceKind.TokenNameTitleHeight), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("BlockRadius", Lang(SkeletonShowCaseLangResourceKind.TokenNameBlockRadius), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("ParagraphMarginTop", Lang(SkeletonShowCaseLangResourceKind.TokenNameParagraphMarginTop), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("AvatarMarginRight", Lang(SkeletonShowCaseLangResourceKind.TokenNameAvatarMarginRight), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("ParagraphLineHeight", Lang(SkeletonShowCaseLangResourceKind.TokenNameParagraphLineHeight), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("ParagraphLineRoundCornerRadius", Lang(SkeletonShowCaseLangResourceKind.TokenNameParagraphLineRoundCornerRadius), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("LoadingMotionDuration", Lang(SkeletonShowCaseLangResourceKind.TokenNameLoadingMotionDuration), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("LoadingBackgroundStart", Lang(SkeletonShowCaseLangResourceKind.TokenNameLoadingBackgroundStart), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("LoadingBackgroundMiddle", Lang(SkeletonShowCaseLangResourceKind.TokenNameLoadingBackgroundMiddle), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("LoadingBackgroundEnd", Lang(SkeletonShowCaseLangResourceKind.TokenNameLoadingBackgroundEnd), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("ImageSize", Lang(SkeletonShowCaseLangResourceKind.TokenNameImageSize), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("ImageContainerSize", Lang(SkeletonShowCaseLangResourceKind.TokenNameImageContainerSize), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SkeletonDesignTokenRow("ImageContainerMaxSize", Lang(SkeletonShowCaseLangResourceKind.TokenNameImageContainerMaxSize), Lang(SkeletonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SkeletonShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(SkeletonShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(SkeletonShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            SkeletonShowCaseLangResourceKind.ApiPropertyIsLoading                       => en_US.ApiPropertyIsLoading,
            SkeletonShowCaseLangResourceKind.ApiPropertyIsShowAvatar                    => en_US.ApiPropertyIsShowAvatar,
            SkeletonShowCaseLangResourceKind.ApiPropertyIsShowParagraph                 => en_US.ApiPropertyIsShowParagraph,
            SkeletonShowCaseLangResourceKind.ApiPropertyIsShowTitle                     => en_US.ApiPropertyIsShowTitle,
            SkeletonShowCaseLangResourceKind.ApiPropertyIsRound                         => en_US.ApiPropertyIsRound,
            SkeletonShowCaseLangResourceKind.ApiPropertyTitleWidth                      => en_US.ApiPropertyTitleWidth,
            SkeletonShowCaseLangResourceKind.ApiPropertyParagraphRows                   => en_US.ApiPropertyParagraphRows,
            SkeletonShowCaseLangResourceKind.ApiPropertyParagraphLastLineWidth          => en_US.ApiPropertyParagraphLastLineWidth,
            SkeletonShowCaseLangResourceKind.ApiPropertyParagraphLineWidths             => en_US.ApiPropertyParagraphLineWidths,
            SkeletonShowCaseLangResourceKind.ApiPropertyAvatarShape                     => en_US.ApiPropertyAvatarShape,
            SkeletonShowCaseLangResourceKind.ApiPropertyAvatarSizeType                  => en_US.ApiPropertyAvatarSizeType,
            SkeletonShowCaseLangResourceKind.ApiPropertyAvatarSize                      => en_US.ApiPropertyAvatarSize,
            SkeletonShowCaseLangResourceKind.ApiPropertyContent                         => en_US.ApiPropertyContent,
            SkeletonShowCaseLangResourceKind.ApiPropertyContentTemplate                 => en_US.ApiPropertyContentTemplate,
            SkeletonShowCaseLangResourceKind.ApiPropertyIsActive                        => en_US.ApiPropertyIsActive,
            SkeletonShowCaseLangResourceKind.ApiPropertyMotionDuration                  => en_US.ApiPropertyMotionDuration,
            SkeletonShowCaseLangResourceKind.ApiPropertyMotionEasingCurve               => en_US.ApiPropertyMotionEasingCurve,
            SkeletonShowCaseLangResourceKind.ApiPropertySizeType                        => en_US.ApiPropertySizeType,
            SkeletonShowCaseLangResourceKind.ApiPropertyIsBlock                         => en_US.ApiPropertyIsBlock,
            SkeletonShowCaseLangResourceKind.ApiPropertyButtonShape                     => en_US.ApiPropertyButtonShape,
            SkeletonShowCaseLangResourceKind.ApiPropertyAvatarElementShape              => en_US.ApiPropertyAvatarElementShape,
            SkeletonShowCaseLangResourceKind.ApiPropertyAvatarElementSize               => en_US.ApiPropertyAvatarElementSize,
            SkeletonShowCaseLangResourceKind.ApiPropertyNodeContent                     => en_US.ApiPropertyNodeContent,
            SkeletonShowCaseLangResourceKind.TokenNameGradientFromColor                 => en_US.TokenNameGradientFromColor,
            SkeletonShowCaseLangResourceKind.TokenNameGradientToColor                   => en_US.TokenNameGradientToColor,
            SkeletonShowCaseLangResourceKind.TokenNameTitleHeight                       => en_US.TokenNameTitleHeight,
            SkeletonShowCaseLangResourceKind.TokenNameBlockRadius                       => en_US.TokenNameBlockRadius,
            SkeletonShowCaseLangResourceKind.TokenNameParagraphMarginTop                => en_US.TokenNameParagraphMarginTop,
            SkeletonShowCaseLangResourceKind.TokenNameAvatarMarginRight                 => en_US.TokenNameAvatarMarginRight,
            SkeletonShowCaseLangResourceKind.TokenNameParagraphLineHeight               => en_US.TokenNameParagraphLineHeight,
            SkeletonShowCaseLangResourceKind.TokenNameParagraphLineRoundCornerRadius    => en_US.TokenNameParagraphLineRoundCornerRadius,
            SkeletonShowCaseLangResourceKind.TokenNameLoadingMotionDuration             => en_US.TokenNameLoadingMotionDuration,
            SkeletonShowCaseLangResourceKind.TokenNameLoadingBackgroundStart            => en_US.TokenNameLoadingBackgroundStart,
            SkeletonShowCaseLangResourceKind.TokenNameLoadingBackgroundMiddle           => en_US.TokenNameLoadingBackgroundMiddle,
            SkeletonShowCaseLangResourceKind.TokenNameLoadingBackgroundEnd              => en_US.TokenNameLoadingBackgroundEnd,
            SkeletonShowCaseLangResourceKind.TokenNameImageSize                         => en_US.TokenNameImageSize,
            SkeletonShowCaseLangResourceKind.TokenNameImageContainerSize                => en_US.TokenNameImageContainerSize,
            SkeletonShowCaseLangResourceKind.TokenNameImageContainerMaxSize             => en_US.TokenNameImageContainerMaxSize,
            SkeletonShowCaseLangResourceKind.TokenScopeComponent                        => en_US.TokenScopeComponent,
            SkeletonShowCaseLangResourceKind.TokenStatusStable                          => en_US.TokenStatusStable,
            _                                                                           => kind.ToString()
        };
    }
}

public sealed record SkeletonApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record SkeletonDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
