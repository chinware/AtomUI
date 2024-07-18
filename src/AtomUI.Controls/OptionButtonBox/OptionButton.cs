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
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

using AvaloniaRadioButton = Avalonia.Controls.RadioButton;

using ButtonSizeType = SizeType;

public enum OptionButtonStyle
{
   Outline,
   Solid
}

public enum OptionButtonPositionTrait
{
   First,
   Last,
   Middle,
   OnlyOne
}

public class OptionButtonPointerEventArgs : EventArgs
{
   public OptionButton? Button { get; }
   public bool IsPressed { get; set; }
   public bool IsHovering { get; set; }
   public OptionButtonPointerEventArgs(OptionButton button)
   {
      Button = button;
   }
}

public partial class OptionButton : AvaloniaRadioButton, 
                                    ISizeTypeAware,
                                    IWaveAdornerInfoProvider, 
                                    IControlCustomStyle
{
   private bool _initialized = false;
   private ControlStyleState _styleState;
   private IControlCustomStyle _customStyle;
   private StackPanel? _stackPanel;
   private Label? _label;
   private CornerRadius? _originCornerRadius;
   
   public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
      AvaloniaProperty.Register<OptionButton, ButtonSizeType>(nameof(SizeType), ButtonSizeType.Middle);
   
   public static readonly StyledProperty<OptionButtonStyle> ButtonStyleProperty =
      AvaloniaProperty.Register<OptionButton, OptionButtonStyle>(nameof(SizeType), OptionButtonStyle.Outline);
   
   public static readonly StyledProperty<string> TextProperty
      = AvaloniaProperty.Register<OptionButton, string>(nameof(Text), string.Empty);
   
   private static readonly DirectProperty<OptionButton, bool> InOptionGroupProperty =
      AvaloniaProperty.RegisterDirect<OptionButton, bool>(
         nameof(InOptionGroup),
         o => o.InOptionGroup,
         (o, v) => o.InOptionGroup = v);
   
   private static readonly DirectProperty<OptionButton, OptionButtonPositionTrait> GroupPositionTraitProperty =
      AvaloniaProperty.RegisterDirect<OptionButton, OptionButtonPositionTrait>(
         nameof(GroupPositionTrait),
         o => o.GroupPositionTrait,
         (o, v) => o.GroupPositionTrait = v,
         OptionButtonPositionTrait.OnlyOne);
   
   public ButtonSizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public OptionButtonStyle ButtonStyle
   {
      get => GetValue(ButtonStyleProperty);
      set => SetValue(ButtonStyleProperty, value);
   }
   
   public string Text
   {
      get => GetValue(TextProperty);
      set => SetValue(TextProperty, value);
   }

   private bool _inOptionGroup = false;
   
   /// <summary>
   /// 是否在 Group 中渲染
   /// </summary>
   internal bool InOptionGroup
   {
      get => _inOptionGroup;
      set => SetAndRaise(InOptionGroupProperty, ref _inOptionGroup, value);
   }

   private OptionButtonPositionTrait _groupPositionTrait = OptionButtonPositionTrait.OnlyOne;
   internal OptionButtonPositionTrait GroupPositionTrait
   {
      get => _groupPositionTrait;
      set => SetAndRaise(GroupPositionTraitProperty, ref _groupPositionTrait, value);
   }
   
   internal event EventHandler<OptionButtonPointerEventArgs>? OptionButtonPointerEvent;
   
   static OptionButton()
   {
      AffectsMeasure<Button>(SizeTypeProperty, ButtonStyleProperty, InOptionGroupProperty);
      AffectsRender<Button>(IsCheckedProperty, CornerRadiusProperty);
   }
   
   public OptionButton()
   {
      _customStyle = this;
   }
   
   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width;
      var targetHeight = Math.Max(size.Height, _controlHeight);
      return new Size(targetWidth, targetHeight);
   }
   
   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.SetupUi();
         _initialized = true;
      }
   }
   
   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      _customStyle.ApplyRenderScalingAwareStyleConfig();
   }
   
   #region IControlCustomStyle 实现
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
      BindUtils.RelayBind(this, WidthProperty, _stackPanel);
      BindUtils.RelayBind(this, HeightProperty, _stackPanel);
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
      ControlStateUtils.InitCommonState(this, ref _styleState);
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
      BindUtils.CreateTokenBinding(this, MotionDurationTokenProperty, GlobalResourceKey.MotionDurationMid);
      BindUtils.CreateTokenBinding(this, ColorPrimaryHoverTokenProperty, GlobalResourceKey.ColorPrimaryHover);
      BindUtils.CreateTokenBinding(this, ColorPrimaryActiveTokenProperty, GlobalResourceKey.ColorPrimaryActive);
   }

   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      BindUtils.CreateTokenBinding(this, BorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Style,
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
      if (ButtonStyle == OptionButtonStyle.Outline) {
         ApplyOutlineStyle();
      } else if (ButtonStyle == OptionButtonStyle.Solid) {
         ApplySolidStyle();
      }
   }

   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (SizeType == ButtonSizeType.Small) {
         BindUtils.CreateTokenBinding(this, ControlHeightTokenProperty, GlobalResourceKey.ControlHeightSM);
         BindUtils.CreateTokenBinding(this, FontSizeProperty, OptionButtonResourceKey.ContentFontSizeSM);
         BindUtils.CreateTokenBinding(this, PaddingProperty, OptionButtonResourceKey.PaddingSM);
         BindUtils.CreateTokenBinding(this, CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      } else if (SizeType == ButtonSizeType.Middle) {
         BindUtils.CreateTokenBinding(this, ControlHeightTokenProperty, GlobalResourceKey.ControlHeight);
         BindUtils.CreateTokenBinding(this, FontSizeProperty, OptionButtonResourceKey.ContentFontSize);
         BindUtils.CreateTokenBinding(this, PaddingProperty, OptionButtonResourceKey.Padding);
         BindUtils.CreateTokenBinding(this, CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      } else if (SizeType == ButtonSizeType.Large) {
         BindUtils.CreateTokenBinding(this, ControlHeightTokenProperty, GlobalResourceKey.ControlHeightLG);
         BindUtils.CreateTokenBinding(this, FontSizeProperty, OptionButtonResourceKey.ContentFontSizeLG);
         BindUtils.CreateTokenBinding(this, PaddingProperty, OptionButtonResourceKey.PaddingLG);
         BindUtils.CreateTokenBinding(this, CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
      }

      _originCornerRadius = CornerRadius;
      CornerRadius = BuildCornerRadius(GroupPositionTrait, _originCornerRadius!.Value);
   }

   private void ApplySolidStyle()
   {
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         if (_styleState.HasFlag(ControlStyleState.Selected)) {
            BindUtils.CreateTokenBinding(this, ForegroundProperty, OptionButtonResourceKey.ButtonSolidCheckedColor);
            BindUtils.CreateTokenBinding(this, BackgroundProperty, OptionButtonResourceKey.ButtonSolidCheckedBackground);
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               BindUtils.CreateTokenBinding(this, BackgroundProperty,
                                               OptionButtonResourceKey.ButtonSolidCheckedActiveBackground,
                                               BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               BindUtils.CreateTokenBinding(this, BackgroundProperty,
                                               OptionButtonResourceKey.ButtonSolidCheckedHoverBackground,
                                               BindingPriority.StyleTrigger);
            }
         } else {
            BindUtils.CreateTokenBinding(this, ForegroundProperty, GlobalResourceKey.ColorText);
            if (InOptionGroup) {
               BindUtils.CreateTokenBinding(this, BackgroundProperty, GlobalResourceKey.ColorTransparent);
            } else {
               BindUtils.CreateTokenBinding(this, BackgroundProperty, GlobalResourceKey.ColorBgContainer);
            }

            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               BindUtils.CreateTokenBinding(this, ForegroundProperty, GlobalResourceKey.ColorPrimaryActive,
                                               BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               BindUtils.CreateTokenBinding(this, ForegroundProperty, GlobalResourceKey.ColorPrimaryHover,
                                               BindingPriority.StyleTrigger);
            }
         }
      } else {
         BindUtils.CreateTokenBinding(this, BorderBrushProperty, GlobalResourceKey.ColorBorder);
         if (_styleState.HasFlag(ControlStyleState.Selected)) {
            BindUtils.CreateTokenBinding(this, ForegroundProperty, OptionButtonResourceKey.ButtonCheckedColorDisabled);
            BindUtils.CreateTokenBinding(this, BackgroundProperty, OptionButtonResourceKey.ButtonCheckedBgDisabled);
         } else {
            BindUtils.CreateTokenBinding(this, ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
            BindUtils.CreateTokenBinding(this, BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
         }
      }
   }

   private void ApplyOutlineStyle()
   {
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         if (_styleState.HasFlag(ControlStyleState.Selected)) {
            if (InOptionGroup) {
               BindUtils.CreateTokenBinding(this, BackgroundProperty, GlobalResourceKey.ColorTransparent);
            } else {
               BindUtils.CreateTokenBinding(this, BackgroundProperty, OptionButtonResourceKey.ButtonBackground);
            }

            BindUtils.CreateTokenBinding(this, BorderBrushProperty, GlobalResourceKey.ColorPrimary);
            BindUtils.CreateTokenBinding(this, ForegroundProperty, GlobalResourceKey.ColorPrimary);
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               BindUtils.CreateTokenBinding(this, BorderBrushProperty, GlobalResourceKey.ColorPrimaryActive,
                                               BindingPriority.StyleTrigger);
               BindUtils.CreateTokenBinding(this, ForegroundProperty, GlobalResourceKey.ColorPrimaryActive,
                                               BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               BindUtils.CreateTokenBinding(this, BorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                                               BindingPriority.StyleTrigger);
               BindUtils.CreateTokenBinding(this, ForegroundProperty, GlobalResourceKey.ColorPrimaryHover,
                                               BindingPriority.StyleTrigger);
            }
         } else {
            if (InOptionGroup) {
               BindUtils.CreateTokenBinding(this, BackgroundProperty, GlobalResourceKey.ColorTransparent);
            } else {
               BindUtils.CreateTokenBinding(this, BackgroundProperty,
                                               OptionButtonResourceKey.ButtonCheckedBackground);
            }

            BindUtils.CreateTokenBinding(this, BorderBrushProperty, GlobalResourceKey.ColorBorder);
            BindUtils.CreateTokenBinding(this, ForegroundProperty, OptionButtonResourceKey.ButtonColor);
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               BindUtils.CreateTokenBinding(this, BorderBrushProperty, GlobalResourceKey.ColorPrimaryActive,
                                               BindingPriority.StyleTrigger);
               BindUtils.CreateTokenBinding(this, ForegroundProperty, GlobalResourceKey.ColorPrimaryActive,
                                               BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               BindUtils.CreateTokenBinding(this, BorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                                               BindingPriority.StyleTrigger);
               BindUtils.CreateTokenBinding(this, ForegroundProperty, GlobalResourceKey.ColorPrimaryHover,
                                               BindingPriority.StyleTrigger);
            }
         }
      } else {
         BindUtils.CreateTokenBinding(this, BorderBrushProperty, GlobalResourceKey.ColorBorder);
         if (_styleState.HasFlag(ControlStyleState.Selected)) {
            BindUtils.CreateTokenBinding(this, ForegroundProperty,
                                            OptionButtonResourceKey.ButtonCheckedColorDisabled);
            BindUtils.CreateTokenBinding(this, BackgroundProperty, OptionButtonResourceKey.ButtonCheckedBgDisabled);
         } else {
            BindUtils.CreateTokenBinding(this, ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
            BindUtils.CreateTokenBinding(this, BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
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
   #endregion

}