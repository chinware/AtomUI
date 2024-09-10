using Avalonia.Media;

namespace AtomUI;

public enum TextDecorationLine

{
    None,
    Underline,
    Overline,
    LineThrough
}

public enum LineStyle
{
    Solid,
    Double,
    Dotted,
    Dashed,
    Wavy
}

public enum Direction
{
    Left,
    Top,
    Right,
    Bottom
}

public enum Corner
{
    None = 0x00,
    TopLeft = 0x01,
    TopRight = 0x02,
    BottomLeft = 0x04,
    BottomRight = 0x08,
    All = TopLeft | TopRight | BottomLeft | BottomRight
}

public enum SizeType
{
    Large,
    Middle,
    Small
}

// 文本修饰信息定义
// 类似 CSS text-decoration
public class TextDecorationInfo
{
    public Color Color { get; set; }
    public TextDecorationLine LineType { get; set; } = TextDecorationLine.None;
    public LineStyle LineStyle { get; set; } = LineStyle.Solid;
    public int Thickness { get; set; } = 1;
}

public enum ColorNameFormat
{
    HexRgb,
    HexArgb
}