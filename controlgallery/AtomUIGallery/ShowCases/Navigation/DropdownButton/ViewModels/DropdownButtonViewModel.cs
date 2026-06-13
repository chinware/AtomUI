using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.DropdownButton;

public class DropdownButtonViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "DropdownButton";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<DropdownButtonApiRow>? _apiRows;
    private ObservableCollection<DropdownButtonDesignTokenRow>? _designTokenRows;

    public ObservableCollection<DropdownButtonApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<DropdownButtonDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public DropdownButtonViewModel(IScreen screen)
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
            new DropdownButtonApiRow("Content", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyContent), "object?", "cyan", "null"),
            new DropdownButtonApiRow("DropdownFlyout", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyDropdownFlyout), "MenuFlyout?", "cyan", "null"),
            new DropdownButtonApiRow("TriggerType", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyTriggerType), "FlyoutTriggerType", "blue", "Hover"),
            new DropdownButtonApiRow("IsArrowVisible", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyIsArrowVisible), "bool", "purple", "false"),
            new DropdownButtonApiRow("IsPointAtCenter", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyIsPointAtCenter), "bool", "purple", "false"),
            new DropdownButtonApiRow("Placement", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyPlacement), "PlacementMode", "blue", "BottomEdgeAlignedLeft"),
            new DropdownButtonApiRow("PlacementAnchor", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyPlacementAnchor), "PopupAnchor", "blue", "None"),
            new DropdownButtonApiRow("PlacementGravity", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyPlacementGravity), "PopupGravity", "blue", "None"),
            new DropdownButtonApiRow("MarginToAnchor", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyMarginToAnchor), "double", "cyan", "0"),
            new DropdownButtonApiRow("MouseEnterDelay", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyMouseEnterDelay), "int", "cyan", "0"),
            new DropdownButtonApiRow("MouseLeaveDelay", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyMouseLeaveDelay), "int", "cyan", "0"),
            new DropdownButtonApiRow("IsShowOpenIndicator", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyIsShowOpenIndicator), "bool", "purple", "true"),
            new DropdownButtonApiRow("OpenIndicator", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyOpenIndicator), "PathIcon?", "cyan", "DownOutlined"),
            new DropdownButtonApiRow("ShouldUseOverlayPopup", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup), "bool", "purple", "true"),
            new DropdownButtonApiRow("ButtonType", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyButtonType), "ButtonType", "blue", "Default"),
            new DropdownButtonApiRow("SizeType", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new DropdownButtonApiRow("IsDanger", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyIsDanger), "bool", "purple", "false"),
            new DropdownButtonApiRow("IsMotionEnabled", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "token"),
            new DropdownButtonApiRow("IsWaveSpiritEnabled", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled), "bool", "purple", "true"),
            new DropdownButtonApiRow("MenuItemClicked", Lang(DropdownButtonShowCaseLangResourceKind.ApiPropertyMenuItemClicked), "event", "default", "null")
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
            new DropdownButtonDesignTokenRow("Padding", Lang(DropdownButtonShowCaseLangResourceKind.TokenNamePadding), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("PaddingLG", Lang(DropdownButtonShowCaseLangResourceKind.TokenNamePaddingLG), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("PaddingSM", Lang(DropdownButtonShowCaseLangResourceKind.TokenNamePaddingSM), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("ContentFontSize", Lang(DropdownButtonShowCaseLangResourceKind.TokenNameContentFontSize), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("ContentFontSizeLG", Lang(DropdownButtonShowCaseLangResourceKind.TokenNameContentFontSizeLG), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("ContentFontSizeSM", Lang(DropdownButtonShowCaseLangResourceKind.TokenNameContentFontSizeSM), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("IconSize", Lang(DropdownButtonShowCaseLangResourceKind.TokenNameIconSize), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("IconSizeLG", Lang(DropdownButtonShowCaseLangResourceKind.TokenNameIconSizeLG), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("IconSizeSM", Lang(DropdownButtonShowCaseLangResourceKind.TokenNameIconSizeSM), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("IconMargin", Lang(DropdownButtonShowCaseLangResourceKind.TokenNameIconMargin), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("GutterToFlyout", Lang(DropdownButtonShowCaseLangResourceKind.TokenNameGutterToFlyout), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("DefaultBg", Lang(DropdownButtonShowCaseLangResourceKind.TokenNameDefaultBg), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DropdownButtonDesignTokenRow("DefaultBorderColor", Lang(DropdownButtonShowCaseLangResourceKind.TokenNameDefaultBorderColor), Lang(DropdownButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DropdownButtonShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(DropdownButtonShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(DropdownButtonShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            DropdownButtonShowCaseLangResourceKind.ApiPropertyContent               => en_US.ApiPropertyContent,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyDropdownFlyout        => en_US.ApiPropertyDropdownFlyout,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyTriggerType           => en_US.ApiPropertyTriggerType,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyIsArrowVisible        => en_US.ApiPropertyIsArrowVisible,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyIsPointAtCenter       => en_US.ApiPropertyIsPointAtCenter,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyPlacement             => en_US.ApiPropertyPlacement,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyPlacementAnchor       => en_US.ApiPropertyPlacementAnchor,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyPlacementGravity      => en_US.ApiPropertyPlacementGravity,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyMarginToAnchor        => en_US.ApiPropertyMarginToAnchor,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyMouseEnterDelay       => en_US.ApiPropertyMouseEnterDelay,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyMouseLeaveDelay       => en_US.ApiPropertyMouseLeaveDelay,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyIsShowOpenIndicator   => en_US.ApiPropertyIsShowOpenIndicator,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyOpenIndicator         => en_US.ApiPropertyOpenIndicator,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup => en_US.ApiPropertyShouldUseOverlayPopup,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyButtonType            => en_US.ApiPropertyButtonType,
            DropdownButtonShowCaseLangResourceKind.ApiPropertySizeType              => en_US.ApiPropertySizeType,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyIsDanger              => en_US.ApiPropertyIsDanger,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyIsMotionEnabled       => en_US.ApiPropertyIsMotionEnabled,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled   => en_US.ApiPropertyIsWaveSpiritEnabled,
            DropdownButtonShowCaseLangResourceKind.ApiPropertyMenuItemClicked       => en_US.ApiPropertyMenuItemClicked,
            DropdownButtonShowCaseLangResourceKind.TokenNamePadding                 => en_US.TokenNamePadding,
            DropdownButtonShowCaseLangResourceKind.TokenNamePaddingLG               => en_US.TokenNamePaddingLG,
            DropdownButtonShowCaseLangResourceKind.TokenNamePaddingSM               => en_US.TokenNamePaddingSM,
            DropdownButtonShowCaseLangResourceKind.TokenNameContentFontSize         => en_US.TokenNameContentFontSize,
            DropdownButtonShowCaseLangResourceKind.TokenNameContentFontSizeLG       => en_US.TokenNameContentFontSizeLG,
            DropdownButtonShowCaseLangResourceKind.TokenNameContentFontSizeSM       => en_US.TokenNameContentFontSizeSM,
            DropdownButtonShowCaseLangResourceKind.TokenNameIconSize                => en_US.TokenNameIconSize,
            DropdownButtonShowCaseLangResourceKind.TokenNameIconSizeLG              => en_US.TokenNameIconSizeLG,
            DropdownButtonShowCaseLangResourceKind.TokenNameIconSizeSM              => en_US.TokenNameIconSizeSM,
            DropdownButtonShowCaseLangResourceKind.TokenNameIconMargin              => en_US.TokenNameIconMargin,
            DropdownButtonShowCaseLangResourceKind.TokenNameGutterToFlyout          => en_US.TokenNameGutterToFlyout,
            DropdownButtonShowCaseLangResourceKind.TokenNameDefaultBg               => en_US.TokenNameDefaultBg,
            DropdownButtonShowCaseLangResourceKind.TokenNameDefaultBorderColor      => en_US.TokenNameDefaultBorderColor,
            DropdownButtonShowCaseLangResourceKind.TokenScopeComponent              => en_US.TokenScopeComponent,
            DropdownButtonShowCaseLangResourceKind.TokenStatusStable                => en_US.TokenStatusStable,
            _                                                                       => kind.ToString()
        };
    }
}

public sealed record DropdownButtonApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record DropdownButtonDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
