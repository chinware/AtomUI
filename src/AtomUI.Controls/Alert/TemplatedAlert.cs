using AtomUI.Data;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public class TemplatedAlert : TemplatedControl, IControlCustomStyle
{
   public static readonly StyledProperty<AlertType> TypeProperty =
      AvaloniaProperty.Register<TemplatedAlert, AlertType>(nameof(Type));

   public static readonly StyledProperty<bool> IsShowIconProperty =
      AvaloniaProperty.Register<TemplatedAlert, bool>(nameof(IsShowIcon));

   public static readonly StyledProperty<bool> IsMessageMarqueEnabledProperty =
      AvaloniaProperty.Register<TemplatedAlert, bool>(nameof(IsMessageMarqueEnabled));

   public static readonly StyledProperty<bool> IsClosableProperty =
      AvaloniaProperty.Register<TemplatedAlert, bool>(nameof(IsClosable));
   
      
   public static readonly StyledProperty<PathIcon?> CloseIconProperty =
      AvaloniaProperty.Register<TemplatedAlert, PathIcon?>(nameof(CloseIcon));

   public static readonly StyledProperty<string> MessageProperty =
      AvaloniaProperty.Register<TemplatedAlert, string>(nameof(Message));

   public static readonly StyledProperty<string?> DescriptionProperty =
      AvaloniaProperty.Register<TemplatedAlert, string?>(nameof(Description));
   
   public static readonly StyledProperty<Control?> ExtraActionProperty =
      AvaloniaProperty.Register<TemplatedAlert, Control?>(nameof(Description));
   
   public AlertType Type
   {
      get => GetValue(TypeProperty);
      set => SetValue(TypeProperty, value);
   }

   public bool IsShowIcon
   {
      get => GetValue(IsShowIconProperty);
      set => SetValue(IsShowIconProperty, value);
   }

   public bool IsMessageMarqueEnabled
   {
      get => GetValue(IsMessageMarqueEnabledProperty);
      set => SetValue(IsMessageMarqueEnabledProperty, value);
   }

   public bool IsClosable
   {
      get => GetValue(IsClosableProperty);
      set => SetValue(IsClosableProperty, value);
   }
   
   public PathIcon? CloseIcon
   {
      get => GetValue(CloseIconProperty);
      set => SetValue(CloseIconProperty, value);
   }

   [Content]
   public string Message
   {
      get => GetValue(MessageProperty);
      set => SetValue(MessageProperty, value);
   }

   public string? Description
   {
      get => GetValue(DescriptionProperty);
      set => SetValue(DescriptionProperty, value);
   }
   
   public Control? ExtraAction
   {
      get => GetValue(ExtraActionProperty);
      set => SetValue(ExtraActionProperty, value);
   }
   
   private readonly IControlCustomStyle _customStyle;
   private readonly ControlTokenBinder _controlTokenBinder;
   private bool _initialized = false;
   
   private Label? _messageLabel;
   private MarqueeLabel? _messageMarqueeLabel;
   private Label? _descriptionLabel;
   private PathIcon? _icon;
   private IconButton? _closeButton;
   private bool _scalingAwareConfigApplied = false;

   static TemplatedAlert()
   {
      AffectsMeasure<Segmented>(IsClosableProperty,
                                IsShowIconProperty,
                                MessageProperty,
                                DescriptionProperty,
                                IsMessageMarqueEnabledProperty,
                                PaddingProperty,
                                ExtraActionProperty,
                                IsMessageMarqueEnabledProperty);
      AffectsRender<Segmented>(TypeProperty);
   }
   
   public TemplatedAlert()
   {
      _controlTokenBinder = new ControlTokenBinder(this, AlertToken.ID);
      _customStyle = this;
      _customStyle.InitOnConstruct();
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
   
   void IControlCustomStyle.SetupUi()
   {
      HorizontalAlignment = HorizontalAlignment.Stretch;
      var controlTheme = new AlertTheme();
      controlTheme.Build();
      Theme = controlTheme;
   }
   
   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.HandleTemplateApplied(e.NameScope);
   }

   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _closeButton = scope.Find<IconButton>(AlertTheme.CloseBtnPart);
      _icon = scope.Find<PathIcon>(AlertTheme.InfoIconPart);
      _descriptionLabel = scope.Find<Label>(AlertTheme.DescriptionLabelPart);
      _messageLabel = scope.Find<Label>(AlertTheme.MessageLabelPart);
      _messageMarqueeLabel = scope.Find<MarqueeLabel>(AlertTheme.MarqueeLabelPart);
      
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyRenderScalingAwareStyleConfig();
      SetupPaddingStyleConfig();
      SetupTypeIcon();
      SetupCloseButton();
      HandleMessageMarqueEnabled(IsMessageMarqueEnabled);
      HandleDescriptionEnabled(!string.IsNullOrEmpty(Description));
   }
   
   #region IControlCustomStyle 实现
   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      if (!_scalingAwareConfigApplied) {
         _scalingAwareConfigApplied = true;
         _controlTokenBinder.AddControlBinding(BorderThicknessProperty, GlobalResourceKey.BorderThickness, BindingPriority.Style, 
                                               new RenderScaleAwareThicknessConfigure(this));
      }
   }
   
   private void ApplyAlertTypeStyleConfig()
   {
      if (Type == AlertType.Success) {
         _controlTokenBinder.AddControlBinding(BackgroundProperty, GlobalResourceKey.ColorSuccessBg);
         _controlTokenBinder.AddControlBinding(BorderBrushProperty, GlobalResourceKey.ColorSuccessBorder);
      } else if (Type == AlertType.Info) {
         _controlTokenBinder.AddControlBinding(BackgroundProperty, GlobalResourceKey.ColorInfoBg);
         _controlTokenBinder.AddControlBinding(BorderBrushProperty, GlobalResourceKey.ColorInfoBorder);
      } else if (Type == AlertType.Warning) {
         _controlTokenBinder.AddControlBinding(BackgroundProperty, GlobalResourceKey.ColorWarningBg);
         _controlTokenBinder.AddControlBinding(BorderBrushProperty, GlobalResourceKey.ColorWarningBorder);
      } else if (Type == AlertType.Error) {
         _controlTokenBinder.AddControlBinding(BackgroundProperty, GlobalResourceKey.ColorErrorBg);
         _controlTokenBinder.AddControlBinding(BorderBrushProperty, GlobalResourceKey.ColorErrorBorder);
      }
      _controlTokenBinder.AddControlBinding(CornerRadiusProperty, GlobalResourceKey.BorderRadiusLG);
   }
   
   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (_initialized) {
         if (e.Property == DescriptionProperty) {
            var desc = e.GetNewValue<string?>();
            var enabled = !string.IsNullOrEmpty(desc);
            HandleDescriptionEnabled(enabled);
            HandleIconForDescriptionEnabled(enabled);
            _descriptionLabel!.IsVisible = enabled;
         } else if (e.Property == IsMessageMarqueEnabledProperty) {
            var enabled = e.GetNewValue<bool>();
            HandleMessageMarqueEnabled(enabled);
         } else if (e.Property == TypeProperty) {
            SetupTypeIcon();
         } else if (e.Property == IsClosableProperty) {
            SetupCloseButton();
         }
      }
   }
   
   private void HandleDescriptionEnabled(bool enabled)
   {
      var messageControl = GetMessageControl();
      if (enabled) {
         _controlTokenBinder.AddControlBinding(messageControl, FontSizeProperty, GlobalResourceKey.FontSizeLG);
         _controlTokenBinder.AddControlBinding(messageControl, MarginProperty, AlertResourceKey.MessageWithDescriptionMargin);
         if (_closeButton is not null) {
            _closeButton.VerticalAlignment = VerticalAlignment.Top;
         }
         messageControl.VerticalAlignment = VerticalAlignment.Top;
      } else {
         messageControl.ClearValue(MarginProperty);
         _controlTokenBinder.AddControlBinding(messageControl, FontSizeProperty, GlobalResourceKey.FontSize);
         messageControl.Margin = new Thickness();
         if (_closeButton is not null) {
            _closeButton.VerticalAlignment = VerticalAlignment.Center;
         }
         messageControl.VerticalAlignment = VerticalAlignment.Stretch;
      }
   }

   private void HandleIconForDescriptionEnabled(bool enabled)
   {
      if (_icon is not null && IsShowIcon) {
         if (enabled) {
            _controlTokenBinder.AddControlBinding(_icon, WidthProperty, AlertResourceKey.WithDescriptionIconSize);
            _controlTokenBinder.AddControlBinding(_icon, HeightProperty, AlertResourceKey.WithDescriptionIconSize);
            _controlTokenBinder.AddControlBinding(_icon, MarginProperty, AlertResourceKey.IconWithDescriptionMargin);
            _icon.VerticalAlignment = VerticalAlignment.Top;
         } else {
            _controlTokenBinder.AddControlBinding(_icon, WidthProperty, AlertResourceKey.IconSize);
            _controlTokenBinder.AddControlBinding(_icon, HeightProperty, AlertResourceKey.IconSize);
            _controlTokenBinder.AddControlBinding(_icon, MarginProperty, AlertResourceKey.IconDefaultMargin);
            _icon.VerticalAlignment = VerticalAlignment.Center;
         }
      }
   }
   
   void IControlCustomStyle.ApplyFixedStyleConfig()
   {
      ApplyAlertTypeStyleConfig();
      _controlTokenBinder.AddControlBinding(FontSizeProperty, GlobalResourceKey.FontSize);
   }
   
   private void HandleMessageMarqueEnabled(bool enabled)
   {
      if (!enabled) {
         _messageMarqueeLabel!.IsVisible = false;
         _messageLabel!.IsVisible = true;
      } else {
         _messageMarqueeLabel!.IsVisible = true;
         _messageLabel!.IsVisible = false;
      }
   }
   
   private void SetupTypeIcon()
   {
      if (_icon is not null) {
         if (IsShowIcon) {
            var kind = string.Empty;
            var resourceKey = string.Empty;
            if (Type == AlertType.Success) {
               kind = "CheckCircleFilled";
               resourceKey = GlobalResourceKey.ColorSuccess;
            } else if (Type == AlertType.Info) {
               kind = "InfoCircleFilled";
               resourceKey = GlobalResourceKey.ColorPrimary;
            } else if (Type == AlertType.Warning) {
               kind = "ExclamationCircleFilled";
               resourceKey = GlobalResourceKey.ColorWarning;
            } else if (Type == AlertType.Error) {
               kind = "CloseCircleFilled";
               resourceKey = GlobalResourceKey.ColorError;
            }
            
            _icon.Kind = kind;
            _controlTokenBinder.AddControlBinding(_icon, PathIcon.NormalFillBrushProperty, resourceKey);
            HandleIconForDescriptionEnabled(_descriptionLabel!.IsVisible);
            _icon.IsVisible = true;
         } else {
            _icon.IsVisible = false;
         }
      }
   }
   
   private void SetupCloseButton()
   {
      if (IsClosable) {
         if (CloseIcon is null) {
            CloseIcon = new PathIcon
            {
               Kind = "CloseOutlined",
            };
            _controlTokenBinder.AddControlBinding(CloseIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorIcon);
            _controlTokenBinder.AddControlBinding(CloseIcon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorIconHover);
         }
         
         BindUtils.RelayBind(this, CloseIconProperty, _closeButton!, IconButton.IconProperty);
         _closeButton!.IsVisible = true;
      } else {
         _closeButton!.IsVisible = false;
      }
   }
   
   private void SetupPaddingStyleConfig()
   {
      if (_descriptionLabel!.IsVisible) {
         _controlTokenBinder.AddControlBinding(PaddingProperty, AlertResourceKey.WithDescriptionPadding);
      } else {
         _controlTokenBinder.AddControlBinding(PaddingProperty, AlertResourceKey.DefaultPadding);
      }
   }
   
   private Control GetMessageControl()
   {
      if (IsMessageMarqueEnabled) {
         return _messageMarqueeLabel!;
      }

      return _messageLabel!;
   }
   
   #endregion
}