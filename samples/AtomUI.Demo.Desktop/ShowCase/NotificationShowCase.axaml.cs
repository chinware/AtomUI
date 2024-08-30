using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PathIcon = AtomUI.Controls.PathIcon;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class NotificationShowCase : UserControl
{
   private WindowNotificationManager? _basicManager;
   private WindowNotificationManager? _topLeftManager;
   private WindowNotificationManager? _topManager;
   private WindowNotificationManager? _topRightManager;
   
   private WindowNotificationManager? _bottomLeftManager;
   private WindowNotificationManager? _bottomManager;
   private WindowNotificationManager? _bottomRightManager;
   
   public NotificationShowCase()
   {
      InitializeComponent();
      HoverOptionGroup.OptionCheckedChanged += HandleHoverOptionGroupCheckedChanged;
   }

   private void HandleHoverOptionGroupCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
   {
      if (_basicManager is not null) {
         if (args.Index == 0) {
            _basicManager.IsPauseOnHover = true;
         } else {
            _basicManager.IsPauseOnHover = false;
         }
      }
   }
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      var topLevel = TopLevel.GetTopLevel(this);
      _basicManager = new WindowNotificationManager(topLevel)
      {
         MaxItems = 3
      };
      
      _topLeftManager = new WindowNotificationManager(topLevel)
      {
         MaxItems = 3,
         Position = NotificationPosition.TopLeft
      };
              
      _topManager = new WindowNotificationManager(topLevel)
      {
         Position = NotificationPosition.TopCenter,
         MaxItems = 3
      };
              
      _topRightManager = new WindowNotificationManager(topLevel)
      {
         Position = NotificationPosition.TopRight,
         MaxItems = 3
      };
      
      _bottomLeftManager = new WindowNotificationManager(topLevel)
      {
         Position = NotificationPosition.BottomLeft,
         MaxItems = 3
      };
              
      _bottomManager = new WindowNotificationManager(topLevel)
      {
         Position = NotificationPosition.BottomCenter,
         MaxItems = 3
      };
              
      _bottomRightManager = new WindowNotificationManager(topLevel)
      {
         Position = NotificationPosition.BottomRight,
         MaxItems = 3
      };
   }

   private void ShowSimpleNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification(
                             title: "Notification Title",
                             content: "Hello, AtomUI/Avalonia!"
                          ));
   }
   
   private void ShowNeverCloseNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification(
                             expiration : TimeSpan.Zero,
                             title : "Notification Title",
                             content : "I will never close automatically. This is a purposely very very long description that has many many characters and words."
                          ));
   }
      
   private void ShowSuccessNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification(
                             type: NotificationType.Success,
                             title: "Notification Title",
                             content: "This is the content of the notification. This is the content of the notification. This is the content of the notification."
                          ));
   }
   
   private void ShowInfoNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification(
                             type: NotificationType.Information,
                             title: "Notification Title",
                             content: "This is the content of the notification. This is the content of the notification. This is the content of the notification."
                          ));
   }
   
   private void ShowWarningNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification(
                             type: NotificationType.Warning,
                             title: "Notification Title",
                             content: "This is the content of the notification. This is the content of the notification. This is the content of the notification."
                          ));
   }
   
   private void ShowErrorNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification(
                             type: NotificationType.Error,
                             title: "Notification Title",
                             content: "This is the content of the notification. This is the content of the notification. This is the content of the notification."
                          ));
   }
   
   private void ShowTopNotification(object? sender, RoutedEventArgs e)
   {
      _topManager?.Show(new Notification(
                           title : "Notification Top",
                           content : "Hello, AtomUI/Avalonia!"
                        ));
   }
   
   private void ShowBottomNotification(object? sender, RoutedEventArgs e)
   {
      _bottomManager?.Show(new Notification(
                              title : "Notification Bottom",
                              content : "Hello, AtomUI/Avalonia!"
                           ));
   }
   
   private void ShowTopLeftNotification(object? sender, RoutedEventArgs e)
   {
      _topLeftManager?.Show(new Notification(
                               title : "Notification TopLeft",
                               content : "Hello, AtomUI/Avalonia!"
                            ));
   }
   
   private void ShowTopRightNotification(object? sender, RoutedEventArgs e)
   {
      _topRightManager?.Show(new Notification(
                                title : "Notification TopRight",
                                content : "Hello, AtomUI/Avalonia!"
                             ));
   }
   
   private void ShowBottomLeftNotification(object? sender, RoutedEventArgs e)
   {
      _bottomLeftManager?.Show(new Notification(
                                  title : "Notification BottomLeft",
                                  content : "Hello, AtomUI/Avalonia!"
                               ));
   }
   
   private void ShowBottomRightNotification(object? sender, RoutedEventArgs e)
   {
      _bottomRightManager?.Show(new Notification(
                                   title : "Notification BottomRight",
                                   content : "Hello, AtomUI/Avalonia!"
                                ));
   }
   
   
   private void ShowCustomIconNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification(
                             title: "Notification Title",
                             content: "This is the content of the notification. This is the content of the notification. This is the content of the notification.",
                             icon: new PathIcon()
                             {
                                Kind = "SettingOutlined"
                             }
                          ));
   }
   
   private void ShowProgressNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification(
                             type: NotificationType.Information,
                             title: "Notification Title",
                             content: "This is the content of the notification. This is the content of the notification. This is the content of the notification.",
                             showProgress:true
                          ));
   }
}