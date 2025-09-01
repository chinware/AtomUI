using AtomUI.Controls.Themes;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

public class GradientColorPickerView : AbstractColorPickerView
{
    #region 公共属性定义
    public static readonly StyledProperty<GradientBrush?> DefaultValueProperty =
        AvaloniaProperty.Register<GradientColorPickerView, GradientBrush?>(nameof(DefaultValue));
    
    public static readonly StyledProperty<GradientBrush?> ValueProperty =
        AvaloniaProperty.Register<GradientColorPickerView, GradientBrush?>(nameof(Value));
    
    public GradientBrush? DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set => SetValue(DefaultValueProperty, value);
    }
    
    public GradientBrush? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    #endregion

    private GradientColorSlider? _gradientColorSlider;
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (IgnorePropertyChanged)
        {
            base.OnPropertyChanged(change);
            return;
        }
        // if (change.Property == ValueProperty)
        // {
        //     IgnorePropertyChanged = true;
        //
        //     SetCurrentValue(HsvValueProperty, Value.ToHsv());
        //
        //     NotifyColorChanged(new ColorChangedEventArgs(
        //         change.GetOldValue<Color>(),
        //         change.GetNewValue<Color>()));
        //
        //     IgnorePropertyChanged = false;
        // }
        if (change.Property == DefaultValueProperty)
        {
            Value ??= DefaultValue;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _gradientColorSlider = e.NameScope.Find<GradientColorSlider>(ColorPickerViewThemeConstants.GradientColorSliderPart);
        if (_gradientColorSlider != null)
        {
            BindUtils.RelayBind(this, ValueProperty, _gradientColorSlider, GradientColorSlider.GradientValueProperty);
        }

        Value ??= DefaultValue;
    }
}