using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUICollapse = AtomUI.Desktop.Controls.Collapse;
using AtomUICollapseItem = AtomUI.Desktop.Controls.CollapseItem;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.CollapseControl;

public class CollapseBehaviorTests
{
    static CollapseBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Item_Padding_Overrides_Update_Template_Padding_Without_Overriding_Item_Local_Values()
    {
        var normalItem = new AtomUICollapseItem
        {
            Header     = "Normal",
            Content    = "Content",
            IsSelected = true
        };
        var explicitItem = new AtomUICollapseItem
        {
            Header         = "Explicit",
            Content        = "Content",
            IsSelected     = true,
            HeaderPadding  = new Thickness(3),
            ContentPadding = new Thickness(4)
        };
        var collapse = new AtomUICollapse
        {
            Items =
            {
                normalItem,
                explicitItem
            }
        };
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 240,
            Content = collapse
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            collapse.ItemHeaderPadding  = new Thickness(7);
            collapse.ItemContentPadding = new Thickness(9);
            Dispatcher.UIThread.RunJobs();

            GetHeaderDecorator(normalItem).Padding.ShouldBe(new Thickness(7));
            GetContentPresenter(normalItem).Padding.ShouldBe(new Thickness(9));
            GetHeaderDecorator(explicitItem).Padding.ShouldBe(new Thickness(3));
            GetContentPresenter(explicitItem).Padding.ShouldBe(new Thickness(4));

            collapse.ItemHeaderPadding  = new Thickness(11);
            collapse.ItemContentPadding = new Thickness(13);
            Dispatcher.UIThread.RunJobs();

            GetHeaderDecorator(normalItem).Padding.ShouldBe(new Thickness(11));
            GetContentPresenter(normalItem).Padding.ShouldBe(new Thickness(13));
            GetHeaderDecorator(explicitItem).Padding.ShouldBe(new Thickness(3));
            GetContentPresenter(explicitItem).Padding.ShouldBe(new Thickness(4));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Ghost_And_Borderless_Mode_Update_Frame_Border_At_Runtime()
    {
        var collapse = new AtomUICollapse
        {
            BorderThickness = new Thickness(5),
            Items =
            {
                new AtomUICollapseItem
                {
                    Header  = "Header",
                    Content = "Content"
                }
            }
        };
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 180,
            Content = collapse
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var frame = FindVisualByName<Border>(collapse, "PART_Frame");
            frame.ShouldNotBeNull();
            frame!.BorderThickness.ShouldBe(new Thickness(5));

            collapse.IsGhostStyle = true;
            Dispatcher.UIThread.RunJobs();
            frame.BorderThickness.ShouldBe(new Thickness(0));

            collapse.IsGhostStyle = false;
            collapse.IsBorderless = true;
            Dispatcher.UIThread.RunJobs();
            frame.BorderThickness.ShouldBe(new Thickness(0));

            collapse.IsBorderless = false;
            Dispatcher.UIThread.RunJobs();
            frame.BorderThickness.ShouldBe(new Thickness(5));
        }
        finally
        {
            window.Close();
        }
    }

    private static Border GetHeaderDecorator(AtomUICollapseItem item)
    {
        var header = FindVisualByName<Border>(item, "PART_HeaderDecorator");
        header.ShouldNotBeNull();
        return header!;
    }

    private static ContentPresenter GetContentPresenter(AtomUICollapseItem item)
    {
        var presenter = FindTemplatePart<ContentPresenter>(item, "PART_ContentPresenter");
        presenter.ShouldNotBeNull();
        return presenter!;
    }

    private static T? FindTemplatePart<T>(AtomUICollapseItem item, string name)
        where T : Control
    {
        return item.GetSelfAndVisualDescendants()
                   .OfType<T>()
                   .FirstOrDefault(control => control.Name == name &&
                                              ReferenceEquals(control.TemplatedParent, item));
    }

    private static T? FindVisualByName<T>(Control root, string name)
        where T : Control
    {
        return root.GetSelfAndVisualDescendants()
                   .OfType<T>()
                   .FirstOrDefault(control => control.Name == name);
    }
}
