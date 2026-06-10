using Avalonia.Controls;
using Avalonia.Controls.Converters;
using ColorPicker = AtomUI.Desktop.Controls.ColorPicker;

namespace AtomUIGallery.ShowCases.ColorPicker;

public partial class ColorPickerShowCase : GalleryReactiveUserControl<ColorPickerViewModel>
{
    public const string LanguageId = nameof(ColorPickerShowCase);

    public ColorPickerShowCase()
    {
        this.WhenActivated(disposables =>
        {
            AtomUIColorPicker.SetColorTextFormatter(CustomRenderText, (color, format) =>
            {
                var colorText = ColorToHexConverter.ToHexString(color, AlphaComponentPosition.Leading, false, true);
                return $"Custom Text ({colorText})";
            });
        });
        InitializeComponent();
    }
}
