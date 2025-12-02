using System.Diagnostics;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public abstract class DrawingInstruction
{
    public Matrix? Transform { get; set; }
    public double Opacity { get; set; } = 1.0;

    public IconBrushType? StrokeBrush;
    public IconBrushType? FillBrush;

    public bool IsStrokeEnabled { get; set; } = false;
    public bool IsStrokeWidthCustomizable { get; set; } = false;
    public bool IsStrokeLinejoinCustomizable { get; set; } = false;
    public bool IsStrokeLinecapCustomizable { get; set; } = false;
    
    public abstract void Draw(DrawingContext drawingContext, Icon icon);

    protected IPen? BuildPen(Icon icon)
    {
        if (!IsStrokeEnabled || StrokeBrush == null)
        {
            return null;
        }

        var pen = new Pen(icon.FindIconBrush(StrokeBrush.Value));
        if (IsStrokeWidthCustomizable)
        {
            pen.Thickness = icon.StrokeWidth;
        }

        if (IsStrokeLinecapCustomizable)
        {
            pen.LineCap = icon.StrokeLineCap;
        }

        if (IsStrokeLinejoinCustomizable)
        {
            pen.LineJoin = icon.StrokeLineJoin;
        }
        return pen;
    }
}

public class RectDrawingInstruction : DrawingInstruction
{
    public Rect Rect { get; set; }
    public double RadiusX { get; set; } = 0;
    public double RadiusY { get; set; } = 0;
    
    public override void Draw(DrawingContext drawingContext, Icon icon)
    {
        IBrush? fillBrush = null;
        if (FillBrush != null)
        {
            fillBrush = icon.FindIconBrush(FillBrush.Value);
        }
        var       pen            = BuildPen(icon);
        var       transform      = Transform ?? Matrix.Identity;
        using var transformState = drawingContext.PushTransform(transform);
        using var opacityState   = drawingContext.PushOpacity(Opacity);
        drawingContext.DrawRectangle(fillBrush, pen, Rect, RadiusX, RadiusY);
    }
}

public class CircleDrawingInstruction : DrawingInstruction
{
    public double Radius { get; set; }
    public Point Center { get; set; }
    
    public override void Draw(DrawingContext drawingContext, Icon icon)
    {
        IBrush? fillBrush = null;
        if (FillBrush != null)
        {
            fillBrush = icon.FindIconBrush(FillBrush.Value);
        }
        var       pen            = BuildPen(icon);
        var       transform      = Transform ?? Matrix.Identity;
        using var transformState = drawingContext.PushTransform(transform);
        using var opacityState   = drawingContext.PushOpacity(Opacity);
        drawingContext.DrawEllipse(fillBrush, pen, Center, Radius, Radius);
    }
}

public class EllipseDrawingInstruction : DrawingInstruction
{
    public double RadiusX { get; set; }
    public double RadiusY { get; set; }
    public Point Center { get; set; }
    
    public override void Draw(DrawingContext drawingContext, Icon icon)
    {
        IBrush? fillBrush = null;
        if (FillBrush != null)
        {
            fillBrush = icon.FindIconBrush(FillBrush.Value);
        }
        var       pen            = BuildPen(icon);
        var       transform      = Transform ?? Matrix.Identity;
        using var transformState = drawingContext.PushTransform(transform);
        using var opacityState   = drawingContext.PushOpacity(Opacity);
        drawingContext.DrawEllipse(fillBrush, pen, Center, RadiusX, RadiusY);
    }
}

public class LineDrawingInstruction : DrawingInstruction
{
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }

    public override void Draw(DrawingContext drawingContext, Icon icon)
    {
        var       transform      = Transform ?? Matrix.Identity;
        using var transformState = drawingContext.PushTransform(transform);
        using var opacityState   = drawingContext.PushOpacity(Opacity);
        var       pen            = BuildPen(icon);
        Debug.Assert(pen != null);
        drawingContext.DrawLine(pen, StartPoint, EndPoint);
    }
}

public class PolylineDrawingInstruction : DrawingInstruction
{
    public IList<Point> Points { get; set; } = Array.Empty<Point>();
    
    public override void Draw(DrawingContext drawingContext, Icon icon)
    {
        var       pen            = BuildPen(icon);
        var       transform      = Transform ?? Matrix.Identity;
        using var transformState = drawingContext.PushTransform(transform);
        using var opacityState   = drawingContext.PushOpacity(Opacity);
        var geometry = new PolylineGeometry()
        {
            Points = Points,
            IsFilled = false,
        };
        drawingContext.DrawGeometry(null, pen, geometry);
    }
}

public class PolygonDrawingInstruction : DrawingInstruction
{
    public IList<Point> Points { get; set; } = Array.Empty<Point>();
    
    public override void Draw(DrawingContext drawingContext, Icon icon)
    {
        IBrush? fillBrush = null;
        if (FillBrush != null)
        {
            fillBrush = icon.FindIconBrush(FillBrush.Value);
        }
        var       pen            = BuildPen(icon);
        var       transform      = Transform ?? Matrix.Identity;
        using var transformState = drawingContext.PushTransform(transform);
        using var opacityState   = drawingContext.PushOpacity(Opacity);
        var geometry = new PolylineGeometry()
        {
            Points   = Points,
            IsFilled = true,
        };
        drawingContext.DrawGeometry(fillBrush, pen, geometry);
    }
}

public class PathDrawingInstruction : DrawingInstruction
{
    public Geometry? Data { get; set; }
    public override void Draw(DrawingContext drawingContext, Icon icon)
    {
        if (Data == null)
        {
            return;
        }
        IBrush? fillBrush = null;
        if (FillBrush != null)
        {
            fillBrush = icon.FindIconBrush(FillBrush.Value);
        }
        var       pen            = BuildPen(icon);
        var       transform      = Transform ?? Matrix.Identity;
        using var transformState = drawingContext.PushTransform(transform);
        using var opacityState   = drawingContext.PushOpacity(Opacity);
        drawingContext.DrawGeometry(fillBrush, pen, Data);
    }
}