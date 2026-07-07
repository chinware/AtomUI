using System;
using System.Collections.Generic;
using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;

namespace AtomUIGallery.ShowCases.ColorPicker;

public partial class ColorPickerShowCase : GalleryReactiveUserControl<ColorPickerViewModel>
{
    public const string LanguageId = nameof(ColorPickerShowCase);

    public ColorPickerShowCase()
    {
        InitializeComponent();
    }

    private void HandleCustomRenderTextAttached(object? sender, VisualTreeAttachmentEventArgs args)
    {
        if (sender is AtomUI.Desktop.Controls.ColorPicker colorPicker)
        {
            AtomUIColorPicker.SetColorTextFormatter(colorPicker, (color, _) =>
            {
                var colorText = ColorToHexConverter.ToHexString(color, AlphaComponentPosition.Leading, false, true);
                return $"Custom Text ({colorText})";
            });
        }
    }
}
