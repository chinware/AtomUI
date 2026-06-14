using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUI.Theme.Language;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Drawer;

public class DrawerViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Drawer";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private DrawerPlacement _multiLevelPlacement = DrawerPlacement.Right;

    public DrawerPlacement MultiLevelPlacement
    {
        get => _multiLevelPlacement;
        set => this.RaiseAndSetIfChanged(ref _multiLevelPlacement, value);
    }

    private DrawerPlacement _extraAndFooterPlacement = DrawerPlacement.Right;

    public DrawerPlacement ExtraAndFooterPlacement
    {
        get => _extraAndFooterPlacement;
        set => this.RaiseAndSetIfChanged(ref _extraAndFooterPlacement, value);
    }

    private DrawerPlacement _customPlacement = DrawerPlacement.Right;
    private ObservableCollection<DrawerApiRow>? _apiRows;
    private ObservableCollection<DrawerDesignTokenRow>? _designTokenRows;

    public DrawerPlacement CustomPlacement
    {
        get => _customPlacement;
        set => this.RaiseAndSetIfChanged(ref _customPlacement, value);
    }

    public ObservableCollection<DrawerApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<DrawerDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public DrawerViewModel(IScreen screen)
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
            new DrawerApiRow("Content", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerContent), "object?", "cyan", "null"),
            new DrawerApiRow("ContentTemplate", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerContentTemplate), "IDataTemplate?", "cyan", "null"),
            new DrawerApiRow("IsOpen", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerIsOpen), "bool", "purple", "false"),
            new DrawerApiRow("Placement", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerPlacement), "DrawerPlacement", "blue", "Right"),
            new DrawerApiRow("OpenOn", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerOpenOn), "Control?", "cyan", "TopLevel"),
            new DrawerApiRow("IsShowMask", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerIsShowMask), "bool", "purple", "true"),
            new DrawerApiRow("IsShowCloseButton", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerIsShowCloseButton), "bool", "purple", "true"),
            new DrawerApiRow("IsCloseOnMaskClick", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerIsCloseOnMaskClick), "bool", "purple", "true"),
            new DrawerApiRow("Title", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerTitle), "string", "cyan", "string.Empty"),
            new DrawerApiRow("Extra", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerExtra), "object?", "cyan", "null"),
            new DrawerApiRow("Footer", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerFooter), "object?", "cyan", "null"),
            new DrawerApiRow("SizeType", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerSizeType), "CustomizableSizeType", "blue", "Small"),
            new DrawerApiRow("DialogSize", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerDialogSize), "Dimension", "cyan", "token"),
            new DrawerApiRow("PushOffsetPercent", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerPushOffsetPercent), "double", "cyan", "0.4"),
            new DrawerApiRow("IsMotionEnabled", Lang(DrawerShowCaseLangResourceKind.ApiPropertyDrawerIsMotionEnabled), "bool", "purple", "true")
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
            new DrawerDesignTokenRow("HeaderMargin", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerHeaderMargin), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("ContentPadding", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerBodyPadding), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("FooterPadding", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerFooterPadding), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("CloseIconPadding", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerCloseIconPadding), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("CloseIconMargin", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerCloseIconMargin), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("SmallSize", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerSmallSize), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("MiddleSize", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerMiddleSize), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("LargeSize", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerLargeSize), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("PushOffsetPercent", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerPushOffsetPercent), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("BoxShadowDrawerLeft", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerBoxShadowLeft), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("BoxShadowDrawerRight", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerBoxShadowRight), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("BoxShadowDrawerUp", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerBoxShadowUp), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DrawerDesignTokenRow("BoxShadowDrawerDown", Lang(DrawerShowCaseLangResourceKind.TokenNameDrawerBoxShadowDown), Lang(DrawerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DrawerShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(DrawerShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(DrawerShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerContent                => en_US.ApiPropertyDrawerContent,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerContentTemplate        => en_US.ApiPropertyDrawerContentTemplate,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerIsOpen                 => en_US.ApiPropertyDrawerIsOpen,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerPlacement              => en_US.ApiPropertyDrawerPlacement,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerOpenOn                 => en_US.ApiPropertyDrawerOpenOn,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerIsShowMask             => en_US.ApiPropertyDrawerIsShowMask,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerIsShowCloseButton      => en_US.ApiPropertyDrawerIsShowCloseButton,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerIsCloseOnMaskClick     => en_US.ApiPropertyDrawerIsCloseOnMaskClick,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerTitle                  => en_US.ApiPropertyDrawerTitle,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerExtra                  => en_US.ApiPropertyDrawerExtra,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerFooter                 => en_US.ApiPropertyDrawerFooter,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerSizeType               => en_US.ApiPropertyDrawerSizeType,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerDialogSize             => en_US.ApiPropertyDrawerDialogSize,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerPushOffsetPercent      => en_US.ApiPropertyDrawerPushOffsetPercent,
            DrawerShowCaseLangResourceKind.ApiPropertyDrawerIsMotionEnabled        => en_US.ApiPropertyDrawerIsMotionEnabled,
            DrawerShowCaseLangResourceKind.TokenNameDrawerHeaderMargin             => en_US.TokenNameDrawerHeaderMargin,
            DrawerShowCaseLangResourceKind.TokenNameDrawerBodyPadding              => en_US.TokenNameDrawerBodyPadding,
            DrawerShowCaseLangResourceKind.TokenNameDrawerFooterPadding            => en_US.TokenNameDrawerFooterPadding,
            DrawerShowCaseLangResourceKind.TokenNameDrawerCloseIconPadding         => en_US.TokenNameDrawerCloseIconPadding,
            DrawerShowCaseLangResourceKind.TokenNameDrawerCloseIconMargin          => en_US.TokenNameDrawerCloseIconMargin,
            DrawerShowCaseLangResourceKind.TokenNameDrawerSmallSize                => en_US.TokenNameDrawerSmallSize,
            DrawerShowCaseLangResourceKind.TokenNameDrawerMiddleSize               => en_US.TokenNameDrawerMiddleSize,
            DrawerShowCaseLangResourceKind.TokenNameDrawerLargeSize                => en_US.TokenNameDrawerLargeSize,
            DrawerShowCaseLangResourceKind.TokenNameDrawerPushOffsetPercent        => en_US.TokenNameDrawerPushOffsetPercent,
            DrawerShowCaseLangResourceKind.TokenNameDrawerBoxShadowLeft            => en_US.TokenNameDrawerBoxShadowLeft,
            DrawerShowCaseLangResourceKind.TokenNameDrawerBoxShadowRight           => en_US.TokenNameDrawerBoxShadowRight,
            DrawerShowCaseLangResourceKind.TokenNameDrawerBoxShadowUp              => en_US.TokenNameDrawerBoxShadowUp,
            DrawerShowCaseLangResourceKind.TokenNameDrawerBoxShadowDown            => en_US.TokenNameDrawerBoxShadowDown,
            DrawerShowCaseLangResourceKind.TokenScopeComponent                     => en_US.TokenScopeComponent,
            DrawerShowCaseLangResourceKind.TokenStatusStable                       => en_US.TokenStatusStable,
            _                                                                      => kind.ToString()
        };
    }
}

public sealed record DrawerApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record DrawerDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
