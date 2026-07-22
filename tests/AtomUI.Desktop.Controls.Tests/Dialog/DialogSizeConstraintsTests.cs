using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Dialog;

public class DialogSizeConstraintsTests
{
    [Theory]
    [InlineData(200, 80, 0, 0, double.PositiveInfinity, double.PositiveInfinity, 800, 600,
        200, 80, 800, 600)]
    [InlineData(200, 80, 320, 160, 700, 500, 800, 600,
        320, 160, 700, 500)]
    [InlineData(200, 80, 0, 0, 100, 40, 800, 600,
        200, 80, 200, 80)]
    [InlineData(200, 80, 400, 300, double.PositiveInfinity, double.PositiveInfinity, 250, 180,
        250, 180, 250, 180)]
    public void Resolve_Combines_Structural_Requested_And_Capacity(
        double structuralWidth,
        double structuralHeight,
        double requestedMinWidth,
        double requestedMinHeight,
        double requestedMaxWidth,
        double requestedMaxHeight,
        double capacityWidth,
        double capacityHeight,
        double expectedMinWidth,
        double expectedMinHeight,
        double expectedMaxWidth,
        double expectedMaxHeight)
    {
        var constraints = DialogSizeConstraints.Resolve(
            new Size(structuralWidth, structuralHeight),
            new Size(requestedMinWidth, requestedMinHeight),
            new Size(requestedMaxWidth, requestedMaxHeight),
            new Size(capacityWidth, capacityHeight));

        constraints.ShouldBe(new DialogSizeConstraints(
            expectedMinWidth,
            expectedMinHeight,
            expectedMaxWidth,
            expectedMaxHeight));
    }

    [Fact]
    public void Resolve_Normalizes_Invalid_Requests_Without_Bypassing_Capacity()
    {
        var constraints = DialogSizeConstraints.Resolve(
            new Size(200, 80),
            new Size(double.NaN, -20),
            new Size(double.NaN, double.NegativeInfinity),
            new Size(320, 180));

        constraints.ShouldBe(new DialogSizeConstraints(200, 80, 320, 180));
    }

    [Fact]
    public void Clamp_Constrains_Each_Surface_Axis_Independently()
    {
        var constraints = new DialogSizeConstraints(200, 100, 600, 400);

        constraints.Clamp(new Size(120, 480)).ShouldBe(new Size(200, 400));
    }
}
