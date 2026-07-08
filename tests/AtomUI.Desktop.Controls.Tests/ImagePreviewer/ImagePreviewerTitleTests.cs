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
        Resolve("https://picsum.photos/id/25/600/400").ShouldBe("25");
        Resolve("avares://AtomUI.Tests/Assets/resource-icon.svg").ShouldBe("resource-icon.svg");
    }

    [Fact]
    public void DefaultImagePreviewTitleResolver_Uses_DisplayName_From_Stream_Source()
    {
        var source = new StreamImagePreviewSource(
            _ => new ValueTask<Stream>(Stream.Null),
            displayName: "stream image.png",
            contentType: "image/png");
        var context = new ImagePreviewTitleResolveContext(source, 0, 1);

        DefaultImagePreviewTitleResolver.Instance.ResolveTitle(context).ShouldBe("stream image.png");
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

    [Fact]
    public void ImagePreviewerDialog_ItemsSource_Change_Preserves_CurrentIndex()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer();
            var dialog = new ImagePreviewerDialog(new Avalonia.Controls.Window(), previewer)
            {
                CurrentIndex = 1,
                ItemsSource =
                [
                    new ImagePreviewItem(new UriImagePreviewSource("avares://AtomUI.Tests/Assets/first.png")),
                    new ImagePreviewItem(new UriImagePreviewSource("avares://AtomUI.Tests/Assets/second.png"))
                ]
            };

            dialog.CurrentIndex.ShouldBe(1);
            dialog.EffectivePreviewTitle.ShouldBe("second.png");
            dialog.IsFirstImage.ShouldBeFalse();
            dialog.IsLastImage.ShouldBeTrue();
        });
    }

    [Fact]
    public void ImagePreviewerOverlayHost_ItemsSource_Change_Preserves_CurrentIndex()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer();
            var host = new ImagePreviewerOverlayHost(new Avalonia.Controls.Window(), previewer)
            {
                CurrentIndex = 1,
                ItemsSource =
                [
                    new ImagePreviewItem(new UriImagePreviewSource("avares://AtomUI.Tests/Assets/first.png")),
                    new ImagePreviewItem(new UriImagePreviewSource("avares://AtomUI.Tests/Assets/second.png"))
                ]
            };

            host.CurrentIndex.ShouldBe(1);
            host.IsFirstImage.ShouldBeFalse();
            host.IsLastImage.ShouldBeTrue();
        });
    }

    [Fact]
    public void ImagePreviewer_Exposes_PreviewTitleIcon_As_PathIcon_Without_Window_Icon_Fallback()
    {
        var previewerSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/ImagePreviewer/AbstractImagePreviewer.cs"));
        var dialogSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerDialog.cs"));
        var titleBarSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/ImagePreviewer/ImagePreviewerTitleBar.cs"));

        previewerSource.ShouldContain("StyledProperty<PathIcon?> PreviewTitleIconProperty");
        previewerSource.ShouldContain("nameof(PreviewTitleIcon)");
        previewerSource.ShouldContain("public PathIcon? PreviewTitleIcon");
        previewerSource.ShouldContain("BindUtils.RelayBind(this, PreviewTitleIconProperty, dialogHost, ImagePreviewerDialog.TitleIconProperty)");
        previewerSource.ShouldNotContain("StyledProperty<PathIcon?> IconProperty");
        previewerSource.ShouldNotContain("public PathIcon? Icon");
        previewerSource.ShouldNotContain("PreviewWindowIcon");
        previewerSource.ShouldNotContain("Window.IconProperty");

        dialogSource.ShouldContain("StyledProperty<PathIcon?> TitleIconProperty");
        dialogSource.ShouldContain("previewerTitleBar[!ImagePreviewerTitleBar.IconProperty]");
        dialogSource.ShouldContain("this[!TitleIconProperty]");
        dialogSource.ShouldNotContain("titleBar[!WindowTitleBar.LogoProperty]");
        dialogSource.ShouldNotContain("titleBar[!WindowTitleBar.LogoTemplateProperty]");

        titleBarSource.ShouldContain("StyledProperty<PathIcon?> IconProperty");
        titleBarSource.ShouldContain("public PathIcon? Icon");
    }

    private static string? Resolve(string source)
    {
        var sourceUri = new UriImagePreviewSource(source);
        var context   = new ImagePreviewTitleResolveContext(sourceUri, 0, 1);
        return DefaultImagePreviewTitleResolver.Instance.ResolveTitle(context);
    }

    private static ImagePreviewerDialog CreateDialog(params string[] sources)
    {
        var previewer = new global::AtomUI.Desktop.Controls.ImagePreviewer();
        var dialog = new ImagePreviewerDialog(new Avalonia.Controls.Window(), previewer)
        {
            ItemsSource = sources.Select(source => new ImagePreviewItem(new UriImagePreviewSource(source))).ToList()
        };
        return dialog;
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
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
