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
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace AtomUI.Controls;

using AvaloniaButton = Avalonia.Controls.Button;
using ButtonSizeType = SizeType;

public enum ButtonType
{
   Default,
   Primary,
   Link,
   Text
}

public enum ButtonShape
{
   Default,
   Circle,
   Round,
}

public partial class Button : AvaloniaButton, 
                              ISizeTypeAware, 
                              IControlCustomStyle,
                              IWaveAdornerInfoProvider
{
   #region 公共属性定义
   public static readonly StyledProperty<ButtonType> ButtonTypeProperty =
      AvaloniaProperty.Register<Button, ButtonType>(nameof(ButtonType), ButtonType.Default);

   public static readonly StyledProperty<ButtonShape> ButtonShapeProperty =
      AvaloniaProperty.Register<Button, ButtonShape>(nameof(Shape), ButtonShape.Default);

   public static readonly StyledProperty<bool> IsDangerProperty =
      AvaloniaProperty.Register<Button, bool>(nameof(IsDanger), false);

   public static readonly StyledProperty<bool> IsGhostProperty =
      AvaloniaProperty.Register<Button, bool>(nameof(IsGhost), false);

   public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
      AvaloniaProperty.Register<Button, ButtonSizeType>(nameof(SizeType), ButtonSizeType.Middle);

   public static readonly StyledProperty<PathIcon?> IconProperty
      = AvaloniaProperty.Register<Button, PathIcon?>(nameof(Icon));

   public static readonly StyledProperty<string?> TextProperty
      = AvaloniaProperty.Register<Button, string?>(nameof(Text));

   public ButtonType ButtonType
   {
      get => GetValue(ButtonTypeProperty);
      set => SetValue(ButtonTypeProperty, value);
   }

   public ButtonShape Shape
   {
      get => GetValue(ButtonShapeProperty);
      set => SetValue(ButtonShapeProperty, value);
   }
   
   public bool IsDanger
   {
      get => GetValue(IsDangerProperty);
      set => SetValue(IsDangerProperty, value);
   }
   
   public bool IsGhost
   {
      get => GetValue(IsGhostProperty);
      set => SetValue(IsGhostProperty, value);
   }
   
   public ButtonSizeType SizeType
   {
      get => GetValue(SizeTypeProperty);
      set => SetValue(SizeTypeProperty, value);
   }
   
   public PathIcon? Icon
   {
      get => GetValue(IconProperty);
      set => SetValue(IconProperty, value);
   }
   
   public string? Text
   {
      get => GetValue(TextProperty);
      set => SetValue(TextProperty, value);
   }

   #endregion
   
   private ControlStyleState _styleState;
   private IControlCustomStyle _customStyle;
   private StackPanel? _stackPanel;
   private Label? _label;
   private bool _initialized = false;
   private BorderRenderHelper _borderRenderHelper;

   static Button()
   {
      AffectsMeasure<Button>(SizeTypeProperty,
                             ButtonShapeProperty,
                             IconProperty,
                             WidthProperty,
                             HeightProperty,
                             PaddingProperty);
      AffectsRender<Button>(ButtonTypeProperty,
                            IsDangerProperty,
                            IsGhostProperty,
                            BackgroundProperty,
                            ForegroundProperty);
   }

   public Button()
   {
      _customStyle = this;
      _borderRenderHelper = new BorderRenderHelper();
   }

   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      var targetWidth = size.Width;
      var targetHeight = size.Height;
      targetWidth += Padding.Left + Padding.Right;
      targetHeight += Padding.Top + Padding.Bottom;
      targetHeight = Math.Max(targetHeight, ControlHeight);
      if (Shape == ButtonShape.Circle) {
         targetWidth = targetHeight;
         CornerRadius = new CornerRadius(targetHeight);
      } else if (Shape == ButtonShape.Round) {
         CornerRadius = new CornerRadius(targetHeight);
      }
 
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
    void IControlCustomStyle.SetupUi()
   {
      SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Left, BindingPriority.Template);
      SetValue(VerticalAlignmentProperty, VerticalAlignment.Bottom, BindingPriority.Template);
      SetupControlTheme();
      Cursor = new Cursor(StandardCursorType.Hand);
      if (Text is null && Content is string content) {
         Text = content;
         Content = null;
      }
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

   private void SetupControlTheme()
   {
      if (ButtonType == ButtonType.Default) {
         BindUtils.CreateTokenBinding(this, ThemeProperty, DefaultButtonTheme.ID);
      } else if (ButtonType == ButtonType.Primary) {
         BindUtils.CreateTokenBinding(this, ThemeProperty, PrimaryButtonTheme.ID);
      } else if (ButtonType == ButtonType.Text) {
         BindUtils.CreateTokenBinding(this, ThemeProperty, TextButtonTheme.ID);
      } else if (ButtonType == ButtonType.Link) {
         BindUtils.CreateTokenBinding(this, ThemeProperty, LinkButtonTheme.ID);
      }
   }

   void ApplyShapeStyleConfig()
   {
      if (Shape == ButtonShape.Circle) {
         BindUtils.CreateTokenBinding(this, PaddingProperty, ButtonResourceKey.CirclePadding);
      }
   }

   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      BindUtils.CreateTokenBinding(this, PaddingXXSTokenProperty, GlobalResourceKey.PaddingXXS);
      BindUtils.CreateTokenBinding(this, DefaultShadowTokenProperty, ButtonResourceKey.DefaultShadow);
      BindUtils.CreateTokenBinding(this, PrimaryShadowTokenProperty, ButtonResourceKey.PrimaryShadow);
      BindUtils.CreateTokenBinding(this, DangerShadowTokenProperty, ButtonResourceKey.DangerShadow);
   }

   void IControlCustomStyle.CollectStyleState()
   {
      ControlStateUtils.InitCommonState(this, ref _styleState);
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
         BindUtils.CreateTokenBinding(this, BorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Style,
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
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.HandleTemplateApplied(e.NameScope);
      if (Transitions is null) {
         _customStyle.SetupTransitions();
      }
   }

   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _label = scope.Find<Label>(BaseButtonTheme.LabelPart);
      _stackPanel = scope.Find<StackPanel>(BaseButtonTheme.StackPanelPart);
      
      _customStyle.CollectStyleState();
      _customStyle.ApplyFixedStyleConfig();
      ApplyShapeStyleConfig();
      SetupIcon();
      ApplyIconModeStyleConfig();
   }
   
   // TODO 针对 primary 的是否是 ghost 没有完成
   private void SetupIcon()
   {
      if (Icon is not null) {
         if (Text is not null) {
            if (SizeType == SizeType.Small) {
               BindUtils.CreateTokenBinding(Icon, WidthProperty, GlobalResourceKey.IconSizeSM);
               BindUtils.CreateTokenBinding(Icon, HeightProperty, GlobalResourceKey.IconSizeSM);
            } else if (SizeType == SizeType.Middle) {
               BindUtils.CreateTokenBinding(Icon, WidthProperty, GlobalResourceKey.IconSize);
               BindUtils.CreateTokenBinding(Icon, HeightProperty, GlobalResourceKey.IconSize);
            } else if (SizeType == SizeType.Large) {
               BindUtils.CreateTokenBinding(Icon, WidthProperty, GlobalResourceKey.IconSizeLG);
               BindUtils.CreateTokenBinding(Icon, HeightProperty, GlobalResourceKey.IconSizeLG);
            }

            Icon.Margin = new Thickness(0, 0, _paddingXXS, 0);
         } else {
            if (SizeType == SizeType.Small) {
               BindUtils.CreateTokenBinding(Icon, WidthProperty, ButtonResourceKey.OnlyIconSizeSM);
               BindUtils.CreateTokenBinding(Icon, HeightProperty, ButtonResourceKey.OnlyIconSizeSM);
            } else if (SizeType == SizeType.Middle) {
               BindUtils.CreateTokenBinding(Icon, WidthProperty, ButtonResourceKey.OnlyIconSize);
               BindUtils.CreateTokenBinding(Icon, HeightProperty, ButtonResourceKey.OnlyIconSize);
            } else if (SizeType == SizeType.Large) {
               BindUtils.CreateTokenBinding(Icon, WidthProperty, ButtonResourceKey.OnlyIconSizeLG);
               BindUtils.CreateTokenBinding(Icon, HeightProperty, ButtonResourceKey.OnlyIconSizeLG);
            }
         }

         if (ButtonType == ButtonType.Primary) {
            SetupPrimaryIconStyle();
         } else if (ButtonType == ButtonType.Default || ButtonType == ButtonType.Link) {
            SetupDefaultOrLinkIconStyle();
         }
         
         _stackPanel!.Children.Insert(0, Icon);
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
            BindUtils.CreateTokenBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorError);
            BindUtils.CreateTokenBinding(Icon, PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorErrorActive);
            BindUtils.CreateTokenBinding(Icon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
         } else {
            BindUtils.CreateTokenBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorPrimary);
            BindUtils.CreateTokenBinding(Icon, PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorPrimaryActive);
            BindUtils.CreateTokenBinding(Icon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorPrimaryHover);
         }
      } else {
         BindUtils.CreateTokenBinding(Icon, PathIcon.NormalFillBrushProperty, ButtonResourceKey.PrimaryColor);
      }
      BindUtils.CreateTokenBinding(Icon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ColorTextDisabled);
   }

   private void SetupDefaultOrLinkIconStyle()
   {
      if (Icon is null || Icon.ThemeType == IconThemeType.TwoTone) {
         return;
      }
      
      if (IsDanger) {
         BindUtils.CreateTokenBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorError);
         BindUtils.CreateTokenBinding(Icon, PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorErrorActive);
         BindUtils.CreateTokenBinding(Icon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorErrorBorderHover);
      } else {
         if (IsGhost) {
            BindUtils.CreateTokenBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorTextLightSolid);
            BindUtils.CreateTokenBinding(Icon, PathIcon.SelectedFilledBrushProperty, GlobalResourceKey.ColorPrimaryActive);
            BindUtils.CreateTokenBinding(Icon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorPrimaryHover);
         } else {
            if (ButtonType == ButtonType.Link) {
               BindUtils.CreateTokenBinding(Icon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorLink);
            } else {
               BindUtils.CreateTokenBinding(Icon, PathIcon.NormalFillBrushProperty, ButtonResourceKey.DefaultColor);
            }
            
            BindUtils.CreateTokenBinding(Icon, PathIcon.SelectedFilledBrushProperty, ButtonResourceKey.DefaultActiveColor);
            BindUtils.CreateTokenBinding(Icon, PathIcon.ActiveFilledBrushProperty, ButtonResourceKey.DefaultHoverColor);
         }
      }
      BindUtils.CreateTokenBinding(Icon, PathIcon.DisabledFilledBrushProperty, GlobalResourceKey.ColorTextDisabled);
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
         if (VisualRoot is not null) {
            _customStyle.ApplySizeTypeStyleConfig();
            SetupControlTheme();
         }
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

   public override void Render(DrawingContext context)
   {
      _borderRenderHelper.Render(context,
                                 Bounds.Size, 
                                 BorderThickness, 
                                 CornerRadius, 
                                 BackgroundSizing, 
                                 Background, 
                                 BorderBrush,
                                 default);
   }

   #endregion
}