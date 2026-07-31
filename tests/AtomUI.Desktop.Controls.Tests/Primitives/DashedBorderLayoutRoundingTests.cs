using System.Reflection;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;
using AtomUIOptionButton = AtomUI.Desktop.Controls.OptionButton;
using AtomUIOptionButtonGroup = AtomUI.Desktop.Controls.OptionButtonGroup;

namespace AtomUI.Desktop.Controls.Tests.Primitives;

public class DashedBorderRenderScaleTests
{
    static DashedBorderRenderScaleTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Render_Scale_Aware_Thickness_Helper_Uses_Physical_Hairline_For_Fractional_Scale()
    {
        var target = new Border();

        ShowInWindow(target, window =>
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            var actual = InvokeRenderScaleAwareThickness(target, new Thickness(1));

            actual.ShouldBe(new Thickness(2d / 3d));
        });
    }

    [Fact]
    public void Render_Scale_Aware_Thickness_Helper_Keeps_Design_Thickness_For_Integer_Scale()
    {
        var target = new Border();

        ShowInWindow(target, window =>
        {
            window.SetRenderScaling(2.0);
            Dispatcher.UIThread.RunJobs();

            var actual = InvokeRenderScaleAwareThickness(target, new Thickness(1));

            actual.ShouldBe(new Thickness(1));
        });
    }

    [Fact]
    public void DashedBorder_Render_Uses_Render_Scale_Aware_BorderThickness()
    {
        var target = new DashedBorder
        {
            Width           = 80,
            Height          = 32,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1),
            StrokeDashArray = new[] { 4d, 2d }
        };

        ShowInWindow(target, window =>
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            GetRenderedPenThickness(target).ShouldBe(2d / 3d, 0.0001);
        });
    }

    [Fact]
    public void DashedBorder_Render_Recomputes_Render_Scale_Aware_BorderThickness_When_Scale_Changes()
    {
        var target = new DashedBorder
        {
            Width           = 80,
            Height          = 32,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1),
            StrokeDashArray = new[] { 4d, 2d }
        };

        ShowInWindow(target, window =>
        {
            GetRenderedPenThickness(target).ShouldBe(1d, 0.0001);

            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            GetRenderedPenThickness(target).ShouldBe(2d / 3d, 0.0001);
        });
    }

    [Fact]
    public void PixelAlignedBorder_Render_Uses_Render_Scale_Aware_BorderThickness()
    {
        var target = new PixelAlignedBorder
        {
            Width           = 80,
            Height          = 32,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1)
        };

        ShowInWindow(target, window =>
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            GetRenderedPenThickness(target).ShouldBe(2d / 3d, 0.0001);
        });
    }

    [Fact]
    public void PixelAlignedBorder_Render_Trims_A_Subpixel_Clip_Remainder()
    {
        var target = new PixelAlignedBorder
        {
            Width           = 80,
            Height          = 32,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1)
        };
        var clippedHost = new Canvas
        {
            Width        = 80,
            Height       = 31.4,
            ClipToBounds = true
        };
        clippedHost.Children.Add(target);

        ShowInWindow(clippedHost, window =>
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            RenderToDrawingGroup(target);

            target.Bounds.Height.ShouldBe(32, 0.0001);
            GetBorderRenderHelperSize(target).Height.ShouldBe(clippedHost.Bounds.Height, 0.0001);
        });
    }

    [Fact]
    public void PixelAlignedBorder_Render_Does_Not_Hide_A_Real_Clip_Overflow()
    {
        var target = new PixelAlignedBorder
        {
            Width           = 80,
            Height          = 32,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1)
        };
        var clippedHost = new Canvas
        {
            Width        = 80,
            Height       = 30,
            ClipToBounds = true
        };
        clippedHost.Children.Add(target);

        ShowInWindow(clippedHost, window =>
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            RenderToDrawingGroup(target);

            GetBorderRenderHelperSize(target).Height.ShouldBe(32, 0.0001);
        });
    }

    [Fact]
    public void OptionButton_Render_Uses_Render_Scale_Aware_BorderThickness()
    {
        var target = new AtomUIOptionButton
        {
            Width           = 80,
            Height          = 32,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1)
        };
        var group = new AtomUIOptionButtonGroup
        {
            Width  = 80,
            Height = 32
        };
        group.Items.Add(target);

        ShowInWindow(group, window =>
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            RenderToDrawingGroup(target);

            GetBorderRenderHelperThickness(target).ShouldBe(new Thickness(2d / 3d));
        });
    }

    [Fact]
    public void OptionButtonGroup_Render_Uses_Render_Scale_Aware_BorderThickness()
    {
        var target = new AtomUIOptionButton
        {
            Width           = 80,
            Height          = 32,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1)
        };
        var group = new AtomUIOptionButtonGroup
        {
            Width           = 80,
            Height          = 32,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1),
            ButtonStyle     = OptionButtonStyle.Solid
        };
        group.Items.Add(target);

        ShowInWindow(group, window =>
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            RenderToDrawingGroup(group);

            GetBorderRenderHelperThickness(group).ShouldBe(new Thickness(2d / 3d));
        });
    }

    private static Thickness InvokeRenderScaleAwareThickness(Layoutable target, Thickness thickness)
    {
        var borderUtilsType = typeof(LineStyle).Assembly.GetType("AtomUI.Utils.BorderUtils");
        borderUtilsType.ShouldNotBeNull();

        var method = borderUtilsType!.GetMethod(
            "BuildRenderScaleAwareThickness",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
            binder: null,
            types: [typeof(Layoutable), typeof(Thickness)],
            modifiers: null);
        method.ShouldNotBeNull();

        return (Thickness)method!.Invoke(null, new object[] { target, thickness })!;
    }

    private static double GetRenderedPenThickness(DashedBorder border)
    {
        return GetRenderedPenThickness((Control)border);
    }

    private static double GetRenderedPenThickness(PixelAlignedBorder border)
    {
        return GetRenderedPenThickness((Control)border);
    }

    private static double GetRenderedPenThickness(Control border)
    {
        var drawingGroup = new DrawingGroup();
        using (var context = drawingGroup.Open())
        {
            border.Render(context);
        }

        return EnumerateGeometryDrawings(drawingGroup)
            .Select(drawing => drawing.Pen?.Thickness)
            .First(thickness => thickness is not null)
            .GetValueOrDefault();
    }

    private static DrawingGroup RenderToDrawingGroup(Control control)
    {
        var drawingGroup = new DrawingGroup();
        using (var context = drawingGroup.Open())
        {
            control.Render(context);
        }

        return drawingGroup;
    }

    private static Thickness GetBorderRenderHelperThickness(AbstractOptionButton optionButton)
    {
        var helperField = typeof(AbstractOptionButton).GetField(
            "_borderRenderHelper",
            BindingFlags.Instance | BindingFlags.NonPublic);
        helperField.ShouldNotBeNull();

        var helper = helperField!.GetValue(optionButton);
        helper.ShouldNotBeNull();

        var thicknessField = helper!.GetType().GetField(
            "_borderThickness",
            BindingFlags.Instance | BindingFlags.NonPublic);
        thicknessField.ShouldNotBeNull();

        return (Thickness)thicknessField!.GetValue(helper)!;
    }

    private static Thickness GetBorderRenderHelperThickness(AbstractOptionButtonGroup optionButtonGroup)
    {
        var helperField = typeof(AbstractOptionButtonGroup).GetField(
            "_borderRenderHelper",
            BindingFlags.Instance | BindingFlags.NonPublic);
        helperField.ShouldNotBeNull();

        var helper = helperField!.GetValue(optionButtonGroup);
        helper.ShouldNotBeNull();

        var thicknessField = helper!.GetType().GetField(
            "_borderThickness",
            BindingFlags.Instance | BindingFlags.NonPublic);
        thicknessField.ShouldNotBeNull();

        return (Thickness)thicknessField!.GetValue(helper)!;
    }

    private static Size GetBorderRenderHelperSize(DashedBorder border)
    {
        var helperField = typeof(DashedBorder).GetField(
            "_borderRenderHelper",
            BindingFlags.Instance | BindingFlags.NonPublic);
        helperField.ShouldNotBeNull();

        var helper = helperField!.GetValue(border);
        helper.ShouldNotBeNull();

        var sizeField = helper!.GetType().GetField(
            "_size",
            BindingFlags.Instance | BindingFlags.NonPublic);
        sizeField.ShouldNotBeNull();

        return (Size)sizeField!.GetValue(helper)!;
    }

    private static IEnumerable<GeometryDrawing> EnumerateGeometryDrawings(Drawing drawing)
    {
        if (drawing is GeometryDrawing geometryDrawing)
        {
            yield return geometryDrawing;
        }
        else if (drawing is DrawingGroup drawingGroup)
        {
            foreach (var child in drawingGroup.Children.SelectMany(EnumerateGeometryDrawings))
            {
                yield return child;
            }
        }
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 120,
            Height  = 80,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion(window);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }
}
