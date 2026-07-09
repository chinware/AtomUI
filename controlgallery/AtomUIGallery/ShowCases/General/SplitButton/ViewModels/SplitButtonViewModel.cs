using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.SplitButton;

public class SplitButtonViewModel : ReactiveObject, IRoutableViewModel, IActivatableViewModel
{
    public static EntityKey ID = "SplitButton";

    public IScreen HostScreen { get; }
    public ViewModelActivator Activator { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<SplitButtonApiRow>? _apiRows;
    private ObservableCollection<SplitButtonDesignTokenRow>? _designTokenRows;

    public ObservableCollection<SplitButtonApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<SplitButtonDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public SplitButtonViewModel(IScreen screen)
    {
        Activator  = new ViewModelActivator();
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
            new SplitButtonApiRow("Content", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyContent), "object?", "cyan", "null"),
            new SplitButtonApiRow("Command", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyCommand), "ICommand?", "cyan", "null"),
            new SplitButtonApiRow("CommandParameter", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyCommandParameter), "object?", "cyan", "null"),
            new SplitButtonApiRow("Flyout", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyFlyout), "Flyout?", "cyan", "null"),
            new SplitButtonApiRow("HotKey", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyHotKey), "KeyGesture?", "cyan", "null"),
            new SplitButtonApiRow("TriggerType", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyTriggerType), "FlyoutTriggerType", "blue", "Click"),
            new SplitButtonApiRow("Placement", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyPlacement), "PlacementMode", "blue", "BottomEdgeAlignedRight"),
            new SplitButtonApiRow("PlacementAnchor", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyPlacementAnchor), "PopupAnchor", "blue", "None"),
            new SplitButtonApiRow("PlacementGravity", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyPlacementGravity), "PopupGravity", "blue", "None"),
            new SplitButtonApiRow("GutterToFlyout", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyGutterToFlyout), "double", "cyan", "0"),
            new SplitButtonApiRow("MouseEnterDelay", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyMouseEnterDelay), "int", "cyan", "0"),
            new SplitButtonApiRow("MouseLeaveDelay", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyMouseLeaveDelay), "int", "cyan", "0"),
            new SplitButtonApiRow("SizeType", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertySizeType), "CustomizableSizeType", "blue", "Middle"),
            new SplitButtonApiRow("Icon", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyIcon), "PathIcon?", "cyan", "null"),
            new SplitButtonApiRow("OpenIndicator", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyOpenIndicator), "PathIcon?", "cyan", "EllipsisOutlined"),
            new SplitButtonApiRow("IsDanger", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyIsDanger), "bool", "green", "false"),
            new SplitButtonApiRow("IsPrimaryButtonType", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyIsPrimaryButtonType), "bool", "green", "false"),
            new SplitButtonApiRow("IsMotionEnabled", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "green", "true"),
            new SplitButtonApiRow("ShouldUseOverlayPopup", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup), "bool", "green", "false"),
            new SplitButtonApiRow("IsWaveSpiritEnabled", Lang(SplitButtonShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled), "bool", "green", "true")
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
            new SplitButtonDesignTokenRow("Padding", Lang(SplitButtonShowCaseLangResourceKind.TokenNamePadding), Lang(SplitButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitButtonDesignTokenRow("PaddingLG", Lang(SplitButtonShowCaseLangResourceKind.TokenNamePaddingLG), Lang(SplitButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitButtonDesignTokenRow("PaddingSM", Lang(SplitButtonShowCaseLangResourceKind.TokenNamePaddingSM), Lang(SplitButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitButtonDesignTokenRow("IconSize", Lang(SplitButtonShowCaseLangResourceKind.TokenNameIconSize), Lang(SplitButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitButtonDesignTokenRow("IconSizeLG", Lang(SplitButtonShowCaseLangResourceKind.TokenNameIconSizeLG), Lang(SplitButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitButtonDesignTokenRow("IconSizeSM", Lang(SplitButtonShowCaseLangResourceKind.TokenNameIconSizeSM), Lang(SplitButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitButtonDesignTokenRow("GroupBorderColor", Lang(SplitButtonShowCaseLangResourceKind.TokenNameGroupBorderColor), Lang(SplitButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitButtonShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SplitButtonDesignTokenRow("GutterToFlyout", Lang(SplitButtonShowCaseLangResourceKind.TokenNameGutterToFlyout), Lang(SplitButtonShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SplitButtonShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(SplitButtonShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(SplitButtonShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            SplitButtonShowCaseLangResourceKind.ApiPropertyContent               => en_US.ApiPropertyContent,
            SplitButtonShowCaseLangResourceKind.ApiPropertyCommand               => en_US.ApiPropertyCommand,
            SplitButtonShowCaseLangResourceKind.ApiPropertyCommandParameter      => en_US.ApiPropertyCommandParameter,
            SplitButtonShowCaseLangResourceKind.ApiPropertyFlyout                => en_US.ApiPropertyFlyout,
            SplitButtonShowCaseLangResourceKind.ApiPropertyHotKey                => en_US.ApiPropertyHotKey,
            SplitButtonShowCaseLangResourceKind.ApiPropertyTriggerType           => en_US.ApiPropertyTriggerType,
            SplitButtonShowCaseLangResourceKind.ApiPropertyPlacement             => en_US.ApiPropertyPlacement,
            SplitButtonShowCaseLangResourceKind.ApiPropertyPlacementAnchor       => en_US.ApiPropertyPlacementAnchor,
            SplitButtonShowCaseLangResourceKind.ApiPropertyPlacementGravity      => en_US.ApiPropertyPlacementGravity,
            SplitButtonShowCaseLangResourceKind.ApiPropertyGutterToFlyout        => en_US.ApiPropertyGutterToFlyout,
            SplitButtonShowCaseLangResourceKind.ApiPropertyMouseEnterDelay       => en_US.ApiPropertyMouseEnterDelay,
            SplitButtonShowCaseLangResourceKind.ApiPropertyMouseLeaveDelay       => en_US.ApiPropertyMouseLeaveDelay,
            SplitButtonShowCaseLangResourceKind.ApiPropertySizeType              => en_US.ApiPropertySizeType,
            SplitButtonShowCaseLangResourceKind.ApiPropertyIcon                  => en_US.ApiPropertyIcon,
            SplitButtonShowCaseLangResourceKind.ApiPropertyOpenIndicator         => en_US.ApiPropertyOpenIndicator,
            SplitButtonShowCaseLangResourceKind.ApiPropertyIsDanger              => en_US.ApiPropertyIsDanger,
            SplitButtonShowCaseLangResourceKind.ApiPropertyIsPrimaryButtonType   => en_US.ApiPropertyIsPrimaryButtonType,
            SplitButtonShowCaseLangResourceKind.ApiPropertyIsMotionEnabled       => en_US.ApiPropertyIsMotionEnabled,
            SplitButtonShowCaseLangResourceKind.ApiPropertyShouldUseOverlayPopup => en_US.ApiPropertyShouldUseOverlayPopup,
            SplitButtonShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled   => en_US.ApiPropertyIsWaveSpiritEnabled,
            SplitButtonShowCaseLangResourceKind.TokenNamePadding                 => en_US.TokenNamePadding,
            SplitButtonShowCaseLangResourceKind.TokenNamePaddingLG               => en_US.TokenNamePaddingLG,
            SplitButtonShowCaseLangResourceKind.TokenNamePaddingSM               => en_US.TokenNamePaddingSM,
            SplitButtonShowCaseLangResourceKind.TokenNameIconSize                => en_US.TokenNameIconSize,
            SplitButtonShowCaseLangResourceKind.TokenNameIconSizeLG              => en_US.TokenNameIconSizeLG,
            SplitButtonShowCaseLangResourceKind.TokenNameIconSizeSM              => en_US.TokenNameIconSizeSM,
            SplitButtonShowCaseLangResourceKind.TokenNameGroupBorderColor        => en_US.TokenNameGroupBorderColor,
            SplitButtonShowCaseLangResourceKind.TokenNameGutterToFlyout          => en_US.TokenNameGutterToFlyout,
            SplitButtonShowCaseLangResourceKind.TokenScopeComponent              => en_US.TokenScopeComponent,
            SplitButtonShowCaseLangResourceKind.TokenStatusStable                => en_US.TokenStatusStable,
            _                                                                    => kind.ToString()
        };
    }
}

public sealed record SplitButtonApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record SplitButtonDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
