using AtomUI.Desktop.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImagePreviewSourceTests
{
    [Fact]
    public void StreamImagePreviewSource_Disposes_Opened_Stream_When_Cancelled_After_Factory_Returns()
    {
        using var cts = new CancellationTokenSource();
        DisposeTrackingStream? openedStream = null;
        var source = new StreamImagePreviewSource(_ =>
        {
            openedStream = new DisposeTrackingStream();
            cts.Cancel();
            return new ValueTask<Stream>(openedStream);
        }, displayName: "stream-source.png");

        Should.Throw<OperationCanceledException>(() =>
            source.OpenReadAsync(cts.Token).AsTask().GetAwaiter().GetResult());

        openedStream.ShouldNotBeNull();
        openedStream.IsDisposed.ShouldBeTrue();
    }

    [Fact]
    public void StreamImagePreviewSource_Uses_Object_Reference_Identity_When_Not_Configured()
    {
        var source = new StreamImagePreviewSource(_ => new ValueTask<Stream>(Stream.Null));

        var identitySource = source.ShouldBeAssignableTo<IImagePreviewSourceIdentity>();

        identitySource.Identity.ShouldBeSameAs(source);
    }

    private sealed class DisposeTrackingStream : MemoryStream
    {
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
