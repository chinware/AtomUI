using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AtomUI;
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

public class DashedBorderLayoutRoundingTests
{
    static DashedBorderLayoutRoundingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Layout_Rounded_Thickness_Helper_Matches_Avalonia_Border_Rounding()
    {
        var target = new Border();

        ShowInWindow(target, window =>
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            var actual = InvokeLayoutRoundedThickness(target, new Thickness(1));

            actual.ShouldBe(LayoutHelper.RoundLayoutThickness(new Thickness(1), 1.5));
        });
    }

    [Fact]
    public void DashedBorder_Render_Uses_Layout_Rounded_BorderThickness()
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

            GetRenderedPenThickness(target).ShouldBe(4d / 3d, 0.0001);
        });
    }

    [Fact]
    public void DashedBorder_Render_Recomputes_Layout_Rounded_BorderThickness_When_Scale_Changes()
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

            GetRenderedPenThickness(target).ShouldBe(4d / 3d, 0.0001);
        });
    }

    [Fact]
    public void OptionButton_Render_Uses_Layout_Rounded_BorderThickness()
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

            GetBorderRenderHelperThickness(target).ShouldBe(new Thickness(4d / 3d));
        });
    }

    [Fact]
    public void OptionButtonGroup_Render_Uses_Layout_Rounded_BorderThickness()
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

            GetBorderRenderHelperThickness(group).ShouldBe(new Thickness(4d / 3d));
        });
    }

    private static Thickness InvokeLayoutRoundedThickness(Layoutable target, Thickness thickness)
    {
        var borderUtilsType = typeof(LineStyle).Assembly.GetType("AtomUI.Utils.BorderUtils");
        borderUtilsType.ShouldNotBeNull();

        var method = borderUtilsType!.GetMethod(
            "BuildLayoutRoundedThickness",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        method.ShouldNotBeNull();

        return (Thickness)method!.Invoke(null, new object[] { target, thickness })!;
    }

    private static double GetRenderedPenThickness(DashedBorder border)
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
