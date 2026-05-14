// This code is auto generated. Do not modify.
// Copyright (c) Qinware Technologies Co., Ltd. 2019-2025. All rights reserved.
// Copyright (c) Ant Design Authors 2025 https://github.com/ant-design/ant-design

using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using AtomUI.Controls;
namespace AtomUI.Icons.AntDesign;

public class HarmonyOSOutlined : AntDesignIcon
{
    public HarmonyOSOutlined()
    {
        IconTheme = IconThemeType.Outlined;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    internal override bool HasGeneratedGeometryMetadata => true;
    internal override Rect GeneratedViewBox => new Rect(0, 0, 1024, 1024);
    internal override Rect GeneratedGeometryBounds => new Rect(134, 65, 755, 896);
    internal override Matrix GeneratedZoomMatrix => new Matrix(1.0700104493207943, 0, 0, 1.0700104493207943, -35.845350052246658, -35.845350052246658);

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M377.5 0C585.987 0 755 169.013 755 377.5S585.987 755 377.5 755 0 585.987 0 377.5 169.013 0 377.5 0m0 64C204.359 64 64 204.359 64 377.5S204.359 691 377.5 691 691 550.641 691 377.5 550.641 64 377.5 64"),
            FillBrush = IconBrushType.Stroke,
            Transform = new Matrix(1, 0, 0, 1, 134, 65)
        }
,         new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M611 824 611 896 144 896 144 824z"),
            FillBrush = IconBrushType.Stroke,
            Transform = new Matrix(1, 0, 0, 1, 134, 65)
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}

