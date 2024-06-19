using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class RadioButton : IWaveAdornerInfoProvider, IControlCustomStyle
{
   private bool _initialized = false;
   private IPen? _cachedPen;
   private ControlStyleState _styleState;
   private IControlCustomStyle _customStyle;
   private TokenResourceBinder _tokenResourceBinder;
   
   void IControlCustomStyle.SetupUi()
   {
      Cursor = new Cursor(StandardCursorType.Hand);
      _customStyle.CollectStyleState();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.SetupTransitions();
   }
   
   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
      _tokenResourceBinder.ReleaseTriggerBindings(this);
      _tokenResourceBinder.AddBinding(RadioBorderBrushProperty, GlobalResourceKey.ColorBorder);
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         // 暂时启用和禁用状态不归为 style trigger
         _tokenResourceBinder.AddBinding(RadioInnerBackgroundProperty, RadioButtonResourceKey.RadioColor);
         if (_styleState.HasFlag(ControlStyleState.On)) {
            _tokenResourceBinder.AddBinding(RadioBorderBrushProperty, GlobalResourceKey.ColorPrimary);
            _tokenResourceBinder.AddBinding(RadioBackgroundProperty, GlobalResourceKey.ColorPrimary);
         } else {
            _tokenResourceBinder.AddBinding(RadioBackgroundProperty, GlobalResourceKey.ColorBgContainer);
            if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _tokenResourceBinder.AddBinding(RadioBorderBrushProperty, GlobalResourceKey.ColorPrimary, 
                  BindingPriority.StyleTrigger);
            }
         }
      } else {
         _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
         _tokenResourceBinder.AddBinding(RadioBackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled, 
            BindingPriority.StyleTrigger);
         _tokenResourceBinder.AddBinding(RadioInnerBackgroundProperty, RadioButtonResourceKey.DotColorDisabled, 
            BindingPriority.StyleTrigger);
      }
      RadioDotEffectSize = CalculateDotSize(IsEnabled, IsChecked.HasValue && IsChecked.Value);
   }
   
   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _tokenResourceBinder.AddBinding(RadioSizeProperty, RadioButtonResourceKey.RadioSize);
      _tokenResourceBinder.AddBinding(DotSizeValueProperty, RadioButtonResourceKey.DotSize);
      _tokenResourceBinder.AddBinding(DotPaddingValueProperty, RadioButtonResourceKey.DotPadding);
      _tokenResourceBinder.AddBinding(PaddingInlineProperty, GlobalResourceKey.PaddingXS);
      _tokenResourceBinder.AddBinding(RadioBorderThicknessProperty, GlobalResourceKey.BorderThickness);
   }

   void IControlCustomStyle.CollectStyleState()
   {
      StyleUtils.InitCommonState(this, ref _styleState);
      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }

      if (IsChecked.HasValue && IsChecked.Value) {
         _styleState |= ControlStyleState.On;
      } else {
         _styleState |= ControlStyleState.Off;
      }
   }

   private double CalculateDotSize(bool isEnabled, bool isChecked)
   {
      double targetValue;
      if (isChecked) {
         if (isEnabled) {
            targetValue = _dotSizeValue;
         } else {
            targetValue = RadioSize - _dotPaddingValue * 2;
         }
      } else {
         targetValue = _dotSizeValue * 0.6;
      }

      return targetValue;
   }

   void IControlCustomStyle.SetupTransitions()
   {
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(RadioBorderBrushProperty),
         AnimationUtils.CreateTransition<DoubleTransition>(RadioDotEffectSizeProperty),
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(RadioBackgroundProperty, GlobalResourceKey.MotionDurationFast)
      };
   }

   // Measure 之后才有值
   private Rect RadioRect()
   {
      var offsetY = (DesiredSize.Height - Margin.Top - Margin.Bottom - RadioSize) / 2;
      return new Rect(0d, offsetY, RadioSize, RadioSize);
   }

   private Rect RadioTextRect()
   {
      var offsetX = RadioSize + PaddingInline;
      return new Rect(offsetX, 0d, DesiredSize.Width - offsetX, DesiredSize.Height);
   }
   
   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsPointerOverProperty ||
          e.Property == IsCheckedProperty ||
          e.Property == IsEnabledProperty) {
         _customStyle.CollectStyleState();
         _customStyle.ApplyVariableStyleConfig();
         if (e.Property == IsCheckedProperty && 
             _styleState.HasFlag(ControlStyleState.Enabled) &&
             _styleState.HasFlag(ControlStyleState.On)) {
            WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.CircleWave);
         }
      }
   }

   public Rect WaveGeometry()
   {
      return RadioRect();
   }
   
   public CornerRadius WaveBorderRadius()
   {
      return new CornerRadius(RadioSize / 2);
   }
}