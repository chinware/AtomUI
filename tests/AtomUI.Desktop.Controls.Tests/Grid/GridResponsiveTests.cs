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

    [Fact]
    public void Col_Span_Zero_Is_Hidden_From_Row_Layout()
    {
        var row = new Row
        {
            Width  = 240,
            Height = 40
        };
        var hidden = new Col
        {
            Span    = 0,
            Content = new Border { Width = 80, Height = 10, Background = Brushes.White }
        };
        var visible = new Col
        {
            Span    = 24,
            Content = new Border { Height = 10, Background = Brushes.White }
        };
        row.Children.Add(hidden);
        row.Children.Add(visible);

        var window = new AvaloniaWindow { Width = 240, Height = 80, Content = row };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            hidden.Bounds.Width.ShouldBe(0);
            hidden.Bounds.Height.ShouldBe(0);
            visible.Bounds.X.ShouldBe(0, 0.5);
            visible.Bounds.Width.ShouldBe(240, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Col_Flex_Fills_Remaining_Row_Width()
    {
        var row = new Row
        {
            Width  = 240,
            Height = 40
        };
        var fixedCol = new Col
        {
            Span    = 8,
            Content = new Border { Height = 10, Background = Brushes.White }
        };
        var flexCol = new Col
        {
            Flex    = 1,
            Content = new Border { Height = 10, Background = Brushes.White }
        };
        row.Children.Add(fixedCol);
        row.Children.Add(flexCol);

        var window = new AvaloniaWindow { Width = 240, Height = 80, Content = row };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            fixedCol.Bounds.Width.ShouldBe(80, 0.5);
            flexCol.Bounds.X.ShouldBe(80, 0.5);
            flexCol.Bounds.Width.ShouldBe(160, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Col_Responsive_Flex_Overrides_Base_Span_At_Active_Breakpoint()
    {
        var host = new TestMediaBreakHost(MediaBreakPoint.Large)
        {
            Width  = 240,
            Height = 80
        };
        var row = new Row();
        var fixedCol = new Col
        {
            Span    = 8,
            Content = new Border { Height = 10, Background = Brushes.White }
        };
        var responsiveFlexCol = new Col
        {
            Span    = 8,
            Lg      = new GridColSize { Flex = 1 },
            Content = new Border { Height = 10, Background = Brushes.White }
        };
        row.Children.Add(fixedCol);
        row.Children.Add(responsiveFlexCol);
        host.Children.Add(row);

        var window = new AvaloniaWindow { Width = 240, Height = 80, Content = host };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            fixedCol.Bounds.Width.ShouldBe(80, 0.5);
            responsiveFlexCol.Bounds.X.ShouldBe(80, 0.5);
            responsiveFlexCol.Bounds.Width.ShouldBe(160, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void ColInfoExtension_Creates_Flex_Override()
    {
        var extension = new ColInfoExtension
        {
            Flex = 2
        };

        var size = extension.ProvideValue(null!) as GridColSize;

        size.ShouldNotBeNull();
        size!.Flex.ShouldBe(new GridColFlex(2));
    }

    [Fact]
    public void Row_Responsive_Justify_Uses_Active_Breakpoint_Value()
    {
        var host = new TestMediaBreakHost(MediaBreakPoint.Large)
        {
            Width  = 240,
            Height = 80
        };
        var row = new Row
        {
            JustifyInfo = GridRowJustifyInfo.Parse("xs: start, md: end")
        };
        var col = new Col
        {
            Span    = 6,
            Content = new Border { Height = 10, Background = Brushes.White }
        };
        row.Children.Add(col);
        host.Children.Add(row);

        var window = new AvaloniaWindow { Width = 240, Height = 80, Content = host };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            col.Bounds.X.ShouldBe(180, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Row_Responsive_Align_Uses_Active_Breakpoint_Value()
    {
        var host = new TestMediaBreakHost(MediaBreakPoint.Large)
        {
            Width  = 240,
            Height = 80
        };
        var row = new Row
        {
            AlignInfo = GridRowAlignInfo.Parse("xs: top, md: bottom")
        };
        var tall = new Col
        {
            Span    = 6,
            Content = new Border { Height = 30, Background = Brushes.White }
        };
        var shortCol = new Col
        {
            Span    = 6,
            Content = new Border { Height = 10, Background = Brushes.White }
        };
        row.Children.Add(tall);
        row.Children.Add(shortCol);
        host.Children.Add(row);

        var window = new AvaloniaWindow { Width = 240, Height = 80, Content = host };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            tall.Bounds.Y.ShouldBe(0, 0.5);
            shortCol.Bounds.Y.ShouldBe(20, 0.5);
        }
        finally
        {
            window.Close();
        }
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
