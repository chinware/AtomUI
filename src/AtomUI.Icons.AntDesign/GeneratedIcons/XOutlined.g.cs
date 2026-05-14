// This code is auto generated. Do not modify.
// Copyright (c) Qinware Technologies Co., Ltd. 2019-2025. All rights reserved.
// Copyright (c) Ant Design Authors 2025 https://github.com/ant-design/ant-design

using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using AtomUI.Controls;
namespace AtomUI.Icons.AntDesign;

public class XOutlined : AntDesignIcon
{
    public XOutlined()
    {
        IconTheme = IconThemeType.Outlined;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    internal override bool HasGeneratedGeometryMetadata => true;
    internal override Rect GeneratedViewBox => new Rect(0, 0, 1024, 1024);
    internal override Rect GeneratedGeometryBounds => new Rect(103, 112, 818, 800);
    internal override Matrix GeneratedZoomMatrix => new Matrix(1.1118349619978285, 0, 0, 1.1118349619978285, -57.25950054288819, -57.25950054288819);

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M818 800 498.11 333.745l.546.437L787.084 0h-96.385L455.738 272 269.15 0H16.367l298.648 435.31-.036-.037L0 800h96.385l261.222-302.618L565.217 800zM230.96 72.727l448.827 654.546h-76.38L154.217 72.727z"),
            FillBrush = IconBrushType.Stroke,
            Transform = new Matrix(1, 0, 0, 1, 103, 112)
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}

