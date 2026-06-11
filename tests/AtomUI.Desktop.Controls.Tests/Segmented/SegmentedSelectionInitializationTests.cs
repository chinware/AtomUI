using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Segmented;

public class SegmentedSelectionInitializationTests
{
    static SegmentedSelectionInitializationTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Bound_SelectedIndex_Is_Preserved_When_Template_Is_Applied()
    {
        var viewModel = new SegmentedSelectionViewModel
        {
            SectionIndex = 1
        };
        var segmented = CreateSegmented();
        segmented.Bind(SelectingItemsControl.SelectedIndexProperty, new Binding(nameof(SegmentedSelectionViewModel.SectionIndex))
        {
            Source = viewModel,
            Mode   = BindingMode.TwoWay
        });

        ShowInWindow(segmented, () =>
        {
            segmented.SelectedIndex.ShouldBe(1);
            viewModel.SectionIndex.ShouldBe(1);
        });
    }

    [Fact]
    public void Explicit_SelectedIndex_Is_Preserved_When_Template_Is_Applied()
    {
        var segmented = CreateSegmented();
        segmented.SelectedIndex = 1;

        ShowInWindow(segmented, () => segmented.SelectedIndex.ShouldBe(1));
    }

    [Fact]
    public void First_Item_Is_Selected_When_No_Selection_Is_Provided()
    {
        var segmented = CreateSegmented();

        ShowInWindow(segmented, () => segmented.SelectedIndex.ShouldBe(0));
    }

    private static AtomUI.Desktop.Controls.Segmented CreateSegmented()
    {
        var segmented = new AtomUI.Desktop.Controls.Segmented
        {
            IsMotionEnabled = false
        };
        segmented.Items.Add("Overview");
        segmented.Items.Add("Details");
        segmented.Items.Add("History");
        return segmented;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new Avalonia.Controls.Window
        {
            Width   = 320,
            Height  = 240,
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

    private sealed class SegmentedSelectionViewModel
    {
        public int SectionIndex { get; set; }
    }
}
