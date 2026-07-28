using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Timeline;

public class TimelineThemeTests
{
    static TimelineThemeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Theme_Propagates_Orientation_And_Isolates_Vertical_Spacing()
    {
        var timelineTheme = ReadRepoFile(
            "src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineTheme.axaml");
        var itemTheme = ReadRepoFile(
            "src/AtomUI.Desktop.Controls/Timeline/Themes/TimelineItemTheme.axaml");

        timelineTheme.ShouldContain(
            "Orientation=\"{Binding Orientation, RelativeSource={RelativeSource AncestorType=atom:Timeline}}\"");
        itemTheme.ShouldContain("Orientation=\"{TemplateBinding Orientation}\"");
        itemTheme.ShouldContain(
            "IndicatorSpacing=\"{atom:SharedTokenResource UniformlyPaddingXS}\"");
        itemTheme.ShouldContain("Selector=\"^[Orientation=Vertical]\"");
        itemTheme.ShouldContain("Selector=\"^[Orientation=Horizontal]\"");
    }

    [Fact]
    public void Orientation_Propagates_Through_The_Runtime_Theme()
    {
        var item = new Desktop.Controls.TimelineItem
        {
            Label   = "Label",
            Content = "Content"
        };
        var timeline = new Desktop.Controls.Timeline
        {
            Width       = 600,
            Orientation = Orientation.Horizontal,
            Items       = { item }
        };

        ShowInWindow(timeline, () =>
        {
            var stackPanel = timeline.GetVisualDescendants()
                                     .OfType<StackPanel>()
                                     .Single(panel => panel.GetType().Name == "TimelineStackPanel");
            var itemPanel = item.GetVisualDescendants()
                                .OfType<Panel>()
                                .Single(panel => panel.Name == "RootLayout");
            var indicator = item.GetVisualDescendants()
                                .OfType<Control>()
                                .Single(control => control.Name == "Indicator");

            stackPanel.Orientation.ShouldBe(Orientation.Horizontal);
            GetOrientation(itemPanel).ShouldBe(Orientation.Horizontal);
            GetOrientation(indicator).ShouldBe(Orientation.Horizontal);

            timeline.Orientation = Orientation.Vertical;
            Dispatcher.UIThread.RunJobs();

            stackPanel.Orientation.ShouldBe(Orientation.Vertical);
            GetOrientation(itemPanel).ShouldBe(Orientation.Vertical);
            GetOrientation(indicator).ShouldBe(Orientation.Vertical);
        });
    }

    private static Orientation GetOrientation(Control control)
    {
        return control.GetType()
                      .GetProperty("Orientation")!
                      .GetValue(control)
                      .ShouldBeOfType<Orientation>();
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 760,
            Height  = 320,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }

    private static string ReadRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
