using AtomUI.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImagePreviewItemTests
{
    [Fact]
    public void ImagePreviewItem_Exposes_Init_Only_Configuration()
    {
        var source = ImageLoadSource.FromUri("avares://AtomUI.Tests/Assets/full.png");
        var thumbnail = ImageLoadSource.FromUri("avares://AtomUI.Tests/Assets/thumb.png");
        var fallback = ImageLoadSource.FromUri("avares://AtomUI.Tests/Assets/fallback.png");
        var options = new ImageRequestOptions { Variant = "dark" };
        var tag = new object();

        var item = new ImagePreviewItem(source)
        {
            ThumbnailSource = thumbnail,
            FallbackSource = fallback,
            RequestOptions = options,
            Title = "Preview title",
            Tag = tag
        };

        item.Source.ShouldBeSameAs(source);
        item.ThumbnailSource.ShouldBeSameAs(thumbnail);
        item.FallbackSource.ShouldBeSameAs(fallback);
        item.RequestOptions.ShouldBeSameAs(options);
        item.Title.ShouldBe("Preview title");
        item.Tag.ShouldBeSameAs(tag);
        foreach (var property in typeof(ImagePreviewItem).GetProperties())
        {
            var setMethod = property.SetMethod.ShouldNotBeNull();
            setMethod.ReturnParameter
                     .GetRequiredCustomModifiers()
                     .ShouldContain(typeof(System.Runtime.CompilerServices.IsExternalInit));
        }
    }

    [Fact]
    public void With_Expression_Creates_A_New_Item_Without_Mutating_The_Original()
    {
        var source = ImageLoadSource.FromUri("avares://AtomUI.Tests/Assets/source.png");
        var original = new ImagePreviewItem(source) { Title = "Original" };

        var changed = original with { Title = "Changed" };

        changed.ShouldNotBeSameAs(original);
        changed.Source.ShouldBeSameAs(source);
        changed.Title.ShouldBe("Changed");
        original.Title.ShouldBe("Original");
    }
}
