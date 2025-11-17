using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class GradientColorSliderThumb : ColorSliderThumb
{
    public const int ActivatedZIndex = 1000;
    public const int NormalZIndex = 1;
    
    public static readonly StyledProperty<double> ValueProperty = 
        AvaloniaProperty.Register<GradientColorSliderThumb, double>(nameof(Value), 0.0);
    
    public static readonly StyledProperty<Color> ColorProperty = 
        AvaloniaProperty.Register<GradientColorSliderThumb, Color>(nameof(Color));
    
    public static readonly StyledProperty<bool> IsActivatedProperty = 
        AvaloniaProperty.Register<GradientColorSliderThumb, bool>(nameof(IsActivated));
    
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }
    
    public Color Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    
    public bool IsActivated
    {
        get => GetValue(IsActivatedProperty);
        set => SetValue(IsActivatedProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == IsActivatedProperty)
        {
            UpdatePseudoClasses();
        }
        else if (change.Property == ColorProperty)
        {
            SetCurrentValue(ColorValueBrushProperty, new SolidColorBrush(Color));
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        UpdatePseudoClasses();
        SetCurrentValue(ColorValueBrushProperty, new SolidColorBrush(Color));
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(StdPseudoClass.Active, IsActivated);
    }
}