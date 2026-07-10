using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.GroupBox;

public class GroupBoxRenderTests
{
    static GroupBoxRenderTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Transparent_Background_Does_Not_Use_Header_Background_Mask()
    {
        var groupBox = new AtomUI.Desktop.Controls.GroupBox
        {
            Width            = 180,
            Height           = 100,
            HeaderTitle      = "Title",
            HeaderTitleColor = Brushes.Transparent,
            Background       = Brushes.Transparent,
            BorderBrush      = Brushes.Black,
            BorderThickness  = new Thickness(2),
            Content          = new Border { Height = 48 }
        };
        var root = new Border
        {
            Width      = 240,
            Height     = 140,
            Background = Brushes.White,
            Padding    = new Thickness(24),
            Child      = groupBox
        };

        ShowInWindow(root, window =>
        {
            var headerContent = groupBox.GetVisualDescendants()
                                        .OfType<Decorator>()
                                        .Single(item => item.Name == "PART_HeaderContent");

            var headerOffset = headerContent.TranslatePoint(default, window);
            headerOffset.ShouldNotBeNull();

            var groupBoxOffset = groupBox.TranslatePoint(default, window);
            groupBoxOffset.ShouldNotBeNull();

            var headerBounds = new Rect(
                headerOffset.Value - groupBoxOffset.Value,
                headerContent.Bounds.Size);
            var drawingGroup = RenderToDrawingGroup(groupBox);

            HasTransparentHeaderMaskDrawing(drawingGroup, headerBounds).ShouldBeFalse(
                "the header gap must be excluded from the border geometry instead of covered with GroupBox.Background");
        });
    }

    [Fact]
    public void Auto_Height_Includes_Header_And_Content_Padding()
    {
        var content = new Border
        {
            Height = 160
        };
        var groupBox = new AtomUI.Desktop.Controls.GroupBox
        {
            HeaderTitle = "Title",
            Content     = content
        };
        var root = new StackPanel
        {
            Width    = 240,
            Children =
            {
                groupBox
            }
        };

        ShowInWindow(root, window =>
        {
            var headerContent = groupBox.GetVisualDescendants()
                                        .OfType<Decorator>()
                                        .Single(item => item.Name == "PART_HeaderContent");
            var contentPresenter = groupBox.GetVisualDescendants()
                                           .OfType<ContentPresenter>()
                                           .Single(item => item.Name == "PART_ContentPresenter");

            groupBox.Bounds.Height.ShouldBeGreaterThan(
                content.Bounds.Height + headerContent.Bounds.Height / 2,
                "auto height must include the fieldset header lane in addition to content height");
            contentPresenter.Bounds.Height.ShouldBeGreaterThanOrEqualTo(
                content.Bounds.Height,
                "the content presenter must allocate enough height for the measured content");
        });
    }

    [Fact]
    public void Render_Uses_Render_Scale_Aware_BorderThickness_For_Geometry()
    {
        var groupBox = new AtomUI.Desktop.Controls.GroupBox
        {
            Width           = 180,
            Height          = 100,
            HeaderTitle     = "Title",
            Background      = Brushes.Transparent,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1),
            CornerRadius    = new CornerRadius(6),
            Content         = new Border { Height = 48 }
        };
        var root = new Border
        {
            Width   = 240,
            Height  = 140,
            Padding = new Thickness(24),
            Child   = groupBox
        };

        ShowInWindow(root, window =>
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            RenderToDrawingGroup(groupBox);

            GetCachedBorderThickness(groupBox).ShouldBe(new Thickness(2d / 3d));
        });
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 240,
            Height  = 140,
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
        }
    }

    private static DrawingGroup RenderToDrawingGroup(AtomUI.Desktop.Controls.GroupBox groupBox)
    {
        var drawingGroup = new DrawingGroup();
        using var context = drawingGroup.Open();
        groupBox.Render(context);
        return drawingGroup;
    }

    private static Thickness GetCachedBorderThickness(AtomUI.Desktop.Controls.GroupBox groupBox)
    {
        var field = typeof(AtomUI.Desktop.Controls.GroupBox).GetField(
            "_cachedBorderThickness",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();

        return (Thickness)field!.GetValue(groupBox)!;
    }

    private static bool HasTransparentHeaderMaskDrawing(DrawingGroup drawingGroup, Rect headerBounds)
    {
        return EnumerateGeometryDrawings(drawingGroup)
            .Any(drawing =>
            {
                if (drawing.Pen is not null || drawing.Brush is not ISolidColorBrush brush || brush.Color.A != 0)
                {
                    return false;
                }

                var bounds = drawing.Geometry?.Bounds ?? default;
                return AreClose(bounds, headerBounds);
            });
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

    private static bool AreClose(Rect left, Rect right)
    {
        const double tolerance = 0.001;
        return Math.Abs(left.X - right.X) < tolerance
               && Math.Abs(left.Y - right.Y) < tolerance
               && Math.Abs(left.Width - right.Width) < tolerance
               && Math.Abs(left.Height - right.Height) < tolerance;
    }
}
