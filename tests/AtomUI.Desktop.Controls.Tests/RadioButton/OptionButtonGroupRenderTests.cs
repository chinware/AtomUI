using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Headless;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.RadioButton;

public class OptionButtonGroupRenderTests
{
    static OptionButtonGroupRenderTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(1.5, 2d / 3d)]
    [InlineData(2.0, 1d)]
    public void Group_Render_Uses_Render_Scale_Aware_BorderThickness(
        double renderScaling,
        double expectedThickness)
    {
        var group = new Desktop.Controls.OptionButtonGroup
        {
            Width           = 220,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1),
            ButtonStyle     = AtomUI.Controls.OptionButtonStyle.Solid
        };
        group.Items.Add(new Desktop.Controls.OptionButton { Content = "One" });

        ShowInWindow(group, window =>
        {
            window.SetRenderScaling(renderScaling);
            Dispatcher.UIThread.RunJobs();

            RenderToDrawingGroup(group);

            GetCachedBorderThickness(group).ShouldBe(new Thickness(expectedThickness));
        });
    }

    [Fact]
    public void Vertical_Separator_Uses_Group_Local_Horizontal_Geometry()
    {
        var group = new Desktop.Controls.OptionButtonGroup
        {
            Orientation     = Orientation.Vertical,
            Width           = 220,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1, 2, 3, 4),
            ItemsPanel = new FuncTemplate<Panel?>(() => new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin      = new Thickness(10)
            })
        };
        group.Items.Add(new Desktop.Controls.OptionButton { Content = "One" });
        group.Items.Add(new Desktop.Controls.OptionButton { Content = "Two" });

        ShowInWindow(group, () =>
        {
            var first = group.ContainerFromIndex(0).ShouldBeOfType<Desktop.Controls.OptionButton>();
            var firstOrigin = first.TranslatePoint(default, group).ShouldNotBeNull();
            var expectedY   = firstOrigin.Y + first.Bounds.Height - 1;
            var separator = EnumerateGeometryDrawings(RenderToDrawingGroup(group))
                .Where(drawing => drawing.Pen?.Brush is ISolidColorBrush brush && brush.Color == Colors.Black)
                .Select(drawing => drawing.Geometry?.Bounds ?? default)
                .Where(bounds => Math.Abs(bounds.Width) < 0.01 || Math.Abs(bounds.Height) < 0.01)
                .ShouldHaveSingleItem();

            separator.Y.ShouldBe(expectedY, 0.01);
            separator.Height.ShouldBe(0, 0.01);
            separator.X.ShouldBe(0, 0.01);
            separator.Width.ShouldBe(group.Bounds.Width, 0.01);
        });
    }

    [Fact]
    public void ItemsSource_Outline_Uses_SelectedIndex_To_Render_Selected_Border()
    {
        var group = new Desktop.Controls.OptionButtonGroup
        {
            Orientation     = Orientation.Vertical,
            Width           = 220,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1),
            ItemsSource     = new[] { "One", "Two", "Three" },
            SelectedIndex   = 1
        };

        ShowInWindow(group, () =>
        {
            group.SelectedIndex.ShouldBe(1);
            var selectedBrush = GetInternalValue<IBrush>(group, "SelectedOptionBorderColor");
            var selectedColor = GetSolidColor(selectedBrush);

            EnumerateGeometryDrawings(RenderToDrawingGroup(group))
                .Count(drawing =>
                    drawing.Brush is ISolidColorBrush fill && fill.Color == selectedColor ||
                    drawing.Pen?.Brush is ISolidColorBrush stroke && stroke.Color == selectedColor)
                .ShouldBeGreaterThan(0);
        });
    }

    [Fact]
    public void Horizontal_Stretch_Preserves_The_Natural_Outer_Border_Width()
    {
        var group = new Desktop.Controls.OptionButtonGroup
        {
            ButtonStyle     = AtomUI.Controls.OptionButtonStyle.Solid,
            BorderBrush     = Brushes.Black,
            BorderThickness = new Thickness(1)
        };
        group.Items.Add(new Desktop.Controls.OptionButton { Content = "One" });
        group.Items.Add(new Desktop.Controls.OptionButton { Content = "Two" });

        ShowInWindow(group, () =>
        {
            group.Bounds.Width.ShouldBeGreaterThan(group.DesiredSize.Width);

            RenderToDrawingGroup(group);

            GetCachedBorderRenderSize(group).Width.ShouldBe(group.DesiredSize.Width, 0.01);
        });
    }

    [Fact]
    public void ChildIndex_Subscription_Is_Owned_By_Visual_Attachment_Lifecycle()
    {
        var source = ReadRepoFile(
            "src/AtomUI.Controls/OptionButtonGroup/AbstractOptionButtonGroup.cs");
        var attachedStart = source.IndexOf("OnAttachedToVisualTree", StringComparison.Ordinal);
        var detachedStart = source.IndexOf("OnDetachedFromVisualTree", StringComparison.Ordinal);

        attachedStart.ShouldBeGreaterThanOrEqualTo(0);
        detachedStart.ShouldBeGreaterThan(attachedStart);
        source[..attachedStart].ShouldNotContain("ChildIndexChanged +=");
        source[attachedStart..detachedStart].ShouldContain("ChildIndexChanged +=");
        source[detachedStart..].ShouldContain("ChildIndexChanged -=");
        source.ShouldContain("SelectedIndexProperty,");
    }

    private static DrawingGroup RenderToDrawingGroup(Desktop.Controls.OptionButtonGroup group)
    {
        var drawingGroup = new DrawingGroup();
        using var context = drawingGroup.Open();
        group.Render(context);
        return drawingGroup;
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

    private static Color GetSolidColor(IBrush brush)
    {
        return brush.ShouldBeAssignableTo<ISolidColorBrush>().Color;
    }

    private static Size GetCachedBorderRenderSize(Desktop.Controls.OptionButtonGroup group)
    {
        var helper = group.GetType()
                          .BaseType!
                          .GetField("_borderRenderHelper", BindingFlags.Instance | BindingFlags.NonPublic)!
                          .GetValue(group)!;
        return helper.GetType()
                     .GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic)!
                     .GetValue(helper)
                     .ShouldBeOfType<Size>();
    }

    private static Thickness GetCachedBorderThickness(Desktop.Controls.OptionButtonGroup group)
    {
        var helper = group.GetType()
                          .BaseType!
                          .GetField("_borderRenderHelper", BindingFlags.Instance | BindingFlags.NonPublic)!
                          .GetValue(group)!;
        return helper.GetType()
                     .GetField("_borderThickness", BindingFlags.Instance | BindingFlags.NonPublic)!
                     .GetValue(helper)
                     .ShouldBeOfType<Thickness>();
    }

    private static T GetInternalValue<T>(object instance, string propertyName)
    {
        return instance.GetType()
                       .BaseType!
                       .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)!
                       .GetValue(instance)
                       .ShouldBeAssignableTo<T>()!;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 480,
            Height  = 420,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 480,
            Height  = 420,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            assertion(window);
        }
        finally
        {
            window.Close();
        }
    }

    private static string ReadRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
