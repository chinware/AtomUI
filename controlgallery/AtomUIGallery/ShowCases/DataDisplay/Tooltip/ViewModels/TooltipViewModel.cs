using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Tooltip;

public class TooltipViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Tooltip";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<TooltipApiRow>? _apiRows;
    private ObservableCollection<TooltipDesignTokenRow>? _designTokenRows;

    public ObservableCollection<TooltipApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<TooltipDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public TooltipViewModel(IScreen screen)
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
            new TooltipApiRow("Tip", Lang(TooltipShowCaseLangResourceKind.ApiPropertyTip), "object?", "cyan", "null"),
            new TooltipApiRow("Placement", Lang(TooltipShowCaseLangResourceKind.ApiPropertyPlacement), "PlacementMode", "purple", "Top"),
            new TooltipApiRow("IsArrowVisible", Lang(TooltipShowCaseLangResourceKind.ApiPropertyIsArrowVisible), "bool", "green", "true"),
            new TooltipApiRow("IsPointAtCenter", Lang(TooltipShowCaseLangResourceKind.ApiPropertyIsPointAtCenter), "bool", "green", "false"),
            new TooltipApiRow("PresetColor", Lang(TooltipShowCaseLangResourceKind.ApiPropertyPresetColor), "PresetColorType?", "cyan", "null"),
            new TooltipApiRow("Color", Lang(TooltipShowCaseLangResourceKind.ApiPropertyColor), "Color?", "cyan", "null"),
            new TooltipApiRow("ShowDelay", Lang(TooltipShowCaseLangResourceKind.ApiPropertyShowDelay), "int", "green", "400"),
            new TooltipApiRow("ShowOnDisabled", Lang(TooltipShowCaseLangResourceKind.ApiPropertyShowOnDisabled), "bool", "green", "false")
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
            new TooltipDesignTokenRow("ToolTipBackground", Lang(TooltipShowCaseLangResourceKind.TokenNameToolTipBackground), Lang(TooltipShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TooltipShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TooltipDesignTokenRow("ToolTipColor", Lang(TooltipShowCaseLangResourceKind.TokenNameToolTipColor), Lang(TooltipShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TooltipShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TooltipDesignTokenRow("ToolTipMaxWidth", Lang(TooltipShowCaseLangResourceKind.TokenNameToolTipMaxWidth), Lang(TooltipShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TooltipShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TooltipDesignTokenRow("BorderRadiusOuter", Lang(TooltipShowCaseLangResourceKind.TokenNameBorderRadiusOuter), Lang(TooltipShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TooltipShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TooltipDesignTokenRow("Padding", Lang(TooltipShowCaseLangResourceKind.TokenNamePadding), Lang(TooltipShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TooltipShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TooltipDesignTokenRow("MotionDuration", Lang(TooltipShowCaseLangResourceKind.TokenNameMotionDuration), Lang(TooltipShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TooltipShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
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

    private static string Lang(TooltipShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(TooltipShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            TooltipShowCaseLangResourceKind.ApiPropertyTip             => en_US.ApiPropertyTip,
            TooltipShowCaseLangResourceKind.ApiPropertyPlacement       => en_US.ApiPropertyPlacement,
            TooltipShowCaseLangResourceKind.ApiPropertyIsArrowVisible  => en_US.ApiPropertyIsArrowVisible,
            TooltipShowCaseLangResourceKind.ApiPropertyIsPointAtCenter => en_US.ApiPropertyIsPointAtCenter,
            TooltipShowCaseLangResourceKind.ApiPropertyPresetColor     => en_US.ApiPropertyPresetColor,
            TooltipShowCaseLangResourceKind.ApiPropertyColor           => en_US.ApiPropertyColor,
            TooltipShowCaseLangResourceKind.ApiPropertyShowDelay       => en_US.ApiPropertyShowDelay,
            TooltipShowCaseLangResourceKind.ApiPropertyShowOnDisabled  => en_US.ApiPropertyShowOnDisabled,
            TooltipShowCaseLangResourceKind.TokenNameToolTipBackground => en_US.TokenNameToolTipBackground,
            TooltipShowCaseLangResourceKind.TokenNameToolTipColor      => en_US.TokenNameToolTipColor,
            TooltipShowCaseLangResourceKind.TokenNameToolTipMaxWidth   => en_US.TokenNameToolTipMaxWidth,
            TooltipShowCaseLangResourceKind.TokenNameBorderRadiusOuter => en_US.TokenNameBorderRadiusOuter,
            TooltipShowCaseLangResourceKind.TokenNamePadding           => en_US.TokenNamePadding,
            TooltipShowCaseLangResourceKind.TokenNameMotionDuration    => en_US.TokenNameMotionDuration,
            TooltipShowCaseLangResourceKind.TokenScopeComponent        => en_US.TokenScopeComponent,
            TooltipShowCaseLangResourceKind.TokenStatusStable          => en_US.TokenStatusStable,
            _                                                          => kind.ToString()
        };
    }
}

public sealed record TooltipApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record TooltipDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
