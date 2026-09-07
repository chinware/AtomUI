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

    [Fact]
    public void Arrange_Keeps_Committed_Item_Columns_When_Width_Changes()
    {
        var narrowFirst = CreateColumnFlipPanel(out _, out _, out var narrowThird);
        MeasureAndArrange(narrowFirst, 210);
        narrowThird.Bounds.X.ShouldBe(110, 0.01);

        MeasureAndArrange(narrowFirst, 310);
        narrowThird.Bounds.X.ShouldBe(160, 0.01,
            "the item must remain in the right column when the effective column count stays at two");

        var wideFirst = CreateColumnFlipPanel(out _, out _, out var wideThird);
        MeasureAndArrange(wideFirst, 310);
        wideThird.Bounds.X.ShouldBe(0, 0.01);

        MeasureAndArrange(wideFirst, 210);
        wideThird.Bounds.X.ShouldBe(0, 0.01,
            "stable assignments must work in both resize directions");
    }

    [Fact]
    public void Arrange_Commits_Final_Width_Assignments_When_Measure_Width_Differs()
    {
        var panel = CreateColumnFlipPanel(out _, out _, out var third);

        panel.MeasureForTest(new Size(310, double.PositiveInfinity));
        panel.ArrangeForTest(new Size(210, 1000));
        third.Bounds.X.ShouldBe(110, 0.01);

        MeasureAndArrange(panel, 310);
        third.Bounds.X.ShouldBe(160, 0.01,
            "the first committed assignment must come from the final Arrange width, not the speculative Measure width");
    }

    [Fact]
    public void Stable_Assignments_Track_Existing_Children_By_Control_Instance()
    {
        var panel = new TestShowCaseMasonryPanel
        {
            MinItemWidth = 100,
            MaxColumns   = 2,
            ColumnGap    = 0,
            RowGap       = 0
        };
        var first  = new VariableHeightControl(100);
        var second = new VariableHeightControl(200);
        var third  = new VariableHeightControl(20);
        panel.Children.Add(first);
        panel.Children.Add(second);
        panel.Children.Add(third);

        MeasureAndArrange(panel, 200);
        new[] { first.Bounds.X, second.Bounds.X, third.Bounds.X }
            .ShouldBe(new[] { 0d, 100d, 0d });

        panel.Children.Insert(0, new VariableHeightControl(10));
        MeasureAndArrange(panel, 200);

        new[] { first.Bounds.X, second.Bounds.X, third.Bounds.X }
            .ShouldBe(new[] { 0d, 100d, 0d },
                "inserting a child must not reassign existing controls by their new indexes");
    }

    [Fact]
    public void Stable_Assignments_Are_Rebuilt_When_Column_Count_Changes()
    {
        var panel = new TestShowCaseMasonryPanel
        {
            MinItemWidth = 100,
            MaxColumns   = 3,
            ColumnGap    = 0,
            RowGap       = 0
        };
        var first  = new VariableHeightControl(100);
        var second = new VariableHeightControl(100);
        var third  = new VariableHeightControl(100);
        var fourth = new VariableHeightControl(100);
        panel.Children.Add(first);
        panel.Children.Add(second);
        panel.Children.Add(third);
        panel.Children.Add(fourth);

        MeasureAndArrange(panel, 200);
        panel.Children.Select(child => child.Bounds.X).ShouldBe(new[] { 0d, 100d, 0d, 100d });

        MeasureAndArrange(panel, 300);
        panel.Children.Select(child => child.Bounds.X).ShouldBe(new[] { 0d, 100d, 200d, 0d });

        first.MeasuredHeight = 200;
        second.MeasuredHeight = 10;
        third.MeasuredHeight = 100;
        fourth.MeasuredHeight = 100;
        first.InvalidateMeasure();
        second.InvalidateMeasure();
        third.InvalidateMeasure();
        fourth.InvalidateMeasure();

        MeasureAndArrange(panel, 300);
        panel.Children.Select(child => child.Bounds.X).ShouldBe(new[] { 0d, 100d, 200d, 0d },
            "the rebuilt three-column mapping must be committed after Arrange; without that commit, the changed heights would move the fourth child to x=100");
    }

    private static TestShowCaseMasonryPanel CreateColumnFlipPanel(
        out Control first,
        out Control second,
        out Control third)
    {
        var panel = new TestShowCaseMasonryPanel
        {
            MinItemWidth = 100,
            MaxColumns   = 2,
            ColumnGap    = 10,
            RowGap       = 0
        };
        first  = new WidthBreakpointHeightControl(100, 20);
        second = new WidthBreakpointHeightControl(40, 100);
        third  = new WidthBreakpointHeightControl(30, 30);
        panel.Children.Add(first);
        panel.Children.Add(second);
        panel.Children.Add(third);
        return panel;
    }

    private static void MeasureAndArrange(TestShowCaseMasonryPanel panel, double width)
    {
        panel.MeasureForTest(new Size(width, double.PositiveInfinity));
        panel.ArrangeForTest(new Size(width, 1000));
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

    private sealed class WidthBreakpointHeightControl : Control
    {
        private readonly double _narrowHeight;
        private readonly double _wideHeight;

        public WidthBreakpointHeightControl(double narrowHeight, double wideHeight)
        {
            _narrowHeight = narrowHeight;
            _wideHeight   = wideHeight;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var height = availableSize.Width < 125 ? _narrowHeight : _wideHeight;
            return new Size(availableSize.Width, height);
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
