using AtomUI.Controls.Themes;
using AtomUI.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.VisualTree;

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
    
    #region 内部属性定义
    
    internal static readonly DirectProperty<GradientColorPickerView, int?> ActivatedStopIndexProperty =
        AvaloniaProperty.RegisterDirect<GradientColorPickerView, int?>(
            nameof(ActivatedStopIndex),
            o => o.ActivatedStopIndex,
            (o, v) => o.ActivatedStopIndex = v);
    
    private int? _activatedStopIndex;

    internal int? ActivatedStopIndex
    {
        get => _activatedStopIndex;
        set => SetAndRaise(ActivatedStopIndexProperty, ref _activatedStopIndex, value);
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
        if (change.Property == ValueProperty)
        {
            NotifyGradientValueChanged(new GradientColorChangedEventArgs(
                change.GetOldValue<LinearGradientBrush>(),
                change.GetNewValue<LinearGradientBrush>()));
        }

        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == DefaultValueProperty)
            {
                if (Value == null)
                {
                    SetCurrentValue(ValueProperty, DefaultValue);
                }
            }
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
            BindUtils.RelayBind(this, ValueProperty, _gradientColorSlider, GradientColorSlider.GradientValueProperty, BindingMode.TwoWay);
            BindUtils.RelayBind(this, ActivatedStopIndexProperty, _gradientColorSlider, GradientColorSlider.ActivatedStopIndexProperty, BindingMode.TwoWay);
        }

        if (Value == null)
        {
            SetCurrentValue(ValueProperty, DefaultValue);
        }
    }
    
    protected override void NotifyColorClearRequest()
    {
        SetCurrentValue(ValueProperty, new LinearGradientBrush()
        {
            StartPoint = new RelativePoint(0, 0.5, RelativeUnit.Relative),
            EndPoint   = new RelativePoint(1, 0.5, RelativeUnit.Relative),
            GradientStops = new GradientStops()
            {
                new GradientStop(Color.FromArgb(0,0, 0, 0), 0.0)
            }
        });
        HsvValue = new HsvColor(0.0, 0, 0, 0);
        InvokeColorValueClearedEvent();
    }
    
    protected override void NotifyPaletteColorSelected(Color color)
    {
        SetCurrentValue(HsvValueProperty, color.ToHsv());
    }
}