using System;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomUIButton = AtomUI.Desktop.Controls.Button;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Buttons;

public class ButtonBehaviorTests
{
    static ButtonBehaviorTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Button_ApplyTemplate_Configures_WaveSpiritType_From_Current_Shape()
    {
        var button = new AtomUIButton
        {
            Shape           = ButtonShape.Circle,
            IsMotionEnabled = false
        };

        ShowInWindow(button, () =>
        {
            GetInternalPropertyValue(button, "WaveSpiritType")
                .ToString()
                .ShouldBe("CircleWave");
        });
    }

    [Fact]
    public void Button_ApplyTemplate_Configures_EffectiveBorderThickness_From_Current_Type()
    {
        var borderThickness = new Thickness(4);
        var button = new AtomUIButton
        {
            ButtonType      = ButtonType.Link,
            BorderThickness = borderThickness,
            IsMotionEnabled = false
        };

        ShowInWindow(button, () =>
        {
            GetInternalPropertyValue<Thickness>(button, "EffectiveBorderThickness")
                .ShouldBe(new Thickness(0));
        });
    }

    [Fact]
    public void Button_ButtonType_Change_Refreshes_EffectiveBorderThickness()
    {
        var borderThickness = new Thickness(3);
        var button = new AtomUIButton
        {
            ButtonType      = ButtonType.Link,
            BorderThickness = borderThickness,
            IsMotionEnabled = false
        };

        ShowInWindow(button, () =>
        {
            GetInternalPropertyValue<Thickness>(button, "EffectiveBorderThickness")
                .ShouldBe(new Thickness(0));

            button.ButtonType = ButtonType.Default;
            Dispatcher.UIThread.RunJobs();
            GetInternalPropertyValue<Thickness>(button, "EffectiveBorderThickness")
                .ShouldBe(borderThickness);

            button.ButtonType = ButtonType.Text;
            Dispatcher.UIThread.RunJobs();
            GetInternalPropertyValue<Thickness>(button, "EffectiveBorderThickness")
                .ShouldBe(new Thickness(0));
        });
    }

    private static object GetInternalPropertyValue(object target, string propertyName)
    {
        var property = target.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        property.ShouldNotBeNull();
        var value = property.GetValue(target);
        value.ShouldNotBeNull();
        return value;
    }

    private static T GetInternalPropertyValue<T>(object target, string propertyName)
    {
        return (T)GetInternalPropertyValue(target, propertyName);
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 240,
            Height  = 160,
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
}
