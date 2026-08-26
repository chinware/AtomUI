using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace AtomUI.Desktop.Controls;

/// <summary>
/// Internal layout engine for <see cref="Masonry"/>. Derives from <see cref="Panel"/> and is
/// assembled as the <c>ItemsPanel</c> by the Masonry control theme. Not exposed to developers.
/// </summary>
/// <remarks>
/// The layout properties below mirror those on <see cref="Masonry"/> and are populated via
/// RelativeSource binding in the control theme. This keeps the engine decoupled from the
/// owner control and lets it be assembled purely in XAML.
/// </remarks>
internal class MasonryPanel : Panel
{
    #region 公共属性定义

    public static readonly StyledProperty<int> ColumnCountProperty =
        Masonry.ColumnCountProperty.AddOwner<MasonryPanel>();

    public static readonly StyledProperty<ResponsiveInt?> ColumnInfoProperty =
        Masonry.ColumnInfoProperty.AddOwner<MasonryPanel>();

    public static readonly StyledProperty<double> MinColumnWidthProperty =
        Masonry.MinColumnWidthProperty.AddOwner<MasonryPanel>();

    public static readonly StyledProperty<int> MaxColumnCountProperty =
        Masonry.MaxColumnCountProperty.AddOwner<MasonryPanel>();

    public static readonly StyledProperty<double> ColumnGapProperty =
        Masonry.ColumnGapProperty.AddOwner<MasonryPanel>();

    public static readonly StyledProperty<double> RowGapProperty =
        Masonry.RowGapProperty.AddOwner<MasonryPanel>();

    public static readonly StyledProperty<ResponsiveGutter?> GutterProperty =
        Masonry.GutterProperty.AddOwner<MasonryPanel>();

    public int ColumnCount
    {
        get => GetValue(ColumnCountProperty);
        set => SetValue(ColumnCountProperty, value);
    }

    public ResponsiveInt? ColumnInfo
    {
        get => GetValue(ColumnInfoProperty);
        set => SetValue(ColumnInfoProperty, value);
    }

    public double MinColumnWidth
    {
        get => GetValue(MinColumnWidthProperty);
        set => SetValue(MinColumnWidthProperty, value);
    }

    public int MaxColumnCount
    {
        get => GetValue(MaxColumnCountProperty);
        set => SetValue(MaxColumnCountProperty, value);
    }

    public double ColumnGap
    {
        get => GetValue(ColumnGapProperty);
        set => SetValue(ColumnGapProperty, value);
    }

    public double RowGap
    {
        get => GetValue(RowGapProperty);
        set => SetValue(RowGapProperty, value);
    }

    public ResponsiveGutter? Gutter
    {
        get => GetValue(GutterProperty);
        set => SetValue(GutterProperty, value);
    }

    #endregion

    private List<Rect> _arrangeRects = new();
    private List<int> _arrangeColumns = new();
    private List<bool> _arrangeFullSpans = new();
    private MasonryLayout? _measuredLayout;
    private double _measuredEffectiveWidth;
    private bool _hasMeasuredLayout;
    private int[]? _lastColumns;
    private bool[]? _lastFullSpans;
    private bool _hasPublishedLayout;
    private MediaBreakPoint? _breakPoint;
    private IMediaBreakAwareControl? _mediaOwner;

    static MasonryPanel()
    {
        AffectsMeasure<MasonryPanel>(
            ColumnCountProperty,
            ColumnInfoProperty,
            MinColumnWidthProperty,
            MaxColumnCountProperty,
            ColumnGapProperty,
            RowGapProperty,
            GutterProperty);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        if (MediaQueryHost.FindOwner(this) is { } mediaOwner)
        {
            _mediaOwner = mediaOwner;
            _breakPoint = mediaOwner.MediaBreakPoint;
            mediaOwner.MediaBreakPointChanged += HandleMediaBreakChanged;
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _hasMeasuredLayout = false;
        _measuredLayout = null;
        if (_mediaOwner != null)
        {
            _mediaOwner.MediaBreakPointChanged -= HandleMediaBreakChanged;
            _mediaOwner = null;
        }
    }

    private void HandleMediaBreakChanged(object? sender, MediaBreakPointChangedEventArgs args)
    {
        _breakPoint = args.MediaBreakPoint;
        _hasMeasuredLayout = false;
        _measuredLayout = null;
        InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var layout = CalculateLayout(availableSize.Width, measureChildren: true);
        _measuredLayout = layout;
        _measuredEffectiveWidth = layout.Width;
        _hasMeasuredLayout = true;
        PublishLayout(layout);
        return new Size(layout.Width, layout.Height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var effectiveWidth = ResolveEffectiveWidth(finalSize.Width);
        var layout = _hasMeasuredLayout && AreClose(_measuredEffectiveWidth, effectiveWidth)
            ? _measuredLayout!.Value
            : CalculateLayout(finalSize.Width, measureChildren: false);
        _hasMeasuredLayout = false;
        _measuredLayout = null;
        PublishLayout(layout);

        for (var i = 0; i < Children.Count; i++)
        {
            Children[i].Arrange(_arrangeRects[i]);
        }

        MaybeNotifyLayoutChanged();
        return finalSize;
    }

    private void PublishLayout(MasonryLayout layout)
    {
        _arrangeRects     = layout.Rects;
        _arrangeColumns   = layout.Columns;
        _arrangeFullSpans = layout.FullSpans;
    }

    private double ResolveEffectiveWidth(double availableWidth)
    {
        var breakPoint = GetBreakPoint();
        var (columnGap, _) = ResolveGaps(breakPoint);
        return ResolveAvailableWidth(availableWidth, columnGap);
    }

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) < 0.01;
    }

    private MasonryLayout CalculateLayout(double availableWidth, bool measureChildren)
    {
        var breakPoint  = GetBreakPoint();
        var (columnGap, rowGap) = ResolveGaps(breakPoint);
        var width       = ResolveAvailableWidth(availableWidth, columnGap);
        var columnCount = CalculateColumnCount(width, columnGap, breakPoint);
        var columnWidth = columnCount == 1
            ? width
            : Math.Max(0, (width - columnGap * (columnCount - 1)) / columnCount);

        var columnHeights = new double[columnCount];
        var rects         = new List<Rect>(Children.Count);
        var columns       = new List<int>(Children.Count);
        var fullSpans     = new List<bool>(Children.Count);

        for (var i = 0; i < Children.Count; i++)
        {
            var child = Children[i];
            if (!child.IsVisible)
            {
                rects.Add(default);
                columns.Add(-1);
                fullSpans.Add(false);
                continue;
            }

            var isFullSpan = child.GetValue(Masonry.SpanProperty) == MasonryItemSpan.Full;
            var targetWidth = isFullSpan ? width : columnWidth;
            if (measureChildren)
            {
                child.Measure(new Size(targetWidth, double.PositiveInfinity));
            }

            var childHeight = child.DesiredSize.Height;
            if (isFullSpan)
            {
                var top = Max(columnHeights);
                var y   = top > 0 ? top + rowGap : 0;
                rects.Add(new Rect(0, y, width, childHeight));
                columns.Add(0);
                fullSpans.Add(true);
                Fill(columnHeights, y + childHeight);
            }
            else
            {
                var explicitColumn = child.GetValue(Masonry.ColumnProperty);
                var columnIndex = explicitColumn.HasValue
                    ? Math.Clamp(explicitColumn.Value, 0, columnCount - 1)
                    : IndexOfShortestColumn(columnHeights);
                var x = columnIndex * (columnWidth + columnGap);
                var y = columnHeights[columnIndex] > 0 ? columnHeights[columnIndex] + rowGap : 0;
                rects.Add(new Rect(x, y, columnWidth, childHeight));
                columns.Add(columnIndex);
                fullSpans.Add(false);
                columnHeights[columnIndex] = y + childHeight;
            }
        }

        return new MasonryLayout(width, Max(columnHeights), rects, columns, fullSpans);
    }

    private double ResolveAvailableWidth(double availableWidth, double columnGap)
    {
        if (!double.IsInfinity(availableWidth))
        {
            return Math.Max(0, availableWidth);
        }

        var maxColumns   = Math.Max(1, MaxColumnCount);
        var minColumnWidth = Math.Max(1, NormalizeFinite(MinColumnWidth, 1d));
        return minColumnWidth * maxColumns + columnGap * (maxColumns - 1);
    }

    private int CalculateColumnCount(double width, double columnGap, MediaBreakPoint breakPoint)
    {
        if (ColumnInfo?.TryResolve(breakPoint, out var responsiveColumnCount) == true && responsiveColumnCount > 0)
        {
            return responsiveColumnCount;
        }

        if (ColumnCount > 0)
        {
            return ColumnCount;
        }

        var maxColumns     = Math.Max(1, MaxColumnCount);
        var minColumnWidth = Math.Max(1, NormalizeFinite(MinColumnWidth, 1d));
        var columnCount    = (int)Math.Floor((width + columnGap) / (minColumnWidth + columnGap));
        return Math.Clamp(columnCount, 1, maxColumns);
    }

    private (double ColumnGap, double RowGap) ResolveGaps(MediaBreakPoint breakPoint)
    {
        var fallback = (NormalizeGap(ColumnGap), NormalizeGap(RowGap));
        if (Gutter.HasValue && Gutter.Value.TryResolve(breakPoint, fallback, out var gutter))
        {
            return (NormalizeGap(gutter.Horizontal), NormalizeGap(gutter.Vertical));
        }

        return fallback;
    }

    private MediaBreakPoint GetBreakPoint()
    {
        if (_breakPoint.HasValue)
        {
            return _breakPoint.Value;
        }

        if (MediaQueryHost.FindOwner(this) is { } mediaOwner)
        {
            _breakPoint = mediaOwner.MediaBreakPoint;
            return _breakPoint.Value;
        }

        return MediaBreakPoint.Large;
    }

    private static double NormalizeGap(double value)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value < 0)
        {
            return 0;
        }
        return value;
    }

    private static double NormalizeFinite(double value, double fallback)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
        {
            return fallback;
        }
        return value;
    }

    private static int IndexOfShortestColumn(double[] columnHeights)
    {
        var columnIndex = 0;
        var minHeight   = columnHeights[0];
        for (var i = 1; i < columnHeights.Length; i++)
        {
            if (columnHeights[i] < minHeight)
            {
                columnIndex = i;
                minHeight   = columnHeights[i];
            }
        }
        return columnIndex;
    }

    private static double Max(double[] values)
    {
        var max = 0d;
        foreach (var value in values)
        {
            max = Math.Max(max, value);
        }
        return max;
    }

    private static void Fill(double[] values, double value)
    {
        for (var i = 0; i < values.Length; i++)
        {
            values[i] = value;
        }
    }

    private void MaybeNotifyLayoutChanged()
    {
        if (_arrangeColumns.Count == 0 && !_hasPublishedLayout)
        {
            return;
        }

        var changed = !_hasPublishedLayout ||
                      _lastColumns is null ||
                      _lastColumns.Length != _arrangeColumns.Count ||
                      _lastFullSpans is null ||
                      _lastFullSpans.Length != _arrangeFullSpans.Count;
        if (!changed)
        {
            for (var i = 0; i < _arrangeColumns.Count; i++)
            {
                if (_arrangeColumns[i] != _lastColumns![i] ||
                    _arrangeFullSpans[i] != _lastFullSpans![i])
                {
                    changed = true;
                    break;
                }
            }
        }

        if (!changed)
        {
            return;
        }

        _lastColumns  = _arrangeColumns.ToArray();
        _lastFullSpans = _arrangeFullSpans.ToArray();
        _hasPublishedLayout = true;

        if (this.FindAncestorOfType<Masonry>() is Masonry owner)
        {
            var items = new List<MasonryItemLayout>(_arrangeColumns.Count);
            for (var i = 0; i < _arrangeColumns.Count && i < Children.Count; i++)
            {
                var col       = _arrangeColumns[i];
                var isFull    = _arrangeFullSpans[i];
                var effective = isFull ? 0 : col;
                items.Add(new MasonryItemLayout(Children[i], i, effective, isFull));
            }
            owner.NotifyLayoutChanged(items);
        }
    }

    private readonly record struct MasonryLayout(
        double Width,
        double Height,
        List<Rect> Rects,
        List<int> Columns,
        List<bool> FullSpans);
}
