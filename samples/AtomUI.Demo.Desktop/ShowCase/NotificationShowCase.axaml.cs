using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

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
      _basicManager?.Show(new Notification()
      {
         ShowProgress = true,
         Title = "Notification Title",
         Content = "Hello, AtomUI/Avalonia!"
      });
   }
   
   private void ShowNeverCloseNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification()
      {
         Expiration = TimeSpan.Zero,
         Title = "Notification Title",
         Content = "I will never close automatically. This is a purposely very very long description that has many many characters and words."
      });
   }
      
   private void ShowSuccessNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification()
      {
         Type = NotificationType.Success,
         Title = "Notification Title",
         Content = "This is the content of the notification. This is the content of the notification. This is the content of the notification."
      });
   }
   
   private void ShowInfoNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification()
      {
         Type = NotificationType.Information,
         Title = "Notification Title",
         Content = "This is the content of the notification. This is the content of the notification. This is the content of the notification."
      });
   }
   
   private void ShowWarningNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification()
      {
         Type = NotificationType.Warning,
         Title = "Notification Title",
         Content = "This is the content of the notification. This is the content of the notification. This is the content of the notification."
      });
   }
   
   private void ShowErrorNotification(object? sender, RoutedEventArgs e)
   {
      _basicManager?.Show(new Notification()
      {
         Type = NotificationType.Error,
         Title = "Notification Title",
         Content = "This is the content of the notification. This is the content of the notification. This is the content of the notification."
      });
   }
   
   private void ShowTopNotification(object? sender, RoutedEventArgs e)
   {
      _topManager?.Show(new Notification()
      {
         Title = "Notification Top",
         Content = "Hello, AtomUI/Avalonia!"
      });
   }
   
   private void ShowBottomNotification(object? sender, RoutedEventArgs e)
   {
      _bottomManager?.Show(new Notification()
      {
         Title = "Notification Bottom",
         Content = "Hello, AtomUI/Avalonia!"
      });
   }
   
   private void ShowTopLeftNotification(object? sender, RoutedEventArgs e)
   {
      _topLeftManager?.Show(new Notification()
      {
         Title = "Notification TopLeft",
         Content = "Hello, AtomUI/Avalonia!"
      });
   }
   
   private void ShowTopRightNotification(object? sender, RoutedEventArgs e)
   {
      _topRightManager?.Show(new Notification()
      {
         Title = "Notification TopRight",
         Content = "Hello, AtomUI/Avalonia!"
      });
   }
   
   private void ShowBottomLeftNotification(object? sender, RoutedEventArgs e)
   {
      _bottomLeftManager?.Show(new Notification()
      {
         Title = "Notification BottomLeft",
         Content = "Hello, AtomUI/Avalonia!"
      });
   }
   
   private void ShowBottomRightNotification(object? sender, RoutedEventArgs e)
   {
      _bottomRightManager?.Show(new Notification()
      {
         Title = "Notification BottomRight",
         Content = "Hello, AtomUI/Avalonia!"
      });
   }
}