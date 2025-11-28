namespace AtomUI.Icons;

public class SvgGraphicElement
{
    public string? Transform { get; set; }
}

public class PathElement : SvgGraphicElement
{
    public string Data { get; set; }
    public string? FillColor { get; set; }

    public PathElement(string data, string? fillColor = null)
    {
        Data      = data;
        FillColor = fillColor;
    }
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