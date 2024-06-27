using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class CheckBox : IWaveAdornerInfoProvider,
                                IControlCustomStyle
{
   private IPen? _cachedPen;
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder _controlTokenBinder;
   private ControlStyleState _styleState;
   private bool _initialized = false;
   
   static CheckBox()
   {
      AffectsRender<CheckBox>(
         IsCheckedProperty,
         IndicatorCheckedMarkEffectSizeProperty,
         PaddingInlineProperty,
         IndicatorBorderBrushProperty,
         IndicatorCheckedMarkBrushProperty,
         IndicatorTristateMarkBrushProperty,
         IndicatorBackgroundProperty,
         IndicatorBorderThicknessProperty,
         IndicatorBorderRadiusProperty);
   }

   void IControlCustomStyle.SetupUi()
   {
      Cursor = new Cursor(StandardCursorType.Hand);
      _customStyle.CollectStyleState();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.SetupTransitions();
   }

   void IControlCustomStyle.CollectStyleState()
   {
      StyleUtils.InitCommonState(this, ref _styleState);
      switch (IsChecked) {
         case true:
            _styleState |= ControlStyleState.On;
            break;
         case false:
            _styleState |= ControlStyleState.Off;
            break;
         default:
            _styleState |= ControlStyleState.Indeterminate;
            break;
      }
      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }
   }

   // Measure 之后才有值
   private Rect IndicatorRect()
   {
      var offsetY = (DesiredSize.Height - Margin.Top - Margin.Bottom - CheckIndicatorSize) / 2;
      return new Rect(0d, offsetY, CheckIndicatorSize, CheckIndicatorSize);
   }

   private Rect TextRect()
   {
      var offsetX = CheckIndicatorSize + PaddingInline;
      return new Rect(offsetX, 0d, DesiredSize.Width - offsetX, DesiredSize.Height);
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(CheckIndicatorSizeProperty, CheckBoxResourceKey.CheckIndicatorSize);
      _controlTokenBinder.AddControlBinding(PaddingInlineProperty, GlobalResourceKey.PaddingXS);
      _controlTokenBinder.AddControlBinding(IndicatorBorderRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      _controlTokenBinder.AddControlBinding(IndicatorTristateMarkSizeProperty, CheckBoxResourceKey.IndicatorTristateMarkSize);
      _controlTokenBinder.AddControlBinding(IndicatorTristateMarkBrushProperty, GlobalResourceKey.ColorPrimary);
      _controlTokenBinder.AddControlBinding(IndicatorBorderThicknessProperty, GlobalResourceKey.BorderThickness);
   }

   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
      _controlTokenBinder.ReleaseTriggerBindings(this);
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         _controlTokenBinder.AddControlBinding(ForegroundProperty, GlobalResourceKey.ColorText);
         _controlTokenBinder.AddControlBinding(IndicatorBackgroundProperty, GlobalResourceKey.ColorBgContainer);
         _controlTokenBinder.AddControlBinding(IndicatorCheckedMarkBrushProperty, GlobalResourceKey.ColorBgContainer);
         _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorBorder);
         if (_styleState.HasFlag(ControlStyleState.On)) {
            IndicatorCheckedMarkEffectSize = CheckIndicatorSize;
            _controlTokenBinder.AddControlBinding(IndicatorBackgroundProperty, GlobalResourceKey.ColorPrimary, 
               BindingPriority.StyleTrigger);
            _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimary,
               BindingPriority.StyleTrigger);
            if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _controlTokenBinder.AddControlBinding(IndicatorBackgroundProperty, GlobalResourceKey.ColorPrimaryHover,
                  BindingPriority.StyleTrigger);
               _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                  BindingPriority.StyleTrigger);
            }
         } else if (_styleState.HasFlag(ControlStyleState.Off)) {
            if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                  BindingPriority.StyleTrigger);
            }
            IndicatorCheckedMarkEffectSize = CheckIndicatorSize * 0.7;
         } else if (_styleState.HasFlag(ControlStyleState.Indeterminate)) {
            IndicatorCheckedMarkEffectSize = CheckIndicatorSize * 0.7;
            if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                  BindingPriority.StyleTrigger);
            }
         }
      } else {
         _controlTokenBinder.AddControlBinding(IndicatorBackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
         _controlTokenBinder.AddControlBinding(IndicatorBorderBrushProperty, GlobalResourceKey.ColorBorder);
         _controlTokenBinder.AddControlBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
         if (_styleState.HasFlag(ControlStyleState.On)) {
            IndicatorCheckedMarkEffectSize = CheckIndicatorSize;
            _controlTokenBinder.AddControlBinding(IndicatorCheckedMarkBrushProperty, GlobalResourceKey.ColorTextDisabled,
               BindingPriority.StyleTrigger);
         } else if (_styleState.HasFlag(ControlStyleState.Indeterminate)) {
            _controlTokenBinder.AddControlBinding(IndicatorTristateMarkBrushProperty, GlobalResourceKey.ColorTextDisabled,
               BindingPriority.StyleTrigger);
         }
      }
   }

   void IControlCustomStyle.SetupTransitions()
   {
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(IndicatorBackgroundProperty),
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(IndicatorBorderBrushProperty),
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(IndicatorTristateMarkBrushProperty),
         AnimationUtils.CreateTransition<DoubleTransition>(IndicatorCheckedMarkEffectSizeProperty, GlobalResourceKey.MotionDurationMid, new BackEaseOut())
      };
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
            WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.RoundRectWave);
         }
      }
   }
   
   public Rect WaveGeometry()
   {
      return IndicatorRect();
   }
   
   public CornerRadius WaveBorderRadius()
   {
      return IndicatorBorderRadius;
   }
}