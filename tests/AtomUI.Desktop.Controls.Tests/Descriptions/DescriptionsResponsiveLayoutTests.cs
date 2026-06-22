using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaGrid = Avalonia.Controls.Grid;
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
            ColumnInfo = ResponsiveInt.Parse("xs: 1, md: 2, lg: 3")
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

    [Fact]
    public void Descriptions_ColumnInfo_Uses_Mobile_First_Cascade()
    {
        var descriptions = new AtomUI.Desktop.Controls.Descriptions
        {
            ColumnInfo = ResponsiveInt.Parse("xs: 1, md: 3")
        };

        descriptions.ColumnInfo!.Value.Resolve(MediaBreakPoint.Large, 9).ShouldBe(3);
    }

    [Fact]
    public void DescriptionItem_Span_Uses_Mobile_First_Cascade()
    {
        var item = new DescriptionItem
        {
            Span = ResponsiveInt.Parse("xs: 1, md: 3")
        };

        item.Span.Resolve(MediaBreakPoint.ExtraExtraLarge, 1).ShouldBe(3);
    }

    [Fact]
    public void Descriptions_Rebuilds_Item_Visuals_When_Horizontal_Bordered_State_Changes()
    {
        var descriptions = new AtomUI.Desktop.Controls.Descriptions();
        descriptions.Items.Add(new DescriptionItem { Label = "Product", Content = "Cloud Database" });
        descriptions.Items.Add(new DescriptionItem { Label = "Billing", Content = "Prepaid" });

        ShowInWindow(descriptions, () =>
        {
            CountVisuals<DescriptionDefaultItem>(descriptions).ShouldBe(2);

            descriptions.IsBordered = true;
            Dispatcher.UIThread.RunJobs();

            CountVisuals<DescriptionDefaultItem>(descriptions).ShouldBe(0);
            CountVisuals<DescriptionBorderedItemLabel>(descriptions).ShouldBe(2);
            CountVisuals<DescriptionBorderedItemContent>(descriptions).ShouldBe(2);

            descriptions.IsBordered = false;
            Dispatcher.UIThread.RunJobs();

            CountVisuals<DescriptionDefaultItem>(descriptions).ShouldBe(2);
            CountVisuals<DescriptionBorderedItemLabel>(descriptions).ShouldBe(0);
            CountVisuals<DescriptionBorderedItemContent>(descriptions).ShouldBe(0);
        });
    }

    [Fact]
    public void Descriptions_Layouts_Value_Equal_Items_By_Position()
    {
        var descriptions = new AtomUI.Desktop.Controls.Descriptions
        {
            ColumnInfo = 1
        };
        descriptions.Items.Add(new DescriptionItem { Label = "Same", Content = "Same content" });
        descriptions.Items.Add(new DescriptionItem { Label = "Same", Content = "Same content" });

        ShowInWindow(descriptions, () =>
        {
            var generatedItems = descriptions.GetSelfAndVisualDescendants()
                                             .OfType<DescriptionDefaultItem>()
                                             .ToList();

            generatedItems.Count.ShouldBe(2);
            AvaloniaGrid.GetRow(generatedItems[0]).ShouldBe(0);
            AvaloniaGrid.GetRow(generatedItems[1]).ShouldBe(1);
        });
    }

    private static void AddDescriptionItems(AtomUI.Desktop.Controls.Descriptions descriptions)
    {
        descriptions.Items.Add(new DescriptionItem { Label = "Product", Content = "Cloud Database" });
        descriptions.Items.Add(new DescriptionItem { Label = "Billing Mode", Content = "Prepaid" });
        descriptions.Items.Add(new DescriptionItem { Label = "Automatic Renewal", Content = "YES" });
        descriptions.Items.Add(new DescriptionItem { Label = "Order Time", Content = "2018-04-24 18:00:00" });
        descriptions.Items.Add(new DescriptionItem { Label = "Usage Time", Content = "2019-04-24 18:00:00", Span = 2 });
        descriptions.Items.Add(new DescriptionItem { Label = "Status", Content = "Running", Span = 3 });
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 800,
            Height  = 600,
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

    private static int CountVisuals<T>(Control root)
        where T : Control
    {
        return root.GetSelfAndVisualDescendants().OfType<T>().Count();
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
