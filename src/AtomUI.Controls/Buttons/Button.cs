using AtomUI.Controls.Utils;
using AtomUI.Data;
using AtomUI.Icon;
using AtomUI.Media;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using AtomUI.Theme.Utils;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
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
// TODO 目前不能动态切换 ButtonType

[PseudoClasses(IconOnlyPC, LoadingPC)]
public class Button : AvaloniaButton, 
                      ISizeTypeAware,
                      IControlCustomStyle, 
                      IWaveAdornerInfoProvider
{
   public const string IconOnlyPC = ":icononly";
   public const string LoadingPC  = ":loading";
   
   #region 公共属性定义

   public static readonly StyledProperty<ButtonType> ButtonTypeProperty =
      AvaloniaProperty.Register<Button, ButtonType>(nameof(ButtonType), ButtonType.Default);

   public static readonly StyledProperty<ButtonShape> ButtonShapeProperty =
      AvaloniaProperty.Register<Button, ButtonShape>(nameof(Shape), ButtonShape.Default);

   public static readonly StyledProperty<bool> IsDangerProperty =
      AvaloniaProperty.Register<Button, bool>(nameof(IsDanger), false);

   public static readonly StyledProperty<bool> IsGhostProperty =
      AvaloniaProperty.Register<Button, bool>(nameof(IsGhost), false);
   
   public static readonly StyledProperty<bool> IsLoadingProperty =
      AvaloniaProperty.Register<Button, bool>(nameof(IsLoading), false);

   public static readonly StyledProperty<ButtonSizeType> SizeTypeProperty =
      AvaloniaProperty.Register<Button, ButtonSizeType>(nameof(SizeType), ButtonSizeType.Middle);

   public static readonly StyledProperty<PathIcon?> IconProperty
      = AvaloniaProperty.Register<Button, PathIcon?>(nameof(Icon));

   public static readonly StyledProperty<string?> TextProperty
      = AvaloniaProperty.Register<Button, string?>(nameof(Text));
   
   public static readonly StyledProperty<bool> IsIconVisibleProperty
      = AvaloniaProperty.Register<Button, bool>(nameof (IsIconVisible), true);

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

   public bool IsLoading
   {
      get => GetValue(IsLoadingProperty);
      set => SetValue(IsLoadingProperty, value);
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
   
   public bool IsIconVisible
   {
      get => GetValue(IsIconVisibleProperty);
      set => SetValue(IsIconVisibleProperty, value);
   }

   #endregion

   #region 内部属性定义

   internal static readonly StyledProperty<double> ControlHeightTokenProperty =
      AvaloniaProperty.Register<Button, double>(
         nameof(ControlHeight));

   internal static readonly StyledProperty<Thickness> IconMarginProperty =
      AvaloniaProperty.Register<Button, Thickness>(
         nameof(IconMargin));
   
   internal static readonly StyledProperty<double> IconSizeProperty =
      AvaloniaProperty.Register<Button, double>(
         nameof(IconSize));

   internal static readonly StyledProperty<BoxShadow> DefaultShadowProperty =
      AvaloniaProperty.Register<Button, BoxShadow>(
         nameof(DefaultShadow));

   internal static readonly StyledProperty<BoxShadow> PrimaryShadowProperty =
      AvaloniaProperty.Register<Button, BoxShadow>(
         nameof(PrimaryShadow));

   internal static readonly StyledProperty<BoxShadow> DangerShadowProperty =
      AvaloniaProperty.Register<Button, BoxShadow>(
         nameof(DangerShadow));
   
   public static readonly StyledProperty<object?> RightExtraContentProperty =
      AvaloniaProperty.Register<Button, object?>(nameof(RightExtraContent));
   
   internal double ControlHeight
   {
      get => GetValue(ControlHeightTokenProperty);
      set => SetValue(ControlHeightTokenProperty, value);
   }

   internal Thickness IconMargin
   {
      get => GetValue(IconMarginProperty);
      set => SetValue(IconMarginProperty, value);
   }
   
   internal double IconSize
   {
      get => GetValue(IconSizeProperty);
      set => SetValue(IconSizeProperty, value);
   }

   internal BoxShadow DefaultShadow
   {
      get => GetValue(DefaultShadowProperty);
      set => SetValue(DefaultShadowProperty, value);
   }

   internal BoxShadow PrimaryShadow
   {
      get => GetValue(PrimaryShadowProperty);
      set => SetValue(PrimaryShadowProperty, value);
   }

   internal BoxShadow DangerShadow
   {
      get => GetValue(DangerShadowProperty);
      set => SetValue(DangerShadowProperty, value);
   }
   
   public object? RightExtraContent
   {
      get => GetValue(RightExtraContentProperty);
      set => SetValue(RightExtraContentProperty, value);
   }
   
   #endregion

   protected ControlStyleState _styleState;
   private IControlCustomStyle _customStyle;
   private bool _initialized = false;
   private BorderRenderHelper _borderRenderHelper;
   private PathIcon? _loadingIcon;
   
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
      HorizontalAlignmentProperty.OverrideDefaultValue<Button>(HorizontalAlignment.Left);
      VerticalAlignmentProperty.OverrideDefaultValue<Button>(VerticalAlignment.Center);
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

      targetHeight += Padding.Top + Padding.Bottom;
      if (Shape != ButtonShape.Round) {
         targetWidth += Padding.Left + Padding.Right;
      } else {
         targetWidth += Math.Max(Padding.Left + Padding.Right, targetHeight);
      }
    
      targetHeight = Math.Max(targetHeight, ControlHeight);
      
      targetWidth = Math.Max(targetWidth, targetHeight);
      
      if (Shape == ButtonShape.Circle) {
         targetWidth = targetHeight;
         CornerRadius = new CornerRadius(targetHeight);
      } else if (Shape == ButtonShape.Round) {
         CornerRadius = new CornerRadius(targetHeight);
         targetWidth = Math.Max(targetWidth, targetHeight + targetHeight / 2);
      }

      return new Size(targetWidth, targetHeight);
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle.HandleAttachedToLogicalTree(e);
         _initialized = true;
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle.HandlePropertyChangedForStyle(e);
   }

   #region IControlCustomStyle 实现

   void IControlCustomStyle.HandleAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      SetupControlTheme();
      if (Text is null && Content is string content) {
         Text = content;
         Content = null;
      }
      PseudoClasses.Set(IconOnlyPC, Icon is not null && Text is null);
   }

   private void SetupControlTheme()
   {
      if (ButtonType == ButtonType.Default) {
         TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, DefaultButtonTheme.ID);
      } else if (ButtonType == ButtonType.Primary) {
         TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, PrimaryButtonTheme.ID);
      } else if (ButtonType == ButtonType.Text) {
         TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, TextButtonTheme.ID);
      } else if (ButtonType == ButtonType.Link) {
         TokenResourceBinder.CreateTokenBinding(this, ThemeProperty, LinkButtonTheme.ID);
      }
   }

   void ApplyShapeStyleConfig()
   {
      if (Shape == ButtonShape.Circle) {
         TokenResourceBinder.CreateTokenBinding(this, PaddingProperty, ButtonTokenResourceKey.CirclePadding);
      }
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

   void IControlCustomStyle.SetupTransitions()
   {
      if (Transitions is null) {
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
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _loadingIcon = e.NameScope.Find<PathIcon>(BaseButtonTheme.LoadingIconPart);
      _customStyle.HandleTemplateApplied(e.NameScope);
      _customStyle.SetupTransitions();
   }

   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      if (ButtonType == ButtonType.Default) {
         if (IsDanger) {
            Effect = new DropShadowEffect
            {
               OffsetX = DangerShadow.OffsetX,
               OffsetY = DangerShadow.OffsetY,
               Color = DangerShadow.Color,
               BlurRadius = DangerShadow.Blur,
            };
         } else {
            Effect = new DropShadowEffect
            {
               OffsetX = DefaultShadow.OffsetX,
               OffsetY = DefaultShadow.OffsetY,
               Color = DefaultShadow.Color,
               BlurRadius = DefaultShadow.Blur,
            };
         }
      } else if (ButtonType == ButtonType.Primary) {
         if (IsDanger) {
            Effect = new DropShadowEffect
            {
               OffsetX = DangerShadow.OffsetX,
               OffsetY = DangerShadow.OffsetY,
               Color = DangerShadow.Color,
               BlurRadius = DangerShadow.Blur,
            };
         } else {
            Effect = new DropShadowEffect
            {
               OffsetX = PrimaryShadow.OffsetX,
               OffsetY = PrimaryShadow.OffsetY,
               Color = PrimaryShadow.Color,
               BlurRadius = PrimaryShadow.Blur,
            };
         }
      }

      _customStyle.CollectStyleState();
      ApplyShapeStyleConfig();
      ApplyIconModeStyleConfig();
      UpdatePseudoClasses();
      SetupIcon();
      SetupIconBrush();
   }

   protected virtual void ApplyIconModeStyleConfig()
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
            if (!IsLoading && _styleState.HasFlag(ControlStyleState.Raised) && (ButtonType == ButtonType.Primary ||
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

      if (e.Property == ButtonTypeProperty) {
         if (VisualRoot is not null) {
            SetupControlTheme();
         }
      } else if (e.Property == ContentProperty || 
                 e.Property == TextProperty ||
                 e.Property == IsLoadingProperty) {
         UpdatePseudoClasses();
      }

      if (e.Property == IconProperty) {
         SetupIcon();
      }

      if (e.Property == IsDangerProperty ||
          e.Property == IsGhostProperty ||
          e.Property == ButtonTypeProperty) {
         SetupIconBrush();
      }
   }

   private void SetupIcon()
   {
      if (Icon is not null) {
         BindUtils.RelayBind(this, IconSizeProperty, Icon, PathIcon.WidthProperty);
         BindUtils.RelayBind(this, IconSizeProperty, Icon, PathIcon.HeightProperty);
         BindUtils.RelayBind(this, IconMarginProperty, Icon, PathIcon.MarginProperty);
      }
   }

   private void SetupIconBrush()
   {
      TokenResourceKey normalFilledBrushKey = ButtonTokenResourceKey.DefaultColor;
      TokenResourceKey selectedFilledBrushKey = ButtonTokenResourceKey.DefaultActiveColor;
      TokenResourceKey activeFilledBrushKey = ButtonTokenResourceKey.DefaultHoverColor;
      TokenResourceKey disabledFilledBrushKey = GlobalTokenResourceKey.ColorTextDisabled;
      if (ButtonType == ButtonType.Default) {
         if (IsGhost) {
            normalFilledBrushKey = GlobalTokenResourceKey.ColorTextLightSolid;
            selectedFilledBrushKey = GlobalTokenResourceKey.ColorPrimaryActive;
            activeFilledBrushKey = GlobalTokenResourceKey.ColorPrimaryHover;
         }
         if (IsDanger) {
            normalFilledBrushKey = GlobalTokenResourceKey.ColorError;
            selectedFilledBrushKey = GlobalTokenResourceKey.ColorErrorActive;
            activeFilledBrushKey = GlobalTokenResourceKey.ColorErrorBorderHover;
         }
      } else if (ButtonType == ButtonType.Primary) { 
         normalFilledBrushKey = ButtonTokenResourceKey.PrimaryColor;
         selectedFilledBrushKey = ButtonTokenResourceKey.PrimaryColor;
         activeFilledBrushKey = ButtonTokenResourceKey.PrimaryColor;
         if (IsGhost) {
            normalFilledBrushKey = GlobalTokenResourceKey.ColorPrimary;
            selectedFilledBrushKey = GlobalTokenResourceKey.ColorPrimaryActive;
            activeFilledBrushKey = GlobalTokenResourceKey.ColorPrimaryHover;
            if (IsDanger) {
               normalFilledBrushKey = GlobalTokenResourceKey.ColorError;
               selectedFilledBrushKey = GlobalTokenResourceKey.ColorErrorActive;
               activeFilledBrushKey = GlobalTokenResourceKey.ColorErrorBorderHover;
            }
         }

      } else if (ButtonType == ButtonType.Text) {
         normalFilledBrushKey = ButtonTokenResourceKey.DefaultColor;
         selectedFilledBrushKey = ButtonTokenResourceKey.DefaultColor;
         activeFilledBrushKey = ButtonTokenResourceKey.DefaultColor;
         if (IsDanger) {
            normalFilledBrushKey = GlobalTokenResourceKey.ColorError;
            selectedFilledBrushKey = GlobalTokenResourceKey.ColorErrorActive;
            activeFilledBrushKey = GlobalTokenResourceKey.ColorErrorBorderHover;
         }
      } else if (ButtonType == ButtonType.Link) {
         normalFilledBrushKey = GlobalTokenResourceKey.ColorLink;
         selectedFilledBrushKey = ButtonTokenResourceKey.DefaultActiveColor;
         activeFilledBrushKey = ButtonTokenResourceKey.DefaultHoverColor;
         if (IsGhost) {
            normalFilledBrushKey = GlobalTokenResourceKey.ColorLink;
            selectedFilledBrushKey = GlobalTokenResourceKey.ColorPrimaryActive;
            activeFilledBrushKey = GlobalTokenResourceKey.ColorPrimaryHover;
         }
         if (IsDanger) {
            normalFilledBrushKey = GlobalTokenResourceKey.ColorError;
            selectedFilledBrushKey = GlobalTokenResourceKey.ColorErrorActive;
            activeFilledBrushKey = GlobalTokenResourceKey.ColorErrorBorderHover;
         }
      }

      if (Icon is not null) {
         TokenResourceBinder.CreateGlobalTokenBinding(Icon, PathIcon.NormalFilledBrushProperty, normalFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(Icon, PathIcon.SelectedFilledBrushProperty, selectedFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(Icon, PathIcon.ActiveFilledBrushProperty, activeFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(Icon, PathIcon.DisabledFilledBrushProperty, disabledFilledBrushKey);
      }

      if (_loadingIcon is not null) {
         TokenResourceBinder.CreateGlobalTokenBinding(_loadingIcon, PathIcon.NormalFilledBrushProperty, normalFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(_loadingIcon, PathIcon.SelectedFilledBrushProperty, selectedFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(_loadingIcon, PathIcon.ActiveFilledBrushProperty, activeFilledBrushKey);
         TokenResourceBinder.CreateGlobalTokenBinding(_loadingIcon, PathIcon.DisabledFilledBrushProperty, disabledFilledBrushKey);
      }
      
      NotifyIconBrushCalculated(in normalFilledBrushKey, in selectedFilledBrushKey, in activeFilledBrushKey, in disabledFilledBrushKey);
   }

   protected virtual void NotifyIconBrushCalculated(in TokenResourceKey normalFilledBrushKey,
                                                    in TokenResourceKey selectedFilledBrushKey,
                                                    in TokenResourceKey activeFilledBrushKey,
                                                    in TokenResourceKey disabledFilledBrushKey)
   {
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
                                 BorderUtils.BuildRenderScaleAwareThickness(BorderThickness, VisualRoot?.RenderScaling ?? 1.0),
                                 CornerRadius,
                                 BackgroundSizing,
                                 Background,
                                 BorderBrush,
                                 default);
   }
   
   private void UpdatePseudoClasses()
   {
      PseudoClasses.Set(IconOnlyPC, Icon is not null && Text is null);
      PseudoClasses.Set(LoadingPC, IsLoading);
   }
   
   #endregion
   
}