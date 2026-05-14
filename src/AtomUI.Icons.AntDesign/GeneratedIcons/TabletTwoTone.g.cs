// This code is auto generated. Do not modify.
// Copyright (c) Qinware Technologies Co., Ltd. 2019-2025. All rights reserved.
// Copyright (c) Ant Design Authors 2025 https://github.com/ant-design/ant-design

using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using AtomUI.Controls;
namespace AtomUI.Icons.AntDesign;

public class TabletTwoTone : AntDesignIcon
{
    public TabletTwoTone()
    {
        IconTheme = IconThemeType.TwoTone;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    internal override bool HasGeneratedGeometryMetadata => true;
    internal override Rect GeneratedViewBox => new Rect(0, 0, 1024, 1024);
    internal override Rect GeneratedGeometryBounds => new Rect(160, 64, 704, 896);
    internal override Matrix GeneratedZoomMatrix => new Matrix(1.0666666666666667, 0, 0, 1.0666666666666667, -34.133333333333326, -34.133333333333326);

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M800 64H224c-35.3 0-64 28.7-64 64v768c0 35.3 28.7 64 64 64h576c35.3 0 64-28.7 64-64V128c0-35.3-28.7-64-64-64zm-8 824H232V136h560v752z"),
            FillBrush = IconBrushType.Stroke,
        }
,         new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M232 888h560V136H232v752zm280-144c22.1 0 40 17.9 40 40s-17.9 40-40 40-40-17.9-40-40 17.9-40 40-40z"),
            FillBrush = IconBrushType.Fill,
        }
,         new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M472 784a40 40 0 1 0 80 0 40 40 0 1 0-80 0z"),
            FillBrush = IconBrushType.Stroke,
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}

