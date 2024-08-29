using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class NotificationShowCase : UserControl
{
   private WindowNotificationManager? _windowNotificationManager;
   public NotificationShowCase()
   {
      InitializeComponent();
   }
   
   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      var topLevel = TopLevel.GetTopLevel(this);
      _windowNotificationManager = new WindowNotificationManager(topLevel)
      {
         MaxItems = 5,
         IsPauseOnHover = true
      };
   }

   private void ShowSimpleNotification(object? sender, RoutedEventArgs e)
   {
      _windowNotificationManager?.Show(new Notification()
      {
         ShowProgress = true,
         Title = "Notification Title",
         Content = "This is the content of the notification. This is the content of the notification. This is the content of the notification. This is the content of the notification."
      });
   }
}