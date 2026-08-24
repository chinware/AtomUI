using AtomUI.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Controls.Shared.Tests.ImageLoading;

public class ImageCacheTests
{
    [Fact]
    public void Decoded_Cache_Eviction_Defers_Disposal_Until_Result_Lease_Is_Released()
    {
        using var cache = new ImageDecodedCache(maxBytes: 1_000, maxEntries: 1);
        var firstImage = new TestImage();
        var secondImage = new TestImage();
        var firstEntry = CreateEntry(firstImage, decodedBytes: 400);
        var secondEntry = CreateEntry(secondImage, decodedBytes: 400);
        var firstKey = CreateDecodedKey("first");
        var secondKey = CreateDecodedKey("second");

        cache.TryAdd(firstKey, firstEntry).ShouldBeTrue();
        using var firstResult = firstEntry.AcquireResult(ImageCacheSource.DecodedMemory);
        cache.TryAdd(secondKey, secondEntry).ShouldBeTrue();

        firstImage.DisposeCount.ShouldBe(0);
        firstResult.Dispose();
        firstImage.DisposeCount.ShouldBe(1);

        cache.Dispose();
        secondImage.DisposeCount.ShouldBe(1);
    }

    [Fact]
    public void Borrowed_Image_Is_Never_Disposed_By_Operation_Or_Result_Lease()
    {
        var image = new TestImage();
        var entry = new ImageDecodedCacheEntry(
            image,
            ownsImage: false,
            10,
            10,
            10,
            10,
            400,
            null);
        entry.RetainOperation();

        using var result = entry.AcquireResult(ImageCacheSource.Local);
        entry.ReleaseOperation();
        result.Dispose();

        image.DisposeCount.ShouldBe(0);
        result.Image.ShouldBeSameAs(image);
    }

    [Fact]
    public void Encoded_Cache_Uses_Lru_And_Partition_Clear()
    {
        using var cache = new ImageEncodedCache(maxBytes: 10, maxEntries: 2);
        var first = new ImageEncodedCacheKey("first", "p1");
        var second = new ImageEncodedCacheKey("second", "p2");
        var third = new ImageEncodedCacheKey("third", "p1");
        cache.Set(first, ImageLoadingTestSupport.CreateContent([1, 2]));
        cache.Set(second, ImageLoadingTestSupport.CreateContent([3, 4]));
        cache.TryGet(first, out _).ShouldBeTrue();

        cache.Set(third, ImageLoadingTestSupport.CreateContent([5, 6]));

        cache.TryGet(second, out _).ShouldBeFalse();
        cache.TryGet(first, out _).ShouldBeTrue();
        cache.TryGet(third, out _).ShouldBeTrue();
        cache.Clear("p1");
        cache.Count.ShouldBe(0);
    }

    [Fact]
    public async Task File_Cache_RoundTrips_Metadata_And_Verifies_Content_Digest()
    {
        var directory = CreateTempDirectory();
        try
        {
            using var cache = new ImageFileCache(directory, maxBytes: 1_000, maxEntries: 10);
            var key = new ImageEncodedCacheKey("entry", "partition");
            var content = new ImageEncodedContent(
                [1, 2, 3, 4],
                "image/png",
                ImageCacheSource.Network,
                DateTimeOffset.UtcNow,
                FreshUntil: DateTimeOffset.UtcNow.AddMinutes(5),
                ETag: "\"etag\"",
                NoCache: true,
                MustRevalidate: true,
                IsRemote: true,
                VaryHeaders: ["Accept-Language"],
                VaryDigest: "digest",
                MaxAge: TimeSpan.FromMinutes(5),
                IsPrivate: true);

            await cache.SetAsync(key, content, CancellationToken.None);
            var loaded = await cache.TryGetAsync(key, CancellationToken.None);

            loaded.ShouldNotBeNull();
            loaded.Bytes.ShouldBe(content.Bytes);
            loaded.CacheSource.ShouldBe(ImageCacheSource.Persistent);
            loaded.ETag.ShouldBe(content.ETag);
            loaded.VaryHeaders.ShouldBe(["Accept-Language"]);
            loaded.VaryDigest.ShouldBe("digest");
            loaded.IsPrivate.ShouldBeTrue();

            await File.WriteAllBytesAsync(
                Path.Combine(directory, "entry.bin"),
                [9, 9, 9],
                TestContext.Current.CancellationToken);
            (await cache.TryGetAsync(key, CancellationToken.None)).ShouldBeNull();
            File.Exists(Path.Combine(directory, "entry.bin")).ShouldBeFalse();
            File.Exists(Path.Combine(directory, "entry.meta")).ShouldBeFalse();
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void File_Cache_Startup_Removes_Temporary_And_Orphaned_Files()
    {
        var directory = CreateTempDirectory();
        try
        {
            File.WriteAllBytes(Path.Combine(directory, "write.tmp"), [1]);
            File.WriteAllBytes(Path.Combine(directory, "orphan-data.bin"), [1]);
            File.WriteAllBytes(Path.Combine(directory, "orphan-metadata.meta"), [1]);

            using var cache = new ImageFileCache(directory, maxBytes: 100, maxEntries: 10);

            Directory.EnumerateFiles(directory).ShouldBeEmpty();
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task File_Cache_Trims_To_Entry_Limit_And_Removes_Only_Requested_Key()
    {
        var directory = CreateTempDirectory();
        try
        {
            using var cache = new ImageFileCache(directory, maxBytes: 100, maxEntries: 1);
            var first = new ImageEncodedCacheKey("first", "partition");
            var second = new ImageEncodedCacheKey("second", "partition");
            await cache.SetAsync(first, ImageLoadingTestSupport.CreateContent([1]), CancellationToken.None);
            File.SetLastAccessTimeUtc(Path.Combine(directory, "first.bin"), DateTime.UtcNow.AddMinutes(-1));
            await cache.SetAsync(second, ImageLoadingTestSupport.CreateContent([2]), CancellationToken.None);

            (await cache.TryGetAsync(first, CancellationToken.None)).ShouldBeNull();
            (await cache.TryGetAsync(second, CancellationToken.None)).ShouldNotBeNull();

            await cache.RemoveAsync(second, CancellationToken.None);
            (await cache.TryGetAsync(second, CancellationToken.None)).ShouldBeNull();
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static ImageDecodedCacheEntry CreateEntry(TestImage image, long decodedBytes)
    {
        return new ImageDecodedCacheEntry(
            image,
            ownsImage: true,
            10,
            10,
            10,
            10,
            decodedBytes,
            "image/png");
    }

    private static ImageDecodedCacheKey CreateDecodedKey(string value)
    {
        return new ImageDecodedCacheKey(
            new ImageEncodedCacheKey(value, string.Empty),
            10,
            10,
            "codec");
    }

    private static string CreateTempDirectory()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"atomui-image-cache-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        return directory;
    }
}
