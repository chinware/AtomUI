using Avalonia;

namespace AtomUI.Icons;

public class SvgGraphicElement
{
    public string? Transform { get; set; }
    public string? FillColor { get; set; }
    public string? StrokeColor { get; set; }
    public string? StrokeWidth { get; set; }
    public string? StrokeLineCap { get; set; }
    public string? StrokeLineJoin { get; set; }
    public double Opacity { get; set; } = 1.0;
}

public class PathElement : SvgGraphicElement
{
    public string? Data { get; set; }
}

public class RectElement : SvgGraphicElement
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double RadiusX { get; set; }
    public double RadiusY { get; set; }
}

public class CircleElement : SvgGraphicElement
{
    public double Radius { get; set; }
    public double CenterX { get; set; }
    public double CenterY { get; set; }
}

public class EllipseElement : SvgGraphicElement
{
    public double RadiusX { get; set; }
    public double RadiusY { get; set; }
    public double CenterX { get; set; }
    public double CenterY { get; set; }
}

public class LineElement : SvgGraphicElement
{
    public double X1 { get; set; }
    public double Y1 { get; set; }
    public double X2 { get; set; }
    public double Y2 { get; set; }
}

public class PolylineElement : SvgGraphicElement
{
    public List<Point> Points { get; set; } = new();
}

public class PolygonElement : SvgGraphicElement
{
    public List<Point> Points { get; set; } = new();
}

public class ViewBox
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class GroupElement : SvgGraphicElement
{
}

public record SvgParsedInfo
{
    public List<SvgGraphicElement> GraphicElements { get; set; } = new();
    public ViewBox ViewBox { get; set; } = new ();
}