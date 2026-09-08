using System.Diagnostics;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.ShowCases;

public class ImagePreviewerAssetLoadingTests
{
    static ImagePreviewerAssetLoadingTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/2.svg")]
    [InlineData("avares://AtomUIGallery/Assets/ImagePreviewerShowCase/3.svg")]
    public void Multiple_Preview_Gallery_Svg_Assets_Load_Through_The_Shared_Pipeline(string source)
    {
        var previewer = new ImagePreviewer
        {
            Width = 200,
            ItemsSource = [new ImagePreviewItem(ImageSource.Parse(source))]
        };
        var window = new Avalonia.Controls.Window
        {
            Width = 240,
            Height = 240,
            Content = previewer
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            previewer.Measure(new Size(200, 200));
            previewer.Arrange(new Rect(0, 0, 200, 200));
            WaitUntil(
                () => previewer.CoverLoadState is ImageLoadState.Loaded or ImageLoadState.Failed,
                $"ImagePreviewer source '{source}' to reach a terminal state");

            previewer.CoverLoadState.ShouldBe(
                ImageLoadState.Loaded,
                previewer.CoverLoadError is null
                    ? null
                    : $"{previewer.CoverLoadError.Code}: {previewer.CoverLoadError.Message}");
            previewer.IsCoverLoaded.ShouldBeTrue();
            previewer.IsCoverFailed.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    private static void WaitUntil(Func<bool> predicate, string description)
    {
        var timeout = Stopwatch.StartNew();
        while (!predicate())
        {
            Dispatcher.UIThread.RunJobs();
            if (timeout.Elapsed >= TimeSpan.FromSeconds(5))
            {
                throw new TimeoutException($"Timed out waiting for {description}.");
            }
            Thread.Yield();
        }
        Dispatcher.UIThread.RunJobs();
    }
}
