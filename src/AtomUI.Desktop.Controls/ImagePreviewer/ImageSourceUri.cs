using System.ComponentModel;

namespace AtomUI.Desktop.Controls;

public enum ImageSourceUriKind
{
    LocalFile,
    AvaloniaResource,
    Remote,
    Unsupported
}

[TypeConverter(typeof(ImageSourceUriTypeConverter))]
public sealed record ImageSourceUri
{
    private ImageSourceUri(string originalString,
                           ImageSourceUriKind kind,
                           string cacheKey,
                           Uri? uri,
                           string? localPath)
    {
        OriginalString = originalString;
        Kind           = kind;
        CacheKey       = cacheKey;
        Uri            = uri;
        LocalPath      = localPath;
    }

    public string OriginalString { get; }

    public ImageSourceUriKind Kind { get; }

    public string CacheKey { get; }

    public Uri? Uri { get; }

    public string? LocalPath { get; }

    public static ImageSourceUri Parse(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentException("Image source uri cannot be empty.", nameof(source));
        }

        if (Uri.TryCreate(source, UriKind.Absolute, out var uri))
        {
            if (uri.IsFile)
            {
                var localPath = Path.GetFullPath(uri.LocalPath);
                return new ImageSourceUri(source, ImageSourceUriKind.LocalFile, localPath, uri, localPath);
            }

            if (string.Equals(uri.Scheme, "avares", StringComparison.OrdinalIgnoreCase))
            {
                return new ImageSourceUri(source, ImageSourceUriKind.AvaloniaResource, source, uri, null);
            }

            if (string.Equals(uri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(uri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
            {
                return new ImageSourceUri(source, ImageSourceUriKind.Remote, uri.AbsoluteUri, uri, null);
            }

            return new ImageSourceUri(source, ImageSourceUriKind.Unsupported, uri.AbsoluteUri, uri, null);
        }

        var fullPath = Path.GetFullPath(source);
        return new ImageSourceUri(source, ImageSourceUriKind.LocalFile, fullPath, null, fullPath);
    }

    public static bool TryParse(string? source, out ImageSourceUri? imageSourceUri)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            imageSourceUri = null;
            return false;
        }

        imageSourceUri = Parse(source);
        return true;
    }

    public override string ToString()
    {
        return OriginalString;
    }

    public static implicit operator ImageSourceUri(string source)
    {
        return Parse(source);
    }
}

internal sealed class ImageSourceUriTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, System.Globalization.CultureInfo? culture, object value)
    {
        return value is string source
            ? ImageSourceUri.Parse(source)
            : base.ConvertFrom(context, culture, value);
    }
}
