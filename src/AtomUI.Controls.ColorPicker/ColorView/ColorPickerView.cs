using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

public class ColorPickerView : AbstractColorPickerView
{
    #region 公共属性定义
    public static readonly StyledProperty<Color> ValueProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, Color>(nameof(Value),
            Colors.White,
            defaultBindingMode: BindingMode.TwoWay,
            coerce: CoerceColor);
    
    public Color Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    #endregion
    
    #region 公共事件定义
    public event EventHandler<ColorChangedEventArgs>? ValueChanged;
    #endregion

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (IgnorePropertyChanged)
        {
            base.OnPropertyChanged(change);
            return;
        }
        if (change.Property == ValueProperty)
        {
            IgnorePropertyChanged = true;

            SetCurrentValue(HsvValueProperty, Value.ToHsv());

            NotifyColorChanged(new ColorChangedEventArgs(
                change.GetOldValue<Color>(),
                change.GetNewValue<Color>()));

            IgnorePropertyChanged = false;
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == HsvValueProperty)
            {
                NotifyColorChanged(new ColorChangedEventArgs(
                    change.GetOldValue<HsvColor>().ToRgb(),
                    change.GetNewValue<HsvColor>().ToRgb()));
            }
        }
    }
    
    protected virtual void NotifyColorChanged(ColorChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }

    protected override void NotifyPaletteColorSelected(Color color)
    {
        SetCurrentValue(ValueProperty, color);
    }
}