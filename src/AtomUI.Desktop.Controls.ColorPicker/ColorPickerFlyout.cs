using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class ColorPickerFlyout : AbstractColorPickerFlyout
{
    public static readonly StyledProperty<Color> ValueProperty =
        ColorPickerView.ValueProperty.AddOwner<ColorPickerFlyout>();
    
    public Color Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    internal ColorPickerView? Presenter { get; set; }
    
    protected override Control CreatePresenter()
    {
        var flyoutPresenter = base.CreatePresenter() as FlyoutPresenter;
        Debug.Assert(flyoutPresenter != null);
        Presenter                    = new ColorPickerView();
        
        Presenter[!ColorPickerView.IsMotionEnabledProperty]       = this[!IsMotionEnabledProperty];
        Presenter[!ColorPickerView.ValueProperty]                 = this[!ValueProperty];
        Presenter[!ColorPickerView.FormatProperty]                = this[!FormatProperty];
        Presenter[!ColorPickerView.IsClearEnabledProperty]        = this[!IsClearEnabledProperty];
        Presenter[!ColorPickerView.IsAlphaEnabledProperty]        = this[!IsAlphaEnabledProperty];
        Presenter[!ColorPickerView.IsFormatEnabledProperty]       = this[!IsFormatEnabledProperty];
        Presenter[!ColorPickerView.IsPaletteGroupEnabledProperty] = this[!IsPaletteGroupEnabledProperty];
        Presenter[!ColorPickerView.PaletteGroupProperty]          = this[!PaletteGroupProperty];
        
        flyoutPresenter.Content = Presenter;
        return flyoutPresenter;
    }
}