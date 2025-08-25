using Avalonia;
using Avalonia.Metadata;
using Thumb = AtomUI.Controls.Primitives.Thumb;
using AvaloniaButton = Avalonia.Controls.Button;

namespace AtomUI.Controls;

internal class ColorPickerSliderTrack : AbstractColorPickerSliderTrack
{
    #region 公共属性定义

    public static readonly StyledProperty<Thumb?> ThumbProperty =
        AvaloniaProperty.Register<ColorPickerSliderTrack, Thumb?>(nameof(Thumb));

    public static readonly StyledProperty<AvaloniaButton?> IncreaseButtonProperty =
        AvaloniaProperty.Register<ColorPickerSliderTrack, AvaloniaButton?>(nameof(IncreaseButton));

    public static readonly StyledProperty<AvaloniaButton?> DecreaseButtonProperty =
        AvaloniaProperty.Register<ColorPickerSliderTrack, AvaloniaButton?>(nameof(DecreaseButton));
    
    [Content]
    public Thumb? Thumb
    {
        get => GetValue(ThumbProperty);
        set => SetValue(ThumbProperty, value);
    }
    
    public AvaloniaButton? IncreaseButton
    {
        get => GetValue(IncreaseButtonProperty);
        set => SetValue(IncreaseButtonProperty, value);
    }

    public AvaloniaButton? DecreaseButton
    {
        get => GetValue(DecreaseButtonProperty);
        set => SetValue(DecreaseButtonProperty, value);
    }
    
    #endregion

    static ColorPickerSliderTrack()
    {
        ThumbProperty.Changed.AddClassHandler<ColorPickerSliderTrack>((x, e) => x.ThumbChanged(e));
        IncreaseButtonProperty.Changed.AddClassHandler<ColorPickerSliderTrack>((x, e) => x.ButtonChanged(e));
        DecreaseButtonProperty.Changed.AddClassHandler<ColorPickerSliderTrack>((x, e) => x.ButtonChanged(e));
    }
    
    protected override Size MeasureOverride(Size availableSize)
    {
        Size desiredSize = new Size(0.0, 0.0);

        // Only measure thumb.
        // Repeat buttons will be sized based on thumb
        if (Thumb != null)
        {
            Thumb.Measure(availableSize);
            desiredSize = Thumb.DesiredSize;
        }
        
        return desiredSize;
    }
    
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        double decreaseButtonLength;
        double thumbLength;
        double increaseButtonLength;
    
        ComputeSliderLengths(arrangeSize, Thumb, out decreaseButtonLength, out thumbLength, out increaseButtonLength);
    
        // Layout the pieces of track
        var offset    = new Point();
        var pieceSize = arrangeSize;
    
        CoerceLength(ref decreaseButtonLength, arrangeSize.Width);
        CoerceLength(ref increaseButtonLength, arrangeSize.Width);
        CoerceLength(ref thumbLength, arrangeSize.Width);
            
        pieceSize = pieceSize.WithWidth(decreaseButtonLength);
    
        DecreaseButton?.Arrange(new Rect(offset, pieceSize));
    
        offset    = offset.WithX(decreaseButtonLength + thumbLength);
        pieceSize = pieceSize.WithWidth(increaseButtonLength);
    
        IncreaseButton?.Arrange(new Rect(offset, pieceSize));
    
        offset    = offset.WithX(decreaseButtonLength);
        pieceSize = pieceSize.WithWidth(thumbLength);
    
        if (Thumb != null)
        {
            var bounds = new Rect(offset, pieceSize);
            var adjust = CalculateThumbAdjustment(Thumb, bounds);
            Thumb.Arrange(bounds);
            Thumb.AdjustDrag(adjust);
        }
    
        ThumbCenterOffset = offset.X + (thumbLength * 0.5);
        LastDrag          = default;
        return arrangeSize;
    }
    
    private void ThumbChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var oldThumb = (Thumb?)e.OldValue;
        var newThumb = (Thumb?)e.NewValue;

        if (oldThumb != null)
        {
            oldThumb.DragDelta     -= ThumbDragged;
            oldThumb.DragCompleted -= ThumbDragCompleted;
            LogicalChildren.Remove(oldThumb);
            VisualChildren.Remove(oldThumb);
        }

        if (newThumb != null)
        {
            newThumb.DragDelta     += ThumbDragged;
            newThumb.DragCompleted += ThumbDragCompleted;
            LogicalChildren.Add(newThumb);
            VisualChildren.Add(newThumb);
        }
    }

    private void ButtonChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var oldButton = (AvaloniaButton?)e.OldValue;
        var newButton = (AvaloniaButton?)e.NewValue;

        if (oldButton != null)
        {
            LogicalChildren.Remove(oldButton);
            VisualChildren.Remove(oldButton);
        }

        if (newButton != null)
        {
            LogicalChildren.Add(newButton);
            VisualChildren.Add(newButton);
        }
    }
}