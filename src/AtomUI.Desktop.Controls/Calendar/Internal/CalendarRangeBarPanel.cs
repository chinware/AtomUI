using System.Globalization;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls.Internal.Calendar;

internal sealed class CalendarRangeBarPanel : Panel
{
    #region 公共属性定义

    public static readonly StyledProperty<DateTime> ValueProperty =
        AvaloniaProperty.Register<CalendarRangeBarPanel, DateTime>(nameof(Value));

    public static readonly StyledProperty<CalendarMode> ModeProperty =
        AvaloniaProperty.Register<CalendarRangeBarPanel, CalendarMode>(nameof(Mode));

    public static readonly StyledProperty<bool> FullscreenProperty =
        AvaloniaProperty.Register<CalendarRangeBarPanel, bool>(nameof(Fullscreen), true);

    public static readonly StyledProperty<bool> ShowWeekProperty =
        AvaloniaProperty.Register<CalendarRangeBarPanel, bool>(nameof(ShowWeek));

    public static readonly StyledProperty<CalendarRangeBarCollection?> RangeBarsProperty =
        AvaloniaProperty.Register<CalendarRangeBarPanel, CalendarRangeBarCollection?>(nameof(RangeBars));

    public static readonly StyledProperty<CultureInfo?> CultureProperty =
        AvaloniaProperty.Register<CalendarRangeBarPanel, CultureInfo?>(nameof(Culture));

    public static readonly StyledProperty<double> RangeBarHeightProperty =
        AvaloniaProperty.Register<CalendarRangeBarPanel, double>(nameof(RangeBarHeight), double.NaN);

    public static readonly StyledProperty<double> WeekHeaderHeightProperty =
        AvaloniaProperty.Register<CalendarRangeBarPanel, double>(nameof(WeekHeaderHeight), 32);

    public static readonly StyledProperty<double> RangeBarTopOffsetProperty =
        AvaloniaProperty.Register<CalendarRangeBarPanel, double>(nameof(RangeBarTopOffset), 32);

    public static readonly StyledProperty<double> RangeBarHorizontalInsetProperty =
        AvaloniaProperty.Register<CalendarRangeBarPanel, double>(nameof(RangeBarHorizontalInset), 4);

    public DateTime Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public CalendarMode Mode
    {
        get => GetValue(ModeProperty);
        set => SetValue(ModeProperty, value);
    }

    public bool Fullscreen
    {
        get => GetValue(FullscreenProperty);
        set => SetValue(FullscreenProperty, value);
    }

    public bool ShowWeek
    {
        get => GetValue(ShowWeekProperty);
        set => SetValue(ShowWeekProperty, value);
    }

    public CalendarRangeBarCollection? RangeBars
    {
        get => GetValue(RangeBarsProperty);
        set => SetValue(RangeBarsProperty, value);
    }

    public CultureInfo? Culture
    {
        get => GetValue(CultureProperty);
        set => SetValue(CultureProperty, value);
    }

    public double RangeBarHeight
    {
        get => GetValue(RangeBarHeightProperty);
        set => SetValue(RangeBarHeightProperty, value);
    }

    public double WeekHeaderHeight
    {
        get => GetValue(WeekHeaderHeightProperty);
        set => SetValue(WeekHeaderHeightProperty, value);
    }

    public double RangeBarTopOffset
    {
        get => GetValue(RangeBarTopOffsetProperty);
        set => SetValue(RangeBarTopOffsetProperty, value);
    }

    public double RangeBarHorizontalInset
    {
        get => GetValue(RangeBarHorizontalInsetProperty);
        set => SetValue(RangeBarHorizontalInsetProperty, value);
    }

    #endregion

    private const double DefaultRangeBarHeight = 20;
    private const double DefaultRangeBarGap = 2;
    private const double DefaultLabelFontSize = 12;
    private const double DefaultLabelHorizontalPadding = 8;

    private static readonly HashSet<AvaloniaProperty> LayoutTriggers = new()
    {
        ValueProperty,
        ModeProperty,
        FullscreenProperty,
        ShowWeekProperty,
        RangeBarsProperty,
        CultureProperty,
        RangeBarHeightProperty,
        WeekHeaderHeightProperty,
        RangeBarTopOffsetProperty,
        RangeBarHorizontalInsetProperty,
        FlowDirectionProperty
    };

    private readonly List<CalendarRangeBarSegment> _segments = new();
    private Size _realizedSize;
    private bool _segmentsDirty = true;

    public CalendarRangeBarPanel()
    {
        IsHitTestVisible = false;
        ClipToBounds = true;
    }

    internal void InvalidateRangeBars()
    {
        _segmentsDirty = true;
        InvalidateMeasure();
        InvalidateArrange();
        InvalidateVisual();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (LayoutTriggers.Contains(change.Property))
        {
            InvalidateRangeBars();
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (HasUsableSize(availableSize))
        {
            EnsureSegments(availableSize);
        }

        MeasureChildren();
        return default;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        EnsureSegments(finalSize);
        MeasureChildren();
        for (var i = 0; i < Children.Count && i < _segments.Count; i++)
        {
            Children[i].Arrange(_segments[i].Bounds);
        }

        return finalSize;
    }

    private static bool HasUsableSize(Size size)
    {
        return double.IsFinite(size.Width) &&
               double.IsFinite(size.Height) &&
               size.Width > 0 &&
               size.Height > 0;
    }

    private void EnsureSegments(Size availableSize)
    {
        if (!_segmentsDirty && _realizedSize == availableSize)
        {
            return;
        }

        _realizedSize = availableSize;
        _segmentsDirty = false;
        BuildSegments(availableSize);
        SyncChildren();
    }

    private void BuildSegments(Size size)
    {
        _segments.Clear();
        if (!CanRender(size))
        {
            return;
        }

        var culture = Culture ?? CultureInfo.CurrentCulture;
        var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
        var gridStart = CalendarViewCellBuilder.GetDateGridStart(Value.Date, firstDayOfWeek);
        var gridEnd = gridStart.AddDays(CalendarViewCellBuilder.DateGridCellCount - 1);
        var totalColumns = ShowWeek
            ? CalendarViewCellBuilder.DateGridColumns + 1
            : CalendarViewCellBuilder.DateGridColumns;
        var dateColumnOffset = ShowWeek ? 1 : 0;
        var weekHeaderHeight = ClampMetric(WeekHeaderHeight, 0, size.Height);
        var dateAreaHeight = Math.Max(0, size.Height - weekHeaderHeight);
        var cellWidth = size.Width / totalColumns;
        var cellHeight = dateAreaHeight / CalendarViewCellBuilder.DateGridRows;
        var horizontalInset = ClampMetric(RangeBarHorizontalInset, 0, cellWidth / 2);
        var laneEnds = new List<DateTime>();

        foreach (var rangeBar in RangeBars!)
        {
            if (!TryNormalizeRange(rangeBar, gridStart, gridEnd, out var rangeStart, out var rangeEnd, out var barHeight))
            {
                continue;
            }

            var lane = AllocateLane(laneEnds, rangeStart, rangeEnd);
            AddRowSegments(
                rangeBar,
                rangeStart,
                rangeEnd,
                lane,
                gridStart,
                gridEnd,
                weekHeaderHeight,
                cellWidth,
                cellHeight,
                dateColumnOffset,
                horizontalInset,
                barHeight,
                size.Width);
        }
    }

    private bool CanRender(Size size)
    {
        return Mode == CalendarMode.Month &&
               Fullscreen &&
               RangeBars is { Count: > 0 } &&
               size.Width > 0 &&
               size.Height > 0;
    }

    private bool TryNormalizeRange(
        CalendarRangeBar rangeBar,
        DateTime gridStart,
        DateTime gridEnd,
        out DateTime rangeStart,
        out DateTime rangeEnd,
        out double barHeight)
    {
        rangeStart = default;
        rangeEnd = default;
        barHeight = default;

        if (rangeBar.StartDate is null || rangeBar.EndDate is null)
        {
            return false;
        }

        rangeStart = rangeBar.StartDate.Value.Date;
        rangeEnd = rangeBar.EndDate.Value.Date;
        if (rangeEnd < rangeStart || rangeStart > gridEnd || rangeEnd < gridStart)
        {
            return false;
        }

        barHeight = GetEffectiveRangeBarHeight(rangeBar);
        return barHeight > 0;
    }

    private double GetEffectiveRangeBarHeight(CalendarRangeBar rangeBar)
    {
        if (double.IsFinite(rangeBar.Height) && rangeBar.Height > 0)
        {
            return rangeBar.Height;
        }

        if (double.IsFinite(RangeBarHeight) && RangeBarHeight > 0)
        {
            return RangeBarHeight;
        }

        return DefaultRangeBarHeight;
    }

    private static int AllocateLane(IList<DateTime> laneEnds, DateTime rangeStart, DateTime rangeEnd)
    {
        for (var lane = 0; lane < laneEnds.Count; lane++)
        {
            if (rangeStart > laneEnds[lane])
            {
                laneEnds[lane] = rangeEnd;
                return lane;
            }
        }

        laneEnds.Add(rangeEnd);
        return laneEnds.Count - 1;
    }

    private void AddRowSegments(
        CalendarRangeBar rangeBar,
        DateTime rangeStart,
        DateTime rangeEnd,
        int lane,
        DateTime gridStart,
        DateTime gridEnd,
        double weekHeaderHeight,
        double cellWidth,
        double cellHeight,
        int dateColumnOffset,
        double horizontalInset,
        double requestedBarHeight,
        double panelWidth)
    {
        for (var row = 0; row < CalendarViewCellBuilder.DateGridRows; row++)
        {
            var rowStart = gridStart.AddDays(row * CalendarViewCellBuilder.DateGridColumns);
            var rowEnd = rowStart.AddDays(CalendarViewCellBuilder.DateGridColumns - 1);
            if (rangeEnd < rowStart || rangeStart > rowEnd)
            {
                continue;
            }

            var segmentStart = MaxDate(rangeStart, rowStart, gridStart);
            var segmentEnd = MinDate(rangeEnd, rowEnd, gridEnd);
            var startColumn = (segmentStart - rowStart).Days;
            var endColumn = (segmentEnd - rowStart).Days;
            var x = (dateColumnOffset + startColumn) * cellWidth + horizontalInset;
            var width = (endColumn - startColumn + 1) * cellWidth - horizontalInset * 2;
            if (width <= 0)
            {
                continue;
            }

            var rowTop = weekHeaderHeight + row * cellHeight;
            var laneOffset = GetLaneOffset(lane, requestedBarHeight);
            if (laneOffset >= cellHeight)
            {
                continue;
            }

            var height = Math.Min(requestedBarHeight, Math.Max(0, cellHeight - laneOffset));
            if (height <= 0)
            {
                continue;
            }

            var rect = new Rect(x, rowTop + laneOffset, width, height);
            var startsRange = segmentStart == rangeStart;
            var endsRange = segmentEnd == rangeEnd;
            if (FlowDirection == FlowDirection.RightToLeft)
            {
                rect = rect.WithX(panelWidth - rect.X - rect.Width);
            }

            _segments.Add(new CalendarRangeBarSegment(
                rect,
                rangeBar.Background ?? Brushes.Transparent,
                BuildCornerRadius(height / 2, startsRange, endsRange),
                startsRange ? rangeBar.Label : null));
        }
    }

    private double GetLaneOffset(int lane, double barHeight)
    {
        var topOffset = Math.Max(0, RangeBarTopOffset);
        return topOffset + lane * (barHeight + DefaultRangeBarGap);
    }

    private CornerRadius BuildCornerRadius(double radius, bool startsRange, bool endsRange)
    {
        var leftRadius = FlowDirection == FlowDirection.RightToLeft ? endsRange : startsRange;
        var rightRadius = FlowDirection == FlowDirection.RightToLeft ? startsRange : endsRange;
        return new CornerRadius(
            leftRadius ? radius : 0,
            rightRadius ? radius : 0,
            rightRadius ? radius : 0,
            leftRadius ? radius : 0);
    }

    private static DateTime MaxDate(DateTime first, DateTime second, DateTime third)
    {
        var result = first > second ? first : second;
        return result > third ? result : third;
    }

    private static DateTime MinDate(DateTime first, DateTime second, DateTime third)
    {
        var result = first < second ? first : second;
        return result < third ? result : third;
    }

    private static double ClampMetric(double value, double min, double max)
    {
        if (!double.IsFinite(value))
        {
            return min;
        }

        return Math.Clamp(value, min, max);
    }

    private void SyncChildren()
    {
        while (Children.Count > _segments.Count)
        {
            Children.RemoveAt(Children.Count - 1);
        }

        while (Children.Count < _segments.Count)
        {
            Children.Add(CreateRangeBarElement());
        }

        for (var i = 0; i < _segments.Count; i++)
        {
            ApplySegment((Border)Children[i], _segments[i]);
        }
    }

    private void MeasureChildren()
    {
        for (var i = 0; i < Children.Count && i < _segments.Count; i++)
        {
            Children[i].Measure(_segments[i].Bounds.Size);
        }
    }

    private static Border CreateRangeBarElement()
    {
        return new Border
        {
            ClipToBounds = true,
            IsHitTestVisible = false
        };
    }

    private void ApplySegment(Border element, CalendarRangeBarSegment segment)
    {
        element.Background = segment.Background;
        element.CornerRadius = segment.CornerRadius;
        element.Width = segment.Bounds.Width;
        element.Height = segment.Bounds.Height;
        element.Padding = new Thickness(DefaultLabelHorizontalPadding, 0);
        element.Child = CreateLabel(segment.Label);
    }

    private TextBlock? CreateLabel(object? label)
    {
        var text = label?.ToString();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        return new TextBlock
        {
            Text = text,
            TextTrimming = TextTrimming.CharacterEllipsis,
            Foreground = Brushes.White,
            FontSize = DefaultLabelFontSize,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = FlowDirection == FlowDirection.RightToLeft ? TextAlignment.Right : TextAlignment.Left,
            IsHitTestVisible = false
        };
    }

    private readonly record struct CalendarRangeBarSegment(
        Rect Bounds,
        IBrush Background,
        CornerRadius CornerRadius,
        object? Label);
}
