using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImageViewerVisualStateTests
{
    public ImageViewerVisualStateTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Sets_HasImage_And_Loading_PseudoClasses_From_DisplayState()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var viewer = new ImageViewer();
            viewer.Classes.Contains(":has-image").ShouldBeFalse();
            viewer.Classes.Contains(":loading").ShouldBeFalse();

            viewer.CurrentImage = new TestImage();
            viewer.Classes.Contains(":has-image").ShouldBeTrue();

            viewer.IsCurrentImageLoading = true;
            viewer.Classes.Contains(":loading").ShouldBeTrue();

            viewer.CurrentImage = null;
            viewer.Classes.Contains(":has-image").ShouldBeFalse();
        });
    }

    [Fact]
    public void Cover_Mask_Is_Stable_Across_Load_States()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var cover = new ImagePreviewerCover { IsShowCoverMask = true };

            // mask 是悬停操作层（点击打开预览在任何加载态都有效），
            // 只由 IsShowCoverMask 决定，与加载/图片/失败状态完全解耦，
            // 两种 ImageSwitchMode 下都必须稳定
            cover.ImageSource = null;
            cover.IsLoading   = true;
            cover.IsCoverMaskVisible.ShouldBeTrue();

            cover.ImageSource = new TestImage();
            cover.IsLoading   = false;
            cover.IsCoverMaskVisible.ShouldBeTrue();

            cover.IsFailed    = true;
            cover.ImageSource = null;
            cover.IsCoverMaskVisible.ShouldBeTrue();

            cover.IsFailed     = false;
            cover.IsShowCoverMask = false;
            cover.IsCoverMaskVisible.ShouldBeFalse();
        });
    }

    private sealed class TestImage : IImage
    {
        public Size Size => new(24, 24);

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
        }
    }
}
