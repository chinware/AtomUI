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

}
