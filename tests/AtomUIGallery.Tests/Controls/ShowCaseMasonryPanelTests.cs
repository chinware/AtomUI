using AtomUIGallery.Controls;
using Avalonia;
using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Controls;

public class ShowCaseMasonryPanelTests
{
    public ShowCaseMasonryPanelTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Masonry_Places_Next_Item_In_Shortest_Column()
    {
        var panel = new ShowCaseMasonryPanel
        {
            MinItemWidth = 100,
            MaxColumns   = 2,
            ColumnGap    = 10,
            RowGap       = 8
        };
        var first  = new FixedSizeControl(100, 100);
        var second = new FixedSizeControl(100, 40);
        var third  = new FixedSizeControl(100, 30);

        panel.Children.Add(first);
        panel.Children.Add(second);
        panel.Children.Add(third);

        MeasureAndArrange(panel, 210);

        first.Bounds.Position.ShouldBe(new Point(0, 0));
        second.Bounds.Position.ShouldBe(new Point(110, 0));
        third.Bounds.Position.ShouldBe(new Point(110, 48));
        panel.Bounds.Height.ShouldBe(100);
    }

    [Fact]
    public void Masonry_Full_Span_Item_Starts_After_Current_Tallest_Column()
    {
        var panel = new ShowCaseMasonryPanel
        {
            MinItemWidth = 100,
            MaxColumns   = 2,
            ColumnGap    = 10,
            RowGap       = 8
        };
        var first    = new FixedSizeControl(100, 100);
        var second   = new FixedSizeControl(100, 40);
        var fullSpan = new FixedSizeShowCaseItem(100, 50)
        {
            Span = ShowCaseItemSpan.Full
        };
        var last = new FixedSizeControl(100, 30);

        panel.Children.Add(first);
        panel.Children.Add(second);
        panel.Children.Add(fullSpan);
        panel.Children.Add(last);

        MeasureAndArrange(panel, 210);

        fullSpan.Bounds.Position.ShouldBe(new Point(0, 108));
        fullSpan.Bounds.Width.ShouldBe(210);
        last.Bounds.Position.ShouldBe(new Point(0, 166));
        panel.Bounds.Height.ShouldBe(196);
    }

    [Fact]
    public void Masonry_Column_Count_Respects_Min_Item_Width_And_Max_Columns()
    {
        var panel = new ShowCaseMasonryPanel
        {
            MinItemWidth = 120,
            MaxColumns   = 3,
            ColumnGap    = 10,
            RowGap       = 8
        };
        var first  = new FixedSizeControl(100, 20);
        var second = new FixedSizeControl(100, 20);
        var third  = new FixedSizeControl(100, 20);

        panel.Children.Add(first);
        panel.Children.Add(second);
        panel.Children.Add(third);

        MeasureAndArrange(panel, 500);

        first.Bounds.Width.ShouldBe(160);
        second.Bounds.X.ShouldBe(170);
        third.Bounds.X.ShouldBe(340);
    }

    private static void MeasureAndArrange(Control control, double width)
    {
        control.Measure(new Size(width, double.PositiveInfinity));
        control.Arrange(new Rect(0, 0, width, control.DesiredSize.Height));
    }

    private sealed class FixedSizeControl : Control
    {
        private readonly Size _size;

        public FixedSizeControl(double width, double height)
        {
            _size = new Size(width, height);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(Math.Min(_size.Width, availableSize.Width), _size.Height);
        }
    }

    private sealed class FixedSizeShowCaseItem : ShowCaseItem
    {
        private readonly Size _size;

        public FixedSizeShowCaseItem(double width, double height)
        {
            _size = new Size(width, height);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(Math.Min(_size.Width, availableSize.Width), _size.Height);
        }
    }
}
