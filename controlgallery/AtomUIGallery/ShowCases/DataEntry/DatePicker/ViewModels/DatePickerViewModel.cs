using System.Collections.ObjectModel;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUI.Desktop.Controls;
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

    private SizeType _pickerSizeType = SizeType.Middle;

    public SizeType PickerSizeType
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

    public DatePickerViewModel(IScreen screen)
    {
        HostScreen = screen;
    }

    public void HandlePickerSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
    {
        if (args.Index == 0)
        {
            PickerSizeType = SizeType.Large;
        }
        else if (args.Index == 1)
        {
            PickerSizeType = SizeType.Middle;
        }
        else
        {
            PickerSizeType = SizeType.Small;
        }
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
            new DatePickerApiRow("RangeDatePicker.RangeStartSelectedDate", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyRangeStartSelectedDate), "DateTime?", "cyan", "null"),
            new DatePickerApiRow("RangeDatePicker.RangeEndSelectedDate", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyRangeEndSelectedDate), "DateTime?", "cyan", "null"),
            new DatePickerApiRow("Format", Lang(DatePickerShowCaseLangResourceKind.ApiPropertyFormat), "string?", "cyan", "null"),
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
            DatePickerShowCaseLangResourceKind.ApiPropertyRangeStartSelectedDate      => en_US.ApiPropertyRangeStartSelectedDate,
            DatePickerShowCaseLangResourceKind.ApiPropertyRangeEndSelectedDate        => en_US.ApiPropertyRangeEndSelectedDate,
            DatePickerShowCaseLangResourceKind.ApiPropertyFormat                      => en_US.ApiPropertyFormat,
            DatePickerShowCaseLangResourceKind.ApiPropertyIsShowTime                  => en_US.ApiPropertyIsShowTime,
            DatePickerShowCaseLangResourceKind.ApiPropertyIsNeedConfirm               => en_US.ApiPropertyIsNeedConfirm,
            DatePickerShowCaseLangResourceKind.ApiPropertyClockIdentifier             => en_US.ApiPropertyClockIdentifier,
            DatePickerShowCaseLangResourceKind.ApiPropertyPickerPlacement             => en_US.ApiPropertyPickerPlacement,
            DatePickerShowCaseLangResourceKind.ApiPropertySecondaryPlaceholderText     => en_US.ApiPropertySecondaryPlaceholderText,
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
