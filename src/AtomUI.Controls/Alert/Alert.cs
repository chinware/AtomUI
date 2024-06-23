using AtomUI.Data;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Controls;
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

public partial class Alert : BorderedStyleControl, ITokenIdProvider
{
   string ITokenIdProvider.TokenId => AlertToken.ID;

   public static readonly DirectProperty<Alert, AlertType> TypeProperty =
      AvaloniaProperty.RegisterDirect<Alert, AlertType>(nameof(AlertType),
                                                        o => o.Type,
                                                        (o, v) => o.Type = v);

   public static readonly DirectProperty<Alert, bool> IsShowIconProperty =
      AvaloniaProperty.RegisterDirect<Alert, bool>(nameof(IsShowIcon),
                                                   o => o.IsShowIcon,
                                                   (o, v) => o.IsShowIcon = v);

   public static readonly DirectProperty<Alert, bool> IsMessageMarqueEnabledProperty =
      AvaloniaProperty.RegisterDirect<Alert, bool>(nameof(IsMessageMarqueEnabled),
                                                   o => o.IsMessageMarqueEnabled,
                                                   (o, v) => o.IsMessageMarqueEnabled = v);

   public static readonly DirectProperty<Alert, bool> IsClosableProperty =
      AvaloniaProperty.RegisterDirect<Alert, bool>(nameof(IsClosable),
                                                   o => o.IsClosable,
                                                   (o, v) => o.IsClosable = v);
   
      
   public static readonly DirectProperty<Alert, PathIcon?> CloseIconProperty =
      AvaloniaProperty.RegisterDirect<Alert, PathIcon?>(nameof(CloseIcon),
                                                        o => o.CloseIcon,
                                                        (o, v) => o.CloseIcon = v);

   public static readonly DirectProperty<Alert, string> MessageProperty =
      AvaloniaProperty.RegisterDirect<Alert, string>(nameof(Message),
                                                     o => o.Message,
                                                     (o, v) => o.Message = v);

   public static readonly DirectProperty<Alert, string?> DescriptionProperty =
      AvaloniaProperty.RegisterDirect<Alert, string?>(nameof(Description),
                                                      o => o.Description,
                                                      (o, v) => o.Description = v);
   
   public static readonly DirectProperty<Alert, Control?> ExtraActionProperty =
      AvaloniaProperty.RegisterDirect<Alert, Control?>(nameof(Description),
                                                       o => o._extraAction,
                                                       (o, v) => o._extraAction = v);

   private AlertType _alertType = AlertType.Success;

   public AlertType Type
   {
      get => _alertType;
      set => SetAndRaise(TypeProperty, ref _alertType, value);
   }

   private bool _isShowIcon = false;

   public bool IsShowIcon
   {
      get => _isShowIcon;
      set => SetAndRaise(IsShowIconProperty, ref _isShowIcon, value);
   }

   private bool _isMessageMarqueEnabled = false;

   public bool IsMessageMarqueEnabled
   {
      get => _isMessageMarqueEnabled;
      set => SetAndRaise(IsMessageMarqueEnabledProperty, ref _isMessageMarqueEnabled, value);
   }

   private bool _isClosable = false;

   public bool IsClosable
   {
      get => _isClosable;
      set => SetAndRaise(IsClosableProperty, ref _isClosable, value);
   }

   private PathIcon? _closeIcon;

   public PathIcon? CloseIcon
   {
      get => _closeIcon;
      set => SetAndRaise(CloseIconProperty, ref _closeIcon, value);
   }

   private string _message = string.Empty;

   [Content]
   public string Message
   {
      get => _message;
      set => SetAndRaise(MessageProperty, ref _message, value);
   }

   private string? _description;

   public string? Description
   {
      get => _description;
      set => SetAndRaise(DescriptionProperty, ref _description, value);
   }

   private Control? _extraAction;
   public Control? ExtraAction
   {
      get => _extraAction;
      set => SetAndRaise(ExtraActionProperty, ref _extraAction, value);
   }

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
      _tokenResourceBinder = new TokenResourceBinder(this);
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
}