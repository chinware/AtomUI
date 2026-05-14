// This code is auto generated. Do not modify.
// Copyright (c) Qinware Technologies Co., Ltd. 2019-2025. All rights reserved.
// Copyright (c) Ant Design Authors 2025 https://github.com/ant-design/ant-design

using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using AtomUI.Controls;
namespace AtomUI.Icons.AntDesign;

public class TwitchFilled : AntDesignIcon
{
    public TwitchFilled()
    {
        IconTheme = IconThemeType.Filled;
        ViewBox = new Rect(0, 0, 1042, 1042);
    }

    internal override bool HasGeneratedGeometryMetadata => true;
    internal override Rect GeneratedViewBox => new Rect(0, 0, 1042, 1042);
    internal override Rect GeneratedGeometryBounds => new Rect(128, 112, 742.85699999999997, 800);
    internal override Matrix GeneratedZoomMatrix => new Matrix(1.1204301075268817, 0, 0, 1.1204301075268817, -62.744086021505382, -62.744086021505382);

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M57.143 0 0 142.857v542.857h171.429V800h114.285L400 685.714h142.857l200-200V0zm314.286 428.571h-85.715V198.214h85.715zm200 0h-85.715V198.214h85.715z"),
            FillBrush = IconBrushType.Fill,
            Transform = new Matrix(1, 0, 0, 1, 128, 112)
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}

