using System;
using System.Linq;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUILineEdit = AtomUI.Desktop.Controls.LineEdit;
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

    private static void ShowInWindow(Control content, Action assertion)
    {
        var window = new AvaloniaWindow
        {
            Width   = 240,
            Height  = 120,
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
