using System.Buffers.Binary;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls.Shared.Tests.ImageLoading;

internal static class ImageLoadingTestSupport
{
    internal static ImageLoadingOptions CreateOptions(Action<ImageLoadingOptionsBuilder>? configure = null)
    {
        var builder = new ImageLoadingOptionsBuilder
        {
            IsPersistentCacheEnabled = false
        };
        configure?.Invoke(builder);
        return builder.Build("AtomUI.Controls.Shared.Tests");
    }

    internal static byte[] CreatePngHeader(int width = 2, int height = 3, bool animated = false)
    {
        var bytes = new byte[animated ? 32 : 24];
        new byte[] { 0x89, (byte)'P', (byte)'N', (byte)'G', 0x0d, 0x0a, 0x1a, 0x0a }
            .CopyTo(bytes, 0);
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(16, 4), checked((uint)width));
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(20, 4), checked((uint)height));
        if (animated)
        {
            "acTL"u8.CopyTo(bytes.AsSpan(24));
        }
        return bytes;
    }

    internal static ImageEncodedContent CreateContent(
        byte[] bytes,
        string? mediaType = "image/png",
        ImageCacheSource cacheSource = ImageCacheSource.Local,
        bool noStore = false)
    {
        return new ImageEncodedContent(
            bytes,
            mediaType,
            cacheSource,
            DateTimeOffset.UtcNow,
            NoStore: noStore);
    }
}

internal sealed class TestImage : IImage, IDisposable
{
    private int _disposeCount;

    internal TestImage(double width = 10, double height = 10)
    {
        Size = new Size(width, height);
    }

    public Size Size { get; }

    internal int DisposeCount => Volatile.Read(ref _disposeCount);

    public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
    {
    }

    public void Dispose()
    {
        Interlocked.Increment(ref _disposeCount);
    }
}
