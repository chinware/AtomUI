namespace AtomUI.Controls;

internal readonly record struct SvgDataImageValidationResult(
    long EncodedBytes,
    long DecodedBytes);

internal sealed class SvgDataImageValidator
{
    private static readonly HashSet<string> s_allowedMediaTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/png",
        "image/jpeg",
        "image/webp"
    };

    private readonly Func<byte[], string, ImageLoadSource, ImageProbeResult> _rasterValidator;

    internal SvgDataImageValidator(
        Func<byte[], string, ImageLoadSource, ImageProbeResult> rasterValidator)
    {
        _rasterValidator = rasterValidator;
    }

    internal SvgDataImageValidationResult Validate(
        string value,
        long remainingEncodedBytes,
        ImageLoadSource source)
    {
        if (!value.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            throw Unsafe("SVG image references must stay in the same document or use an allowed data image.", source);
        }

        var commaIndex = value.IndexOf(',');
        if (commaIndex <= 5)
        {
            throw Invalid("SVG data image URI is invalid.", source);
        }

        var metadata = value.AsSpan(5, commaIndex - 5);
        var payload = value.AsSpan(commaIndex + 1);
        var separatorIndex = metadata.IndexOf(';');
        var mediaType = (separatorIndex < 0 ? metadata : metadata[..separatorIndex]).ToString().Trim();
        if (!s_allowedMediaTypes.Contains(mediaType))
        {
            throw Unsafe("SVG data image media type is not allowed.", source);
        }

        var isBase64 = false;
        if (separatorIndex >= 0)
        {
            var parameters = metadata[(separatorIndex + 1)..];
            if (!parameters.Equals("base64", StringComparison.OrdinalIgnoreCase))
            {
                throw Invalid("SVG data image parameters are invalid.", source);
            }
            isBase64 = true;
        }

        var bytes = isBase64
            ? DecodeBase64(payload, remainingEncodedBytes, source)
            : DecodePercentEncoded(payload, remainingEncodedBytes, source);
        var probe = _rasterValidator(bytes, mediaType, source);
        long decodedBytes;
        try
        {
            decodedBytes = checked((long)probe.PixelWidth * probe.PixelHeight * 4);
        }
        catch (OverflowException)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.DecodedByteLimitExceeded,
                "SVG embedded image decoded size is too large.",
                source.DisplayName);
        }
        return new SvgDataImageValidationResult(bytes.LongLength, decodedBytes);
    }

    private static byte[] DecodeBase64(
        ReadOnlySpan<char> payload,
        long remainingEncodedBytes,
        ImageLoadSource source)
    {
        if (payload.IsEmpty || payload.Length % 4 != 0)
        {
            throw Invalid("SVG base64 data image is invalid.", source);
        }
        var paddingStarted = false;
        var paddingCount = 0;
        foreach (var character in payload)
        {
            if (character == '=')
            {
                paddingStarted = true;
                paddingCount++;
                if (paddingCount > 2)
                {
                    throw Invalid("SVG base64 data image is invalid.", source);
                }
                continue;
            }
            if (paddingStarted ||
                !(character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9' or '+' or '/'))
            {
                throw Invalid("SVG base64 data image is invalid.", source);
            }
        }

        var estimatedLength = checked((long)payload.Length / 4 * 3 - paddingCount);
        if (estimatedLength > remainingEncodedBytes)
        {
            throw EmbeddedLimit(source);
        }
        try
        {
            return Convert.FromBase64String(payload.ToString());
        }
        catch (FormatException exception)
        {
            throw ImageSourceReadHelpers.Failure(
                ImageLoadErrorCode.InvalidImageData,
                "SVG base64 data image is invalid.",
                source.DisplayName,
                exception);
        }
    }

    private static byte[] DecodePercentEncoded(
        ReadOnlySpan<char> payload,
        long remainingEncodedBytes,
        ImageLoadSource source)
    {
        if (payload.Length > remainingEncodedBytes * 3)
        {
            throw EmbeddedLimit(source);
        }
        var bytes = new byte[Math.Min(payload.Length, checked((int)Math.Min(remainingEncodedBytes, int.MaxValue)))];
        var count = 0;
        for (var index = 0; index < payload.Length; index++)
        {
            if (count >= remainingEncodedBytes)
            {
                throw EmbeddedLimit(source);
            }
            var character = payload[index];
            if (character == '%')
            {
                if (index + 2 >= payload.Length ||
                    !TryHex(payload[index + 1], out var high) ||
                    !TryHex(payload[index + 2], out var low))
                {
                    throw Invalid("SVG percent-encoded data image is invalid.", source);
                }
                bytes[count++] = (byte)((high << 4) | low);
                index += 2;
                continue;
            }
            if (character > 0x7f)
            {
                throw Invalid("SVG data image contains non-ASCII unescaped bytes.", source);
            }
            bytes[count++] = (byte)character;
        }
        return bytes.AsSpan(0, count).ToArray();
    }

    private static bool TryHex(char value, out int result)
    {
        if (value is >= '0' and <= '9')
        {
            result = value - '0';
            return true;
        }
        if (value is >= 'a' and <= 'f')
        {
            result = value - 'a' + 10;
            return true;
        }
        if (value is >= 'A' and <= 'F')
        {
            result = value - 'A' + 10;
            return true;
        }
        result = 0;
        return false;
    }

    private static ImageLoadFailureException Invalid(string message, ImageLoadSource source) =>
        ImageSourceReadHelpers.Failure(ImageLoadErrorCode.InvalidImageData, message, source.DisplayName);

    private static ImageLoadFailureException Unsafe(string message, ImageLoadSource source) =>
        ImageSourceReadHelpers.Failure(ImageLoadErrorCode.UnsafeVectorContent, message, source.DisplayName);

    private static ImageLoadFailureException EmbeddedLimit(ImageLoadSource source) =>
        ImageSourceReadHelpers.Failure(
            ImageLoadErrorCode.EmbeddedResourceLimitExceeded,
            "SVG embedded image bytes exceed the configured limit.",
            source.DisplayName);
}
