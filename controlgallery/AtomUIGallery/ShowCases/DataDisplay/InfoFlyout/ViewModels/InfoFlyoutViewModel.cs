using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.InfoFlyout;

public class InfoFlyoutViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "InfoFlyout";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<InfoFlyoutApiRow>? _apiRows;
    private ObservableCollection<InfoFlyoutDesignTokenRow>? _designTokenRows;

    public ObservableCollection<InfoFlyoutApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<InfoFlyoutDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private bool _showArrow = true;

    public bool ShowArrow
    {
        get => _showArrow;
        set => this.RaiseAndSetIfChanged(ref _showArrow, value);
    }

    private bool _isPointAtCenter;

    public bool IsPointAtCenter
    {
        get => _isPointAtCenter;
        set => this.RaiseAndSetIfChanged(ref _isPointAtCenter, value);
    }

    public InfoFlyoutViewModel(IScreen screen)
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
            new InfoFlyoutApiRow("FlyoutHost.Flyout", Lang(InfoFlyoutShowCaseLangResourceKind.ApiPropertyFlyout), "Flyout?", "cyan", "null"),
            new InfoFlyoutApiRow("FlyoutHost.Trigger", Lang(InfoFlyoutShowCaseLangResourceKind.ApiPropertyTrigger), "FlyoutTriggerType", "blue", "Click"),
            new InfoFlyoutApiRow("FlyoutHost.Placement", Lang(InfoFlyoutShowCaseLangResourceKind.ApiPropertyPlacement), "PlacementMode", "purple", "Top"),
            new InfoFlyoutApiRow("FlyoutHost.IsArrowVisible", Lang(InfoFlyoutShowCaseLangResourceKind.ApiPropertyIsArrowVisible), "bool", "green", "false"),
            new InfoFlyoutApiRow("FlyoutHost.IsPointAtCenter", Lang(InfoFlyoutShowCaseLangResourceKind.ApiPropertyIsPointAtCenter), "bool", "green", "false"),
            new InfoFlyoutApiRow("FlyoutHost.ShouldUseOverlayPopup", Lang(InfoFlyoutShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup), "bool", "green", "true"),
            new InfoFlyoutApiRow("FlyoutHost.MarginToAnchor", Lang(InfoFlyoutShowCaseLangResourceKind.ApiPropertyMarginToAnchor), "double", "green", "Token"),
            new InfoFlyoutApiRow("FlyoutHost.MouseEnterDelay", Lang(InfoFlyoutShowCaseLangResourceKind.ApiPropertyMouseEnterDelay), "int", "green", "200"),
            new InfoFlyoutApiRow("FlyoutHost.MouseLeaveDelay", Lang(InfoFlyoutShowCaseLangResourceKind.ApiPropertyMouseLeaveDelay), "int", "green", "200"),
            new InfoFlyoutApiRow("Flyout.Content", Lang(InfoFlyoutShowCaseLangResourceKind.ApiPropertyContent), "object", "cyan", "null"),
            new InfoFlyoutApiRow("Flyout.IsLightDismissEnabled", Lang(InfoFlyoutShowCaseLangResourceKind.ApiPropertyIsLightDismissEnabled), "bool", "green", "true")
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
            new InfoFlyoutDesignTokenRow("MarginToAnchor", Lang(InfoFlyoutShowCaseLangResourceKind.TokenNameMarginToAnchor), Lang(InfoFlyoutShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(InfoFlyoutShowCaseLangResourceKind.TokenStatusStable), "success"),
            new InfoFlyoutDesignTokenRow("OverlayHostShadow", Lang(InfoFlyoutShowCaseLangResourceKind.TokenNameOverlayHostShadow), Lang(InfoFlyoutShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(InfoFlyoutShowCaseLangResourceKind.TokenStatusStable), "success"),
            new InfoFlyoutDesignTokenRow("PopupRootShadow", Lang(InfoFlyoutShowCaseLangResourceKind.TokenNamePopupRootShadow), Lang(InfoFlyoutShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(InfoFlyoutShowCaseLangResourceKind.TokenStatusStable), "success"),
            new InfoFlyoutDesignTokenRow("HorizontalOffset", Lang(InfoFlyoutShowCaseLangResourceKind.TokenNameHorizontalOffset), Lang(InfoFlyoutShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(InfoFlyoutShowCaseLangResourceKind.TokenStatusStable), "success"),
            new InfoFlyoutDesignTokenRow("VerticalOffset", Lang(InfoFlyoutShowCaseLangResourceKind.TokenNameVerticalOffset), Lang(InfoFlyoutShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(InfoFlyoutShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    public void HandleSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (sender is AtomUISegmented segmented)
        {
            if (segmented.SelectedIndex == 0)
            {
                ShowArrow       = true;
                IsPointAtCenter = false;
            }
            else if (segmented.SelectedIndex == 1)
            {
                ShowArrow       = false;
                IsPointAtCenter = false;
            }
            else if (segmented.SelectedIndex == 2)
            {
                IsPointAtCenter = true;
                ShowArrow       = true;
            }
        }
    }

    private static string Lang(InfoFlyoutShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(InfoFlyoutShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            InfoFlyoutShowCaseLangResourceKind.ApiPropertyFlyout                   => en_US.ApiPropertyFlyout,
            InfoFlyoutShowCaseLangResourceKind.ApiPropertyTrigger                  => en_US.ApiPropertyTrigger,
            InfoFlyoutShowCaseLangResourceKind.ApiPropertyPlacement                => en_US.ApiPropertyPlacement,
            InfoFlyoutShowCaseLangResourceKind.ApiPropertyIsArrowVisible           => en_US.ApiPropertyIsArrowVisible,
            InfoFlyoutShowCaseLangResourceKind.ApiPropertyIsPointAtCenter          => en_US.ApiPropertyIsPointAtCenter,
            InfoFlyoutShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup    => en_US.ApiPropertyShouldUseOverlayPopup,
            InfoFlyoutShowCaseLangResourceKind.ApiPropertyMarginToAnchor           => en_US.ApiPropertyMarginToAnchor,
            InfoFlyoutShowCaseLangResourceKind.ApiPropertyMouseEnterDelay          => en_US.ApiPropertyMouseEnterDelay,
            InfoFlyoutShowCaseLangResourceKind.ApiPropertyMouseLeaveDelay          => en_US.ApiPropertyMouseLeaveDelay,
            InfoFlyoutShowCaseLangResourceKind.ApiPropertyContent                  => en_US.ApiPropertyContent,
            InfoFlyoutShowCaseLangResourceKind.ApiPropertyIsLightDismissEnabled    => en_US.ApiPropertyIsLightDismissEnabled,
            InfoFlyoutShowCaseLangResourceKind.TokenNameMarginToAnchor             => en_US.TokenNameMarginToAnchor,
            InfoFlyoutShowCaseLangResourceKind.TokenNameOverlayHostShadow          => en_US.TokenNameOverlayHostShadow,
            InfoFlyoutShowCaseLangResourceKind.TokenNamePopupRootShadow            => en_US.TokenNamePopupRootShadow,
            InfoFlyoutShowCaseLangResourceKind.TokenNameHorizontalOffset           => en_US.TokenNameHorizontalOffset,
            InfoFlyoutShowCaseLangResourceKind.TokenNameVerticalOffset             => en_US.TokenNameVerticalOffset,
            InfoFlyoutShowCaseLangResourceKind.TokenScopeComponent                 => en_US.TokenScopeComponent,
            InfoFlyoutShowCaseLangResourceKind.TokenStatusStable                   => en_US.TokenStatusStable,
            _                                                                      => kind.ToString()
        };
    }
}

public sealed record InfoFlyoutApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record InfoFlyoutDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
