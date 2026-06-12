using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Statistic;

public class StatisticViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Statistic";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<StatisticApiRow>? _apiRows;
    private ObservableCollection<StatisticDesignTokenRow>? _designTokenRows;

    public ObservableCollection<StatisticApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<StatisticDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private double _values = 112893;

    public double Values
    {
        get => _values;
        set => this.RaiseAndSetIfChanged(ref _values, value);
    }

    private DateTime _deadline;

    public DateTime Deadline
    {
        get => _deadline;
        set => this.RaiseAndSetIfChanged(ref _deadline, value);
    }

    private DateTime _tenSecondsLater;

    public DateTime TenSecondsLater
    {
        get => _tenSecondsLater;
        set => this.RaiseAndSetIfChanged(ref _tenSecondsLater, value);
    }

    private DateTime _before;

    public DateTime Before
    {
        get => _before;
        set => this.RaiseAndSetIfChanged(ref _before, value);
    }

    public StatisticViewModel(IScreen screen)
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
            new StatisticApiRow("Header", Lang(StatisticShowCaseLangResourceKind.ApiPropertyHeader), "object?", "cyan", "null"),
            new StatisticApiRow("Value", Lang(StatisticShowCaseLangResourceKind.ApiPropertyValue), "object?", "cyan", "null"),
            new StatisticApiRow("Formatter", Lang(StatisticShowCaseLangResourceKind.ApiPropertyFormatter), "Func<Statistic, object?, string?>?", "purple", "null"),
            new StatisticApiRow("DecimalSeparator", Lang(StatisticShowCaseLangResourceKind.ApiPropertyDecimalSeparator), "string", "cyan", "\".\""),
            new StatisticApiRow("GroupSeparator", Lang(StatisticShowCaseLangResourceKind.ApiPropertyGroupSeparator), "string", "cyan", "\",\""),
            new StatisticApiRow("Precision", Lang(StatisticShowCaseLangResourceKind.ApiPropertyPrecision), "int", "green", "0"),
            new StatisticApiRow("IsLoading", Lang(StatisticShowCaseLangResourceKind.ApiPropertyIsLoading), "bool", "green", "false"),
            new StatisticApiRow("ValuePrefixAddOn", Lang(StatisticShowCaseLangResourceKind.ApiPropertyValuePrefixAddOn), "object?", "cyan", "null"),
            new StatisticApiRow("ValueSuffixAddOn", Lang(StatisticShowCaseLangResourceKind.ApiPropertyValueSuffixAddOn), "object?", "cyan", "null"),
            new StatisticApiRow("ContentForeground", Lang(StatisticShowCaseLangResourceKind.ApiPropertyContentForeground), "IBrush?", "cyan", "ColorTextHeading"),
            new StatisticApiRow("ContentFontSize", Lang(StatisticShowCaseLangResourceKind.ApiPropertyContentFontSize), "double", "green", "ContentFontSize"),
            new StatisticApiRow("TimerStatistic.Value", Lang(StatisticShowCaseLangResourceKind.ApiPropertyTimerValue), "DateTime", "blue", "default"),
            new StatisticApiRow("TimerStatistic.Format", Lang(StatisticShowCaseLangResourceKind.ApiPropertyFormat), "string?", "cyan", "null"),
            new StatisticApiRow("TimerStatistic.RefreshDuration", Lang(StatisticShowCaseLangResourceKind.ApiPropertyRefreshDuration), "TimeSpan", "blue", "10ms"),
            new StatisticApiRow("TimerStatistic.CountdownFinished", Lang(StatisticShowCaseLangResourceKind.ApiEventCountdownFinished), "event EventHandler?", "purple", "-"),
            new StatisticApiRow("StatisticCountUp.EndValue", Lang(StatisticShowCaseLangResourceKind.ApiPropertyEndValue), "double", "green", "0")
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
            new StatisticDesignTokenRow("TitleFontSize", Lang(StatisticShowCaseLangResourceKind.TokenNameTitleFontSize), Lang(StatisticShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StatisticShowCaseLangResourceKind.TokenStatusStable), "success"),
            new StatisticDesignTokenRow("ContentFontSize", Lang(StatisticShowCaseLangResourceKind.TokenNameContentFontSize), Lang(StatisticShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(StatisticShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(StatisticShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(StatisticShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            StatisticShowCaseLangResourceKind.ApiPropertyHeader             => en_US.ApiPropertyHeader,
            StatisticShowCaseLangResourceKind.ApiPropertyValue              => en_US.ApiPropertyValue,
            StatisticShowCaseLangResourceKind.ApiPropertyFormatter          => en_US.ApiPropertyFormatter,
            StatisticShowCaseLangResourceKind.ApiPropertyDecimalSeparator   => en_US.ApiPropertyDecimalSeparator,
            StatisticShowCaseLangResourceKind.ApiPropertyGroupSeparator     => en_US.ApiPropertyGroupSeparator,
            StatisticShowCaseLangResourceKind.ApiPropertyPrecision          => en_US.ApiPropertyPrecision,
            StatisticShowCaseLangResourceKind.ApiPropertyIsLoading          => en_US.ApiPropertyIsLoading,
            StatisticShowCaseLangResourceKind.ApiPropertyValuePrefixAddOn   => en_US.ApiPropertyValuePrefixAddOn,
            StatisticShowCaseLangResourceKind.ApiPropertyValueSuffixAddOn   => en_US.ApiPropertyValueSuffixAddOn,
            StatisticShowCaseLangResourceKind.ApiPropertyContentForeground  => en_US.ApiPropertyContentForeground,
            StatisticShowCaseLangResourceKind.ApiPropertyContentFontSize    => en_US.ApiPropertyContentFontSize,
            StatisticShowCaseLangResourceKind.ApiPropertyTimerValue         => en_US.ApiPropertyTimerValue,
            StatisticShowCaseLangResourceKind.ApiPropertyFormat             => en_US.ApiPropertyFormat,
            StatisticShowCaseLangResourceKind.ApiPropertyRefreshDuration    => en_US.ApiPropertyRefreshDuration,
            StatisticShowCaseLangResourceKind.ApiEventCountdownFinished     => en_US.ApiEventCountdownFinished,
            StatisticShowCaseLangResourceKind.ApiPropertyEndValue           => en_US.ApiPropertyEndValue,
            StatisticShowCaseLangResourceKind.TokenNameTitleFontSize        => en_US.TokenNameTitleFontSize,
            StatisticShowCaseLangResourceKind.TokenNameContentFontSize      => en_US.TokenNameContentFontSize,
            StatisticShowCaseLangResourceKind.TokenScopeComponent           => en_US.TokenScopeComponent,
            StatisticShowCaseLangResourceKind.TokenStatusStable             => en_US.TokenStatusStable,
            _                                                               => kind.ToString()
        };
    }
}

public sealed record StatisticApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record StatisticDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
