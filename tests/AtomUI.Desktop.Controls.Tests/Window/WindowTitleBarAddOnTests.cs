using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Shouldly;
using Xunit;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;
using WindowTitleBar = AtomUI.Desktop.Controls.WindowTitleBar;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class WindowTitleBarAddOnTests
{
    static WindowTitleBarAddOnTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Window_Exposes_And_Projects_Title_Bar_Add_Ons()
    {
        var window = new AtomUIWindow();
        var titleBar = new WindowTitleBar();
        var leftAddOn = new object();
        var rightAddOn = new object();
        var replacementLeftAddOn = new object();
        var leftAddOnTemplate = new FuncDataTemplate<object?>((_, _) => new Border());
        var rightAddOnTemplate = new FuncDataTemplate<object?>((_, _) => new Border());

        var leftAddOnProperty = GetPublicProperty(nameof(WindowTitleBar.LeftAddOn));
        var leftAddOnTemplateProperty = GetPublicProperty(nameof(WindowTitleBar.LeftAddOnTemplate));
        var rightAddOnProperty = GetPublicProperty(nameof(WindowTitleBar.RightAddOn));
        var rightAddOnTemplateProperty = GetPublicProperty(nameof(WindowTitleBar.RightAddOnTemplate));

        leftAddOnProperty.PropertyType.ShouldBe(typeof(object));
        leftAddOnTemplateProperty.PropertyType.ShouldBe(typeof(IDataTemplate));
        rightAddOnProperty.PropertyType.ShouldBe(typeof(object));
        rightAddOnTemplateProperty.PropertyType.ShouldBe(typeof(IDataTemplate));
        window.GetValue(GetPublicPropertyField(nameof(WindowTitleBar.LeftAddOn)))
              .ShouldBeNull();
        window.GetValue(GetPublicPropertyField(nameof(WindowTitleBar.LeftAddOnTemplate)))
              .ShouldBeNull();
        window.GetValue(GetPublicPropertyField(nameof(WindowTitleBar.RightAddOn)))
              .ShouldBeNull();
        window.GetValue(GetPublicPropertyField(nameof(WindowTitleBar.RightAddOnTemplate)))
              .ShouldBeNull();

        leftAddOnProperty.SetValue(window, leftAddOn);
        leftAddOnTemplateProperty.SetValue(window, leftAddOnTemplate);
        rightAddOnProperty.SetValue(window, rightAddOn);
        rightAddOnTemplateProperty.SetValue(window, rightAddOnTemplate);
        ConfigureTitleBar(window, titleBar);

        titleBar.LeftAddOn.ShouldBe(leftAddOn);
        titleBar.LeftAddOnTemplate.ShouldBe(leftAddOnTemplate);
        titleBar.RightAddOn.ShouldBe(rightAddOn);
        titleBar.RightAddOnTemplate.ShouldBe(rightAddOnTemplate);

        leftAddOnProperty.SetValue(window, replacementLeftAddOn);

        titleBar.LeftAddOn.ShouldBe(replacementLeftAddOn);
    }

    [Fact]
    public void Window_Add_On_Projection_Does_Not_Override_A_Derived_Title_Bar_Local_Content()
    {
        var window = new AtomUIWindow();
        var titleBar = new TitleBarWithLocalLeftAddOn();

        ConfigureTitleBar(window, titleBar);

        titleBar.LeftAddOn.ShouldBe(titleBar.LocalLeftAddOn);
    }

    private static PropertyInfo GetPublicProperty(string name)
    {
        return typeof(AtomUIWindow)
               .GetProperty(name, BindingFlags.Instance | BindingFlags.Public)
               .ShouldNotBeNull();
    }

    private static AvaloniaProperty GetPublicPropertyField(string propertyName)
    {
        return typeof(AtomUIWindow)
               .GetField($"{propertyName}Property", BindingFlags.Static | BindingFlags.Public)
               .ShouldNotBeNull()
               .GetValue(null)
               .ShouldBeAssignableTo<AvaloniaProperty>()!;
    }

    private static void ConfigureTitleBar(AtomUIWindow window, WindowTitleBar titleBar)
    {
        typeof(AtomUIWindow)
            .GetMethod("NotifyConfigureTitleBar", BindingFlags.Instance | BindingFlags.NonPublic)
            .ShouldNotBeNull()
            .Invoke(window, [titleBar]);
    }

    private sealed class TitleBarWithLocalLeftAddOn : WindowTitleBar
    {
        public object LocalLeftAddOn { get; } = new();

        public TitleBarWithLocalLeftAddOn()
        {
            LeftAddOn = LocalLeftAddOn;
        }
    }
}
