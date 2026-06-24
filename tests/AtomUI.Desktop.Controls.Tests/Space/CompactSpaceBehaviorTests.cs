using System.Reflection;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Space;

public class CompactSpaceBehaviorTests
{
    static CompactSpaceBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Removing_Child_Clears_CompactSpace_State_From_Target()
    {
        var firstButton  = new Button { Content = "First" };
        var secondButton = new Button { Content = "Second" };
        var compactSpace = new CompactSpace
        {
            Children =
            {
                firstButton,
                secondButton
            }
        };

        var window = ShowInWindow(compactSpace);
        try
        {
            GetInternalProperty<bool>(firstButton, "IsUsedInCompactSpace").ShouldBeTrue();
            GetInternalProperty<object?>(firstButton, "CompactSpaceItemPosition").ShouldNotBeNull();

            compactSpace.Children.Remove(firstButton);
            Dispatcher.UIThread.RunJobs();

            GetInternalProperty<bool>(firstButton, "IsUsedInCompactSpace").ShouldBeFalse();
            GetInternalProperty<object?>(firstButton, "CompactSpaceItemPosition").ShouldBeNull();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Pointer_Exit_Restores_NonFocused_Item_ZIndex()
    {
        var firstButton = new Button
        {
            Content = "First",
            Width   = 80,
            Height  = 32
        };
        var secondButton = new Button
        {
            Content = "Second",
            Width   = 80,
            Height  = 32
        };
        var compactSpace = new CompactSpace
        {
            Margin = new Thickness(20),
            Children =
            {
                firstButton,
                secondButton
            }
        };

        var window = ShowInWindow(compactSpace);
        try
        {
            var firstItem = GetCompactSpaceItem(firstButton);

            window.MouseMove(GetCenterPoint(firstButton, window));
            Dispatcher.UIThread.RunJobs();

            firstItem.ZIndex.ShouldBeGreaterThan(0);

            window.MouseMove(new Point(window.Bounds.Width - 4, window.Bounds.Height - 4));
            Dispatcher.UIThread.RunJobs();

            firstItem.ZIndex.ShouldBe(0);
        }
        finally
        {
            window.Close();
        }
    }

    private static AvaloniaWindow ShowInWindow(Control content)
    {
        var window = new AvaloniaWindow
        {
            Width   = 300,
            Height  = 160,
            Content = content
        };
        window.Show();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private static Control GetCompactSpaceItem(Control child)
    {
        return child.GetVisualAncestors()
                    .OfType<Control>()
                    .First(control => control.GetType().Name == "CompactSpaceItem");
    }

    private static Point GetCenterPoint(Control control, AvaloniaWindow window)
    {
        var point = control.TranslatePoint(
            new Point(control.Bounds.Width / 2, control.Bounds.Height / 2),
            window);

        point.ShouldNotBeNull();
        return point.Value;
    }

    private static T GetInternalProperty<T>(object instance, string propertyName)
    {
        var property = instance.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        property.ShouldNotBeNull();
        return (T)property.GetValue(instance)!;
    }
}
