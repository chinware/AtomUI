using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class AbstractProgressBar : IControlCustomStyle
{
   protected bool _initialized = false;
   protected ControlStyleState _styleState;
   protected ControlTokenBinder _controlTokenBinder;
   internal IControlCustomStyle _customStyle;
   protected Label? _percentageLabel;
   protected PathIcon? _successCompletedIcon;
   protected PathIcon? _exceptionCompletedIcon;

   void IControlCustomStyle.SetupUi()
   {
      _customStyle.CollectStyleState();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.ApplySizeTypeStyleConfig();
      _customStyle.SetupTransitions();
      
      NotifySetupUi();
      
      _initialized = true;
   }

   void IControlCustomStyle.AfterUiStructureReady()
   {
      NotifyUiStructureReady();
   }

   protected virtual void NotifyUiStructureReady()
   {
      // 创建完更新调用一次
      NotifyEffectSizeTypeChanged();
      UpdateProgress(); 
   }

   void IControlCustomStyle.SetupTransitions()
   {
      var transitions = new Transitions();
      
      transitions.Add(AnimationUtils.CreateTransition<DoubleTransition>(ValueProperty, GlobalResourceKey.MotionDurationVerySlow, new ExponentialEaseOut()));
      transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(IndicatorBarBrushProperty, GlobalResourceKey.MotionDurationFast));
      
      NotifySetupTransitions(ref transitions);
      Transitions = transitions;
   }

   void IControlCustomStyle.CollectStyleState()
   {
      StyleUtils.InitCommonState(this, ref _styleState);
   }
   
   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      ApplyIndicatorBarBackgroundStyleConfig();
      NotifyApplyFixedStyleConfig();
   }

   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      ApplyEffectiveSizeTypeStyleConfig();
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == SizeTypeProperty) {
         EffectiveSizeType = e.GetNewValue<SizeType>();
      } else if (e.Property == EffectiveSizeTypeProperty) {
         if (_initialized) {
            NotifyEffectSizeTypeChanged();
         }
      }

      if (e.Property == IsEnabledProperty || 
          e.Property == PercentageProperty) {
         _customStyle.CollectStyleState();
         _customStyle.ApplyVariableStyleConfig();
      }

      NotifyPropertyChanged(e);
   }
   
   protected virtual void NotifySetupTransitions(ref Transitions transitions) {}
   protected virtual void ApplyIndicatorBarBackgroundStyleConfig() {}
   protected virtual void ApplyEffectiveSizeTypeStyleConfig() {}

   protected virtual void NotifySetupUi()
   {
      var label = GetOrCreatePercentInfoLabel();
      AddChildControl(label);
      CreateCompletedIcons();
   }

   protected abstract void CreateCompletedIcons();

   protected virtual void NotifyApplyFixedStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(SuccessThresholdBrushProperty, GlobalResourceKey.ColorSuccess);
   }

   protected virtual void NotifyPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
   }

   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
      _controlTokenBinder.ReleaseTriggerBindings(this);
      NotifyApplyVariableStyleConfig();
   }

   protected virtual void NotifyApplyVariableStyleConfig()
   {
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         if (TrailColor.HasValue) {
            GrooveBrush = new SolidColorBrush(TrailColor.Value);
         } else {
            _controlTokenBinder.AddControlBinding(GrooveBrushProperty, ProgressBarResourceKey.RemainingColor);
         }
       
         if (Status == ProgressStatus.Success || MathUtils.AreClose(Value, Maximum)) {
            _controlTokenBinder.AddControlBinding(IndicatorBarBrushProperty, GlobalResourceKey.ColorSuccess);
         } else if (Status == ProgressStatus.Exception) {
            _controlTokenBinder.AddControlBinding(IndicatorBarBrushProperty, GlobalResourceKey.ColorError);
         } else {
              _controlTokenBinder.AddControlBinding(IndicatorBarBrushProperty, ProgressBarResourceKey.DefaultColor);
         }
         _controlTokenBinder.AddControlBinding(ForegroundProperty, GlobalResourceKey.ColorTextLabel);
         if (_initialized) {
            _exceptionCompletedIcon!.IconMode = IconMode.Normal;
            _successCompletedIcon!.IconMode = IconMode.Normal;
         }
   
      } else {
         _controlTokenBinder.AddControlBinding(GrooveBrushProperty, GlobalResourceKey.ColorBgContainerDisabled);
         _controlTokenBinder.AddControlBinding(IndicatorBarBrushProperty, GlobalResourceKey.ControlItemBgActiveDisabled);
         _controlTokenBinder.AddControlBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
         if (_initialized) {
            _exceptionCompletedIcon!.IconMode = IconMode.Disabled;
            _successCompletedIcon!.IconMode = IconMode.Disabled;
         }
      }
   }

   protected void AddChildControl(Control child)
   {
      VisualChildren.Add(child);
      (child as ISetLogicalParent).SetParent(this);
   }
   
   public override void Render(DrawingContext context)
   {
      NotifyPrepareDrawingContext(context);
      RenderGroove(context);
      RenderIndicatorBar(context);
   }

   protected virtual void NotifyPrepareDrawingContext(DrawingContext context) {}
}