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
    public void Source_Loads_Local_Image_Item()
    {
        var path = CreatePngFile();
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            Source = new UriImagePreviewSource(path)
        };

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems.Count.ShouldBe(1);
        previewer.EffectiveItems[0].LoadedSource.ShouldNotBeNull();
    }

    [Fact]
    public void Source_Uses_FallbackSource_When_Load_Fails()
    {
        var fallbackPath = CreatePngFile();
        var missingPath  = Path.Combine(Path.GetTempPath(), $"atomui-missing-{Guid.NewGuid():N}.png");
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            Source         = new UriImagePreviewSource(missingPath),
            FallbackSource = new UriImagePreviewSource(fallbackPath)
        };

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems[0].Source.ShouldBeSameAs(previewer.FallbackSource);
    }

    [Fact]
    public void Sources_Closed_State_Loads_Only_CoverIndex_Stream_Source()
    {
        var openCounts = new int[3];
        var sources = Enumerable.Range(0, openCounts.Length)
                                .Select(index => CreateStreamSource($"stream-{index}", openCounts, index))
                                .ToList<IImagePreviewSource>();
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            CoverIndex = 1,
            Sources    = sources
        };

        WaitUntil(() => previewer.EffectiveItems is { Count: 3 } items &&
                        items[1].State == ImagePreviewItemState.Loaded)
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        openCounts.ShouldBe([0, 1, 0]);
        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems[0].State.ShouldBe(ImagePreviewItemState.Pending);
        previewer.EffectiveItems[1].Source.ShouldBeSameAs(sources[1]);
        previewer.EffectiveItems[2].State.ShouldBe(ImagePreviewItemState.Pending);
    }

    [Fact]
    public void Sources_Wins_When_Source_Is_Also_Set()
    {
        var sourceOpenCount  = 0;
        var sourcesOpenCount = 0;
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            Sources =
            [
                new StreamImagePreviewSource(_ =>
                {
                    sourcesOpenCount++;
                    return new ValueTask<Stream>(new MemoryStream(CreatePngBytes(), writable: false));
                }, displayName: "collection.png", contentType: "image/png")
            ],
            Source = new StreamImagePreviewSource(_ =>
            {
                sourceOpenCount++;
                return new ValueTask<Stream>(new MemoryStream(CreatePngBytes(), writable: false));
            }, displayName: "single.png", contentType: "image/png")
        };

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        sourceOpenCount.ShouldBe(0);
        sourcesOpenCount.ShouldBe(1);
        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems[0].Source.ShouldBeSameAs(previewer.Sources![0]);
    }

    [Fact]
    public void FallbackSource_Loads_When_All_Stream_Sources_Fail()
    {
        var fallbackOpenCount = 0;
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            Sources =
            [
                CreateFailingStreamSource("failed-0"),
                CreateFailingStreamSource("failed-1")
            ],
            FallbackSource = new StreamImagePreviewSource(_ =>
            {
                fallbackOpenCount++;
                return new ValueTask<Stream>(new MemoryStream(CreatePngBytes(), writable: false));
            }, displayName: "fallback.png", contentType: "image/png")
        };

        WaitUntil(() => previewer.EffectiveItems is { Count: 2 } items &&
                        items[0].State == ImagePreviewItemState.Failed)
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.CurrentIndex = 1;
        previewer.RequestPreviewLoads();

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        fallbackOpenCount.ShouldBe(1);
        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems[0].Source.ShouldBeSameAs(previewer.FallbackSource);
    }

    [Fact]
    public void Sources_Closed_State_Loads_Only_CoverIndex_Item()
    {
        var firstPath  = CreatePngFile();
        var secondPath = CreatePngFile();
        var thirdPath  = CreatePngFile();
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            CoverIndex = 1,
            Sources =
            [
                new UriImagePreviewSource(firstPath),
                new UriImagePreviewSource(secondPath),
                new UriImagePreviewSource(thirdPath)
            ]
        };

        WaitUntil(() => previewer.EffectiveItems is { Count: 3 } items &&
                        items[1].State == ImagePreviewItemState.Loaded &&
                        previewer.EffectiveCoverImage is not null)
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems[0].State.ShouldBe(ImagePreviewItemState.Pending);
        previewer.EffectiveItems[1].Source.ShouldBeSameAs(previewer.Sources![1]);
        previewer.EffectiveItems[1].LoadedSource.ShouldBeSameAs(previewer.EffectiveCoverImage);
        previewer.EffectiveItems[2].State.ShouldBe(ImagePreviewItemState.Pending);
    }

    [Fact]
    public void Sources_Keep_Pending_Items_When_Only_CoverIndex_Loads()
    {
        var loadedPath  = CreatePngFile();
        var pendingPath = CreatePngFile();
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            CoverIndex = 0,
            Sources =
            [
                new UriImagePreviewSource(loadedPath),
                new UriImagePreviewSource(pendingPath)
            ]
        };

        WaitUntil(() => previewer.EffectiveItems is { Count: 2 } items &&
                        items[0].State == ImagePreviewItemState.Loaded)
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems.Count.ShouldBe(2);
        previewer.EffectiveItems[0].Source.ShouldBeSameAs(previewer.Sources![0]);
        previewer.EffectiveItems[1].State.ShouldBe(ImagePreviewItemState.Pending);
    }

    [Fact]
    public void Sources_Do_Not_Use_Fallback_When_Cover_Fails_But_Other_Sources_Are_Pending()
    {
        var pendingPath  = CreatePngFile();
        var fallbackPath = CreatePngFile();
        var missingPath  = Path.Combine(Path.GetTempPath(), $"atomui-missing-{Guid.NewGuid():N}.png");
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            FallbackSource = new UriImagePreviewSource(fallbackPath),
            Sources =
            [
                new UriImagePreviewSource(missingPath),
                new UriImagePreviewSource(pendingPath)
            ]
        };

        WaitUntil(() => previewer.EffectiveItems is { Count: 2 } items &&
                        items[0].State == ImagePreviewItemState.Failed)
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems.Count.ShouldBe(2);
        previewer.EffectiveItems[0].State.ShouldBe(ImagePreviewItemState.Failed);
        previewer.EffectiveItems[1].State.ShouldBe(ImagePreviewItemState.Pending);
        previewer.EffectiveItems.Any(item => ReferenceEquals(item.Source, previewer.FallbackSource)).ShouldBeFalse();
    }

    [Fact]
    public void Sources_Keep_Loaded_Items_Instead_Of_Fallback_When_Only_Some_Items_Fail()
    {
        var loadedPath   = CreatePngFile();
        var fallbackPath = CreatePngFile();
        var missingPath  = Path.Combine(Path.GetTempPath(), $"atomui-missing-{Guid.NewGuid():N}.png");
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            FallbackSource = new UriImagePreviewSource(fallbackPath),
            Sources =
            [
                new UriImagePreviewSource(missingPath),
                new UriImagePreviewSource(loadedPath)
            ]
        };

        WaitUntil(() => previewer.EffectiveItems is { Count: 2 } items &&
                        items[0].State == ImagePreviewItemState.Failed)
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.CurrentIndex = 1;
        previewer.RequestPreviewLoads();

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems.Count.ShouldBe(1);
        previewer.EffectiveItems[0].Source.ShouldBeSameAs(previewer.Sources![1]);
    }

    [Fact]
    public void Sources_Use_FallbackSource_When_All_Source_Items_Fail()
    {
        var fallbackPath      = CreatePngFile();
        var firstMissingPath  = Path.Combine(Path.GetTempPath(), $"atomui-missing-{Guid.NewGuid():N}.png");
        var secondMissingPath = Path.Combine(Path.GetTempPath(), $"atomui-missing-{Guid.NewGuid():N}.png");
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            FallbackSource = new UriImagePreviewSource(fallbackPath),
            Sources =
            [
                new UriImagePreviewSource(firstMissingPath),
                new UriImagePreviewSource(secondMissingPath)
            ]
        };

        WaitUntil(() => previewer.EffectiveItems is { Count: 2 } items &&
                        items[0].State == ImagePreviewItemState.Failed)
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.CurrentIndex = 1;
        previewer.RequestPreviewLoads();

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems.Count.ShouldBe(1);
        previewer.EffectiveItems[0].Source.ShouldBeSameAs(previewer.FallbackSource);
    }

    [Fact]
    public void CoverIndex_Does_Not_Synchronize_With_CurrentIndex()
    {
        var firstPath  = CreatePngFile();
        var secondPath = CreatePngFile();
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            CurrentIndex = 1,
            CoverIndex   = 0,
            Sources =
            [
                new UriImagePreviewSource(firstPath),
                new UriImagePreviewSource(secondPath)
            ]
        };

        WaitUntil(() => previewer.EffectiveItems is { Count: 2 } items &&
                        items[0].State == ImagePreviewItemState.Loaded &&
                        previewer.EffectiveCoverImage is not null)
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.CurrentIndex.ShouldBe(1);
        previewer.EffectiveCoverImage.ShouldBeSameAs(previewer.EffectiveItems![0].LoadedSource);

        previewer.CurrentIndex = 0;
        Dispatcher.UIThread.RunJobs();

        previewer.CoverIndex.ShouldBe(0);
        previewer.EffectiveCoverImage.ShouldBeSameAs(previewer.EffectiveItems[0].LoadedSource);

        previewer.CoverIndex = 1;

        WaitUntil(() => previewer.EffectiveItems is { Count: 2 } items &&
                        items[1].State == ImagePreviewItemState.Loaded &&
                        ReferenceEquals(previewer.EffectiveCoverImage, items[1].LoadedSource))
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.CurrentIndex.ShouldBe(0);
    }

    [Fact]
    public void PrepareDialogOpen_Keeps_Loaded_Cover_When_Sources_Are_Unchanged()
    {
        var loader = new TrackingImageSourceLoader();
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer(loader)
        {
            CoverIndex   = 0,
            CurrentIndex = 5,
            PreloadCount = 1,
            Sources =
            [
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/0.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/1.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/2.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/3.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/4.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/5.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/6.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/7.png")
            ]
        };

        WaitUntil(() => loader.StartedCount >= 1)
            .ShouldBeTrue("cover load did not start");
        CompleteLoaderUntilIdle(loader, expectedStartedCount: 1);

        WaitUntil(() => previewer.EffectiveItems is { Count: 8 } items &&
                        items[0].IsLoaded &&
                        previewer.EffectiveCoverImage is not null)
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));
        var coverImage = previewer.EffectiveCoverImage;

        InvokePrepareDialogOpen(previewer);

        previewer.EffectiveCoverImage.ShouldBeSameAs(coverImage);
        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems[0].LoadedSource.ShouldBeSameAs(coverImage);
    }

    [Fact]
    public void StreamSource_Stable_Identity_Reuses_Loaded_Item_And_Updates_Source()
    {
        var firstOpenCount  = 0;
        var secondOpenCount = 0;
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            Source = new StreamImagePreviewSource(_ =>
            {
                firstOpenCount++;
                return new ValueTask<Stream>(new MemoryStream(CreatePngBytes(), writable: false));
            }, displayName: "avatar.png", contentType: "image/png", identity: "avatar:42")
        };

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        var item         = previewer.EffectiveItems![0];
        var loadedSource = item.LoadedSource;
        var secondSource = new StreamImagePreviewSource(_ =>
        {
            secondOpenCount++;
            return new ValueTask<Stream>(new MemoryStream(CreatePngBytes(), writable: false));
        }, displayName: "avatar.png", contentType: "image/png", identity: "avatar:42");

        previewer.Source = secondSource;
        Dispatcher.UIThread.RunJobs();

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems[0].ShouldBeSameAs(item);
        previewer.EffectiveItems[0].Source.ShouldBeSameAs(secondSource);
        previewer.EffectiveItems[0].LoadedSource.ShouldBeSameAs(loadedSource);
        firstOpenCount.ShouldBe(1);
        secondOpenCount.ShouldBe(0);
    }

    [Fact]
    public void StreamSource_Without_Identity_Uses_Source_Object_As_Identity()
    {
        var firstOpenCount  = 0;
        var secondOpenCount = 0;
        var firstSource = new StreamImagePreviewSource(_ =>
        {
            firstOpenCount++;
            return new ValueTask<Stream>(new MemoryStream(CreatePngBytes(), writable: false));
        }, displayName: "avatar.png", contentType: "image/png");
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            Source = firstSource
        };

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        var firstItem = previewer.EffectiveItems![0];
        var secondSource = new StreamImagePreviewSource(_ =>
        {
            secondOpenCount++;
            return new ValueTask<Stream>(new MemoryStream(CreatePngBytes(), writable: false));
        }, displayName: "avatar.png", contentType: "image/png");

        previewer.Source = secondSource;

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }] items &&
                        ReferenceEquals(items[0].Source, secondSource))
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems[0].ShouldNotBeSameAs(firstItem);
        previewer.EffectiveItems[0].Source.ShouldBeSameAs(secondSource);
        firstOpenCount.ShouldBe(1);
        secondOpenCount.ShouldBe(1);
    }

    [Fact]
    public void FallbackSource_Change_Does_Not_Reload_Primary_Source()
    {
        var sourcePath   = CreatePngFile();
        var fallbackPath = CreatePngFile();
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            Source = new UriImagePreviewSource(sourcePath)
        };

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        var effectiveItems = previewer.EffectiveItems;
        var primaryItem    = effectiveItems![0];
        var loadedSource   = primaryItem.LoadedSource;

        previewer.FallbackSource = new UriImagePreviewSource(fallbackPath);
        Dispatcher.UIThread.RunJobs();

        previewer.EffectiveItems.ShouldBeSameAs(effectiveItems);
        previewer.EffectiveItems![0].ShouldBeSameAs(primaryItem);
        previewer.EffectiveItems[0].LoadedSource.ShouldBeSameAs(loadedSource);
        previewer.EffectiveItems[0].Source.ShouldBeSameAs(previewer.Source);
    }

    [Fact]
    public void FallbackSource_Loads_When_No_Main_Source()
    {
        var fallbackPath = CreatePngFile();
        var previewer    = new global::AtomUI.Desktop.Controls.ImagePreviewer();

        previewer.FallbackSource = new UriImagePreviewSource(fallbackPath);

        WaitUntil(() => previewer.EffectiveItems is [{ State: ImagePreviewItemState.Loaded }])
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems[0].Source.ShouldBeSameAs(previewer.FallbackSource);
    }

    [Fact]
    public void MaxConcurrentLoads_Limits_Loading_Tasks()
    {
        var loader = new TrackingImageSourceLoader();
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer(loader)
        {
            MaxConcurrentLoads = 2,
            PreloadCount       = 4,
            Sources =
            [
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/0.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/1.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/2.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/3.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/4.png")
            ]
        };

        WaitUntil(() => loader.StartedCount >= 1)
            .ShouldBeTrue("cover load did not start");

        previewer.CurrentIndex = 2;
        previewer.RequestPreviewLoads();

        WaitUntil(() => loader.StartedCount >= 2)
            .ShouldBeTrue($"started={loader.StartedCount}, active={loader.ActiveCount}");

        loader.MaxObservedActiveCount.ShouldBeLessThanOrEqualTo(2);

        CompleteLoaderUntilIdle(loader, expectedStartedCount: 5);
    }

    [Fact]
    public void ImageGroupPreviewer_Sources_Loads_Default_Cover_Items()
    {
        var loader = new TrackingImageSourceLoader();
        var previewer = new ImageGroupPreviewer(loader)
        {
            MaxConcurrentLoads = 2,
            Sources =
            [
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/group-0.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/group-1.png"),
                new UriImagePreviewSource("avares://AtomUI.Tests/Assets/group-2.png")
            ]
        };

        WaitUntil(() => loader.StartedCount >= 2)
            .ShouldBeTrue($"started={loader.StartedCount}, active={loader.ActiveCount}");

        loader.MaxObservedActiveCount.ShouldBeLessThanOrEqualTo(2);

        CompleteLoaderUntilIdle(loader, expectedStartedCount: 3);

        WaitUntil(() => previewer.EffectiveItems is { Count: 3 } items &&
                        items.All(item => item.IsLoaded))
            .ShouldBeTrue(DescribeItems(previewer.EffectiveItems));

        previewer.EffectiveItems.ShouldNotBeNull();
        previewer.EffectiveItems.Count.ShouldBe(3);
        previewer.EffectiveItems.All(item => item.IsLoaded).ShouldBeTrue(DescribeItems(previewer.EffectiveItems));
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
            $"{DescribeSource(item.Source)}:{item.State}:{item.Error?.GetType().Name}:{item.Error?.Message}"));
    }

    private static string DescribeSource(IImagePreviewSource source)
    {
        return source switch
        {
            UriImagePreviewSource uriSource => uriSource.SourceUri.CacheKey,
            IImagePreviewSourceIdentity identitySource => identitySource.Identity.ToString() ?? source.GetType().Name,
            _ => source.DisplayName ?? source.GetType().Name
        };
    }

    private static string CreatePngFile()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"atomui-image-previewer-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "sample.png");
        File.WriteAllBytes(path, CreatePngBytes());
        return path;
    }

    private static byte[] CreatePngBytes()
    {
        const string base64 =
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+/p9sAAAAASUVORK5CYII=";
        return Convert.FromBase64String(base64);
    }

    private static IImagePreviewSource CreateStreamSource(string key, int[] openCounts, int index)
    {
        return new StreamImagePreviewSource(_ =>
        {
            openCounts[index]++;
            return new ValueTask<Stream>(new MemoryStream(CreatePngBytes(), writable: false));
        }, displayName: $"{key}.png", contentType: "image/png", identity: key);
    }

    private static IImagePreviewSource CreateFailingStreamSource(string key)
    {
        return new StreamImagePreviewSource(_ => throw new IOException(key), displayName: $"{key}.png", identity: key);
    }

    private static void CompleteLoaderUntilIdle(TrackingImageSourceLoader loader, int expectedStartedCount)
    {
        for (var i = 0; i < 50; i++)
        {
            loader.CompleteAll();
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(20);

            if (loader.StartedCount >= expectedStartedCount && loader.ActiveCount == 0)
            {
                return;
            }
        }

        throw new TimeoutException(
            $"loader did not become idle: started={loader.StartedCount}, active={loader.ActiveCount}");
    }

    private static void InvokePrepareDialogOpen(AbstractImagePreviewer previewer)
    {
        var method = typeof(AbstractImagePreviewer).GetMethod(
            "PrepareDialogOpen",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        method.ShouldNotBeNull();
        method.Invoke(previewer, null);
    }

    private sealed class TrackingImageSourceLoader : IImageSourceLoader
    {
        private readonly List<TaskCompletionSource<LoadedImageSource>> _pendingLoads = [];
        private readonly object _syncRoot = new();

        public int ActiveCount { get; private set; }

        public int StartedCount { get; private set; }

        public int MaxObservedActiveCount { get; private set; }

        public Task<LoadedImageSource> LoadAsync(IImagePreviewSource source, CancellationToken cancellationToken)
        {
            lock (_syncRoot)
            {
                ActiveCount++;
                StartedCount++;
                MaxObservedActiveCount = Math.Max(MaxObservedActiveCount, ActiveCount);
            }

            var completion = new TaskCompletionSource<LoadedImageSource>(TaskCreationOptions.RunContinuationsAsynchronously);
            lock (_pendingLoads)
            {
                _pendingLoads.Add(completion);
            }

            cancellationToken.Register(() =>
            {
                completion.TrySetCanceled(cancellationToken);
            });

            return completion.Task.ContinueWith(task =>
            {
                lock (_syncRoot)
                {
                    ActiveCount--;
                }

                return task.GetAwaiter().GetResult();
            }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        public void CompleteAll()
        {
            List<TaskCompletionSource<LoadedImageSource>> pendingLoads;
            lock (_pendingLoads)
            {
                pendingLoads = _pendingLoads.ToList();
                _pendingLoads.Clear();
            }

            foreach (var pendingLoad in pendingLoads)
            {
                pendingLoad.TrySetResult(LoadedImageSource.CreateSvg("<svg xmlns=\"http://www.w3.org/2000/svg\"/>"));
            }
        }
    }
}
