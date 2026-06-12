using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Timeline;

public class TimelineViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Timeline";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ObservableCollection<TimelineApiRow>? _apiRows;
    private ObservableCollection<TimelineDesignTokenRow>? _designTokenRows;

    public ObservableCollection<TimelineApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<TimelineDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public TimelineViewModel(IScreen screen)
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
            new TimelineApiRow("Mode", Lang(TimelineShowCaseLangResourceKind.ApiPropertyMode), "TimelineMode", "purple", "Left"),
            new TimelineApiRow("Pending", Lang(TimelineShowCaseLangResourceKind.ApiPropertyPending), "object?", "cyan", "null"),
            new TimelineApiRow("PendingIcon", Lang(TimelineShowCaseLangResourceKind.ApiPropertyPendingIcon), "PathIcon?", "cyan", "null"),
            new TimelineApiRow("IsReverse", Lang(TimelineShowCaseLangResourceKind.ApiPropertyIsReverse), "bool", "green", "false"),
            new TimelineApiRow("Label", Lang(TimelineShowCaseLangResourceKind.ApiPropertyLabel), "string?", "cyan", "null"),
            new TimelineApiRow("IndicatorIcon", Lang(TimelineShowCaseLangResourceKind.ApiPropertyIndicatorIcon), "PathIcon?", "cyan", "null"),
            new TimelineApiRow("IndicatorColor", Lang(TimelineShowCaseLangResourceKind.ApiPropertyIndicatorColor), "IBrush?", "cyan", "null")
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
            new TimelineDesignTokenRow("IndicatorTailColor", Lang(TimelineShowCaseLangResourceKind.TokenNameIndicatorTailColor), Lang(TimelineShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimelineShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimelineDesignTokenRow("IndicatorTailWidth", Lang(TimelineShowCaseLangResourceKind.TokenNameIndicatorTailWidth), Lang(TimelineShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimelineShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimelineDesignTokenRow("ItemPaddingBottom", Lang(TimelineShowCaseLangResourceKind.TokenNameItemPaddingBottom), Lang(TimelineShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimelineShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimelineDesignTokenRow("ItemPaddingBottomLG", Lang(TimelineShowCaseLangResourceKind.TokenNameItemPaddingBottomLG), Lang(TimelineShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimelineShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimelineDesignTokenRow("LastItemContentMinHeight", Lang(TimelineShowCaseLangResourceKind.TokenNameLastItemContentMinHeight), Lang(TimelineShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimelineShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimelineDesignTokenRow("IndicatorSize", Lang(TimelineShowCaseLangResourceKind.TokenNameIndicatorSize), Lang(TimelineShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimelineShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimelineDesignTokenRow("IndicatorDotSize", Lang(TimelineShowCaseLangResourceKind.TokenNameIndicatorDotSize), Lang(TimelineShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimelineShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimelineDesignTokenRow("IndicatorLeftModeMargin", Lang(TimelineShowCaseLangResourceKind.TokenNameIndicatorLeftModeMargin), Lang(TimelineShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimelineShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimelineDesignTokenRow("IndicatorRightModeMargin", Lang(TimelineShowCaseLangResourceKind.TokenNameIndicatorRightModeMargin), Lang(TimelineShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimelineShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimelineDesignTokenRow("IndicatorMiddleModeMargin", Lang(TimelineShowCaseLangResourceKind.TokenNameIndicatorMiddleModeMargin), Lang(TimelineShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimelineShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimelineDesignTokenRow("IndicatorDotBorderWidth", Lang(TimelineShowCaseLangResourceKind.TokenNameIndicatorDotBorderWidth), Lang(TimelineShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimelineShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(TimelineShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(TimelineShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            TimelineShowCaseLangResourceKind.ApiPropertyMode                    => en_US.ApiPropertyMode,
            TimelineShowCaseLangResourceKind.ApiPropertyPending                 => en_US.ApiPropertyPending,
            TimelineShowCaseLangResourceKind.ApiPropertyPendingIcon             => en_US.ApiPropertyPendingIcon,
            TimelineShowCaseLangResourceKind.ApiPropertyIsReverse               => en_US.ApiPropertyIsReverse,
            TimelineShowCaseLangResourceKind.ApiPropertyLabel                   => en_US.ApiPropertyLabel,
            TimelineShowCaseLangResourceKind.ApiPropertyIndicatorIcon           => en_US.ApiPropertyIndicatorIcon,
            TimelineShowCaseLangResourceKind.ApiPropertyIndicatorColor          => en_US.ApiPropertyIndicatorColor,
            TimelineShowCaseLangResourceKind.TokenNameIndicatorTailColor        => en_US.TokenNameIndicatorTailColor,
            TimelineShowCaseLangResourceKind.TokenNameIndicatorTailWidth        => en_US.TokenNameIndicatorTailWidth,
            TimelineShowCaseLangResourceKind.TokenNameItemPaddingBottom         => en_US.TokenNameItemPaddingBottom,
            TimelineShowCaseLangResourceKind.TokenNameItemPaddingBottomLG       => en_US.TokenNameItemPaddingBottomLG,
            TimelineShowCaseLangResourceKind.TokenNameLastItemContentMinHeight  => en_US.TokenNameLastItemContentMinHeight,
            TimelineShowCaseLangResourceKind.TokenNameIndicatorSize             => en_US.TokenNameIndicatorSize,
            TimelineShowCaseLangResourceKind.TokenNameIndicatorDotSize          => en_US.TokenNameIndicatorDotSize,
            TimelineShowCaseLangResourceKind.TokenNameIndicatorLeftModeMargin   => en_US.TokenNameIndicatorLeftModeMargin,
            TimelineShowCaseLangResourceKind.TokenNameIndicatorRightModeMargin  => en_US.TokenNameIndicatorRightModeMargin,
            TimelineShowCaseLangResourceKind.TokenNameIndicatorMiddleModeMargin => en_US.TokenNameIndicatorMiddleModeMargin,
            TimelineShowCaseLangResourceKind.TokenNameIndicatorDotBorderWidth   => en_US.TokenNameIndicatorDotBorderWidth,
            TimelineShowCaseLangResourceKind.TokenScopeComponent                => en_US.TokenScopeComponent,
            TimelineShowCaseLangResourceKind.TokenStatusStable                  => en_US.TokenStatusStable,
            _                                                                   => kind.ToString()
        };
    }
}

public sealed record TimelineApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record TimelineDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
