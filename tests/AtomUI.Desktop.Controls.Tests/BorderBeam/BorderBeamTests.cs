using System;
using System.Linq;
using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.BorderBeam;

public class BorderBeamTests
{
    static BorderBeamTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Default_Property_Values_Are_Stable()
    {
        var borderBeam = new AtomUI.Desktop.Controls.BorderBeam();

        borderBeam.Color.ShouldBeNull();
        borderBeam.ColorStops.ShouldBeEmpty();
        borderBeam.Outset.ShouldBeNull();
        borderBeam.BorderThickness.ShouldBe(default);
        borderBeam.CornerRadius.ShouldBe(default);
        borderBeam.IsMotionEnabled.ShouldBeTrue();
        borderBeam.Duration.ShouldBe(TimeSpan.FromSeconds(6));
        borderBeam.BeamSize.ShouldBe(100d);
    }

    [Fact]
    public void Color_Stop_Percent_Maps_To_Ant_Design_Visible_Segment()
    {
        var stops = new[]
        {
            new BorderBeamColorStop { Color = Colors.Blue, Percent = 0 },
            new BorderBeamColorStop { Color = Colors.Cyan, Percent = 50 },
            new BorderBeamColorStop { Color = Colors.Lime, Percent = 100 }
        };

        var normalized = BorderBeamColorStops.Normalize(stops, Colors.Transparent, Colors.Transparent, 70).ToArray();

        normalized.Select(stop => stop.Offset).ShouldBe(new[] { 0d, 0.35d, 0.70d, 1d });
        normalized.Select(stop => stop.Color).ShouldBe(new[]
        {
            Colors.Blue,
            Colors.Cyan,
            Colors.Lime,
            Colors.Transparent
        });
    }

    [Fact]
    public void Color_Stop_Normalization_Clamps_Input_Percent_And_Fills_Final_Stop()
    {
        var stops = new[]
        {
            new BorderBeamColorStop { Color = Colors.Red, Percent = -10 },
            new BorderBeamColorStop { Color = Colors.Orange, Percent = 40 },
            new BorderBeamColorStop { Color = Colors.Yellow, Percent = 120 }
        };

        var normalized = BorderBeamColorStops.Normalize(stops, Colors.Transparent, Colors.Transparent, 70).ToArray();

        normalized.Select(stop => stop.Offset).ShouldBe(new[] { 0d, 0.28d, 0.70d, 1d });
        normalized[^1].Color.ShouldBe(Colors.Transparent);
    }

    [Fact]
    public void Aware_Content_Geometry_Takes_Precedence_And_Refreshes_On_Notification()
    {
        var content = new TestBorderBeamAwareControl(
            new Thickness(3, 4, 5, 6),
            new CornerRadius(8, 9, 10, 11));
        var borderBeam = new AtomUI.Desktop.Controls.BorderBeam
        {
            Content         = content,
            BorderThickness = new Thickness(1),
            CornerRadius    = new CornerRadius(2)
        };

        borderBeam.EffectiveBorderBeamGeometry.ShouldBe(
            new BorderBeamGeometry(new Thickness(3, 4, 5, 6), new CornerRadius(8, 9, 10, 11)));

        content.UpdateGeometry(new Thickness(7), new CornerRadius(12));

        borderBeam.EffectiveBorderBeamGeometry.ShouldBe(
            new BorderBeamGeometry(new Thickness(7), new CornerRadius(12)));
    }

    [Fact]
    public void Own_Geometry_Is_Used_When_Content_Is_Not_Aware()
    {
        var borderBeam = new AtomUI.Desktop.Controls.BorderBeam
        {
            Content         = new Border(),
            BorderThickness = new Thickness(2),
            CornerRadius    = new CornerRadius(6)
        };

        borderBeam.EffectiveBorderBeamGeometry.ShouldBe(
            new BorderBeamGeometry(new Thickness(2), new CornerRadius(6)));
    }

    [Fact]
    public void Default_Theme_Applies_Token_Backed_Colors()
    {
        var borderBeam = new AtomUI.Desktop.Controls.BorderBeam
        {
            Width  = 120,
            Height = 60,
            Content = new Border
            {
                Width  = 120,
                Height = 60
            }
        };

        ShowInWindow(borderBeam, () =>
        {
            borderBeam.ApplyTemplate();
            borderBeam.DefaultStartColor.ShouldBeAssignableTo<ISolidColorBrush>();
            borderBeam.DefaultEndColor.ShouldBeAssignableTo<ISolidColorBrush>();
        });
    }

    [Fact]
    public void Default_Theme_Keeps_Beam_Enabled_When_Global_Motion_Is_Disabled()
    {
        var borderBeam = new AtomUI.Desktop.Controls.BorderBeam
        {
            Width  = 120,
            Height = 60,
            Content = new Border
            {
                Width  = 120,
                Height = 60
            }
        };
        var provider = new ThemeConfigProvider
        {
            Config = new ThemeConfigBuilder()
                     .WithToken(nameof(SharedTokenKind.EnableMotion), "false")
                     .Build(),
            Child = borderBeam
        };

        ShowInWindow(provider, () =>
        {
            var presenter = borderBeam.GetVisualDescendants()
                                      .OfType<BorderBeamPresenter>()
                                      .Single(item => item.Name == "PART_BeamPresenter");

            borderBeam.IsMotionEnabled.ShouldBeTrue();
            presenter.IsMotionEnabled.ShouldBeTrue();
            presenter.IsVisible.ShouldBeTrue();
        });
    }

    private sealed class TestBorderBeamAwareControl : Control, IBorderBeamAwareControl
    {
        private BorderBeamGeometry _geometry;

        public TestBorderBeamAwareControl(Thickness borderThickness, CornerRadius cornerRadius)
        {
            _geometry = new BorderBeamGeometry(borderThickness, cornerRadius);
        }

        public event EventHandler? BorderBeamGeometryChanged;

        public BorderBeamGeometry GetBorderBeamGeometry() => _geometry;

        public void UpdateGeometry(Thickness borderThickness, CornerRadius cornerRadius)
        {
            _geometry = new BorderBeamGeometry(borderThickness, cornerRadius);
            BorderBeamGeometryChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 240,
            Height  = 160,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void Path_Sampler_Uses_Beam_Size_Rounded_Motion_Path()
    {
        var point = BorderBeamPathSampler.GetPointAtProgress(
            new Rect(0, 0, 420, 180),
            100d,
            0d);

        point.Point.ShouldBe(new Point(100, 0));
        point.Tangent.ShouldBe(new Vector(1, 0));
    }

    [Fact]
    public void Beam_Transform_Uses_Ant_Design_Offset_Anchor()
    {
        var pathPoint = new BorderBeamPathPoint(new Point(100, 0), new Vector(1, 0));
        var transform = BorderBeamPathSampler.CreateBeamTransform(pathPoint, 100d);

        transform.Transform(new Point(90, 50)).ShouldBe(pathPoint.Point);
        transform.Transform(new Point(100, 50)).ShouldBe(new Point(110, 0));
    }

    [Fact]
    public void Path_Sampler_Keeps_Elliptical_Corner_Speed_Uniform()
    {
        var samples = new[]
        {
            0d,
            1d / 16d,
            2d / 16d,
            3d / 16d,
            4d / 16d
        }.Select(progress => BorderBeamPathSampler.GetPointAtProgress(
            new Rect(0, 0, 200, 80),
            100d,
            progress).Point).ToArray();
        var distances = samples
            .Zip(samples.Skip(1), (start, end) => GetDistance(start, end))
            .ToArray();

        (distances.Max() / distances.Min()).ShouldBeLessThan(1.5d);
    }

    private static double GetDistance(Point start, Point end)
    {
        return new Vector(end.X - start.X, end.Y - start.Y).Length;
    }
}
