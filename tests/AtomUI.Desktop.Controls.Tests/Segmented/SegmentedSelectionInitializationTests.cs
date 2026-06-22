using AtomUI.Controls;
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

    [Fact]
    public void Form_Item_Set_Value_Selects_Provided_Item()
    {
        var segmented = CreateSegmented();
        var formItem  = (IFormItemAware)segmented;

        ShowInWindow(segmented, () =>
        {
            formItem.SetFormValue("History");

            segmented.SelectedItem.ShouldBe("History");
            segmented.SelectedIndex.ShouldBe(2);
            formItem.GetFormValue().ShouldBe("History");
        });
    }

    [Fact]
    public void Expanding_Layout_Distributes_Width_To_Visible_Items()
    {
        var first = new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content = "First"
        };
        var hidden = new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content   = "Hidden",
            IsVisible = false
        };
        var second = new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content = "Second"
        };
        var segmented = new AtomUI.Desktop.Controls.Segmented
        {
            IsExpanding     = true,
            IsMotionEnabled = false,
            Width           = 300
        };
        segmented.Items.Add(first);
        segmented.Items.Add(hidden);
        segmented.Items.Add(second);

        ShowInWindow(segmented, () =>
        {
            var expectedItemWidth = (segmented.Bounds.Width - segmented.Padding.Left - segmented.Padding.Right) / 2;
            first.Bounds.Width.ShouldBe(expectedItemWidth, 0.5);
            second.Bounds.Width.ShouldBe(expectedItemWidth, 0.5);
        });
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
