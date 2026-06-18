using AtomUI.Controls;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Grid;

public class GridResponsiveTests
{
    static GridResponsiveTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Col_Exposes_Xxxl_Breakpoint_Property()
    {
        var col = new Col
        {
            Xxxl = new GridColSize { Span = 6 }
        };

        col.Xxxl!.Span.ShouldBe(6);
    }

    [Fact]
    public void Row_Gutter_Uses_Mobile_First_Cascade_For_Partial_Map()
    {
        var host = new TestMediaBreakHost(MediaBreakPoint.Large)
        {
            Width = 240,
            Height = 100
        };
        var row = new Row
        {
            Gutter = GridGutter.Parse("xs: 8, md: 16")
        };
        row.Children.Add(new Col { Span = 12, Content = new Border { Height = 10, Background = Brushes.White } });
        row.Children.Add(new Col { Span = 12, Content = new Border { Height = 10, Background = Brushes.White } });
        host.Children.Add(row);

        var window = new AvaloniaWindow { Width = 240, Height = 100, Content = host };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            row.Children[0].Bounds.X.ShouldBe(8, 0.5);
            row.Children[1].Bounds.X.ShouldBe(128, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void GridColSpanInfo_Uses_Mobile_First_Cascade_For_Partial_Map()
    {
        var span = GridColSpanInfo.Parse("xs: 24, md: 12");

        span.GetValue(MediaBreakPoint.Large).ShouldBe(12);
    }

    private sealed class TestMediaBreakHost : Panel, IMediaBreakAwareControl
    {
        public TestMediaBreakHost(MediaBreakPoint breakPoint)
        {
            MediaBreakPoint = breakPoint;
        }

        public MediaBreakPoint MediaBreakPoint { get; private set; }

        public event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;

        public void SetMediaBreakPoint(MediaBreakPoint mediaBreakPoint)
        {
            MediaBreakPoint = mediaBreakPoint;
            MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(mediaBreakPoint));
        }
    }
}
