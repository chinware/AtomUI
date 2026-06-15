using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Cascader;

public class CascaderChangeOnSelectTests
{
    static CascaderChangeOnSelectTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Clicking_Parent_With_ChangeOnSelect_Selects_Immediately_And_Keeps_DropDown_Open()
    {
        var zhejiang = new CascaderOption
        {
            Header = "Zhejiang",
            Value  = "zhejiang",
            IsLeaf = false,
            Children =
            [
                new CascaderOption { Header = "Hangzhou", Value = "hangzhou", IsLeaf = true }
            ]
        };
        var cascader = new AtomUI.Desktop.Controls.Cascader
        {
            Width               = 240,
            IsMotionEnabled     = false,
            IsAllowSelectParent = true,
            OptionsSource       = new[] { zhejiang }
        };
        var window = CreateWindow(cascader);

        try
        {
            cascader.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var zhejiangItem = WaitFor(
                () => FindCascaderViewItem(window, zhejiang),
                "the root item should be realized after the dropdown opens.");

            Click(zhejiangItem, window);

            cascader.SelectedOption.ShouldBeSameAs(zhejiang);
            zhejiangItem.IsSelected.ShouldBeTrue();
            zhejiangItem.IsExpanded.ShouldBeTrue();
            cascader.IsDropDownOpen.ShouldBeTrue();
            FindCascaderViewItem(window, "Hangzhou").ShouldNotBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    private static AvaloniaWindow CreateWindow(Control content)
    {
        var overlayPanel = new ScopeAwareOverlayLayerPanel
        {
            Width  = 420,
            Height = 320
        };
        overlayPanel.Children.Add(content);

        var visualLayerManager = new VisualLayerManager
        {
            Child = overlayPanel
        };
        EnablePopupOverlayLayer(visualLayerManager);

        var window = new AvaloniaWindow
        {
            Width   = 420,
            Height  = 320,
            Content = visualLayerManager
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static void Click(Control control, AvaloniaWindow window)
    {
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window);

        point.ShouldNotBeNull();
        window.MouseMove(point.Value);
        window.MouseDown(point.Value, MouseButton.Left);
        window.MouseUp(point.Value, MouseButton.Left);
        Dispatcher.UIThread.RunJobs();
    }

    private static CascaderViewItem? FindCascaderViewItem(Visual root, ICascaderOption option)
    {
        return root.GetVisualDescendants()
                   .OfType<CascaderViewItem>()
                   .FirstOrDefault(x => ReferenceEquals(x.DataContext, option));
    }

    private static CascaderViewItem? FindCascaderViewItem(Visual root, string header)
    {
        return root.GetVisualDescendants()
                   .OfType<CascaderViewItem>()
                   .FirstOrDefault(x => x.DataContext is ICascaderOption option &&
                                        string.Equals(option.Header?.ToString(), header, StringComparison.Ordinal));
    }

    private static T WaitFor<T>(Func<T?> probe, string because)
        where T : class
    {
        for (var i = 0; i < 80; i++)
        {
            Dispatcher.UIThread.RunJobs();
            var value = probe();
            if (value != null)
            {
                return value;
            }
            Thread.Sleep(10);
        }

        throw new TimeoutException(because);
    }

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }
}
