using System.Collections.ObjectModel;
using System.Globalization;
using AtomUI;
using AtomUI.Controls;
using AtomUI.Data;
using AtomUIGallery.Localization;
using Avalonia;
using Avalonia.Threading;
using ReactiveUI;

namespace AtomUIGallery.ShowCases.TimePicker;

public class TimePickerViewModel : ReactiveObject, IRoutableViewModel
{
    public static EntityKey ID = "TimePicker";

    public IScreen HostScreen { get; }

    public string? UrlPathSegment => ID.ToString();

    private ObservableCollection<TimePickerApiRow>? _apiRows;
    private ObservableCollection<TimePickerDesignTokenRow>? _designTokenRows;

    public ObservableCollection<TimePickerApiRow>? ApiRows
    {
        get => _apiRows;
        private set => this.RaiseAndSetIfChanged(ref _apiRows, value);
    }

    public ObservableCollection<TimePickerDesignTokenRow>? DesignTokenRows
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

    private TimeSpan? _boundSelectedTime = new(10, 9, 20);

    public TimeSpan? BoundSelectedTime
    {
        get => _boundSelectedTime;
        set
        {
            this.RaiseAndSetIfChanged(ref _boundSelectedTime, value);
            this.RaisePropertyChanged(nameof(BoundSelectedTimeText));
        }
    }

    public string BoundSelectedTimeText => BoundSelectedTime?.ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture) ?? "-";

    private TimeSpan? _boundRangeStartSelectedTime = new(9, 0, 0);

    public TimeSpan? BoundRangeStartSelectedTime
    {
        get => _boundRangeStartSelectedTime;
        set
        {
            this.RaiseAndSetIfChanged(ref _boundRangeStartSelectedTime, value);
            this.RaisePropertyChanged(nameof(BoundRangeSelectedTimeText));
        }
    }

    private TimeSpan? _boundRangeEndSelectedTime = new(18, 0, 0);

    public TimeSpan? BoundRangeEndSelectedTime
    {
        get => _boundRangeEndSelectedTime;
        set
        {
            this.RaiseAndSetIfChanged(ref _boundRangeEndSelectedTime, value);
            this.RaisePropertyChanged(nameof(BoundRangeSelectedTimeText));
        }
    }

    public string BoundRangeSelectedTimeText
    {
        get
        {
            var startText = BoundRangeStartSelectedTime?.ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture) ?? "-";
            var endText   = BoundRangeEndSelectedTime?.ToString(@"hh\:mm\:ss", CultureInfo.CurrentCulture) ?? "-";
            return $"{startText} → {endText}";
        }
    }

    public TimePickerViewModel(IScreen screen)
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

    public void EnsureApiRows()
    {
        if (ApiRows is not null)
        {
            return;
        }

        ApiRows =
        [
            new TimePickerApiRow("TimePicker.SelectedTime", Lang(TimePickerShowCaseLangResourceKind.ApiPropertySelectedTime), "TimeSpan?", "cyan", "null"),
            new TimePickerApiRow("TimePicker.DefaultTime", Lang(TimePickerShowCaseLangResourceKind.ApiPropertyDefaultTime), "TimeSpan?", "cyan", "null"),
            new TimePickerApiRow("TimePicker.PickerDisplayTime", Lang(TimePickerShowCaseLangResourceKind.ApiPropertyPickerDisplayTime), "TimeSpan?", "cyan", "null"),
            new TimePickerApiRow("TimePicker.IsNeedConfirm", Lang(TimePickerShowCaseLangResourceKind.ApiPropertyIsNeedConfirm), "bool", "green", "false"),
            new TimePickerApiRow("TimePicker.IsShowNow", Lang(TimePickerShowCaseLangResourceKind.ApiPropertyIsShowNow), "bool", "green", "true"),
            new TimePickerApiRow("TimePicker.MinuteIncrement", Lang(TimePickerShowCaseLangResourceKind.ApiPropertyMinuteIncrement), "int", "cyan", "1"),
            new TimePickerApiRow("TimePicker.SecondIncrement", Lang(TimePickerShowCaseLangResourceKind.ApiPropertySecondIncrement), "int", "cyan", "1"),
            new TimePickerApiRow("TimePicker.ClockIdentifier", Lang(TimePickerShowCaseLangResourceKind.ApiPropertyClockIdentifier), "ClockIdentifierType", "blue", "HourClock12"),
            new TimePickerApiRow("RangeTimePicker.RangeStartSelectedTime", Lang(TimePickerShowCaseLangResourceKind.ApiPropertyRangeStartSelectedTime), "TimeSpan?", "cyan", "null"),
            new TimePickerApiRow("RangeTimePicker.RangeEndSelectedTime", Lang(TimePickerShowCaseLangResourceKind.ApiPropertyRangeEndSelectedTime), "TimeSpan?", "cyan", "null"),
            new TimePickerApiRow("RangeTimePicker.RangeStartDefaultTime", Lang(TimePickerShowCaseLangResourceKind.ApiPropertyRangeStartDefaultTime), "TimeSpan?", "cyan", "null"),
            new TimePickerApiRow("RangeTimePicker.RangeEndDefaultTime", Lang(TimePickerShowCaseLangResourceKind.ApiPropertyRangeEndDefaultTime), "TimeSpan?", "cyan", "null")
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
            new TimePickerDesignTokenRow("ItemHeight", Lang(TimePickerShowCaseLangResourceKind.TokenNameItemHeight), Lang(TimePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimePickerDesignTokenRow("ItemWidth", Lang(TimePickerShowCaseLangResourceKind.TokenNameItemWidth), Lang(TimePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimePickerDesignTokenRow("PeriodHostWidth", Lang(TimePickerShowCaseLangResourceKind.TokenNamePeriodHostWidth), Lang(TimePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimePickerDesignTokenRow("ItemPadding", Lang(TimePickerShowCaseLangResourceKind.TokenNameItemPadding), Lang(TimePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimePickerDesignTokenRow("ButtonsMargin", Lang(TimePickerShowCaseLangResourceKind.TokenNameButtonsMargin), Lang(TimePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimePickerDesignTokenRow("RangePickerArrowMargin", Lang(TimePickerShowCaseLangResourceKind.TokenNameRangePickerArrowMargin), Lang(TimePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimePickerDesignTokenRow("RangePickerIndicatorThickness", Lang(TimePickerShowCaseLangResourceKind.TokenNameRangePickerIndicatorThickness), Lang(TimePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimePickerShowCaseLangResourceKind.TokenStatusStable), "success"),
            new TimePickerDesignTokenRow("HeaderMargin", Lang(TimePickerShowCaseLangResourceKind.TokenNameHeaderMargin), Lang(TimePickerShowCaseLangResourceKind.TokenScopeComponent), "cyan", Lang(TimePickerShowCaseLangResourceKind.TokenStatusStable), "success")
        ];
    }

    private static string Lang(TimePickerShowCaseLangResourceKind kind)
    {
        if (Application.Current is not null && Dispatcher.UIThread.CheckAccess())
        {
            return LanguageResourceBinder.GetLangResource(kind) ?? FallbackLang(kind);
        }

        return FallbackLang(kind);
    }

    private static string FallbackLang(TimePickerShowCaseLangResourceKind kind)
    {
        return kind switch
        {
            TimePickerShowCaseLangResourceKind.ApiPropertySelectedTime                  => en_US.ApiPropertySelectedTime,
            TimePickerShowCaseLangResourceKind.ApiPropertyDefaultTime                   => en_US.ApiPropertyDefaultTime,
            TimePickerShowCaseLangResourceKind.ApiPropertyPickerDisplayTime             => en_US.ApiPropertyPickerDisplayTime,
            TimePickerShowCaseLangResourceKind.ApiPropertyIsNeedConfirm                 => en_US.ApiPropertyIsNeedConfirm,
            TimePickerShowCaseLangResourceKind.ApiPropertyIsShowNow                     => en_US.ApiPropertyIsShowNow,
            TimePickerShowCaseLangResourceKind.ApiPropertyMinuteIncrement               => en_US.ApiPropertyMinuteIncrement,
            TimePickerShowCaseLangResourceKind.ApiPropertySecondIncrement               => en_US.ApiPropertySecondIncrement,
            TimePickerShowCaseLangResourceKind.ApiPropertyClockIdentifier               => en_US.ApiPropertyClockIdentifier,
            TimePickerShowCaseLangResourceKind.ApiPropertyRangeStartSelectedTime        => en_US.ApiPropertyRangeStartSelectedTime,
            TimePickerShowCaseLangResourceKind.ApiPropertyRangeEndSelectedTime          => en_US.ApiPropertyRangeEndSelectedTime,
            TimePickerShowCaseLangResourceKind.ApiPropertyRangeStartDefaultTime         => en_US.ApiPropertyRangeStartDefaultTime,
            TimePickerShowCaseLangResourceKind.ApiPropertyRangeEndDefaultTime           => en_US.ApiPropertyRangeEndDefaultTime,
            TimePickerShowCaseLangResourceKind.TokenNameItemHeight                      => en_US.TokenNameItemHeight,
            TimePickerShowCaseLangResourceKind.TokenNameItemWidth                       => en_US.TokenNameItemWidth,
            TimePickerShowCaseLangResourceKind.TokenNamePeriodHostWidth                 => en_US.TokenNamePeriodHostWidth,
            TimePickerShowCaseLangResourceKind.TokenNameItemPadding                     => en_US.TokenNameItemPadding,
            TimePickerShowCaseLangResourceKind.TokenNameButtonsMargin                   => en_US.TokenNameButtonsMargin,
            TimePickerShowCaseLangResourceKind.TokenNameRangePickerArrowMargin          => en_US.TokenNameRangePickerArrowMargin,
            TimePickerShowCaseLangResourceKind.TokenNameRangePickerIndicatorThickness   => en_US.TokenNameRangePickerIndicatorThickness,
            TimePickerShowCaseLangResourceKind.TokenNameHeaderMargin                    => en_US.TokenNameHeaderMargin,
            TimePickerShowCaseLangResourceKind.TokenScopeComponent                      => en_US.TokenScopeComponent,
            TimePickerShowCaseLangResourceKind.TokenStatusStable                        => en_US.TokenStatusStable,
            TimePickerShowCaseLangResourceKind.BindingTitle                             => en_US.BindingTitle,
            TimePickerShowCaseLangResourceKind.BindingDescription                       => en_US.BindingDescription,
            TimePickerShowCaseLangResourceKind.P2TextSelectedTime                       => en_US.P2TextSelectedTime,
            TimePickerShowCaseLangResourceKind.P2TextSelectedTimeRange                  => en_US.P2TextSelectedTimeRange,
            TimePickerShowCaseLangResourceKind.P2ContentSetNoon                         => en_US.P2ContentSetNoon,
            TimePickerShowCaseLangResourceKind.P2ContentSetWorkHours                    => en_US.P2ContentSetWorkHours,
            TimePickerShowCaseLangResourceKind.P2ContentClear                           => en_US.P2ContentClear,
            _                                                                           => kind.ToString()
        };
    }
}

public sealed record TimePickerApiRow(
    string Property,
    string Description,
    string Type,
    string TypeTagColor,
    string Default);

public sealed record TimePickerDesignTokenRow(
    string Token,
    string Description,
    string Scope,
    string ScopeTagColor,
    string Status,
    string StatusTagColor);
