using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImagePreviewItemTests
{
    [Fact]
    public void CompleteLoading_Ignores_Stale_Result_And_Disposes_It()
    {
        var item          = new ImagePreviewItem(new UriImagePreviewSource("a.png"));
        var firstVersion  = item.BeginLoading();
        var secondVersion = item.BeginLoading();
        using var stale   = LoadedImageSource.CreateSvg("<svg />", new Avalonia.Size(1, 1));

        item.CompleteLoading(firstVersion, stale);

        item.State.ShouldBe(ImagePreviewItemState.Loading);
        item.LoadedSource.ShouldBeNull();

        var current = LoadedImageSource.CreateSvg("<svg />", new Avalonia.Size(2, 2));
        item.CompleteLoading(secondVersion, current);

        item.State.ShouldBe(ImagePreviewItemState.Loaded);
        item.LoadedSource.ShouldBeSameAs(current);
    }

    [Fact]
    public void FailLoading_Ignores_Stale_Failure()
    {
        var item          = new ImagePreviewItem(new UriImagePreviewSource("a.png"));
        var firstVersion  = item.BeginLoading();
        var secondVersion = item.BeginLoading();

        item.FailLoading(firstVersion, new IOException("old"));

        item.State.ShouldBe(ImagePreviewItemState.Loading);
        item.Error.ShouldBeNull();

        item.FailLoading(secondVersion, new IOException("current"));

        item.State.ShouldBe(ImagePreviewItemState.Failed);
        item.Error.ShouldNotBeNull();
    }
}
