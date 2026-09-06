using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;

namespace AtomUI.Toolkits.GalleryBase.Tests.Controls;

public class SemanticPartPreviewLayoutTests
{
    static SemanticPartPreviewLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Preview_Content_Stays_Within_The_Host_Height_Clamp()
    {
        var preview = new SemanticPartPreview
        {
            Width = 900,
            Title = "DatePicker",
            PreviewStageMinHeight = 560,
            PreviewContentAlignment = VerticalAlignment.Top,
            PreviewContent = new Border
            {
                Height = 560,
                Background = Brushes.Transparent
            }
        };

        // 模拟 GalleryShowCaseHost 把语义内容钳制到视口剩余高度的场景。
        var host = new ContentPresenter { MaxHeight = 500, Content = preview };
        var window = new Window
        {
            Width = 980,
            Height = 720,
            Content = new StackPanel
            {
                Children = { new Border { Height = 120 }, host }
            }
        };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();

            preview.Bounds.Height.ShouldBeLessThanOrEqualTo(500,
                "preview card must respect the host height clamp instead of spilling out of the viewport");

            var stage = preview.GetVisualDescendants()
                               .OfType<Border>()
                               .First(control => control.Name == "PART_PreviewStage");
            stage.Bounds.Height.ShouldBeLessThanOrEqualTo(500);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Preview_Stage_Grows_To_The_Configured_MinHeight_When_Space_Allows()
    {
        var preview = new SemanticPartPreview
        {
            Width = 900,
            Title = "DatePicker",
            PreviewStageMinHeight = 560,
            PreviewContentAlignment = VerticalAlignment.Top,
            PreviewContent = new TextBox()
        };

        var window = new Window
        {
            Width = 980,
            Height = 900,
            Content = preview
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            Dispatcher.UIThread.RunJobs();

            var stage = preview.GetVisualDescendants()
                               .OfType<Border>()
                               .First(control => control.Name == "PART_PreviewStage");
            stage.Bounds.Height.ShouldBeGreaterThanOrEqualTo(560);
        }
        finally
        {
            window.Close();
        }
    }
}
