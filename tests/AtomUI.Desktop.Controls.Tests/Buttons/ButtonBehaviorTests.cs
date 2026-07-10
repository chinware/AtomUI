using System;
using System.Linq;
using System.Reflection;
using AtomUI.Controls;
using AtomUI.Controls.Primitives;
using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
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

    [Theory]
    [InlineData(ButtonType.Default, false, "Default", "Outlined", true)]
    [InlineData(ButtonType.Dashed, false, "Default", "Dashed", true)]
    [InlineData(ButtonType.Primary, false, "Primary", "Solid", true)]
    [InlineData(ButtonType.Text, false, "Default", "Text", false)]
    [InlineData(ButtonType.Link, false, "Default", "Link", false)]
    [InlineData(ButtonType.Default, true, "Danger", "Outlined", true)]
    [InlineData(ButtonType.Primary, true, "Danger", "Solid", true)]
    [InlineData(ButtonType.Text, true, "Danger", "Text", false)]
    [InlineData(ButtonType.Link, true, "Danger", "Link", false)]
    public void Button_Compatibility_Apis_Configure_Effective_Color_And_Variant(
        ButtonType buttonType,
        bool isDanger,
        string expectedColor,
        string expectedVariant,
        bool expectedBordered)
    {
        var borderThickness = new Thickness(2);
        var button = new AtomUIButton
        {
            ButtonType      = buttonType,
            IsDanger        = isDanger,
            BorderThickness = borderThickness,
            IsMotionEnabled = false
        };

        ShowInWindow(button, () =>
        {
            GetInternalPropertyValue(button, "EffectiveColor")
                .ToString()
                .ShouldBe(expectedColor);
            GetInternalPropertyValue(button, "EffectiveVariant")
                .ToString()
                .ShouldBe(expectedVariant);
            GetInternalPropertyValue<bool>(button, "EffectiveIsBordered")
                .ShouldBe(expectedBordered);
            GetInternalPropertyValue<Thickness>(button, "EffectiveBorderThickness")
                .ShouldBe(expectedBordered ? borderThickness : new Thickness(0));
        });
    }

    [Fact]
    public void Button_Explicit_Color_And_Variant_Override_ButtonType_And_Danger()
    {
        var button = new AtomUIButton
        {
            ButtonType      = ButtonType.Primary,
            IsDanger        = true,
            Color           = ButtonColor.Pink,
            Variant         = ButtonVariant.Filled,
            BorderThickness = new Thickness(2),
            IsMotionEnabled = false
        };

        ShowInWindow(button, () =>
        {
            GetInternalPropertyValue<ButtonColor>(button, "EffectiveColor")
                .ShouldBe(ButtonColor.Pink);
            GetInternalPropertyValue<ButtonVariant>(button, "EffectiveVariant")
                .ShouldBe(ButtonVariant.Filled);
            GetInternalPropertyValue<bool>(button, "EffectiveIsBordered")
                .ShouldBeFalse();
            GetInternalPropertyValue<Thickness>(button, "EffectiveBorderThickness")
                .ShouldBe(new Thickness(0));
        });
    }

    [Fact]
    public void Button_IconPlacement_Defaults_To_Start_And_Can_Move_Icon_To_End()
    {
        var iconPlacementProperty = typeof(AtomUIButton).GetProperty(
            "IconPlacement",
            BindingFlags.Instance | BindingFlags.Public);
        iconPlacementProperty.ShouldNotBeNull();
        iconPlacementProperty!.PropertyType.IsEnum.ShouldBeTrue();
        Enum.GetNames(iconPlacementProperty.PropertyType).ShouldBe(["Start", "End"], ignoreOrder: false);

        var startButton = CreateIconPlacementButton();
        var endButton   = CreateIconPlacementButton();
        var endValue    = Enum.Parse(iconPlacementProperty.PropertyType, "End");
        iconPlacementProperty.SetValue(endButton, endValue);

        var host = new StackPanel
        {
            Children =
            {
                startButton,
                endButton
            }
        };

        ShowInWindow(host, () =>
        {
            iconPlacementProperty.GetValue(startButton)!.ToString().ShouldBe("Start");

            var startIcon = FindTemplateIcon(startButton);
            var endIcon   = FindTemplateIcon(endButton);

            DockPanel.GetDock(startIcon).ShouldBe(Dock.Left);
            DockPanel.GetDock(endIcon).ShouldBe(Dock.Right);
            endIcon.Margin.Left.ShouldBe(startIcon.Margin.Right);
            endIcon.Margin.Right.ShouldBe(startIcon.Margin.Left);
        });
    }

    [Fact]
    public void Button_Solid_Variant_Without_Color_Uses_Primary_Color()
    {
        var button = new AtomUIButton
        {
            Variant         = ButtonVariant.Solid,
            BorderThickness = new Thickness(2),
            IsMotionEnabled = false
        };

        ShowInWindow(button, () =>
        {
            GetInternalPropertyValue<ButtonColor>(button, "EffectiveColor")
                .ShouldBe(ButtonColor.Primary);
            GetInternalPropertyValue<ButtonVariant>(button, "EffectiveVariant")
                .ShouldBe(ButtonVariant.Solid);
            GetInternalPropertyValue<bool>(button, "EffectiveIsBordered")
                .ShouldBeTrue();
            GetInternalPropertyValue<Thickness>(button, "EffectiveBorderThickness")
                .ShouldBe(button.BorderThickness);
        });
    }

    [Fact]
    public void Button_Default_Color_And_Solid_Variant_Uses_Default_Solid_Surface()
    {
        var button = new AtomUIButton
        {
            Color           = ButtonColor.Default,
            Variant         = ButtonVariant.Solid,
            BorderThickness = new Thickness(2),
            IsMotionEnabled = false
        };

        ShowInWindow(button, () =>
        {
            GetInternalPropertyValue<ButtonColor>(button, "EffectiveColor")
                .ShouldBe(ButtonColor.Default);
            GetInternalPropertyValue<ButtonVariant>(button, "EffectiveVariant")
                .ShouldBe(ButtonVariant.Solid);
            GetInternalPropertyValue<bool>(button, "EffectiveIsBordered")
                .ShouldBeTrue();
            GetInternalPropertyValue<Thickness>(button, "EffectiveBorderThickness")
                .ShouldBe(button.BorderThickness);
            BrushShouldHaveSameColor(button.Foreground, GetInternalPropertyValue<IBrush?>(button, "VariantTextBrush"));
            BrushShouldHaveSameColor(button.Background, GetInternalPropertyValue<IBrush?>(button, "VariantBackgroundBrush"));

            var foreground = GetSolidBrushColor(button.Foreground);
            var background = GetSolidBrushColor(button.Background);
            background.A.ShouldBe((byte)255);
            foreground.ShouldNotBe(background);
        });
    }

    [Theory]
    [InlineData("button-type-primary")]
    [InlineData("explicit-primary-solid")]
    public void Button_Primary_Variant_Background_Follows_Scoped_ColorPrimary(
        string scenario)
    {
        var scopedPrimaryColor = Color.Parse("#00A1D6");
        var button             = CreateScopedPrimaryButton(scenario);
        var provider = new ThemeConfigProvider
        {
            SharedTokenSetters =
            [
                new TokenSetter(null, "ColorPrimary", "#00A1D6")
            ],
            Content = button
        };

        ShowInWindow(provider, () =>
        {
            BrushShouldHaveColor(button.Background, scopedPrimaryColor);
            BrushShouldHaveColor(GetInternalPropertyValue<IBrush?>(button, "VariantBackgroundBrush"),
                scopedPrimaryColor);
            BrushShouldHaveSameColor(button.Background,
                GetInternalPropertyValue<IBrush?>(button, "VariantBackgroundBrush"));
        });
    }

    [Fact]
    public void Button_Ghost_Solid_Variant_Becomes_Outlined()
    {
        var button = new AtomUIButton
        {
            Color           = ButtonColor.Primary,
            Variant         = ButtonVariant.Solid,
            IsGhost         = true,
            BorderThickness = new Thickness(2),
            IsMotionEnabled = false
        };

        ShowInWindow(button, () =>
        {
            GetInternalPropertyValue<ButtonColor>(button, "EffectiveColor")
                .ShouldBe(ButtonColor.Primary);
            GetInternalPropertyValue<ButtonVariant>(button, "EffectiveVariant")
                .ShouldBe(ButtonVariant.Outlined);
            GetInternalPropertyValue<bool>(button, "EffectiveIsBordered")
                .ShouldBeTrue();
            GetInternalPropertyValue<Thickness>(button, "EffectiveBorderThickness")
                .ShouldBe(button.BorderThickness);
        });
    }

    [Theory]
    [InlineData(ButtonVariant.Text)]
    [InlineData(ButtonVariant.Link)]
    public void Button_Text_And_Link_Variants_Keep_Normal_Surface_Transparent_And_Shadowless(
        ButtonVariant variant)
    {
        var button = new AtomUIButton
        {
            Color           = ButtonColor.Pink,
            Variant         = variant,
            BorderThickness = new Thickness(2),
            IsMotionEnabled = false
        };

        ShowInWindow(button, () =>
        {
            BrushShouldBeTransparent(button.Background);
            BrushShouldBeTransparent(GetInternalPropertyValue<IBrush?>(button, "VariantBackgroundBrush"));
            GetInternalPropertyValue<BoxShadows>(button, "VariantShadow")
                .ShouldBe(new BoxShadows());
            GetInternalPropertyValue<Thickness>(button, "EffectiveBorderThickness")
                .ShouldBe(new Thickness(0));

            var shadowFrame = FindTemplateBorder(button, "ShadowsFrame");
            BrushShouldBeTransparent(shadowFrame.Background);
            shadowFrame.BoxShadow.ShouldBe(new BoxShadows());
        });
    }

    [Theory]
    [InlineData("primary-solid", true)]
    [InlineData("primary-outlined", false)]
    [InlineData("primary-text", false)]
    [InlineData("danger-solid", false)]
    [InlineData("disabled-primary-solid", false)]
    public void Button_CustomBackground_Visibility_Follows_Effective_State(
        string scenario,
        bool expectedVisible)
    {
        var customBackground = CreateCustomBackground();
        var button           = CreateCustomBackgroundButton(scenario, customBackground);

        ShowInWindow(button, () =>
        {
            GetInternalPropertyValue<bool>(button, "HasCustomBackground")
                .ShouldBe(expectedVisible);

            var customBackgroundLayer = FindTemplateBorder(button, "CustomBackgroundLayer");
            customBackgroundLayer.Background.ShouldBeSameAs(customBackground);
            customBackgroundLayer.IsVisible.ShouldBe(expectedVisible);
            customBackgroundLayer.Opacity.ShouldBe(expectedVisible ? 1.0 : 0.0);
        });
    }

    [Theory]
    [InlineData(ButtonColor.Pink, ButtonVariant.Solid, "VariantBackgroundBrush")]
    [InlineData(ButtonColor.Pink, ButtonVariant.Outlined, "VariantBorderBrush")]
    [InlineData(ButtonColor.Cyan, ButtonVariant.Dashed, "VariantBorderBrush")]
    public void Button_WaveSpiritBrush_Follows_Effective_Color_And_Variant(
        ButtonColor color,
        ButtonVariant variant,
        string expectedBrushProperty)
    {
        var button = new AtomUIButton
        {
            Color           = color,
            Variant         = variant,
            IsMotionEnabled = false
        };

        ShowInWindow(button, () =>
        {
            var waveSpiritDecorator = GetPrivateFieldValue(button, "_waveSpiritDecorator");
            var waveBrush           = GetPublicPropertyValue<IBrush?>(waveSpiritDecorator, "WaveBrush");
            var expectedBrush       = GetInternalPropertyValue<IBrush?>(button, expectedBrushProperty);

            BrushShouldHaveSameColor(waveBrush, expectedBrush);
        });
    }

    [Fact]
    public void Button_CustomBackground_Does_Not_Change_WaveSpiritBrush()
    {
        var customBackground = CreateCustomBackground();
        var button = new AtomUIButton
        {
            ButtonType       = ButtonType.Primary,
            CustomBackground = customBackground,
            IsMotionEnabled  = false
        };

        ShowInWindow(button, () =>
        {
            var waveSpiritDecorator = GetPrivateFieldValue(button, "_waveSpiritDecorator");
            var waveBrush           = GetPublicPropertyValue<IBrush?>(waveSpiritDecorator, "WaveBrush");
            var expectedBrush       = GetInternalPropertyValue<IBrush?>(button, "VariantBackgroundBrush");

            waveBrush.ShouldNotBeSameAs(customBackground);
            BrushShouldHaveSameColor(waveBrush, expectedBrush);
        });
    }

    [Fact]
    public void Button_CustomBackground_Layer_Overlays_Frame_In_Normal_State()
    {
        var button = new AtomUIButton
        {
            ButtonType       = ButtonType.Primary,
            CustomBackground = CreateCustomBackground(),
            IsMotionEnabled  = false
        };

        ShowInWindow(button, () =>
        {
            var frame                 = FindTemplateDashedBorder(button, "Frame");
            var customBackgroundLayer = FindTemplateBorder(button, "CustomBackgroundLayer");
            var rootPanel             = frame.GetVisualParent<Panel>();
            rootPanel.ShouldNotBeNull();

            var rootChildren                = rootPanel!.GetVisualChildren().ToList();
            var frameIndex                  = rootChildren.IndexOf(frame);
            var customBackgroundLayerIndex  = rootChildren.IndexOf(customBackgroundLayer);

            frameIndex.ShouldBeGreaterThanOrEqualTo(0);
            customBackgroundLayerIndex.ShouldBeGreaterThan(frameIndex);
            BrushShouldHaveSameColor(frame.Background, GetInternalPropertyValue<IBrush?>(button, "VariantBackgroundBrush"));
        });
    }

    [Fact]
    public void Button_Default_Frame_Uses_AtomUI_Border_Rendering()
    {
        var button = new AtomUIButton
        {
            ButtonType      = ButtonType.Default,
            BorderThickness = new Thickness(1),
            BorderBrush     = Brushes.Black,
            IsMotionEnabled = false
        };

        ShowInWindow(button, window =>
        {
            window.SetRenderScaling(1.5);
            Dispatcher.UIThread.RunJobs();

            var frame = FindTemplateDashedBorder(button, "Frame");
            RenderToDrawingGroup(frame);

            GetBorderRenderHelperThickness(frame).ShouldBe(new Thickness(2d / 3d));
        });
    }

    private static AtomUIButton CreateCustomBackgroundButton(string scenario, IBrush customBackground)
    {
        var button = new AtomUIButton
        {
            CustomBackground = customBackground,
            IsMotionEnabled  = false
        };

        switch (scenario)
        {
            case "primary-solid":
                button.ButtonType = ButtonType.Primary;
                break;
            case "primary-outlined":
                button.Color   = ButtonColor.Primary;
                button.Variant = ButtonVariant.Outlined;
                break;
            case "primary-text":
                button.Color   = ButtonColor.Primary;
                button.Variant = ButtonVariant.Text;
                break;
            case "danger-solid":
                button.ButtonType = ButtonType.Primary;
                button.IsDanger   = true;
                break;
            case "disabled-primary-solid":
                button.ButtonType = ButtonType.Primary;
                button.IsEnabled  = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
        }

        return button;
    }

    private static AtomUIButton CreateScopedPrimaryButton(string scenario)
    {
        var button = new AtomUIButton
        {
            IsMotionEnabled = false
        };

        switch (scenario)
        {
            case "button-type-primary":
                button.ButtonType = ButtonType.Primary;
                break;
            case "explicit-primary-solid":
                button.Color   = ButtonColor.Primary;
                button.Variant = ButtonVariant.Solid;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
        }

        return button;
    }

    private static LinearGradientBrush CreateCustomBackground()
    {
        return new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop { Color = Colors.MediumPurple, Offset = 0 },
                new GradientStop { Color = Colors.DeepSkyBlue, Offset   = 1 }
            }
        };
    }

    private static AtomUIButton CreateIconPlacementButton()
    {
        return new AtomUIButton
        {
            Content         = "Search",
            Icon            = new PathIcon { Data = Geometry.Parse("M0,0 L10,0 L10,10 Z") },
            IsMotionEnabled = false
        };
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

    private static object GetPrivateFieldValue(object target, string fieldName)
    {
        var field = target.GetType().GetField(
            fieldName,
            BindingFlags.Instance | BindingFlags.NonPublic);
        field.ShouldNotBeNull();
        var value = field.GetValue(target);
        value.ShouldNotBeNull();
        return value;
    }

    private static T GetPublicPropertyValue<T>(object target, string propertyName)
    {
        var property = target.GetType().GetProperty(
            propertyName,
            BindingFlags.Instance | BindingFlags.Public);
        property.ShouldNotBeNull();
        return (T)property.GetValue(target)!;
    }

    private static Border FindTemplateBorder(Control control, string name)
    {
        var border = control.GetVisualDescendants()
                            .OfType<Border>()
                            .SingleOrDefault(item => item.Name == name);
        border.ShouldNotBeNull();
        return border;
    }

    private static DashedBorder FindTemplateDashedBorder(Control control, string name)
    {
        var border = control.GetVisualDescendants()
                            .OfType<DashedBorder>()
                            .SingleOrDefault(item => item.Name == name);
        border.ShouldNotBeNull();
        return border!;
    }

    private static DrawingGroup RenderToDrawingGroup(Control control)
    {
        var drawingGroup = new DrawingGroup();
        using (var context = drawingGroup.Open())
        {
            control.Render(context);
        }

        return drawingGroup;
    }

    private static Thickness GetBorderRenderHelperThickness(DashedBorder border)
    {
        var helperField = typeof(DashedBorder).GetField(
            "_borderRenderHelper",
            BindingFlags.Instance | BindingFlags.NonPublic);
        helperField.ShouldNotBeNull();

        var helper = helperField!.GetValue(border);
        helper.ShouldNotBeNull();

        var thicknessField = helper!.GetType().GetField(
            "_borderThickness",
            BindingFlags.Instance | BindingFlags.NonPublic);
        thicknessField.ShouldNotBeNull();

        return (Thickness)thicknessField!.GetValue(helper)!;
    }

    private static IconPresenter FindTemplateIcon(Control control)
    {
        var icon = control.GetVisualDescendants()
                          .OfType<IconPresenter>()
                          .SingleOrDefault(item => item.Name == "PART_ButtonIcon");
        icon.ShouldNotBeNull();
        return icon!;
    }

    private static void BrushShouldBeTransparent(IBrush? brush)
    {
        if (brush is null)
        {
            return;
        }

        GetSolidBrushColor(brush).A.ShouldBe((byte)0);
    }

    private static void BrushShouldHaveSameColor(IBrush? actual, IBrush? expected)
    {
        GetSolidBrushColor(actual).ShouldBe(GetSolidBrushColor(expected));
    }

    private static void BrushShouldHaveColor(IBrush? actual, Color expected)
    {
        GetSolidBrushColor(actual).ShouldBe(expected);
    }

    private static Color GetSolidBrushColor(IBrush? brush)
    {
        brush.ShouldNotBeNull();
        brush.ShouldBeAssignableTo<ISolidColorBrush>();
        return ((ISolidColorBrush)brush!).Color;
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        ShowInWindow(content, _ => assertion());
    }

    private static void ShowInWindow(Control content, Action<AvaloniaWindow> assertion)
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
            assertion(window);
        }
        finally
        {
            window.Close();
        }
    }
}
