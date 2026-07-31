using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Shouldly;
using Xunit;
using AtomUINumericUpDown = AtomUI.Desktop.Controls.NumericUpDown;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.NumericUpDown;

public class NumericUpDownStringModeTests
{
    static NumericUpDownStringModeTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void String_Mode_Preserves_Invalid_Raw_Text_And_Clears_Value()
    {
        var numericUpDown = new AtomUINumericUpDown
        {
            IsStringMode = true
        };

        numericUpDown.StringValue = "not-a-number";

        numericUpDown.StringValue.ShouldBe("not-a-number");
        numericUpDown.Text.ShouldBe("not-a-number");
        numericUpDown.Value.ShouldBeNull();
    }

    [Fact]
    public void String_Mode_Restores_User_TextConverter_When_Disabled()
    {
        var converter = new IdentityConverter();
        var numericUpDown = new AtomUINumericUpDown
        {
            TextConverter = converter
        };

        numericUpDown.IsStringMode = true;

        numericUpDown.TextConverter.ShouldNotBeSameAs(converter);

        numericUpDown.IsStringMode = false;

        numericUpDown.TextConverter.ShouldBeSameAs(converter);
    }

    [Fact]
    public void Template_Relays_IsCustomFontSize_To_Inner_TextBox()
    {
        var numericUpDown = new AtomUINumericUpDown
        {
            Width            = 160,
            IsCustomFontSize = true
        };

        ShowInWindow(numericUpDown, () =>
        {
            var textBox = numericUpDown.GetVisualDescendants()
                                       .OfType<TextBox>()
                                       .Single(item => item.Name == "PART_TextBox");

            textBox.IsCustomFontSize.ShouldBeTrue();
        });
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

    private sealed class IdentityConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
