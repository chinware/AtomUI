namespace AtomUI.Controls;

public interface INotificationManager
{
   public void Show(INotification notification, string[]? classes = null);
}