using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public abstract class WatermarkGlyph : AvaloniaObject
{
    public double Space
    {
        get => GetValue(SpaceProperty);
        set => SetValue(SpaceProperty, value);
    }
    public static readonly StyledProperty<double> SpaceProperty = AvaloniaProperty
        .Register<WatermarkGlyph, double>(nameof(Space));

    public double Angle
    {
        get => GetValue(AngleProperty);
        set => SetValue(AngleProperty, value);
    }
    public static readonly StyledProperty<double> AngleProperty = AvaloniaProperty
        .Register<WatermarkGlyph, double>(nameof(Angle));

    public double Opacity
    {
        get => GetValue(OpacityProperty);
        set => SetValue(OpacityProperty, value);
    }
    public static readonly StyledProperty<double> OpacityProperty = AvaloniaProperty
        .Register<WatermarkGlyph, double>(nameof(Opacity));

    public bool UseMirror
    {
        get => GetValue(UseMirrorProperty);
        set => SetValue(UseMirrorProperty, value);
    }
    public static readonly StyledProperty<bool> UseMirrorProperty = AvaloniaProperty
        .Register<WatermarkGlyph, bool>(nameof(UseMirror));

    public bool UseCross
    {
        get => GetValue(UseCrossProperty);
        set => SetValue(UseCrossProperty, value);
    }
    public static readonly StyledProperty<bool> UseCrossProperty = AvaloniaProperty
        .Register<WatermarkGlyph, bool>(nameof(UseCross));
    
    public abstract void Render(DrawingContext context);
    
    public abstract Size GetDesiredSize();
}