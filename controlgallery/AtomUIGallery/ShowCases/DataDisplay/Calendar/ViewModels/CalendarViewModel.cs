using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.Calendar;

public class CalendarViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "Calendar";

    public IScreen HostScreen { get; }

    public string UrlPathSegment { get; } = ID.ToString();

    private ObservableCollection<CalendarApiRow>? _apiRows;
    private ObservableCollection<CalendarDesignTokenRow>? _designTokenRows;

    public ObservableCollection<CalendarApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<CalendarDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    public CalendarViewModel(IScreen screen)
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
            new CalendarApiRow("FirstDayOfWeek", Lang(CalendarShowCaseLangResourceKind.ApiPropertyFirstDayOfWeek), "DayOfWeek", "blue", "Current culture"),
            new CalendarApiRow("IsTodayHighlighted", Lang(CalendarShowCaseLangResourceKind.ApiPropertyIsTodayHighlighted), "bool", "green", "true"),
            new CalendarApiRow("HeaderBackground", Lang(CalendarShowCaseLangResourceKind.ApiPropertyHeaderBackground), "IBrush?", "cyan", "null"),
            new CalendarApiRow("DisplayMode", Lang(CalendarShowCaseLangResourceKind.ApiPropertyDisplayMode), "CalendarMode", "blue", "Month"),
            new CalendarApiRow("SelectionMode", Lang(CalendarShowCaseLangResourceKind.ApiPropertySelectionMode), "CalendarSelectionMode", "blue", "SingleRange"),
            new CalendarApiRow("SelectedDate", Lang(CalendarShowCaseLangResourceKind.ApiPropertySelectedDate), "DateTime?", "cyan", "null"),
            new CalendarApiRow("DisplayDate", Lang(CalendarShowCaseLangResourceKind.ApiPropertyDisplayDate), "DateTime", "cyan", "Today"),
            new CalendarApiRow("DisplayDateStart", Lang(CalendarShowCaseLangResourceKind.ApiPropertyDisplayDateStart), "DateTime?", "cyan", "null"),
            new CalendarApiRow("DisplayDateEnd", Lang(CalendarShowCaseLangResourceKind.ApiPropertyDisplayDateEnd), "DateTime?", "cyan", "null"),
            new CalendarApiRow("IsMotionEnabled", Lang(CalendarShowCaseLangResourceKind.ApiPropertyIsMotionEnabled), "bool", "green", "SharedToken.EnableMotion")
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
            new CalendarDesignTokenRow("CellHoverBg", Lang(CalendarShowCaseLangResourceKind.TokenNameCellHoverBg), Lang(CalendarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CalendarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CalendarDesignTokenRow("CellActiveWithRangeBg", Lang(CalendarShowCaseLangResourceKind.TokenNameCellActiveWithRangeBg), Lang(CalendarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CalendarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CalendarDesignTokenRow("CellHoverWithRangeBg", Lang(CalendarShowCaseLangResourceKind.TokenNameCellHoverWithRangeBg), Lang(CalendarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CalendarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CalendarDesignTokenRow("CellBgDisabled", Lang(CalendarShowCaseLangResourceKind.TokenNameCellBgDisabled), Lang(CalendarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CalendarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CalendarDesignTokenRow("CellRangeBorderColor", Lang(CalendarShowCaseLangResourceKind.TokenNameCellRangeBorderColor), Lang(CalendarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CalendarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CalendarDesignTokenRow("CellHeight", Lang(CalendarShowCaseLangResourceKind.TokenNameCellHeight), Lang(CalendarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CalendarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CalendarDesignTokenRow("CellWidth", Lang(CalendarShowCaseLangResourceKind.TokenNameCellWidth), Lang(CalendarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CalendarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CalendarDesignTokenRow("CellLineHeight", Lang(CalendarShowCaseLangResourceKind.TokenNameCellLineHeight), Lang(CalendarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CalendarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CalendarDesignTokenRow("PanelContentPadding", Lang(CalendarShowCaseLangResourceKind.TokenNamePanelContentPadding), Lang(CalendarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CalendarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CalendarDesignTokenRow("ItemPanelMinWidth", Lang(CalendarShowCaseLangResourceKind.TokenNameItemPanelMinWidth), Lang(CalendarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CalendarShowCaseLangResourceKind.TokenStatusStable), "success"),
            new CalendarDesignTokenRow("ItemPanelMinHeight", Lang(CalendarShowCaseLangResourceKind.TokenNameItemPanelMinHeight), Lang(CalendarShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(CalendarShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(CalendarShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(CalendarShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            CalendarShowCaseLangResourceKind.ApiPropertyFirstDayOfWeek      => en_US.ApiPropertyFirstDayOfWeek,
            CalendarShowCaseLangResourceKind.ApiPropertyIsTodayHighlighted  => en_US.ApiPropertyIsTodayHighlighted,
            CalendarShowCaseLangResourceKind.ApiPropertyHeaderBackground    => en_US.ApiPropertyHeaderBackground,
            CalendarShowCaseLangResourceKind.ApiPropertyDisplayMode         => en_US.ApiPropertyDisplayMode,
            CalendarShowCaseLangResourceKind.ApiPropertySelectionMode       => en_US.ApiPropertySelectionMode,
            CalendarShowCaseLangResourceKind.ApiPropertySelectedDate        => en_US.ApiPropertySelectedDate,
            CalendarShowCaseLangResourceKind.ApiPropertyDisplayDate         => en_US.ApiPropertyDisplayDate,
            CalendarShowCaseLangResourceKind.ApiPropertyDisplayDateStart    => en_US.ApiPropertyDisplayDateStart,
            CalendarShowCaseLangResourceKind.ApiPropertyDisplayDateEnd      => en_US.ApiPropertyDisplayDateEnd,
            CalendarShowCaseLangResourceKind.ApiPropertyIsMotionEnabled     => en_US.ApiPropertyIsMotionEnabled,
            CalendarShowCaseLangResourceKind.TokenNameCellHoverBg           => en_US.TokenNameCellHoverBg,
            CalendarShowCaseLangResourceKind.TokenNameCellActiveWithRangeBg => en_US.TokenNameCellActiveWithRangeBg,
            CalendarShowCaseLangResourceKind.TokenNameCellHoverWithRangeBg  => en_US.TokenNameCellHoverWithRangeBg,
            CalendarShowCaseLangResourceKind.TokenNameCellBgDisabled        => en_US.TokenNameCellBgDisabled,
            CalendarShowCaseLangResourceKind.TokenNameCellRangeBorderColor  => en_US.TokenNameCellRangeBorderColor,
            CalendarShowCaseLangResourceKind.TokenNameCellHeight            => en_US.TokenNameCellHeight,
            CalendarShowCaseLangResourceKind.TokenNameCellWidth             => en_US.TokenNameCellWidth,
            CalendarShowCaseLangResourceKind.TokenNameCellLineHeight        => en_US.TokenNameCellLineHeight,
            CalendarShowCaseLangResourceKind.TokenNamePanelContentPadding   => en_US.TokenNamePanelContentPadding,
            CalendarShowCaseLangResourceKind.TokenNameItemPanelMinWidth     => en_US.TokenNameItemPanelMinWidth,
            CalendarShowCaseLangResourceKind.TokenNameItemPanelMinHeight    => en_US.TokenNameItemPanelMinHeight,
            CalendarShowCaseLangResourceKind.TokenScopeComponent            => en_US.TokenScopeComponent,
            CalendarShowCaseLangResourceKind.TokenStatusStable              => en_US.TokenStatusStable,
            _                                                               => kind.ToString()
        };
    }
}

public sealed record CalendarApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record CalendarDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
