using System;
using System.Linq;
using AtomUI.Controls.Primitives;
using AtomUI.Generated.AtomUI_Desktop_Controls;
using AtomUI.Theme;
using AtomUI.Theme.Schema;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUIButton = AtomUI.Desktop.Controls.Button;
using AtomUITextBox = AtomUI.Desktop.Controls.TextBox;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Buttons;

[Collection(ButtonThemeScopeTestCollection.Name)]
public class ButtonThemeScopeTests
{
    private static readonly Color GlobalColorPrimary = Color.Parse("#1677ff");
    private static readonly Color ButtonPrimary = Color.Parse("#00b96b");
    private static readonly Color UpdatedButtonPrimary = Color.Parse("#ff4d4f");
    private static readonly Color TextBoxPrimary = Color.Parse("#722ed1");

    static ButtonThemeScopeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Button_Component_ColorPrimary_Does_Not_Leak_To_Content()
    {
        using var _ = UseThemeManager();
        var content = new Border
        {
            Width  = 12,
            Height = 12
        };
        content.Bind(
            Border.BackgroundProperty,
            new DynamicResourceExtension(SharedTokenKind.ColorPrimary));

        var button = CreatePrimaryButton(content);
        var provider = Provider(
            button,
            ComponentSharedToken(ButtonToken.ID, nameof(DesignToken.ColorPrimary), "#00b96b"));

        ShowInWindow(provider, () =>
        {
            SetPrimary(button);

            BrushColor(button.Background).ShouldBe(ButtonPrimary);
            BrushColor(content.Background).ShouldBe(GlobalColorPrimary);
        });
    }

    [Fact]
    public void Button_Component_ColorPrimary_Update_Refreshes_Without_Reattach()
    {
        using var _ = UseThemeManager();
        var primarySetter = new TokenSetter(null, nameof(DesignToken.ColorPrimary), "#00b96b");
        var button        = CreatePrimaryButton("Save");
        var provider = Provider(
            button,
            Component(ButtonToken.ID, primarySetter));

        ShowInWindow(provider, () =>
        {
            SetPrimary(button);
            BrushColor(button.Background).ShouldBe(ButtonPrimary);

            primarySetter.Value = "#ff4d4f";
            FlushThemeUpdates();

            BrushColor(button.Background).ShouldBe(UpdatedButtonPrimary);
        });
    }

    [Fact]
    public void Nested_TextBox_Uses_TextBox_Component_Config_Inside_Button()
    {
        using var _ = UseThemeManager();
        var textBox = new AtomUITextBox
        {
            Width           = 120,
            IsMotionEnabled = false
        };
        var button = CreatePrimaryButton(textBox);
        var provider = Provider(
            button,
            ComponentSharedToken(ButtonToken.ID, nameof(DesignToken.ColorPrimary), "#00b96b"),
            ComponentSharedToken(TextBoxToken.ID, nameof(DesignToken.ColorPrimary), "#722ed1"));

        ShowInWindow(provider, () =>
        {
            SetPrimary(button);
            textBox.Focus();
            FlushThemeUpdates();

            var innerBox = textBox.GetVisualDescendants()
                                  .OfType<PixelAlignedBorder>()
                                  .Single(item => item.Name == "InnerBoxDecorator");
            BrushColor(button.Background).ShouldBe(ButtonPrimary);
            BrushColor(innerBox.BorderBrush).ShouldBe(TextBoxPrimary);
        });
    }

    [Fact]
    public void Button_Component_Shared_Resource_Falls_Back_To_Updated_Global_Token()
    {
        using var _ = UseThemeManager();
        var primarySetter = new TokenSetter(null, nameof(DesignToken.ColorPrimary), "#00b96b");
        var button = CreatePrimaryButton("Save");
        var provider = new ThemeConfigProvider
        {
            Content = button
        };
        ThrowOnCompileFailure(provider);
        provider.SharedTokenSetters.Add(primarySetter);

        ShowInWindow(provider, () =>
        {
            SetPrimary(button);
            BrushColor(button.Background).ShouldBe(ButtonPrimary);

            primarySetter.Value = "#ff4d4f";
            FlushThemeUpdates();

            BrushColor(button.Background).ShouldBe(UpdatedButtonPrimary);
        });
    }

    private static AtomUIButton CreatePrimaryButton(object? content)
    {
        return new AtomUIButton
        {
            IsMotionEnabled = false,
            Content         = content
        };
    }

    private static void SetPrimary(AtomUIButton button)
    {
        button.ButtonType = ButtonType.Primary;
        FlushThemeUpdates();
    }

    private static ThemeConfigProvider Provider(
        Control content,
        params ControlTokenInfoSetter[] componentSetters)
    {
        var provider = new ThemeConfigProvider
        {
            Content = content
        };
        ThrowOnCompileFailure(provider);
        foreach (var setter in componentSetters)
        {
            provider.ControlTokenInfoSetters.Add(setter);
        }

        return provider;
    }

    private static void ThrowOnCompileFailure(ThemeConfigProvider provider)
    {
        provider.ThemeScopeCompileFailed += static (_, args) =>
        {
            var diagnostics = string.Join(
                Environment.NewLine,
                args.Diagnostics.Select(diagnostic => diagnostic.Message));
            throw new InvalidOperationException(args.Exception?.Message ?? diagnostics);
        };
    }

    private static ControlTokenInfoSetter ComponentSharedToken(
        string tokenId,
        string key,
        string value)
    {
        return Component(tokenId, new TokenSetter(null, key, value));
    }

    private static ControlTokenInfoSetter Component(string tokenId, params TokenSetter[] setters)
    {
        var component = new ControlTokenInfoSetter(tokenId);
        foreach (var setter in setters)
        {
            component.Setters.Add(setter);
        }

        return component;
    }

    private static IDisposable UseThemeManager()
    {
        var scope = AvaloniaLocator.EnterScope();
        var manager = new ThemeManager();
        foreach (var descriptor in GeneratedThemeSchema.GetControls())
        {
            manager.RegisterControlTokenDescriptor(descriptor);
        }
        AvaloniaLocator.CurrentMutable.BindToSelf(manager);
        return scope;
    }

    private static void FlushThemeUpdates()
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.RunJobs();
            return;
        }

        Dispatcher.UIThread.Invoke(static () => Dispatcher.UIThread.RunJobs());
    }

    private static Color BrushColor(IBrush? brush)
    {
        brush.ShouldNotBeNull();
        brush.ShouldBeAssignableTo<ISolidColorBrush>();
        return ((ISolidColorBrush)brush!).Color;
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
            FlushThemeUpdates();
            assertion();
        }
        finally
        {
            window.Close();
        }
    }
}

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ButtonThemeScopeTestCollection
{
    public const string Name = "ButtonThemeScope";
}
