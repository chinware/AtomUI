using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

[PseudoClasses(ErrorPC, InformationPC, SuccessPC, WarningPC)]
public class NotificationCard : ContentControl
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
   
   public static readonly StyledProperty<bool> IsShowProgressProperty =
      AvaloniaProperty.Register<NotificationCard, bool>(nameof(IsShowProgress), false);
   
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
   
   public static readonly StyledProperty<string> TitleProperty =
      AvaloniaProperty.Register<NotificationCard, string>(nameof(Title));
   
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
   
   public bool IsShowProgress
   {
      get => GetValue(IsShowProgressProperty);
      set => SetValue(IsShowProgressProperty, value);
   }
      
   /// <summary>
   /// Gets or sets the type of the notification
   /// </summary>
   public NotificationType NotificationType
   {
      get => GetValue(NotificationTypeProperty);
      set => SetValue(NotificationTypeProperty, value);
   }
   
   public string Title
   {
      get => GetValue(TitleProperty);
      set => SetValue(TitleProperty, value);
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
   
   #region 内部属性定义

   internal static readonly DirectProperty<NotificationCard, bool> EffectiveShowProgressProperty =
      AvaloniaProperty.RegisterDirect<NotificationCard, bool>(nameof(EffectiveShowProgress),
                                                              o => o.EffectiveShowProgress,
                                                              (o, v) => o.EffectiveShowProgress = v);
   
   internal static readonly DirectProperty<NotificationCard, NotificationPosition> PositionProperty =
      AvaloniaProperty.RegisterDirect<NotificationCard, NotificationPosition>(
         nameof(Position), 
         o => o.Position,
         (o, v) => o.Position = v);

   private bool _effectiveShowProgress;
   internal bool EffectiveShowProgress
   {
      get => _effectiveShowProgress;
      set => SetAndRaise(EffectiveShowProgressProperty, ref _effectiveShowProgress, value);
   }

   private NotificationPosition _position;
   internal NotificationPosition Position
   {
      get => _position;
      set => SetAndRaise(PositionProperty, ref _position, value);
   }

   #endregion
   
   /// <summary>
   /// Gets the expiration time of the notification after which it will automatically close.
   /// If the value is null then the notification will remain open until the user closes it.
   /// </summary>
   public TimeSpan? Expiration { get; set; }
   private bool _isClosing;
   private NotificationProgressBar? _progressBar;
   private WindowNotificationManager _notificationManager;
   private IconButton? _closeButton;
   
   /// <summary>
   /// Initializes a new instance of the <see cref="NotificationCard"/> class.
   /// </summary>
   public NotificationCard(WindowNotificationManager manager)
   {
      UpdateNotificationType();
      ClipToBounds = false;
      _notificationManager = manager;
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

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      UpdatePseudoClasses(Position);
   }

   protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
   {
      base.OnApplyTemplate(e);
      if (Icon is null) {
         SetupNotificationIcon();
         UpdateNotificationType();
      }

      _progressBar = e.NameScope.Find<NotificationProgressBar>(NotificationCardTheme.ProgressBarPart);
      _closeButton = e.NameScope.Find<IconButton>(NotificationCardTheme.CloseButtonPart);
      if (_progressBar is not null) {
         if (Expiration is null) {
            _progressBar.IsVisible = false;
         } else {
            _progressBar.Expiration = Expiration.Value;
         }
      }

      if (_closeButton is not null) {
         _closeButton.Click += HandleCloseButtonClose;
      }
      SetupEffectiveShowProgress();
   }

   private void HandleCloseButtonClose(object? sender, EventArgs args)
   {
      Close();
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

      if (e.Property == ContentProperty) {
         if (e.NewValue is string) {
            SetupContent();
         }
      }

      if (e.Property == IsShowProgressProperty ||
          e.Property == IsClosedProperty) {
         SetupEffectiveShowProgress();
      }
      
      if (e.Property == PositionProperty) {
         UpdatePseudoClasses(e.GetNewValue<NotificationPosition>());
      }
   }

   private void SetupEffectiveShowProgress()
   {
      if (!IsShowProgress) {
         EffectiveShowProgress = false;
      } else {
         if (Expiration is not null) {
            EffectiveShowProgress = true;
         } else {
            EffectiveShowProgress = false;
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
      if (Content is string content) {
         var textBlock = new SelectableTextBlock()
         {
            Text = content,
         };
         TokenResourceBinder.CreateGlobalTokenBinding(textBlock, SelectableTextBlock.SelectionBrushProperty, GlobalTokenResourceKey.SelectionBackground);
         TokenResourceBinder.CreateGlobalTokenBinding(textBlock, SelectableTextBlock.SelectionForegroundBrushProperty, GlobalTokenResourceKey.SelectionForeground);
         Content = textBlock;
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
      SetCurrentValue(IconProperty, icon);
   }

   internal bool NotifyCloseTick(TimeSpan cycleDuration)
   {
      InvalidateVisual();
      if (Expiration is null) {
         return false;
      }
      Expiration -= cycleDuration;
      if (_progressBar is not null) {
         _progressBar.CurrentExpiration = Expiration.Value;
      }
      if (Expiration.Value.TotalMilliseconds < 0) {
         return true;
      }

      return false;
   }

   protected override void OnPointerEntered(PointerEventArgs e)
   {
      base.OnPointerEntered(e);
      if (_notificationManager.IsPauseOnHover) {
         _notificationManager.StopExpiredTimer();
      }
   }

   protected override void OnPointerMoved(PointerEventArgs e)
   {
      base.OnPointerMoved(e);
      if (_notificationManager.IsPauseOnHover) {
         _notificationManager.StopExpiredTimer();
      }
   }

   protected override void OnPointerExited(PointerEventArgs e)
   {
      base.OnPointerExited(e);
      if (_notificationManager.IsPauseOnHover) {
         _notificationManager.StartExpiredTimer();
      }
   }
   
   private void UpdatePseudoClasses(NotificationPosition position)
   {
      PseudoClasses.Set(WindowNotificationManager.TopLeftPC, position == NotificationPosition.TopLeft);
      PseudoClasses.Set(WindowNotificationManager.TopRightPC, position == NotificationPosition.TopRight);
      PseudoClasses.Set(WindowNotificationManager.BottomLeftPC, position == NotificationPosition.BottomLeft);
      PseudoClasses.Set(WindowNotificationManager.BottomRightPC, position == NotificationPosition.BottomRight);
      PseudoClasses.Set(WindowNotificationManager.TopCenterPC, position == NotificationPosition.TopCenter);
      PseudoClasses.Set(WindowNotificationManager.BottomCenterPC, position == NotificationPosition.BottomCenter);
   }
}