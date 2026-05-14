// This code is auto generated. Do not modify.
// Copyright (c) Qinware Technologies Co., Ltd. 2019-2025. All rights reserved.
// Copyright (c) Ant Design Authors 2025 https://github.com/ant-design/ant-design

using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using AtomUI.Controls;
namespace AtomUI.Icons.AntDesign;

public class XFilled : AntDesignIcon
{
    public XFilled()
    {
        IconTheme = IconThemeType.Filled;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    internal override bool HasGeneratedGeometryMetadata => true;
    internal override Rect GeneratedViewBox => new Rect(0, 0, 1024, 1024);
    internal override Rect GeneratedGeometryBounds => new Rect(112, 112, 800, 800);
    internal override Matrix GeneratedZoomMatrix => new Matrix(1.1228070175438596, 0, 0, 1.1228070175438596, -62.877192982456108, -62.877192982456108);

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M711.111 800H88.89C39.8 800 0 760.2 0 711.111V88.89C0 39.8 39.8 0 88.889 0H711.11C760.2 0 800 39.8 800 88.889V711.11C800 760.2 760.2 800 711.111 800"),
            FillBrush = IconBrushType.Fill,
            Transform = new Matrix(1, 0, 0, 1, 112, 112)
        }
,         new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M628 623H484.942L174 179h143.058zm-126.012-37.651h56.96L300.013 216.65h-56.96z"),
            FillBrush = IconBrushType.Fill,
            Transform = new Matrix(1, 0, 0, 1, 112, 112)
        }
,         new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M219.296885 623 379 437.732409 358.114212 410 174 623z"),
            FillBrush = IconBrushType.Fill,
            Transform = new Matrix(1, 0, 0, 1, 112, 112)
        }
,         new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M409 348.387347 429.212986 377 603 177 558.330417 177z"),
            FillBrush = IconBrushType.Fill,
            Transform = new Matrix(1, 0, 0, 1, 112, 112)
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}

