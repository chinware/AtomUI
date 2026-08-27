using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia;
using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Tests.Controls;

public class ShowCaseMasonryPanelTests
{
    public ShowCaseMasonryPanelTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Arrange_Reuses_Measured_Layout_When_Width_Is_Unchanged()
    {
        var panel = new TestShowCaseMasonryPanel
        {
            MinItemWidth = 1,
            MaxColumns = 1,
            ColumnGap = 0,
            RowGap = 0
        };
        var child = new VariableHeightControl(40);
        panel.Children.Add(child);

        panel.MeasureForTest(new Size(100, double.PositiveInfinity));
        child.MeasuredHeight = 80;
        child.InvalidateMeasure();
        child.Measure(new Size(100, double.PositiveInfinity));
        panel.ArrangeForTest(new Size(100, 100));

        child.Bounds.Height.ShouldBe(40,
            "Arrange must reuse the layout produced by Measure when the effective width is unchanged");
    }

    [Fact]
    public void Arrange_Recalculates_Layout_When_Width_Changes()
    {
        var panel = new TestShowCaseMasonryPanel
        {
            MinItemWidth = 1,
            MaxColumns = 1,
            ColumnGap = 0,
            RowGap = 0
        };
        var child = new VariableHeightControl(40);
        panel.Children.Add(child);

        panel.MeasureForTest(new Size(100, double.PositiveInfinity));
        child.MeasuredHeight = 80;
        child.InvalidateMeasure();
        child.Measure(new Size(200, double.PositiveInfinity));
        panel.ArrangeForTest(new Size(200, 100));

        child.Bounds.Height.ShouldBe(80,
            "a changed effective width must invalidate the cached Masonry layout");
    }

    [Fact]
    public void Arrange_ReMeasures_Children_When_Unbounded_Measure_Width_Differs_From_Final_Width()
    {
        var panel = new TestShowCaseMasonryPanel
        {
            MinItemWidth = 1,
            MaxColumns = 2,
            ColumnGap = 0,
            RowGap = 0
        };
        var first = new WidthDependentHeightControl();
        var second = new WidthDependentHeightControl();
        panel.Children.Add(first);
        panel.Children.Add(second);

        panel.MeasureForTest(new Size(double.PositiveInfinity, double.PositiveInfinity));
        panel.ArrangeForTest(new Size(400, 400));

        first.Bounds.Width.ShouldBe(200, 0.01);
        first.Bounds.Height.ShouldBe(100, 0.01,
            "a child measured with an unbounded width must be re-measured for the final Masonry column width");
        second.Bounds.Height.ShouldBe(100, 0.01);
    }

    private sealed class VariableHeightControl : Control
    {
        public VariableHeightControl(double height)
        {
            MeasuredHeight = height;
        }

        public double MeasuredHeight { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(Math.Min(availableSize.Width, 100), MeasuredHeight);
        }
    }

    private sealed class WidthDependentHeightControl : Control
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(availableSize.Width, availableSize.Width / 2);
        }
    }

    private sealed class TestShowCaseMasonryPanel : ShowCaseMasonryPanel
    {
        public Size MeasureForTest(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        public Size ArrangeForTest(Size finalSize)
        {
            return base.ArrangeOverride(finalSize);
        }
    }
}
