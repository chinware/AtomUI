using AtomUI.Controls.Primitives;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Cascader;

public class CascaderSearchSelectionTests
{
    static CascaderSearchSelectionTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Selecting_Filter_Result_Expands_And_Selects_Matched_Path()
    {
        var (options, lingyin) = CreateProvinceOptions();
        var cascaderView = new CascaderView
        {
            IsMotionEnabled      = false,
            IsShowEmptyIndicator = false,
            OptionsSource        = options
        };
        var window = CreateWindow(cascaderView);

        try
        {
            cascaderView.FilterValue = "lin";
            Dispatcher.UIThread.RunJobs();

            var filterList = WaitFor(
                () => cascaderView.GetVisualDescendants()
                                  .OfType<CascaderViewFilterList>()
                                  .FirstOrDefault(list => list.IsVisible),
                "the filter result list should be visible after setting a filter value.");
            filterList.ItemCount.ShouldBe(1);

            filterList.SelectedIndex = 0;
            Dispatcher.UIThread.RunJobs();

            var zhejiangItem = WaitFor(
                () => FindCascaderViewItem(cascaderView, "Zhejiang"),
                "the root option should remain visible after selecting a filter result.");
            var hangzhouItem = WaitFor(
                () => FindCascaderViewItem(cascaderView, "Hangzhou"),
                "selecting a filter result should expand the matched parent path.");
            var lingyinItem = WaitFor(
                () => FindCascaderViewItem(cascaderView, "Lingyin shi"),
                "selecting a filter result should realize the matched leaf item.");

            cascaderView.SelectedOption.ShouldBeSameAs(lingyin);
            zhejiangItem.IsExpanded.ShouldBeTrue();
            hangzhouItem.IsExpanded.ShouldBeTrue();
            lingyinItem.IsSelected.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Selecting_Filter_Result_In_Cascader_Popup_Updates_Selection_Without_Keeping_Filter_Mode()
    {
        var (options, lingyin) = CreateProvinceOptions();
        var cascader = new Desktop.Controls.Cascader
        {
            Width           = 240,
            IsFilterEnabled = true,
            IsMotionEnabled = false,
            OptionsSource   = options
        };
        var window = CreateWindow(cascader);

        try
        {
            cascader.IsDropDownOpen = true;
            Dispatcher.UIThread.RunJobs();

            var cascaderView = GetCascaderView(cascader);
            cascader.FilterValue = "lin";
            Dispatcher.UIThread.RunJobs();

            var filterList = WaitFor(
                () => cascaderView.GetVisualDescendants()
                                  .OfType<CascaderViewFilterList>()
                                  .FirstOrDefault(list => list.IsVisible),
                "the popup filter result list should be visible after setting a filter value.");
            filterList.ItemCount.ShouldBe(1);

            filterList.SelectedIndex = 0;
            Dispatcher.UIThread.RunJobs();

            cascader.SelectedOption.ShouldBeSameAs(lingyin);
            cascaderView.IsFiltering.ShouldBeFalse();
            cascaderView.FilterValue.ShouldBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Selecting_Filter_Result_From_Popup_Hosted_CascaderView_Clears_Filter_Before_Selecting()
    {
        var (options, lingyin) = CreateProvinceOptions();
        var cascaderView = new CascaderView
        {
            IsMotionEnabled      = false,
            IsShowEmptyIndicator = false,
            OptionsSource        = options
        };
        var popup = new Popup
        {
            Child           = cascaderView,
            IsMotionEnabled = false,
            IsOpen          = true
        };
        var root = new Panel();
        root.Children.Add(popup);
        var window = CreateWindow(root);

        try
        {
            cascaderView.FilterValue = "lin";
            Dispatcher.UIThread.RunJobs();

            var filterList = WaitFor(
                () => cascaderView.GetVisualDescendants()
                                  .OfType<CascaderViewFilterList>()
                                  .FirstOrDefault(list => list.IsVisible),
                "the popup-hosted filter result list should be visible after setting a filter value.");
            filterList.ItemCount.ShouldBe(1);

            filterList.SelectedIndex = 0;
            Dispatcher.UIThread.RunJobs();

            cascaderView.SelectedOption.ShouldBeSameAs(lingyin);
            cascaderView.IsFiltering.ShouldBeFalse();
            cascaderView.FilterValue.ShouldBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Selecting_Filter_Result_Does_Not_Require_Already_Realized_Path_Containers()
    {
        var (options, lingyin) = CreateProvinceOptions();
        var cascaderView = new CascaderView
        {
            IsMotionEnabled      = false,
            IsShowEmptyIndicator = false,
            OptionsSource        = options
        };
        var window = CreateWindow(cascaderView);

        try
        {
            cascaderView.FilterValue = "lin";
            Dispatcher.UIThread.RunJobs();

            var filterList = WaitFor(
                () => cascaderView.GetVisualDescendants()
                                  .OfType<CascaderViewFilterList>()
                                  .FirstOrDefault(list => list.IsVisible),
                "the filter result list should be visible after setting a filter value.");

            cascaderView.IsVisible = false;
            Dispatcher.UIThread.RunJobs();

            filterList.SelectedIndex = 0;
            Dispatcher.UIThread.RunJobs();

            cascaderView.IsVisible = true;
            Dispatcher.UIThread.RunJobs();

            cascaderView.SelectedOption.ShouldBeSameAs(lingyin);
            var lingyinItem = WaitFor(
                () => FindCascaderViewItem(cascaderView, "Lingyin shi"),
                "the matched leaf item should be realized after the CascaderView becomes visible again.");
            lingyinItem.IsSelected.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
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

    private static (IReadOnlyList<ICascaderOption> Options, ICascaderOption Lingyin) CreateProvinceOptions()
    {
        var lingyin = new CascaderOption
        {
            Header = "Lingyin shi",
            Value  = "lingyin"
        };
        var zhejiang = new CascaderOption
        {
            Header = "Zhejiang",
            Value  = "zhejiang",
            Children =
            [
                new CascaderOption
                {
                    Header = "Hangzhou",
                    Value  = "hangzhou",
                    Children =
                    [
                        new CascaderOption
                        {
                            Header = "West Lake",
                            Value  = "west-lake"
                        },
                        lingyin
                    ]
                }
            ]
        };
        var jiangsu = new CascaderOption
        {
            Header = "Jiangsu",
            Value  = "jiangsu"
        };

        return ([zhejiang, jiangsu], lingyin);
    }

    private static CascaderView GetCascaderView(Desktop.Controls.Cascader cascader)
    {
        var field = typeof(Desktop.Controls.Cascader).GetField(
            "_cascaderView",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        field.ShouldNotBeNull();
        var cascaderView = field.GetValue(cascader) as CascaderView;
        cascaderView.ShouldNotBeNull();
        return cascaderView!;
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

    private static void EnablePopupOverlayLayer(VisualLayerManager visualLayerManager)
    {
        var property = typeof(VisualLayerManager).GetProperty(
            "EnablePopupOverlayLayer",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        property.ShouldNotBeNull();
        property.SetValue(visualLayerManager, true);
    }
}
