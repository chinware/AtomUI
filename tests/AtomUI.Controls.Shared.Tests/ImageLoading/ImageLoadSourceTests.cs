using AtomUI.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Shared.Tests.ImageLoading;

public class ImageLoadSourceTests
{
    [Fact]
    public void Parse_Normalizes_Http_Identity_And_Display_Name()
    {
        var source = ImageLoadSource.Parse(
            "HTTPS://EXAMPLE.COM:443/assets/remote%20image.png?size=large#preview");

        source.Kind.ShouldBe(ImageLoadSourceKind.Http);
        source.DisplayName.ShouldBe("remote image.png");
        source.Identity.ShouldBe("http:https://example.com/assets/remote%20image.png?size=large");
    }

    [Fact]
    public void Http_Display_Name_Uses_Meaningful_Segment_Before_Trailing_Dimensions()
    {
        ImageLoadSource.FromUri("https://picsum.photos/id/25/600/400")
                       .DisplayName.ShouldBe("25");
    }

    [Fact]
    public void TryParse_Rejects_Unknown_And_Relative_Schemes()
    {
        ImageLoadSource.TryParse("ftp://example.com/image.png", out var unknown).ShouldBeFalse();
        unknown.ShouldBeNull();
        ImageLoadSource.TryParse("images/photo.png", out var relative).ShouldBeFalse();
        relative.ShouldBeNull();
    }

    [Theory]
    [InlineData("avares:///Assets/image.png")]
    [InlineData("avares://AtomUI.Tests/Assets/image.png?theme=dark")]
    [InlineData("avares://AtomUI.Tests/Assets/image.png#fragment")]
    public void Asset_Source_Requires_Assembly_And_Pure_Resource_Path(string value)
    {
        Should.Throw<ArgumentException>(() => ImageLoadSource.FromUri(value));
    }

    [Fact]
    public void Unkeyed_Object_Sources_Receive_Unique_Identities()
    {
        var first = ImageLoadSource.FromBytes(new byte[] { 1, 2, 3 });
        var second = ImageLoadSource.FromBytes(new byte[] { 1, 2, 3 });

        first.Identity.ShouldNotBe(second.Identity);
    }

    [Fact]
    public void Keyed_Sources_Use_Stable_Key_And_Version_Identity()
    {
        var first = ImageLoadSource.FromBytes(new byte[] { 1 }, "avatar", "v2");
        var second = ImageLoadSource.FromBytes(new byte[] { 2 }, "avatar", "v2");
        var changed = ImageLoadSource.FromBytes(new byte[] { 1 }, "avatar", "v3");

        first.Identity.ShouldBe(second.Identity);
        changed.Identity.ShouldNotBe(first.Identity);
    }

    [Fact]
    public void Keyed_Sources_Without_Version_Use_Object_Identity()
    {
        var first = ImageLoadSource.FromBytes(new byte[] { 1 }, "avatar");
        var second = ImageLoadSource.FromBytes(new byte[] { 2 }, "avatar");

        first.Identity.ShouldNotBe(second.Identity);
    }

    [Fact]
    public void Normalize_Uses_NoStore_And_Unique_Share_Scope_For_Unpartitioned_Credentials()
    {
        var options = ImageLoadingTestSupport.CreateOptions();
        var request = new ImageLoadRequest(ImageLoadSource.FromUri("https://example.com/image.png"))
        {
            Options = new ImageRequestOptions
            {
                Headers = new Dictionary<string, string> { ["Authorization"] = "Bearer secret" }
            }
        };

        var first = ImageCacheKey.Normalize(request, options, forceReload: false);
        var second = ImageCacheKey.Normalize(request, options, forceReload: false);

        first.CacheMode.ShouldBe(ImageCacheMode.NoStore);
        first.CanPersist.ShouldBeFalse();
        first.CanShare.ShouldBeFalse();
        first.EncodedOperationKey.ShareScope.ShouldNotBe(second.EncodedOperationKey.ShareScope);
    }

    [Fact]
    public void Normalize_Allows_Partitioned_Credential_Request_To_Share_But_Not_Persist_By_Default()
    {
        var options = ImageLoadingTestSupport.CreateOptions();
        var normalized = ImageCacheKey.Normalize(
            new ImageLoadRequest(ImageLoadSource.FromUri("https://example.com/image.png"))
            {
                Options = new ImageRequestOptions
                {
                    CachePartition = "account:42",
                    Headers = new Dictionary<string, string> { ["Cookie"] = "session=secret" }
                }
            },
            options,
            forceReload: false);

        normalized.CacheMode.ShouldBe(ImageCacheMode.Default);
        normalized.CanShare.ShouldBeTrue();
        normalized.CanPersist.ShouldBeFalse();
        normalized.PartitionHash.ShouldNotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData("Host")]
    [InlineData("Content-Length")]
    [InlineData("Range")]
    public void Normalize_Rejects_Transport_Owned_Headers(string header)
    {
        var options = ImageLoadingTestSupport.CreateOptions();
        var request = new ImageLoadRequest(ImageLoadSource.FromUri("https://example.com/image.png"))
        {
            Options = new ImageRequestOptions
            {
                Headers = new Dictionary<string, string> { [header] = "value" }
            }
        };

        Should.Throw<ArgumentException>(() => ImageCacheKey.Normalize(request, options, false));
    }
}
