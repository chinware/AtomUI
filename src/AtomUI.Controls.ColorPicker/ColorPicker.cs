using System.Reactive.Disposables;
using AtomUI.Controls.ColorPickerLang;
using AtomUI.Controls.Themes;
using AtomUI.Data;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.VisualTree;

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
    
    public static readonly StyledProperty<ColorPickerValueSyncMode> ValueSyncStrategyProperty =
        AvaloniaProperty.Register<ColorPicker, ColorPickerValueSyncMode>(nameof(ValueSyncStrategy), ColorPickerValueSyncMode.Immediate);
    
    public Color? DefaultValue
    {
        get => GetValue(DefaultValueProperty);
        set => SetValue(DefaultValueProperty, value);
    }
    
    public Color? Value
    {
        get => GetValue(ValueProperty);
        private set => SetValue(ValueProperty, value);
    }
    
    public static Func<Color, ColorFormat, string>? GetColorTextFormatter(ColorPicker colorPicker)
    {
        return colorPicker.GetValue(ColorTextFormatterProperty);
    }

    public static void SetColorTextFormatter(ColorPicker colorPicker, Func<Color, ColorFormat, string> formatter)
    {
        colorPicker.SetValue(ColorTextFormatterProperty, formatter);
    }
    
    public ColorPickerValueSyncMode ValueSyncStrategy
    {
        get => GetValue(ValueSyncStrategyProperty);
        set => SetValue(ValueSyncStrategyProperty, value);
    }
    #endregion
    
    #region 公共事件定义
    public event EventHandler<ColorChangedEventArgs>? ValueChanged;
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
    
    private ColorPickerView? _presenter;
    private CompositeDisposable? _flyoutBindingDisposables;
    private Color? _latestSyncValue;
    private ColorBlock? _colorIndicator;
    private IDisposable? _emptyTextBindingDisposable;
    
    static ColorPicker()
    {
        AffectsMeasure<ColorPicker>(ColorTextFormatterProperty);
    }
    
    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == ValueProperty)
        {
            GenerateValueText();
            GenerateColorBlockBackground();
            NotifyValueChanged(new ColorChangedEventArgs(change.GetOldValue<Color?>(), change.GetNewValue<Color?>()));
        }

        if (change.Property == ColorTextFormatterProperty)
        {
            GenerateValueText();
        }
        
        if (this.IsAttachedToVisualTree())
        {
            if (change.Property == DefaultValueProperty)
            {
                Value ??= DefaultValue;
            }
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
                    SetCurrentValue(ColorTextProperty, customFormatter(Value.Value, Format));
                }
                else
                {
                    SetCurrentValue(ColorTextProperty, FormatColor(Value.Value, Format));
                }
            }
            else
            {
                SetCurrentValue(ColorTextProperty, EmptyColorText);
                _emptyTextBindingDisposable?.Dispose();
                _emptyTextBindingDisposable = BindUtils.RelayBind(this, EmptyColorTextProperty, this, ColorTextProperty);
            }
        }
    }

    protected override void GenerateColorBlockBackground()
    {
        if (_colorIndicator != null)
        {
            _colorIndicator.SetCurrentValue(ColorBlock.IsEmptyColorModeProperty, false);
        }
        if (Value == null)
        {
            SetCurrentValue(ColorBlockBackgroundProperty, new SolidColorBrush(Colors.Transparent));
        }
        else
        {
            SetCurrentValue(ColorBlockBackgroundProperty, new SolidColorBrush(Value.Value));
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Value ??= DefaultValue;
        _colorIndicator =  e.NameScope.Find<ColorBlock>(ColorPickerThemeConstants.ColorIndicatorPart);
    }
    
    protected override Flyout CreatePickerFlyout()
    {
        var flyout = new ColorPickerFlyout();
        flyout.IsDetectMouseClickEnabled = false;
        _flyoutBindingDisposables?.Dispose();
        _flyoutBindingDisposables = new CompositeDisposable(7);
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsMotionEnabledProperty, flyout, ColorPickerFlyout.IsMotionEnabledProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsClearEnabledProperty, flyout, ColorPickerFlyout.IsClearEnabledProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, FormatProperty, flyout, ColorPickerFlyout.FormatProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsAlphaEnabledProperty, flyout, ColorPickerFlyout.IsAlphaEnabledProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, IsFormatEnabledProperty, flyout, ColorPickerFlyout.IsFormatEnabledProperty));
        _flyoutBindingDisposables.Add(BindUtils.RelayBind(this, PaletteGroupProperty, flyout, ColorPickerFlyout.PaletteGroupProperty));
        
        return flyout;
    }
    
    protected override void NotifyFlyoutPresenterCreated(Control control)
    {
        if (control is FlyoutPresenter flyoutPresenter && flyoutPresenter.Content is ColorPickerView presenter)
        {
            _presenter                   =  presenter;
            _presenter.ValueChanged      += HandleColorPickerViewValueChanged;
            _presenter.ColorValueCleared += HandleColorCleared;
        }
    }

    private void HandleColorPickerViewValueChanged(object? sender, ColorChangedEventArgs args)
    {
        if (ValueSyncStrategy == ColorPickerValueSyncMode.Immediate)
        {
            SetCurrentValue(ValueProperty, args.NewColor);
        }
        else
        {
            _latestSyncValue = args.NewColor;
        }
    }

    protected override void NotifyFlyoutOpened()
    {
        if (PickerFlyout is ColorPickerFlyout colorPickerFlyout)
        {
            var effectiveColor = Value ?? DefaultValue;
            if (effectiveColor != null)
            {
                colorPickerFlyout.SetCurrentValue(ColorPickerFlyout.ValueProperty, effectiveColor);
            }
        }
    }
    
    protected override void NotifyFlyoutClosed()
    {
        if (_presenter != null)
        {
            if (ValueSyncStrategy == ColorPickerValueSyncMode.OnCompleted)
            {
                SetCurrentValue(ValueProperty, _latestSyncValue);
            }
            _presenter.ValueChanged      -= HandleColorPickerViewValueChanged;
            _presenter.ColorValueCleared -= HandleColorCleared;
            _presenter                   =  null;
        }
    }
    
    internal void NotifyValueChanged(ColorChangedEventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }

    private void HandleColorCleared(object? sender, EventArgs args)
    {
        if (_colorIndicator != null)
        {
            _colorIndicator.SetCurrentValue(ColorBlock.IsEmptyColorModeProperty, true);
            _emptyTextBindingDisposable?.Dispose();
            _emptyTextBindingDisposable = BindUtils.RelayBind(this, EmptyColorTextProperty, this, ColorTextProperty);
        }
    }
}
