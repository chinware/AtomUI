using AtomUI.Controls;
using Avalonia;

namespace AtomUI.Desktop.Controls;

internal class DualMonthArrowDecoratedBox : ArrowDecoratedBox
{
    public static readonly DirectProperty<DualMonthArrowDecoratedBox, double> RangePickerIndicatorOffsetStartProperty =
        AvaloniaProperty.RegisterDirect<DualMonthArrowDecoratedBox, double>(nameof(RangePickerIndicatorOffsetStart),
            o => o.RangePickerIndicatorOffsetStart,
            (o, v) => o.RangePickerIndicatorOffsetStart = v);
    
    public static readonly DirectProperty<DualMonthArrowDecoratedBox, double> RangePickerIndicatorOffsetEndProperty =
        AvaloniaProperty.RegisterDirect<DualMonthArrowDecoratedBox, double>(nameof(RangePickerIndicatorOffsetEnd),
            o => o.RangePickerIndicatorOffsetEnd,
            (o, v) => o.RangePickerIndicatorOffsetEnd = v);
    
    public static readonly DirectProperty<DualMonthArrowDecoratedBox, bool> IsHorizontalFlippedProperty =
        AvaloniaProperty.RegisterDirect<DualMonthArrowDecoratedBox, bool>(nameof(IsHorizontalFlipped),
            o => o.IsHorizontalFlipped,
            (o, v) => o.IsHorizontalFlipped = v);
    
    private double _rangePickerIndicatorStart;

    internal double RangePickerIndicatorOffsetStart
    {
        get => _rangePickerIndicatorStart;
        set => SetAndRaise(RangePickerIndicatorOffsetStartProperty, ref _rangePickerIndicatorStart, value);
    }
    
    private double _rangePickerIndicatorEnd;

    internal double RangePickerIndicatorOffsetEnd
    {
        get => _rangePickerIndicatorEnd;
        set => SetAndRaise(RangePickerIndicatorOffsetEndProperty, ref _rangePickerIndicatorEnd, value);
    }
    
    private bool _isHorizontalFlipped;

    public bool IsHorizontalFlipped
    {
        get => _isHorizontalFlipped;
        private set => SetAndRaise(IsHorizontalFlippedProperty, ref _isHorizontalFlipped, value);
    }
    
    static DualMonthArrowDecoratedBox()
    {
        AffectsArrange<DualMonthArrowDecoratedBox>(RangePickerIndicatorOffsetStartProperty, RangePickerIndicatorOffsetEndProperty, IsHorizontalFlippedProperty);
    }

    // protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    // {
    //     base.OnPropertyChanged(change);
    //     if (change.Property == RangePickerIndicatorBoundsProperty)
    //     {
    //         Console.WriteLine($"RangePickerIndicatorBounds changed: {RangePickerIndicatorBounds}");
    //     }
    // }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);
        if (IsArrowVisible)
        {
            ArrangeArrow(finalSize);
        }
        return size;
    }
    
    private void ArrangeArrow(Size finalSize)
    {
        if (ArrowIndicatorLayout is null)
        {
            return;
        }

        var offsetX  = 0d;
        var offsetY  = 0d;
        var position = ArrowPosition;
        var size     = ArrowIndicatorLayout.DesiredSize;
        
        var minValue = Math.Max(size.Width, size.Height);

        if (position == ArrowPosition.Top ||
            position == ArrowPosition.TopEdgeAlignedLeft ||
            position == ArrowPosition.TopEdgeAlignedRight)
        {
            offsetY = 0.5d;
        }
        else
        {
            offsetY = -0.5d;
        }

        if (!IsHorizontalFlipped)
        {
            offsetX = Math.Max(RangePickerIndicatorOffsetStart, minValue);
        }
        else
        {
            offsetX = DesiredSize.Width - Math.Max(RangePickerIndicatorOffsetEnd, minValue);  
        }
        
        var adjustedOffset = AdjustArrowOffset(new Point(offsetX, offsetY), finalSize, size);
        ArrowIndicatorLayout.Arrange(new Rect(adjustedOffset, size));
        if (ArrowIndicator != null)
        {
            ArrowIndicatorBounds = ArrowIndicator.Bounds;
        }

        ArrowIndicatorLayoutBounds = ArrowIndicatorLayout.Bounds;
        ArrowPlacementFlipped      = false;
    }
}