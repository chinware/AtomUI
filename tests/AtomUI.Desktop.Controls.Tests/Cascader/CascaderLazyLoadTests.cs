using System.Reflection;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls.DataLoad;
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

public class CascaderLazyLoadTests
{
    static CascaderLazyLoadTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Clicking_Unloaded_Async_Node_Keeps_DropDown_Open_And_Shows_Loaded_Children()
    {
        var zhejiang = new CascaderOption
        {
            Header = "Zhejiang",
            Value  = "zhejiang",
            IsLeaf = false
        };
        var jiangsu = new CascaderOption
        {
            Header = "Jiangsu",
            Value  = "jiangsu",
            IsLeaf = false,
            Children =
            [
                new CascaderOption { Header = "Nanjing", Value = "nanjing", IsLeaf = true }
            ]
        };
        CascaderViewItem? zhejiangItem = null;
        var observedDropDownOpenWhileLoading = false;
        var observedItemLoading              = false;
        var cascader = new AtomUI.Desktop.Controls.Cascader
        {
            Width           = 240,
            IsMotionEnabled = false,
            OptionsSource   = new[] { zhejiang, jiangsu }
        };
        cascader.DataLoader = new ObservingCascaderItemDataLoader(
            targetNode =>
            {
                targetNode.ShouldBeSameAs(zhejiang);
                observedDropDownOpenWhileLoading = cascader.IsDropDownOpen;
                observedItemLoading              = zhejiangItem?.IsLoading == true;
            },
            [
                new CascaderOption { Header = "Hangzhou", Value = "hangzhou", IsLeaf = true },
                new CascaderOption { Header = "Ningbo", Value = "ningbo", IsLeaf = true }
            ]);
        var window = CreateWindow(cascader);

        try
        {
            cascader.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            zhejiangItem = WaitFor(
                () => FindCascaderViewItem(window, zhejiang),
                "the root lazy-load item should be realized after the dropdown opens.");

            Click(zhejiangItem, window);

            observedDropDownOpenWhileLoading.ShouldBeTrue(
                "the popup must remain open while an async parent node is loading.");
            observedItemLoading.ShouldBeTrue(
                "the clicked lazy-load item should show its loading indicator.");
            cascader.SelectedOption.ShouldBeNull("an unloaded async parent node is not a selectable leaf in single-select mode.");

            var hangzhouItem = WaitFor(
                () => FindCascaderViewItem(window, "Hangzhou"),
                "loaded children should appear in a new column without reopening the dropdown.");

            cascader.IsDropDownOpen.ShouldBeTrue();
            zhejiang.Children.Count.ShouldBe(2);
            zhejiangItem.IsLoading.ShouldBeFalse();
            hangzhouItem.ShouldNotBeNull();
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
            BindingFlags.Instance | BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }

    private sealed class ObservingCascaderItemDataLoader : ICascaderItemDataLoader
    {
        private readonly Action<ICascaderOption> _onLoad;
        private readonly IReadOnlyList<ICascaderOption> _children;

        public ObservingCascaderItemDataLoader(Action<ICascaderOption> onLoad, IReadOnlyList<ICascaderOption> children)
        {
            _onLoad  = onLoad;
            _children = children;
        }

        public async Task<CascaderItemLoadResult> LoadAsync(ICascaderOption targetNode, CancellationToken token)
        {
            _onLoad(targetNode);
            await Task.Delay(20, token).ConfigureAwait(false);
            return new CascaderItemLoadResult
            {
                IsSuccess = true,
                Data      = _children
            };
        }
    }
}
