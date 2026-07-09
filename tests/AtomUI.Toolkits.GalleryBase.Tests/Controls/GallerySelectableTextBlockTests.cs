using AtomUI.Toolkits.GalleryBase.Controls;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomUIContextMenu = AtomUI.Desktop.Controls.ContextMenu;
using AtomUIMenuItem = AtomUI.Desktop.Controls.MenuItem;

namespace AtomUI.Toolkits.GalleryBase.Tests.Controls;

public class GallerySelectableTextBlockTests
{
    static GallerySelectableTextBlockTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Copy_Menu_Is_Disabled_Until_Text_Is_Selected()
    {
        var textBlock = new GallerySelectableTextBlock
        {
            Text = "Splash.Progress"
        };

        ShowInWindow(textBlock, () =>
        {
            var contextMenu = textBlock.ContextMenu.ShouldBeOfType<AtomUIContextMenu>();
            var copyItem = contextMenu.Items
                                      .OfType<AtomUIMenuItem>()
                                      .Single();

            copyItem.Header.ShouldBe("Copy");
            copyItem.IsEnabled.ShouldBeFalse();

            textBlock.SelectionStart = 0;
            textBlock.SelectionEnd   = 6;
            Dispatcher.UIThread.RunJobs();

            copyItem.IsEnabled.ShouldBeTrue();
        });
    }

    [Fact]
    public async Task Copy_Menu_Writes_Selected_Text_To_Clipboard()
    {
        var textBlock = new GallerySelectableTextBlock
        {
            Text = "Splash.Progress"
        };

        await ShowInWindowAsync(textBlock, async () =>
        {
            textBlock.SelectionStart = 7;
            textBlock.SelectionEnd   = 15;
            Dispatcher.UIThread.RunJobs();

            var copyItem = textBlock.ContextMenu
                                    .ShouldBeOfType<AtomUIContextMenu>()
                                    .Items
                                    .OfType<AtomUIMenuItem>()
                                    .Single();

            copyItem.RaiseEvent(new RoutedEventArgs(AtomUIMenuItem.ClickEvent, copyItem));

            await WaitForClipboardTextAsync(textBlock, "Progress");
        });
    }

    [Fact]
    public void Code_Font_Selection_Background_Uses_Gallery_Line_Height()
    {
        const string text = "Splash.LogoTemplate";
        var textBlock = new GallerySelectableTextBlock
        {
            Text           = text,
            FontFamily     = FontFamily.Parse("Consolas"),
            SelectionStart = 0,
            SelectionEnd   = text.Length
        };

        ShowInWindow(textBlock, () =>
        {
            textBlock.LineHeight.ShouldBeGreaterThan(textBlock.FontSize);

            var selectionBounds = textBlock.TextLayout
                                            .HitTestTextRange(0, text.Length)
                                            .Single();

            selectionBounds.Height.ShouldBe(textBlock.LineHeight, 0.5);
        });
    }

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new Window
        {
            Width   = 320,
            Height  = 160,
            Content = content
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        content.ApplyTemplate();
        Dispatcher.UIThread.RunJobs();

        try
        {
            assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
        }
    }

    private static async Task ShowInWindowAsync(Control content, Func<Task> assertion)
    {
        var window = new Window
        {
            Width   = 320,
            Height  = 160,
            Content = content
        };

        window.Show();
        Dispatcher.UIThread.RunJobs();
        content.ApplyTemplate();
        Dispatcher.UIThread.RunJobs();

        try
        {
            await assertion();
        }
        finally
        {
            window.Close();
            Dispatcher.UIThread.RunJobs();
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
}
