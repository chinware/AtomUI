using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class GradientColorPickerFlyout : AbstractColorPickerFlyout
{
    public static readonly StyledProperty<LinearGradientBrush?> ValueProperty =
        GradientColorPickerView.ValueProperty.AddOwner<GradientColorPickerFlyout>();
    
    public LinearGradientBrush? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    internal GradientColorPickerView? Presenter { get; set; }
    
    protected override Control CreatePresenter()
    {
        var flyoutPresenter = base.CreatePresenter() as FlyoutPresenter;
        Debug.Assert(flyoutPresenter != null);
        Presenter                    = new GradientColorPickerView();

        Presenter[!GradientColorPickerView.IsMotionEnabledProperty]       = this[!IsMotionEnabledProperty];
        Presenter[!GradientColorPickerView.ValueProperty]                 = this[!ValueProperty];
        Presenter[!GradientColorPickerView.FormatProperty]                = this[!FormatProperty];
        Presenter[!GradientColorPickerView.IsClearEnabledProperty]        = this[!IsClearEnabledProperty];
        Presenter[!GradientColorPickerView.IsAlphaEnabledProperty]        = this[!IsAlphaEnabledProperty];
        Presenter[!GradientColorPickerView.IsFormatEnabledProperty]       = this[!IsFormatEnabledProperty];
        Presenter[!GradientColorPickerView.IsPaletteGroupEnabledProperty] = this[!IsPaletteGroupEnabledProperty];
        Presenter[!GradientColorPickerView.PaletteGroupProperty]          = this[!PaletteGroupProperty];
        
        flyoutPresenter.Content = Presenter;
        NotifyPresenterCreated(flyoutPresenter);
        return flyoutPresenter;
    }
}
