using System.Collections.ObjectModel;
using System.Globalization;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using AtomUIGallery.Localization;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.DatePicker;

public class DatePickerViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "DatePicker";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<DatePickerApiRow>? _apiRows;
    private ObservableCollection<DatePickerDesignTokenRow>? _designTokenRows;

    public ObservableCollection<DatePickerApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<DatePickerDesignTokenRow>? DesignTokenRows
    {
        get => _designTokenRows;
        private set => this.RaiseAndSetIfChanged(ref _designTokenRows, value);
    }

    private CustomizableSizeType _pickerSizeType = CustomizableSizeType.Middle;

    public CustomizableSizeType PickerSizeType
    {
        get => _pickerSizeType;
        set => this.RaiseAndSetIfChanged(ref _pickerSizeType, value);
    }

    private PlacementMode _pickerPlacement;

    public PlacementMode PickerPlacement
    {
        get => _pickerPlacement;
        set => this.RaiseAndSetIfChanged(ref _pickerPlacement, value);
    }

    private DateTime? _boundSelectedDateTime = new DateTime(2026, 7, 5);

    public DateTime? BoundSelectedDateTime
    {
        get => _boundSelectedDateTime;
        set
        {
            this.RaiseAndSetIfChanged(ref _boundSelectedDateTime, value);
            this.RaisePropertyChanged(nameof(BoundSelectedDateTimeText));
        }
    }

    public string BoundSelectedDateTimeText => BoundSelectedDateTime?.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) ?? "-";

    private DateTime? _boundRangeStartSelectedDate = new DateTime(2026, 7, 6);

    public DateTime? BoundRangeStartSelectedDate
    {
        get => _boundRangeStartSelectedDate;
        set
        {
            this.RaiseAndSetIfChanged(ref _boundRangeStartSelectedDate, value);
            this.RaisePropertyChanged(nameof(BoundRangeSelectedDateText));
        }
    }

    private DateTime? _boundRangeEndSelectedDate = new DateTime(2026, 7, 12);

    public DateTime? BoundRangeEndSelectedDate
    {
        get => _boundRangeEndSelectedDate;
        set
        {
            this.RaiseAndSetIfChanged(ref _boundRangeEndSelectedDate, value);
            this.RaisePropertyChanged(nameof(BoundRangeSelectedDateText));
        }
    }

    public string BoundRangeSelectedDateText
    {
        get
        {
            var startText = BoundRangeStartSelectedDate?.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) ?? "-";
            var endText   = BoundRangeEndSelectedDate?.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) ?? "-";
            return $"{startText} → {endText}";
        }
    }

    public DatePickerViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void HandlePickerSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        PickerSizeType = args.Index switch
        {
            0 => CustomizableSizeType.Large,
            2 => CustomizableSizeType.Small,
            3 => CustomizableSizeType.Custom,
            _ => CustomizableSizeType.Middle
        };
    }

    public void HandlePickerPlacementCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            PickerPlacement = PlacementMode.TopEdgeAlignedLeft;
        }
        else if (args.Index == 1)
        {
            PickerPlacement = PlacementMode.TopEdgeAlignedRight;
        }
        else if (args.Index == 2)
        {
            PickerPlacement = PlacementMode.BottomEdgeAlignedLeft;
        }
        else
        {
            PickerPlacement = PlacementMode.BottomEdgeAlignedRight;
        }
    }

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new DatePickerApiRow("SelectedDateTime", Lang(DatePickerShowCaseLangResourceKind.ApiPropertySelectedDateTime), "DateTime?", "cyan", "null"),
            new DatePickerApiRow("DefaultDateTime", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyDefaultDateTime), "DateTime?", "cyan", "null"),
            new DatePickerApiRow("PickerDisplayDate", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyPickerDisplayDate), "DateTime?", "cyan", "null"),
            new DatePickerApiRow("RangeDatePicker.RangeStartSelectedDate", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyRangeStartSelectedDate), "DateTime?", "cyan", "null"),
            new DatePickerApiRow("RangeDatePicker.RangeEndSelectedDate", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyRangeEndSelectedDate), "DateTime?", "cyan", "null"),
            new DatePickerApiRow("Format", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyFormat), "string?", "cyan", "null"),
            new DatePickerApiRow("PickerMode", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyPickerMode), "DatePickerMode", "blue", "Date"),
            new DatePickerApiRow("IsShowTime", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyIsShowTime), "bool", "green", "false"),
            new DatePickerApiRow("IsNeedConfirm", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyIsNeedConfirm), "bool", "green", "false"),
            new DatePickerApiRow("ClockIdentifier", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyClockIdentifier), "ClockIdentifierType", "blue", "HourClock12"),
            new DatePickerApiRow("PickerPlacement", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyPickerPlacement), "PlacementMode", "blue", "BottomEdgeAlignedLeft"),
            new DatePickerApiRow("RangeDatePicker.SecondaryPlaceholderText", Lang(DatePickerShowCaseLangResourceKind.ApiPropertySecondaryPlaceholderText), "string?", "cyan", "null")
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
            new DatePickerDesignTokenRow("CellHoverBg", Lang(DatePickerShowCaseLangResourceKind.TokenNameCellHoverBg), Lang(DatePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DatePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DatePickerDesignTokenRow("CellActiveWithRangeBg", Lang(DatePickerShowCaseLangResourceKind.TokenNameCellActiveWithRangeBg), Lang(DatePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DatePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DatePickerDesignTokenRow("CellHoverWithRangeBg", Lang(DatePickerShowCaseLangResourceKind.TokenNameCellHoverWithRangeBg), Lang(DatePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DatePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DatePickerDesignTokenRow("CellBgDisabled", Lang(DatePickerShowCaseLangResourceKind.TokenNameCellBgDisabled), Lang(DatePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DatePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DatePickerDesignTokenRow("CellRangeBorderColor", Lang(DatePickerShowCaseLangResourceKind.TokenNameCellRangeBorderColor), Lang(DatePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DatePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DatePickerDesignTokenRow("CellWidth", Lang(DatePickerShowCaseLangResourceKind.TokenNameCellWidth), Lang(DatePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DatePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DatePickerDesignTokenRow("CellHeight", Lang(DatePickerShowCaseLangResourceKind.TokenNameCellHeight), Lang(DatePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DatePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DatePickerDesignTokenRow("PanelContentPadding", Lang(DatePickerShowCaseLangResourceKind.TokenNamePanelContentPadding), Lang(DatePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DatePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DatePickerDesignTokenRow("ItemPanelMinWidth", Lang(DatePickerShowCaseLangResourceKind.TokenNameItemPanelMinWidth), Lang(DatePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DatePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new DatePickerDesignTokenRow("RangeCalendarSpacing", Lang(DatePickerShowCaseLangResourceKind.TokenNameRangeCalendarSpacing), Lang(DatePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(DatePickerShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(DatePickerShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(DatePickerShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            DatePickerShowCaseLangResourceKind.ApiPropertySelectedDateTime            => en_US.ApiPropertySelectedDateTime,
            DatePickerShowCaseLangResourceKind.ApiPropertyDefaultDateTime             => en_US.ApiPropertyDefaultDateTime,
            DatePickerShowCaseLangResourceKind.ApiPropertyPickerDisplayDate           => en_US.ApiPropertyPickerDisplayDate,
            DatePickerShowCaseLangResourceKind.ApiPropertyRangeStartSelectedDate      => en_US.ApiPropertyRangeStartSelectedDate,
            DatePickerShowCaseLangResourceKind.ApiPropertyRangeEndSelectedDate        => en_US.ApiPropertyRangeEndSelectedDate,
            DatePickerShowCaseLangResourceKind.ApiPropertyFormat                      => en_US.ApiPropertyFormat,
            DatePickerShowCaseLangResourceKind.ApiPropertyPickerMode                  => en_US.ApiPropertyPickerMode,
            DatePickerShowCaseLangResourceKind.ApiPropertyIsShowTime                  => en_US.ApiPropertyIsShowTime,
            DatePickerShowCaseLangResourceKind.ApiPropertyIsNeedConfirm               => en_US.ApiPropertyIsNeedConfirm,
            DatePickerShowCaseLangResourceKind.ApiPropertyClockIdentifier             => en_US.ApiPropertyClockIdentifier,
            DatePickerShowCaseLangResourceKind.ApiPropertyPickerPlacement             => en_US.ApiPropertyPickerPlacement,
            DatePickerShowCaseLangResourceKind.ApiPropertySecondaryPlaceholderText     => en_US.ApiPropertySecondaryPlaceholderText,
            DatePickerShowCaseLangResourceKind.BindingDescription                     => en_US.BindingDescription,
            DatePickerShowCaseLangResourceKind.BindingTitle                           => en_US.BindingTitle,
            DatePickerShowCaseLangResourceKind.TokenNameCellHoverBg                   => en_US.TokenNameCellHoverBg,
            DatePickerShowCaseLangResourceKind.TokenNameCellActiveWithRangeBg         => en_US.TokenNameCellActiveWithRangeBg,
            DatePickerShowCaseLangResourceKind.TokenNameCellHoverWithRangeBg          => en_US.TokenNameCellHoverWithRangeBg,
            DatePickerShowCaseLangResourceKind.TokenNameCellBgDisabled                => en_US.TokenNameCellBgDisabled,
            DatePickerShowCaseLangResourceKind.TokenNameCellRangeBorderColor          => en_US.TokenNameCellRangeBorderColor,
            DatePickerShowCaseLangResourceKind.TokenNameCellWidth                     => en_US.TokenNameCellWidth,
            DatePickerShowCaseLangResourceKind.TokenNameCellHeight                    => en_US.TokenNameCellHeight,
            DatePickerShowCaseLangResourceKind.TokenNamePanelContentPadding           => en_US.TokenNamePanelContentPadding,
            DatePickerShowCaseLangResourceKind.TokenNameItemPanelMinWidth             => en_US.TokenNameItemPanelMinWidth,
            DatePickerShowCaseLangResourceKind.TokenNameRangeCalendarSpacing          => en_US.TokenNameRangeCalendarSpacing,
            DatePickerShowCaseLangResourceKind.TokenScopeComponent                    => en_US.TokenScopeComponent,
            DatePickerShowCaseLangResourceKind.TokenStatusStable                      => en_US.TokenStatusStable,
            DatePickerShowCaseLangResourceKind.P2ContentClear                         => en_US.P2ContentClear,
            DatePickerShowCaseLangResourceKind.P2ContentSetThisWeek                   => en_US.P2ContentSetThisWeek,
            DatePickerShowCaseLangResourceKind.P2ContentSetTomorrow                   => en_US.P2ContentSetTomorrow,
            DatePickerShowCaseLangResourceKind.P2TextSelectedDateRange                => en_US.P2TextSelectedDateRange,
            DatePickerShowCaseLangResourceKind.P2TextSelectedDateTime                 => en_US.P2TextSelectedDateTime,
            _                                                                         => kind.ToString()
        };
    }
}

public sealed record DatePickerApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record DatePickerDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
