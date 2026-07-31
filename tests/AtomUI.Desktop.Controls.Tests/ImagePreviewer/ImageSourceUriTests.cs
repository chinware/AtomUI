using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImageSourceUriTests
{
    [Fact]
    public void Parse_Classifies_Avalonia_Resource_Uri()
    {
        var source = ImageSourceUri.Parse("avares://AtomUI.Core/Assets/Themes/DaybreakBlue.theme.xml");

        source.Kind.ShouldBe(ImageSourceUriKind.AvaloniaResource);
        source.CacheKey.ShouldBe("avares://AtomUI.Core/Assets/Themes/DaybreakBlue.theme.xml");
    }

    [Fact]
    public void Parse_Classifies_Http_Uri()
    {
        var source = ImageSourceUri.Parse("https://example.com/a.png");

        source.Kind.ShouldBe(ImageSourceUriKind.Remote);
        source.CacheKey.ShouldBe("https://example.com/a.png");
    }

    [Fact]
    public void Parse_Classifies_Absolute_Local_Path()
    {
        var path   = Path.Combine(Path.GetTempPath(), "atomui-image-preview.png");
        var source = ImageSourceUri.Parse(path);

        source.Kind.ShouldBe(ImageSourceUriKind.LocalFile);
        source.CacheKey.ShouldBe(Path.GetFullPath(path));
    }

    [Fact]
    public void Parse_Classifies_Relative_Local_Path()
    {
        var source = ImageSourceUri.Parse("./images/a.png");

        source.Kind.ShouldBe(ImageSourceUriKind.LocalFile);
        source.CacheKey.ShouldBe(Path.GetFullPath("./images/a.png"));
    }
}
