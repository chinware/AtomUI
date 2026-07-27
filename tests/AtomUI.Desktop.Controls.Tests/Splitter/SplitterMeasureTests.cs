using AtomUI;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;
using AtomUISplitter = AtomUI.Desktop.Controls.Splitter;

namespace AtomUI.Desktop.Controls.Tests.Splitter;

public class SplitterMeasureTests
{
    static SplitterMeasureTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Splitter_Inside_Same_Orientation_StackPanel_Measures_With_Finite_DesiredSize()
    {
        var splitter = new AtomUISplitter
        {
            Orientation = Orientation.Vertical
        };

        splitter.Children.Add(new Border
        {
            Width     = 120,
            Height    = 80,
            Background = Brushes.Red
        });
        splitter.Children.Add(new Border
        {
            Width      = 160,
            Height     = 100,
            Background  = Brushes.Blue
        });

        var host = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Children =
            {
                splitter
            }
        };

        var window = new AvaloniaWindow
        {
            Width  = 480,
            Height = 320,
            Content = host
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            splitter.DesiredSize.Width.ShouldBeGreaterThan(0);
            splitter.DesiredSize.Height.ShouldBeGreaterThan(0);
            double.IsInfinity(splitter.DesiredSize.Width).ShouldBeFalse();
            double.IsInfinity(splitter.DesiredSize.Height).ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }
}
