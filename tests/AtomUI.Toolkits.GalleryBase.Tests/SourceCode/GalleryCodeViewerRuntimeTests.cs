using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AtomUI.Controls;
using AtomUI.Theme;
using AtomUI.Toolkits.GalleryBase.Controls;
using AtomUI.Toolkits.GalleryBase.SourceCode;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.Threading;
using Avalonia.VisualTree;
using AvaloniaEdit;
using AvaloniaEdit.Editing;
using Shouldly;
using TextMateSharp.Registry;
using TextMateSharp.Grammars;
using Xunit;
using AtomUIContextMenu = AtomUI.Desktop.Controls.ContextMenu;
using AtomUIMenuItem = AtomUI.Desktop.Controls.MenuItem;
using DesktopTabControl = AtomUI.Desktop.Controls.TabControl;
using DesktopTabItem = AtomUI.Desktop.Controls.TabItem;

namespace AtomUI.Toolkits.GalleryBase.Tests.SourceCode;

public class GalleryCodeViewerRuntimeTests
{
    static GalleryCodeViewerRuntimeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void GalleryCodeViewer_Applies_TextEditor_Template_Without_Fluent_Theme_Resources()
    {
        var viewer = new GalleryCodeViewer
        {
            CodeText = "public class Demo { }",
            Language = "csharp"
        };

        Should.NotThrow(() => ShowInWindow(viewer, _ => { }));
    }

    [Fact]
    public void GalleryCodeViewer_Uses_Light_And_Dark_Syntax_Theme_Defaults()
    {
        var viewer = new GalleryCodeViewer();

        viewer.LightSyntaxTheme.ShouldBe(ThemeName.LightPlus);
        viewer.DarkSyntaxTheme.ShouldBe(ThemeName.DarkPlus);
        viewer.Dispose();
    }

    [Fact]
    public void GalleryCodeViewer_Updates_TextMate_Theme_When_ThemeVariant_Changes()
    {
        var viewer = new GalleryCodeViewer
        {
            CodeText = "<WrapPanel><Button Content=\"Demo\" /></WrapPanel>",
            Language = "axaml"
        };
        var themeScope = new ThemeVariantScope
        {
            RequestedThemeVariant = ThemeVariant.Light,
            Child                 = viewer
        };

        ShowInWindow(viewer, _ =>
        {
            GetTextMateThemeColor(viewer, "editor.background").ShouldBe("#FFFFFF", StringCompareShould.IgnoreCase);

            themeScope.RequestedThemeVariant = ThemeVariant.Dark;
            Dispatcher.UIThread.RunJobs();

            GetTextMateThemeColor(viewer, "editor.background")
                .ToUpperInvariant()
                .ShouldNotBe("#FFFFFF");
        }, themeScope);
    }

    [Fact]
    public void GalleryCodeViewer_Horizontal_ScrollBar_Starts_After_Line_Number_Gutter()
    {
        var viewer = new GalleryCodeViewer
        {
            CodeText = string.Join(
                "\n",
                Enumerable.Range(1, 40).Select(index => $"{index}: {new string('x', 220)}")),
            Language = "csharp"
        };

        ShowInWindow(viewer, editor =>
        {
            var lineNumberMargin = editor.GetVisualDescendants()
                                         .OfType<LineNumberMargin>()
                                         .Single();
            var horizontalScrollBar = editor.GetVisualDescendants()
                                            .OfType<ScrollBar>()
                                            .Single(scrollBar => scrollBar.Orientation == Orientation.Horizontal);

            horizontalScrollBar.IsVisible.ShouldBeTrue();
            lineNumberMargin.Bounds.Width.ShouldBeGreaterThan(0);

            var lineNumberRightPoint = lineNumberMargin.TranslatePoint(
                new Point(lineNumberMargin.Bounds.Width, 0),
                editor);
            var scrollBarLeftPoint = horizontalScrollBar.TranslatePoint(new Point(), editor);

            lineNumberRightPoint.ShouldNotBeNull();
            scrollBarLeftPoint.ShouldNotBeNull();
            scrollBarLeftPoint.Value.X.ShouldBeGreaterThanOrEqualTo(lineNumberRightPoint.Value.X - 0.5);
        });
    }

    [Fact]
    public void GalleryCodeViewer_Uses_Dark_Syntax_Theme_When_AtomUI_Dark_Mode_Is_Enabled()
    {
        var application = Application.Current!;
        var previousDarkMode = application.IsDarkThemeMode();

        try
        {
            application.SetDarkThemeMode(true);
            Dispatcher.UIThread.RunJobs();

            var viewer = new GalleryCodeViewer
            {
                CodeText = "<StackPanel Margin=\"20\" Spacing=\"12\" />",
                Language = "axaml"
            };

            ShowInWindow(viewer, _ =>
            {
                GetCurrentSyntaxTheme(viewer).ShouldBe(ThemeName.DarkPlus);
                GetAxamlTagNameColor(viewer).ShouldBe("#569CD6", StringCompareShould.IgnoreCase);
            });
        }
        finally
        {
            application.SetDarkThemeMode(previousDarkMode);
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void GalleryCodeViewer_Treats_AtomUI_Custom_Dark_ThemeVariant_As_Dark_Syntax_Theme()
    {
        var application = Application.Current!;
        var previousDarkMode = application.IsDarkThemeMode();
        var actualThemeVariantChangedCount = 0;
        var isDarkModeWhenThemeManagerVariantRaised = false;
        EventHandler handler = (_, _) => actualThemeVariantChangedCount++;
        var themeManager = application.GetThemeManager().ShouldNotBeNull();
        using var themeVariantSubscription = themeManager.BindingSource
                                                         .GetObservable(IThemeManager.ThemeVariantProperty)
                                                         .Subscribe(_ =>
                                                         {
                                                             isDarkModeWhenThemeManagerVariantRaised =
                                                                 themeManager.ActivatedTheme?.IsDarkMode == true;
                                                         });

        try
        {
            application.SetDarkThemeMode(false);
            Dispatcher.UIThread.RunJobs();

            application.ActualThemeVariantChanged += handler;
            application.SetDarkThemeMode(true);
            Dispatcher.UIThread.RunJobs();

            actualThemeVariantChangedCount.ShouldBeGreaterThan(0);
            isDarkModeWhenThemeManagerVariantRaised.ShouldBeTrue();
            application.ActualThemeVariant.ShouldNotBe(ThemeVariant.Dark);
            themeManager.ActivatedTheme
                       .ShouldNotBeNull()
                       .Algorithms
                       .ShouldContain(ThemeAlgorithm.Dark);

            var viewer = new GalleryCodeViewer
            {
                CodeText = "<StackPanel Margin=\"20\" Spacing=\"12\" />",
                Language = "axaml"
            };

            ShowInWindow(viewer, _ =>
            {
                viewer.ActualThemeVariant.ShouldNotBe(ThemeVariant.Dark);
                themeManager.ActivatedTheme
                           .ShouldNotBeNull()
                           .IsDarkMode
                           .ShouldBeTrue();
                GetCurrentSyntaxTheme(viewer).ShouldBe(ThemeName.DarkPlus);
            });
        }
        finally
        {
            application.ActualThemeVariantChanged -= handler;
            application.SetDarkThemeMode(previousDarkMode);
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void GalleryCodeViewer_Updates_TextMate_Theme_When_AtomUI_Dark_Mode_Changes()
    {
        var application = Application.Current!;
        var previousDarkMode = application.IsDarkThemeMode();

        try
        {
            application.SetDarkThemeMode(false);
            Dispatcher.UIThread.RunJobs();

            var viewer = new GalleryCodeViewer
            {
                CodeText = "<StackPanel Margin=\"20\" Spacing=\"12\" />",
                Language = "axaml"
            };

            ShowInWindow(viewer, _ =>
            {
                GetCurrentSyntaxTheme(viewer).ShouldBe(ThemeName.LightPlus);

                application.SetDarkThemeMode(true);
                Dispatcher.UIThread.RunJobs();

                GetCurrentSyntaxTheme(viewer).ShouldBe(ThemeName.DarkPlus);
                GetAxamlTagNameColor(viewer).ShouldBe("#569CD6", StringCompareShould.IgnoreCase);

                application.SetDarkThemeMode(false);
                Dispatcher.UIThread.RunJobs();

                GetCurrentSyntaxTheme(viewer).ShouldBe(ThemeName.LightPlus);
            });
        }
        finally
        {
            application.SetDarkThemeMode(previousDarkMode);
            Dispatcher.UIThread.RunJobs();
        }
    }

    [Fact]
    public void GalleryCodeViewer_Line_Number_Text_Keeps_Distance_From_Separator()
    {
        var viewer = new GalleryCodeViewer
        {
            CodeText = string.Join("\n", Enumerable.Range(1, 8).Select(index => $"Line {index}")),
            Language = "text"
        };

        ShowInWindow(viewer, editor =>
        {
            var lineNumberMargin = editor.GetVisualDescendants()
                                         .OfType<LineNumberMargin>()
                                         .Single();
            var separator = editor.TextArea.LeftMargins.Single(DottedLineMargin.IsDottedLineMargin);

            var lineNumberRightPoint = lineNumberMargin.TranslatePoint(
                new Point(lineNumberMargin.Bounds.Width, 0),
                editor);
            var separatorLeftPoint = separator.TranslatePoint(new Point(), editor);

            lineNumberRightPoint.ShouldNotBeNull();
            separatorLeftPoint.ShouldNotBeNull();
            (separatorLeftPoint.Value.X - lineNumberRightPoint.Value.X).ShouldBeGreaterThanOrEqualTo(8);
        });
    }

    [Fact]
    public void GalleryCodeViewer_Uses_AtomUI_ContextMenu_For_Copying_Selected_Code()
    {
        var viewer = new GalleryCodeViewer
        {
            CodeText = "public class Demo { }",
            Language = "csharp"
        };

        ShowInWindow(viewer, editor =>
        {
            var contextMenu = editor.ContextMenu.ShouldBeOfType<AtomUIContextMenu>();
            var copyItem = contextMenu.Items
                                      .OfType<AtomUIMenuItem>()
                                      .Single();

            copyItem.Header.ShouldBe("Copy");
            copyItem.IsEnabled.ShouldBeFalse();

            editor.Select(0, 6);
            Dispatcher.UIThread.RunJobs();

            copyItem.IsEnabled.ShouldBeTrue();
        });
    }

    [Fact]
    public async Task GalleryCodeViewer_ContextMenu_Copy_Writes_Selected_Code_To_Clipboard()
    {
        var viewer = new GalleryCodeViewer
        {
            CodeText = "public class Demo { }",
            Language = "csharp"
        };

        await ShowInWindowAsync(viewer, async editor =>
        {
            editor.Select(0, 6);
            Dispatcher.UIThread.RunJobs();

            var copyItem = editor.ContextMenu
                                 .ShouldBeOfType<AtomUIContextMenu>()
                                 .Items
                                 .OfType<AtomUIMenuItem>()
                                 .Single();

            copyItem.RaiseEvent(new RoutedEventArgs(AtomUIMenuItem.ClickEvent, copyItem));

            await WaitForClipboardTextAsync(editor, "public");
        });
    }

    [Fact]
    public void DrawerContent_Renders_Tab_Header_Per_Snippet()
    {
        var group = new ShowCaseCodeSnippetGroup(
            "Basic",
            new[]
            {
                CreateSnippet("AXAML", "axaml", "<Button Content=\"Primary\" />"),
                CreateSnippet("Code-behind", "csharp", "private void HandleClick() { }"),
                CreateSnippet("ViewModel", "csharp", "public string SelectedValue { get; set; }")
            });

        ShowDrawerContentInWindow(group, content =>
        {
            var tabControl = content.GetVisualDescendants()
                                    .OfType<DesktopTabControl>()
                                    .Single();

            tabControl.Items
                      .OfType<DesktopTabItem>()
                      .Select(static tabItem => tabItem.Header)
                      .ShouldBe(new object?[] { "AXAML", "Code-behind", "ViewModel" });
        });
    }

    [Fact]
    public void DrawerContent_Creates_Viewers_Lazily_Per_Selected_Tab()
    {
        var group = new ShowCaseCodeSnippetGroup(
            "Basic",
            new[]
            {
                CreateSnippet("AXAML", "axaml", "<Button Content=\"Primary\" />"),
                CreateSnippet("Code-behind", "csharp", "private void HandleClick() { }"),
                CreateSnippet("ViewModel", "csharp", "public string SelectedValue { get; set; }")
            });

        ShowDrawerContentInWindow(group, content =>
        {
            var tabControl = content.GetVisualDescendants()
                                    .OfType<DesktopTabControl>()
                                    .Single();
            var tabItems = tabControl.Items.OfType<DesktopTabItem>().ToArray();

            // Only the initially selected tab materializes a viewer.
            tabItems[0].Content.ShouldBeOfType<GalleryCodeViewer>();
            tabItems[1].Content.ShouldBeNull();
            tabItems[2].Content.ShouldBeNull();

            tabControl.SelectedIndex = 2;
            Dispatcher.UIThread.RunJobs();

            // Selecting another tab materializes its viewer; the untouched tab stays empty.
            tabItems[2].Content.ShouldBeOfType<GalleryCodeViewer>();
            tabItems[1].Content.ShouldBeNull();
        });
    }

    [Fact]
    public void DrawerContent_Shows_Placeholder_When_No_Snippets_Available()
    {
        var key = new ShowCaseCodeSnippetKey(
            "AtomUIGallery.ShowCases.General.Button.Views.ButtonShowCase",
            "ExamplesContent",
            0);

        ShowDrawerContentInWindow(null, key, content =>
        {
            content.GetVisualDescendants()
                   .OfType<DesktopTabControl>()
                   .ShouldBeEmpty();

            content.GetVisualDescendants()
                   .OfType<TextBlock>()
                   .Select(static textBlock => textBlock.Text)
                   .ShouldContain(text => text != null && text.Contains("No source snippet found"));
        });
    }

    private static ShowCaseCodeSnippet CreateSnippet(string tabTitle, string language, string text)
    {
        return new ShowCaseCodeSnippet(tabTitle, language, text, SourceFilePath: null, StartLine: 1, EndLine: 1);
    }

    private static void ShowDrawerContentInWindow(ShowCaseCodeSnippetGroup group,
                                                  Action<GalleryShowCaseCodeDrawerContent> verify)
    {
        var key = new ShowCaseCodeSnippetKey("Demo", "ExamplesContent", 0);
        ShowDrawerContentInWindow(group, key, verify);
    }

    private static void ShowDrawerContentInWindow(ShowCaseCodeSnippetGroup? group,
                                                  ShowCaseCodeSnippetKey key,
                                                  Action<GalleryShowCaseCodeDrawerContent> verify)
    {
        var content = new GalleryShowCaseCodeDrawerContent(group, key);
        var window = new Window
        {
            Width = 640,
            Height = 480,
            Content = content
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            content.ApplyTemplate();
            Dispatcher.UIThread.RunJobs();
            verify(content);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
            content.Dispose();
        }
    }

    private static void ShowInWindow(GalleryCodeViewer viewer, Action<TextEditor> verify, Control? content = null)
    {
        var window = new Window
        {
            Width = 640,
            Height = 480,
            Content = content ?? viewer
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            viewer.ApplyTemplate();

            var editor = viewer.GetVisualDescendants()
                               .OfType<TextEditor>()
                               .Single();
            editor.ApplyTemplate();

            Dispatcher.UIThread.RunJobs();
            verify(editor);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
            viewer.Dispose();
        }
    }

    private static async Task ShowInWindowAsync(GalleryCodeViewer viewer,
                                                Func<TextEditor, Task> verify,
                                                Control? content = null)
    {
        var window = new Window
        {
            Width = 640,
            Height = 480,
            Content = content ?? viewer
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            viewer.ApplyTemplate();

            var editor = viewer.GetVisualDescendants()
                               .OfType<TextEditor>()
                               .Single();
            editor.ApplyTemplate();

            Dispatcher.UIThread.RunJobs();
            await verify(editor);
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
            viewer.Dispose();
        }
    }

    private static async Task WaitForClipboardTextAsync(Control control, string expectedText)
    {
        var clipboard = TopLevel.GetTopLevel(control)?.Clipboard;
        clipboard.ShouldNotBeNull();

        string? actualText = null;
        for (var i = 0; i < 20; i++)
        {
            Dispatcher.UIThread.RunJobs();
            using var dataTransfer = await clipboard!.TryGetInProcessDataAsync();
            actualText = dataTransfer is null
                ? null
                : await dataTransfer.TryGetValueAsync(DataFormat.Text);
            if (actualText == expectedText)
            {
                return;
            }

            await Task.Delay(10);
        }

        actualText.ShouldBe(expectedText);
    }

    private static string GetTextMateThemeColor(GalleryCodeViewer viewer, string colorKey)
    {
        var installationField = typeof(GalleryCodeViewer).GetField(
            "_textMateInstallation",
            BindingFlags.Instance | BindingFlags.NonPublic);
        installationField.ShouldNotBeNull();

        var installation = installationField.GetValue(viewer);
        installation.ShouldNotBeNull();

        var arguments = new object?[] { colorKey, null };
        var result = installation.GetType()
                                 .GetMethod("TryGetThemeColor")!
                                 .Invoke(installation, arguments);
        result.ShouldBe(true);

        return arguments[1].ShouldBeOfType<string>();
    }

    private static ThemeName GetCurrentSyntaxTheme(GalleryCodeViewer viewer)
    {
        var syntaxThemeField = typeof(GalleryCodeViewer).GetField(
            "_currentSyntaxTheme",
            BindingFlags.Instance | BindingFlags.NonPublic);
        syntaxThemeField.ShouldNotBeNull();

        return syntaxThemeField.GetValue(viewer).ShouldBeOfType<ThemeName>();
    }

    private static string? GetAxamlTagNameColor(GalleryCodeViewer viewer)
    {
        return GetAxamlTokenColor(viewer, "StackPanel");
    }

    private static string? GetAxamlTokenColor(GalleryCodeViewer viewer, string tokenText)
    {
        const string lineText = "<StackPanel Margin=\"20\" Spacing=\"12\">";

        var registryOptions = new RegistryOptions(GetCurrentSyntaxTheme(viewer));
        var registry = new Registry(registryOptions);
        var grammar = registry.LoadGrammar(registryOptions.GetScopeByExtension(".xml"));
        var theme = registry.GetTheme();
        var tokens = grammar.TokenizeLine(lineText).Tokens;

        var token = tokens.Single(token => lineText.Substring(token.StartIndex, token.Length) == tokenText);
        var foregroundColorId = 0;
        foreach (var rule in theme.Match(token.Scopes))
        {
            if (foregroundColorId == 0 && rule.foreground > 0)
            {
                foregroundColorId = rule.foreground;
            }
        }

        return foregroundColorId > 0 ? theme.GetColor(foregroundColorId) : null;
    }
}
