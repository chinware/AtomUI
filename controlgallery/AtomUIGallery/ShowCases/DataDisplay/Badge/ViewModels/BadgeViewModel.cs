using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Badge;

public class BadgeViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Badge";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<BadgeApiRow>? _apiRows;
    private ObservableCollection<BadgeDesignTokenRow>? _designTokenRows;
    private double _dynamicBadgeCount = 5;

    public ObservableCollection<BadgeApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<BadgeDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public double DynamicBadgeCount
    {
        get => _dynamicBadgeCount;
        set => this.RaiseAndSetIfChanged(ref _dynamicBadgeCount, value);
    }

    private bool _dynamicDotBadgeVisible = true;

    public bool DynamicDotBadgeVisible
    {
        get => _dynamicDotBadgeVisible;
        set => this.RaiseAndSetIfChanged(ref _dynamicDotBadgeVisible, value);
    }

    private bool _standaloneSwitchChecked;

    public bool StandaloneSwitchChecked
    {
        get => _standaloneSwitchChecked;
        set
        {
            if (_standaloneSwitchChecked == value)
            {
                return;
            }

            this.RaiseAndSetIfChanged(ref _standaloneSwitchChecked, value);
            HandleStandaloneSwitchChecked(value);
        }
    }

    private double _standaloneBadgeCount1;

    public double StandaloneBadgeCount1
    {
        get => _standaloneBadgeCount1;
        set => this.RaiseAndSetIfChanged(ref _standaloneBadgeCount1, value);
    }

    private double _standaloneBadgeCount2;

    public double StandaloneBadgeCount2
    {
        get => _standaloneBadgeCount2;
        set => this.RaiseAndSetIfChanged(ref _standaloneBadgeCount2, value);
    }

    private double _standaloneBadgeCount3;

    public double StandaloneBadgeCount3
    {
        get => _standaloneBadgeCount3;
        set => this.RaiseAndSetIfChanged(ref _standaloneBadgeCount3, value);
    }

    public BadgeViewModel(IScreen screen)
    {
        HostScreen = screen;
        HandleStandaloneSwitchChecked(StandaloneSwitchChecked);
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new BadgeApiRow("Count", Lang(BadgeShowCaseLangResourceKind.ApiPropertyCount), "int", "green", "0"),
            new BadgeApiRow("OverflowCount", Lang(BadgeShowCaseLangResourceKind.ApiPropertyOverflowCount), "int", "green", "99"),
            new BadgeApiRow("BadgeColor", Lang(BadgeShowCaseLangResourceKind.ApiPropertyBadgeColor), "string?", "cyan", "null"),
            new BadgeApiRow("IsZeroVisible", Lang(BadgeShowCaseLangResourceKind.ApiPropertyIsZeroVisible), "bool", "green", "false"),
            new BadgeApiRow("Offset", Lang(BadgeShowCaseLangResourceKind.ApiPropertyOffset), "Point", "blue", "0,0"),
            new BadgeApiRow("Size", Lang(BadgeShowCaseLangResourceKind.ApiPropertySize), "CountBadgeSize", "blue", "Default"),
            new BadgeApiRow("BadgeIsVisible", Lang(BadgeShowCaseLangResourceKind.ApiPropertyBadgeIsVisible), "bool", "green", "true"),
            new BadgeApiRow("Status", Lang(BadgeShowCaseLangResourceKind.ApiPropertyStatus), "DotBadgeStatus?", "blue", "null"),
            new BadgeApiRow("DotColor", Lang(BadgeShowCaseLangResourceKind.ApiPropertyDotColor), "string?", "cyan", "null"),
            new BadgeApiRow("Text", Lang(BadgeShowCaseLangResourceKind.ApiPropertyText), "string?", "cyan", "null"),
            new BadgeApiRow("RibbonColor", Lang(BadgeShowCaseLangResourceKind.ApiPropertyRibbonColor), "string?", "cyan", "null"),
            new BadgeApiRow("Placement", Lang(BadgeShowCaseLangResourceKind.ApiPropertyPlacement), "RibbonBadgePlacement", "blue", "End")
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
            new BadgeDesignTokenRow(
                "BadgeColor",
                Lang(BadgeShowCaseLangResourceKind.TokenNameBadgeColor),
                Lang(BadgeShowCaseLangResourceKind.TokenScopeComponent),
                "cyan",
                Lang(BadgeShowCaseLangResourceKind.TokenStatusStable),
                "success"),
            new BadgeDesignTokenRow(
                "IndicatorHeight",
                Lang(BadgeShowCaseLangResourceKind.TokenNameIndicatorHeight),
                Lang(BadgeShowCaseLangResourceKind.TokenScopeComponent),
                "cyan",
                Lang(BadgeShowCaseLangResourceKind.TokenStatusStable),
                "success"),
            new BadgeDesignTokenRow(
                "IndicatorHeightSM",
                Lang(BadgeShowCaseLangResourceKind.TokenNameIndicatorHeightSM),
                Lang(BadgeShowCaseLangResourceKind.TokenScopeComponent),
                "cyan",
                Lang(BadgeShowCaseLangResourceKind.TokenStatusStable),
                "success"),
            new BadgeDesignTokenRow(
                "DotSize",
                Lang(BadgeShowCaseLangResourceKind.TokenNameDotSize),
                Lang(BadgeShowCaseLangResourceKind.TokenScopeComponent),
                "cyan",
                Lang(BadgeShowCaseLangResourceKind.TokenStatusStable),
                "success"),
            new BadgeDesignTokenRow(
                "TextFontSize",
                Lang(BadgeShowCaseLangResourceKind.TokenNameTextFontSize),
                Lang(BadgeShowCaseLangResourceKind.TokenScopeComponent),
                "cyan",
                Lang(BadgeShowCaseLangResourceKind.TokenStatusStable),
                "success"),
            new BadgeDesignTokenRow(
                "StatusSize",
                Lang(BadgeShowCaseLangResourceKind.TokenNameStatusSize),
                Lang(BadgeShowCaseLangResourceKind.TokenScopeComponent),
                "cyan",
                Lang(BadgeShowCaseLangResourceKind.TokenStatusStable),
                "success")
        ];
    }

    private void HandleStandaloneSwitchChecked(bool value)
    {
        if (value)
        {
            StandaloneBadgeCount1 = 11;
            StandaloneBadgeCount2 = 25;
            StandaloneBadgeCount3 = 109;
        }
        else
        {
            StandaloneBadgeCount1 = 0;
            StandaloneBadgeCount2 = 0;
            StandaloneBadgeCount3 = 0;
        }
    }

    public void AddDynamicBadgeCount()
    {
        DynamicBadgeCount += 1;
    }

    public void SubDynamicBadgeCount()
    {
        var value = DynamicBadgeCount;
        value             -= 1;
        value             =  Math.Max(value, 0);
        DynamicBadgeCount =  value;
    }

    public void RandomDynamicBadgeCount()
    {
        var random = new Random();
        DynamicBadgeCount = random.Next(0, 110);
    }

    private static string Lang(BadgeShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(BadgeShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            BadgeShowCaseLangResourceKind.ApiPropertyCount            => en_US.ApiPropertyCount,
            BadgeShowCaseLangResourceKind.ApiPropertyOverflowCount    => en_US.ApiPropertyOverflowCount,
            BadgeShowCaseLangResourceKind.ApiPropertyBadgeColor       => en_US.ApiPropertyBadgeColor,
            BadgeShowCaseLangResourceKind.ApiPropertyIsZeroVisible    => en_US.ApiPropertyIsZeroVisible,
            BadgeShowCaseLangResourceKind.ApiPropertyOffset           => en_US.ApiPropertyOffset,
            BadgeShowCaseLangResourceKind.ApiPropertySize             => en_US.ApiPropertySize,
            BadgeShowCaseLangResourceKind.ApiPropertyBadgeIsVisible   => en_US.ApiPropertyBadgeIsVisible,
            BadgeShowCaseLangResourceKind.ApiPropertyStatus           => en_US.ApiPropertyStatus,
            BadgeShowCaseLangResourceKind.ApiPropertyDotColor         => en_US.ApiPropertyDotColor,
            BadgeShowCaseLangResourceKind.ApiPropertyText             => en_US.ApiPropertyText,
            BadgeShowCaseLangResourceKind.ApiPropertyRibbonColor      => en_US.ApiPropertyRibbonColor,
            BadgeShowCaseLangResourceKind.ApiPropertyPlacement        => en_US.ApiPropertyPlacement,
            BadgeShowCaseLangResourceKind.TokenNameBadgeColor         => en_US.TokenNameBadgeColor,
            BadgeShowCaseLangResourceKind.TokenNameIndicatorHeight    => en_US.TokenNameIndicatorHeight,
            BadgeShowCaseLangResourceKind.TokenNameIndicatorHeightSM  => en_US.TokenNameIndicatorHeightSM,
            BadgeShowCaseLangResourceKind.TokenNameDotSize            => en_US.TokenNameDotSize,
            BadgeShowCaseLangResourceKind.TokenNameTextFontSize       => en_US.TokenNameTextFontSize,
            BadgeShowCaseLangResourceKind.TokenNameStatusSize         => en_US.TokenNameStatusSize,
            BadgeShowCaseLangResourceKind.TokenScopeComponent         => en_US.TokenScopeComponent,
            BadgeShowCaseLangResourceKind.TokenStatusStable           => en_US.TokenStatusStable,
            _                                                         => kind.ToString()
        };
    }
}

public sealed record BadgeApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record BadgeDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
