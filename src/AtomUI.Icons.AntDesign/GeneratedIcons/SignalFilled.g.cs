// This code is auto generated. Do not modify.
// Copyright (c) Qinware Technologies Co., Ltd. 2019-2025. All rights reserved.
// Copyright (c) Ant Design Authors 2025 https://github.com/ant-design/ant-design

using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using AtomUI.Controls;
namespace AtomUI.Icons.AntDesign;

public class SignalFilled : AntDesignIcon
{
    public SignalFilled()
    {
        IconTheme = IconThemeType.Filled;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    internal override bool HasGeneratedGeometryMetadata => true;
    internal override Rect GeneratedViewBox => new Rect(0, 0, 1024, 1024);
    internal override Rect GeneratedGeometryBounds => new Rect(100, 64, 824, 896);
    internal override Matrix GeneratedZoomMatrix => new Matrix(1.0666666666666667, 0, 0, 1.0666666666666667, -34.133333333333326, -34.133333333333326);

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M584 352H440c-17.7 0-32 14.3-32 32v544c0 17.7 14.3 32 32 32h144c17.7 0 32-14.3 32-32V384c0-17.7-14.3-32-32-32zM892 64H748c-17.7 0-32 14.3-32 32v832c0 17.7 14.3 32 32 32h144c17.7 0 32-14.3 32-32V96c0-17.7-14.3-32-32-32zM276 640H132c-17.7 0-32 14.3-32 32v256c0 17.7 14.3 32 32 32h144c17.7 0 32-14.3 32-32V672c0-17.7-14.3-32-32-32z"),
            FillBrush = IconBrushType.Fill,
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}

