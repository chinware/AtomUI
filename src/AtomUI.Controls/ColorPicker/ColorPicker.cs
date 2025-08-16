using AtomUI.Controls.ColorPickerLang;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace AtomUI.Controls;

public class ColorPicker : AbstractColorPicker
{
    #region 公共属性定义
    public static readonly StyledProperty<Color?> DefaultValueProperty =
        AvaloniaProperty.Register<ColorPicker, Color?>(nameof(DefaultValue));
    
    public static readonly StyledProperty<Color?> ValueProperty =
        AvaloniaProperty.Register<ColorPicker, Color?>(nameof(Value));
    
    public static readonly AttachedProperty<Func<Color, ColorFormat, string>?> ColorTextFormatterProperty =
        AvaloniaProperty.RegisterAttached<ColorPicker, Control, Func<Color, ColorFormat, string>?>("ColorTextFormatter");
    
    public Color? DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set => SetValue(DefaultValueProperty, value);
    }
    
    public Color? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public static Func<Color, ColorFormat, string>? GetColorTextFormatter(ColorPicker colorPicker)
    {
        return colorPicker.GetValue(ColorTextFormatterProperty);
    }

    public static void SetPresetColor(ColorPicker colorPicker, Func<Color, ColorFormat, string> formatter)
    {
        colorPicker.SetValue(ColorTextFormatterProperty, formatter);
    }
    #endregion

    #region 内部属性定义

    internal static readonly DirectProperty<ColorPicker, string?> ColorTextProperty =
        AvaloniaProperty.RegisterDirect<ColorPicker, string?>(
            nameof(ColorText),
            o => o.ColorText,
            (o, v) => o.ColorText = v);
    
    private string? _colorText;

    internal string? ColorText
    {
        get => _colorText;
        set => SetAndRaise(ColorTextProperty, ref _colorText, value);
    }

    #endregion
    
    static ColorPicker()
    {
        AffectsMeasure<AbstractColorPicker>(ColorTextFormatterProperty);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty)
        {
            GenerateValueText();
        }
    }
    
    protected override void GenerateValueText()
    {
        if (IsShowText)
        {
            if (Value != null)
            {
                var customFormatter = GetColorTextFormatter(this);
                if (customFormatter != null)
                {
                    SetValue(ColorTextProperty, customFormatter(Value.Value, Format), BindingPriority.Template);
                }
                else
                {
                    if (Format == ColorFormat.Hex)
                    {
                        SetValue(ColorTextProperty, Value.ToString(), BindingPriority.Template);
                    }
                    else if (Format == ColorFormat.Rgba)
                    {
                        SetValue(ColorTextProperty, $"rgb({(int)Value.Value.R},{(int)Value.Value.G},{(int)Value.Value.B})", BindingPriority.Template);
                    }
                    else if (Format == ColorFormat.Hsva)
                    {
                        SetValue(ColorTextProperty, Value.Value.ToHsv().ToString(), BindingPriority.Template);
                    }
                }
            }
            else
            {
                this.AddResourceBindingDisposable(LanguageResourceBinder.CreateBinding(this, ColorTextProperty, ColorPickerLangResourceKey.EmptyColorText));
            }
        }
    }
}
