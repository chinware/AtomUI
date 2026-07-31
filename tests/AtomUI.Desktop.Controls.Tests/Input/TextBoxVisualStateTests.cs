using System.Reflection;
using AtomUI.Controls;
using AtomUI.Controls.Commons;
using AtomUI.Controls.Primitives;
using AtomUI.Desktop.Controls.DesignTokens;
using AtomUI.Theme;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUILineEdit = AtomUI.Desktop.Controls.LineEdit;
using AtomUIEmbeddedTextBox = AtomUI.Desktop.Controls.EmbeddedTextBox;
using AtomUISearchEdit = AtomUI.Desktop.Controls.SearchEdit;
using AtomUITextArea = AtomUI.Desktop.Controls.TextArea;
using AtomUITextBox = AtomUI.Desktop.Controls.TextBox;
using AtomUIScrollViewer = AtomUI.Desktop.Controls.ScrollViewer;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Input;

public class TextBoxVisualStateTests
{
    static TextBoxVisualStateTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Disabled_TextBox_Uses_Disabled_Text_Foreground()
    {
        var textBox = new AtomUITextBox
        {
            Width           = 160,
            Text            = "3",
            IsEnabled       = false,
            IsMotionEnabled = false
        };

        ShowInWindow(textBox, () =>
        {
            var scrollViewer = textBox.GetVisualDescendants()
                                      .OfType<AtomUIScrollViewer>()
                                      .Single(item => item.Name == "ScrollViewer");

            BrushShouldHaveSameColor(
                scrollViewer.Foreground,
                GetThemeResource<IBrush>(SharedTokenKind.ColorTextDisabled));
        });
    }

    [Fact]
    public void TextBox_InnerBoxDecorator_Uses_TextBox_Border_Tokens_For_Default_Hover_And_Focus()
    {
        var textBox = new AtomUITextBox
        {
            Width           = 180,
            IsMotionEnabled = false
        };

        ShowInWindow(textBox, () =>
        {
            var border = FindTemplatePart<PixelAlignedBorder>(textBox, "InnerBoxDecorator");
            var expectedThickness = GetTextBoxTokenResource<Thickness>("BorderThickness");
            var defaultBorder     = GetTextBoxTokenResource<IBrush>("BorderColor");
            var hoverBorder       = GetTextBoxTokenResource<IBrush>("HoverBorderColor");
            var activeBorder      = GetTextBoxTokenResource<IBrush>("ActiveBorderColor");
            var activeShadow      = GetTextBoxTokenResource<BoxShadows>("ActiveShadow");

            textBox.BorderThickness.ShouldBe(expectedThickness);
            border.BorderThickness.ShouldBe(expectedThickness);
            BrushShouldHaveSameColor(border.BorderBrush, defaultBorder);

            SetPseudoClass(textBox, StdPseudoClass.PointerOver, true);
            Dispatcher.UIThread.RunJobs();

            border.BorderThickness.ShouldBe(expectedThickness);
            BrushShouldHaveSameColor(border.BorderBrush, hoverBorder);

            textBox.Focus();
            Dispatcher.UIThread.RunJobs();

            textBox.IsFocused.ShouldBeTrue();
            border.BorderThickness.ShouldBe(expectedThickness);
            BrushShouldHaveSameColor(border.BorderBrush, activeBorder);
            border.BoxShadow.ShouldBe(activeShadow);
        });
    }

    [Fact]
    public void TextBox_Uses_Customizable_SizeType_Contract_And_Control_Shared_Token_Resources()
    {
        typeof(ICustomizableSizeTypeAware).IsAssignableFrom(typeof(AtomUITextBox)).ShouldBeTrue();

        var property = typeof(AtomUITextBox).GetProperty(
            "SizeType",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        property.ShouldNotBeNull();
        property!.PropertyType.ShouldBe(typeof(CustomizableSizeType));

        var field = typeof(AtomUITextBox).GetField(
            "SizeTypeProperty",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        field.ShouldNotBeNull();
        field!.FieldType.ShouldBe(typeof(StyledProperty<CustomizableSizeType>));

        var textBoxTokenType = typeof(AtomUITextBox).Assembly.GetType("AtomUI.Desktop.Controls.TextBoxToken");
        textBoxTokenType.ShouldNotBeNull();
        textBoxTokenType!
            .GetField("ScopeProvider", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
            .ShouldBeNull();

        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/Input/Themes/TextBoxTheme.axaml");
        source.ShouldContain("SharedTokenResource FontSize");
        source.ShouldContain("themeResources:ControlTokenScope.Identity=");
        source.ShouldNotContain("TokenSharedTokenResource");
    }

    [Fact]
    public void TextBox_Uses_Custom_SizeType_For_Template_Owned_Padding_Without_Extra_State()
    {
        var property = typeof(AtomUITextBox).GetProperty(
            "IsCustomPadding",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        property.ShouldBeNull();

        var field = typeof(AtomUITextBox).GetField(
            "IsCustomPaddingProperty",
            BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        field.ShouldBeNull();
    }

    [Fact]
    public void EmbeddedTextBox_Is_Internal_TextBox_For_Template_Owned_Input_Chrome()
    {
        typeof(AtomUITextBox).IsAssignableFrom(typeof(AtomUIEmbeddedTextBox)).ShouldBeTrue();
        typeof(ICustomizableSizeTypeAware).IsAssignableFrom(typeof(AtomUIEmbeddedTextBox)).ShouldBeTrue();

        var embeddedTextBox = new AtomUIEmbeddedTextBox
        {
            Width     = 160,
            Text      = "embedded",
            IsEnabled = false
        };

        ShowInWindow(embeddedTextBox, () =>
        {
            embeddedTextBox.SizeType.ShouldBe(CustomizableSizeType.Custom);
            embeddedTextBox.Padding.ShouldBe(new Thickness(0));
            embeddedTextBox.BorderThickness.ShouldBe(new Thickness(0));
            embeddedTextBox.IsCustomFontSize.ShouldBeTrue();
            embeddedTextBox.IsAllowClear.ShouldBeFalse();
            embeddedTextBox.IsEnableRevealButton.ShouldBeFalse();

            var border = FindTemplatePart<PixelAlignedBorder>(embeddedTextBox, "InnerBoxDecorator");
            border.BorderThickness.ShouldBe(new Thickness(0));
            BrushShouldHaveSameColor(border.Background, Brushes.Transparent);
            BrushShouldHaveSameColor(border.BorderBrush, Brushes.Transparent);
        });
    }

    [Fact]
    public void EmbeddedTextBox_Theme_Suppresses_Own_Disabled_Chrome()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/Input/Themes/EmbeddedTextBoxTheme.axaml");

        source.ShouldContain("TargetType=\"atom:EmbeddedTextBox\"");
        source.ShouldContain("<Setter Property=\"Background\" Value=\"Transparent\" />");
        source.ShouldContain("<Setter Property=\"BorderBrush\" Value=\"Transparent\" />");
        source.ShouldContain("<Setter Property=\"BorderThickness\" Value=\"0\" />");
        source.ShouldContain("<Setter Property=\"Padding\" Value=\"0\" />");
        source.ShouldContain("<Style Selector=\"^:disabled /template/ atom|PixelAlignedBorder#InnerBoxDecorator\">");
        source.ShouldContain("<Setter Property=\"Background\" Value=\"Transparent\" />");
        source.ShouldContain("<Setter Property=\"BorderBrush\" Value=\"Transparent\" />");
        source.ShouldNotContain("ColorBgContainerDisabled");
    }

    [Theory]
    [InlineData("src/AtomUI.Desktop.Controls/NumericUpDown/Themes/NumericUpDownTheme.axaml")]
    [InlineData("src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput/Themes/InfoPickerInputTheme.axaml")]
    [InlineData("src/AtomUI.Desktop.Controls/Primitives/InfoPickerInput/Themes/RangeInfoPickerInputTheme.axaml")]
    [InlineData("src/AtomUI.Desktop.Controls/DatePicker/Themes/RangeDatePickerTheme.axaml")]
    public void TemplateOwned_Input_Chrome_Uses_EmbeddedTextBox(string relativePath)
    {
        var source = ReadRepoFile(relativePath);

        source.ShouldContain("<atom:EmbeddedTextBox");
        source.ShouldNotContain("<atom:TextBox");
    }

    [Fact]
    public void TextBox_Theme_Uses_TextBox_Token_Resources()
    {
        var source = ReadRepoFile("src/AtomUI.Desktop.Controls/Input/Themes/TextBoxTheme.axaml");

        source.ShouldContain("TextBoxTokenResource BorderColor");
        source.ShouldContain("TextBoxTokenResource BorderThickness");
        source.ShouldContain("TextBoxTokenResource BorderRadiusLG");
        source.ShouldContain("TextBoxTokenResource BorderRadius");
        source.ShouldContain("TextBoxTokenResource BorderRadiusSM");
        source.ShouldContain("TextBoxTokenResource HoverBorderColor");
        source.ShouldContain("TextBoxTokenResource ActiveBorderColor");
        source.ShouldContain("TextBoxTokenResource ActiveShadow");
        source.ShouldContain("TextBoxTokenResource PaddingLG");
        source.ShouldContain("TextBoxTokenResource Padding");
        source.ShouldContain("TextBoxTokenResource PaddingSM");
        source.ShouldNotContain("IsCustomPadding");
        source.ShouldContain("SharedTokenResource UniformlyPaddingXXS");
        source.ShouldContain("SharedTokenResource FontHeightLG");
        source.ShouldContain("SharedTokenResource FontHeight");
        source.ShouldContain("SharedTokenResource FontHeightSM");
        source.ShouldContain("SharedTokenResource FontSizeLG");
        source.ShouldContain("SharedTokenResource FontSize");
        source.ShouldContain("SharedTokenResource FontSizeSM");
        source.ShouldContain("SharedTokenResource ColorTextPlaceholder");
        source.ShouldContain("SharedTokenResource ColorTextDisabled");
        source.ShouldContain("themeResources:ControlTokenScope.Identity=");
        source.ShouldNotContain("LineEditTokenResource");
        source.ShouldNotContain("AddOnDecoratedBoxTokenResource");
        source.ShouldNotContain("AddOn");
        source.ShouldNotContain("TextBoxTokenResource LineHeight");
        source.ShouldNotContain("TextBoxTokenResource InputFontSize");
        source.ShouldNotContain("TextBoxTokenResource PlaceholderColor");
        source.ShouldNotContain("TextBoxTokenResource ContentMargin");
        source.ShouldNotContain("TextBoxTokenResource DisabledColor");
        source.ShouldNotContain("TextBoxTokenResource AddOnSpacing");
        source.ShouldNotContain("InnerLeftContent");
        source.ShouldNotContain("InnerLeftContentTemplate");
        source.ShouldNotContain("InnerRightContent");
        source.ShouldNotContain("InnerRightContentTemplate");
        source.ShouldNotContain("PART_LeftInnerContentLayout");
        source.ShouldNotContain("PART_RightInnerContentLayout");
        source.ShouldNotContain("PART_LeftInnerContent");
        source.ShouldNotContain("PART_RightInnerContent");
        source.ShouldContain("<atom:InputClearIconButton");
        source.ShouldContain("Name=\"PART_ClearButton\"");
        source.ShouldContain("<atom:RevealButton");
        source.ShouldContain("Name=\"PART_RevealButton\"");
    }

    [Fact]
    public void TextBox_Token_Only_Exposes_TextBox_Visual_Resources()
    {
        Enum.GetNames(typeof(TextBoxTokenKind))
            .OrderBy(name => name)
            .ShouldBe(new[]
            {
                "ActiveBorderColor",
                "ActiveShadow",
                "BorderColor",
                "BorderRadius",
                "BorderRadiusLG",
                "BorderRadiusSM",
                "BorderThickness",
                "HoverBorderColor",
                "Padding",
                "PaddingLG",
                "PaddingSM"
            });
    }

    [Theory]
    [InlineData(CustomizableSizeType.Large, "PaddingLG")]
    [InlineData(CustomizableSizeType.Middle, "Padding")]
    [InlineData(CustomizableSizeType.Small, "PaddingSM")]
    public void TextBox_Padding_Follows_TextBox_Size_Tokens(CustomizableSizeType sizeType, string tokenKind)
    {
        var textBox = new AtomUITextBox
        {
            Width           = 180,
            SizeType        = sizeType,
            IsMotionEnabled = false
        };

        ShowInWindow(textBox, () =>
        {
            textBox.Padding.ShouldBe(GetTextBoxTokenResource<Thickness>(tokenKind));
        });
    }

    [Fact]
    public void TextBox_Custom_Size_Does_Not_Apply_TextBox_Padding_Token()
    {
        var textBox = new AtomUITextBox
        {
            Width           = 180,
            SizeType        = CustomizableSizeType.Custom,
            IsMotionEnabled = false
        };

        ShowInWindow(textBox, () =>
        {
            textBox.Padding.ShouldBe(new Thickness(0));
        });
    }

    [Fact]
    public void TextBox_Default_Motion_State_Follows_EnableMotion_Resource()
    {
        var textBox = new AtomUITextBox
        {
            Width = 180
        };
        var provider = new ThemeConfigProvider
        {
            Config = new ThemeConfigBuilder()
                     .WithControl(
                         new ControlTokenIdentity("AtomUI", TextBoxToken.ID),
                         new ControlThemeConfigBuilder()
                             .WithAlgorithm(ControlAlgorithmMode.Disabled)
                             .WithToken(nameof(SharedTokenKind.EnableMotion), "false")
                             .Build())
                     .Build(),
            Child = textBox
        };

        ShowInWindow(provider, () =>
        {
            var border = FindTemplatePart<PixelAlignedBorder>(textBox, "InnerBoxDecorator");

            textBox.IsMotionEnabled.ShouldBeFalse();
            border.Transitions.ShouldBeNull();
        });
    }

    [Fact]
    public void TextBox_InnerBoxDecorator_Transitions_Animate_Background_Border_And_Shadow()
    {
        var textBox = new AtomUITextBox
        {
            Width           = 180,
            IsMotionEnabled = true
        };

        ShowInWindow(textBox, () =>
        {
            var border = FindTemplatePart<PixelAlignedBorder>(textBox, "InnerBoxDecorator");
            var transitionProperties = border.Transitions.ShouldNotBeNull()
                                             .Select(transition => transition.Property)
                                             .ToArray();

            transitionProperties.ShouldContain(PixelAlignedBorder.BackgroundProperty);
            transitionProperties.ShouldContain(PixelAlignedBorder.BorderBrushProperty);
            transitionProperties.ShouldContain(PixelAlignedBorder.BoxShadowProperty);
        });
    }

    [Fact]
    public void TextBox_Clear_And_Reveal_Buttons_Use_LineEdit_Default_Icons()
    {
        var textBox = new AtomUITextBox
        {
            Width                = 180,
            Text                 = "secret",
            PasswordChar         = '*',
            IsAllowClear         = true,
            IsEnableRevealButton = true,
            IsMotionEnabled      = false
        };
        var lineEdit = new AtomUILineEdit
        {
            Width                = 180,
            Text                 = "secret",
            PasswordChar         = '*',
            IsAllowClear         = true,
            IsEnableRevealButton = true,
            IsMotionEnabled      = false
        };
        var layout = new StackPanel
        {
            Children =
            {
                textBox,
                lineEdit
            }
        };

        ShowInWindow(layout, () =>
        {
            var textBoxClear  = FindTemplatePart<InputClearIconButton>(textBox, "PART_ClearButton");
            var lineEditClear = FindTemplatePart<InputClearIconButton>(lineEdit, "PART_ClearButton");
            var textBoxReveal  = FindTemplatePart<RevealButton>(textBox, "PART_RevealButton");
            var lineEditReveal = FindTemplatePart<RevealButton>(lineEdit, "PART_RevealButton");

            textBoxClear.IsVisible.ShouldBeTrue();
            lineEditClear.IsVisible.ShouldBeTrue();
            textBoxReveal.IsVisible.ShouldBeTrue();
            lineEditReveal.IsVisible.ShouldBeTrue();

            textBoxClear.Icon.ShouldNotBeNull();
            lineEditClear.Icon.ShouldNotBeNull();
            textBoxClear.Icon!.GetType().ShouldBe(lineEditClear.Icon!.GetType());

            AssertSameIconType(textBoxReveal, lineEditReveal, AbstractToggleIconButton.CheckedIconProperty);
            AssertSameIconType(textBoxReveal, lineEditReveal, AbstractToggleIconButton.UnCheckedIconProperty);
        });
    }

    [Theory]
    [MemberData(nameof(TextInputControlsWithPlaceholder))]
    public void Placeholder_Hides_While_Ime_Preedit_Text_Is_Rendered(Control textInput)
    {
        ShowInWindow(textInput, () =>
        {
            var placeholder = FindTemplatePart<TextBlock>(textInput, "Placeholder");
            var presenter   = FindTemplatePart<TextPresenter>(textInput, "PART_TextPresenter");

            placeholder.IsVisible.ShouldBeTrue();

            presenter.SetCurrentValue(TextPresenter.PreeditTextProperty, "测");
            Dispatcher.UIThread.RunJobs();

            placeholder.IsVisible.ShouldBeFalse(
                "IME preedit text is rendered by TextPresenter before TextBox.Text is committed, so the placeholder must not remain over it.");

            presenter.SetCurrentValue(TextPresenter.PreeditTextProperty, string.Empty);
            Dispatcher.UIThread.RunJobs();

            placeholder.IsVisible.ShouldBeTrue(
                "The placeholder should return when IME preedit text is cleared and the input text is still empty.");
        });
    }

    [Fact]
    public void LineEdit_Forwards_DataValidationErrors_To_AddOnDecoratedBox()
    {
        var lineEdit = new AtomUILineEdit
        {
            Width = 180
        };
        var validationError = new InvalidOperationException("native");

        ShowInWindow(lineEdit, () =>
        {
            var addOnDecoratedBox = lineEdit.GetVisualDescendants()
                                            .OfType<global::AtomUI.Desktop.Controls.AddOnDecoratedBox>()
                                            .Single(item => item.Name == global::AtomUI.Desktop.Controls.AddOnDecoratedBox.AddOnDecoratedBoxPart);

            DataValidationErrors.SetError(lineEdit, validationError);
            Dispatcher.UIThread.RunJobs();

            DataValidationErrors.GetHasErrors(addOnDecoratedBox).ShouldBeTrue();
            DataValidationErrors.GetErrors(addOnDecoratedBox).ShouldBe([validationError]);
        });
    }

    [Fact]
    public void LineEdit_DataValidationError_Overrides_Manual_Warning_For_AddOn_EffectiveStatus()
    {
        var lineEdit = new AtomUILineEdit
        {
            Width  = 180,
            Status = InputControlStatus.Warning
        };
        var validationError = new InvalidOperationException("native");

        ShowInWindow(lineEdit, () =>
        {
            var addOnDecoratedBox = lineEdit.GetVisualDescendants()
                                            .OfType<global::AtomUI.Desktop.Controls.AddOnDecoratedBox>()
                                            .Single(item => item.Name == global::AtomUI.Desktop.Controls.AddOnDecoratedBox.AddOnDecoratedBoxPart);

            addOnDecoratedBox.EffectiveStatus.ShouldBe(InputControlStatus.Warning);

            DataValidationErrors.SetError(lineEdit, validationError);
            Dispatcher.UIThread.RunJobs();

            addOnDecoratedBox.EffectiveStatus.ShouldBe(InputControlStatus.Error);

            DataValidationErrors.ClearErrors(lineEdit);
            Dispatcher.UIThread.RunJobs();

            addOnDecoratedBox.EffectiveStatus.ShouldBe(InputControlStatus.Warning);
        });
    }

    [Fact]
    public void LineEdit_Manual_Error_Status_Does_Not_Set_Native_Error_PseudoClass()
    {
        var lineEdit = new AtomUILineEdit
        {
            Width  = 180,
            Status = InputControlStatus.Error
        };

        ShowInWindow(lineEdit, () =>
        {
            lineEdit.Classes.Contains(":error").ShouldBeFalse();
            DataValidationErrors.GetHasErrors(lineEdit).ShouldBeFalse();
        });
    }

    public static TheoryData<Control> TextInputControlsWithPlaceholder()
    {
        return new TheoryData<Control>
        {
            new AtomUITextBox
            {
                Width           = 180,
                PlaceholderText = "请输入"
            },
            new AtomUILineEdit
            {
                Width           = 180,
                PlaceholderText = "请输入"
            },
            new AtomUISearchEdit
            {
                Width           = 180,
                PlaceholderText = "搜索"
            },
            new AtomUITextArea
            {
                Width           = 180,
                Height          = 80,
                PlaceholderText = "请输入"
            }
        };
    }

    private static T GetThemeResource<T>(object key)
    {
        var application = Application.Current;
        application.ShouldNotBeNull();
        application!.TryGetResource(key, application.ActualThemeVariant, out var value).ShouldBeTrue();
        value.ShouldBeAssignableTo<T>();
        return (T)value!;
    }

    private static T GetTextBoxTokenResource<T>(string kindName)
    {
        var tokenKind = (TextBoxTokenKind)Enum.Parse(typeof(TextBoxTokenKind), kindName);
        return GetThemeResource<T>(tokenKind);
    }

    private static void BrushShouldHaveSameColor(IBrush? actual, IBrush? expected)
    {
        GetSolidBrushColor(actual).ShouldBe(GetSolidBrushColor(expected));
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

    private static Color GetSolidBrushColor(IBrush? brush)
    {
        brush.ShouldNotBeNull();
        brush.ShouldBeAssignableTo<ISolidColorBrush>();
        return ((ISolidColorBrush)brush!).Color;
    }

    private static void SetPseudoClass(Control control, string pseudoClass, bool value)
    {
        ((IPseudoClasses)control.Classes).Set(pseudoClass, value);
    }

    private static void AssertSameIconType(AvaloniaObject actualOwner,
                                           AvaloniaObject expectedOwner,
                                           StyledProperty<PathIcon?> property)
    {
        var actual   = actualOwner.GetValue(property);
        var expected = expectedOwner.GetValue(property);

        actual.ShouldNotBeNull();
        expected.ShouldNotBeNull();
        actual!.GetType().ShouldBe(expected!.GetType());
    }

    private static void ShowInWindow(Control content, Action assertion, Action<AvaloniaWindow>? configureWindow = null)
    {
        var window = new AvaloniaWindow
        {
            Width   = 240,
            Height  = 120,
            Content = content
        };
        configureWindow?.Invoke(window);

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

    private static string ReadRepoFile(string relativePath)
    {
        var path = GetRepoFile(relativePath);
        File.Exists(path).ShouldBeTrue($"Expected repository file to exist: {relativePath}");
        return File.ReadAllText(path);
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return Path.Combine(AppContext.BaseDirectory, relativePath);
    }
}
