using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Controls;

public class GradientColorPicker : AbstractColorPicker
{
    #region 公共属性定义
    public static readonly StyledProperty<List<GradientStop>?> ValueProperty =
        AvaloniaProperty.Register<AbstractColorPicker, List<GradientStop>?>(nameof(Value));
    
    public static readonly AttachedProperty<Func<List<GradientStop>, ColorFormat, string>?> ColorTextFormatterProperty =
        AvaloniaProperty.RegisterAttached<ColorPicker, Control, Func<List<GradientStop>, ColorFormat, string>?>("ColorTextFormatter");
    
    public List<GradientStop>? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public static Func<List<GradientStop>, ColorFormat, string>? GetColorTextFormatter(GradientColorPicker colorPicker)
    {
        return colorPicker.GetValue(ColorTextFormatterProperty);
    }

    public static void SetPresetColor(GradientColorPicker colorPicker, Func<List<GradientStop>, ColorFormat, string> formatter)
    {
        colorPicker.SetValue(ColorTextFormatterProperty, formatter);
    }
    #endregion
    
    protected override void GenerateValueText()
    {
    }
    
    protected override void GenerateColorBlockBackground()
    {
    }

    protected override Flyout CreatePickerFlyout()
    {
        return default!;
    }
}