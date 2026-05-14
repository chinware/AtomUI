// This code is auto generated. Do not modify.
// Copyright (c) Qinware Technologies Co., Ltd. 2019-2025. All rights reserved.
// Copyright (c) Ant Design Authors 2025 https://github.com/ant-design/ant-design

using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using AtomUI.Controls;
namespace AtomUI.Icons.AntDesign;

public class WindowPinOutlined : AntDesignIcon
{
    public WindowPinOutlined()
    {
        IconTheme = IconThemeType.Outlined;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    internal override bool HasGeneratedGeometryMetadata => true;
    internal override Rect GeneratedViewBox => new Rect(0, 0, 1024, 1024);
    internal override Rect GeneratedGeometryBounds => new Rect(213.33333300000001, 128, 597.33333400000004, 853.33333300000004);
    internal override Matrix GeneratedZoomMatrix => new Matrix(1.1428571441326529, 0, 0, 1.1428571441326529, -73.142857795918303, -73.142857795918303);

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M768 128v85.333333h-42.666667v256l85.333334 128v85.333334h-256v298.666666h-85.333334v-298.666666H213.333333v-85.333334l85.333334-128V213.333333H256V128h512zM384 213.333333v281.856L315.904 597.333333h392.192L640 495.189333V213.333333H384z"),
            FillBrush = IconBrushType.Stroke,
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}

