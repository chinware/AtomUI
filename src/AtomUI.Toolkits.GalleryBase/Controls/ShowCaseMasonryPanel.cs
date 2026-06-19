using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Toolkits.GalleryBase.Controls;

public class ShowCaseMasonryPanel : Panel
{
    public static readonly StyledProperty<double> MinItemWidthProperty =
        AvaloniaProperty.Register<ShowCaseMasonryPanel, double>(nameof(MinItemWidth), 320);

    public static readonly StyledProperty<int> MaxColumnsProperty =
        AvaloniaProperty.Register<ShowCaseMasonryPanel, int>(nameof(MaxColumns), 2);

    public static readonly StyledProperty<double> ColumnGapProperty =
        AvaloniaProperty.Register<ShowCaseMasonryPanel, double>(nameof(ColumnGap), 16);

    public static readonly StyledProperty<double> RowGapProperty =
        AvaloniaProperty.Register<ShowCaseMasonryPanel, double>(nameof(RowGap), 16);

    private List<Rect> _arrangeRects = new();

    public double MinItemWidth
    {
        get => GetValue(MinItemWidthProperty);
        set => SetValue(MinItemWidthProperty, value);
    }

    public int MaxColumns
    {
        get => GetValue(MaxColumnsProperty);
        set => SetValue(MaxColumnsProperty, value);
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

    static ShowCaseMasonryPanel()
    {
        AffectsMeasure<ShowCaseMasonryPanel>(
            MinItemWidthProperty,
            MaxColumnsProperty,
            ColumnGapProperty,
            RowGapProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var layout = CalculateLayout(availableSize.Width, true);
        _arrangeRects = layout.Rects;
        return new Size(layout.Width, layout.Height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var layout = CalculateLayout(finalSize.Width, false);
        _arrangeRects = layout.Rects;

        for (var i = 0; i < Children.Count; i++)
        {
            Children[i].Arrange(_arrangeRects[i]);
        }

        return finalSize;
    }

    private MasonryLayout CalculateLayout(double availableWidth, bool measureChildren)
    {
        var width       = ResolveAvailableWidth(availableWidth);
        var columnCount = CalculateColumnCount(width);
        var columnGap   = Math.Max(0, ColumnGap);
        var rowGap      = Math.Max(0, RowGap);
        var columnWidth = columnCount == 1
            ? width
            : Math.Max(0, (width - columnGap * (columnCount - 1)) / columnCount);
        var columnHeights = new double[columnCount];
        var rects         = new List<Rect>(Children.Count);

        foreach (var child in Children)
        {
            if (!child.IsVisible)
            {
                rects.Add(default);
                continue;
            }

            var isFullSpan  = IsFullSpan(child);
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
                Fill(columnHeights, y + childHeight);
            }
            else
            {
                var columnIndex = IndexOfShortestColumn(columnHeights);
                var x           = columnIndex * (columnWidth + columnGap);
                var y           = columnHeights[columnIndex] > 0 ? columnHeights[columnIndex] + rowGap : 0;
                rects.Add(new Rect(x, y, columnWidth, childHeight));
                columnHeights[columnIndex] = y + childHeight;
            }
        }

        return new MasonryLayout(width, Max(columnHeights), rects);
    }

    private double ResolveAvailableWidth(double availableWidth)
    {
        if (!double.IsInfinity(availableWidth))
        {
            return Math.Max(0, availableWidth);
        }

        var maxColumns   = Math.Max(1, MaxColumns);
        var minItemWidth = Math.Max(1, MinItemWidth);
        return minItemWidth * maxColumns + Math.Max(0, ColumnGap) * (maxColumns - 1);
    }

    private int CalculateColumnCount(double width)
    {
        var maxColumns   = Math.Max(1, MaxColumns);
        var minItemWidth = Math.Max(1, MinItemWidth);
        var columnGap    = Math.Max(0, ColumnGap);
        var columnCount  = (int)Math.Floor((width + columnGap) / (minItemWidth + columnGap));
        return Math.Clamp(columnCount, 1, maxColumns);
    }

    private static bool IsFullSpan(Control child)
    {
        return child is ShowCaseItem { Span: ShowCaseItemSpan.Full } or ShowCaseItem { IsOccupyEntireRow: true };
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

    private readonly record struct MasonryLayout(double Width, double Height, List<Rect> Rects);
}
