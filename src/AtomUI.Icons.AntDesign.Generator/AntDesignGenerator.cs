using System.Globalization;
using System.Text;
using AtomUI.Controls;
using AtomUI.Media;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Media;

namespace AtomUI.Icons.AntDesign.Generator;

public class AntDesignGenerator : DefaultIconPackageGenerator
{
    private List<string> _twoToneTplPrimaryColors;
    private readonly List<string> _twoToneTplSecondaryColors;

    public AntDesignGenerator(string sourcePath, string targetPath)
        : base(sourcePath, targetPath)
    {
        PackageName              = "AntDesign";
        PackageNamespace         = "AtomUI.Icons.AntDesign";
        _twoToneTplPrimaryColors = ["#333"];
        _twoToneTplSecondaryColors = [
            "#E6E6E6",
            "#D9D9D9",
            "#D8D8D8"
        ];
    }

    public static async Task<int> Main(string[] args)
    {
        try
        {
            SetupAvalonia();
            var targetProjectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../src/AtomUI.Icons.AntDesign"));
            var sourceProjectPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../../src/AtomUI.Icons.AntDesign.Generator"));
            var sourcePath        = Path.Combine(sourceProjectPath, "Assets/Svg");
            var generator         = new AntDesignGenerator(sourcePath, targetProjectPath);
            await generator.GenerateAsync().ConfigureAwait(false);
            return 0;
        }
        catch  (Exception e)
        {
            Console.Error.WriteLine($"Generate error: {e.Message}");
#if DEBUG
            throw;
#else
            return 1;
#endif
        }
    }

    private static void SetupAvalonia()
    {
        AppBuilder.Configure<GeneratorApplication>()
                  .UseHeadless(new AvaloniaHeadlessPlatformOptions())
                  .SetupWithLifetime(new ClassicDesktopStyleApplicationLifetime());
        SynchronizationContext.SetSynchronizationContext(null);
    }

    protected override async Task GenerateIconPackageClass(IconFileInfo iconFileInfo, Stream output)
    {
        var      sourceText = new StringBuilder();
        sourceText.AppendLine("// This code is auto generated. Do not modify.");
        sourceText.AppendLine("// Copyright (c) Qinware Technologies Co., Ltd. 2019-2025. All rights reserved.");
        sourceText.AppendLine("// Copyright (c) Ant Design Authors 2025 https://github.com/ant-design/ant-design");
        sourceText.AppendLine("");
        sourceText.AppendLine("using System.Collections.Generic;");
        sourceText.AppendLine("using Avalonia;");
        sourceText.AppendLine("using Avalonia.Media;");
        sourceText.AppendLine("using AtomUI.Controls;");
        sourceText.AppendLine($"namespace {PackageNamespace};");
        sourceText.AppendLine("");
        var svgSource     = await File.ReadAllTextAsync(iconFileInfo.FilePath);
        var    svgParsedInfo = SvgParser.Parse(svgSource);
        var    viewBox       = svgParsedInfo.ViewBox;
        var    viewBoxRect   = new Rect(viewBox.X, viewBox.Y, viewBox.Width, viewBox.Height);
        var    geometryBounds = CalculateGeometryBounds(svgParsedInfo);
        var    zoomMatrix     = CalculateZoomToFit(viewBoxRect, geometryBounds);
        var    className     = $"{iconFileInfo.Name}{iconFileInfo.ThemeType}";
        sourceText.AppendLine($"public class {className} : AntDesignIcon");
        sourceText.AppendLine(@"{");
        sourceText.AppendLine($"    public {className}()");
        sourceText.AppendLine(@"    {");
        sourceText.AppendLine($"        IconTheme = IconThemeType.{iconFileInfo.ThemeType};");
        sourceText.AppendLine($"        ViewBox = {FormatRect(viewBoxRect)};");
        sourceText.AppendLine(@"    }");
        sourceText.AppendLine(@"");
        sourceText.AppendLine(@"    internal override bool HasGeneratedGeometryMetadata => true;");
        sourceText.AppendLine($"    internal override Rect GeneratedViewBox => {FormatRect(viewBoxRect)};");
        sourceText.AppendLine($"    internal override Rect GeneratedGeometryBounds => {FormatRect(geometryBounds)};");
        sourceText.AppendLine($"    internal override Matrix GeneratedZoomMatrix => {FormatMatrix(zoomMatrix)};");
        sourceText.AppendLine(@"");
        sourceText.AppendLine(@"    private static readonly DrawingInstruction[] StaticInstructions = [");
        for (var i = 0; i < svgParsedInfo.GraphicElements.Count; i++)
        {
            var graphicElement  = svgParsedInfo.GraphicElements[i];
            if (graphicElement is PathElement pathElement)
            {
                sourceText.AppendLine(@"        new PathDrawingInstruction()");
                sourceText.AppendLine(@"        {");
                sourceText.AppendLine($"            Data = StreamGeometry.Parse(\"{pathElement.Data}\"),");
                if (iconFileInfo.ThemeType == IconThemeType.Filled)
                {
                    sourceText.AppendLine($"            FillBrush = IconBrushType.Fill,");
                }
                else if (iconFileInfo.ThemeType == IconThemeType.Outlined)
                {
                    sourceText.AppendLine($"            FillBrush = IconBrushType.Stroke,");
                }
                else if (iconFileInfo.ThemeType == IconThemeType.TwoTone)
                {
                    var isPrimary = !(pathElement.FillColor != null &&
                                      _twoToneTplSecondaryColors.Contains(pathElement.FillColor));
                    if (isPrimary)
                    {
                        sourceText.AppendLine($"            FillBrush = IconBrushType.Stroke,");
                    }
                    else
                    {
                        sourceText.AppendLine($"            FillBrush = IconBrushType.Fill,");
                    }
                }

                if (!string.IsNullOrEmpty(pathElement.Transform))
                {
                    sourceText.AppendLine($"            Transform = {FormatMatrix(TransformParser.Parse(pathElement.Transform).Value)}");
                }
                sourceText.AppendLine(@"        }");
            }

            if (i != svgParsedInfo.GraphicElements.Count - 1)
            {
                sourceText.Append(", ");
            }
        }
        sourceText.AppendLine(@"    ];");
        sourceText.AppendLine(@"");
        sourceText.AppendLine(@"    protected override IList<DrawingInstruction> DrawingInstructions => StaticInstructions;");
        sourceText.AppendLine("}");
        sourceText.AppendLine("");

        await output.WriteAsync(Encoding.UTF8.GetBytes(sourceText.ToString()));
    }

    private static Rect CalculateGeometryBounds(SvgParsedInfo svgParsedInfo)
    {
        var group = new GeometryGroup();
        foreach (var graphicElement in svgParsedInfo.GraphicElements)
        {
            if (graphicElement is not PathElement pathElement || string.IsNullOrEmpty(pathElement.Data))
            {
                continue;
            }

            var geometry = StreamGeometry.Parse(pathElement.Data);
            if (!string.IsNullOrEmpty(pathElement.Transform))
            {
                geometry.Transform = new MatrixTransform(TransformParser.Parse(pathElement.Transform).Value);
            }

            group.Children.Add(geometry);
        }

        return group.Bounds;
    }

    private static Matrix CalculateZoomToFit(Rect viewbox, Rect iconBounds)
    {
        var viewboxCenter = new Point(
            viewbox.Left + viewbox.Width / 2,
            viewbox.Top + viewbox.Height / 2
        );

        var leftDelta   = iconBounds.Left - viewbox.Left;
        var rightDelta  = viewbox.Right - iconBounds.Right;
        var topDelta    = iconBounds.Top - viewbox.Top;
        var bottomDelta = viewbox.Bottom - iconBounds.Bottom;

        var minDelta = leftDelta;
        if (rightDelta < minDelta)
        {
            minDelta = rightDelta;
        }

        if (topDelta < minDelta)
        {
            minDelta = topDelta;
        }

        if (bottomDelta < minDelta)
        {
            minDelta = bottomDelta;
        }

        minDelta /= 2;

        var iconLeftDist   = iconBounds.Left - viewboxCenter.X - minDelta;
        var iconRightDist  = iconBounds.Right - viewboxCenter.X - minDelta;
        var iconTopDist    = iconBounds.Top - viewboxCenter.Y -  minDelta;
        var iconBottomDist = iconBounds.Bottom - viewboxCenter.Y - minDelta;

        var viewboxLeftDist   = viewbox.Left - viewboxCenter.X;
        var viewboxRightDist  = viewbox.Right - viewboxCenter.X;
        var viewboxTopDist    = viewbox.Top - viewboxCenter.Y;
        var viewboxBottomDist = viewbox.Bottom - viewboxCenter.Y;

        var maxScale = double.MaxValue;

        if (Math.Abs(iconLeftDist) > 0.0001)
        {
            var scaleLeft = viewboxLeftDist / iconLeftDist;
            if (scaleLeft > 0 && scaleLeft < maxScale)
            {
                maxScale = scaleLeft;
            }
        }

        if (Math.Abs(iconRightDist) > 0.0001)
        {
            var scaleRight = viewboxRightDist / iconRightDist;
            if (scaleRight > 0 && scaleRight < maxScale)
            {
                maxScale = scaleRight;
            }
        }

        if (Math.Abs(iconTopDist) > 0.0001)
        {
            var scaleTop = viewboxTopDist / iconTopDist;
            if (scaleTop > 0 && scaleTop < maxScale)
            {
                maxScale = scaleTop;
            }
        }

        if (Math.Abs(iconBottomDist) > 0.0001)
        {
            var scaleBottom = viewboxBottomDist / iconBottomDist;
            if (scaleBottom > 0 && scaleBottom < maxScale)
            {
                maxScale = scaleBottom;
            }
        }

        if (maxScale > 1000 || maxScale <= 0)
        {
            maxScale = 1.0;
        }

        var transform = Matrix.Identity;
        transform *= Matrix.CreateTranslation(-viewboxCenter.X, -viewboxCenter.Y);
        transform *= Matrix.CreateScale(maxScale, maxScale);
        transform *= Matrix.CreateTranslation(viewboxCenter.X, viewboxCenter.Y);

        return transform;
    }

    private static string FormatRect(Rect rect)
    {
        return $"new Rect({FormatDouble(rect.X)}, {FormatDouble(rect.Y)}, {FormatDouble(rect.Width)}, {FormatDouble(rect.Height)})";
    }

    private static string FormatMatrix(Matrix matrix)
    {
        return $"new Matrix({FormatDouble(matrix.M11)}, {FormatDouble(matrix.M12)}, {FormatDouble(matrix.M21)}, {FormatDouble(matrix.M22)}, {FormatDouble(matrix.M31)}, {FormatDouble(matrix.M32)})";
    }

    private static string FormatDouble(double value)
    {
        if (Math.Abs(value) < 1e-12)
        {
            value = 0;
        }
        return value.ToString("G17", CultureInfo.InvariantCulture);
    }

    private sealed class GeneratorApplication : Application;
}
