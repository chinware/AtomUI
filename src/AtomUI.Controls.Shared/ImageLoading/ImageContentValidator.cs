using System.Buffers.Binary;
using System.Text;

namespace AtomUI.Controls;

internal enum ImageContentFormat
{
    Png,
    Jpeg,
    WebP,
    Bmp,
    Gif,
    Svg
}

internal readonly record struct ImageProbeResult(
    ImageContentFormat Format,
    string MediaType,
    int PixelWidth,
    int PixelHeight,
    bool IsAnimated);

internal sealed class ImageContentValidator
{
    private readonly ImageLoadingOptions _options;

    internal ImageContentValidator(ImageLoadingOptions options)
    {
        _options = options;
    }

    internal ImageProbeResult Validate(ImageEncodedContent content, ImageLoadSource source)
    {
        var bytes = content.Bytes.AsSpan();
        if (bytes.IsEmpty)
        {
            throw Failure(ImageLoadErrorCode.InvalidImageData, "Image content is empty.", source);
        }

        ImageProbeResult probe;
        if (IsPng(bytes))
        {
            probe = ProbePng(bytes, source);
        }
        else if (IsJpeg(bytes))
        {
            probe = ProbeJpeg(bytes, source);
        }
        else if (IsGif(bytes))
        {
            probe = ProbeGif(bytes, source);
        }
        else if (IsBmp(bytes))
        {
            probe = ProbeBmp(bytes, source);
        }
        else if (IsWebP(bytes))
        {
            probe = ProbeWebP(bytes, source);
        }
        else if (LooksLikeMarkup(bytes, out var isSvg))
        {
            if (!isSvg || !content.IsTrustedAsset || source.Kind != ImageLoadSourceKind.Asset)
            {
                throw Failure(
                    ImageLoadErrorCode.UnsafeVectorContent,
                    "Remote and untrusted vector or markup image content is not allowed.",
                    source);
            }
            probe = new ImageProbeResult(ImageContentFormat.Svg, "image/svg+xml", 0, 0, false);
        }
        else
        {
            throw Failure(ImageLoadErrorCode.UnsupportedFormat, "Image format is not supported.", source);
        }

        ValidateMediaType(content.MediaType, probe, source);
        ValidateDimensions(probe, source);
        return probe;
    }

    private void ValidateDimensions(ImageProbeResult probe, ImageLoadSource source)
    {
        if (probe.Format == ImageContentFormat.Svg)
        {
            return;
        }
        if (probe.PixelWidth <= 0 || probe.PixelHeight <= 0)
        {
            throw Failure(ImageLoadErrorCode.InvalidImageData, "Image dimensions are invalid.", source);
        }
        if (probe.PixelWidth > _options.MaxImageWidth || probe.PixelHeight > _options.MaxImageHeight)
        {
            throw Failure(
                ImageLoadErrorCode.DimensionLimitExceeded,
                "Image dimensions exceed the configured limit.",
                source);
        }
        long pixels;
        long decodedBytes;
        try
        {
            pixels = checked((long)probe.PixelWidth * probe.PixelHeight);
            decodedBytes = checked(pixels * 4);
        }
        catch (OverflowException)
        {
            throw Failure(ImageLoadErrorCode.PixelLimitExceeded, "Image dimensions are too large.", source);
        }
        if (pixels > _options.MaxImagePixelCount)
        {
            throw Failure(ImageLoadErrorCode.PixelLimitExceeded, "Image pixel count exceeds the configured limit.", source);
        }
        if (decodedBytes > _options.MaxDecodedImageBytes)
        {
            throw Failure(
                ImageLoadErrorCode.DecodedByteLimitExceeded,
                "Decoded image size exceeds the configured limit.",
                source);
        }
        if (probe.IsAnimated)
        {
            throw Failure(ImageLoadErrorCode.AnimationNotSupported, "Animated images are not supported.", source);
        }
    }

    private static void ValidateMediaType(
        string? declaredMediaType,
        ImageProbeResult probe,
        ImageLoadSource source)
    {
        if (string.IsNullOrWhiteSpace(declaredMediaType) ||
            declaredMediaType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }
        if (declaredMediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase) ||
            declaredMediaType is "application/xml" or "image/svg+xml")
        {
            if (probe.Format != ImageContentFormat.Svg || source.Kind != ImageLoadSourceKind.Asset)
            {
                throw Failure(ImageLoadErrorCode.UnsafeVectorContent, "Unsafe image content type was rejected.", source);
            }
        }
        if (!declaredMediaType.Equals(probe.MediaType, StringComparison.OrdinalIgnoreCase))
        {
            throw Failure(
                ImageLoadErrorCode.ContentTypeMismatch,
                "Image content does not match its declared content type.",
                source);
        }
    }

    private static ImageProbeResult ProbePng(ReadOnlySpan<byte> bytes, ImageLoadSource source)
    {
        if (bytes.Length < 24)
        {
            throw Failure(ImageLoadErrorCode.InvalidImageData, "PNG header is incomplete.", source);
        }
        var width = checked((int)BinaryPrimitives.ReadUInt32BigEndian(bytes[16..20]));
        var height = checked((int)BinaryPrimitives.ReadUInt32BigEndian(bytes[20..24]));
        var animated = IndexOf(bytes, "acTL"u8) >= 0;
        return new ImageProbeResult(ImageContentFormat.Png, "image/png", width, height, animated);
    }

    private static ImageProbeResult ProbeJpeg(ReadOnlySpan<byte> bytes, ImageLoadSource source)
    {
        var offset = 2;
        while (offset + 4 <= bytes.Length)
        {
            if (bytes[offset] != 0xff)
            {
                offset++;
                continue;
            }
            while (offset < bytes.Length && bytes[offset] == 0xff)
            {
                offset++;
            }
            if (offset >= bytes.Length)
            {
                break;
            }
            var marker = bytes[offset++];
            if (marker is 0xd8 or 0xd9)
            {
                continue;
            }
            if (offset + 2 > bytes.Length)
            {
                break;
            }
            var segmentLength = BinaryPrimitives.ReadUInt16BigEndian(bytes[offset..]);
            if (segmentLength < 2 || offset + segmentLength > bytes.Length)
            {
                break;
            }
            if (marker is >= 0xc0 and <= 0xc3 or >= 0xc5 and <= 0xc7 or >= 0xc9 and <= 0xcb or >= 0xcd and <= 0xcf)
            {
                if (segmentLength < 7)
                {
                    break;
                }
                var height = BinaryPrimitives.ReadUInt16BigEndian(bytes[(offset + 3)..]);
                var width = BinaryPrimitives.ReadUInt16BigEndian(bytes[(offset + 5)..]);
                return new ImageProbeResult(ImageContentFormat.Jpeg, "image/jpeg", width, height, false);
            }
            offset += segmentLength;
        }
        throw Failure(ImageLoadErrorCode.InvalidImageData, "JPEG dimensions could not be read.", source);
    }

    private static ImageProbeResult ProbeGif(ReadOnlySpan<byte> bytes, ImageLoadSource source)
    {
        if (bytes.Length < 10)
        {
            throw Failure(ImageLoadErrorCode.InvalidImageData, "GIF header is incomplete.", source);
        }
        var width = BinaryPrimitives.ReadUInt16LittleEndian(bytes[6..8]);
        var height = BinaryPrimitives.ReadUInt16LittleEndian(bytes[8..10]);
        var frameCount = 0;
        for (var index = 10; index < bytes.Length; index++)
        {
            if (bytes[index] == 0x2c && ++frameCount > 1)
            {
                break;
            }
        }
        return new ImageProbeResult(ImageContentFormat.Gif, "image/gif", width, height, frameCount > 1);
    }

    private static ImageProbeResult ProbeBmp(ReadOnlySpan<byte> bytes, ImageLoadSource source)
    {
        if (bytes.Length < 26)
        {
            throw Failure(ImageLoadErrorCode.InvalidImageData, "BMP header is incomplete.", source);
        }
        var width = Math.Abs(BinaryPrimitives.ReadInt32LittleEndian(bytes[18..22]));
        var height = Math.Abs(BinaryPrimitives.ReadInt32LittleEndian(bytes[22..26]));
        return new ImageProbeResult(ImageContentFormat.Bmp, "image/bmp", width, height, false);
    }

    private static ImageProbeResult ProbeWebP(ReadOnlySpan<byte> bytes, ImageLoadSource source)
    {
        if (bytes.Length < 30)
        {
            throw Failure(ImageLoadErrorCode.InvalidImageData, "WebP header is incomplete.", source);
        }
        var chunk = Encoding.ASCII.GetString(bytes[12..16]);
        int width;
        int height;
        var animated = false;
        switch (chunk)
        {
            case "VP8X":
                animated = (bytes[20] & 0x02) != 0;
                width = 1 + ReadUInt24LittleEndian(bytes[24..27]);
                height = 1 + ReadUInt24LittleEndian(bytes[27..30]);
                break;
            case "VP8 ":
                if (bytes.Length < 30 || bytes[23] != 0x9d || bytes[24] != 0x01 || bytes[25] != 0x2a)
                {
                    throw Failure(ImageLoadErrorCode.InvalidImageData, "WebP VP8 header is invalid.", source);
                }
                width = BinaryPrimitives.ReadUInt16LittleEndian(bytes[26..28]) & 0x3fff;
                height = BinaryPrimitives.ReadUInt16LittleEndian(bytes[28..30]) & 0x3fff;
                break;
            case "VP8L":
                if (bytes[20] != 0x2f)
                {
                    throw Failure(ImageLoadErrorCode.InvalidImageData, "WebP VP8L header is invalid.", source);
                }
                var bits = BinaryPrimitives.ReadUInt32LittleEndian(bytes[21..25]);
                width = (int)(bits & 0x3fff) + 1;
                height = (int)((bits >> 14) & 0x3fff) + 1;
                break;
            default:
                throw Failure(ImageLoadErrorCode.InvalidImageData, "WebP format is invalid.", source);
        }
        return new ImageProbeResult(ImageContentFormat.WebP, "image/webp", width, height, animated);
    }

    private static bool LooksLikeMarkup(ReadOnlySpan<byte> bytes, out bool isSvg)
    {
        var prefixLength = Math.Min(bytes.Length, 4096);
        var text = Encoding.UTF8.GetString(bytes[..prefixLength]).TrimStart('\ufeff', ' ', '\t', '\r', '\n');
        isSvg = text.StartsWith("<svg", StringComparison.OrdinalIgnoreCase) ||
            (text.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase) &&
             text.Contains("<svg", StringComparison.OrdinalIgnoreCase));
        return isSvg || text.StartsWith("<", StringComparison.Ordinal);
    }

    private static int IndexOf(ReadOnlySpan<byte> value, ReadOnlySpan<byte> pattern)
    {
        return value.IndexOf(pattern);
    }

    private static int ReadUInt24LittleEndian(ReadOnlySpan<byte> value)
    {
        return value[0] | value[1] << 8 | value[2] << 16;
    }

    private static bool IsPng(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 8 &&
        bytes[0] == 0x89 &&
        bytes[1] == (byte)'P' &&
        bytes[2] == (byte)'N' &&
        bytes[3] == (byte)'G' &&
        bytes[4] == 0x0d &&
        bytes[5] == 0x0a &&
        bytes[6] == 0x1a &&
        bytes[7] == 0x0a;

    private static bool IsJpeg(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 3 && bytes[0] == 0xff && bytes[1] == 0xd8 && bytes[2] == 0xff;

    private static bool IsGif(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 6 &&
        (bytes[..6].SequenceEqual("GIF87a"u8) || bytes[..6].SequenceEqual("GIF89a"u8));

    private static bool IsBmp(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 2 && bytes[0] == (byte)'B' && bytes[1] == (byte)'M';

    private static bool IsWebP(ReadOnlySpan<byte> bytes) =>
        bytes.Length >= 12 && bytes[..4].SequenceEqual("RIFF"u8) && bytes[8..12].SequenceEqual("WEBP"u8);

    private static ImageLoadFailureException Failure(
        ImageLoadErrorCode code,
        string message,
        ImageLoadSource source)
    {
        return ImageSourceReadHelpers.Failure(code, message, source.DisplayName);
    }
}
