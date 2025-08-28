using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Utilities;
using Thumb = AtomUI.Controls.Primitives.Thumb;

namespace AtomUI.Controls;

[PseudoClasses(StdPseudoClass.Vertical, StdPseudoClass.Horizontal)]
internal abstract class AbstractColorPickerSliderTrack : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<double> MinimumProperty =
        RangeBase.MinimumProperty.AddOwner<AbstractColorPickerSliderTrack>();
    
    public static readonly StyledProperty<double> MaximumProperty =
        RangeBase.MaximumProperty.AddOwner<AbstractColorPickerSliderTrack>();
    
    public static readonly StyledProperty<double> ValueProperty =
        RangeBase.ValueProperty.AddOwner<AbstractColorPickerSliderTrack>();
    
    public static readonly StyledProperty<bool> IgnoreThumbDragProperty =
        AvaloniaProperty.Register<AbstractColorPickerSliderTrack, bool>(nameof(IgnoreThumbDrag));

    public static readonly StyledProperty<bool> DeferThumbDragProperty =
        AvaloniaProperty.Register<AbstractColorPickerSliderTrack, bool>(nameof(DeferThumbDrag));
    
    public double Minimum
    {
        get => GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }
    
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public bool IgnoreThumbDrag
    {
        get => GetValue(IgnoreThumbDragProperty);
        set => SetValue(IgnoreThumbDragProperty, value);
    }

    public bool DeferThumbDrag
    {
        get => GetValue(DeferThumbDragProperty);
        set => SetValue(DeferThumbDragProperty, value);
    }
    
    #endregion
    
    protected double ThumbCenterOffset { get; set; }
    protected double Density { get; set; }
    
    protected double ThumbValue => Value + (DeferredThumbDrag == null ? 0 : ValueFromDistance(DeferredThumbDrag.Vector.X, DeferredThumbDrag.Vector.Y));
    protected VectorEventArgs? DeferredThumbDrag;
    protected Vector LastDrag;
    
    static AbstractColorPickerSliderTrack()
    {
        AffectsArrange<AbstractColorPickerSliderTrack>(MinimumProperty, MaximumProperty, ValueProperty);
    }
    
    public virtual double ValueFromPoint(Point point)
    {
        double val = ThumbValue + ValueFromDistance(point.X - ThumbCenterOffset, point.Y - (Bounds.Height * 0.5));
        return Math.Max(Minimum, Math.Min(Maximum, val));
    }
    
    public virtual double ValueFromDistance(double horizontal, double vertical)
    {
        return horizontal * Density;
    }
    
    protected static void CoerceLength(ref double componentLength, double trackLength)
    {
        if (componentLength < 0)
        {
            componentLength = 0.0;
        }
        else if (componentLength > trackLength || double.IsNaN(componentLength))
        {
            componentLength = trackLength;
        }
    }
    
    protected Vector CalculateThumbAdjustment(Thumb thumb, Rect newThumbBounds)
    {
        var thumbDelta = newThumbBounds.Position - thumb.Bounds.Position;
        return LastDrag - thumbDelta;
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        
        if (change.Property == DeferThumbDragProperty)
        {
            if (!change.GetNewValue<bool>())
            {
                ApplyDeferredThumbDrag();
            }
        }
    }
    
    protected void ThumbDragCompleted(object? sender, EventArgs e) => ApplyDeferredThumbDrag();
    
    protected void ApplyDeferredThumbDrag()
    {
        if (DeferredThumbDrag != null)
        {
            ApplyThumbDrag(DeferredThumbDrag);
            DeferredThumbDrag = null;
        }
    }
    
    protected void ApplyThumbDrag(VectorEventArgs e)
    {
        var delta    = ValueFromDistance(e.Vector.X, e.Vector.Y);
        var factor   = e.Vector / delta;
        var oldValue = Value;

        SetCurrentValue(ValueProperty, MathUtilities.Clamp(
            Value + delta,
            Minimum,
            Maximum));

        // Record the part of the drag that actually had effect as the last drag delta.
        // Due to clamping, we need to compare the two values instead of using the drag delta.
        LastDrag = (Value - oldValue) * factor;
    }
    
    protected void ComputeSliderLengths(Size arrangeSize, out double decreaseButtonLength, out double increaseButtonLength)
    {
        double min    = Minimum;
        double range  = Math.Max(0.0, Maximum - min);
        double offset = Math.Min(range, ThumbValue - min);

        // Compute thumb size
        double trackLength = arrangeSize.Width;
        double remainingTrackLength = trackLength;

        decreaseButtonLength = remainingTrackLength * offset / range;
        CoerceLength(ref decreaseButtonLength, remainingTrackLength);

        increaseButtonLength = remainingTrackLength - decreaseButtonLength;
        CoerceLength(ref increaseButtonLength, remainingTrackLength);

        Density = range / remainingTrackLength;
    }
    
    protected void ThumbDragged(object? sender, VectorEventArgs e)
    {
        if (IgnoreThumbDrag)
        {
            return;
        }

        if (DeferThumbDrag)
        {
            DeferredThumbDrag = e;
            InvalidateArrange();
        }
        else
        {
            ApplyThumbDrag(e);
        }
    }
}