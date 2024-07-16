using AtomUI.Data;
using AtomUI.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace AtomUI.Controls;

public enum AlertType
{
   Success,
   Info,
   Warning,
   Error
}

public class Alert : TemplatedControl, IControlCustomStyle
{
   public static readonly StyledProperty<AlertType> TypeProperty =
      AvaloniaProperty.Register<Alert, AlertType>(nameof(Type));

   public static readonly StyledProperty<bool> IsShowIconProperty =
      AvaloniaProperty.Register<Alert, bool>(nameof(IsShowIcon));

   public static readonly StyledProperty<bool> IsMessageMarqueEnabledProperty =
      AvaloniaProperty.Register<Alert, bool>(nameof(IsMessageMarqueEnabled));

   public static readonly StyledProperty<bool> IsClosableProperty =
      AvaloniaProperty.Register<Alert, bool>(nameof(IsClosable));


   public static readonly StyledProperty<PathIcon?> CloseIconProperty =
      AvaloniaProperty.Register<Alert, PathIcon?>(nameof(CloseIcon));

   public static readonly StyledProperty<string> MessageProperty =
      AvaloniaProperty.Register<Alert, string>(nameof(Message));

   public static readonly StyledProperty<string?> DescriptionProperty =
      AvaloniaProperty.Register<Alert, string?>(nameof(Description));

   public static readonly StyledProperty<Control?> ExtraActionProperty =
      AvaloniaProperty.Register<Alert, Control?>(nameof(Description));

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
   
   private bool _scalingAwareConfigApplied = false;

   static Alert()
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

   public Alert()
   {
      _controlTokenBinder = new ControlTokenBinder(this, AlertToken.ID);
      _customStyle = this;
      _customStyle.InitOnConstruct();
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

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      _customStyle.HandleTemplateApplied(e.NameScope);
   }

   void IControlCustomStyle.HandleTemplateApplied(INameScope scope)
   {
      _customStyle.ApplyFixedStyleConfig();
      _customStyle.ApplyRenderScalingAwareStyleConfig();

      SetupCloseButton();
   }

   #region IControlCustomStyle 实现

   void IControlCustomStyle.ApplyRenderScalingAwareStyleConfig()
   {
      if (!_scalingAwareConfigApplied) {
         _scalingAwareConfigApplied = true;
         _controlTokenBinder.AddControlBinding(BorderThicknessProperty, GlobalResourceKey.BorderThickness,
                                               BindingPriority.Style,
                                               new RenderScaleAwareThicknessConfigure(this));
      }
   }

   void IControlCustomStyle.HandlePropertyChangedForStyle(AvaloniaPropertyChangedEventArgs e)
   {
      if (VisualRoot is not null) {
         if (e.Property == IsClosableProperty) {
            SetupCloseButton();
         }
      }
   }

   private void SetupCloseButton()
   {
      if (CloseIcon is null) {
         CloseIcon = new PathIcon
         {
            Kind = "CloseOutlined",
         };
         BindUtils.CreateTokenBinding(CloseIcon, PathIcon.NormalFillBrushProperty, GlobalResourceKey.ColorIcon);
         BindUtils.CreateTokenBinding(CloseIcon, PathIcon.ActiveFilledBrushProperty, GlobalResourceKey.ColorIconHover);
      }
   }
   
   #endregion
}