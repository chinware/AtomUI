// This code is auto generated. Do not modify.
// Generated Date: 2025-12-02

using Avalonia;
using System;
using Avalonia.Media;
using AtomUI.Controls;
using AtomUI.Media;
namespace AtomUI.Icons.AntDesign;

public class TwitchFilled : Icon
{
    public TwitchFilled()
    {
        IconTheme = IconThemeType.Filled;
        ViewBox = new Rect(0, 0, 1042, 1042);
    }

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M57.143 0 0 142.857v542.857h171.429V800h114.285L400 685.714h142.857l200-200V0zm314.286 428.571h-85.715V198.214h85.715zm200 0h-85.715V198.214h85.715z"),
            FillBrush = IconBrushType.Fill,
            Transform = TransformParser.Parse("translate(128 112)").Value
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}

