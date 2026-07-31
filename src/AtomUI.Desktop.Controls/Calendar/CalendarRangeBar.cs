using AtomUI.Controls;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

[GenerateScopedResourceHost]
public partial class CalendarRangeBar : AvaloniaObject
{
    #region 公共属性定义

    public static readonly DirectProperty<CalendarRangeBar, DateTime?> StartDateProperty =
        AvaloniaProperty.RegisterDirect<CalendarRangeBar, DateTime?>(
            nameof(StartDate),
            o => o.StartDate,
            (o, v) => o.StartDate = v);

    public static readonly DirectProperty<CalendarRangeBar, DateTime?> EndDateProperty =
        AvaloniaProperty.RegisterDirect<CalendarRangeBar, DateTime?>(
            nameof(EndDate),
            o => o.EndDate,
            (o, v) => o.EndDate = v);

    public static readonly DirectProperty<CalendarRangeBar, object?> LabelProperty =
        AvaloniaProperty.RegisterDirect<CalendarRangeBar, object?>(
            nameof(Label),
            o => o.Label,
            (o, v) => o.Label = v);

    public static readonly DirectProperty<CalendarRangeBar, IBrush?> BackgroundProperty =
        AvaloniaProperty.RegisterDirect<CalendarRangeBar, IBrush?>(
            nameof(Background),
            o => o.Background,
            (o, v) => o.Background = v);

    public static readonly DirectProperty<CalendarRangeBar, double> HeightProperty =
        AvaloniaProperty.RegisterDirect<CalendarRangeBar, double>(
            nameof(Height),
            o => o.Height,
            (o, v) => o.Height = v);

    private DateTime? _startDate;

    public DateTime? StartDate
    {
        get => _startDate;
        set => SetAndRaise(StartDateProperty, ref _startDate, value);
    }

    private DateTime? _endDate;

    public DateTime? EndDate
    {
        get => _endDate;
        set => SetAndRaise(EndDateProperty, ref _endDate, value);
    }

    private object? _label;

    public object? Label
    {
        get => _label;
        set => SetAndRaise(LabelProperty, ref _label, value);
    }

    private IBrush? _background;

    public IBrush? Background
    {
        get => _background;
        set => SetAndRaise(BackgroundProperty, ref _background, value);
    }

    private double _height = double.NaN;

    public double Height
    {
        get => _height;
        set => SetAndRaise(HeightProperty, ref _height, value);
    }

    #endregion
}

public class CalendarRangeBarCollection : AvaloniaList<CalendarRangeBar>
{
}
