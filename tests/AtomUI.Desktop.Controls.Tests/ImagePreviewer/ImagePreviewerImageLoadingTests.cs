using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImagePreviewerImageLoadingTests
{
    public ImagePreviewerImageLoadingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Open_Previewer_Loads_Replacement_Enumerable_Current_Item()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var first = new TestImage();
            var second = new TestImage();
            var previewer = CreatePreviewer(new ImagePreviewItem(ImageLoadSource.FromImage(first)));
            using var host = new PreviewerHost(previewer);
            previewer.OpenDialog();
            WaitUntil(() => previewer.IsCurrentLoaded, "initial current item");

            previewer.ItemsSource =
            [
                new ImagePreviewItem(ImageLoadSource.FromImage(second))
            ];

            WaitUntil(() => previewer.IsCurrentLoaded, "replacement current item");
            previewer.CurrentItem.ShouldNotBeNull().Source.ShouldBeSameAs(
                previewer.ItemsSource.ShouldNotBeNull().Single().Source);
            previewer.EffectiveItems.ShouldNotBeNull().Single().FullImage.ShouldBeSameAs(second);
        });
    }

    [Fact]
    public void Current_Index_Switch_Does_Not_Commit_The_Previous_Full_Request()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var firstStarted = NewSignal();
            var releaseFirst = NewSignal();
            var firstSource = ImageLoadSource.FromStream(
                async token =>
                {
                    firstStarted.TrySetResult();
                    await releaseFirst.Task.WaitAsync(token);
                    return new MemoryStream(CreatePng(32, 32));
                },
                $"preview-switch-first-{Guid.NewGuid():N}",
                "v1");
            var secondImage = new TestImage();
            var previewer = CreatePreviewer(
                new ImagePreviewItem(firstSource),
                new ImagePreviewItem(ImageLoadSource.FromImage(secondImage)));
            using var host = new PreviewerHost(previewer);

            previewer.OpenDialog();
            WaitUntil(() => firstStarted.Task.IsCompleted, "first full request start");

            previewer.CurrentIndex = 1;
            WaitUntil(
                () => previewer.IsCurrentLoaded &&
                      ReferenceEquals(previewer.EffectiveItems![1].FullImage, secondImage),
                "second current full request");

            releaseFirst.TrySetResult();
            WaitUntil(() => previewer.EffectiveItems![0].FullState != ImageLoadState.Loading,
                "cancelled first full request cleanup");

            var entries = previewer.EffectiveItems.ShouldNotBeNull();
            previewer.CurrentIndex.ShouldBe(1);
            previewer.CurrentItem.ShouldBeSameAs(previewer.ItemsSource!.ElementAt(1));
            entries[1].FullImage.ShouldBeSameAs(secondImage);
        });
    }

    [Fact]
    public void Open_Previewer_Loads_Current_Item_After_Observable_Reset()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var first = new TestImage();
            var second = new TestImage();
            var items = new ResettableCollection<ImagePreviewItem>(
            [
                new ImagePreviewItem(ImageLoadSource.FromImage(first))
            ]);
            var previewer = CreatePreviewer(items);
            using var host = new PreviewerHost(previewer);
            previewer.OpenDialog();
            WaitUntil(() => previewer.IsCurrentLoaded, "initial current item");

            items.ResetWith(
            [
                new ImagePreviewItem(ImageLoadSource.FromImage(second))
            ]);

            WaitUntil(() => previewer.IsCurrentLoaded, "reset current item");
            previewer.EffectiveItems.ShouldNotBeNull().Single().FullImage.ShouldBeSameAs(second);
        });
    }

    [Fact]
    public void Observable_Collection_Operations_Preserve_Or_Dispose_The_Correct_Entries()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var first = CreateItem("first");
            var second = CreateItem("second");
            var third = CreateItem("third");
            var items = new ObservableCollection<ImagePreviewItem>([first, second, third]);
            var previewer = CreatePreviewer(items);
            using var host = new PreviewerHost(previewer);
            var initial = previewer.EffectiveItems.ShouldNotBeNull().ToArray();

            var added = CreateItem("added");
            items.Insert(1, added);

            previewer.EffectiveItems.ShouldNotBeNull()[0].ShouldBeSameAs(initial[0]);
            previewer.EffectiveItems[1].Item.ShouldBeSameAs(added);
            previewer.EffectiveItems[2].ShouldBeSameAs(initial[1]);
            previewer.EffectiveItems[3].ShouldBeSameAs(initial[2]);

            items.Move(2, 0);

            previewer.EffectiveItems[0].ShouldBeSameAs(initial[1]);
            var replacedEntry = previewer.EffectiveItems[1];
            var replacement = CreateItem("replacement");
            items[1] = replacement;

            previewer.EffectiveItems[1].Item.ShouldBeSameAs(replacement);
            Should.Throw<ObjectDisposedException>(() =>
                replacedEntry.LoadFull(16, 16, ImageRequestPriority.Critical));

            var removedEntry = previewer.EffectiveItems[2];
            items.RemoveAt(2);

            Should.Throw<ObjectDisposedException>(() =>
                removedEntry.LoadThumbnail(16, 16, ImageRequestPriority.High));
            previewer.EffectiveItems.Count.ShouldBe(3);
        });
    }

    [Fact]
    public void Detach_Releases_Entries_Unsubscribes_Collection_And_Reattach_Reconciles_Changes()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var full = new TestImage();
            var thumbnail = new TestImage();
            var first = new ImagePreviewItem(ImageLoadSource.FromImage(full))
            {
                ThumbnailSource = ImageLoadSource.FromImage(thumbnail)
            };
            var items = new ObservableCollection<ImagePreviewItem>([first]);
            var previewer = CreatePreviewer(items);
            using var host = new PreviewerHost(previewer);
            previewer.RequestPreviewLoads();
            var entry = previewer.EffectiveItems.ShouldNotBeNull().Single();
            WaitUntil(
                () => entry.FullState == ImageLoadState.Loaded &&
                      entry.ThumbnailState == ImageLoadState.Loaded,
                "full and thumbnail loads");

            host.Detach();

            entry.FullImage.ShouldBeNull();
            entry.ThumbnailImage.ShouldBeNull();
            entry.FullState.ShouldBe(ImageLoadState.Idle);
            entry.ThumbnailState.ShouldBe(ImageLoadState.Idle);
            items.Add(CreateItem("added-while-detached"));
            previewer.EffectiveItems.Count.ShouldBe(1);

            host.Attach(previewer);

            WaitUntil(() => previewer.EffectiveItems?.Count == 2, "reattach collection reconciliation");
        });
    }

    [Fact]
    public void Entries_Hold_Loading_State_Independently_From_The_Immutable_Item()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var image = new TestImage();
            var item = new ImagePreviewItem(ImageLoadSource.FromImage(image));
            using var first = new ImagePreviewEntry(item);
            using var second = new ImagePreviewEntry(item);

            first.LoadFull(16, 16, ImageRequestPriority.Critical);
            WaitUntil(() => first.FullState == ImageLoadState.Loaded, "first entry load");

            first.FullImage.ShouldBeSameAs(image);
            second.FullImage.ShouldBeNull();
            second.FullState.ShouldBe(ImageLoadState.Idle);
            item.GetType().GetProperties().Select(property => property.Name)
                .ShouldNotContain(nameof(ImagePreviewEntry.FullState));
        });
    }

    [Fact]
    public void Cover_Size_Change_Upgrades_The_Thumbnail_Decode_Bucket()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var source = ImageLoadSource.FromBytes(
                CreatePng(128, 128),
                $"cover-size-{Guid.NewGuid():N}",
                "v1");
            var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
            {
                CoverWidth = 16,
                CoverHeight = 16,
                ItemsSource = [new ImagePreviewItem(source)]
            };
            using var host = new PreviewerHost(previewer);
            var entry = previewer.EffectiveItems.ShouldNotBeNull().Single();
            WaitUntil(
                () => entry.ThumbnailState == ImageLoadState.Loaded,
                "initial thumbnail load");
            GetThumbnailPixelSize(entry).ShouldBe(new PixelSize(16, 16));

            previewer.CoverWidth = 96;
            previewer.CoverHeight = 96;

            WaitUntil(
                () => entry.ThumbnailState == ImageLoadState.Loaded &&
                      GetThumbnailPixelSize(entry) == new PixelSize(96, 96),
                "upgraded thumbnail load");
        });
    }

    [Fact]
    public void Auto_Sized_Cover_Upgrades_From_The_Placeholder_To_Final_Bounds()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var source = ImageLoadSource.FromBytes(
                CreatePng(512, 512),
                $"auto-sized-cover-{Guid.NewGuid():N}",
                "v1");
            var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
            {
                Width = 200,
                ItemsSource = [new ImagePreviewItem(source)]
            };
            using var host = new PreviewerHost(previewer);
            var entry = previewer.EffectiveItems.ShouldNotBeNull().Single();

            WaitUntil(
                () => entry.ThumbnailState == ImageLoadState.Loaded &&
                      GetThumbnailPixelSize(entry).Width >= 200 &&
                      GetThumbnailPixelSize(entry).Height >= 200,
                "auto-sized cover final decode bucket");

            GetThumbnailPixelSize(entry).ShouldBe(new PixelSize(208, 208));
        });
    }

    [Fact]
    public void Repeating_The_Same_Thumbnail_Request_Is_Idempotent()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var reads = 0;
            var bytes = CreatePng(128, 128);
            var source = ImageLoadSource.FromStream(
                _ =>
                {
                    reads++;
                    return ValueTask.FromResult<Stream>(new MemoryStream(bytes));
                },
                $"thumbnail-idempotency-{Guid.NewGuid():N}",
                "v1");
            var item = new ImagePreviewItem(source)
            {
                RequestOptions = new ImageRequestOptions { CacheMode = ImageCacheMode.NoStore }
            };
            using var entry = new ImagePreviewEntry(item);

            entry.LoadThumbnail(32, 32, ImageRequestPriority.High);
            WaitUntil(() => entry.ThumbnailState == ImageLoadState.Loaded, "initial thumbnail request");
            reads.ShouldBe(1);

            entry.LoadThumbnail(32, 32, ImageRequestPriority.High);
            WaitUntil(() => entry.ThumbnailState != ImageLoadState.Loading, "repeated thumbnail request");

            reads.ShouldBe(1);
            GetThumbnailPixelSize(entry).ShouldBe(new PixelSize(32, 32));
        });
    }

    [Fact]
    public void Smaller_Thumbnail_Request_Reuses_The_Existing_Decode()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var reads = 0;
            var bytes = CreatePng(512, 256);
            var source = ImageLoadSource.FromStream(
                _ =>
                {
                    reads++;
                    return ValueTask.FromResult<Stream>(new MemoryStream(bytes));
                },
                $"thumbnail-downsize-{Guid.NewGuid():N}",
                "v1");
            var item = new ImagePreviewItem(source)
            {
                RequestOptions = new ImageRequestOptions { CacheMode = ImageCacheMode.NoStore }
            };
            using var entry = new ImagePreviewEntry(item);

            entry.LoadThumbnail(208, 208, ImageRequestPriority.High);
            WaitUntil(() => entry.ThumbnailState == ImageLoadState.Loaded, "initial thumbnail request");

            entry.LoadThumbnail(208, 112, ImageRequestPriority.High);
            Dispatcher.UIThread.RunJobs();

            reads.ShouldBe(1);
            entry.ThumbnailState.ShouldBe(ImageLoadState.Loaded);
        });
    }

    [Fact]
    public void Auto_Sized_Rectangular_Cover_Settles_After_The_Initial_Load()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var reads = 0;
            var bytes = CreatePng(512, 256);
            var source = ImageLoadSource.FromStream(
                _ =>
                {
                    reads++;
                    return ValueTask.FromResult<Stream>(new MemoryStream(bytes));
                },
                $"auto-cover-settle-{Guid.NewGuid():N}",
                "v1");
            var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
            {
                Width = 200,
                ItemsSource =
                [
                    new ImagePreviewItem(source)
                    {
                        RequestOptions = new ImageRequestOptions { CacheMode = ImageCacheMode.NoStore }
                    }
                ]
            };
            using var host = new PreviewerHost(previewer);

            WaitUntil(() => previewer.IsCoverLoaded, "auto-sized rectangular cover");
            for (var index = 0; index < 10; index++)
            {
                Dispatcher.UIThread.RunJobs();
            }

            reads.ShouldBeLessThanOrEqualTo(2);
            previewer.Bounds.Width.ShouldBe(200);
            previewer.Bounds.Height.ShouldBeGreaterThan(0);
        });
    }

    [Fact]
    public void Width_Only_Cover_Decode_Leaves_The_Auto_Height_Unbounded()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var source = ImageLoadSource.FromBytes(
                CreatePng(1004, 986),
                $"width-only-cover-{Guid.NewGuid():N}",
                "v1");
            var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer
            {
                Width = 200,
                ItemsSource = [new ImagePreviewItem(source)]
            };
            using var host = new PreviewerHost(previewer, renderScaling: 2);
            var entry = previewer.EffectiveItems.ShouldNotBeNull().Single();

            WaitUntil(() => entry.ThumbnailState == ImageLoadState.Loaded, "width-only cover decode");

            GetThumbnailResultRequestSize(entry).ShouldBe(new PixelSize(400, 0));
            GetThumbnailPixelSize(entry).Width.ShouldBe(400);
        });
    }

    [Fact]
    public void Thumbnail_Upgrade_Keeps_The_Previous_Image_Until_Replacement_Succeeds()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var reads = 0;
            var upgradeStarted = NewSignal();
            var releaseUpgrade = NewSignal();
            var bytes = CreatePng(128, 128);
            var source = ImageLoadSource.FromStream(
                async token =>
                {
                    if (Interlocked.Increment(ref reads) == 2)
                    {
                        upgradeStarted.TrySetResult();
                        await releaseUpgrade.Task.WaitAsync(token);
                    }
                    return new MemoryStream(bytes);
                },
                $"thumbnail-upgrade-{Guid.NewGuid():N}",
                "v1");
            var item = new ImagePreviewItem(source)
            {
                RequestOptions = new ImageRequestOptions { CacheMode = ImageCacheMode.NoStore }
            };
            using var entry = new ImagePreviewEntry(item);

            entry.LoadThumbnail(16, 16, ImageRequestPriority.High);
            WaitUntil(() => entry.ThumbnailState == ImageLoadState.Loaded, "initial thumbnail");
            var previous = entry.ThumbnailImage.ShouldNotBeNull();
            GetThumbnailPixelSize(entry).ShouldBe(new PixelSize(16, 16));

            entry.LoadThumbnail(96, 96, ImageRequestPriority.High);
            WaitUntil(() => upgradeStarted.Task.IsCompleted, "thumbnail upgrade start");

            entry.ThumbnailState.ShouldBe(ImageLoadState.Loading);
            entry.ThumbnailImage.ShouldBeSameAs(previous);
            GetThumbnailPixelSize(entry).ShouldBe(new PixelSize(16, 16));

            releaseUpgrade.TrySetResult();
            WaitUntil(
                () => entry.ThumbnailState == ImageLoadState.Loaded &&
                      GetThumbnailPixelSize(entry) == new PixelSize(96, 96),
                "thumbnail upgrade completion");
            entry.ThumbnailImage.ShouldNotBeSameAs(previous);
        });
    }

    [Fact]
    public void Current_Item_Fallback_Does_Not_Change_Other_Item_Failure_State()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var fallback = new TestImage();
            var current = new ImagePreviewItem(
                ImageLoadSource.FromBytes(new byte[] { 1 }, "current-primary", "v1"))
            {
                FallbackSource = ImageLoadSource.FromImage(fallback)
            };
            var neighbor = new ImagePreviewItem(
                ImageLoadSource.FromBytes(new byte[] { 2 }, "neighbor-primary", "v1"));
            var previewer = new TestPreviewer
            {
                Width = 96,
                Height = 96,
                PreloadCount = 1,
                ItemsSource = new[] { current, neighbor }
            };
            ImagePreviewOpenedEventArgs? opened = null;
            ImagePreviewFailedEventArgs? failed = null;
            previewer.ImageOpened += (_, args) => opened = args;
            previewer.ImageFailed += (_, args) => failed = args;
            using var host = new PreviewerHost(previewer);

            previewer.RequestPreviewLoads();
            var entries = previewer.EffectiveItems.ShouldNotBeNull();
            WaitUntil(
                () => entries[0].FullState == ImageLoadState.Loaded &&
                      entries[1].FullState == ImageLoadState.Failed,
                "fallback and neighbor terminal states");

            entries[0].FullImage.ShouldBeSameAs(fallback);
            entries[1].FullImage.ShouldBeNull();
            opened.ShouldNotBeNull().Item.ShouldBeSameAs(current);
            failed.ShouldBeNull();
        });
    }

    [Fact]
    public void Reload_Methods_Only_Restart_The_Target_Entry()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var firstReads = 0;
            var secondReads = 0;
            var first = new ImagePreviewItem(CreateFailingStreamSource("first", () => firstReads++));
            var second = new ImagePreviewItem(CreateFailingStreamSource("second", () => secondReads++));
            var previewer = new TestPreviewer
            {
                Width = 96,
                Height = 96,
                PreloadCount = 1,
                ItemsSource = new[] { first, second }
            };
            using var host = new PreviewerHost(previewer);

            previewer.RequestPreviewLoads();
            var entries = previewer.EffectiveItems.ShouldNotBeNull();
            WaitUntil(
                () => entries.All(entry => entry.FullState == ImageLoadState.Failed),
                "initial failed reads");
            firstReads.ShouldBe(1);
            secondReads.ShouldBe(1);

            previewer.ReloadItem(1);
            WaitUntil(() => secondReads == 2 && entries[1].FullState == ImageLoadState.Failed, "item reload");
            firstReads.ShouldBe(1);

            previewer.ReloadCurrent();
            WaitUntil(() => firstReads == 2 && entries[0].FullState == ImageLoadState.Failed, "current reload");
            secondReads.ShouldBe(2);
        });
    }

    [Fact]
    public void ReloadCover_Only_Restarts_The_Cover_Thumbnail_Channel()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var thumbnailReads = 0;
            var full = new TestImage();
            var previewer = CreatePreviewer(new ImagePreviewItem(ImageLoadSource.FromImage(full))
            {
                ThumbnailSource = CreateFailingStreamSource("cover-thumbnail", () => thumbnailReads++)
            });
            using var host = new PreviewerHost(previewer);
            var entry = previewer.EffectiveItems.ShouldNotBeNull().Single();
            WaitUntil(() => previewer.IsCoverLoaded, "initial cover fallback");
            thumbnailReads.ShouldBe(1);
            entry.FullState.ShouldBe(ImageLoadState.Idle);

            previewer.ReloadCover();

            WaitUntil(
                () => thumbnailReads == 2 && previewer.CoverLoadState == ImageLoadState.Loaded,
                "cover reload");
            entry.FullState.ShouldBe(ImageLoadState.Idle);
            entry.FullImage.ShouldBeNull();
        });
    }

    [Fact]
    public void Dialog_Close_Releases_Full_Leases_But_Keeps_Cover_Lease()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var full = new TestImage();
            var thumbnail = new TestImage();
            var previewer = new ClosableImagePreviewer
            {
                Width = 96,
                Height = 96,
                ItemsSource =
                [
                    new ImagePreviewItem(ImageLoadSource.FromImage(full))
                    {
                        ThumbnailSource = ImageLoadSource.FromImage(thumbnail)
                    }
                ]
            };
            using var host = new PreviewerHost(previewer);
            WaitUntil(() => previewer.IsCoverLoaded, "cover load");
            previewer.OpenDialog();
            WaitUntil(() => previewer.IsCurrentLoaded, "full preview load");
            var entry = previewer.EffectiveItems.ShouldNotBeNull().Single();

            previewer.CloseForTest();

            entry.FullImage.ShouldBeNull();
            entry.FullState.ShouldBe(ImageLoadState.Idle);
            entry.ThumbnailImage.ShouldBeSameAs(thumbnail);
            entry.ThumbnailState.ShouldBe(ImageLoadState.Loaded);
        });
    }

    [Fact]
    public void Scheduler_Starts_Current_Then_Cover_Then_Neighbor_When_The_Read_Slot_Opens()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var blockerStarted = NewSignal();
            var releaseBlocker = NewSignal();
            var criticalStarted = NewSignal();
            var releaseCritical = NewSignal();
            var highStarted = NewSignal();
            var releaseHigh = NewSignal();
            var preloadStarted = NewSignal();
            var releasePreload = NewSignal();
            var loader = Application.Current.ShouldNotBeNull().GetImageLoader();
            var blockerTask = loader.LoadAsync(new ImageLoadRequest(CreateGatedSource(
                "blocker",
                blockerStarted,
                releaseBlocker))).AsTask();
            WaitUntil(() => blockerStarted.Task.IsCompleted, "scheduler blocker");

            var current = new ImagePreviewItem(CreateGatedSource(
                "critical",
                criticalStarted,
                releaseCritical))
            {
                ThumbnailSource = CreateGatedSource("high", highStarted, releaseHigh)
            };
            var neighbor = new ImagePreviewItem(CreateGatedSource(
                "preload",
                preloadStarted,
                releasePreload));
            var previewer = CreatePreviewer(current, neighbor);
            previewer.PreloadCount = 1;
            using var host = new PreviewerHost(previewer);
            WaitUntil(() => previewer.CoverLoadState == ImageLoadState.Loading, "queued cover load");
            previewer.RequestPreviewLoads();

            try
            {
                releaseBlocker.TrySetResult();
                WaitUntil(() => criticalStarted.Task.IsCompleted, "critical request start");
                highStarted.Task.IsCompleted.ShouldBeFalse();
                preloadStarted.Task.IsCompleted.ShouldBeFalse();

                releaseCritical.TrySetResult();
                WaitUntil(() => highStarted.Task.IsCompleted, "high request start");
                preloadStarted.Task.IsCompleted.ShouldBeFalse();

                releaseHigh.TrySetResult();
                WaitUntil(() => preloadStarted.Task.IsCompleted, "preload request start");
            }
            finally
            {
                releaseBlocker.TrySetResult();
                releaseCritical.TrySetResult();
                releaseHigh.TrySetResult();
                releasePreload.TrySetResult();
            }

            WaitUntil(() => blockerTask.IsCompleted, "blocker completion");
            blockerTask.GetAwaiter().GetResult().Dispose();
        });
    }

    private static global::AtomUI.Desktop.Controls.ImagePreviewer CreatePreviewer(
        params ImagePreviewItem[] items)
    {
        return CreatePreviewer((IEnumerable<ImagePreviewItem>)items);
    }

    private static global::AtomUI.Desktop.Controls.ImagePreviewer CreatePreviewer(
        IEnumerable<ImagePreviewItem> items)
    {
        return new global::AtomUI.Desktop.Controls.ImagePreviewer
        {
            Width = 96,
            Height = 96,
            PreloadCount = 0,
            ItemsSource = items
        };
    }

    private static ImagePreviewItem CreateItem(string key)
    {
        return new ImagePreviewItem(ImageLoadSource.FromImage(new TestImage(), key));
    }

    private static ImageLoadSource CreateFailingStreamSource(string key, Action onOpen)
    {
        return ImageLoadSource.FromStream(
            _ =>
            {
                onOpen();
                return ValueTask.FromResult<Stream>(new MemoryStream(new byte[] { 1 }));
            },
            key,
            "v1");
    }

    private static ImageLoadSource CreateGatedSource(
        string key,
        TaskCompletionSource started,
        TaskCompletionSource release)
    {
        return ImageLoadSource.FromStream(
            async token =>
            {
                started.TrySetResult();
                await release.Task.WaitAsync(token);
                return new MemoryStream(new byte[] { 1 });
            },
            key,
            "v1");
    }

    private static TaskCompletionSource NewSignal() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static PixelSize GetThumbnailPixelSize(ImagePreviewEntry entry)
    {
        return entry.ThumbnailImage.ShouldBeOfType<Bitmap>().PixelSize;
    }

    private static PixelSize? GetThumbnailResultRequestSize(ImagePreviewEntry entry)
    {
        return (PixelSize?)typeof(ImagePreviewEntry)
            .GetField(
                "_thumbnailResultRequestSize",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .GetValue(entry);
    }

    private static byte[] CreatePng(int width, int height)
    {
        using var output = new MemoryStream();
        output.Write([0x89, (byte)'P', (byte)'N', (byte)'G', 0x0d, 0x0a, 0x1a, 0x0a]);

        Span<byte> header = stackalloc byte[13];
        BinaryPrimitives.WriteUInt32BigEndian(header[..4], checked((uint)width));
        BinaryPrimitives.WriteUInt32BigEndian(header[4..8], checked((uint)height));
        header[8] = 8;
        header[9] = 6;
        WriteChunk(output, "IHDR", header);

        using var raw = new MemoryStream();
        var row = new byte[checked(width * 4 + 1)];
        for (var y = 0; y < height; y++)
        {
            raw.Write(row);
        }
        raw.Position = 0;
        using var compressed = new MemoryStream();
        using (var zlib = new ZLibStream(compressed, CompressionLevel.SmallestSize, leaveOpen: true))
        {
            raw.CopyTo(zlib);
        }
        WriteChunk(output, "IDAT", compressed.ToArray());
        WriteChunk(output, "IEND", []);
        return output.ToArray();
    }

    private static void WriteChunk(Stream output, string type, ReadOnlySpan<byte> data)
    {
        Span<byte> length = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(length, checked((uint)data.Length));
        output.Write(length);
        var typeBytes = Encoding.ASCII.GetBytes(type);
        output.Write(typeBytes);
        output.Write(data);

        var crcBytes = new byte[typeBytes.Length + data.Length];
        typeBytes.CopyTo(crcBytes, 0);
        data.CopyTo(crcBytes.AsSpan(typeBytes.Length));
        Span<byte> crc = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(crc, CalculateCrc32(crcBytes));
        output.Write(crc);
    }

    private static uint CalculateCrc32(ReadOnlySpan<byte> bytes)
    {
        var crc = uint.MaxValue;
        foreach (var value in bytes)
        {
            crc ^= value;
            for (var bit = 0; bit < 8; bit++)
            {
                crc = (crc & 1) != 0 ? 0xedb88320u ^ (crc >> 1) : crc >> 1;
            }
        }
        return ~crc;
    }

    private static void WaitUntil(Func<bool> predicate, string description)
    {
        var timeout = System.Diagnostics.Stopwatch.StartNew();
        while (!predicate())
        {
            Dispatcher.UIThread.RunJobs();
            if (timeout.Elapsed >= TimeSpan.FromSeconds(5))
            {
                throw new TimeoutException($"Timed out waiting for {description}.");
            }
            Thread.Yield();
        }
        Dispatcher.UIThread.RunJobs();
    }

    private sealed class PreviewerHost : IDisposable
    {
        private readonly Avalonia.Controls.Window _window;

        internal PreviewerHost(Control previewer, double? renderScaling = null)
        {
            _window = new Avalonia.Controls.Window
            {
                Width = 320,
                Height = 240,
                Content = previewer
            };
            _window.Show();
            if (renderScaling is not null)
            {
                _window.SetRenderScaling(renderScaling.Value);
                _window.UpdateLayout();
            }
            Dispatcher.UIThread.RunJobs();
        }

        internal void Detach()
        {
            _window.Content = null;
            Dispatcher.UIThread.RunJobs();
        }

        internal void Attach(Control previewer)
        {
            _window.Content = previewer;
            Dispatcher.UIThread.RunJobs();
        }

        public void Dispose()
        {
            _window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private sealed class TestPreviewer : AbstractImagePreviewer
    {
    }

    private sealed class ClosableImagePreviewer : global::AtomUI.Desktop.Controls.ImagePreviewer
    {
        internal void CloseForTest()
        {
            CloseDialog();
        }
    }

    private sealed class ResettableCollection<T> : ObservableCollection<T>
    {
        internal ResettableCollection(IEnumerable<T> items)
            : base(items)
        {
        }

        internal void ResetWith(IEnumerable<T> items)
        {
            Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }

    private sealed class TestImage : IImage
    {
        public Size Size => new(24, 24);

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
        }
    }
}
