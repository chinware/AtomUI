using System.Reflection;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.RadioButton;

public class OptionButtonGroupThemeTests
{
    static OptionButtonGroupThemeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Theme_Binds_Standard_StackPanel_And_Leaves_Custom_Size_Unselected()
    {
        var groupTheme = ReadRepoFile(
            "src/AtomUI.Desktop.Controls/OptionButtonGroup/Themes/OptionButtonGroupTheme.axaml");
        var itemTheme = ReadRepoFile(
            "src/AtomUI.Desktop.Controls/OptionButtonGroup/Themes/OptionButtonTheme.axaml");

        groupTheme.ShouldContain("<ItemsPanelTemplate>");
        groupTheme.ShouldContain("<StackPanel");
        groupTheme.ShouldContain(
            "Orientation=\"{CompiledBinding $parent[atom:OptionButtonGroup].Orientation}\"");
        groupTheme.ShouldContain("Selector=\"^[Orientation=Horizontal]\"");
        itemTheme.ShouldContain("Selector=\"^[GroupOrientation=Vertical]\"");
        itemTheme.ShouldContain("CornerRadius=\"{TemplateBinding EffectiveCornerRadius}\"");
        groupTheme.ShouldNotContain("SizeType=Custom");
        itemTheme.ShouldNotContain("SizeType=Custom");
    }

    [Fact]
    public void Vertical_Layout_Stacks_Items_And_Uses_The_Widest_Natural_Width()
    {
        var group = CreateGroup(Orientation.Vertical, "A", "A much longer option");
        group.HorizontalAlignment = HorizontalAlignment.Left;
        group.VerticalAlignment   = VerticalAlignment.Top;

        ShowInWindow(group, () =>
        {
            var first  = GetContainer(group, 0);
            var second = GetContainer(group, 1);

            second.Bounds.Y.ShouldBe(first.Bounds.Bottom, 0.5);
            first.Bounds.Width.ShouldBe(second.Bounds.Width, 0.5);
            group.Bounds.Height.ShouldBe(first.Bounds.Height + second.Bounds.Height, 0.5);
        });
    }

    [Fact]
    public void Empty_Vertical_Group_Has_Zero_Content_Height()
    {
        var group = CreateGroup(Orientation.Vertical);
        group.HorizontalAlignment = HorizontalAlignment.Left;
        group.VerticalAlignment   = VerticalAlignment.Top;

        ShowInWindow(group, () => group.Bounds.Height.ShouldBe(0, 0.01));
    }

    [Fact]
    public void Vertical_Stretch_And_Explicit_Width_Are_Applied_To_Every_Item()
    {
        var stretched = CreateGroup(Orientation.Vertical, "One", "Two");
        var explicitWidth = CreateGroup(Orientation.Vertical, "One", "Two");
        explicitWidth.Width               = 240;
        explicitWidth.HorizontalAlignment = HorizontalAlignment.Left;
        var host = new StackPanel
        {
            Width    = 360,
            Children =
            {
                stretched,
                explicitWidth
            }
        };

        ShowInWindow(host, () =>
        {
            GetContainer(stretched, 0).Bounds.Width.ShouldBe(stretched.Bounds.Width, 0.5);
            GetContainer(stretched, 1).Bounds.Width.ShouldBe(stretched.Bounds.Width, 0.5);
            stretched.Bounds.Width.ShouldBe(360, 0.5);

            explicitWidth.Bounds.Width.ShouldBe(240, 0.5);
            GetContainer(explicitWidth, 0).Bounds.Width.ShouldBe(240, 0.5);
            GetContainer(explicitWidth, 1).Bounds.Width.ShouldBe(240, 0.5);
        });
    }

    [Fact]
    public void Runtime_Orientation_Change_Reorients_The_Default_ItemsPanel()
    {
        var group = CreateGroup(Orientation.Horizontal, "One", "Two");
        group.HorizontalAlignment = HorizontalAlignment.Left;
        group.VerticalAlignment   = VerticalAlignment.Top;

        ShowInWindow(group, () =>
        {
            var first  = GetContainer(group, 0);
            var second = GetContainer(group, 1);
            second.Bounds.X.ShouldBe(first.Bounds.Right, 0.5);

            group.Orientation = Orientation.Vertical;
            Dispatcher.UIThread.RunJobs();
            group.InvalidateMeasure();
            group.UpdateLayout();

            second.Bounds.X.ShouldBe(0, 0.5);
            second.Bounds.Y.ShouldBe(first.Bounds.Bottom, 0.5);
        });
    }

    [Fact]
    public void Vertical_Item_Content_Stretches_And_Wave_Uses_Effective_Corners()
    {
        var group = CreateGroup(Orientation.Vertical, "One", "Two");
        group.Width = 240;

        ShowInWindow(group, () =>
        {
            var first = GetContainer(group, 0);
            var contentLayout = first.GetVisualDescendants()
                                     .OfType<DockPanel>()
                                     .Single(panel => panel.Name == "ContentLayout");
            var wave = first.GetVisualDescendants()
                            .Single(control => control.Name == "PART_WaveSpirit");

            contentLayout.HorizontalAlignment.ShouldBe(HorizontalAlignment.Stretch);
            GetPropertyValue<Avalonia.CornerRadius>(wave, "CornerRadius")
                .ShouldBe(GetInternalValue<Avalonia.CornerRadius>(first, "EffectiveCornerRadius"));
        });
    }

    [Fact]
    public void Vertical_Item_Content_Uses_Button_Padding_As_Inner_Inset()
    {
        var group = CreateGroup(Orientation.Vertical, "One");
        group.Width = 240;

        ShowInWindow(group, () =>
        {
            var item = GetContainer(group, 0);
            var contentLayout = item.GetVisualDescendants()
                                    .OfType<DockPanel>()
                                    .Single(panel => panel.Name == "ContentLayout");

            contentLayout.Margin.ShouldBe(item.Padding);
        });
    }

    [Fact]
    public void Natural_Item_Width_Counts_Button_Padding_Only_Once()
    {
        var group = CreateGroup(Orientation.Vertical, "One");
        group.HorizontalAlignment = HorizontalAlignment.Left;

        ShowInWindow(group, () =>
        {
            var item = GetContainer(group, 0);
            var label = item.GetVisualDescendants()
                            .OfType<Desktop.Controls.TextBlock>()
                            .Single();

            var horizontalPadding = item.Padding.Left + item.Padding.Right;
            (item.DesiredSize.Width - label.DesiredSize.Width)
                .ShouldBe(horizontalPadding, 0.5);
        });
    }

    [Fact]
    public void Custom_Size_Allows_Group_And_Item_Height_Overrides()
    {
        var horizontal = CreateGroup(Orientation.Horizontal, "One", "Two");
        horizontal.SizeType = CustomizableSizeType.Custom;
        horizontal.Height   = 52;

        var vertical = CreateGroup(Orientation.Vertical, "One", "Two");
        vertical.SizeType = CustomizableSizeType.Custom;
        vertical.Items.OfType<Desktop.Controls.OptionButton>().First().Height = 44;

        var host = new StackPanel
        {
            Children =
            {
                horizontal,
                vertical
            }
        };

        ShowInWindow(host, () =>
        {
            horizontal.Bounds.Height.ShouldBe(52, 0.5);
            GetContainer(vertical, 0).Bounds.Height.ShouldBe(44, 0.5);
            vertical.Bounds.Height.ShouldBeGreaterThan(44);
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

    private static T GetInternalValue<T>(object instance, string propertyName)
    {
        return instance.GetType()
                       .BaseType!
                       .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic)!
                       .GetValue(instance)
                       .ShouldBeOfType<T>();
    }

    private static T GetPropertyValue<T>(object instance, string propertyName)
    {
        return instance.GetType()
                       .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!
                       .GetValue(instance)
                       .ShouldBeOfType<T>();
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 480,
            Height  = 420,
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

    private static string ReadRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return File.ReadAllText(candidate);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
