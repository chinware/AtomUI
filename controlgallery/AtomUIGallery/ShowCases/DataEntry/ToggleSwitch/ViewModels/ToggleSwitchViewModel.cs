using System.Collections.ObjectModel;
using System.Reactive;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.ToggleSwitch;

public class ToggleSwitchViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "ToggleSwitch";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<ToggleSwitchApiRow>? _apiRows;
    private ObservableCollection<ToggleSwitchDesignTokenRow>? _designTokenRows;
    private bool _isDisabledDemoEnabled = true;
    private bool _isLoadingDemoLoading  = true;

    public ObservableCollection<ToggleSwitchApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<ToggleSwitchDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public bool IsDisabledDemoEnabled
    {
        get => _isDisabledDemoEnabled;
        set => this.RaiseAndSetIfChanged(ref _isDisabledDemoEnabled, value);
    }

    public bool IsLoadingDemoLoading
    {
        get => _isLoadingDemoLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoadingDemoLoading, value);
    }

    public ReactiveCommand<Unit, Unit> ToggleDisabledCommand { get; }

    public ReactiveCommand<Unit, Unit> ToggleLoadingCommand { get; }

    public ToggleSwitchViewModel(IScreen screen)
    {
        HostScreen             = screen;
        ToggleDisabledCommand  = ReactiveCommand.Create(() => { IsDisabledDemoEnabled = !IsDisabledDemoEnabled; });
        ToggleLoadingCommand   = ReactiveCommand.Create(() => { IsLoadingDemoLoading = !IsLoadingDemoLoading; });
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new ToggleSwitchApiRow("IsChecked", Lang(ToggleSwitchShowCaseLangResourceKind.ApiPropertyIsChecked), "bool?", "purple", "false"),
            new ToggleSwitchApiRow("GrooveBackground", Lang(ToggleSwitchShowCaseLangResourceKind.ApiPropertyGrooveBackground), "IBrush?", "cyan", "null"),
            new ToggleSwitchApiRow("OnContent", Lang(ToggleSwitchShowCaseLangResourceKind.ApiPropertyOnContent), "object?", "cyan", "null"),
            new ToggleSwitchApiRow("OnContentTemplate", Lang(ToggleSwitchShowCaseLangResourceKind.ApiPropertyOnContentTemplate), "IDataTemplate?", "cyan", "null"),
            new ToggleSwitchApiRow("OffContent", Lang(ToggleSwitchShowCaseLangResourceKind.ApiPropertyOffContent), "object?", "cyan", "null"),
            new ToggleSwitchApiRow("OffContentTemplate", Lang(ToggleSwitchShowCaseLangResourceKind.ApiPropertyOffContentTemplate), "IDataTemplate?", "cyan", "null"),
            new ToggleSwitchApiRow("SizeType", Lang(ToggleSwitchShowCaseLangResourceKind.ApiPropertySizeType), "SizeType", "blue", "Middle"),
            new ToggleSwitchApiRow("IsLoading", Lang(ToggleSwitchShowCaseLangResourceKind.ApiPropertyIsLoading), "bool", "purple", "false"),
            new ToggleSwitchApiRow("IsMotionEnabled", Lang(ToggleSwitchShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "token"),
            new ToggleSwitchApiRow("IsWaveSpiritEnabled", Lang(ToggleSwitchShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled), "bool", "purple", "token")
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
            new ToggleSwitchDesignTokenRow("TrackHeight", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameTrackHeight), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("TrackHeightSM", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameTrackHeightSM), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("TrackMinWidth", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameTrackMinWidth), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("TrackMinWidthSM", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameTrackMinWidthSM), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("TrackPadding", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameTrackPadding), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("HandleBg", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameHandleBg), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("HandleShadow", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameHandleShadow), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("HandleSize", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameHandleSize), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("HandleSizeSM", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameHandleSizeSM), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("InnerMinMargin", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameInnerMinMargin), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("InnerMaxMargin", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameInnerMaxMargin), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("InnerMinMarginSM", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameInnerMinMarginSM), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("InnerMaxMarginSM", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameInnerMaxMarginSM), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("IconSize", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameIconSize), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("IconSizeSM", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameIconSizeSM), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("SwitchColor", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameSwitchColor), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("SwitchDisabledOpacity", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameSwitchDisabledOpacity), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("ExtraInfoFontSize", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameExtraInfoFontSize), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("ExtraInfoFontSizeSM", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameExtraInfoFontSizeSM), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("LoadingAnimationDuration", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameLoadingAnimationDuration), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success"),
            new ToggleSwitchDesignTokenRow("OffStateLoadIndicatorColor", Lang(ToggleSwitchShowCaseLangResourceKind.TokenNameOffStateLoadIndicatorColor), Lang(ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(ToggleSwitchShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(ToggleSwitchShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(ToggleSwitchShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            ToggleSwitchShowCaseLangResourceKind.ApiPropertyIsChecked                  => en_US.ApiPropertyIsChecked,
            ToggleSwitchShowCaseLangResourceKind.ApiPropertyGrooveBackground           => en_US.ApiPropertyGrooveBackground,
            ToggleSwitchShowCaseLangResourceKind.ApiPropertyOnContent                  => en_US.ApiPropertyOnContent,
            ToggleSwitchShowCaseLangResourceKind.ApiPropertyOnContentTemplate          => en_US.ApiPropertyOnContentTemplate,
            ToggleSwitchShowCaseLangResourceKind.ApiPropertyOffContent                 => en_US.ApiPropertyOffContent,
            ToggleSwitchShowCaseLangResourceKind.ApiPropertyOffContentTemplate         => en_US.ApiPropertyOffContentTemplate,
            ToggleSwitchShowCaseLangResourceKind.ApiPropertySizeType                   => en_US.ApiPropertySizeType,
            ToggleSwitchShowCaseLangResourceKind.ApiPropertyIsLoading                  => en_US.ApiPropertyIsLoading,
            ToggleSwitchShowCaseLangResourceKind.ApiPropertyIsMotionEnabled            => en_US.ApiPropertyIsMotionEnabled,
            ToggleSwitchShowCaseLangResourceKind.ApiPropertyIsWaveSpiritEnabled        => en_US.ApiPropertyIsWaveSpiritEnabled,
            ToggleSwitchShowCaseLangResourceKind.TokenNameTrackHeight                  => en_US.TokenNameTrackHeight,
            ToggleSwitchShowCaseLangResourceKind.TokenNameTrackHeightSM                => en_US.TokenNameTrackHeightSM,
            ToggleSwitchShowCaseLangResourceKind.TokenNameTrackMinWidth                => en_US.TokenNameTrackMinWidth,
            ToggleSwitchShowCaseLangResourceKind.TokenNameTrackMinWidthSM              => en_US.TokenNameTrackMinWidthSM,
            ToggleSwitchShowCaseLangResourceKind.TokenNameTrackPadding                 => en_US.TokenNameTrackPadding,
            ToggleSwitchShowCaseLangResourceKind.TokenNameHandleBg                     => en_US.TokenNameHandleBg,
            ToggleSwitchShowCaseLangResourceKind.TokenNameHandleShadow                 => en_US.TokenNameHandleShadow,
            ToggleSwitchShowCaseLangResourceKind.TokenNameHandleSize                   => en_US.TokenNameHandleSize,
            ToggleSwitchShowCaseLangResourceKind.TokenNameHandleSizeSM                 => en_US.TokenNameHandleSizeSM,
            ToggleSwitchShowCaseLangResourceKind.TokenNameInnerMinMargin               => en_US.TokenNameInnerMinMargin,
            ToggleSwitchShowCaseLangResourceKind.TokenNameInnerMaxMargin               => en_US.TokenNameInnerMaxMargin,
            ToggleSwitchShowCaseLangResourceKind.TokenNameInnerMinMarginSM             => en_US.TokenNameInnerMinMarginSM,
            ToggleSwitchShowCaseLangResourceKind.TokenNameInnerMaxMarginSM             => en_US.TokenNameInnerMaxMarginSM,
            ToggleSwitchShowCaseLangResourceKind.TokenNameIconSize                     => en_US.TokenNameIconSize,
            ToggleSwitchShowCaseLangResourceKind.TokenNameIconSizeSM                   => en_US.TokenNameIconSizeSM,
            ToggleSwitchShowCaseLangResourceKind.TokenNameSwitchColor                  => en_US.TokenNameSwitchColor,
            ToggleSwitchShowCaseLangResourceKind.TokenNameSwitchDisabledOpacity        => en_US.TokenNameSwitchDisabledOpacity,
            ToggleSwitchShowCaseLangResourceKind.TokenNameExtraInfoFontSize            => en_US.TokenNameExtraInfoFontSize,
            ToggleSwitchShowCaseLangResourceKind.TokenNameExtraInfoFontSizeSM          => en_US.TokenNameExtraInfoFontSizeSM,
            ToggleSwitchShowCaseLangResourceKind.TokenNameLoadingAnimationDuration     => en_US.TokenNameLoadingAnimationDuration,
            ToggleSwitchShowCaseLangResourceKind.TokenNameOffStateLoadIndicatorColor   => en_US.TokenNameOffStateLoadIndicatorColor,
            ToggleSwitchShowCaseLangResourceKind.TokenScopeComponent                   => en_US.TokenScopeComponent,
            ToggleSwitchShowCaseLangResourceKind.TokenStatusStable                     => en_US.TokenStatusStable,
            _                                                                          => kind.ToString()
        };
    }
}

public sealed record ToggleSwitchApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record ToggleSwitchDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
