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
        var bytes = new byte[animated ? 65 : 45];
        new byte[] { 0x89, (byte)'P', (byte)'N', (byte)'G', 0x0d, 0x0a, 0x1a, 0x0a }
            .CopyTo(bytes, 0);
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(8, 4), 13);
        "IHDR"u8.CopyTo(bytes.AsSpan(12));
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(16, 4), checked((uint)width));
        BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(20, 4), checked((uint)height));
        bytes[24] = 8;
        bytes[25] = 6;
        bytes[26] = 0;
        bytes[27] = 0;
        bytes[28] = 0;
        if (animated)
        {
            BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(33, 4), 8);
            "acTL"u8.CopyTo(bytes.AsSpan(37));
            BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(41, 4), 1);
            BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan(45, 4), 0);
            "IEND"u8.CopyTo(bytes.AsSpan(57));
        }
        else
        {
            "IEND"u8.CopyTo(bytes.AsSpan(37));
        }
        return bytes;
    }

    internal static ImageEncodedContent CreateContent(
        byte[] bytes,
        string? mediaType = "image/png",
        ImageLoadOrigin cacheSource = ImageLoadOrigin.Local,
        bool noStore = false)
    {
        return new ImageEncodedContent(
            bytes,
            mediaType,
            cacheSource,
            DateTimeOffset.UtcNow,
            NoStore: noStore,
            SecurityPolicyVersion: ImageSecurityPolicy.Version);
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
