using System.Net;
using System.Net.Http;
using System.Text;
using AtomUI.Desktop.Controls;
using Avalonia.Threading;
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
    public void LoadAsync_Loads_Local_Bitmap_Path()
    {
        var path   = CreatePngFile();
        var loader = new DefaultImageSourceLoader();

        using var image = WaitForLoad(loader.LoadAsync(new UriImagePreviewSource(path), CancellationToken.None));

        image.IsBitmap.ShouldBeTrue();
        image.SourceSize.Width.ShouldBeGreaterThan(0);
        image.SourceSize.Height.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void LoadAsync_Loads_Local_Svg_Path()
    {
        var path   = CreateSvgFile();
        var loader = new DefaultImageSourceLoader();

        using var image = WaitForLoad(loader.LoadAsync(new UriImagePreviewSource(path), CancellationToken.None));

        image.IsSvg.ShouldBeTrue();
        image.SvgContent.ShouldNotBeNull().ShouldContain("<svg");
    }

    [Fact]
    public void LoadAsync_Loads_Remote_Bitmap_Uri()
    {
        var bytes      = CreatePngBytes();
        var httpClient = new HttpClient(new TestImageMessageHandler(bytes, "image/png"));
        var loader     = new DefaultImageSourceLoader(httpClient);

        using (httpClient)
        using (var image = WaitForLoad(loader.LoadAsync(new UriImagePreviewSource("https://example.com/sample.png"), CancellationToken.None)))
        {
            image.IsBitmap.ShouldBeTrue();
            image.SourceSize.Width.ShouldBeGreaterThan(0);
            image.SourceSize.Height.ShouldBeGreaterThan(0);
        }
    }

    [Fact]
    public void LoadAsync_Loads_Remote_Svg_From_Content_Type()
    {
        var bytes      = Encoding.UTF8.GetBytes("<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"1\" height=\"1\" />");
        var httpClient = new HttpClient(new TestImageMessageHandler(bytes, "image/svg+xml"));
        var loader     = new DefaultImageSourceLoader(httpClient);

        using (httpClient)
        using (var image = WaitForLoad(loader.LoadAsync(new UriImagePreviewSource("https://example.com/render?id=1"), CancellationToken.None)))
        {
            image.IsSvg.ShouldBeTrue();
            image.SvgContent.ShouldNotBeNull().ShouldContain("<svg");
        }
    }

    [Fact]
    public void LoadAsync_Loads_Stream_Source_And_Disposes_Input_Stream()
    {
        DisposeTrackingStream? openedStream = null;
        var source = new StreamImagePreviewSource(_ =>
        {
            openedStream = new DisposeTrackingStream(CreatePngBytes());
            return new ValueTask<Stream>(openedStream);
        }, displayName: "stream-source.png", contentType: "image/png");
        var loader = new DefaultImageSourceLoader();

        using var image = WaitForLoad(loader.LoadAsync(source, CancellationToken.None));

        image.IsBitmap.ShouldBeTrue();
        openedStream.ShouldNotBeNull();
        openedStream.IsDisposed.ShouldBeTrue();
    }

    [Fact]
    public void LoadAsync_Honors_Cancellation()
    {
        var loader = new DefaultImageSourceLoader();
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Should.Throw<OperationCanceledException>(() =>
            WaitForLoad(loader.LoadAsync(new UriImagePreviewSource("https://example.com/image.png"), cts.Token)));
    }

    private static LoadedImageSource WaitForLoad(Task<LoadedImageSource> task)
    {
        for (var i = 0; i < 250; i++)
        {
            if (task.IsCompleted)
            {
                break;
            }

            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(20);
        }

        task.IsCompleted.ShouldBeTrue("image load task did not complete");
        return task.GetAwaiter().GetResult();
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

    private sealed class DisposeTrackingStream : MemoryStream
    {
        public DisposeTrackingStream(byte[] buffer)
            : base(buffer, writable: false)
        {
        }

        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }

        public override ValueTask DisposeAsync()
        {
            IsDisposed = true;
            return base.DisposeAsync();
        }
    }
}
