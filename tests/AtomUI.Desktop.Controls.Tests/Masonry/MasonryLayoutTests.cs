using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AtomUI.Controls;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Masonry;

public class MasonryLayoutTests
{
    static MasonryLayoutTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Default_Layout_Property_Values_Are_Stable()
    {
        var masonry = new AtomUI.Desktop.Controls.Masonry();

        masonry.ColumnCount.ShouldBe(0);
        masonry.ColumnInfo.ShouldBeNull();
        masonry.MinColumnWidth.ShouldBe(320d);
        masonry.MaxColumnCount.ShouldBe(4);
        masonry.ColumnGap.ShouldBe(16d);
        masonry.RowGap.ShouldBe(16d);
        masonry.Gutter.ShouldBeNull();
    }

    /// <summary>
    /// Regression: a fixed ColumnCount=4 with many varying-height items must arrange the
    /// generated containers into 4 distinct x-columns and never overlap. Previously the
    /// Masonry items collapsed into a single solid block in the Gallery.
    /// </summary>
    [Fact]
    public void Fixed_Column_Count_Produces_Non_Overlapping_Columns()
    {
        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnCount = 4,
            ColumnGap   = 16,
            RowGap      = 16,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemTemplate = new FuncDataTemplate<_Item>(
                _ => true,
                item => new Border
                {
                    Height = item!.Height,
                    Background = Brushes.White,
                    Child = new TextBlock { Text = item.Index.ToString() }
                })
        };

        var items = new ObservableCollection<_Item>();
        var heights = new double[] { 150, 50, 90, 70, 180, 150, 130, 80, 50, 90, 100, 150, 60, 50, 80 };
        for (var i = 0; i < heights.Length; i++)
        {
            items.Add(new _Item(i + 1, heights[i]));
        }
        masonry.ItemsSource = items;

        var window = new AvaloniaWindow
        {
            Width   = 1024,
            Height  = 800,
            Content = masonry
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            // Collect every generated item container (ContentPresenter wrapping the template).
            var presenters = masonry
                .GetLogicalDescendants()
                .OfType<ContentPresenter>()
                .ToArray();

            presenters.Length.ShouldBe(heights.Length,
                "Masonry should generate one container per item");

            // After layout, every container must have a non-zero arranged rect.
            foreach (var p in presenters)
            {
                p.Bounds.Width.ShouldBeGreaterThan(0);
                p.Bounds.Height.ShouldBeGreaterThan(0);
            }

            // Group containers by their column (x position bucket). Expect <= 4 columns.
            var xs = presenters.Select(p => Math.Round(p.Bounds.X)).Distinct().OrderBy(v => v).ToArray();
            xs.Length.ShouldBeLessThanOrEqualTo(4);
            xs.Length.ShouldBeGreaterThan(1,
                "Items must spread across multiple columns, not collapse into one");

            // No two same-column items may vertically overlap.
            foreach (var group in presenters.GroupBy(p => Math.Round(p.Bounds.X)))
            {
                var ordered = group.OrderBy(p => p.Bounds.Y).ToArray();
                for (var i = 1; i < ordered.Length; i++)
                {
                    var prevBottom = ordered[i - 1].Bounds.Bottom;
                    ordered[i].Bounds.Y.ShouldBeGreaterThanOrEqualTo(prevBottom,
                        $"item at x={group.Key} overlaps previous item (prevBottom={prevBottom})");
                }
            }
        }
        finally
        {
            window.Close();
        }
    }

    /// <summary>
    /// Regression: an item whose template contains a child with an intrinsic (large) desired
    /// size — e.g. an Image whose source loads asynchronously — must NOT inflate the masonry
    /// card beyond its bound Height. Previously the cover image's native size leaked into the
    /// card's DesiredSize, so MasonryPanel (which lays out by child.DesiredSize.Height) placed
    /// the card with the wrong height and the image overflowed onto neighbouring cards.
    /// The fix pins the cover region to a fixed-height border so the card height is known up
    /// front and does not change when the image arrives.
    /// </summary>
    [Fact]
    public void Cover_Image_Does_Not_Inflate_Card_Height()
    {
        // A child that reports a large intrinsic desired size, mirroring an Image whose source
        // has loaded with a native pixel size.
        Control BigCoverFactory() => new Border
        {
            Height = 100,                 // fixed cover region (the fix)
            ClipToBounds = true,
            Child = new Border { Width = 523, Height = 392, Background = Brushes.CornflowerBlue }
        };

        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnCount = 4,
            ColumnGap   = 16,
            RowGap      = 16,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemTemplate = new FuncDataTemplate<_Item>(
                _ => true,
                item =>
                {
                    // Card-like host: a fixed Height wrapping a content presenter.
                    return new Border
                    {
                        Height = item!.Height,
                        ClipToBounds = true,
                        Child = item.IsSpecial
                            ? new StackPanel { Children = { BigCoverFactory() } }
                            : (Control)new TextBlock { Text = item.Index.ToString() }
                    };
                })
        };

        var items = new ObservableCollection<_Item>
        {
            new _Item(1, 150, false),
            new _Item(2, 50, false),
            new _Item(3, 90, false),
            new _Item(4, 70, false),
            new _Item(5, 180, true),   // special card with the big cover
            new _Item(6, 150, false),
        };
        masonry.ItemsSource = items;

        var window = new AvaloniaWindow { Width = 1024, Height = 800, Content = masonry };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var presenters = masonry.GetLogicalDescendants().OfType<ContentPresenter>().ToArray();
            presenters.Length.ShouldBe(items.Count);

            // Each container's arranged height must equal the bound item height — never the
            // inflated 392px intrinsic cover size.
            foreach (var p in presenters)
            {
                p.Bounds.Height.ShouldBeLessThanOrEqualTo(200,
                    "card height must stay bounded by the bound Height, not the cover's intrinsic size");
                p.Bounds.Height.ShouldBeGreaterThan(0);
            }

            // The 4-column invariant still holds.
            var xs = presenters.Select(p => Math.Round(p.Bounds.X)).Distinct().OrderBy(v => v).ToArray();
            xs.Length.ShouldBeLessThanOrEqualTo(4);
            xs.Length.ShouldBeGreaterThan(1);

            // No vertical overlap within a column.
            foreach (var group in presenters.GroupBy(p => Math.Round(p.Bounds.X)))
            {
                var ordered = group.OrderBy(p => p.Bounds.Y).ToArray();
                for (var i = 1; i < ordered.Length; i++)
                {
                    ordered[i].Bounds.Y.ShouldBeGreaterThanOrEqualTo(ordered[i - 1].Bounds.Bottom);
                }
            }
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Responsive_ColumnInfo_Cascades_To_Current_Breakpoint()
    {
        var host = new TestMediaBreakHost(MediaBreakPoint.Large)
        {
            Width = 900,
            Height = 600
        };
        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnInfo = ResponsiveInt.Parse("xs: 1, md: 3"),
            ColumnGap = 0,
            RowGap = 0,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemTemplate = new FuncDataTemplate<_Item>(
                _ => true,
                item => new Border { Height = item!.Height, Child = new TextBlock { Text = item.Index.ToString() } })
        };

        masonry.ItemsSource = new ObservableCollection<_Item>
        {
            new(1, 40),
            new(2, 40),
            new(3, 40),
            new(4, 40),
            new(5, 40),
            new(6, 40)
        };
        host.Children.Add(masonry);

        var window = new AvaloniaWindow { Width = 900, Height = 600, Content = host };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var presenters = masonry.GetLogicalDescendants().OfType<ContentPresenter>().ToArray();
            var xs = presenters.Select(p => Math.Round(p.Bounds.X)).Distinct().OrderBy(v => v).ToArray();
            xs.Length.ShouldBe(3);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Responsive_ColumnInfo_Falls_Back_To_ColumnCount_When_Current_Breakpoint_Is_Not_Configured()
    {
        var host = new TestMediaBreakHost(MediaBreakPoint.Large)
        {
            Width = 900,
            Height = 600
        };
        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnInfo = ResponsiveInt.Parse("xl: 4"),
            ColumnCount = 2,
            ColumnGap = 0,
            RowGap = 0,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemTemplate = new FuncDataTemplate<_Item>(
                _ => true,
                item => new Border { Height = item!.Height, Child = new TextBlock { Text = item.Index.ToString() } })
        };

        masonry.ItemsSource = new ObservableCollection<_Item>
        {
            new(1, 40),
            new(2, 40),
            new(3, 40),
            new(4, 40)
        };
        host.Children.Add(masonry);

        var window = new AvaloniaWindow { Width = 900, Height = 600, Content = host };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var presenters = masonry.GetLogicalDescendants().OfType<ContentPresenter>().ToArray();
            presenters.Select(p => Math.Round(p.Bounds.X)).Distinct().Count().ShouldBe(2);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Responsive_Gutter_Cascades_Horizontal_And_Vertical_Independently()
    {
        var host = new TestMediaBreakHost(MediaBreakPoint.Large)
        {
            Width = 500,
            Height = 600
        };
        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnInfo = 2,
            Gutter = ResponsiveGutter.Parse("xs: 8, md: 20; xs: 4, md: 12"),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemTemplate = new FuncDataTemplate<_Item>(
                _ => true,
                item => new Border { Height = item!.Height, Child = new TextBlock { Text = item.Index.ToString() } })
        };

        masonry.ItemsSource = new ObservableCollection<_Item>
        {
            new(1, 40),
            new(2, 40),
            new(3, 40)
        };
        host.Children.Add(masonry);

        var window = new AvaloniaWindow { Width = 500, Height = 600, Content = host };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var presenters = masonry.GetLogicalDescendants().OfType<ContentPresenter>().OrderBy(p => p.Bounds.Y).ThenBy(p => p.Bounds.X).ToArray();
            presenters[1].Bounds.X.ShouldBe(260, 0.5);
            presenters[2].Bounds.Y.ShouldBe(52, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Responsive_Gutter_Falls_Back_To_ColumnGap_And_RowGap_When_Current_Breakpoint_Is_Not_Configured()
    {
        var host = new TestMediaBreakHost(MediaBreakPoint.Large)
        {
            Width = 500,
            Height = 600
        };
        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnInfo = 2,
            Gutter = ResponsiveGutter.Parse("xl: 30; xl: 30"),
            ColumnGap = 20,
            RowGap = 8,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemTemplate = new FuncDataTemplate<_Item>(
                _ => true,
                item => new Border { Height = item!.Height, Child = new TextBlock { Text = item.Index.ToString() } })
        };

        masonry.ItemsSource = new ObservableCollection<_Item>
        {
            new(1, 40),
            new(2, 40),
            new(3, 40)
        };
        host.Children.Add(masonry);

        var window = new AvaloniaWindow { Width = 500, Height = 600, Content = host };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var presenters = masonry.GetLogicalDescendants().OfType<ContentPresenter>().OrderBy(p => p.Bounds.Y).ThenBy(p => p.Bounds.X).ToArray();
            presenters[1].Bounds.X.ShouldBe(260, 0.5);
            presenters[2].Bounds.Y.ShouldBe(48, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Responsive_Gutter_Scalar_Applies_To_Column_And_Row_Gap()
    {
        var host = new TestMediaBreakHost(MediaBreakPoint.Large)
        {
            Width = 500,
            Height = 600
        };
        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnInfo = 2,
            Gutter = ResponsiveGutter.Parse("16"),
            RowGap = 4,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemTemplate = new FuncDataTemplate<_Item>(
                _ => true,
                item => new Border { Height = item!.Height, Child = new TextBlock { Text = item.Index.ToString() } })
        };

        masonry.ItemsSource = new ObservableCollection<_Item>
        {
            new(1, 40),
            new(2, 40),
            new(3, 40)
        };
        host.Children.Add(masonry);

        var window = new AvaloniaWindow { Width = 500, Height = 600, Content = host };
        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var presenters = masonry.GetLogicalDescendants().OfType<ContentPresenter>().OrderBy(p => p.Bounds.Y).ThenBy(p => p.Bounds.X).ToArray();
            presenters[1].Bounds.X.ShouldBe(258, 0.5);
            presenters[2].Bounds.Y.ShouldBe(56, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Responsive_Breakpoint_Change_Recalculates_Columns_And_Gutter()
    {
        var host = new TestMediaBreakHost(MediaBreakPoint.Small)
        {
            Width = 500,
            Height = 600
        };
        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnInfo = ResponsiveInt.Parse("xs: 1, md: 2"),
            Gutter = ResponsiveGutter.Parse("xs: 8, md: 20; xs: 4, md: 12"),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemTemplate = new FuncDataTemplate<_Item>(
                _ => true,
                item => new Border { Height = item!.Height, Child = new TextBlock { Text = item.Index.ToString() } })
        };

        masonry.ItemsSource = new ObservableCollection<_Item>
        {
            new(1, 40),
            new(2, 40),
            new(3, 40)
        };
        host.Children.Add(masonry);

        var window = new AvaloniaWindow { Width = 500, Height = 600, Content = host };
        try
        {
            window.Show();
            RunLayoutJobs();

            var presenters = masonry.GetLogicalDescendants().OfType<ContentPresenter>().OrderBy(p => p.Bounds.Y).ThenBy(p => p.Bounds.X).ToArray();
            presenters.Select(p => Math.Round(p.Bounds.X)).Distinct().Count().ShouldBe(1);
            presenters[2].Bounds.Y.ShouldBe(88, 0.5);

            host.SetMediaBreakPoint(MediaBreakPoint.Medium);
            RunLayoutJobs();

            presenters = masonry.GetLogicalDescendants().OfType<ContentPresenter>().OrderBy(p => p.Bounds.Y).ThenBy(p => p.Bounds.X).ToArray();
            presenters.Select(p => Math.Round(p.Bounds.X)).Distinct().Count().ShouldBe(2);
            presenters[1].Bounds.X.ShouldBe(260, 0.5);
            presenters[2].Bounds.Y.ShouldBe(52, 0.5);
        }
        finally
        {
            window.Close();
        }
    }

    private sealed class _Item
    {
        public int Index { get; }
        public double Height { get; }
        public bool IsSpecial { get; }
        public _Item(int index, double height, bool isSpecial = false)
        {
            Index = index;
            Height = height;
            IsSpecial = isSpecial;
        }
    }

    private sealed class _Vm
    {
        public ObservableCollection<_Item> Items { get; }
        public _Vm()
        {
            var heights = new double[] { 150, 50, 90, 70, 180, 150, 130, 80, 50, 90, 100, 150, 60, 50, 80 };
            var list = new ObservableCollection<_Item>();
            for (var i = 0; i < heights.Length; i++) list.Add(new _Item(i + 1, heights[i]));
            Items = list;
        }
    }

    /// <summary>
    /// Gallery-faithful reproduction: ItemsSource bound via DataContext (like the showcase's
    /// DeferredContentTemplate), and the Masonry hosted inside a ScrollViewer that offers an
    /// unconstrained (infinite) measure width — the real nesting path. Must still produce
    /// 4 distinct, non-overlapping columns rather than collapsing.
    /// </summary>
    [Fact]
    public void Gallery_Nesting_DataContext_Binding_And_Scroll_Host_Still_Columns()
    {
        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnCount = 4,
            ColumnGap   = 16,
            RowGap      = 16,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            ItemTemplate = new FuncDataTemplate<_Item>(
                _ => true,
                item => new TextBlock { Text = item!.Index.ToString() })
        };
        masonry.Bind(ItemsControl.ItemsSourceProperty, new Binding(nameof(_Vm.Items)) { Source = new _Vm() });

        // ScrollViewer is the gallery-like host that gives an infinite measure width to content.
        var scroll = new ScrollViewer
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            VerticalScrollBarVisibility   = ScrollBarVisibility.Auto,
            Width = 1024,
            Height = 800,
            Content = masonry
        };

        var window = new AvaloniaWindow { Width = 1100, Height = 900, Content = scroll };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var presenters = masonry.GetLogicalDescendants().OfType<ContentPresenter>().ToArray();
            presenters.Length.ShouldBe(15);

            var xs = presenters.Select(p => Math.Round(p.Bounds.X)).Distinct().OrderBy(v => v).ToArray();
            xs.Length.ShouldBe(4, $"expected exactly 4 columns, got {xs.Length} at x={string.Join(",", xs)}");

            // Column width must be the masonry-allocated column width, not stretched full-width.
            var colWidths = presenters.Select(p => p.Bounds.Width).Distinct().ToArray();
            foreach (var w in colWidths)
            {
                w.ShouldBeLessThan(900, "column width must be a fraction of the panel, not the full viewport");
            }

            // No vertical overlap within a column.
            foreach (var group in presenters.GroupBy(p => Math.Round(p.Bounds.X)))
            {
                var ordered = group.OrderBy(p => p.Bounds.Y).ToArray();
                for (var i = 1; i < ordered.Length; i++)
                {
                    ordered[i].Bounds.Y.ShouldBeGreaterThanOrEqualTo(ordered[i - 1].Bounds.Bottom);
                }
            }
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Direct_Child_Attached_Property_Change_Invalidates_Layout()
    {
        var first  = new Border { Height = 100, Background = Brushes.White };
        var second = new Border { Height = 100, Background = Brushes.White };
        var third  = new Border { Height = 20, Background = Brushes.White };

        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnCount = 2,
            ColumnGap   = 10,
            RowGap      = 0,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        masonry.Items.Add(first);
        masonry.Items.Add(second);
        masonry.Items.Add(third);

        var window = new AvaloniaWindow
        {
            Width   = 210,
            Height  = 300,
            Content = masonry
        };

        try
        {
            window.Show();
            RunLayoutJobs();

            Math.Round(third.Bounds.X).ShouldBe(0);

            AtomUI.Desktop.Controls.Masonry.SetColumn(third, 1);
            RunLayoutJobs();

            Math.Round(third.Bounds.X).ShouldBe(110,
                "changing Masonry.Column on a direct item container must trigger a new layout pass");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void LayoutChanged_Notifies_Once_When_ItemsSource_Becomes_Empty()
    {
        var items = new ObservableCollection<_Item>
        {
            new(1, 60),
            new(2, 60)
        };
        var notifications = new List<MasonryLayoutChangedEventArgs>();
        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnCount = 2,
            ColumnGap   = 10,
            RowGap      = 0,
            ItemsSource = items,
            ItemTemplate = new FuncDataTemplate<_Item>(
                _ => true,
                item => new Border
                {
                    Height = item!.Height,
                    Background = Brushes.White
                })
        };
        masonry.LayoutChanged += (_, args) => notifications.Add(args);

        var window = new AvaloniaWindow
        {
            Width   = 210,
            Height  = 300,
            Content = masonry
        };

        try
        {
            window.Show();
            RunLayoutJobs();
            notifications.Count.ShouldBe(1);
            notifications[0].Items.Count.ShouldBe(2);

            items.Clear();
            RunLayoutJobs();

            notifications.Count.ShouldBe(2,
                "transition from a non-empty layout to an empty layout must dispatch exactly one empty notification");
            notifications[1].Items.Count.ShouldBe(0);

            RunLayoutJobs();
            notifications.Count.ShouldBe(2,
                "a layout that remains empty must not repeatedly dispatch equivalent empty notifications");
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Replacing_ItemsPanel_Replaces_Default_Masonry_Layout_Engine()
    {
        var first  = new Border { Height = 40, Background = Brushes.White };
        var second = new Border { Height = 40, Background = Brushes.White };

        var masonry = new AtomUI.Desktop.Controls.Masonry
        {
            ColumnCount = 2,
            ColumnGap   = 10,
            RowGap      = 0,
            ItemsPanel  = new FuncTemplate<Panel?>(() => new StackPanel()),
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        masonry.Items.Add(first);
        masonry.Items.Add(second);

        var window = new AvaloniaWindow
        {
            Width   = 210,
            Height  = 300,
            Content = masonry
        };

        try
        {
            window.Show();
            RunLayoutJobs();

            Math.Round(first.Bounds.X).ShouldBe(0);
            Math.Round(second.Bounds.X).ShouldBe(0,
                "an explicit ItemsPanel must be honored instead of forcing the internal MasonryPanel");
            second.Bounds.Y.ShouldBeGreaterThan(0);
        }
        finally
        {
            window.Close();
        }
    }

    private static void RunLayoutJobs()
    {
        Dispatcher.UIThread.RunJobs();
        Dispatcher.UIThread.RunJobs();
    }

    private sealed class TestMediaBreakHost : Panel, IMediaBreakAwareControl
    {
        public TestMediaBreakHost(MediaBreakPoint breakPoint)
        {
            MediaBreakPoint = breakPoint;
        }

        public MediaBreakPoint MediaBreakPoint { get; private set; }

        public event EventHandler<MediaBreakPointChangedEventArgs>? MediaBreakPointChanged;

        public void SetMediaBreakPoint(MediaBreakPoint mediaBreakPoint)
        {
            MediaBreakPoint = mediaBreakPoint;
            MediaBreakPointChanged?.Invoke(this, new MediaBreakPointChangedEventArgs(mediaBreakPoint));
        }
    }
}
