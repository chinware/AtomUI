using AtomUI.Desktop.Controls;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImagePreviewerSourceLoadingTests
{
    public ImagePreviewerSourceLoadingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void SourceUris_Load_Local_Image_Items()
    {
        var path = CreatePngFile();
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            SourceUris = [ImageSourceUri.Parse(path)]
        };

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems.Count.ShouldBe(1);
        previewer.EffectiveItems[0].LoadedSource.ShouldNotBeNull();
    }

    [Fact]
    public void SourceUri_Uses_FallbackSourceUri_When_Load_Fails()
    {
        var fallbackPath = CreatePngFile();
        var missingPath  = Path.Combine(Path.GetTempPath(), $"atomui-missing-{Guid.NewGuid():N}.png");
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            SourceUri          = ImageSourceUri.Parse(missingPath),
            FallbackSourceUri  = ImageSourceUri.Parse(fallbackPath)
        };

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems[0].SourceUri.CacheKey.ShouldBe(Path.GetFullPath(fallbackPath));
    }

    [Fact]
    public void FallbackSourceUri_Change_Does_Not_Reload_Primary_Source()
    {
        var sourcePath   = CreatePngFile();
        var fallbackPath = CreatePngFile();
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            SourceUri = ImageSourceUri.Parse(sourcePath)
        };

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        var effectiveItems = previewer.EffectiveItems;
        var primaryItem    = effectiveItems![0];
        var loadedSource   = primaryItem.LoadedSource;

        previewer.FallbackSourceUri = ImageSourceUri.Parse(fallbackPath);
        Dispatcher.UIThread.RunJobs();

        previewer.EffectiveItems.ShouldBeSameAs(effectiveItems);
        previewer.EffectiveItems![0].ShouldBeSameAs(primaryItem);
        previewer.EffectiveItems[0].LoadedSource.ShouldBeSameAs(loadedSource);
        previewer.EffectiveItems[0].SourceUri.CacheKey.ShouldBe(Path.GetFullPath(sourcePath));
    }

    [Fact]
    public void FallbackSourceUri_Loads_When_No_Main_Source()
    {
        var fallbackPath = CreatePngFile();
        var previewer    = new global::AtomUI.Desktop.Controls.ImagePreviewer();

        previewer.FallbackSourceUri = ImageSourceUri.Parse(fallbackPath);

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems[0].SourceUri.CacheKey.ShouldBe(Path.GetFullPath(fallbackPath));
    }

    [Fact]
    public void FallbackSourceUri_Change_Retries_Failed_Cover_Fallback()
    {
        var missingCoverPath = Path.Combine(Path.GetTempPath(), $"atomui-missing-cover-{Guid.NewGuid():N}.png");
        var fallbackPath     = CreatePngFile();
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            CoverSourceUri = ImageSourceUri.Parse(missingCoverPath)
        };

        WaitUntil(() => previewer.IsCoverImageFailed)
            .ShouldBeTrue($"cover failed: loading={previewer.IsCoverImageLoading}");

        previewer.FallbackSourceUri = ImageSourceUri.Parse(fallbackPath);

        WaitUntil(() => previewer.EffectiveCoverImage is not null && !previewer.IsCoverImageFailed)
            .ShouldBeTrue($"cover loading={previewer.IsCoverImageLoading}, failed={previewer.IsCoverImageFailed}");

        previewer.EffectiveCoverImage.ShouldNotBeNull();
    }

    [Fact]
    public void OpenDialog_With_No_Source_Does_Not_Open()
    {
        var previewer = new ImageGroupPreviewer();

        Should.NotThrow(() => previewer.OpenDialog());

        previewer.IsOpen.ShouldBeFalse();
        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems.Count.ShouldBe(0);
    }

    private static bool WaitUntil(Func<bool> predicate)
    {
        for (var i = 0; i < 250; i++)
        {
            if (predicate())
            {
                return true;
            }

            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(20);
        }

        return predicate();
    }

    private static string DescribeItems(IList<ImagePreviewItem>? items)
    {
        if (items is null)
        {
            return "items: null";
        }

        return string.Join(", ", items.Select(item =>
            $"{item.SourceUri.OriginalString}:{item.State}:{item.Error?.GetType().Name}:{item.Error?.Message}"));
    }

    private static string CreatePngFile()
    {
        const string base64 =
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/p9sAAAAASUVORK5CYII=";
        var directory = Path.Combine(Path.GetTempPath(), $"atomui-image-previewer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "sample.png");
        File.WriteAllBytes(path, Convert.FromBase64String(base64));
        return path;
    }
}
