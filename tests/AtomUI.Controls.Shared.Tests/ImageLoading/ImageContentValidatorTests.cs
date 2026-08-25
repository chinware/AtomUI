using AtomUI.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Shared.Tests.ImageLoading;

public class ImageContentValidatorTests
{
    [Fact]
    public void Valid_Png_Uses_Magic_Bytes_And_Reports_Dimensions()
    {
        var validator = new ImageContentValidator(ImageLoadingTestSupport.CreateOptions());
        var probe = validator.Validate(
            ImageLoadingTestSupport.CreateContent(ImageLoadingTestSupport.CreatePngHeader(32, 24)),
            ImageLoadSource.FromBytes(new byte[] { 1 }, "png", "v1"));

        probe.Format.ShouldBe(ImageContentFormat.Png);
        probe.MediaType.ShouldBe("image/png");
        probe.PixelWidth.ShouldBe(32);
        probe.PixelHeight.ShouldBe(24);
    }

    [Fact]
    public void Declared_Media_Type_Must_Match_Detected_Content()
    {
        AssertFailure(
            ImageLoadErrorCode.ContentTypeMismatch,
            ImageLoadingTestSupport.CreateContent(
                ImageLoadingTestSupport.CreatePngHeader(),
                "image/jpeg"),
            ImageLoadSource.FromBytes(new byte[] { 1 }, "png", "v1"));
    }

    [Fact]
    public void Remote_And_Untrusted_Svg_Are_Rejected()
    {
        AssertFailure(
            ImageLoadErrorCode.UnsafeVectorContent,
            ImageLoadingTestSupport.CreateContent("<svg />"u8.ToArray(), "image/svg+xml"),
            ImageLoadSource.FromUri("https://example.com/image.svg"));
    }

    [Fact]
    public void Trusted_Avalonia_Asset_Svg_Is_Allowed()
    {
        var validator = new ImageContentValidator(ImageLoadingTestSupport.CreateOptions());
        var content = ImageLoadingTestSupport.CreateContent("<svg />"u8.ToArray(), "image/svg+xml") with
        {
            IsTrustedAsset = true
        };

        var probe = validator.Validate(
            content,
            ImageLoadSource.FromUri("avares://AtomUI.Tests/Assets/image.svg"));

        probe.Format.ShouldBe(ImageContentFormat.Svg);
    }

    [Fact]
    public void Dimension_Pixel_And_Decoded_Byte_Limits_Produce_Distinct_Errors()
    {
        AssertFailure(
            ImageLoadErrorCode.DimensionLimitExceeded,
            ImageLoadingTestSupport.CreateContent(ImageLoadingTestSupport.CreatePngHeader(5, 2)),
            options: ImageLoadingTestSupport.CreateOptions(builder => builder.MaxImageWidth = 4));
        AssertFailure(
            ImageLoadErrorCode.PixelLimitExceeded,
            ImageLoadingTestSupport.CreateContent(ImageLoadingTestSupport.CreatePngHeader(3, 2)),
            options: ImageLoadingTestSupport.CreateOptions(builder => builder.MaxImagePixelCount = 5));
        AssertFailure(
            ImageLoadErrorCode.DecodedByteLimitExceeded,
            ImageLoadingTestSupport.CreateContent(ImageLoadingTestSupport.CreatePngHeader(2, 2)),
            options: ImageLoadingTestSupport.CreateOptions(builder => builder.MaxDecodedImageBytes = 15));
    }

    [Fact]
    public void Animated_Png_Is_Rejected()
    {
        AssertFailure(
            ImageLoadErrorCode.AnimationNotSupported,
            ImageLoadingTestSupport.CreateContent(
                ImageLoadingTestSupport.CreatePngHeader(animated: true)));
    }

    [Fact]
    public void Static_Gif_With_Comma_Byte_In_Image_Data_Is_Not_Marked_Animated()
    {
        var bytes = new byte[]
        {
            (byte)'G', (byte)'I', (byte)'F', (byte)'8', (byte)'9', (byte)'a',
            1, 0, 1, 0, 0, 0, 0,
            0x2c, 0, 0, 0, 0, 1, 0, 1, 0, 0,
            0x02, 0x01, 0x2c, 0x00, 0x3b
        };

        var probe = new ImageContentValidator(ImageLoadingTestSupport.CreateOptions()).Validate(
            ImageLoadingTestSupport.CreateContent(bytes, "image/gif"),
            ImageLoadSource.FromBytes(new byte[] { 1 }, "gif", "v1"));

        probe.IsAnimated.ShouldBeFalse();
        probe.PixelWidth.ShouldBe(1);
        probe.PixelHeight.ShouldBe(1);
    }

    [Fact]
    public void Extreme_Png_And_Bmp_Dimensions_Return_Typed_Failures()
    {
        var png = ImageLoadingTestSupport.CreatePngHeader();
        png[16] = 0xff;
        png[17] = 0xff;
        png[18] = 0xff;
        png[19] = 0xff;
        AssertFailure(
            ImageLoadErrorCode.DimensionLimitExceeded,
            ImageLoadingTestSupport.CreateContent(png),
            ImageLoadSource.FromBytes(new byte[] { 1 }, "png", "overflow"));

        var bmp = new byte[26];
        bmp[0] = (byte)'B';
        bmp[1] = (byte)'M';
        System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(bmp.AsSpan(18, 4), int.MinValue);
        System.Buffers.Binary.BinaryPrimitives.WriteInt32LittleEndian(bmp.AsSpan(22, 4), 1);
        AssertFailure(
            ImageLoadErrorCode.DimensionLimitExceeded,
            ImageLoadingTestSupport.CreateContent(bmp, "image/bmp"),
            ImageLoadSource.FromBytes(new byte[] { 1 }, "bmp", "overflow"));
    }

    [Fact]
    public void Empty_Markup_And_Unknown_Bytes_Are_Rejected_With_Typed_Errors()
    {
        AssertFailure(
            ImageLoadErrorCode.InvalidImageData,
            ImageLoadingTestSupport.CreateContent([]));
        AssertFailure(
            ImageLoadErrorCode.UnsafeVectorContent,
            ImageLoadingTestSupport.CreateContent("<html />"u8.ToArray(), "text/html"));
        AssertFailure(
            ImageLoadErrorCode.UnsupportedFormat,
            ImageLoadingTestSupport.CreateContent([1, 2, 3, 4], null));
    }

    private static void AssertFailure(
        ImageLoadErrorCode expected,
        ImageEncodedContent content,
        ImageLoadSource? source = null,
        ImageLoadingOptions? options = null)
    {
        var validator = new ImageContentValidator(options ?? ImageLoadingTestSupport.CreateOptions());
        var exception = Should.Throw<ImageLoadFailureException>(() => validator.Validate(
            content,
            source ?? ImageLoadSource.FromBytes(new byte[] { 1 }, "content", "v1")));
        exception.Error.Code.ShouldBe(expected);
    }
}
