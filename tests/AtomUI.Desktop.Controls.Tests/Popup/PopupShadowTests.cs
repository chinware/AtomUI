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

        var shadowRenderer = GetFrameRenderer(container);

        shadowRenderer.ShouldNotBeNull();
        shadowRenderer!.ClipToBounds.ShouldBeFalse();
    }

    [Fact]
    public void Popup_Exposes_SurfaceBackground_As_A_Public_Styled_Property()
    {
        Popup.SurfaceBackgroundProperty.OwnerType.ShouldBe(typeof(Popup));
        typeof(Popup).GetProperty(nameof(Popup.SurfaceBackground)).ShouldNotBeNull();
    }

    [Fact]
    public void ShadowsAwareContainer_Frame_Renderer_Draws_Configured_Surface()
    {
        var container = new ShadowsAwareContainer
        {
            BoxShadow         = BoxShadows.Parse("0 8 24 0 #66000000"),
            SurfaceBackground = Brushes.White,
            Child             = new Border()
        };

        var frameRenderer = GetFrameRenderer(container);

        frameRenderer.ShouldNotBeNull();
        frameRenderer!.GetType()
                      .GetProperty("SurfaceBackground", BindingFlags.Instance | BindingFlags.Public)
                      .ShouldNotBeNull()!
                      .GetValue(frameRenderer)
                      .ShouldBe(Brushes.White);
    }

    [Fact]
    public void ShadowsAwareContainer_Null_Surface_Preserves_Transparent_Frame_Fill()
    {
        var container = new ShadowsAwareContainer
        {
            BoxShadow         = BoxShadows.Parse("0 8 24 0 #66000000"),
            SurfaceBackground = null,
            Child             = new Border()
        };

        var frameRenderer = GetFrameRenderer(container);

        frameRenderer.ShouldNotBeNull();
        frameRenderer!.GetType()
                      .GetProperty("SurfaceBackground", BindingFlags.Instance | BindingFlags.Public)
                      .ShouldNotBeNull()!
                      .GetValue(frameRenderer)
                      .ShouldBeNull();
    }

    private static Control? GetFrameRenderer(ShadowsAwareContainer container)
    {
        var field = typeof(ShadowsAwareContainer).GetField(
            "_frameRenderer",
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        return (Control?)field!.GetValue(container);
    }
}
