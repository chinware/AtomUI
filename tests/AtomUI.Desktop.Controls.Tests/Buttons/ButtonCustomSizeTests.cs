using System;
using System.Linq;
using System.Reflection;
using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIButton = AtomUI.Desktop.Controls.Button;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Buttons;

public class ButtonCustomSizeTests
{
    static ButtonCustomSizeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Theory]
    [InlineData(typeof(AtomUIButton))]
    [InlineData(typeof(DropdownButton))]
    [InlineData(typeof(SplitButton))]
    [InlineData(typeof(HyperLinkButton))]
    public void Button_Family_Uses_Customizable_SizeType_Contract(Type controlType)
    {
        typeof(ICustomizableSizeTypeAware).IsAssignableFrom(controlType).ShouldBeTrue();

        var property = controlType.GetProperty(
            "SizeType",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        property.ShouldNotBeNull();
        property!.PropertyType.ShouldBe(typeof(CustomizableSizeType));

        var field = controlType.GetField(
            "SizeTypeProperty",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        field.ShouldNotBeNull();
        field!.FieldType.ShouldBe(typeof(StyledProperty<CustomizableSizeType>));
    }

    [Fact]
    public void Button_Custom_Size_Defaults_To_Middle_Metrics()
    {
        var middle = CreateButton(CustomizableSizeType.Middle);
        var custom = CreateButton(CustomizableSizeType.Custom);
        var host = new StackPanel
        {
            Children =
            {
                middle,
                custom
            }
        };

        ShowInWindow(host, () => AssertButtonMetricsMatch(custom, middle));
    }

    [Fact]
    public void Button_Custom_Size_Local_Properties_Override_Defaults()
    {
        var button = CreateButton(CustomizableSizeType.Custom);
        button.Height       = 44;
        button.Padding      = new Thickness(18, 0);
        button.FontSize     = 15;
        button.CornerRadius = new CornerRadius(7);

        ShowInWindow(button, () =>
        {
            button.Height.ShouldBe(44);
            button.Padding.ShouldBe(new Thickness(18, 0));
            button.FontSize.ShouldBe(15);
            button.CornerRadius.ShouldBe(new CornerRadius(7));
            FindFrame(button).Height.ShouldBe(44);
        });
    }

    [Fact]
    public void HyperLinkButton_Custom_Size_Defaults_To_Middle_Metrics()
    {
        var middle = CreateHyperLinkButton(CustomizableSizeType.Middle);
        var custom = CreateHyperLinkButton(CustomizableSizeType.Custom);
        var host = new StackPanel
        {
            Children =
            {
                middle,
                custom
            }
        };

        ShowInWindow(host, () =>
        {
            custom.Height.ShouldBe(middle.Height);
            custom.Padding.ShouldBe(middle.Padding);
            custom.FontSize.ShouldBe(middle.FontSize);
            custom.IconWidth.ShouldBe(middle.IconWidth);
            custom.IconHeight.ShouldBe(middle.IconHeight);
            FindFrame(custom).Height.ShouldBe(FindFrame(middle).Height);
        });
    }

    [Fact]
    public void HyperLinkButton_Custom_Size_Local_Properties_Override_Defaults()
    {
        var button = CreateHyperLinkButton(CustomizableSizeType.Custom);
        button.Height   = 42;
        button.Padding  = new Thickness(16, 0);
        button.FontSize = 15;

        ShowInWindow(button, () =>
        {
            button.Height.ShouldBe(42);
            button.Padding.ShouldBe(new Thickness(16, 0));
            button.FontSize.ShouldBe(15);
            FindFrame(button).Height.ShouldBe(42);
        });
    }

    [Fact]
    public void SplitButton_Custom_Size_Propagates_To_Internal_Buttons()
    {
        var splitButton = new SplitButton
        {
            Content  = "Split",
            Height   = 44,
            FontSize = 15,
            Padding  = new Thickness(18, 0)
        };
        SetSizeType(splitButton, CustomizableSizeType.Custom);

        ShowInWindow(splitButton, () =>
        {
            var primaryButton = splitButton.GetVisualDescendants()
                                           .OfType<AtomUIButton>()
                                           .Single(button => button.Name == "PART_PrimaryButton");
            var secondaryButton = splitButton.GetVisualDescendants()
                                             .OfType<AtomUIButton>()
                                             .Single(button => button.Name == "PART_SecondaryButton");

            GetSizeType(primaryButton).ShouldBe(CustomizableSizeType.Custom);
            GetSizeType(secondaryButton).ShouldBe(CustomizableSizeType.Custom);
            primaryButton.Height.ShouldBe(44);
            secondaryButton.Height.ShouldBe(44);
            primaryButton.FontSize.ShouldBe(15);
            secondaryButton.FontSize.ShouldBe(15);
            primaryButton.Padding.ShouldBe(new Thickness(18, 0));
        });
    }

    [Fact]
    public void SplitButton_Custom_Size_Only_Propagates_Locally_Set_Properties()
    {
        var middleSplitButton = new SplitButton
        {
            Content = "Middle"
        };
        SetSizeType(middleSplitButton, CustomizableSizeType.Middle);

        var customSplitButton = new SplitButton
        {
            Content = "Custom",
            Height  = 44
        };
        SetSizeType(customSplitButton, CustomizableSizeType.Custom);

        var host = new StackPanel
        {
            Children =
            {
                middleSplitButton,
                customSplitButton
            }
        };

        ShowInWindow(host, () =>
        {
            var middlePrimary = FindSplitButtonPart(middleSplitButton, "PART_PrimaryButton");
            var customPrimary = FindSplitButtonPart(customSplitButton, "PART_PrimaryButton");

            customPrimary.Height.ShouldBe(44);
            customPrimary.Padding.ShouldBe(middlePrimary.Padding);
            customPrimary.FontSize.ShouldBe(middlePrimary.FontSize);
        });
    }

    private static AtomUIButton CreateButton(CustomizableSizeType sizeType)
    {
        var button = new AtomUIButton
        {
            Content         = "Button",
            Icon            = CreateIcon(),
            IsMotionEnabled = false
        };
        SetSizeType(button, sizeType);
        return button;
    }

    private static HyperLinkButton CreateHyperLinkButton(CustomizableSizeType sizeType)
    {
        var button = new HyperLinkButton
        {
            Content         = "Link",
            Icon            = CreateIcon(),
            IsMotionEnabled = false
        };
        SetSizeType(button, sizeType);
        return button;
    }

    private static PathIcon CreateIcon()
    {
        return new PathIcon { Data = Geometry.Parse("M0,0 L10,0 L10,10 Z") };
    }

    private static void AssertButtonMetricsMatch(AtomUIButton actual, AtomUIButton expected)
    {
        actual.Height.ShouldBe(expected.Height);
        actual.Padding.ShouldBe(expected.Padding);
        actual.FontSize.ShouldBe(expected.FontSize);
        actual.CornerRadius.ShouldBe(expected.CornerRadius);
        FindFrame(actual).Height.ShouldBe(FindFrame(expected).Height);

        var actualIcon = FindTemplatePart<IconPresenter>(actual, "PART_ButtonIcon");
        var expectedIcon = FindTemplatePart<IconPresenter>(expected, "PART_ButtonIcon");
        actualIcon.Width.ShouldBe(expectedIcon.Width);
        actualIcon.Height.ShouldBe(expectedIcon.Height);

        var actualLoadingIcon = FindTemplatePart<Icon>(actual, "PART_LoadingIcon");
        var expectedLoadingIcon = FindTemplatePart<Icon>(expected, "PART_LoadingIcon");
        actualLoadingIcon.Width.ShouldBe(expectedLoadingIcon.Width);
        actualLoadingIcon.Height.ShouldBe(expectedLoadingIcon.Height);
    }

    private static void SetSizeType(object control, CustomizableSizeType sizeType)
    {
        var property = control.GetType().GetProperty(
            "SizeType",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        property.ShouldNotBeNull();
        property!.SetValue(control, sizeType);
    }

    private static CustomizableSizeType GetSizeType(object control)
    {
        var property = control.GetType().GetProperty(
            "SizeType",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        property.ShouldNotBeNull();
        var value = property!.GetValue(control);
        value.ShouldBeOfType<CustomizableSizeType>();
        return (CustomizableSizeType)value!;
    }

    private static Control FindFrame(Control control)
    {
        return FindTemplatePart<Control>(control, "Frame");
    }

    private static T FindTemplatePart<T>(Control control, string name)
        where T : Control
    {
        var part = control.GetVisualDescendants()
                          .OfType<T>()
                          .SingleOrDefault(item => item.Name == name);
        part.ShouldNotBeNull();
        return part!;
    }

    private static AtomUIButton FindSplitButtonPart(SplitButton splitButton, string name)
    {
        return splitButton.GetVisualDescendants()
                          .OfType<AtomUIButton>()
                          .Single(button => button.Name == name);
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 320,
            Height  = 220,
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
