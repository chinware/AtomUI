// This code is auto generated. Do not modify.
// Copyright (c) Qinware Technologies Co., Ltd. 2019-2025. All rights reserved.
// Copyright (c) Ant Design Authors 2025 https://github.com/ant-design/ant-design

using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using AtomUI.Controls;
namespace AtomUI.Icons.AntDesign;

public class WindowUnpinOutlined : AntDesignIcon
{
    public WindowUnpinOutlined()
    {
        IconTheme = IconThemeType.Outlined;
        ViewBox = new Rect(0, 0, 1024, 1024);
    }

    internal override bool HasGeneratedGeometryMetadata => true;
    internal override Rect GeneratedViewBox => new Rect(0, 0, 1024, 1024);
    internal override Rect GeneratedGeometryBounds => new Rect(114.602667, 56.661332999999999, 814.57066599999996, 903.33866699999999);
    internal override Matrix GeneratedZoomMatrix => new Matrix(1.0585744527050938, 0, 0, 1.0585744527050938, -29.990119785008005, -29.990119785008005);

    private static readonly DrawingInstruction[] StaticInstructions = [
        new PathDrawingInstruction()
        {
            Data = StreamGeometry.Parse("M616.277333 618.666667H554.666667v341.333333h-85.333334V618.666667H170.666667v-85.333334l128-158.784v-73.493333L114.602667 116.992 174.933333 56.661333 929.173333 810.88l-60.330666 60.330667L616.256 618.666667zM384.64 387.050667V405.333333l-105.856 128h252.16l-146.304-146.282666zM301.781333 64H810.666667v85.333333h-85.333334v224l128 160v82.218667L639.957333 402.176V149.333333H387.136l-85.333333-85.354666z"),
            FillBrush = IconBrushType.Stroke,
        }
    ];

    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;
}

