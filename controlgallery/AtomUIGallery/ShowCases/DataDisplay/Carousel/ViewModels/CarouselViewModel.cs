using System.Collections.ObjectModel;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Carousel;

public class CarouselViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Carousel";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private CarouselPaginationPosition _paginationPosition = CarouselPaginationPosition.Bottom;

    public CarouselPaginationPosition PaginationPosition
    {
        get => _paginationPosition;
        set => this.RaiseAndSetIfChanged(ref _paginationPosition, value);
    }

    private ObservableCollection<CarouselApiRow>? _apiRows;
    private ObservableCollection<CarouselDesignTokenRow>? _designTokenRows;

    public ObservableCollection<CarouselApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<CarouselDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public CarouselViewModel(IScreen screen)
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
            new CarouselApiRow("IsShowNavButtons", Lang(CarouselShowCaseLangResourceKind.ApiPropertyIsShowNavButtons), "bool", "purple", "false"),
            new CarouselApiRow("IsAutoPlay", Lang(CarouselShowCaseLangResourceKind.ApiPropertyIsAutoPlay), "bool", "purple", "false"),
            new CarouselApiRow("AutoPlaySpeed", Lang(CarouselShowCaseLangResourceKind.ApiPropertyAutoPlaySpeed), "TimeSpan", "blue", "3000ms"),
            new CarouselApiRow("PaginationPosition", Lang(CarouselShowCaseLangResourceKind.ApiPropertyPaginationPosition), "CarouselPaginationPosition", "blue", "Bottom"),
            new CarouselApiRow("IsShowPagination", Lang(CarouselShowCaseLangResourceKind.ApiPropertyIsShowPagination), "bool", "purple", "true"),
            new CarouselApiRow("IsShowTransitionProgress", Lang(CarouselShowCaseLangResourceKind.ApiPropertyIsShowTransitionProgress), "bool", "purple", "false"),
            new CarouselApiRow("IsInfinite", Lang(CarouselShowCaseLangResourceKind.ApiPropertyIsInfinite), "bool", "purple", "true"),
            new CarouselApiRow("PageTransitionDuration", Lang(CarouselShowCaseLangResourceKind.ApiPropertyPageTransitionDuration), "TimeSpan", "blue", "token"),
            new CarouselApiRow("TransitionEffect", Lang(CarouselShowCaseLangResourceKind.ApiPropertyTransitionEffect), "CarouselTransitionEffect", "blue", "Scroll"),
            new CarouselApiRow("IsMotionEnabled", Lang(CarouselShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "purple", "true"),
            new CarouselApiRow("IsSwipeEnabled", Lang(CarouselShowCaseLangResourceKind.ApiPropertyIsSwipeEnabled), "bool", "purple", "false")
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
            new CarouselDesignTokenRow("IndicatorWidth", Lang(CarouselShowCaseLangResourceKind.TokenNameIndicatorWidth), Lang(CarouselShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CarouselShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CarouselDesignTokenRow("IndicatorHeight", Lang(CarouselShowCaseLangResourceKind.TokenNameIndicatorHeight), Lang(CarouselShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CarouselShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CarouselDesignTokenRow("IndicatorGap", Lang(CarouselShowCaseLangResourceKind.TokenNameIndicatorGap), Lang(CarouselShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CarouselShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CarouselDesignTokenRow("PaginationOffset", Lang(CarouselShowCaseLangResourceKind.TokenNamePaginationOffset), Lang(CarouselShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CarouselShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CarouselDesignTokenRow("IndicatorActiveWidth", Lang(CarouselShowCaseLangResourceKind.TokenNameIndicatorActiveWidth), Lang(CarouselShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CarouselShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CarouselDesignTokenRow("ArrowSize", Lang(CarouselShowCaseLangResourceKind.TokenNameArrowSize), Lang(CarouselShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CarouselShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CarouselDesignTokenRow("ArrowOffset", Lang(CarouselShowCaseLangResourceKind.TokenNameArrowOffset), Lang(CarouselShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CarouselShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(CarouselShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(CarouselShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            CarouselShowCaseLangResourceKind.ApiPropertyIsShowNavButtons         => en_US.ApiPropertyIsShowNavButtons,
            CarouselShowCaseLangResourceKind.ApiPropertyIsAutoPlay              => en_US.ApiPropertyIsAutoPlay,
            CarouselShowCaseLangResourceKind.ApiPropertyAutoPlaySpeed           => en_US.ApiPropertyAutoPlaySpeed,
            CarouselShowCaseLangResourceKind.ApiPropertyPaginationPosition      => en_US.ApiPropertyPaginationPosition,
            CarouselShowCaseLangResourceKind.ApiPropertyIsShowPagination        => en_US.ApiPropertyIsShowPagination,
            CarouselShowCaseLangResourceKind.ApiPropertyIsShowTransitionProgress => en_US.ApiPropertyIsShowTransitionProgress,
            CarouselShowCaseLangResourceKind.ApiPropertyIsInfinite              => en_US.ApiPropertyIsInfinite,
            CarouselShowCaseLangResourceKind.ApiPropertyPageTransitionDuration  => en_US.ApiPropertyPageTransitionDuration,
            CarouselShowCaseLangResourceKind.ApiPropertyTransitionEffect        => en_US.ApiPropertyTransitionEffect,
            CarouselShowCaseLangResourceKind.ApiPropertyIsMotionEnabled         => en_US.ApiPropertyIsMotionEnabled,
            CarouselShowCaseLangResourceKind.ApiPropertyIsSwipeEnabled          => en_US.ApiPropertyIsSwipeEnabled,
            CarouselShowCaseLangResourceKind.TokenNameIndicatorWidth            => en_US.TokenNameIndicatorWidth,
            CarouselShowCaseLangResourceKind.TokenNameIndicatorHeight           => en_US.TokenNameIndicatorHeight,
            CarouselShowCaseLangResourceKind.TokenNameIndicatorGap              => en_US.TokenNameIndicatorGap,
            CarouselShowCaseLangResourceKind.TokenNamePaginationOffset          => en_US.TokenNamePaginationOffset,
            CarouselShowCaseLangResourceKind.TokenNameIndicatorActiveWidth      => en_US.TokenNameIndicatorActiveWidth,
            CarouselShowCaseLangResourceKind.TokenNameArrowSize                 => en_US.TokenNameArrowSize,
            CarouselShowCaseLangResourceKind.TokenNameArrowOffset               => en_US.TokenNameArrowOffset,
            CarouselShowCaseLangResourceKind.TokenScopeComponent                => en_US.TokenScopeComponent,
            CarouselShowCaseLangResourceKind.TokenStatusStable                  => en_US.TokenStatusStable,
            _                                                                   => kind.ToString()
        };
    }
}

public sealed record CarouselApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record CarouselDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
