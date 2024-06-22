using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Media;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class Button : IWaveAdornerInfoProvider, IControlCustomStyle
{
   private ControlStyleState _styleState;
   private TokenResourceBinder _tokenResourceBinder;
   private IControlCustomStyle _customStyle;
   private StackPanel? _stackPanel;
   private Label? _label;
   private bool _initialized = false;

   void IControlCustomStyle.SetupUi()
   {
      HorizontalAlignment = HorizontalAlignment.Left;
      VerticalAlignment = VerticalAlignment.Bottom;
      Cursor = new Cursor(StandardCursorType.Hand);
      _customStyle.CollectStyleState();
      CreateMainLayout();
      _customStyle.ApplySizeTypeStyleConfig();
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyVariableStyleConfig();
      ApplyShapeStyleConfig();
      SetupIcon();
      ApplyIconModeStyleConfig();
      _customStyle.SetupTransitions();

      if (ButtonType == ButtonType.Default) {
         if (IsDanger) {
            Effect = new DropShadowEffect
            {
               OffsetX = _dangerShadow.OffsetX,
               OffsetY = _dangerShadow.OffsetY,
               Color = _dangerShadow.Color,
               BlurRadius = _dangerShadow.Blur,
            };
         } else {
            Effect = new DropShadowEffect
            {
               OffsetX = _defaultShadow.OffsetX,
               OffsetY = _defaultShadow.OffsetY,
               Color = _defaultShadow.Color,
               BlurRadius = _defaultShadow.Blur,
            };
         }
      } else if (ButtonType == ButtonType.Primary) {
         if (IsDanger) {
            Effect = new DropShadowEffect
            {
               OffsetX = _dangerShadow.OffsetX,
               OffsetY = _dangerShadow.OffsetY,
               Color = _dangerShadow.Color,
               BlurRadius = _dangerShadow.Blur,
            };
         } else {
            Effect = new DropShadowEffect
            {
               OffsetX = _primaryShadow.OffsetX,
               OffsetY = _primaryShadow.OffsetY,
               Color = _primaryShadow.Color,
               BlurRadius = _primaryShadow.Blur,
            };
         }
      }
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

   void IControlCustomStyle.ApplyVariableStyleConfig()
   {
      _tokenResourceBinder.ReleaseTriggerBindings(this);
      if (ButtonType == ButtonType.Primary) {
         ApplyPrimaryStyle();
      } else if (ButtonType == ButtonType.Default) {
         ApplyDefaultStyle();
      } else if (ButtonType == ButtonType.Text) {
         ApplyTextStyle();
      } else if (ButtonType == ButtonType.Link) {
         ApplyLinkStyle();
      }
   }

   void ApplyShapeStyleConfig()
   {
      if (Shape == ButtonShape.Circle) {
         _tokenResourceBinder.AddBinding(PaddingProperty, "CirclePadding");
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      _tokenResourceBinder.AddBinding(PaddingXXSTokenProperty, "PaddingXXS");
      _tokenResourceBinder.AddBinding(DefaultShadowTokenProperty, "DefaultShadow");
      _tokenResourceBinder.AddBinding(PrimaryShadowTokenProperty, "PrimaryShadow");
      _tokenResourceBinder.AddBinding(DangerShadowTokenProperty, "DangerShadow");
   }

   void IControlCustomStyle.CollectStyleState()
   {
      StyleUtils.InitCommonState(this, ref _styleState);
      if (IsPressed) {
         _styleState |= ControlStyleState.Sunken;
      } else {
         _styleState |= ControlStyleState.Raised;
      }
   }

   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      if (ButtonType == ButtonType.Default || 
          (ButtonType == ButtonType.Primary && (IsGhost || !IsEnabled))) {
         _tokenResourceBinder.AddBinding(BorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Style,
                                         new RenderScaleAwareThicknessConfigure(this));
      }
   }

   void IControlCustomStyle.SetupTransitions()
   {
      var transitions = new Transitions();
      if (ButtonType == ButtonType.Primary) {
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
         if (IsGhost) {
            transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
            transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
         }
      } else if (ButtonType == ButtonType.Default) {
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BorderBrushProperty));
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
      } else if (ButtonType == ButtonType.Text) {
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(BackgroundProperty));
      } else if (ButtonType == ButtonType.Link) {
         transitions.Add(AnimationUtils.CreateTransition<SolidColorBrushTransition>(ForegroundProperty));
      }

      Transitions = transitions;
   }

   void IControlCustomStyle.ApplySizeTypeStyleConfig()
   {
      if (SizeType == SizeType.Small) {
         _tokenResourceBinder.AddBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeightSM);
         _tokenResourceBinder.AddBinding(FontSizeProperty, ButtonResourceKey.ContentFontSizeSM);
         _tokenResourceBinder.AddBinding(PaddingProperty, ButtonResourceKey.PaddingSM);
         _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusSM);
      } else if (SizeType == SizeType.Middle) {
         _tokenResourceBinder.AddBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeight);
         _tokenResourceBinder.AddBinding(FontSizeProperty, ButtonResourceKey.ContentFontSize);
         _tokenResourceBinder.AddBinding(PaddingProperty, GlobalResourceKey.Padding);
         _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadius);
      } else if (SizeType == SizeType.Large) {
         _tokenResourceBinder.AddBinding(ControlHeightTokenProperty, GlobalResourceKey.ControlHeightLG);
         _tokenResourceBinder.AddBinding(FontSizeProperty, ButtonResourceKey.ContentFontSizeLG);
         _tokenResourceBinder.AddBinding(PaddingProperty, ButtonResourceKey.PaddingLG);
         _tokenResourceBinder.AddBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
      }

      if (Icon is not null) {
         if (SizeType == SizeType.Small) {
            _tokenResourceBinder.AddBinding(Icon, WidthProperty, ButtonResourceKey.IconSizeSM);
            _tokenResourceBinder.AddBinding(Icon, HeightProperty, ButtonResourceKey.IconSizeSM);
         } else if (SizeType == SizeType.Middle) {
            _tokenResourceBinder.AddBinding(Icon, WidthProperty, ButtonResourceKey.IconSize);
            _tokenResourceBinder.AddBinding(Icon, HeightProperty, ButtonResourceKey.IconSize);
         } else if (SizeType == SizeType.Large) {
            _tokenResourceBinder.AddBinding(Icon, WidthProperty, ButtonResourceKey.IconSizeLG);
            _tokenResourceBinder.AddBinding(Icon, HeightProperty, ButtonResourceKey.IconSizeLG);
         }
      }
   }

   private void ApplyPrimaryStyle()
   {
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         // IsGhost 优先级最高
         if (IsGhost) {
            Background = new SolidColorBrush(Colors.Transparent);
            if (IsDanger) {
               _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorError);
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorError);

               if (_styleState.HasFlag(ControlStyleState.Sunken)) {
                  _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorErrorActive,
                     BindingPriority.StyleTrigger);
                  _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorErrorActive,
                     BindingPriority.StyleTrigger);
               } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
                  _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorErrorBorderHover,
                     BindingPriority.StyleTrigger);
                  _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorErrorBorderHover,
                     BindingPriority.StyleTrigger);
               }
            } else {
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorPrimary);
               _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorPrimary);

               if (_styleState.HasFlag(ControlStyleState.Sunken)) {
                  _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorPrimaryActive,
                     BindingPriority.StyleTrigger);
                  _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorPrimaryActive,
                     BindingPriority.StyleTrigger);
               } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
                  _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorPrimaryHover,
                     BindingPriority.StyleTrigger);
                  _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                     BindingPriority.StyleTrigger);
               }
            }
         } else {
            _tokenResourceBinder.AddBinding(ForegroundProperty, ButtonResourceKey.PrimaryColor);
            if (IsDanger) {
               _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorError);
               if (_styleState.HasFlag(ControlStyleState.Sunken)) {
                  _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorErrorActive,
                     BindingPriority.StyleTrigger);
               } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
                  _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorErrorHover,
                     BindingPriority.StyleTrigger);
               }
            } else {
               _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorPrimary);
               if (_styleState.HasFlag(ControlStyleState.Sunken)) {
                  _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorPrimaryActive,
                     BindingPriority.StyleTrigger);
               } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
                  _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorPrimaryHover,
                     BindingPriority.StyleTrigger);
               }
            }
         }
      } else {
         _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
         _tokenResourceBinder.AddBinding(BorderBrushProperty, ButtonResourceKey.BorderColorDisabled);
         _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      }
   }

   private void ApplyDefaultStyle()
   {
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         _tokenResourceBinder.AddBinding(BackgroundProperty, ButtonResourceKey.DefaultBg);
         if (IsDanger) {
            _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorError);
            _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorError);

            if (IsGhost) {
               _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorTransparent);
            }

            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorErrorActive,
                  BindingPriority.StyleTrigger);
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorErrorActive,
                  BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorErrorBorderHover,
                  BindingPriority.StyleTrigger);
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorErrorBorderHover,
                  BindingPriority.StyleTrigger);
            }
         } else {
            if (IsGhost) {
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorTextLightSolid);
               _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorTextLightSolid);
               _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorTransparent);
               
               if (_styleState.HasFlag(ControlStyleState.Sunken)) {
                  _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorPrimaryActive,
                     BindingPriority.StyleTrigger);
                  _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorPrimaryActive,
                     BindingPriority.StyleTrigger);
               } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
                  _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorPrimaryHover,
                     BindingPriority.StyleTrigger);
                  _tokenResourceBinder.AddBinding(BorderBrushProperty, GlobalResourceKey.ColorPrimaryHover,
                     BindingPriority.StyleTrigger);
               }
            } else {
               _tokenResourceBinder.AddBinding(BorderBrushProperty, ButtonResourceKey.DefaultBorderColor);
               _tokenResourceBinder.AddBinding(ForegroundProperty, ButtonResourceKey.DefaultColor);
               if (_styleState.HasFlag(ControlStyleState.Sunken)) {
                  _tokenResourceBinder.AddBinding(BorderBrushProperty, ButtonResourceKey.DefaultActiveBorderColor,
                     BindingPriority.StyleTrigger);
                  _tokenResourceBinder.AddBinding(ForegroundProperty, ButtonResourceKey.DefaultActiveColor,
                     BindingPriority.StyleTrigger);
               } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
                  _tokenResourceBinder.AddBinding(BorderBrushProperty, ButtonResourceKey.DefaultHoverBorderColor,
                     BindingPriority.StyleTrigger);
                  _tokenResourceBinder.AddBinding(ForegroundProperty, ButtonResourceKey.DefaultHoverColor,
                     BindingPriority.StyleTrigger);
               }
            }
         }
      } else {
         _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
         _tokenResourceBinder.AddBinding(BorderBrushProperty, ButtonResourceKey.BorderColorDisabled);
         _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorBgContainerDisabled);
      }
   }

   private void ApplyTextStyle()
   {
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorTransparent);

         if (IsDanger) {
            _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorError);
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorErrorBgActive,
                  BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorErrorBgHover,
                  BindingPriority.StyleTrigger);
            }
         } else {
            _tokenResourceBinder.AddBinding(ForegroundProperty, ButtonResourceKey.DefaultColor);
            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorBgTextActive,
                  BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _tokenResourceBinder.AddBinding(BackgroundProperty, ButtonResourceKey.TextHoverBg,
                  BindingPriority.StyleTrigger);
            }
         }
      } else {
         _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      }
   }

   private void ApplyLinkStyle()
   {
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         if (IsGhost) {
            _tokenResourceBinder.AddBinding(BackgroundProperty, GlobalResourceKey.ColorTransparent);
         } else {
            _tokenResourceBinder.AddBinding(BackgroundProperty, ButtonResourceKey.DefaultBg);
         }

         if (IsDanger) {
            _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorError);

            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorErrorActive,
                  BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorErrorHover,
                  BindingPriority.StyleTrigger);
            }
         } else {
            _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorLink);

            if (_styleState.HasFlag(ControlStyleState.Sunken)) {
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorLinkActive,
                  BindingPriority.StyleTrigger);
            } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
               _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorLinkHover,
                  BindingPriority.StyleTrigger);
            }
         }
      } else {
         _tokenResourceBinder.AddBinding(ForegroundProperty, GlobalResourceKey.ColorTextDisabled);
      }
   }

   // TODO 针对 primary 的是否是 ghost 没有完成
   private void SetupIcon()
   {
      if (Icon is not null) {
         _stackPanel!.Children.Insert(0, Icon);
         if (Text.Length != 0) {
            if (SizeType == SizeType.Small) {
               _tokenResourceBinder.AddBinding(Icon, WidthProperty, GlobalResourceKey.IconSizeSM);
               _tokenResourceBinder.AddBinding(Icon, HeightProperty, GlobalResourceKey.IconSizeSM);
            } else if (SizeType == SizeType.Middle) {
               _tokenResourceBinder.AddBinding(Icon, WidthProperty, GlobalResourceKey.IconSize);
               _tokenResourceBinder.AddBinding(Icon, HeightProperty, GlobalResourceKey.IconSize);
            } else if (SizeType == SizeType.Large) {
               _tokenResourceBinder.AddBinding(Icon, WidthProperty, GlobalResourceKey.IconSizeLG);
               _tokenResourceBinder.AddBinding(Icon, HeightProperty, GlobalResourceKey.IconSizeLG);
            }

            Icon.Margin = new Thickness(0, 0, _paddingXXS, 0);
         } else {
            if (SizeType == SizeType.Small) {
               _tokenResourceBinder.AddBinding(Icon, WidthProperty, ButtonResourceKey.OnlyIconSizeSM);
               _tokenResourceBinder.AddBinding(Icon, HeightProperty, ButtonResourceKey.OnlyIconSizeSM);
            } else if (SizeType == SizeType.Middle) {
               _tokenResourceBinder.AddBinding(Icon, WidthProperty, ButtonResourceKey.OnlyIconSize);
               _tokenResourceBinder.AddBinding(Icon, HeightProperty, ButtonResourceKey.OnlyIconSize);
            } else if (SizeType == SizeType.Large) {
               _tokenResourceBinder.AddBinding(Icon, WidthProperty, ButtonResourceKey.OnlyIconSizeLG);
               _tokenResourceBinder.AddBinding(Icon, HeightProperty, ButtonResourceKey.OnlyIconSizeLG);
            }
         }

         if (ButtonType == ButtonType.Primary) {
            SetupPrimaryIconStyle();
         } else if (ButtonType == ButtonType.Default || ButtonType == ButtonType.Link) {
            SetupDefaultOrLinkIconStyle();
         }
      }
   }
   
   private void SetupPrimaryIconStyle()
   {
      if (Icon is null || Icon.ThemeType == IconThemeType.TwoTone) {
         return;
      }
      // IsGhost 优先级最高
      if (IsGhost) {
         if (IsDanger) {
            _tokenResourceBinder.AddBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorError);
            _tokenResourceBinder.AddBinding(Icon, PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorErrorActive);
            _tokenResourceBinder.AddBinding(Icon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
         } else {
            _tokenResourceBinder.AddBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorPrimary);
            _tokenResourceBinder.AddBinding(Icon, PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorPrimaryActive);
            _tokenResourceBinder.AddBinding(Icon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorPrimaryHover);
         }
      } else {
         _tokenResourceBinder.AddBinding(Icon, PathIcon.NormalFillBrushProperty, ButtonResourceKey.PrimaryColor);
      }
      _tokenResourceBinder.AddBinding(Icon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ColorTextDisabled);
   }

   private void SetupDefaultOrLinkIconStyle()
   {
      if (Icon is null || Icon.ThemeType == IconThemeType.TwoTone) {
         return;
      }
      
      if (IsDanger) {
         _tokenResourceBinder.AddBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorError);
         _tokenResourceBinder.AddBinding(Icon, PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorErrorActive);
         _tokenResourceBinder.AddBinding(Icon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
      } else {
         if (IsGhost) {
            _tokenResourceBinder.AddBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorTextLightSolid);
            _tokenResourceBinder.AddBinding(Icon, PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorPrimaryActive);
            _tokenResourceBinder.AddBinding(Icon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorPrimaryHover);
         } else {
            if (ButtonType == ButtonType.Link) {
               _tokenResourceBinder.AddBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorLink);
            } else {
               _tokenResourceBinder.AddBinding(Icon, PathIcon.NormalFillBrushProperty, ButtonResourceKey.DefaultColor);
            }
            
            _tokenResourceBinder.AddBinding(Icon, PathIcon.SelectedFilledBrushProperty, ButtonResourceKey.DefaultActiveColor);
            _tokenResourceBinder.AddBinding(Icon, PathIcon.ActiveFilledBrushProperty, ButtonResourceKey.DefaultHoverColor);
         }
      }
      _tokenResourceBinder.AddBinding(Icon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ColorTextDisabled);
   }

   private void ApplyIconModeStyleConfig()
   {
      if (Icon is null) {
         return;
      }
      if (_styleState.HasFlag(ControlStyleState.Enabled)) {
         if (_styleState.HasFlag(ControlStyleState.Sunken)) {
            Icon.IconMode = IconMode.Selected;
         } else if (_styleState.HasFlag(ControlStyleState.MouseOver)) {
            Icon.IconMode = IconMode.Active;
         } else {
            Icon.IconMode = IconMode.Normal;
         }
      } else {
         Icon.IconMode = IconMode.Disabled;
      }
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (e.Property == IsPointerOverProperty ||
          e.Property == IsPressedProperty ||
          e.Property == IsEnabledProperty) {
         _customStyle.CollectStyleState();
         _customStyle.ApplyVariableStyleConfig();
         ApplyIconModeStyleConfig();
         if (e.Property == IsPressedProperty) {
            if (_styleState.HasFlag(ControlStyleState.Raised) && (ButtonType == ButtonType.Primary ||
                                                                  ButtonType == ButtonType.Default)) {
               WaveType waveType = default;
               if (Shape == ButtonShape.Default) {
                  waveType = WaveType.RoundRectWave;
               } else if (Shape == ButtonShape.Round) {
                  waveType = WaveType.PillWave;
               } else if (Shape == ButtonShape.Circle) {
                  waveType = WaveType.CircleWave;
               }

               Color? waveColor = null;
               if (IsDanger) {
                  if (ButtonType == ButtonType.Primary && !IsGhost) {
                     waveColor = Color.Parse(Background?.ToString()!);
                  } else {
                     waveColor = Color.Parse(Foreground?.ToString()!);
                  }
               }

               WaveSpiritAdorner.ShowWaveAdorner(this, waveType, waveColor);
            }
         }
      }

      if (e.Property == SizeTypeProperty) {
         _customStyle.ApplySizeTypeStyleConfig();
      }

      if (_initialized && e.Property == IconProperty) {
         var oldValue = e.GetOldValue<PathIcon?>();
         var newValue = e.GetNewValue<PathIcon?>();
         if (oldValue is not null) {
            _stackPanel!.Children.Remove(oldValue);
         }

         if (newValue is not null) {
            SetupIcon();
         }
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
   }

   public Rect WaveGeometry()
   {
      return new Rect(0, 0, Bounds.Width, Bounds.Height);
   }

   public CornerRadius WaveBorderRadius()
   {
      return CornerRadius;
   }
}