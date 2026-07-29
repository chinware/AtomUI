using System.Linq;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
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
    public void Orientation_And_Shape_Use_Ant_Design_Defaults()
    {
        var segmentedType = typeof(AtomUI.Desktop.Controls.Segmented);
        var orientation   = segmentedType.GetProperty("Orientation");
        var shape         = segmentedType.GetProperty("Shape");

        orientation.ShouldNotBeNull();
        orientation.PropertyType.ShouldBe(typeof(Orientation));
        orientation.GetValue(new AtomUI.Desktop.Controls.Segmented()).ShouldBe(Orientation.Horizontal);

        shape.ShouldNotBeNull();
        shape.PropertyType.FullName.ShouldBe("AtomUI.Controls.SegmentedShape");
        shape.GetValue(new AtomUI.Desktop.Controls.Segmented())?.ToString().ShouldBe("Default");
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

    [Fact]
    public void Item_Text_Uses_Character_Ellipsis_When_Width_Is_Constrained()
    {
        var item = new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content = "longtext-longtext-longtext-longtext"
        };
        var segmented = new AtomUI.Desktop.Controls.Segmented
        {
            IsExpanding     = true,
            IsMotionEnabled = false,
            Width           = 120
        };
        segmented.Items.Add(item);

        ShowInWindow(segmented, () =>
        {
            var textBlock = item.GetVisualDescendants().OfType<Avalonia.Controls.TextBlock>().ShouldHaveSingleItem();
            textBlock.TextWrapping.ShouldBe(TextWrapping.NoWrap);
            textBlock.TextTrimming.ShouldBe(TextTrimming.CharacterEllipsis);
        });
    }

    [Fact]
    public void Vertical_Layout_Stacks_Items_And_Uses_Track_Width()
    {
        var first = new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content = "List"
        };
        var second = new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content = "Kanban"
        };
        var segmented = new AtomUI.Desktop.Controls.Segmented
        {
            Orientation     = Orientation.Vertical,
            IsMotionEnabled = false
        };
        segmented.Items.Add(first);
        segmented.Items.Add(second);

        ShowInWindow(segmented, () =>
        {
            second.Bounds.Y.ShouldBe(first.Bounds.Height, 0.5);
            first.Bounds.Width.ShouldBe(second.Bounds.Width, 0.5);
            first.Bounds.Width.ShouldBeGreaterThan(0);
        });
    }

    [Fact]
    public void Round_Shape_Applies_Capsule_Radius_To_Track_Item_And_Thumb()
    {
        var first = new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content = "Light"
        };
        var segmented = new AtomUI.Desktop.Controls.Segmented
        {
            Shape           = SegmentedShape.Round,
            IsMotionEnabled = false
        };
        segmented.Items.Add(first);
        segmented.Items.Add(new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content = "Dark"
        });

        ShowInWindow(segmented, () =>
        {
            segmented.CornerRadius.TopLeft.ShouldBeGreaterThan(1000);
            first.CornerRadius.TopLeft.ShouldBeGreaterThan(1000);

            var thumbRadius = typeof(AtomUI.Controls.Commons.AbstractSegmented)
                .GetProperty("SelectedThumbCornerRadius", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
                ?.GetValue(segmented);
            thumbRadius.ShouldBeOfType<CornerRadius>().TopLeft.ShouldBeGreaterThan(1000);
        });
    }

    [Fact]
    public void Direction_Keys_Wrap_And_Skip_Disabled_And_Hidden_Items()
    {
        var first = new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content = "First"
        };
        var disabled = new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content   = "Disabled",
            IsEnabled = false
        };
        var hidden = new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content   = "Hidden",
            IsVisible = false
        };
        var last = new AtomUI.Desktop.Controls.SegmentedItem
        {
            Content = "Last"
        };
        var segmented = new AtomUI.Desktop.Controls.Segmented
        {
            IsMotionEnabled = false
        };
        segmented.Items.Add(first);
        segmented.Items.Add(disabled);
        segmented.Items.Add(hidden);
        segmented.Items.Add(last);

        ShowInWindow(segmented, () =>
        {
            first.Focus();
            RaiseKey(first, Key.Right);
            segmented.SelectedIndex.ShouldBe(3);

            RaiseKey(last, Key.Down);
            segmented.SelectedIndex.ShouldBe(0);
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

    private static void RaiseKey(Control source, Key key)
    {
        source.RaiseEvent(new KeyEventArgs
        {
            RoutedEvent = InputElement.KeyDownEvent,
            Source      = source,
            Key         = key
        });
        Dispatcher.UIThread.RunJobs();
    }

    private sealed class SegmentedSelectionViewModel
    {
        public int SectionIndex { get; set; }
    }
}
