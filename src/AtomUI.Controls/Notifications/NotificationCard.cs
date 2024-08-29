using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

[PseudoClasses(ErrorPC, InformationPC, SuccessPC, WarningPC)]
public class NotificationCard : TemplatedControl
{
   public const string ErrorPC = ":error";
   public const string InformationPC = ":information";
   public const string SuccessPC = ":success";
   public const string WarningPC = ":warning";

   #region 公共属性定义

   /// <summary>
   /// Defines the <see cref="IsClosing"/> property.
   /// </summary>
   public static readonly DirectProperty<NotificationCard, bool> IsClosingProperty =
      AvaloniaProperty.RegisterDirect<NotificationCard, bool>(nameof(IsClosing), o => o.IsClosing);
   
   /// <summary>
   /// Defines the <see cref="IsClosed"/> property.
   /// </summary>
   public static readonly StyledProperty<bool> IsClosedProperty =
      AvaloniaProperty.Register<NotificationCard, bool>(nameof(IsClosed));
   
   /// <summary>
   /// Defines the <see cref="NotificationType" /> property
   /// </summary>
   public static readonly StyledProperty<NotificationType> NotificationTypeProperty =
      AvaloniaProperty.Register<NotificationCard, NotificationType>(nameof(NotificationType));
   
   /// <summary>
   /// Defines the <see cref="NotificationClosed"/> event.
   /// </summary>
   public static readonly RoutedEvent<RoutedEventArgs> NotificationClosedEvent =
      RoutedEvent.Register<NotificationCard, RoutedEventArgs>(nameof(NotificationClosed), RoutingStrategies.Bubble);
   
   /// <summary>
   /// Defines the CloseOnClick property.
   /// </summary>
   public static readonly AttachedProperty<bool> CloseOnClickProperty =
      AvaloniaProperty.RegisterAttached<NotificationCard, Button, bool>("CloseOnClick", defaultValue: false);
   
   public static readonly StyledProperty<string?> TitleProperty =
      AvaloniaProperty.Register<NotificationCard, string?>(nameof(Title));
   
   public static readonly StyledProperty<object?> CardContentProperty =
      AvaloniaProperty.Register<NotificationCard, object?>(nameof(CardContent));
   
   public static readonly StyledProperty<IDataTemplate?> CardContentTemplateProperty =
      AvaloniaProperty.Register<NotificationCard, IDataTemplate?>(nameof(CardContentTemplate));
   
   public static readonly StyledProperty<PathIcon?> IconProperty
      = AvaloniaProperty.Register<NotificationCard, PathIcon?>(nameof(Icon));
   
   /// <summary>
   /// Determines if the notification is already closing.
   /// </summary>
   public bool IsClosing
   {
      get => _isClosing;
      private set => SetAndRaise(IsClosingProperty, ref _isClosing, value);
   }
   
   /// <summary>
   /// Determines if the notification is closed.
   /// </summary>
   public bool IsClosed
   {
      get => GetValue(IsClosedProperty);
      set => SetValue(IsClosedProperty, value);
   }
   
   /// <summary>
   /// Gets or sets the type of the notification
   /// </summary>
   public NotificationType NotificationType
   {
      get => GetValue(NotificationTypeProperty);
      set => SetValue(NotificationTypeProperty, value);
   }
   
   public string? Title
   {
      get => GetValue(TitleProperty);
      set => SetValue(TitleProperty, value);
   }
   
   public object? CardContent
   {
      get => GetValue(CardContentProperty);
      set => SetValue(CardContentProperty, value);
   }
   
   public object? CardContentTemplate
   {
      get => GetValue(CardContentTemplateProperty);
      set => SetValue(CardContentTemplateProperty, value);
   }
   
   public PathIcon? Icon
   {
      get => GetValue(IconProperty);
      set => SetValue(IconProperty, value);
   }
   
   /// <summary>
   /// Raised when the <see cref="NotificationCard"/> has closed.
   /// </summary>
   public event EventHandler<RoutedEventArgs>? NotificationClosed
   {
      add => AddHandler(NotificationClosedEvent, value);
      remove => RemoveHandler(NotificationClosedEvent, value);
   }

   #endregion
   
   public static bool GetCloseOnClick(Button obj)
   {
      _ = obj ?? throw new ArgumentNullException(nameof(obj));
      return (bool)obj.GetValue(CloseOnClickProperty);
   }

   public static void SetCloseOnClick(Button obj, bool value)
   {
      _ = obj ?? throw new ArgumentNullException(nameof(obj));
      obj.SetValue(CloseOnClickProperty, value);
   }
   
   private bool _isClosing;

   static NotificationCard()
   {
      CloseOnClickProperty.Changed.AddClassHandler<Button>(OnCloseOnClickPropertyChanged);
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="NotificationCard"/> class.
   /// </summary>
   public NotificationCard()
   {
      UpdateNotificationType();
      ClipToBounds = false;
   }

   private static void OnCloseOnClickPropertyChanged(AvaloniaObject d, AvaloniaPropertyChangedEventArgs e)
   {
      var button = (Button)d;
      var value = (bool)e.NewValue!;
      if (value) {
         button.Click += Button_Click;
      } else {
         button.Click -= Button_Click;
      }
   }

   /// <summary>
   /// Called when a button inside the Notification is clicked.
   /// </summary>
   private static void Button_Click(object? sender, RoutedEventArgs e)
   {
      var btn = sender as ILogical;
      var notification = btn?.GetLogicalAncestors().OfType<NotificationCard>().FirstOrDefault();
      notification?.Close();
   }

   /// <summary>
   /// Closes the <see cref="NotificationCard"/>.
   /// </summary>
   public void Close()
   {
      if (IsClosing) {
         return;
      }

      IsClosing = true;
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      SetupContent();
      if (Icon is null) {
         SetupNotificationIcon();
         UpdateNotificationType();
      }
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);

      if (e.Property == NotificationTypeProperty) {
         SetupNotificationIcon();
         UpdateNotificationType();
      }

      if (e.Property == IsClosedProperty) {
         if (!IsClosing && !IsClosed) {
            return;
         }

         RaiseEvent(new RoutedEventArgs(NotificationClosedEvent));
      }

      if (e.Property == CardContentProperty) {
         if (e.NewValue is string) {
            SetupContent();
         }
      }
   }

   private void UpdateNotificationType()
   {
      switch (NotificationType) {
         case NotificationType.Error:
            PseudoClasses.Add(NotificationCard.ErrorPC);
            break;

         case NotificationType.Information:
            PseudoClasses.Add(NotificationCard.InformationPC);
            break;

         case NotificationType.Success:
            PseudoClasses.Add(NotificationCard.SuccessPC);
            break;

         case NotificationType.Warning:
            PseudoClasses.Add(NotificationCard.WarningPC);
            break;
      }

      if (Icon is not null) {
         SetupNotificationIconColor(Icon);
      }
   }

   private void SetupNotificationIconColor(PathIcon icon)
   {
      if (NotificationType == NotificationType.Error) {
         TokenResourceBinder.CreateGlobalTokenBinding(icon, PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorError);
      } else if (NotificationType == NotificationType.Information) {
         TokenResourceBinder.CreateGlobalTokenBinding(icon, PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorPrimary);
      } else if (NotificationType == NotificationType.Success) {
         TokenResourceBinder.CreateGlobalTokenBinding(icon, PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorSuccess);
      } else if (NotificationType == NotificationType.Warning) {
         TokenResourceBinder.CreateGlobalTokenBinding(icon, PathIcon.NormalFilledBrushProperty, GlobalTokenResourceKey.ColorWarning);
      }
   }

   private void SetupContent()
   {
      if (CardContent is string content) {
         var textBlock = new SelectableTextBlock()
         {
            Text = content,
         };
         TokenResourceBinder.CreateGlobalTokenBinding(textBlock, SelectableTextBlock.SelectionBrushProperty, GlobalTokenResourceKey.SelectionBackground);
         TokenResourceBinder.CreateGlobalTokenBinding(textBlock, SelectableTextBlock.SelectionForegroundBrushProperty, GlobalTokenResourceKey.SelectionForeground);
         CardContent = textBlock;
      }
   }

   private void SetupNotificationIcon()
   {
      PathIcon? icon = null;
      if (NotificationType == NotificationType.Information) {
         icon = new PathIcon()
         {
            Kind = "InfoCircleFilled"
         };
      } else if (NotificationType == NotificationType.Success) {
         icon = new PathIcon()
         {
            Kind = "CheckCircleFilled"
         };  
      } else if (NotificationType == NotificationType.Error) {
         icon = new PathIcon()
         {
            Kind = "CloseCircleFilled"
         };
      } else if (NotificationType == NotificationType.Warning) {
         icon = new PathIcon()
         {
            Kind = "ExclamationCircleFilled"
         };
      }

      if (icon is not null) {
         SetupNotificationIconColor(icon);
      }
      SetValue(IconProperty, icon, BindingPriority.Template);
   }
}