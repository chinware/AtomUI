using System;
using System.Linq;
using AtomUI.Controls.Primitives;
using AtomUI.Theme;
using AtomUI.Theme.Configuration;
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
        var button = CreatePrimaryButton("Save");
        var provider = Provider(
            button,
            ComponentSharedToken(ButtonToken.ID, nameof(DesignToken.ColorPrimary), "#00b96b"));

        ShowInWindow(provider, () =>
        {
            SetPrimary(button);
            BrushColor(button.Background).ShouldBe(ButtonPrimary);

            provider.Config = BuildComponentConfig(
                ComponentSharedToken(ButtonToken.ID, nameof(DesignToken.ColorPrimary), "#ff4d4f"));
            FlushThemeUpdates();

            BrushColor(button.Background).ShouldBe(UpdatedButtonPrimary);
        });
    }

    [Fact]
    public void Nested_TextBox_Uses_TextBox_Component_Config_Inside_Button()
    {
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
        var button = CreatePrimaryButton("Save");
        var provider = new ThemeConfigProvider
        {
            Child  = button,
            Config = new ThemeConfigBuilder()
                     .WithToken(nameof(DesignToken.ColorPrimary), "#00b96b")
                     .Build()
        };
        ThrowOnCompileFailure(provider);

        ShowInWindow(provider, () =>
        {
            SetPrimary(button);
            BrushColor(button.Background).ShouldBe(ButtonPrimary);

            provider.Config = new ThemeConfigBuilder()
                              .WithToken(nameof(DesignToken.ColorPrimary), "#ff4d4f")
                              .Build();
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
        params ComponentTokenConfig[] componentConfigs)
    {
        var provider = new ThemeConfigProvider
        {
            Child   = content,
            Config  = BuildComponentConfig(componentConfigs)
        };
        ThrowOnCompileFailure(provider);
        return provider;
    }

    private static void ThrowOnCompileFailure(ThemeConfigProvider provider)
    {
        provider.ThemeChangeFailed += static (_, args) =>
        {
            var diagnostics = string.Join(
                Environment.NewLine,
                args.Diagnostics.Select(diagnostic => diagnostic.Message));
            throw new InvalidOperationException(args.Exception?.Message ?? diagnostics);
        };
    }

    private static ComponentTokenConfig ComponentSharedToken(
        string tokenId,
        string key,
        string value)
    {
        return new ComponentTokenConfig(tokenId, key, value);
    }

    private static ThemeConfig BuildComponentConfig(params ComponentTokenConfig[] componentConfigs)
    {
        var builder = new ThemeConfigBuilder();
        foreach (var component in componentConfigs)
        {
            builder.WithControl(
                new ControlTokenIdentity("AtomUI", component.TokenId),
                new ControlThemeConfigBuilder()
                    .WithAlgorithm(ControlAlgorithmMode.Disabled)
                    .WithToken(component.Key, component.Value)
                    .Build());
        }

        return builder.Build();
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

    private sealed record ComponentTokenConfig(string TokenId, string Key, string Value);
}

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class ButtonThemeScopeTestCollection
{
    public const string Name = "ButtonThemeScope";
}
