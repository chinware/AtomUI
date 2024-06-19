using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;

namespace AtomUI.Controls;

using ButtonSizeType = SizeType;

public partial class OptionButton : IWaveAdornerInfoProvider, IControlCustomStyle
{
   private bool _initialized = false;
   private ControlStyleState _styleState;
   private IControlCustomStyle _customStyle;
   private TokenResourceBinder _tokenResourceBinder;
   private StackPanel? _stackPanel;
   private Label? _label;
   private CornerRadius? _originCornerRadius;

   public Rect WaveGeometry()
   {
      return new Rect(0, 0, Bounds.Width, Bounds.Height);
   }

   public CornerRadius WaveBorderRadius()
   {
      return CornerRadius;
   }

   void IControlCustomStyle.SetupUi()
   {
      Cursor = new Cursor(StandardCursorType.Hand);
      VerticalAlignment = VerticalAlignment.Center;
      _customStyle.CollectStyleState();
      CreateMainLayout();
      _customStyle.ApplySizeTypeStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.SetupTransitions();
   }

   private void CreateMainLayout()
   {
      if (Text.Length == 0 && Content is string content) {
         Text = content;
      }

      _label = new Label()
      {
         Content = Text,
         Padding = new Thickness(0),
         VerticalContentAlignment = VerticalAlignment.Center,
         HorizontalContentAlignment = HorizontalAlignment.Center,
         VerticalAlignment = VerticalAlignment.Center
      };
      _stackPanel = new StackPanel()
      {
         UseLayoutRounding = false,
         VerticalAlignment = VerticalAlignment.Center,
         HorizontalAlignment = HorizontalAlignment.Center,
         Orientation = Orientation.Horizontal,
         ClipToBounds = true
      };
      _stackPanel.Children.Add(_label);
      Content = _stackPanel;
      BindUtils.RelayBind(this, "Width", _stackPanel);
      BindUtils.RelayBind(this, "Height", _stackPanel);
   }

   void IControlCustomStyle.SetupTransitions()
   {
      var transitions = new Transitions();
      if (ButtonStyle == OptionButtonStyle.Solid) {
         transitions.Add(new SolidColorBrushTransition()
         {
            Property = BackgroundProperty,
            Duration = _motionDuration
         });
      } else if (ButtonStyle == OptionButtonStyle.Outline) {
         transitions.Add(new SolidColorBrushTransition()
         {
            Property = BorderBrushProperty,
            Duration = _motionDuration
         });
      }

      transitions.Add(new SolidColorBrushTransition()
      {
         Property = ForegroundProperty,
         Duration = _motionDuration
      });
      Transitions = transitions;
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
         _styleState |= ControlStyleState.Selected;
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _tokenResourceBinder.AddBinding(MotionDurationTokenProperty, GlobalResourceKey.MotionDurationMid);
      _tokenResourceBinder.AddBinding(ColorPrimaryHoverTokenProperty, GlobalResourceKey.ColorPrimaryHover);
      _tokenResourceBinder.AddBinding(ColorPrimaryActiveTokenProperty, GlobalResourceKey.ColorPrimaryActive);
   }

   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      _tokenResourceBinder.AddBinding(BorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Style,
                                      new RenderScaleAwareThicknessConfigure(this, thickness =>
                                      {
                                         if (InOptionGroup) {
                                            return new Thickness(0);
                                         }

                                         return thickness;
                                      }));
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsPointerOverProperty ||
          e.Property == IsPressedProperty ||
          e.Property == IsCheckedProperty) {
         _customStyle.CollectStyleState();
         _customStyle.ApplyVariableStyleConfig();
         if (e.Property == IsPressedProperty) {
            if (_styleState.HasFlag(ControlStyleState.Raised)) {
               WaveSpiritAdorner.ShowWaveAdorner(this, WaveType.RoundRectWave);
            }
         }
      }

      if (e.Property == GroupPositionTraitProperty) {
         if (_originCornerRadius.HasValue) {
            CornerRadius = BuildCornerRadius(GroupPositionTrait, _originCornerRadius!.Value);
         }
      }

      if (_initialized && e.Property == SizeTypeProperty) {
         _customStyle.ApplySizeTypeStyleConfig();
      }

      if (_initialized && e.Property == ContentProperty) {
         // 不推荐，尽最大能力还原
         var oldText = (_label!.Content as string)!;
         var newContent = e.GetNewValue<object?>();
         if (newContent is string newText) {
            if (oldText != newText) {
               _label!.Content = newText;
            }
         }

         Content = _stackPanel;
      }

      if (_initialized && e.Property == TextProperty) {
         _label!.Content = Text;
      }

      if (e.Property == InOptionGroupProperty) {
         _customStyle.ApplyRenderScalingAwareStyleConfig();
      }
   }

   private CornerRadius BuildCornerRadius(OptionButtonPositionTrait positionTrait, CornerRadius cornerRadius)
   {
      if (positionTrait == OptionButtonPositionTrait.First) {
         return new CornerRadius(cornerRadius.TopLeft,
                                 0,
                                 0,
                                 cornerRadius.BottomLeft);
      } else if (positionTrait == OptionButtonPositionTrait.Last) {
         return new CornerRadius(0,
                                 cornerRadius.TopRight,
                                 cornerRadius.BottomRight,
                                 0);
      } else if (positionTrait == OptionButtonPositionTrait.Middle) {
         return new CornerRadius(0);
      }

      return cornerRadius;
   }

   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
      _tokenResourceBinder.ReleaseTriggerBindings(this);
      if (ButtonStyle == OptionButtonStyle.Outline) {
         ApplyOutlineStyle();
      } else if (ButtonStyle == OptionButtonStyle.Solid) {
         ApplySolidStyle();
      }
   }

   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (SizeType == ButtonSizeType.Small) {
         _tokenResourceBinder.AddBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeightSM);
         _tokenResourceBinder.AddBinding(FontSizeProperty, OptionButtonResourceKey.ContentFontSizeSM);
         _tokenResourceBinder.AddBinding(PaddingProperty, OptionButtonResourceKey.PaddingSM);
         _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      } else if (SizeType == ButtonSizeType.Middle) {
         _tokenResourceBinder.AddBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeight);
         _tokenResourceBinder.AddBinding(FontSizeProperty, OptionButtonResourceKey.ContentFontSize);
         _tokenResourceBinder.AddBinding(PaddingProperty, OptionButtonResourceKey.Padding);
         _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      } else if (SizeType == ButtonSizeType.Large) {
         _tokenResourceBinder.AddBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeightLG);
         _tokenResourceBinder.AddBinding(FontSizeProperty, OptionButtonResourceKey.ContentFontSizeLG);
         _tokenResourceBinder.AddBinding(PaddingProperty, OptionButtonResourceKey.PaddingLG);
         _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
      }

      _originCornerRadius = CornerRadius;
      CornerRadius = BuildCornerRadius(GroupPositionTrait, _originCornerRadius!.Value);
   }

   private void ApplySolidStyle()
   {
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         if (_styleState.HasFlag(ControlStyleState.Selected)) {
            _tokenResourceBinder.AddBinding(ForegroundProperty, OptionButtonResourceKey.ButtonSolidCheckedColor);
            _tokenResourceBinder.AddBinding(BackgroundProperty, OptionButtonResourceKey.ButtonSolidCheckedBackground);
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _tokenResourceBinder.AddBinding(BackgroundProperty,
                                               OptionButtonResourceKey.ButtonSolidCheckedActiveBackground,
                                               BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _tokenResourceBinder.AddBinding(BackgroundProperty,
                                               OptionButtonResourceKey.ButtonSolidCheckedHoverBackground,
                                               BindingPriority.StyleTrigger);
            }
         } else {
            _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorText);
            if (InOptionGroup) {
               _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorTransparent);
            } else {
               _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorBgContainer);
            }

            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorPrimaryActive,
                                               BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorPrimaryHover,
                                               BindingPriority.StyleTrigger);
            }
         }
      } else {
         _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorBorder);
         if (_styleState.HasFlag(ControlStyleState.Selected)) {
            _tokenResourceBinder.AddBinding(ForegroundProperty, OptionButtonResourceKey.ButtonCheckedColorDisabled);
            _tokenResourceBinder.AddBinding(BackgroundProperty, OptionButtonResourceKey.ButtonCheckedBgDisabled);
         } else {
            _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
            _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
         }
      }
   }

   private void ApplyOutlineStyle()
   {
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         if (_styleState.HasFlag(ControlStyleState.Selected)) {
            if (InOptionGroup) {
               _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorTransparent);
            } else {
               _tokenResourceBinder.AddBinding(BackgroundProperty, OptionButtonResourceKey.ButtonBackground);
            }

            _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorPrimary);
            _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorPrimary);
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorPrimaryActive,
                                               BindingPriority.StyleTrigger);
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorPrimaryActive,
                                               BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _tokenResourceBinder.AddBinding(this, BorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                                               BindingPriority.StyleTrigger);
               _tokenResourceBinder.AddBinding(this, ForegroundProperty, GlobalResourceKey.ColorPrimaryHover,
                                               BindingPriority.StyleTrigger);
            }
         } else {
            if (InOptionGroup) {
               _tokenResourceBinder.AddBinding(this, BackgroundProperty, GlobalResourceKey.ColorTransparent);
            } else {
               _tokenResourceBinder.AddBinding(this, BackgroundProperty,
                                               OptionButtonResourceKey.ButtonCheckedBackground);
            }

            _tokenResourceBinder.AddBinding(this, BorderBrushProperty, GlobalResourceKey.ColorBorder);
            _tokenResourceBinder.AddBinding(this, ForegroundProperty, OptionButtonResourceKey.ButtonColor);
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _tokenResourceBinder.AddBinding(this, BorderBrushProperty, GlobalResourceKey.ColorPrimaryActive,
                                               BindingPriority.StyleTrigger);
               _tokenResourceBinder.AddBinding(this, ForegroundProperty, GlobalResourceKey.ColorPrimaryActive,
                                               BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _tokenResourceBinder.AddBinding(this, BorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                                               BindingPriority.StyleTrigger);
               _tokenResourceBinder.AddBinding(this, ForegroundProperty, GlobalResourceKey.ColorPrimaryHover,
                                               BindingPriority.StyleTrigger);
            }
         }
      } else {
         _tokenResourceBinder.AddBinding(this, BorderBrushProperty, GlobalResourceKey.ColorBorder);
         if (_styleState.HasFlag(ControlStyleState.Selected)) {
            _tokenResourceBinder.AddBinding(this, ForegroundProperty,
                                            OptionButtonResourceKey.ButtonCheckedColorDisabled);
            _tokenResourceBinder.AddBinding(this, BackgroundProperty, OptionButtonResourceKey.ButtonCheckedBgDisabled);
         } else {
            _tokenResourceBinder.AddBinding(this, ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
            _tokenResourceBinder.AddBinding(this, BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
         }
      }
   }

   protected override void OnPointerPressed(PointerPressedEventArgs e)
   {
      base.OnPointerPressed(e);
      OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
      {
         IsHovering = true,
         IsPressed = true
      });
   }

   protected override void OnPointerReleased(PointerReleasedEventArgs e)
   {
      base.OnPointerReleased(e);
      OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
      {
         IsHovering = true,
         IsPressed = false
      });
   }

   protected override void OnPointerEntered(PointerEventArgs e)
   {
      base.OnPointerEntered(e);
      OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
      {
         IsHovering = true,
         IsPressed = false
      });
   }

   protected override void OnPointerExited(PointerEventArgs e)
   {
      base.OnPointerExited(e);
      OptionButtonPointerEvent?.Invoke(this, new OptionButtonPointerEventArgs(this)
      {
         IsHovering = false,
         IsPressed = false
      });
   }
}