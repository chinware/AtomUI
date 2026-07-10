using System;
using System.Linq;
using System.Threading;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AtomUI.Controls.Primitives;
using AtomUI.MotionScene;
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
            GetContentFrame(normalItem).Padding.ShouldBe(new Thickness(9));
            GetHeaderDecorator(explicitItem).Padding.ShouldBe(new Thickness(3));
            GetContentFrame(explicitItem).Padding.ShouldBe(new Thickness(4));

            collapse.ItemHeaderPadding  = new Thickness(11);
            collapse.ItemContentPadding = new Thickness(13);
            Dispatcher.UIThread.RunJobs();

            GetHeaderDecorator(normalItem).Padding.ShouldBe(new Thickness(11));
            GetContentFrame(normalItem).Padding.ShouldBe(new Thickness(13));
            GetHeaderDecorator(explicitItem).Padding.ShouldBe(new Thickness(3));
            GetContentFrame(explicitItem).Padding.ShouldBe(new Thickness(4));
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

            var frame = FindVisualByName<PixelAlignedBorder>(collapse, "PART_Frame");
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

    [Fact]
    public void Collapsed_Items_Apply_Visible_Header_Separator_Brushes()
    {
        var firstItem = new AtomUICollapseItem
        {
            Header  = "First",
            Content = "Content"
        };
        var middleItem = new AtomUICollapseItem
        {
            Header  = "Middle",
            Content = "Content"
        };
        var lastItem = new AtomUICollapseItem
        {
            Header  = "Last",
            Content = "Content"
        };
        var collapse = new AtomUICollapse
        {
            BorderThickness = new Thickness(1),
            IsMotionEnabled = false,
            Items =
            {
                firstItem,
                middleItem,
                lastItem
            }
        };
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 260,
            Content = collapse
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var firstHeader = GetHeaderDecorator(firstItem);
            firstHeader.BorderThickness.ShouldBe(new Thickness(0, 0, 0, 1));
            firstHeader.BorderBrush.ShouldNotBeNull();

            var middleHeader = GetHeaderDecorator(middleItem);
            middleHeader.BorderThickness.ShouldBe(new Thickness(0, 0, 0, 1));
            middleHeader.BorderBrush.ShouldNotBeNull();

            GetHeaderDecorator(lastItem).BorderThickness.ShouldBe(new Thickness(0));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Expanded_Items_Use_Content_Top_Border_For_Header_Content_Separator()
    {
        var firstItem = new AtomUICollapseItem
        {
            Header  = "First",
            Content = "Content"
        };
        var middleItem = new AtomUICollapseItem
        {
            Header     = "Middle",
            Content    = "Content",
            IsSelected = true
        };
        var lastItem = new AtomUICollapseItem
        {
            Header     = "Last",
            Content    = "Content",
            IsSelected = true
        };
        var collapse = new AtomUICollapse
        {
            BorderThickness = new Thickness(1),
            IsMotionEnabled = false,
            Items =
            {
                firstItem,
                middleItem,
                lastItem
            }
        };
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 320,
            Content = collapse
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            GetHeaderDecorator(firstItem).BorderThickness.ShouldBe(new Thickness(0, 0, 0, 1));
            GetHeaderDecorator(firstItem).BorderBrush.ShouldNotBeNull();
            firstItem.ContentBorderThickness.ShouldBe(new Thickness(0));

            GetHeaderDecorator(middleItem).BorderThickness.ShouldBe(new Thickness(0));
            GetContentFrame(middleItem).BorderThickness.ShouldBe(new Thickness(0, 1, 0, 1));
            GetContentFrame(middleItem).BorderBrush.ShouldNotBeNull();

            GetHeaderDecorator(lastItem).BorderThickness.ShouldBe(new Thickness(0));
            GetContentFrame(lastItem).BorderThickness.ShouldBe(new Thickness(0, 1, 0, 0));
            GetContentFrame(lastItem).BorderBrush.ShouldNotBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Closing_Last_Item_Keeps_Content_Top_Border_Until_Motion_Completes()
    {
        var lastItem = new AtomUICollapseItem
        {
            Header         = "Last",
            Content        = "Content",
            IsSelected     = true,
            MotionDuration = System.TimeSpan.Zero
        };
        var collapse = new AtomUICollapse
        {
            BorderThickness = new Thickness(1),
            Items =
            {
                new AtomUICollapseItem
                {
                    Header  = "First",
                    Content = "Content"
                },
                lastItem
            }
        };
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 260,
            Content = collapse
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            lastItem.Transitions = null;
            lastItem.IsSelected = false;

            GetHeaderDecorator(lastItem).BorderThickness.ShouldBe(new Thickness(0));
            GetContentFrame(lastItem).BorderThickness.ShouldBe(new Thickness(0, 1, 0, 0));
            GetContentFrame(lastItem).BorderBrush.ShouldNotBeNull();

            Dispatcher.UIThread.RunJobs();

            lastItem.HeaderBorderThickness.ShouldBe(new Thickness(0));
            lastItem.ContentBorderThickness.ShouldBe(new Thickness(0));
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Accordion_Mode_Normalizes_Initial_Selected_Items_To_First_Active_Item()
    {
        var firstItem = new AtomUICollapseItem
        {
            Header     = "First",
            Content    = "Content",
            IsSelected = true
        };
        var secondItem = new AtomUICollapseItem
        {
            Header     = "Second",
            Content    = "Content",
            IsSelected = true
        };
        var thirdItem = new AtomUICollapseItem
        {
            Header  = "Third",
            Content = "Content"
        };
        var collapse = new AtomUICollapse
        {
            IsAccordion     = true,
            IsMotionEnabled = false,
            Items =
            {
                firstItem,
                secondItem,
                thirdItem
            }
        };
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 320,
            Content = collapse
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            firstItem.IsSelected.ShouldBeTrue();
            secondItem.IsSelected.ShouldBeFalse();
            thirdItem.IsSelected.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Accordion_Expand_Button_Click_Closes_Previous_Item_And_Opens_Target()
    {
        var firstItem = new AtomUICollapseItem
        {
            Header     = "First",
            Content    = "Content",
            IsSelected = true
        };
        var secondItem = new AtomUICollapseItem
        {
            Header  = "Second",
            Content = "Content"
        };
        var collapse = new AtomUICollapse
        {
            IsAccordion     = true,
            IsMotionEnabled = false,
            Items =
            {
                firstItem,
                secondItem
            }
        };
        var window = new AvaloniaWindow
        {
            Width   = 360,
            Height  = 260,
            Content = collapse
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            var expandButton = GetExpandButton(secondItem);
            expandButton.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, expandButton));
            Dispatcher.UIThread.RunJobs();

            firstItem.IsSelected.ShouldBeFalse();
            secondItem.IsSelected.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Accordion_Clicking_Active_Item_Collapses_To_No_Active_Item()
    {
        var item = new AtomUICollapseItem
        {
            Header     = "First",
            Content    = "Content",
            IsSelected = true
        };
        var collapse = new AtomUICollapse
        {
            IsAccordion     = true,
            IsMotionEnabled = false,
            Items =
            {
                item
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

            collapse.UpdateSelectionFromEvent(item, new RoutedEventArgs());
            Dispatcher.UIThread.RunJobs();

            item.IsSelected.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Item_Motion_Reconciles_Final_Visibility_When_Selection_Changes_During_Animation()
    {
        var item = new AtomUICollapseItem
        {
            Header         = "First",
            Content        = "Content",
            MotionDuration = TimeSpan.FromMilliseconds(1)
        };
        var collapse = new AtomUICollapse
        {
            IsMotionEnabled = true,
            Items =
            {
                item
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

            item.IsSelected = true;
            item.IsSelected = false;
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(20);
            Dispatcher.UIThread.RunJobs();

            var motionActor = GetContentMotionActor(item);
            item.IsSelected.ShouldBeFalse();
            motionActor.IsVisible.ShouldBeFalse();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Item_Content_Motion_Uses_Layout_Aware_Transform_Instead_Of_Height()
    {
        var item = new AtomUICollapseItem
        {
            Header         = "First",
            Content        = "Content",
            MotionDuration = TimeSpan.FromMilliseconds(100)
        };
        var collapse = new AtomUICollapse
        {
            IsMotionEnabled = true,
            Items =
            {
                item
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

            item.IsSelected = true;
            Dispatcher.UIThread.RunJobs();

            var motionActor = GetContentMotionActor(item);
            motionActor.MotionTransform.ShouldNotBeNull();
            motionActor.Height.ShouldBe(double.NaN);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Item_Content_Motion_Clears_Local_Height_After_Completion()
    {
        var item = new AtomUICollapseItem
        {
            Header         = "First",
            Content        = "Content",
            MotionDuration = TimeSpan.FromMilliseconds(1)
        };
        var collapse = new AtomUICollapse
        {
            IsMotionEnabled = true,
            Items =
            {
                item
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

            item.IsSelected = true;
            Dispatcher.UIThread.RunJobs();
            Thread.Sleep(20);
            Dispatcher.UIThread.RunJobs();

            var motionActor = GetContentMotionActor(item);
            motionActor.Height.ShouldBe(double.NaN);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Item_Content_Motion_Starts_Growing_For_Control_Content()
    {
        var wrappedContent = new TextBlock
        {
            Text         = "Long wrapped collapse content that needs multiple lines when measured with the item width.",
            TextWrapping = TextWrapping.Wrap
        };
        var item = new AtomUICollapseItem
        {
            Header         = "First",
            Content        = wrappedContent,
            MotionDuration = TimeSpan.FromMilliseconds(100)
        };
        var collapse = new AtomUICollapse
        {
            IsMotionEnabled = true,
            Items =
            {
                item
            }
        };
        var window = new AvaloniaWindow
        {
            Width   = 180,
            Height  = 260,
            Content = collapse
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            item.IsSelected = true;
            Dispatcher.UIThread.RunJobs();

            var motionActor = GetContentMotionActor(item);
            WaitForLayoutCondition(() => motionActor.Bounds.Height > 0.0);
            motionActor.Bounds.Height.ShouldBeGreaterThan(0.0);
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Item_Content_Motion_Starts_Growing_For_String_Content()
    {
        var item = new AtomUICollapseItem
        {
            Header         = "First",
            Content        = "String content",
            MotionDuration = TimeSpan.FromMilliseconds(100)
        };
        var collapse = new AtomUICollapse
        {
            IsMotionEnabled = true,
            Items =
            {
                item
            }
        };
        var window = new AvaloniaWindow
        {
            Width   = 240,
            Height  = 180,
            Content = collapse
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();

            item.IsSelected = true;
            Dispatcher.UIThread.RunJobs();

            var motionActor = GetContentMotionActor(item);
            WaitForLayoutCondition(() => motionActor.Bounds.Height > 0.0);
            motionActor.Bounds.Height.ShouldBeGreaterThan(0.0);
        }
        finally
        {
            window.Close();
        }
    }

    private static PixelAlignedBorder GetHeaderDecorator(AtomUICollapseItem item)
    {
        var header = FindVisualByName<PixelAlignedBorder>(item, "PART_HeaderDecorator");
        header.ShouldNotBeNull();
        return header!;
    }

    private static PixelAlignedBorder GetContentFrame(AtomUICollapseItem item)
    {
        var frame = FindVisualByName<PixelAlignedBorder>(item, "PART_ContentFrame");
        frame.ShouldNotBeNull();
        return frame!;
    }

    private static IconButton GetExpandButton(AtomUICollapseItem item)
    {
        var button = FindTemplatePart<IconButton>(item, "PART_ExpandButton");
        button.ShouldNotBeNull();
        return button!;
    }

    private static BaseMotionActor GetContentMotionActor(AtomUICollapseItem item)
    {
        var motionActor = FindTemplatePart<BaseMotionActor>(item, "PART_ContentMotionActor");
        motionActor.ShouldNotBeNull();
        return motionActor!;
    }

    private static void WaitForLayoutCondition(Func<bool> condition, int timeoutMilliseconds = 500)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(timeoutMilliseconds);
        while (DateTime.UtcNow < deadline)
        {
            Dispatcher.UIThread.RunJobs();
            if (condition())
            {
                return;
            }

            Thread.Sleep(10);
        }

        Dispatcher.UIThread.RunJobs();
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
