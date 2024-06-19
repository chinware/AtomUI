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
   private TokenResourceBinder _tokenResourceBinder;
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
         _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadius);
         _tokenResourceBinder.AddBinding(FontSizeProperty, GlobalResourceKey.FontSizeLG);
      } else if (SizeType == SizeType.Middle) {
         _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
         _tokenResourceBinder.AddBinding(FontSizeProperty, GlobalResourceKey.FontSize);
      } else if (SizeType == SizeType.Small) {
         _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusXS);
         _tokenResourceBinder.AddBinding(FontSizeProperty, GlobalResourceKey.FontSize);
      }
   }

   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
      _tokenResourceBinder.ReleaseTriggerBindings(this);
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         if (!_styleState.HasFlag(ControlStyleState.Selected)) {
            _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorTransparent);
            _tokenResourceBinder.AddBinding(ForegroundProperty, SegmentedResourceKey.ItemColor);
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _tokenResourceBinder.AddBinding(BackgroundProperty, SegmentedResourceKey.ItemActiveBg, BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _tokenResourceBinder.AddBinding(BackgroundProperty, SegmentedResourceKey.ItemHoverBg, BindingPriority.StyleTrigger);
               _tokenResourceBinder.AddBinding(ForegroundProperty, SegmentedResourceKey.ItemHoverColor, BindingPriority.StyleTrigger);
            }
         } else {
            _tokenResourceBinder.AddBinding(ForegroundProperty, SegmentedResourceKey.ItemSelectedColor);
         }
      } else {
        _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _tokenResourceBinder.AddBinding(ControlHeightSMTokenProperty, GlobalResourceKey.ControlHeightSM);
      _tokenResourceBinder.AddBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeight);
      _tokenResourceBinder.AddBinding(ControlHeightLGTokenProperty, GlobalResourceKey.ControlHeightLG);
      _tokenResourceBinder.AddBinding(TrackPaddingTokenProperty, SegmentedResourceKey.TrackPadding);
      _tokenResourceBinder.AddBinding(SegmentedItemPaddingSMTokenProperty, SegmentedResourceKey.SegmentedItemPaddingSM);
      _tokenResourceBinder.AddBinding(SegmentedItemPaddingTokenProperty, SegmentedResourceKey.SegmentedItemPadding);
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