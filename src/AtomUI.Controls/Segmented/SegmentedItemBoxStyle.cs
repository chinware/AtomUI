using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Data;
using Avalonia.Input;

namespace AtomUI.Controls;

internal partial class SegmentedItemBox : IControlCustomStyle
{
   private bool _initialized = false;
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder _controlTokenBinder;
   private bool _isPressed = false;
   private ControlStyleState _styleState;

   void IControlCustomStyle.SetupUi()
   {
      Cursor = new Cursor(StandardCursorType.Hand);
      _customStyle.ApplySizeTypeStyleConfig();
      _customStyle.CollectStyleState();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.SetupTransitions();
   }

   void IControlCustomStyle.SetupTransitions()
   { 
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty),
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty),
      };
   }

   void IControlCustomStyle.CollectStyleState()
   {
      StyleUtils.InitCommonState(this, ref _styleState);
      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }

      if (IsCurrentItem) {
         _styleState |= ControlStyleState.Selected;
      }
   }

   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (SizeType == SizeType.Large) {
         _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadius);
         _controlTokenBinder.AddControlBinding(FontSizeProperty, GlobalResourceKey.FontSizeLG);
      } else if (SizeType == SizeType.Middle) {
         _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
         _controlTokenBinder.AddControlBinding(FontSizeProperty, GlobalResourceKey.FontSize);
      } else if (SizeType == SizeType.Small) {
         _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusXS);
         _controlTokenBinder.AddControlBinding(FontSizeProperty, GlobalResourceKey.FontSize);
      }
   }

   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
      _controlTokenBinder.ReleaseTriggerBindings(this);
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         if (!_styleState.HasFlag(ControlStyleState.Selected)) {
            _controlTokenBinder.AddControlBinding(BackgroundProperty, GlobalResourceKey.ColorTransparent);
            _controlTokenBinder.AddControlBinding(ForegroundProperty, SegmentedResourceKey.ItemColor);
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _controlTokenBinder.AddControlBinding(BackgroundProperty, SegmentedResourceKey.ItemActiveBg, BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _controlTokenBinder.AddControlBinding(BackgroundProperty, SegmentedResourceKey.ItemHoverBg, BindingPriority.StyleTrigger);
               _controlTokenBinder.AddControlBinding(ForegroundProperty, SegmentedResourceKey.ItemHoverColor, BindingPriority.StyleTrigger);
            }
         } else {
            _controlTokenBinder.AddControlBinding(ForegroundProperty, SegmentedResourceKey.ItemSelectedColor);
         }
      } else {
        _controlTokenBinder.AddControlBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(ControlHeightSMTokenProperty, GlobalResourceKey.ControlHeightSM);
      _controlTokenBinder.AddControlBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeight);
      _controlTokenBinder.AddControlBinding(ControlHeightLGTokenProperty, GlobalResourceKey.ControlHeightLG);
      _controlTokenBinder.AddControlBinding(TrackPaddingTokenProperty, SegmentedResourceKey.TrackPadding);
      _controlTokenBinder.AddControlBinding(SegmentedItemPaddingSMTokenProperty, SegmentedResourceKey.SegmentedItemPaddingSM);
      _controlTokenBinder.AddControlBinding(SegmentedItemPaddingTokenProperty, SegmentedResourceKey.SegmentedItemPadding);
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsPressedProperty ||
          e.Property == IsPointerOverProperty ||
          e.Property == IsCurrentItemProperty) {
         _customStyle.CollectStyleState();
         _customStyle.ApplyVariableStyleConfig();
      }

      if (_initialized) {
         if (e.Property == SizeTypeProperty) {
            _customStyle.ApplySizeTypeStyleConfig();
         }
      }
   }
   
}