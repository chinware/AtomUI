using AtomUI.Theme;
using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;

namespace AtomUI.Controls;

public abstract class AbstractColorPickerView : TemplatedControl,
                                                IMotionAwareControl,
                                                IControlSharedTokenResourcesHost
{
    #region 公共属性定义
    public static readonly StyledProperty<Color> ValueProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, Color>(nameof(Value),
            Colors.White,
            defaultBindingMode: BindingMode.TwoWay,
            coerce: CoerceColor);
    
    public static readonly StyledProperty<HsvColor> HsvValueProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, HsvColor>(
            nameof(HsvValue),
            Colors.White.ToHsv(),
            defaultBindingMode: BindingMode.TwoWay,
            coerce: CoerceHsvColor);
    
    public static readonly StyledProperty<ColorSpectrumComponents> ColorSpectrumComponentsProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, ColorSpectrumComponents>(
            nameof(ColorSpectrumComponents),
            ColorSpectrumComponents.HueSaturation);
    
    public static readonly StyledProperty<ColorFormat> FormatProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, ColorFormat>(nameof(Format), ColorFormat.Hex);
    
    public static readonly StyledProperty<bool> IsFormatEnabledProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, bool>(nameof(IsFormatEnabled));
    
    public static readonly StyledProperty<bool> IsAlphaEnabledProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, bool>(
            nameof(IsAlphaEnabled),
            true);
    
    public static readonly StyledProperty<bool> IsAlphaVisibleProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, bool>(
            nameof(IsAlphaVisible),
            true);
    
    public static readonly StyledProperty<bool> IsColorPaletteVisibleProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, bool>(
            nameof(IsColorPaletteVisible),
            true);
    
    public static readonly StyledProperty<bool> IsMotionEnabledProperty =
        MotionAwareControlProperty.IsMotionEnabledProperty.AddOwner<AbstractColorPickerView>();
    
    public static readonly StyledProperty<int> MaxHueProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, int>(
            nameof(MaxHue),
            359);
    
    public static readonly StyledProperty<int> MaxSaturationProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, int>(
            nameof(MaxSaturation),
            100);
    
    public static readonly StyledProperty<int> MaxValueProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, int>(
            nameof(MaxValue),
            100);
    
    public static readonly StyledProperty<int> MinHueProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, int>(
            nameof(MinHue),
            0);

    public static readonly StyledProperty<int> MinSaturationProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, int>(
            nameof(MinSaturation),
            0);
    
    public static readonly StyledProperty<int> MinValueProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, int>(
            nameof(MinValue),
            0);
    
    public static readonly StyledProperty<List<ColorPickerPalette>?> PaletteGroupProperty =
        AvaloniaProperty.Register<AbstractColorPickerView, List<ColorPickerPalette>?>(
            nameof(PaletteGroup));
    
    public Color? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public HsvColor HsvValue
    {
        get => GetValue(HsvValueProperty);
        set => SetValue(HsvValueProperty, value);
    }
    
    public ColorSpectrumComponents ColorSpectrumComponents
    {
        get => GetValue(ColorSpectrumComponentsProperty);
        set => SetValue(ColorSpectrumComponentsProperty, value);
    }
    
    public ColorFormat Format
    {
        get => GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }
    
    public bool IsFormatEnabled
    {
        get => GetValue(IsFormatEnabledProperty);
        set => SetValue(IsFormatEnabledProperty, value);
    }
    
    public bool IsMotionEnabled
    {
        get => GetValue(IsMotionEnabledProperty);
        set => SetValue(IsMotionEnabledProperty, value);
    }
    
    public bool IsAlphaEnabled
    {
        get => GetValue(IsAlphaEnabledProperty);
        set => SetValue(IsAlphaEnabledProperty, value);
    }
    
    public bool IsAlphaVisible
    {
        get => GetValue(IsAlphaVisibleProperty);
        set => SetValue(IsAlphaVisibleProperty, value);
    }
    
    public bool IsColorPaletteVisible
    {
        get => GetValue(IsColorPaletteVisibleProperty);
        set => SetValue(IsColorPaletteVisibleProperty, value);
    }
    
    public int MaxHue
    {
        get => GetValue(MaxHueProperty);
        set => SetValue(MaxHueProperty, value);
    }

    public int MaxSaturation
    {
        get => GetValue(MaxSaturationProperty);
        set => SetValue(MaxSaturationProperty, value);
    }
    
    public int MaxValue
    {
        get => GetValue(MaxValueProperty);
        set => SetValue(MaxValueProperty, value);
    }

    public int MinHue
    {
        get => GetValue(MinHueProperty);
        set => SetValue(MinHueProperty, value);
    }

    public int MinSaturation
    {
        get => GetValue(MinSaturationProperty);
        set => SetValue(MinSaturationProperty, value);
    }
    
    public int MinValue
    {
        get => GetValue(MinValueProperty);
        set => SetValue(MinValueProperty, value);
    }
    
    public List<ColorPickerPalette>? PaletteGroup
    {
        get => GetValue(PaletteGroupProperty);
        set => SetValue(PaletteGroupProperty, value);
    }
    #endregion
    
    #region 内部属性定义
    Control IMotionAwareControl.PropertyBindTarget => this;
    Control IControlSharedTokenResourcesHost.HostControl => this;
    string IControlSharedTokenResourcesHost.TokenId => ColorPickerToken.ID;
    #endregion

    public AbstractColorPickerView()
    {
        this.RegisterResources();
    }
    
    private static Color CoerceColor(AvaloniaObject instance, Color value)
    {
        if (instance is AbstractColorPickerView colorView)
        {
            return colorView.NotifyCoerceColor(value);
        }

        return value;
    }
    
    private static HsvColor CoerceHsvColor(AvaloniaObject instance, HsvColor value)
    {
        if (instance is AbstractColorPickerView colorView)
        {
            return colorView.NotifyCoerceHsvColor(value);
        }

        return value;
    }
    
    protected virtual Color NotifyCoerceColor(Color value)
    {
        if (IsAlphaEnabled == false)
        {
            return new Color(255, value.R, value.G, value.B);
        }

        return value;
    }
    
    protected virtual HsvColor NotifyCoerceHsvColor(HsvColor value)
    {
        if (IsAlphaEnabled == false)
        {
            return new HsvColor(1.0, value.H, value.S, value.V);
        }

        return value;
    }
}