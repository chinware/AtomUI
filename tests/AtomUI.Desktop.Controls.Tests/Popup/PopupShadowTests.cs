using System.Reflection;
using Avalonia.Controls;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Popups;

public class PopupShadowTests
{
    static PopupShadowTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void ShadowsAwareContainer_Shadow_Renderer_Does_Not_Clip_BoxShadow()
    {
        var container = new ShadowsAwareContainer
        {
            BoxShadow = BoxShadows.Parse("0 8 24 0 #66000000"),
            Child     = new Border()
        };

        var shadowRenderer = GetShadowRenderer(container);

        shadowRenderer.ShouldNotBeNull();
        shadowRenderer!.ClipToBounds.ShouldBeFalse();
    }

    [Fact]
    public void ShadowsAwareContainer_Shadow_Renderer_Does_Not_Draw_Own_Surface()
    {
        var container = new ShadowsAwareContainer
        {
            BoxShadow = BoxShadows.Parse("0 8 24 0 #66000000"),
            Child     = new Border()
        };

        var shadowRenderer = GetShadowRenderer(container);

        shadowRenderer.ShouldNotBeNull();
        (shadowRenderer is Border).ShouldBeFalse();
        shadowRenderer!.GetType()
                       .GetProperty("Background", BindingFlags.Instance | BindingFlags.Public)
                       .ShouldBeNull();
    }

    private static Control? GetShadowRenderer(ShadowsAwareContainer container)
    {
        var field = typeof(ShadowsAwareContainer).GetField(
            "_shadowsRenderer",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        return (Control?)field!.GetValue(container);
    }
}
