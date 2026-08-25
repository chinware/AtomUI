using AtomUI.Controls;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Upload;

public class UploadImageLoadingTests
{
    public UploadImageLoadingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Picture_Preview_Content_Replaces_And_Clears_The_Shared_Image_Source()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var first = new Uri("https://example.test/first.png");
            var second = new Uri("https://example.test/second.png");

            AssertPreviewContent(
                new global::AtomUI.Desktop.Controls.UploadPicturePreviewContent(),
                first,
                second,
                content => content.ItemsSource);
            AssertPreviewContent(
                new global::AtomUI.Desktop.Controls.UploadPictureShapePreviewContent(),
                first,
                second,
                content => content.ItemsSource);
        });
    }

    private static void AssertPreviewContent<TContent>(
        TContent content,
        Uri first,
        Uri second,
        Func<TContent, IEnumerable<ImagePreviewItem>?> getItems)
        where TContent : AbstractUploadPictureContent
    {
        content.FilePath = first;
        var firstItem = getItems(content).ShouldNotBeNull().Single();
        firstItem.Source.Kind.ShouldBe(ImageLoadSourceKind.Http);
        firstItem.Source.DisplayName.ShouldBe("first.png");

        content.FilePath = second;
        var secondItem = getItems(content).ShouldNotBeNull().Single();
        secondItem.Source.DisplayName.ShouldBe("second.png");
        secondItem.ShouldNotBeSameAs(firstItem);

        content.FilePath = null;
        getItems(content).ShouldBeNull();
    }
}
