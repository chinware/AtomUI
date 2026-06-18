using AtomUI.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Shared.Tests.MediaQuery;

public class ResponsiveValueTests
{
    [Fact]
    public void ResponsiveInt_Uses_Mobile_First_Cascade_For_Partial_Map()
    {
        var value = ResponsiveInt.Parse("xs: 1, md: 3");

        value.Resolve(MediaBreakPoint.ExtraSmall, 9).ShouldBe(1);
        value.Resolve(MediaBreakPoint.Small, 9).ShouldBe(1);
        value.Resolve(MediaBreakPoint.Medium, 9).ShouldBe(3);
        value.Resolve(MediaBreakPoint.Large, 9).ShouldBe(3);
        value.Resolve(MediaBreakPoint.ExtraExtraLarge, 9).ShouldBe(3);
    }

    [Fact]
    public void ResponsiveInt_Returns_Fallback_When_No_Configured_Breakpoint_Is_Active()
    {
        var value = ResponsiveInt.Parse("xl: 4");

        value.Resolve(MediaBreakPoint.Small, 2).ShouldBe(2);
    }

    [Fact]
    public void ResponsiveInt_TryResolve_Distinguishes_Configured_Hit_From_Fallback()
    {
        var value = ResponsiveInt.Parse("xl: 4");

        value.TryResolve(MediaBreakPoint.Large, out _).ShouldBeFalse();
        value.TryResolve(MediaBreakPoint.ExtraLarge, out var resolved).ShouldBeTrue();
        resolved.ShouldBe(4);
    }

    [Fact]
    public void ResponsiveInt_Scalar_TryResolve_Applies_To_All_Breakpoints()
    {
        var value = new ResponsiveInt(3);

        value.TryResolve(MediaBreakPoint.ExtraSmall, out var resolved).ShouldBeTrue();
        resolved.ShouldBe(3);
    }

    [Fact]
    public void ResponsiveInt_Supports_Xxxl_Breakpoint()
    {
        var value = ResponsiveInt.Parse("xs: 1, xxl: 3, xxxl: 4");

        value.Resolve(MediaBreakPoint.ExtraExtraExtraLarge, 9).ShouldBe(4);
    }

    [Fact]
    public void ResponsiveInt_Scalar_Value_Applies_To_All_Breakpoints()
    {
        var value = ResponsiveInt.Parse("3");

        value.Resolve(MediaBreakPoint.ExtraSmall, 1).ShouldBe(3);
        value.Resolve(MediaBreakPoint.ExtraExtraExtraLarge, 1).ShouldBe(3);
    }

    [Fact]
    public void ResponsiveInt_Rejects_Duplicate_Breakpoint_Key()
    {
        Should.Throw<FormatException>(() => ResponsiveInt.Parse("xs: 1, xs: 2"));
    }

    [Fact]
    public void ResponsiveDouble_Uses_Mobile_First_Cascade_For_Partial_Map()
    {
        var value = ResponsiveDouble.Parse("xs: 8, md: 16");

        value.Resolve(MediaBreakPoint.Small, 0).ShouldBe(8);
        value.Resolve(MediaBreakPoint.Large, 0).ShouldBe(16);
    }

    [Fact]
    public void ResponsiveGutter_Resolves_Horizontal_And_Vertical_Independently()
    {
        var gutter = ResponsiveGutter.Parse("xs: 8, md: 16; xs: 4, xl: 24");

        gutter.Resolve(MediaBreakPoint.Large, (0, 0)).ShouldBe((16, 4));
        gutter.Resolve(MediaBreakPoint.ExtraLarge, (0, 0)).ShouldBe((16, 24));
    }

    [Fact]
    public void ResponsiveGutter_Uses_Fallback_For_Implicit_Vertical_Value()
    {
        var gutter = ResponsiveGutter.Parse("xs: 8, md: 16");

        gutter.Resolve(MediaBreakPoint.Large, (0, 12)).ShouldBe((16, 12));
    }
}
