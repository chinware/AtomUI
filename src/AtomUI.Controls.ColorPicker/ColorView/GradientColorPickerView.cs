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
    public static readonly StyledProperty<LinearGradientBrush?> DefaultValueProperty =
        AvaloniaProperty.Register<GradientColorPickerView, LinearGradientBrush?>(nameof(DefaultValue));
    
    public static readonly StyledProperty<LinearGradientBrush?> ValueProperty =
        AvaloniaProperty.Register<GradientColorPickerView, LinearGradientBrush?>(nameof(Value));
    
    public LinearGradientBrush? DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set => SetValue(DefaultValueProperty, value);
    }
    
    public LinearGradientBrush? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    #endregion
    
    #region 公共事件定义
    public event EventHandler<GradientColorChangedEventArgs>? GradientValueChanged;
    #endregion

    private GradientColorSlider? _gradientColorSlider;
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (IgnorePropertyChanged)
        {
            base.OnPropertyChanged(change);
            return;
        }
        if (change.Property == ValueProperty)
        {
            NotifyGradientValueChanged(new GradientColorChangedEventArgs(
                change.GetOldValue<LinearGradientBrush>(),
                change.GetNewValue<LinearGradientBrush>()));
        }
        if (change.Property == DefaultValueProperty)
        {
            Value ??= DefaultValue;
        }
    }
    
    protected virtual void NotifyGradientValueChanged(GradientColorChangedEventArgs e)
    {
        GradientValueChanged?.Invoke(this, e);
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