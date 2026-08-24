using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Tests.ImageLoading;

public class AsyncImageTests
{
    static AsyncImageTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void No_Source_Is_Idle_And_Explicit_Zero_Size_Is_A_Typed_Failure()
    {
        var borrowed = new TestBorrowedImage();
        var image = new AsyncImage
        {
            Width = 32,
            Height = 32,
            DecodeMode = ImageDecodeMode.Explicit,
            Source = ImageLoadSource.FromImage(borrowed)
        };
        using var host = new ImageControlTestHost(image);

        ImageControlTestHost.WaitUntil(() => image.IsFailed, "invalid explicit decode state");
        image.LoadError.ShouldNotBeNull().Code.ShouldBe(ImageLoadErrorCode.InvalidSource);
        image.Classes.ShouldContain(":failed");

        image.DecodePixelWidth = 16;
        ImageControlTestHost.WaitUntil(() => image.IsLoaded, "explicit image load");
        image.LoadedImage.ShouldBeSameAs(borrowed);

        image.Source = null;
        image.LoadState.ShouldBe(ImageLoadState.Idle);
        image.LoadError.ShouldBeNull();
        image.LoadedImage.ShouldBeNull();
    }

    [Fact]
    public void Borrowed_Image_Transitions_To_Loaded_And_Is_Not_Disposed()
    {
        var borrowed = new TestBorrowedImage();
        ImageOpenedEventArgs? opened = null;
        var image = new AsyncImage
        {
            Width = 32,
            Height = 32,
            Source = ImageLoadSource.FromImage(borrowed)
        };
        image.ImageOpened += (_, args) => opened = args;

        using (var host = new ImageControlTestHost(image))
        {
            ImageControlTestHost.WaitUntil(() => image.IsLoaded, "borrowed image load");

            image.LoadState.ShouldBe(ImageLoadState.Loaded);
            image.IsLoading.ShouldBeFalse();
            image.IsFailed.ShouldBeFalse();
            image.LoadedImage.ShouldBeSameAs(borrowed);
            image.Classes.ShouldContain(":loaded");
            image.Classes.ShouldContain(":has-image");
            opened.ShouldNotBeNull().IsFallback.ShouldBeFalse();
            var presenter = image.GetVisualDescendants()
                                 .OfType<Image>()
                                 .Single(control => control.Name == "PART_ImagePresenter");
            presenter.IsVisible.ShouldBeTrue();
            presenter.Source.ShouldBeSameAs(borrowed);
        }

        borrowed.DisposeCount.ShouldBe(0);
    }

    [Fact]
    public void Failed_Primary_Uses_Fallback_Within_The_Same_Generation()
    {
        var fallback = new TestBorrowedImage();
        ImageOpenedEventArgs? opened = null;
        var image = new AsyncImage
        {
            Width = 32,
            Height = 32,
            Source = ImageLoadSource.FromBytes(new byte[] { 1 }, "invalid", "v1"),
            FallbackSource = ImageLoadSource.FromImage(fallback)
        };
        image.ImageOpened += (_, args) => opened = args;
        using var host = new ImageControlTestHost(image);

        ImageControlTestHost.WaitUntil(() => image.IsLoaded, "fallback image load");

        image.LoadedImage.ShouldBeSameAs(fallback);
        image.Classes.ShouldContain(":fallback");
        opened.ShouldNotBeNull().IsFallback.ShouldBeTrue();
        opened.Source.ShouldBeSameAs(image.FallbackSource);
    }

    [Fact]
    public void Terminal_Failure_Shows_Error_Content_And_Raises_Event()
    {
        ImageFailedEventArgs? failed = null;
        var image = new AsyncImage
        {
            Width = 32,
            Height = 32,
            Source = ImageLoadSource.FromBytes(new byte[] { 1 }, "invalid", "v1"),
            ErrorContent = "Unable to load"
        };
        image.ImageFailed += (_, args) => failed = args;
        using var host = new ImageControlTestHost(image);

        ImageControlTestHost.WaitUntil(() => image.IsFailed, "image failure");

        image.LoadError.ShouldNotBeNull().Exception.ShouldBeNull();
        image.LoadError.ShouldNotBeNull().Code.ShouldBe(ImageLoadErrorCode.UnsupportedFormat);
        failed.ShouldNotBeNull().Error.ShouldBeSameAs(image.LoadError);
        image.Classes.ShouldContain(":failed");
        var presenter = image.GetVisualDescendants()
                             .OfType<ContentPresenter>()
                             .Single(control => control.Name == "PART_ErrorPresenter");
        presenter.IsVisible.ShouldBeTrue();
        presenter.Content.ShouldBe("Unable to load");
    }

    [Fact]
    public void Source_Replacement_Does_Not_Commit_Stale_Result()
    {
        var firstStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseFirst = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var first = ImageLoadSource.FromStream(
            async _ =>
            {
                firstStarted.TrySetResult();
                await releaseFirst.Task;
                return new MemoryStream(ImageControlTestHost.CreatePng(16, 16));
            },
            "first",
            "v1");
        var secondImage = new TestBorrowedImage();
        var second = ImageLoadSource.FromImage(secondImage);
        var image = new AsyncImage { Width = 32, Height = 32, Source = first };
        using var host = new ImageControlTestHost(image);
        ImageControlTestHost.WaitUntil(() => firstStarted.Task.IsCompleted, "first request start");

        image.Source = second;
        ImageControlTestHost.WaitUntil(
            () => image.IsLoaded && ReferenceEquals(image.LoadedImage, secondImage),
            "second image commit");
        releaseFirst.TrySetResult();
        Avalonia.Threading.Dispatcher.UIThread.RunJobs();

        image.Source.ShouldBeSameAs(second);
        image.LoadedImage.ShouldBeSameAs(secondImage);
        image.LoadState.ShouldBe(ImageLoadState.Loaded);
    }

    [Fact]
    public void Reload_Bypasses_Cache_And_Starts_A_New_Read()
    {
        var bytes = ImageControlTestHost.CreatePng(32, 32);
        var reads = 0;
        var source = ImageLoadSource.FromStream(
            _ =>
            {
                Interlocked.Increment(ref reads);
                return ValueTask.FromResult<Stream>(new MemoryStream(bytes));
            },
            "reload",
            "v1");
        var image = new AsyncImage { Width = 32, Height = 32, Source = source };
        using var host = new ImageControlTestHost(image);
        ImageControlTestHost.WaitUntil(
            () => image.LoadState is ImageLoadState.Loaded or ImageLoadState.Failed,
            "initial image terminal state");
        image.LoadError.ShouldBeNull();
        reads.ShouldBe(1);

        image.Reload();
        ImageControlTestHost.WaitUntil(() => image.IsLoaded && reads == 2, "reloaded image");

        reads.ShouldBe(2);
    }

    [Fact]
    public void Detach_Releases_State_And_Reattach_Loads_Current_Source_Again()
    {
        var borrowed = new TestBorrowedImage();
        var image = new AsyncImage
        {
            Width = 32,
            Height = 32,
            Source = ImageLoadSource.FromImage(borrowed)
        };
        using var host = new ImageControlTestHost(image);
        ImageControlTestHost.WaitUntil(() => image.IsLoaded, "initial attach load");

        host.Detach();
        image.LoadState.ShouldBe(ImageLoadState.Idle);
        image.LoadedImage.ShouldBeNull();

        host.Attach(image);
        ImageControlTestHost.WaitUntil(() => image.IsLoaded, "reattach load");
        image.LoadedImage.ShouldBeSameAs(borrowed);
        borrowed.DisposeCount.ShouldBe(0);
    }

    [Fact]
    public void Auto_Decode_Uses_Sixteen_Physical_Pixel_Buckets()
    {
        var bytes = ImageControlTestHost.CreatePng(64, 64);
        ImageOpenedEventArgs? opened = null;
        var image = new AsyncImage
        {
            Width = 17,
            Height = 17,
            Source = ImageLoadSource.FromBytes(bytes, "bucket", "v1")
        };
        image.ImageOpened += (_, args) => opened = args;
        using var host = new ImageControlTestHost(image);

        ImageControlTestHost.WaitUntil(
            () => image.LoadState is ImageLoadState.Loaded or ImageLoadState.Failed,
            "bucketed image terminal state");
        image.LoadError.ShouldBeNull();

        opened.ShouldNotBeNull().PixelWidth.ShouldBe(32);
        opened.PixelHeight.ShouldBe(32);
    }

    [Fact]
    public void Asset_Svg_Decoded_Off_Thread_Can_Be_Measured_On_The_Ui_Thread()
    {
        var image = new AsyncImage
        {
            Width = 48,
            Height = 32,
            Source = ImageLoadSource.FromAsset(
                new Uri("avares://AtomUI.Controls.Tests/Assets/ImageLoading/Test.svg"))
        };
        using var host = new ImageControlTestHost(image, width: 48, height: 32);

        ImageControlTestHost.WaitUntil(
            () => image.LoadState is ImageLoadState.Loaded or ImageLoadState.Failed,
            "SVG image terminal state");

        image.LoadError.ShouldBeNull();
        image.LoadedImage.ShouldNotBeNull().Size.ShouldBe(new Size(48, 32));
        Should.NotThrow(() => image.Measure(new Size(48, 32)));
    }

    [Fact]
    public void Asset_Svg_Can_Be_Released_By_Background_Cache_Clear()
    {
        var loader = Application.Current.ShouldNotBeNull().GetImageLoader();
        var source = ImageLoadSource.FromAsset(
            new Uri("avares://AtomUI.Controls.Tests/Assets/ImageLoading/Test.svg"));
        ImageLoadResult? result = null;
        Exception? loadException = null;
        var loadTask = Task.Run(async () =>
        {
            try
            {
                result = await loader.LoadAsync(
                    new ImageLoadRequest(source),
                    TestContext.Current.CancellationToken);
            }
            catch (Exception exception)
            {
                loadException = exception;
            }
        }, TestContext.Current.CancellationToken);
        ImageControlTestHost.WaitUntil(() => loadTask.IsCompleted, "SVG loader result");
        loadException.ShouldBeNull();
        var loadedResult = result.ShouldNotBeNull();
        loadedResult.IsSuccess.ShouldBeTrue();

        Exception? releaseException = null;
        var releaseTask = Task.Run(async () =>
        {
            try
            {
                loadedResult.Dispose();
                await loader.ClearCacheAsync(
                    new ImageCacheClearRequest(),
                    TestContext.Current.CancellationToken);
            }
            catch (Exception exception)
            {
                releaseException = exception;
            }
        }, TestContext.Current.CancellationToken);
        ImageControlTestHost.WaitUntil(() => releaseTask.IsCompleted, "background SVG release");

        releaseException.ShouldBeNull();
        Avalonia.Threading.Dispatcher.UIThread.RunJobs();
    }

    private sealed class TestBorrowedImage : IImage, IDisposable
    {
        private int _disposeCount;

        public Size Size => new(24, 24);

        internal int DisposeCount => Volatile.Read(ref _disposeCount);

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
        }

        public void Dispose()
        {
            Interlocked.Increment(ref _disposeCount);
        }
    }
}
