using AtomUI.Controls.Switch;
using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;

namespace AtomUI.Controls;

public partial class ToggleSwitch : IControlCustomStyle, IWaveAdornerInfoProvider
{
   private IControlCustomStyle _customStyle;
   private ControlTokenBinder _controlTokenBinder;
   private ControlStyleState _styleState;
   private Canvas? _togglePanel;
   
   void IControlCustomStyle.SetupUi()
   {
      if (!IsLoading) {
         Cursor = new Cursor(StandardCursorType.Hand);
      }
      
      _customStyle.CollectStyleState();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.ApplySizeTypeStyleConfig();

      var handleSize = HandleSize();
      _switchKnob.KnobSize = new Size(handleSize, handleSize);
      
      _controlTokenBinder.AddControlBinding(_switchKnob, SwitchKnob.KnobBackgroundColorProperty, ToggleSwitchResourceKey.HandleBg);
      _controlTokenBinder.AddControlBinding(_switchKnob, SwitchKnob.KnobBoxShadowProperty, ToggleSwitchResourceKey.HandleShadow);
     
      _togglePanel = new Canvas();
      _togglePanel.Children.Add(_switchKnob);
      if (OffContent is Control offControl) {
         _togglePanel.Children.Add(offControl);
         if (OffContent is TemplatedControl templatedControl) {
            templatedControl.Padding = new Thickness(0);
         } else if (OffContent is PathIcon iconControl) {
            if (iconControl.ThemeType != IconThemeType.TwoTone) {
               iconControl.NormalFilledBrush ??= Foreground;
            }
         }
         offControl.Width = offControl.Height = IconSize();
      } else if (OffContent is string offStr) {
         var label = new Label
         {
            Content = offStr,
            Padding = new Thickness(0)
         };
         _togglePanel.Children.Add(label);
         OffContent = label;
      }
      
      // TODO 这里要不要限定支持的 control 类型呢？
      if (OnContent is Control onControl) {
         _togglePanel.Children.Add(onControl);
         if (OnContent is TemplatedControl templatedControl) {
            templatedControl.Padding = new Thickness(0);
         } else if (OnContent is PathIcon iconControl) {
            if (iconControl.ThemeType != IconThemeType.TwoTone) {
               iconControl.NormalFilledBrush ??= Foreground;
            }
         }

         onControl.Width = onControl.Height = IconSize();
      } else if (OnContent is string onStr) {
         var label = new Label
         {
            Content = onStr,
            Padding = new Thickness(0)
         };
         _togglePanel.Children.Add(label);
         OnContent = label;
      }

      HandleLoadingState(IsLoading);
      
      Content = _togglePanel;
   }

   private void CalculateElementsOffset(Size controlSize)
   {
      var isChecked = IsChecked.HasValue && IsChecked.Value;
      var handleRect = HandleRect(isChecked, controlSize);
      KnobOffset = handleRect.TopLeft;

      var onExtraInfoRect = ExtraInfoRect(true, controlSize);
      var offExtraInfoRect = ExtraInfoRect(false, controlSize);
      if (isChecked) {
         OnContentOffset = onExtraInfoRect.TopLeft;
         OffContentOffset = new Point(controlSize.Width + 1, onExtraInfoRect.Top);
      } else {
         OffContentOffset = offExtraInfoRect.TopLeft;
         OnContentOffset = new Point(-offExtraInfoRect.Width, offExtraInfoRect.Top);
      }
   }

   void IControlCustomStyle.SetupTransitions()
   { 
      Transitions = new Transitions()
      {
         AnimationUtils.CreateTransition<PointTransition>(KnobOffsetProperty),
         AnimationUtils.CreateTransition<PointTransition>(OnContentOffsetProperty),
         AnimationUtils.CreateTransition<PointTransition>(OffContentOffsetProperty),
         AnimationUtils.CreateTransition<SolidColorBrushTransition>(GrooveBackgroundProperty),
         AnimationUtils.CreateTransition<DoubleTransition>(SwitchOpacityProperty),
      };
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

   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
      _controlTokenBinder.ReleaseTriggerBindings(this);
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         SwitchOpacity = 1.0;
         if (_styleState.HasFlag(ControlStyleState.On)) {
            _controlTokenBinder.AddControlBinding(GrooveBackgroundProperty, ToggleSwitchResourceKey.SwitchColor);
            if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _controlTokenBinder.AddControlBinding(GrooveBackgroundProperty, GlobalResourceKey.ColorPrimaryHover, BindingPriority.StyleTrigger);
            }
         } else {
            _controlTokenBinder.AddControlBinding(GrooveBackgroundProperty, GlobalResourceKey.ColorTextQuaternary);
            if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _controlTokenBinder.AddControlBinding(GrooveBackgroundProperty, GlobalResourceKey.ColorTextTertiary, BindingPriority.StyleTrigger);
            }
         }
      } else {
         SwitchOpacity = _switchDisabledOpacity;
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _controlTokenBinder.AddControlBinding(HandleSizeTokenProperty, ToggleSwitchResourceKey.HandleSize);
      _controlTokenBinder.AddControlBinding(HandleSizeSMTokenProperty, ToggleSwitchResourceKey.HandleSizeSM);
      _controlTokenBinder.AddControlBinding(InnerMaxMarginTokenProperty, ToggleSwitchResourceKey.InnerMaxMargin);
      _controlTokenBinder.AddControlBinding(this, InnerMaxMarginSMTokenProperty, ToggleSwitchResourceKey.InnerMaxMarginSM);
      _controlTokenBinder.AddControlBinding(this, InnerMinMarginTokenProperty, ToggleSwitchResourceKey.InnerMinMargin);
      _controlTokenBinder.AddControlBinding(this, InnerMinMarginSMTokenProperty, ToggleSwitchResourceKey.InnerMinMarginSM);
      _controlTokenBinder.AddControlBinding(this, TrackHeightTokenProperty, ToggleSwitchResourceKey.TrackHeight);
      _controlTokenBinder.AddControlBinding(this, TrackHeightSMTokenProperty, ToggleSwitchResourceKey.TrackHeightSM);
      _controlTokenBinder.AddControlBinding(this, TrackMinWidthTokenProperty, ToggleSwitchResourceKey.TrackMinWidth);
      _controlTokenBinder.AddControlBinding(this, TrackMinWidthSMTokenProperty, ToggleSwitchResourceKey.TrackMinWidthSM);
      _controlTokenBinder.AddControlBinding(this, TrackPaddingTokenProperty, ToggleSwitchResourceKey.TrackPadding);
      _controlTokenBinder.AddControlBinding(this, SwitchDisabledOpacityTokenProperty, ToggleSwitchResourceKey.SwitchDisabledOpacity);
      
      _controlTokenBinder.AddControlBinding(this, ForegroundProperty, GlobalResourceKey.ColorTextLightSolid);

      SwitchOpacity = 1d;
   }
   
   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (IsDefaultSize()) {
         _controlTokenBinder.AddControlBinding(this, FontSizeProperty, ToggleSwitchResourceKey.ExtraInfoFontSize);
      } else {
         _controlTokenBinder.AddControlBinding(this, FontSizeProperty, ToggleSwitchResourceKey.ExtraInfoFontSizeSM);
      }
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == DesiredSizeProperty) {
         var handleSize = HandleSize();
         _switchKnob.KnobSize = new Size(handleSize, handleSize);
      }
      if ((e.Property == IsPointerOverProperty && !IsLoading) ||
          e.Property == IsCheckedProperty ||
          e.Property == IsEnabledProperty) {
         
         _customStyle.CollectStyleState();
         _customStyle.ApplyVariableStyleConfig();
         if (e.Property == IsCheckedProperty) {
            CalculateElementsOffset(DesiredSize);
            WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.PillWave);
            _switchKnob.IsCheckedState = IsChecked.HasValue && IsChecked.Value;
         }
      }
      
      if (e.Property == IsLoadingProperty) {
         HandleLoadingState(IsLoading);
      }
      
      if (e.Property == SizeTypeProperty) {
         _customStyle.ApplySizeTypeStyleConfig();
      }
   }

   private bool IsDefaultSize()
   {
      return SizeType == SizeType.Middle || SizeType == SizeType.Large;
   }

   private void HandleLoadingState(bool isLoading)
   {
      if (isLoading) {
         Cursor = new Cursor(StandardCursorType.Arrow);
         SwitchOpacity = _switchDisabledOpacity;
         _switchKnob.NotifyStartLoading();
      } else {
         Cursor = new Cursor(StandardCursorType.Hand);
         SwitchOpacity = 1d;
         _switchKnob.NotifyStopLoading();
      }
   }

   private double HandleSize()
   {
      if (IsDefaultSize()) {
         return _handleSize;
      }
      return _handleSizeSM;
   }

   private double InnerMaxMargin()
   {
      if (IsDefaultSize()) {
         return _innerMaxMargin;
      } 
      return _innerMaxMarginSM;
   }
   
   private double InnerMinMargin()
   {
      if (IsDefaultSize()) {
         return _innerMinMargin;
      }
      return _innerMinMarginSM;
   }

   private double TrackHeight()
   {
      if (IsDefaultSize()) {
         return _trackHeight;
      } 
      return _trackHeightSM;
   }

   private double TrackMinWidth()
   {
      if (IsDefaultSize()) {
         return _trackMinWidth;
      } 
      return _trackMinWidthSM;
   }

   private double IconSize()
   {
      if (IsDefaultSize()) {
         return _trackHeightSM;
      }
      return _trackHeightSM - 4;
   }

   private double PressOffsetRange()
   {
      double value;
      if (IsDefaultSize()) {
         value = _handleSize;
      } else {
         value = _handleSizeSM;
      }

      value /= 3;
      return value;
   }

   private Rect GrooveRect()
   {
      return new Rect(new Point(0, 0), DesiredSize);
   }

   private Rect HandleRect()
   {
      return HandleRect(IsChecked.HasValue && IsChecked.Value, DesiredSize);
   }

   private Rect HandleRect(bool isChecked, Size controlSize)
   {
      double handlePosX;
      double handlePosY;
      double handleSize = HandleSize();
      double offsetX = _trackPadding;
      double offsetY = _trackPadding;
      if (!isChecked) {
         handlePosX = offsetX;
         handlePosY = offsetY;
      } else {
         if (IsPressed) {
            handleSize *= STRETCH_FACTOR;
         }
         handlePosX = controlSize.Width - offsetX - handleSize;
         handlePosY = offsetY;
      }

      return new Rect(handlePosX, handlePosY, handleSize, handleSize);
   }

   private Rect ExtraInfoRect()
   {
      return ExtraInfoRect(IsChecked.HasValue && IsChecked.Value, DesiredSize);
   }

   private Rect ExtraInfoRect(bool isChecked, Size controlSize)
   {
      double innerMinMargin = InnerMinMargin();
      double innerMaxMargin = InnerMaxMargin();
      double yAdjustValue = 0;
      Rect targetRect = new Rect(new Point(0, 0), controlSize);
      if (isChecked) {
         if (OffContent is Control offControl) {
            yAdjustValue = (controlSize.Height - offControl.DesiredSize.Height) / 2;
         }
         targetRect = targetRect.Inflate(new Thickness(-innerMinMargin, -yAdjustValue, innerMaxMargin, 0));
      } else {
         if (OnContent is Control onControl) {
            yAdjustValue = (controlSize.Height - onControl.DesiredSize.Height) / 2;
         }
         targetRect = targetRect.Inflate(new Thickness(-innerMaxMargin, -yAdjustValue, innerMinMargin, 0));
      }

      return targetRect;
   }

   public Rect WaveGeometry()
   {
      return GrooveRect();
   }

   public CornerRadius WaveBorderRadius()
   {
      return new CornerRadius(DesiredSize.Height / 2);
   }
}