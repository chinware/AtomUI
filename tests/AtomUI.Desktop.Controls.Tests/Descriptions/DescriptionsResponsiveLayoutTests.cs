using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Descriptions;

public class DescriptionsResponsiveLayoutTests
{
    static DescriptionsResponsiveLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Descriptions_Invalidates_Measure_When_Media_Breakpoint_Changes()
    {
        var host = new TestMediaBreakHost
        {
            Width  = 800,
            Height = 600
        };
        var descriptions = new AtomUI.Desktop.Controls.Descriptions
        {
            IsBordered = true,
            ColumnInfo = new DescriptionsMediaBreakInfo(1, 1, 2, 3, 3, 3)
        };
        AddDescriptionItems(descriptions);
        host.Children.Add(descriptions);

        var window = new AvaloniaWindow
        {
            Width   = 800,
            Height  = 600,
            Content = host
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            descriptions.IsMeasureValid.ShouldBeTrue();

            host.SetMediaBreakPoint(MediaBreakPoint.ExtraSmall);

            descriptions.IsMeasureValid.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    private static void AddDescriptionItems(AtomUI.Desktop.Controls.Descriptions descriptions)
    {
        descriptions.Items.Add(new DescriptionItem { Label = "Product", Content = "Cloud Database" });
        descriptions.Items.Add(new DescriptionItem { Label = "Billing Mode", Content = "Prepaid" });
        descriptions.Items.Add(new DescriptionItem { Label = "Automatic Renewal", Content = "YES" });
        descriptions.Items.Add(new DescriptionItem { Label = "Order Time", Content = "2018-04-24 18:00:00" });
        descriptions.Items.Add(new DescriptionItem { Label = "Usage Time", Content = "2019-04-24 18:00:00", Span = new DescriptionsMediaBreakInfo(2) });
        descriptions.Items.Add(new DescriptionItem { Label = "Status", Content = "Running", Span = new DescriptionsMediaBreakInfo(3) });
    }

    private sealed class TestMediaBreakHost : Panel, IMediaBreakAwareControl
    {
        public MediaBreakPoint MediaBreakPoint { get; private set; } = MediaBreakPoint.Large;

        public event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;

        public void SetMediaBreakPoint(MediaBreakPoint mediaBreakPoint)
        {
            MediaBreakPoint = mediaBreakPoint;
            MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(mediaBreakPoint));
        }
    }
}
