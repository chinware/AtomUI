using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Spin;

public class SpinViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Spin";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ObservableCollection<SpinApiRow>? _apiRows;
    private ObservableCollection<SpinDesignTokenRow>? _designTokenRows;

    public ObservableCollection<SpinApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<SpinDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private bool _isLoadingSwitchChecked;

    public bool IsLoadingSwitchChecked
    {
        get => _isLoadingSwitchChecked;
        set => this.RaiseAndSetIfChanged(ref _isLoadingSwitchChecked, value);
    }

    public SpinViewModel(IScreen screen)
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
            new SpinApiRow("Spin.SizeType", Lang(SpinShowCaseLangResourceKind.ApiPropertySizeType), "CustomizableSizeType", "cyan", "Middle"),
            new SpinApiRow("Spin.Tip", Lang(SpinShowCaseLangResourceKind.ApiPropertyTip), "string?", "cyan", "null"),
            new SpinApiRow("Spin.IsTipVisible", Lang(SpinShowCaseLangResourceKind.ApiPropertyIsTipVisible), "bool", "purple", "false"),
            new SpinApiRow("Spin.CustomIndicator", Lang(SpinShowCaseLangResourceKind.ApiPropertyCustomIndicator), "object?", "cyan", "null"),
            new SpinApiRow("Spin.CustomIndicatorTemplate", Lang(SpinShowCaseLangResourceKind.ApiPropertyCustomIndicatorTemplate), "IDataTemplate?", "cyan", "null"),
            new SpinApiRow("Spin.MotionDuration", Lang(SpinShowCaseLangResourceKind.ApiPropertyMotionDuration), "TimeSpan", "cyan", "token"),
            new SpinApiRow("Spin.MotionEasingCurve", Lang(SpinShowCaseLangResourceKind.ApiPropertyMotionEasingCurve), "Easing?", "cyan", "null"),
            new SpinApiRow("Spin.IsSpinning", Lang(SpinShowCaseLangResourceKind.ApiPropertyIsSpinning), "bool", "purple", "false"),
            new SpinApiRow("Spin.IsMaskBlurEnabled", Lang(SpinShowCaseLangResourceKind.ApiPropertyIsMaskBlurEnabled), "bool", "purple", "false"),
            new SpinApiRow("Spin.IsMaskBackgroundEnabled", Lang(SpinShowCaseLangResourceKind.ApiPropertyIsMaskBackgroundEnabled), "bool", "purple", "false"),
            new SpinApiRow("Spin.IsMotionEnabled", Lang(SpinShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "true"),
            new SpinApiRow("Spin.Content", Lang(SpinShowCaseLangResourceKind.ApiPropertyContent), "object?", "cyan", "null"),
            new SpinApiRow("SpinIndicator.SizeType", Lang(SpinShowCaseLangResourceKind.ApiPropertyIndicatorSizeType), "CustomizableSizeType", "cyan", "Middle"),
            new SpinApiRow("SpinIndicator.IndicatorSize", Lang(SpinShowCaseLangResourceKind.ApiPropertyIndicatorSize), "double", "blue", "token"),
            new SpinApiRow("SpinIndicator.DotSize", Lang(SpinShowCaseLangResourceKind.ApiPropertyIndicatorDotSize), "double", "blue", "token"),
            new SpinApiRow("SpinIndicator.CustomIndicator", Lang(SpinShowCaseLangResourceKind.ApiPropertyIndicatorCustomIndicator), "object?", "cyan", "null"),
            new SpinApiRow("SpinIndicator.CustomIndicatorTemplate", Lang(SpinShowCaseLangResourceKind.ApiPropertyIndicatorCustomIndicatorTemplate), "IDataTemplate?", "cyan", "null"),
            new SpinApiRow("SpinIndicator.MotionDuration", Lang(SpinShowCaseLangResourceKind.ApiPropertyIndicatorMotionDuration), "TimeSpan", "cyan", "token"),
            new SpinApiRow("SpinIndicator.MotionEasingCurve", Lang(SpinShowCaseLangResourceKind.ApiPropertyIndicatorMotionEasingCurve), "Easing?", "cyan", "null")
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
            new SpinDesignTokenRow("DotSize", Lang(SpinShowCaseLangResourceKind.TokenNameDotSize), Lang(SpinShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SpinShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SpinDesignTokenRow("DotSizeSM", Lang(SpinShowCaseLangResourceKind.TokenNameDotSizeSM), Lang(SpinShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SpinShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SpinDesignTokenRow("DotSizeLG", Lang(SpinShowCaseLangResourceKind.TokenNameDotSizeLG), Lang(SpinShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SpinShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SpinDesignTokenRow("IndicatorSize", Lang(SpinShowCaseLangResourceKind.TokenNameIndicatorSize), Lang(SpinShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SpinShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SpinDesignTokenRow("IndicatorSizeSM", Lang(SpinShowCaseLangResourceKind.TokenNameIndicatorSizeSM), Lang(SpinShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SpinShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SpinDesignTokenRow("IndicatorSizeLG", Lang(SpinShowCaseLangResourceKind.TokenNameIndicatorSizeLG), Lang(SpinShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SpinShowCaseLangResourceKind.TokenStatusStable), "success"),
            new SpinDesignTokenRow("IndicatorDuration", Lang(SpinShowCaseLangResourceKind.TokenNameIndicatorDuration), Lang(SpinShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(SpinShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(SpinShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(SpinShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            SpinShowCaseLangResourceKind.ApiPropertySizeType                         => en_US.ApiPropertySizeType,
            SpinShowCaseLangResourceKind.ApiPropertyTip                              => en_US.ApiPropertyTip,
            SpinShowCaseLangResourceKind.ApiPropertyIsTipVisible                     => en_US.ApiPropertyIsTipVisible,
            SpinShowCaseLangResourceKind.ApiPropertyCustomIndicator                  => en_US.ApiPropertyCustomIndicator,
            SpinShowCaseLangResourceKind.ApiPropertyCustomIndicatorTemplate          => en_US.ApiPropertyCustomIndicatorTemplate,
            SpinShowCaseLangResourceKind.ApiPropertyMotionDuration                   => en_US.ApiPropertyMotionDuration,
            SpinShowCaseLangResourceKind.ApiPropertyMotionEasingCurve                => en_US.ApiPropertyMotionEasingCurve,
            SpinShowCaseLangResourceKind.ApiPropertyIsSpinning                       => en_US.ApiPropertyIsSpinning,
            SpinShowCaseLangResourceKind.ApiPropertyIsMaskBlurEnabled                => en_US.ApiPropertyIsMaskBlurEnabled,
            SpinShowCaseLangResourceKind.ApiPropertyIsMaskBackgroundEnabled          => en_US.ApiPropertyIsMaskBackgroundEnabled,
            SpinShowCaseLangResourceKind.ApiPropertyIsMotionEnabled                  => en_US.ApiPropertyIsMotionEnabled,
            SpinShowCaseLangResourceKind.ApiPropertyContent                          => en_US.ApiPropertyContent,
            SpinShowCaseLangResourceKind.ApiPropertyIndicatorSizeType                => en_US.ApiPropertyIndicatorSizeType,
            SpinShowCaseLangResourceKind.ApiPropertyIndicatorSize                    => en_US.ApiPropertyIndicatorSize,
            SpinShowCaseLangResourceKind.ApiPropertyIndicatorDotSize                 => en_US.ApiPropertyIndicatorDotSize,
            SpinShowCaseLangResourceKind.ApiPropertyIndicatorCustomIndicator         => en_US.ApiPropertyIndicatorCustomIndicator,
            SpinShowCaseLangResourceKind.ApiPropertyIndicatorCustomIndicatorTemplate => en_US.ApiPropertyIndicatorCustomIndicatorTemplate,
            SpinShowCaseLangResourceKind.ApiPropertyIndicatorMotionDuration          => en_US.ApiPropertyIndicatorMotionDuration,
            SpinShowCaseLangResourceKind.ApiPropertyIndicatorMotionEasingCurve       => en_US.ApiPropertyIndicatorMotionEasingCurve,
            SpinShowCaseLangResourceKind.TokenNameDotSize                            => en_US.TokenNameDotSize,
            SpinShowCaseLangResourceKind.TokenNameDotSizeSM                          => en_US.TokenNameDotSizeSM,
            SpinShowCaseLangResourceKind.TokenNameDotSizeLG                          => en_US.TokenNameDotSizeLG,
            SpinShowCaseLangResourceKind.TokenNameIndicatorSize                      => en_US.TokenNameIndicatorSize,
            SpinShowCaseLangResourceKind.TokenNameIndicatorSizeSM                    => en_US.TokenNameIndicatorSizeSM,
            SpinShowCaseLangResourceKind.TokenNameIndicatorSizeLG                    => en_US.TokenNameIndicatorSizeLG,
            SpinShowCaseLangResourceKind.TokenNameIndicatorDuration                  => en_US.TokenNameIndicatorDuration,
            SpinShowCaseLangResourceKind.TokenScopeComponent                         => en_US.TokenScopeComponent,
            SpinShowCaseLangResourceKind.TokenStatusStable                           => en_US.TokenStatusStable,
            _                                                                        => kind.ToString()
        };
    }
}

public sealed record SpinApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record SpinDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
