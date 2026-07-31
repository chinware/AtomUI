using System.Reflection;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
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

    [Fact]
    public void CompactSpace_Does_Not_Disable_Child_Layout_Rounding()
    {
        var firstAddOn = CreateCompactAddOn("First");
        var secondAddOn = CreateCompactAddOn("Second");
        var compactSpace = new CompactSpace
        {
            Children =
            {
                firstAddOn,
                secondAddOn
            }
        };

        var window = ShowInWindow(compactSpace);
        try
        {
            firstAddOn.UseLayoutRounding.ShouldBeTrue();
            secondAddOn.UseLayoutRounding.ShouldBeTrue();
        }
        finally
        {
            window.Close();
        }
    }

    [Fact]
    public void Compact_Item_Overlap_Uses_Render_Scale_Aware_Border_Thickness()
    {
        var firstAddOn = CreateCompactAddOn("First");
        var secondAddOn = CreateCompactAddOn("Second");
        var compactSpace = new CompactSpace
        {
            Children =
            {
                firstAddOn,
                secondAddOn
            }
        };

        var window = ShowInWindow(compactSpace, window => window.SetRenderScaling(1.5));
        try
        {
            var secondItem = GetCompactSpaceItem(secondAddOn);
            var transform  = secondItem.RenderTransform as TranslateTransform;

            transform.ShouldNotBeNull();
            transform!.X.ShouldBe(-2d / 3d, 0.0001);
            transform.Y.ShouldBe(0d, 0.0001);
        }
        finally
        {
            window.Close();
        }
    }

    private static CompactSpaceAddOn CreateCompactAddOn(string content)
    {
        return new CompactSpaceAddOn
        {
            Content         = content,
            Width           = 80,
            Height          = 32,
            BorderThickness = new Thickness(1),
            StyleVariant    = InputControlStyleVariant.Outlined
        };
    }

    private static AvaloniaWindow ShowInWindow(Control content, Action<AvaloniaWindow>? configureWindow = null)
    {
        var window = new AvaloniaWindow
        {
            Width   = 300,
            Height  = 160,
            Content = content
        };
        configureWindow?.Invoke(window);
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
