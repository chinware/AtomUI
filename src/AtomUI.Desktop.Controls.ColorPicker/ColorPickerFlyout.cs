using System.Diagnostics;
using System.Reactive.Disposables;
using AtomUI.Data;
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
    private CompositeDisposable? _presenterBindingDisposables;
    
    protected override Control CreatePresenter()
    {
        var flyoutPresenter = base.CreatePresenter() as FlyoutPresenter;
        Debug.Assert(flyoutPresenter != null);
        _presenterBindingDisposables?.Dispose();
        Presenter                    = new ColorPickerView();
        _presenterBindingDisposables = new  CompositeDisposable(8);
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, Presenter, ColorPickerView.IsMotionEnabledProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, ValueProperty, Presenter, ColorPickerView.ValueProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, FormatProperty, Presenter, ColorPickerView.FormatProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsClearEnabledProperty, Presenter, ColorPickerView.IsClearEnabledProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsAlphaEnabledProperty, Presenter, ColorPickerView.IsAlphaEnabledProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsFormatEnabledProperty, Presenter, ColorPickerView.IsFormatEnabledProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, IsPaletteGroupEnabledProperty, Presenter, ColorPickerView.IsPaletteGroupEnabledProperty));
        _presenterBindingDisposables.Add(BindUtils.RelayBind(this, PaletteGroupProperty, Presenter, ColorPickerView.PaletteGroupProperty));
        flyoutPresenter.Content = Presenter;
        return flyoutPresenter;
    }
}