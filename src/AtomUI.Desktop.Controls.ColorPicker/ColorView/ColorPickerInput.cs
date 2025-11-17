using AtomUI.Controls.Themes;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Converters;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

internal class ColorPickerInput : TemplatedControl
{
   #region 公共属性定义
   public static readonly StyledProperty<ColorFormat> FormatProperty =
      AbstractColorPickerView.FormatProperty.AddOwner<ColorPickerInput>();
   
   public static readonly StyledProperty<bool> IsFormatEnabledProperty =
      AbstractColorPickerView.IsFormatEnabledProperty.AddOwner<ColorPickerInput>();
    
   public static readonly StyledProperty<HsvColor> ColorValueProperty =
      AvaloniaProperty.Register<ColorPickerInput, HsvColor>(nameof(ColorValue),
         Colors.White.ToHsv());
   
   public static readonly StyledProperty<bool> IsClearEnabledProperty =
      AbstractColorPickerView.IsClearEnabledProperty.AddOwner<ColorPickerInput>();
   
   public static readonly StyledProperty<bool> IsAlphaVisibleProperty =
      AbstractColorPickerView.IsAlphaVisibleProperty.AddOwner<ColorPickerInput>();
   
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
    
   public HsvColor ColorValue
   {
      get => GetValue(ColorValueProperty);
      set => SetValue(ColorValueProperty, value);
   }
   
   public bool IsClearEnabled
   {
      get => GetValue(IsClearEnabledProperty);
      set => SetValue(IsClearEnabledProperty, value);
   }
   
   public bool IsAlphaVisible
   {
      get => GetValue(IsAlphaVisibleProperty);
      set => SetValue(IsAlphaVisibleProperty, value);
   }
   #endregion

   private ComboBox? _colorFormatComboBox;
   private NumericUpDown? _alphaInput;
   private LineEdit? _hexValueInput;
   private NumericUpDown? _hValueInput;
   private NumericUpDown? _sValueInput;
   private NumericUpDown? _vValueInput;
   private NumericUpDown? _rValueInput;
   private NumericUpDown? _gValueInput;
   private NumericUpDown? _bValueInput;
   private bool _ignoringConfigureValues;
   private bool _alphaInputPassiveChanged;
   private bool _hexValueInputPassiveChanged;
   private bool _hValueInputPassiveChanged;
   private bool _sValueInputPassiveChanged;
   private bool _vValueInputPassiveChanged;
   private bool _rValueInputPassiveChanged;
   private bool _gValueInputPassiveChanged;
   private bool _bValueInputPassiveChanged;
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
   {
      base.OnPropertyChanged(change);
      if (this.IsAttachedToVisualTree())
      {
         if (change.Property == ColorValueProperty)
         {
            if (_ignoringConfigureValues)
            {
               return;
            }
            ConfigureColorValues();
         }
         else if (change.Property == FormatProperty)
         {
            ConfigureComboBoxSelected();
         }
      }
   }

   private void ConfigureComboBoxSelected()
   {
      if (_colorFormatComboBox != null)
      {
         if (Format == ColorFormat.Hex)
         {
            _colorFormatComboBox.SelectedIndex = 0;
         }
         else if (Format == ColorFormat.Hsva)
         {
            _colorFormatComboBox.SelectedIndex = 1;
         } else if (Format == ColorFormat.Rgba)
         {
            _colorFormatComboBox.SelectedIndex = 2;
         }
      }
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _colorFormatComboBox = e.NameScope.Find<ComboBox>(ColorPickerInputThemeConstants.ColorFormatComboBoxPart);
      _alphaInput          = e.NameScope.Find<NumericUpDown>(ColorPickerInputThemeConstants.AlphaInputPart);
      _hexValueInput       = e.NameScope.Find<LineEdit>(ColorPickerInputThemeConstants.HexValueInputPart);
      _hValueInput         = e.NameScope.Find<NumericUpDown>(ColorPickerInputThemeConstants.HValueInputPart);
      _sValueInput         = e.NameScope.Find<NumericUpDown>(ColorPickerInputThemeConstants.SValueInputPart);
      _vValueInput         = e.NameScope.Find<NumericUpDown>(ColorPickerInputThemeConstants.VValueInputPart);
      _rValueInput         = e.NameScope.Find<NumericUpDown>(ColorPickerInputThemeConstants.RValueInputPart);
      _gValueInput         = e.NameScope.Find<NumericUpDown>(ColorPickerInputThemeConstants.GValueInputPart);
      _bValueInput         = e.NameScope.Find<NumericUpDown>(ColorPickerInputThemeConstants.BValueInputPart);

      if (_colorFormatComboBox != null)
      {
         _colorFormatComboBox.SelectionChanged += HandleFormatChanged;
         ConfigureComboBoxSelected();
      }
      
      if (_alphaInput != null)
      {
         _alphaInput.ValueChanged += (sender, args) =>
         {
            if (_alphaInputPassiveChanged)
            {
               _alphaInputPassiveChanged = false;
               return;
            }
            SyncInputValue();
         };
      }
      
      if (_hexValueInput != null)
      {
         _hexValueInput.TextChanged += (sender, args) =>
         {
            if (_hexValueInputPassiveChanged)
            {
               _hexValueInputPassiveChanged = false;
               return;
            }
            SyncInputValue();
         };
      }

      if (_hValueInput != null)
      {
         _hValueInput.ValueChanged += (sender, args) =>
         {
            if (_hValueInputPassiveChanged)
            {
               _hValueInputPassiveChanged = false;
               return;
            }
            SyncInputValue();
         };
      }

      if (_sValueInput != null)
      {
         _sValueInput.ValueChanged += (sender, args) =>
         {
            if (_sValueInputPassiveChanged)
            {
               _sValueInputPassiveChanged = false;
               return;
            }
            SyncInputValue();
         };
      }
      
      if (_vValueInput != null)
      {
         _vValueInput.ValueChanged += (sender, args) =>
         {
            if (_vValueInputPassiveChanged)
            {
               _vValueInputPassiveChanged = false;
               return;
            }
            SyncInputValue();
         };
      }

      if (_rValueInput != null)
      {
         _rValueInput.ValueChanged += (sender, args) =>
         {
            if (_rValueInputPassiveChanged)
            {
               _rValueInputPassiveChanged = false;
               return;
            }
            SyncInputValue();
         };
      }
      
      if (_gValueInput != null)
      {
         _gValueInput.ValueChanged += (sender, args) =>
         {
            if (_gValueInputPassiveChanged)
            {
               _gValueInputPassiveChanged = false;
               return;
            }
            SyncInputValue();
         };
      }
      
      if (_bValueInput != null)
      {
         _bValueInput.ValueChanged += (sender, args) =>
         {
            if (_bValueInputPassiveChanged)
            {
               _bValueInputPassiveChanged = false;
               return;
            }
            SyncInputValue();
         };
      }
      ConfigureColorValues();
   }

   private void HandleFormatChanged(object? sender, SelectionChangedEventArgs args)
   {
      if (_colorFormatComboBox?.SelectedItem is ComboBoxItem comboBoxItem &&
          comboBoxItem.Tag is ColorFormat colorFormat)
      {
         Format = colorFormat;
         using var scope = BeginIgnoringConfigureValues();
         ConfigureColorValues();
      }
   }

   private void ConfigureColorValues()
   {
      _alphaInputPassiveChanged    = true;
      _hexValueInputPassiveChanged = true;
      _hValueInputPassiveChanged   = true;
      _sValueInputPassiveChanged   = true;
      _vValueInputPassiveChanged   = true;
      _rValueInputPassiveChanged   = true;
      _gValueInputPassiveChanged   = true;
      _bValueInputPassiveChanged   = true;
      if (Format == ColorFormat.Hex)
      {
         var rgbValue = ColorValue.ToRgb();
         if (_hexValueInput != null)
         {
            _hexValueInput.Text = ColorToHexConverter.ToHexString(rgbValue, AlphaComponentPosition.Leading, false, true);
         }
      }
      else if (Format == ColorFormat.Hsva)
      {
         if (_hValueInput != null)
         {
            _hValueInput.Value = (int)ColorValue.H;
         }
         if (_sValueInput != null)
         {
            _sValueInput.Value = new decimal(ColorValue.S * 100);
         }
         if (_vValueInput != null)
         {
            _vValueInput.Value = new decimal(ColorValue.V * 100);
         }
      }
      else if (Format == ColorFormat.Rgba)
      {
         var rgbValue = ColorValue.ToRgb();
         if (_rValueInput != null)
         {
            _rValueInput.Value = rgbValue.R;
         }

         if (_gValueInput != null)
         {
            _gValueInput.Value = rgbValue.G;
         }
         
         if (_bValueInput != null)
         {
            _bValueInput.Value = rgbValue.B;
         }
      }
      if (_alphaInput != null)
      {
         var alpha = (int)(ColorValue.A * 100);
         _alphaInput.Value = alpha;
      }
   }

   private void SyncInputValue()
   {
      if (Format == ColorFormat.Hex)
      {
         var colorValue = _hexValueInput?.Text?.Trim();
         if (colorValue != null)
         {
            if (Color.TryParse(colorValue, out var value))
            {
               var       percentage   = _alphaInput?.Value ?? 100m;
               byte      alphaValue   = (byte)(percentage * 2.55m);
               var       combineValue = Color.FromArgb(alphaValue, value.R, value.G, value.B);
               using var scope        = BeginIgnoringConfigureValues();
               SetCurrentValue(ColorValueProperty, combineValue.ToHsv());
            }
         }
      }
      else if (Format == ColorFormat.Hsva)
      {
         var hValue     = (double?)_hValueInput?.Value;
         var sValue     = (double?)_sValueInput?.Value;
         var vValue     = (double?)_vValueInput?.Value;
         var percentage = _alphaInput?.Value ?? 100m;
         var alphaValue = (double)percentage / 100;
         if (hValue != null && sValue != null && vValue != null)
         {
            using var scope         = BeginIgnoringConfigureValues();
            var       newColorValue = HsvColor.FromAhsv(alphaValue, hValue.Value, sValue.Value / 100, vValue.Value / 100);
            SetCurrentValue(ColorValueProperty, newColorValue);
         }
      }
      else if (Format == ColorFormat.Rgba)
      {
         var  rValue     = _rValueInput?.Value;
         var  gValue     = _gValueInput?.Value;
         var  bValue     = _bValueInput?.Value;
         var  percentage = _alphaInput?.Value ?? 100m;
         byte alphaValue = (byte)(percentage * 2.55m);
         if (rValue != null && gValue != null && bValue != null)
         {
            using var scope         = BeginIgnoringConfigureValues();
            var       newColorValue = Color.FromArgb(alphaValue, (byte)rValue.Value, (byte)gValue.Value, (byte)bValue.Value);
            SetCurrentValue(ColorValueProperty, newColorValue.ToHsv());
         }
      }
   }
   
   private IgnoreConfigureValues BeginIgnoringConfigureValues() => new IgnoreConfigureValues(this);
    
   private readonly struct IgnoreConfigureValues : IDisposable
   {
      private readonly ColorPickerInput _owner;

      public IgnoreConfigureValues(ColorPickerInput owner)
      {
         _owner                          = owner;
         _owner._ignoringConfigureValues = true;
      }

      public void Dispose() => _owner._ignoringConfigureValues = false;
   }
}