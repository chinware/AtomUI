using AtomUI.Media;
using Avalonia.Media;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AtomUI.Tests.Utils;

public class ColorUtilsTest
{
    private readonly ITestOutputHelper _output;

    public ColorUtilsTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TestColorGenerate()
    {
        _output.WriteLine(Color.Parse("red").HexName());
        _output.WriteLine(ColorUtils.Brighten("#ffff0000").HexName());
        ColorUtils.Lighten("#f00").ShouldBeEquivalentTo(Color.Parse("#ff3333"));
        ColorUtils.Lighten("#f00", 100).ShouldBeEquivalentTo(Color.Parse("#ffffff"));

        //ColorUtils.Brighten("#f00").ShouldBeEquivalentTo(Color.Parse("#ff1a1a"));
    }

    [Fact]
    public void TestDesaturation()
    {
        var items = ColorUtilsTestData.DESATURATIONS;
        for (var i = 0; i < items.Count; ++i)
        {
            // output.WriteLine($"{i} - {ColorUtils.Desaturate(Color.Parse("red"), i).HexName()} - {items[i]}");
            ColorUtils.Desaturate("red", i).HexName().ShouldBeEquivalentTo(items[i]);
        }
    }

    [Fact]
    public void TestSaturation()
    {
        var items = ColorUtilsTestData.SATURATIONS;
        for (var i = 0; i < items.Count; ++i)
        {
            // output.WriteLine($"{i} - {ColorUtils.Saturate(Color.Parse("red"), i).HexName()} - {items[i]}");
            ColorUtils.Saturate("red", i).HexName().ShouldBeEquivalentTo(items[i]);
        }
    }

    [Fact]
    public void TestLighten()
    {
        var items = ColorUtilsTestData.LIGHTENS;
        for (var i = 0; i < items.Count; ++i)
        {
            //output.WriteLine($"{i} - {ColorUtils.Lighten(Color.Parse("red"), i).HexName()} - {items[i]}"); 
            ColorUtils.Lighten("red", i).HexName().ShouldBeEquivalentTo(items[i]);
        }
    }

    [Fact]
    public void TestBrighten()
    {
        var items = ColorUtilsTestData.BRIGHTENS;
        for (var i = 0; i < items.Count; ++i)
        {
            //output.WriteLine($"{i} - {ColorUtils.Brighten(Color.Parse("red"), i).HexName()} - {items[i]}"); 
            ColorUtils.Brighten("red", i).HexName().ShouldBeEquivalentTo(items[i]);
        }
    }

    [Fact]
    public void TestDarken()
    {
        var items = ColorUtilsTestData.DARKENS;
        for (var i = 0; i < items.Count; ++i)
        {
            // output.WriteLine($"{i} - {ColorUtils.Darken(Color.Parse("red"), i).HexName()} - {items[i]}"); 
            ColorUtils.Darken("red", i).HexName().ShouldBeEquivalentTo(items[i]);
        }
    }

    [Fact]
    public void TestSpin()
    {
        var testColor = Color.Parse("#f00");
        //output.WriteLine(ColorUtils.Spin(Color.Parse("#f00"), -1234).HexName());
        Math.Round(testColor.Spin(-1234).ToHsl().H).ShouldBe(206);
        Math.Round(testColor.Spin(-360).ToHsl().H).ShouldBe(0);
        Math.Round(testColor.Spin(-120).ToHsl().H).ShouldBe(240);
        Math.Round(testColor.Spin(0).ToHsl().H).ShouldBe(0);
        Math.Round(testColor.Spin().ToHsl().H).ShouldBe(10);
        Math.Round(testColor.Spin(360).ToHsl().H).ShouldBe(0);
        Math.Round(testColor.Spin(2345).ToHsl().H).ShouldBe(185);
    }

    [Fact]
    public void TestOnBackground()
    {
        {
            var targetColor = ColorUtils.OnBackground(Color.Parse("#ffffff"), Color.Parse("#000"));
            targetColor.HexName().ShouldBe("#ffffffff");
            // output.WriteLine(targetColor.HexName());
        }
        {
            var targetColor = ColorUtils.OnBackground(Color.Parse("#ffffff00"), Color.Parse("#000"));
            targetColor.HexName().ShouldBe("#ffffff00");
            // output.WriteLine(targetColor.HexName());
        }
        {
            var targetColor = ColorUtils.OnBackground(Color.Parse("#ffffff77"), Color.Parse("#000"));
            targetColor.HexName().ShouldBe("#ffffff77");
            // output.WriteLine(targetColor.HexName());
        }
        {
            var targetColor = ColorUtils.OnBackground(Color.Parse("#262a6d82"), Color.Parse("#644242"));
            targetColor.HexName().ShouldBe("#ff5b484c");
            // output.WriteLine(targetColor.HexName());
        }
    }
}