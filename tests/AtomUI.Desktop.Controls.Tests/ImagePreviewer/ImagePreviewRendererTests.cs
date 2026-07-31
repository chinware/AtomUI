using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImagePreviewRendererTests
{
    public ImagePreviewRendererTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Stretch_Clr_Wrapper_Uses_Renderer_StretchProperty()
    {
        var renderer = new ImagePreviewRenderer();

        renderer.Stretch = Stretch.Fill;

        renderer.GetValue(ImagePreviewRenderer.StretchProperty).ShouldBe(Stretch.Fill);
    }
}
