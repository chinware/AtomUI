using AtomUIGallery.ShowCases.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using ReactiveUI;
using ReactiveUI.Avalonia;
using ColorPicker = AtomUI.Desktop.Controls.ColorPicker;

namespace AtomUIGallery.ShowCases.Views;

public partial class ColorPickerShowCase: ReactiveUserControl<ColorPickerViewModel>
{
    public ColorPickerShowCase()
    {
        this.WhenActivated(disposables =>
        {
            ColorPicker.SetColorTextFormatter(CustomRenderText, (color, format) =>
            {
                var colorText = ColorToHexConverter.ToHexString(color, AlphaComponentPosition.Leading, false, true);
                return $"Custom Text ({colorText})";
            });
        });
        InitializeComponent();
    }
}