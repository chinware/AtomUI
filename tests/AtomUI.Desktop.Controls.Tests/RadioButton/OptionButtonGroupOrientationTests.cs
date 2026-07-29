using System.Reflection;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.RadioButton;

public class OptionButtonGroupOrientationTests
{
    static OptionButtonGroupOrientationTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Orientation_Defaults_To_Horizontal()
    {
        var group = new Desktop.Controls.OptionButtonGroup();

        group.Orientation.ShouldBe(Orientation.Horizontal);
        Desktop.Controls.OptionButtonGroup.OrientationProperty.PropertyType.ShouldBe(typeof(Orientation));
    }

    [Fact]
    public void Vertical_Orientation_Is_Projected_To_Realized_Containers()
    {
        var group = CreateGroup(Orientation.Vertical, "One", "Two", "Three");

        ShowInWindow(group, () =>
        {
            GetInternalValue<Orientation>(GetContainer(group, 0), "GroupOrientation")
                .ShouldBe(Orientation.Vertical);
            GetInternalValue<Orientation>(GetContainer(group, 1), "GroupOrientation")
                .ShouldBe(Orientation.Vertical);
            GetInternalValue<Orientation>(GetContainer(group, 2), "GroupOrientation")
                .ShouldBe(Orientation.Vertical);
        });
    }

    [Fact]
    public void Runtime_Orientation_Change_Is_Projected_To_Realized_Containers()
    {
        var group = CreateGroup(Orientation.Horizontal, "One", "Two");

        ShowInWindow(group, () =>
        {
            group.Orientation = Orientation.Vertical;
            Dispatcher.UIThread.RunJobs();

            GetInternalValue<Orientation>(GetContainer(group, 0), "GroupOrientation")
                .ShouldBe(Orientation.Vertical);
            GetInternalValue<Orientation>(GetContainer(group, 1), "GroupOrientation")
                .ShouldBe(Orientation.Vertical);
        });
    }

    [Fact]
    public void Vertical_Group_Derives_Effective_Corners_Without_Changing_Nominal_Corners()
    {
        var nominal = new CornerRadius(1, 2, 3, 4);
        var group   = CreateGroup(Orientation.Vertical, "One", "Two", "Three");
        foreach (var item in group.Items.OfType<Desktop.Controls.OptionButton>())
        {
            item.CornerRadius = nominal;
        }

        ShowInWindow(group, () =>
        {
            var first  = GetContainer(group, 0);
            var middle = GetContainer(group, 1);
            var last   = GetContainer(group, 2);

            first.CornerRadius.ShouldBe(nominal);
            middle.CornerRadius.ShouldBe(nominal);
            last.CornerRadius.ShouldBe(nominal);
            GetInternalValue<CornerRadius>(first, "EffectiveCornerRadius")
                .ShouldBe(new CornerRadius(1, 2, 0, 0));
            GetInternalValue<CornerRadius>(middle, "EffectiveCornerRadius")
                .ShouldBe(default);
            GetInternalValue<CornerRadius>(last, "EffectiveCornerRadius")
                .ShouldBe(new CornerRadius(0, 0, 3, 4));
        });
    }

    [Fact]
    public void Single_Item_Uses_OnlyOne_Position_And_All_Nominal_Corners()
    {
        var nominal = new CornerRadius(1, 2, 3, 4);
        var group   = CreateGroup(Orientation.Vertical, "Only");
        group.Items.OfType<Desktop.Controls.OptionButton>().Single().CornerRadius = nominal;

        ShowInWindow(group, () =>
        {
            var item = GetContainer(group, 0);

            GetInternalValue<AtomUI.Controls.OptionButtonPositionTrait>(item, "GroupPositionTrait")
                .ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.OnlyOne);
            GetInternalValue<CornerRadius>(item, "EffectiveCornerRadius").ShouldBe(nominal);
            item.CornerRadius.ShouldBe(nominal);
        });
    }

    [Fact]
    public void ItemsSource_Containers_Replay_First_Middle_And_Last_After_Collection_Changes()
    {
        var source = new ObservableCollection<string> { "One", "Two", "Three" };
        var group = new Desktop.Controls.OptionButtonGroup
        {
            ItemsSource = source
        };

        ShowInWindow(group, () =>
        {
            GetPosition(group, 0).ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.First);
            GetPosition(group, 1).ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.Middle);
            GetPosition(group, 2).ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.Last);

            source.RemoveAt(0);
            Dispatcher.UIThread.RunJobs();
            group.UpdateLayout();

            GetPosition(group, 0).ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.First);
            GetPosition(group, 1).ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.Last);
        });
    }

    [Fact]
    public void ItemsSource_Move_And_Reset_Replay_Container_Positions()
    {
        var source = new ObservableCollection<string> { "One", "Two", "Three" };
        var group = new Desktop.Controls.OptionButtonGroup
        {
            ItemsSource = source
        };

        ShowInWindow(group, () =>
        {
            source.Move(2, 0);
            Dispatcher.UIThread.RunJobs();
            group.UpdateLayout();

            GetContainer(group, 0).DataContext.ShouldBe("Three");
            GetPosition(group, 0).ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.First);
            GetPosition(group, 1).ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.Middle);
            GetPosition(group, 2).ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.Last);

            source.Clear();
            source.Add("Only");
            Dispatcher.UIThread.RunJobs();
            group.UpdateLayout();

            GetContainer(group, 0).DataContext.ShouldBe("Only");
            GetPosition(group, 0).ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.OnlyOne);
        });
    }

    [Fact]
    public void Cleared_Container_Does_Not_Retain_Previous_Group_State()
    {
        var source = new ObservableCollection<string> { "One", "Two", "Three" };
        var group = new Desktop.Controls.OptionButtonGroup
        {
            Orientation = Orientation.Vertical,
            ItemsSource = source
        };

        ShowInWindow(group, () =>
        {
            var removed = GetContainer(group, 1);

            source.RemoveAt(1);
            Dispatcher.UIThread.RunJobs();

            GetInternalValue<Orientation>(removed, "GroupOrientation")
                .ShouldBe(Orientation.Horizontal);
            GetInternalValue<AtomUI.Controls.OptionButtonPositionTrait>(removed, "GroupPositionTrait")
                .ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.OnlyOne);
        });
    }

    [Fact]
    public void Removed_Generated_Container_Does_Not_Retain_Owner_Bindings()
    {
        var source = new ObservableCollection<string> { "One", "Two" };
        var group = new Desktop.Controls.OptionButtonGroup
        {
            SizeType   = AtomUI.CustomizableSizeType.Large,
            ItemsSource = source
        };

        ShowInWindow(group, () =>
        {
            var removed = GetContainer(group, 1);

            source.RemoveAt(1);
            Dispatcher.UIThread.RunJobs();
            var detachedSizeType = GetInternalValue<AtomUI.CustomizableSizeType>(removed, "SizeType");

            group.SizeType = AtomUI.CustomizableSizeType.Small;
            Dispatcher.UIThread.RunJobs();

            GetInternalValue<AtomUI.CustomizableSizeType>(removed, "SizeType")
                .ShouldBe(detachedSizeType);
        });
    }

    [Fact]
    public void Removed_Direct_Item_Releases_Owner_State_Bindings_And_Event_Handler()
    {
        var item = new Desktop.Controls.OptionButton { Content = "Direct" };
        var group = new Desktop.Controls.OptionButtonGroup
        {
            Orientation = Orientation.Vertical,
            SizeType    = AtomUI.CustomizableSizeType.Large,
            ButtonStyle = AtomUI.Controls.OptionButtonStyle.Solid,
            Items       = { item }
        };
        var checkedEventCount = 0;
        group.OptionCheckedChanged += (_, _) => checkedEventCount++;

        ShowInWindow(group, () =>
        {
            group.Items.Remove(item);
            Dispatcher.UIThread.RunJobs();
            var detachedSizeType = GetInternalValue<AtomUI.CustomizableSizeType>(item, "SizeType");

            group.SizeType    = AtomUI.CustomizableSizeType.Small;
            group.ButtonStyle = AtomUI.Controls.OptionButtonStyle.Outline;
            item.IsChecked    = false;
            checkedEventCount = 0;
            item.IsChecked    = true;
            Dispatcher.UIThread.RunJobs();

            GetInternalValue<Orientation>(item, "GroupOrientation").ShouldBe(Orientation.Horizontal);
            GetPositionValue(item).ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.OnlyOne);
            GetInternalValue<AtomUI.CustomizableSizeType>(item, "SizeType")
                .ShouldBe(detachedSizeType);
            checkedEventCount.ShouldBe(0);
        });
    }

    [Fact]
    public void Cleared_Direct_Items_Release_Owner_State_Bindings_And_Event_Handler()
    {
        var item = new Desktop.Controls.OptionButton { Content = "Direct" };
        var group = new Desktop.Controls.OptionButtonGroup
        {
            Orientation = Orientation.Vertical,
            SizeType    = AtomUI.CustomizableSizeType.Large,
            Items       = { item }
        };
        var checkedEventCount = 0;
        group.OptionCheckedChanged += (_, _) => checkedEventCount++;

        ShowInWindow(group, () =>
        {
            group.Items.Clear();
            Dispatcher.UIThread.RunJobs();
            var detachedSizeType = GetInternalValue<AtomUI.CustomizableSizeType>(item, "SizeType");

            group.SizeType    = AtomUI.CustomizableSizeType.Small;
            item.IsChecked    = false;
            checkedEventCount = 0;
            item.IsChecked    = true;
            Dispatcher.UIThread.RunJobs();

            GetInternalValue<Orientation>(item, "GroupOrientation").ShouldBe(Orientation.Horizontal);
            GetPositionValue(item).ShouldBe(AtomUI.Controls.OptionButtonPositionTrait.OnlyOne);
            GetInternalValue<AtomUI.CustomizableSizeType>(item, "SizeType")
                .ShouldBe(detachedSizeType);
            checkedEventCount.ShouldBe(0);
        });
    }

    [Fact]
    public void Removed_Direct_Item_Preserves_User_ContentTemplate()
    {
        var contentTemplate = new FuncDataTemplate<object?>((_, _) => new TextBlock());
        var item = new Desktop.Controls.OptionButton
        {
            Content         = "Direct",
            ContentTemplate = contentTemplate
        };
        var group = new Desktop.Controls.OptionButtonGroup
        {
            Items = { item }
        };

        ShowInWindow(group, () =>
        {
            group.Items.Remove(item);
            Dispatcher.UIThread.RunJobs();

            item.ContentTemplate.ShouldBeSameAs(contentTemplate);
        });
    }

    [Fact]
    public void Initial_Selected_Container_Raises_One_Checked_Event()
    {
        var group = CreateGroup(Orientation.Horizontal, "One", "Two");
        group.SelectedIndex = 1;
        var checkedEventCount = 0;
        group.OptionCheckedChanged += (_, _) => checkedEventCount++;

        ShowInWindow(group, () =>
        {
            group.SelectedIndex.ShouldBe(1);
            GetContainer(group, 1).IsChecked.ShouldBe(true);
            checkedEventCount.ShouldBe(1);
        });
    }

    [Theory]
    [InlineData(Orientation.Horizontal, Key.Down, Key.Right)]
    [InlineData(Orientation.Vertical, Key.Right, Key.Down)]
    public void Direction_Keys_Follow_The_Group_Orientation(
        Orientation orientation,
        Key ignoredKey,
        Key nextKey)
    {
        var group = CreateGroup(orientation, "One", "Two", "Three");
        group.Items.OfType<Desktop.Controls.OptionButton>().First().IsChecked = true;

        ShowInWindow(group, () =>
        {
            var first  = GetContainer(group, 0);
            var second = GetContainer(group, 1);
            first.Focus();

            RaiseKey(first, ignoredKey);
            group.SelectedIndex.ShouldBe(0);

            RaiseKey(first, nextKey);
            group.SelectedIndex.ShouldBe(1);
            second.IsChecked.ShouldBe(true);
        });
    }

    private static Desktop.Controls.OptionButtonGroup CreateGroup(
        Orientation orientation,
        params string[] contents)
    {
        var group = new Desktop.Controls.OptionButtonGroup
        {
            Orientation = orientation
        };
        foreach (var content in contents)
        {
            group.Items.Add(new Desktop.Controls.OptionButton
            {
                Content = content
            });
        }

        return group;
    }

    private static Desktop.Controls.OptionButton GetContainer(
        Desktop.Controls.OptionButtonGroup group,
        int index)
    {
        return group.ContainerFromIndex(index).ShouldBeOfType<Desktop.Controls.OptionButton>();
    }

    private static AtomUI.Controls.OptionButtonPositionTrait GetPosition(
        Desktop.Controls.OptionButtonGroup group,
        int index)
    {
        return GetInternalValue<AtomUI.Controls.OptionButtonPositionTrait>(
            GetContainer(group, index),
            "GroupPositionTrait");
    }

    private static AtomUI.Controls.OptionButtonPositionTrait GetPositionValue(
        Desktop.Controls.OptionButton item)
    {
        return GetInternalValue<AtomUI.Controls.OptionButtonPositionTrait>(
            item,
            "GroupPositionTrait");
    }

    private static T GetInternalValue<T>(object instance, string propertyName)
    {
        return instance.GetType()
                       .BaseType!
                       .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)!
                       .GetValue(instance)
                       .ShouldBeOfType<T>();
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

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 480,
            Height  = 320,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            window.UpdateLayout();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}
