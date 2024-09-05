using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public abstract class WatermarkGlyph : AvaloniaObject
{
    public double HorizontalSpace
    {
        get => GetValue(HorizontalSpaceProperty);
        set => SetValue(HorizontalSpaceProperty, value);
    }
    public static readonly StyledProperty<double> HorizontalSpaceProperty = AvaloniaProperty
        .Register<WatermarkGlyph, double>(nameof(HorizontalSpace), 280d);

    public double VerticalSpace
    {
        get => GetValue(VerticalSpaceProperty);
        set => SetValue(VerticalSpaceProperty, value);
    }
    public static readonly StyledProperty<double> VerticalSpaceProperty = AvaloniaProperty
        .Register<WatermarkGlyph, double>(nameof(VerticalSpace), 40d);

    public double HorizontalOffset
    {
        get => GetValue(HorizontalOffsetProperty);
        set => SetValue(HorizontalOffsetProperty, value);
    }
    public static readonly StyledProperty<double> HorizontalOffsetProperty = AvaloniaProperty
        .Register<WatermarkGlyph, double>(nameof(HorizontalOffset), 0);

    public double VerticalOffset
    {
        get => GetValue(VerticalOffsetProperty);
        set => SetValue(VerticalOffsetProperty, value);
    }
    public static readonly StyledProperty<double> VerticalOffsetProperty = AvaloniaProperty
        .Register<WatermarkGlyph, double>(nameof(VerticalOffset), 0);

    public double Rotate
    {
        get => GetValue(RotateProperty);
        set => SetValue(RotateProperty, value);
    }
    public static readonly StyledProperty<double> RotateProperty = AvaloniaProperty
        .Register<WatermarkGlyph, double>(nameof(Rotate), -20);

    public double Opacity
    {
        get => GetValue(OpacityProperty);
        set => SetValue(OpacityProperty, value);
    }
    public static readonly StyledProperty<double> OpacityProperty = AvaloniaProperty
        .Register<WatermarkGlyph, double>(nameof(Opacity), 0.3);

    public bool UseMirror
    {
        get => GetValue(UseMirrorProperty);
        set => SetValue(UseMirrorProperty, value);
    }
    public static readonly StyledProperty<bool> UseMirrorProperty = AvaloniaProperty
        .Register<WatermarkGlyph, bool>(nameof(UseMirror), false);

    public bool UseCross
    {
        get => GetValue(UseCrossProperty);
        set => SetValue(UseCrossProperty, value);
    }
    public static readonly StyledProperty<bool> UseCrossProperty = AvaloniaProperty
        .Register<WatermarkGlyph, bool>(nameof(UseCross), true);
    
    public abstract void Render(DrawingContext context);
    
    public abstract Size GetDesiredSize();
}