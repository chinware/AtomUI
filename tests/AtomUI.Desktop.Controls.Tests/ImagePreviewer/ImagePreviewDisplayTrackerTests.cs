using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using System.Text;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImagePreviewDisplayTrackerTests
{
    public ImagePreviewDisplayTrackerTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void WaitForLoaded_Keeps_The_Previous_Image_While_The_Target_Is_Loading()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var first = NewLoadedEntry("first");
            var secondStarted = NewSignal();
            var releaseSecond = NewSignal();
            var second = NewGatedEntry("second", secondStarted, releaseSecond);
            var mode = ImageSwitchMode.WaitForLoaded;
            using var tracker = NewTracker(() => mode);

            tracker.SetCurrentItem(first);
            WaitUntil(() => ReferenceEquals(tracker.EffectiveImage, first.FullImage), "first display");

            tracker.SetCurrentItem(second);
            StartGatedLoad(second);
            WaitUntil(() => secondStarted.Task.IsCompleted, "second load start");
            Dispatcher.UIThread.RunJobs();
            tracker.EffectiveImage.ShouldBeSameAs(first.FullImage);
            tracker.IsCurrentLoading.ShouldBeTrue();
            tracker.IsCurrentFailed.ShouldBeFalse();

            releaseSecond.TrySetResult();
            WaitUntil(() => ReferenceEquals(tracker.EffectiveImage, second.FullImage), "display swap after load");
            tracker.IsCurrentLoading.ShouldBeFalse();

            second.Dispose();
            first.Dispose();
        });
    }

    [Fact]
    public void Immediate_Holds_The_Previous_Image_Within_Grace_Then_Falls_Back_To_Placeholder()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var first = NewLoadedEntry("first");
            var secondStarted = NewSignal();
            var releaseSecond = NewSignal();
            var second = NewGatedEntry("second", secondStarted, releaseSecond);
            var mode = ImageSwitchMode.Immediate;
            using var tracker = NewTracker(() => mode);

            tracker.SetCurrentItem(first);
            WaitUntil(() => ReferenceEquals(tracker.EffectiveImage, first.FullImage), "first display");

            tracker.SetCurrentItem(second);
            StartGatedLoad(second);
            WaitUntil(() => secondStarted.Task.IsCompleted, "second load start");
            Dispatcher.UIThread.RunJobs();
            tracker.EffectiveImage.ShouldBeSameAs(first.FullImage); // 宽限期内保留旧图：无空白帧

            // 宽限期（300ms）届满仍未就绪：Immediate 回退加载占位
            WaitUntil(() => tracker.EffectiveImage is null, "placeholder after grace");
            tracker.IsCurrentLoading.ShouldBeTrue();

            releaseSecond.TrySetResult();
            WaitUntil(() => ReferenceEquals(tracker.EffectiveImage, second.FullImage), "display after load");

            second.Dispose();
            first.Dispose();
        });
    }

    [Fact]
    public void WaitForLoaded_Drops_The_Held_Image_When_The_Target_Fails()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var first = NewLoadedEntry("first");
            var failing = new ImagePreviewEntry(new ImagePreviewItem(ImageLoadSource.FromBytes(
                new byte[] { 1 }, $"tracker-fail-{Guid.NewGuid():N}", "v1")));
            var mode = ImageSwitchMode.WaitForLoaded;
            using var tracker = NewTracker(() => mode);

            tracker.SetCurrentItem(first);
            WaitUntil(() => ReferenceEquals(tracker.EffectiveImage, first.FullImage), "first display");

            tracker.SetCurrentItem(failing);
            failing.LoadFull(16, 16, ImageRequestPriority.Critical);
            WaitUntil(() => tracker.IsCurrentFailed && tracker.EffectiveImage is null, "failed display");

            failing.Dispose();
            first.Dispose();
        });
    }

    [Fact]
    public void Disposing_The_Retained_Source_Clears_The_Held_Image()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var first = NewLoadedEntry("first");
            var secondStarted = NewSignal();
            var releaseSecond = NewSignal();
            var second = NewGatedEntry("second", secondStarted, releaseSecond);
            var mode = ImageSwitchMode.WaitForLoaded;
            using var tracker = NewTracker(() => mode);

            tracker.SetCurrentItem(first);
            WaitUntil(() => ReferenceEquals(tracker.EffectiveImage, first.FullImage), "first display");
            tracker.SetCurrentItem(second);
            StartGatedLoad(second);
            WaitUntil(() => secondStarted.Task.IsCompleted && tracker.EffectiveImage is not null,
                "held display");
            tracker.EffectiveImage.ShouldBeSameAs(first.FullImage);

            first.Dispose(); // 模拟集合移除保留帧源

            WaitUntil(() => tracker.EffectiveImage is null, "held image cleared after dispose");
            tracker.IsCurrentLoading.ShouldBeTrue();

            releaseSecond.TrySetResult();
            second.Dispose();
        });
    }

    [Fact]
    public void Clear_Drops_Everything_And_Unsubscribes_All_Entries()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var notified = false;
            var secondStarted = NewSignal();
            var releaseSecond = NewSignal();
            var first = NewLoadedEntry("first");
            var second = NewGatedEntry("second", secondStarted, releaseSecond);
            var mode = ImageSwitchMode.WaitForLoaded;
            using var tracker = new ImagePreviewDisplayTracker(() => mode, () => notified = true);

            tracker.SetCurrentItem(first);
            WaitUntil(() => tracker.EffectiveImage is not null, "first display");
            tracker.SetCurrentItem(second);
            StartGatedLoad(second);
            WaitUntil(() => secondStarted.Task.IsCompleted, "second load start");

            tracker.Clear();

            tracker.EffectiveImage.ShouldBeNull();
            tracker.IsCurrentLoading.ShouldBeFalse();
            tracker.IsCurrentFailed.ShouldBeFalse();
            notified.ShouldBeTrue(); // Clear 归零显示，回调按设计触发一次
            notified = false;

            releaseSecond.TrySetResult();
            WaitUntil(() => second.FullState == ImageLoadState.Loaded, "second load completes unobserved");
            Dispatcher.UIThread.RunJobs();
            notified.ShouldBeFalse(); // 退订生效：后续事件不再触达 tracker
            tracker.EffectiveImage.ShouldBeNull();

            second.Dispose();
            first.Dispose();
        });
    }

    [Fact]
    public void Switching_Current_Unsubscribes_The_Previous_Current_Entry()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var notified = false;
            var secondStarted = NewSignal();
            var releaseSecond = NewSignal();
            var first = NewLoadedEntry("first");
            var second = NewGatedEntry("second", secondStarted, releaseSecond);
            var third = NewLoadedEntry("third");
            var mode = ImageSwitchMode.Immediate;
            using var tracker = new ImagePreviewDisplayTracker(() => mode, () => notified = true);

            tracker.SetCurrentItem(second);
            StartGatedLoad(second);
            WaitUntil(() => secondStarted.Task.IsCompleted, "second load start");
            tracker.SetCurrentItem(third);
            WaitUntil(() => tracker.EffectiveImage is not null, "third display");
            notified = false;

            releaseSecond.TrySetResult(); // 已被切走的 second 完成，不应触达 tracker
            WaitUntil(() => second.FullState == ImageLoadState.Loaded, "second load completes unobserved");
            Dispatcher.UIThread.RunJobs();
            notified.ShouldBeFalse();
            tracker.EffectiveImage.ShouldBeSameAs(third.FullImage);

            second.Dispose();
            third.Dispose();
            first.Dispose();
        });
    }

    [Fact]
    public void Immediate_Mode_Retained_Bound_Within_Grace()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            // 统一显示连续性后 Immediate 在宽限期内也持有保留帧，
            // 上界仍为 1 且只在有可用图时持有
            var first = NewLoadedEntry("first");
            var second = NewLoadedEntry("second");
            var mode = ImageSwitchMode.Immediate;
            using var tracker = NewTracker(() => mode);

            tracker.SetCurrentItem(first);
            WaitUntil(() => tracker.EffectiveImage is not null, "first display");
            tracker.SetCurrentItem(second);
            WaitUntil(() => ReferenceEquals(tracker.EffectiveImage, second.FullImage), "second display");

            var retained = GetRetainedItem(tracker);
            if (retained is not null)
            {
                retained.FullImage.ShouldNotBeNull();
            }

            first.Dispose();
            second.Dispose();
        });
    }

    [Fact]
    public void Retained_Item_Invariant_Holds_Across_Switches()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var first = NewLoadedEntry("first");
            var secondStarted = NewSignal();
            var releaseSecond = NewSignal();
            var second = NewGatedEntry("second", secondStarted, releaseSecond);
            var mode = ImageSwitchMode.WaitForLoaded;
            using var tracker = NewTracker(() => mode);

            tracker.SetCurrentItem(first);
            WaitUntil(() => tracker.EffectiveImage is not null, "first display");
            tracker.SetCurrentItem(second);
            StartGatedLoad(second);
            WaitUntil(() => secondStarted.Task.IsCompleted, "second load start");

            var retained = GetRetainedItem(tracker).ShouldNotBeNull();
            retained.ShouldBeSameAs(first);
            retained.FullImage.ShouldNotBeNull(); // 不变量：retained 恒持有可用图

            releaseSecond.TrySetResult();
            WaitUntil(() => ReferenceEquals(tracker.EffectiveImage, second.FullImage), "display swap");
            GetRetainedItem(tracker).ShouldBeSameAs(second);

            second.Dispose();
            first.Dispose();
        });
    }

    [Fact]
    public void WaitForLoaded_Holds_The_Retained_Image_During_The_Idle_Transient()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var first = NewLoadedEntry("first");
            // 目标项尚处 Idle（LoadFull 未调用）：切换瞬间的重算不得产生显示空洞
            var pending = new ImagePreviewEntry(
                new ImagePreviewItem(ImageLoadSource.FromBytes(
                    new byte[] { 1 }, $"tracker-idle-{Guid.NewGuid():N}", "v1")));
            var mode = ImageSwitchMode.WaitForLoaded;
            using var tracker = NewTracker(() => mode);

            tracker.SetCurrentItem(first);
            WaitUntil(() => ReferenceEquals(tracker.EffectiveImage, first.FullImage), "first display");

            tracker.SetCurrentItem(pending); // Idle：无图、未加载、未失败

            tracker.EffectiveImage.ShouldBeSameAs(first.FullImage);

            pending.Dispose();
            first.Dispose();
        });
    }

    [Fact]
    public void WaitForLoaded_Pulls_The_Retained_Seed_When_The_Current_Never_Completes()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var seed = NewLoadedEntry("seed");
            var pending = new ImagePreviewEntry(
                new ImagePreviewItem(ImageLoadSource.FromBytes(
                    new byte[] { 1 }, $"tracker-seed-{Guid.NewGuid():N}", "v1")));
            var mode = ImageSwitchMode.WaitForLoaded;
            using var tracker = new ImagePreviewDisplayTracker(
                () => mode,
                () => { },
                () => seed);

            tracker.SetCurrentItem(pending); // 无图目标：从种子补充保留帧

            tracker.EffectiveImage.ShouldBeSameAs(seed.FullImage);

            seed.Dispose();
            WaitUntil(() => tracker.EffectiveImage is null, "seed invalidated clears display");

            pending.Dispose();
        });
    }

    private static ImagePreviewDisplayTracker NewTracker(Func<ImageSwitchMode> mode)
    {
        return new ImagePreviewDisplayTracker(mode, () => { });
    }

    private static ImagePreviewEntry NewLoadedEntry(string key)
    {
        var entry = new ImagePreviewEntry(
            new ImagePreviewItem(ImageLoadSource.FromImage(new TestImage(), key)));
        // tracker 不发起加载，测试显式驱动（与既有 entry 级测试一致）
        entry.LoadFull(16, 16, ImageRequestPriority.Critical);
        WaitUntil(() => entry.FullState == ImageLoadState.Loaded, $"{key} load");
        return entry;
    }

    private static ImagePreviewEntry NewGatedEntry(
        string key,
        TaskCompletionSource started,
        TaskCompletionSource release)
    {
        return new ImagePreviewEntry(new ImagePreviewItem(ImageLoadSource.FromStream(
            async token =>
            {
                started.TrySetResult();
                await release.Task.WaitAsync(token);
                return new MemoryStream(CreatePng(16, 16));
            },
            $"tracker-{key}-{Guid.NewGuid():N}",
            "v1")));
    }

    private static void StartGatedLoad(ImagePreviewEntry entry)
    {
        entry.LoadFull(16, 16, ImageRequestPriority.Critical);
    }

    private static ImagePreviewEntry? GetRetainedItem(ImagePreviewDisplayTracker tracker)
    {
        return (ImagePreviewEntry?)typeof(ImagePreviewDisplayTracker)
            .GetField(
                "_retainedItem",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .GetValue(tracker);
    }

    private static TaskCompletionSource NewSignal() =>
        new(TaskCreationOptions.RunContinuationsAsynchronously);

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

    private static uint CalculateCrc32(byte[] bytes)
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

    private sealed class TestImage : IImage
    {
        public Size Size => new(24, 24);

        public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
        {
        }
    }
}
