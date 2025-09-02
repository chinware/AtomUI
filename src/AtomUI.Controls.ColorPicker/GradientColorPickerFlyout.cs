using System.Reactive.Disposables;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Controls;

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
    private CompositeDisposable? _presenterBindingDisposables;
    
    protected override Control CreatePresenter()
    {
        _presenterBindingDisposables?.Dispose();
        Presenter                    = new GradientColorPickerView();
        _presenterBindingDisposables = new  CompositeDisposable(6);
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, Presenter, GradientColorPickerView.IsMotionEnabledProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, ValueProperty, Presenter, GradientColorPickerView.ValueProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, FormatProperty, Presenter, GradientColorPickerView.FormatProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsAlphaEnabledProperty, Presenter, GradientColorPickerView.IsAlphaEnabledProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsFormatEnabledProperty, Presenter, GradientColorPickerView.IsFormatEnabledProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, PaletteGroupProperty, Presenter, GradientColorPickerView.PaletteGroupProperty));
        
        var flyoutPresenter = new FlyoutPresenter
        {
            Content = Presenter
        };
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsShowArrowEffectiveProperty, flyoutPresenter, IsShowArrowProperty));
        
        CalculateShowArrowEffective();
        SetupArrowPosition(Popup, flyoutPresenter);
        return flyoutPresenter;
    }
}