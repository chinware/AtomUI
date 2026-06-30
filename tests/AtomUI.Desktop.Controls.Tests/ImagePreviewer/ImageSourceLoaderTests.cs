using System.Net;
using System.Net.Http;
using System.Text;
using AtomUI.Desktop.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImageSourceLoaderTests
{
    public ImageSourceLoaderTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public async Task LoadAsync_Loads_Local_Bitmap_Path()
    {
        var path   = CreatePngFile();
        var loader = new DefaultImageSourceLoader();

        using var image = await loader.LoadAsync(ImageSourceUri.Parse(path), CancellationToken.None);

        image.IsBitmap.ShouldBeTrue();
        image.SourceSize.Width.ShouldBeGreaterThan(0);
        image.SourceSize.Height.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task LoadAsync_Loads_Local_Svg_Path()
    {
        var path   = CreateSvgFile();
        var loader = new DefaultImageSourceLoader();

        using var image = await loader.LoadAsync(ImageSourceUri.Parse(path), CancellationToken.None);

        image.IsSvg.ShouldBeTrue();
        image.SvgContent.ShouldNotBeNull().ShouldContain("<svg");
    }

    [Fact]
    public async Task LoadAsync_Loads_Remote_Bitmap_Uri()
    {
        var bytes      = CreatePngBytes();
        var httpClient = new HttpClient(new TestImageMessageHandler(bytes, "image/png"));
        var loader     = new DefaultImageSourceLoader(httpClient);

        using (httpClient)
        using (var image = await loader.LoadAsync(ImageSourceUri.Parse("https://example.com/sample.png"), CancellationToken.None))
        {
            image.IsBitmap.ShouldBeTrue();
            image.SourceSize.Width.ShouldBeGreaterThan(0);
            image.SourceSize.Height.ShouldBeGreaterThan(0);
        }
    }

    [Fact]
    public async Task LoadAsync_Loads_Remote_Svg_From_Content_Type()
    {
        var bytes      = Encoding.UTF8.GetBytes("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"1\" height=\"1\" />");
        var httpClient = new HttpClient(new TestImageMessageHandler(bytes, "image/svg+xml"));
        var loader     = new DefaultImageSourceLoader(httpClient);

        using (httpClient)
        using (var image = await loader.LoadAsync(ImageSourceUri.Parse("https://example.com/render?id=1"), CancellationToken.None))
        {
            image.IsSvg.ShouldBeTrue();
            image.SvgContent.ShouldNotBeNull().ShouldContain("<svg");
        }
    }

    [Fact]
    public async Task LoadAsync_Honors_Cancellation()
    {
        var loader = new DefaultImageSourceLoader();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Should.ThrowAsync<OperationCanceledException>(() =>
            loader.LoadAsync(ImageSourceUri.Parse("https://example.com/image.png"), cts.Token));
    }

    private static string CreatePngFile()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"atomui-image-loader-{Guid.NewGuid():N}");
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

    private static string CreateSvgFile()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"atomui-image-loader-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "sample.svg");
        File.WriteAllText(path, "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"1\" height=\"1\" />");
        return path;
    }

    private sealed class TestImageMessageHandler : HttpMessageHandler
    {
        private readonly byte[] _content;
        private readonly string _contentType;

        public TestImageMessageHandler(byte[] content, string contentType)
        {
            _content     = content;
            _contentType = contentType;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StreamContent(new AsyncOnlyReadStream(_content))
            };
            response.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(_contentType);
            response.Content.Headers.ContentLength = _content.Length;
            return Task.FromResult(response);
        }
    }

    private sealed class AsyncOnlyReadStream : Stream
    {
        private readonly MemoryStream _inner;

        public AsyncOnlyReadStream(byte[] content)
        {
            _inner = new MemoryStream(content, writable: false);
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException("HTTP response streams must be copied asynchronously before image decoding.");
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return _inner.ReadAsync(buffer, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _inner.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
