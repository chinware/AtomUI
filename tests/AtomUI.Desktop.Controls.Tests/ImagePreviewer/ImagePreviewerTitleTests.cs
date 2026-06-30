using AtomUI.Desktop.Controls;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.ImagePreviewer;

public class ImagePreviewerTitleTests
{
    public ImagePreviewerTitleTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void DefaultImagePreviewTitleResolver_Uses_File_Name_From_Supported_Source_Uris()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"atomui-title-{Guid.NewGuid():N}");
        var localPath = Path.Combine(directory, "local image.png");

        Resolve(localPath).ShouldBe("local image.png");
        Resolve(new Uri(localPath).AbsoluteUri).ShouldBe("local image.png");
        Resolve("https://example.com/assets/remote-image.jpg?size=large#preview").ShouldBe("remote-image.jpg");
        Resolve("avares://AtomUI.Tests/Assets/resource-icon.svg").ShouldBe("resource-icon.svg");
    }

    [Fact]
    public void ImagePreviewerDialog_Title_Uses_Window_Title_Before_Resolver()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var dialog = CreateDialog(
                "avares://AtomUI.Tests/Assets/first.png",
                "avares://AtomUI.Tests/Assets/second.png");

            dialog.CurrentIndex = 1;
            dialog.Title = "Pinned title";

            dialog.EffectivePreviewTitle.ShouldBe("Pinned title");

            dialog.Title = " ";

            dialog.EffectivePreviewTitle.ShouldBe("second.png");
        });
    }

    [Fact]
    public void ImagePreviewerDialog_Title_Updates_With_Current_Item_And_Clamps_Display_Index()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var dialog = CreateDialog(
                "avares://AtomUI.Tests/Assets/first.png",
                "avares://AtomUI.Tests/Assets/second.png");

            dialog.CurrentIndex = 1;
            dialog.EffectivePreviewTitle.ShouldBe("second.png");

            dialog.CurrentIndex = 12;
            dialog.EffectivePreviewTitle.ShouldBe("second.png");

            dialog.CurrentIndex = -5;
            dialog.EffectivePreviewTitle.ShouldBe("first.png");
        });
    }

    [Fact]
    public void ImagePreviewerDialog_Title_Recomputes_When_Resolver_Changes()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var dialog = CreateDialog("avares://AtomUI.Tests/Assets/source.png");

            dialog.EffectivePreviewTitle.ShouldBe("source.png");

            dialog.PreviewTitleResolver = new PrefixTitleResolver("custom");

            dialog.EffectivePreviewTitle.ShouldBe("custom:source.png:1/1");
        });
    }

    private static string? Resolve(string source)
    {
        var sourceUri = ImageSourceUri.Parse(source);
        var context   = new ImagePreviewTitleResolveContext(sourceUri, 0, 1);
        return DefaultImagePreviewTitleResolver.Instance.ResolveTitle(context);
    }

    private static ImagePreviewerDialog CreateDialog(params string[] sources)
    {
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer();
        var dialog = new ImagePreviewerDialog(new Avalonia.Controls.Window(), previewer)
        {
            ItemsSource = sources.Select(source => new ImagePreviewItem(ImageSourceUri.Parse(source))).ToList()
        };
        return dialog;
    }

    private sealed class PrefixTitleResolver : IImagePreviewTitleResolver
    {
        private readonly string _prefix;

        public PrefixTitleResolver(string prefix)
        {
            _prefix = prefix;
        }

        public string? ResolveTitle(ImagePreviewTitleResolveContext context)
        {
            var fileName = DefaultImagePreviewTitleResolver.Instance.ResolveTitle(context);
            return $"{_prefix}:{fileName}:{context.CurrentIndex + 1}/{context.Count}";
        }
    }
}
