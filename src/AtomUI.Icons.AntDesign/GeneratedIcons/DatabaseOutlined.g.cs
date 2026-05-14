// This code is auto generated. Do not modify.
// Copyright (c) Qinware Technologies Co., Ltd. 2019-2025. All rights reserved.
// Copyright (c) Ant Design Authors 2025 https://github.com/ant-design/ant-design

using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using AtomUI.Controls;
namespace AtomUI.Icons.AntDesign;

public class DatabaseOutlined : AntDesignIcon
{
    public DatabaseOutlined()
    {
        IconTheme = IconThemeType.Outlined;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    internal override bool HasGeneratedGeometryMetadata => true;
    internal override Rect GeneratedViewBox => new Rect(0, 0, 1024, 1024);
    internal override Rect GeneratedGeometryBounds => new Rect(160, 64, 704, 896);
    internal override Matrix GeneratedZoomMatrix => new Matrix(1.0666666666666667, 0, 0, 1.0666666666666667, -34.133333333333326, -34.133333333333326);

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M832 64H192c-17.7 0-32 14.3-32 32v832c0 17.7 14.3 32 32 32h640c17.7 0 32-14.3 32-32V96c0-17.7-14.3-32-32-32zm-600 72h560v208H232V136zm560 480H232V408h560v208zm0 272H232V680h560v208zM304 240a40 40 0 1 0 80 0 40 40 0 1 0-80 0zm0 272a40 40 0 1 0 80 0 40 40 0 1 0-80 0zm0 272a40 40 0 1 0 80 0 40 40 0 1 0-80 0z"),
            FillBrush = IconBrushType.Stroke,
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}

