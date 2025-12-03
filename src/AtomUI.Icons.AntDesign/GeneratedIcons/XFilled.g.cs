// This code is auto generated. Do not modify.
// Generated Date: 2025-12-02

using Avalonia;
using System;
using Avalonia.Media;
using AtomUI.Controls;
using AtomUI.Media;
namespace AtomUI.Icons.AntDesign;

public class XFilled : Icon
{
    public XFilled()
    {
        IconTheme = IconThemeType.Filled;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M711.111 800H88.89C39.8 800 0 760.2 0 711.111V88.89C0 39.8 39.8 0 88.889 0H711.11C760.2 0 800 39.8 800 88.889V711.11C800 760.2 760.2 800 711.111 800"),
            FillBrush = IconBrushType.Fill,
            Transform = TransformParser.Parse("translate(112 112)").Value
        }
,         new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M628 623H484.942L174 179h143.058zm-126.012-37.651h56.96L300.013 216.65h-56.96z"),
            FillBrush = IconBrushType.Fill,
            Transform = TransformParser.Parse("translate(112 112)").Value
        }
,         new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M219.296885 623 379 437.732409 358.114212 410 174 623z"),
            FillBrush = IconBrushType.Fill,
            Transform = TransformParser.Parse("translate(112 112)").Value
        }
,         new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M409 348.387347 429.212986 377 603 177 558.330417 177z"),
            FillBrush = IconBrushType.Fill,
            Transform = TransformParser.Parse("translate(112 112)").Value
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}

