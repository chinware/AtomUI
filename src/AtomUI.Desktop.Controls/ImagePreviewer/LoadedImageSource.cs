using System.Text;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace AtomUI.Desktop.Controls;

internal enum LoadedImageSourceType
{
    Svg,
    Bitmap
}

internal sealed class LoadedImageSource : IDisposable
{
    private LoadedImageSource(LoadedImageSourceType type, string? svgContent, IImage? bitmap, Size sourceSize)
    {
        Type        = type;
        SvgContent  = svgContent;
        Bitmap      = bitmap;
        SourceSize  = sourceSize;
    }

    public LoadedImageSourceType Type { get; }

    public string? SvgContent { get; }

    public IImage? Bitmap { get; }

    public Size SourceSize { get; }

    public bool IsSvg => Type == LoadedImageSourceType.Svg;

    public bool IsBitmap => Type == LoadedImageSourceType.Bitmap;

    public static LoadedImageSource CreateSvg(string svgContent, Size sourceSize = default)
    {
        return new LoadedImageSource(LoadedImageSourceType.Svg, svgContent, null, sourceSize);
    }

    public static LoadedImageSource CreateSvg(Stream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096);
        return CreateSvg(reader.ReadToEnd());
    }

    public static LoadedImageSource CreateBitmap(Stream stream)
    {
        var bitmap = new Bitmap(stream);
        return new LoadedImageSource(LoadedImageSourceType.Bitmap, null, bitmap, bitmap.Size);
    }

    public void Dispose()
    {
        (Bitmap as IDisposable)?.Dispose();
    }
}
