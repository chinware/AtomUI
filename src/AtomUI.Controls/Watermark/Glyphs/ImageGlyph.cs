using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public class ImageGlyph : WatermarkGlyph
{
    public static readonly StyledProperty<double> HeightProperty = AvaloniaProperty
        .Register<ImageGlyph, double>(nameof(Height), 28);

    public static readonly StyledProperty<IImage?> SourceProperty = AvaloniaProperty
        .Register<ImageGlyph, IImage?>(nameof(Source));

    static ImageGlyph()
    {
    }

    public double Height
    {
        get => GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }

    public IImage? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        if (Source == null)
        {
            return;
        }

        var w = Height / Source.Size.Height * Source.Size.Width;
        Source.Draw(context, new Rect(new Point(), Source.Size), new Rect(new Point(), new Size(w, Height)));
    }

    public override Size GetDesiredSize()
    {
        if (Source == null)
        {
            return new Size();
        }

        var w = Height / Source.Size.Height * Source.Size.Width;
        return new Size(w, Height);
    }
}