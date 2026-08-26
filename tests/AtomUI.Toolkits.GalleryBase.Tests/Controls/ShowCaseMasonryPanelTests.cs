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
