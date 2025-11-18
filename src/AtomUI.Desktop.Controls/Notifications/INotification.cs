using AtomUI.Controls;

namespace AtomUI.Desktop.Controls;

public interface INotification
{
   /// <summary>
   /// Gets the Title of the notification.
   /// </summary>
   string Title { get; }

   /// <summary>
   /// Gets the notification message.
   /// </summary>
   object? Content { get; }

   /// <summary>
   /// 自定义图标
   /// </summary>
   Icon? Icon { get; }

   /// <summary>
   /// Gets the <see cref="NotificationType" /> of the notification.
   /// </summary>
   NotificationType Type { get; }

   /// <summary>
   /// Gets the expiration time of the notification after which it will automatically close.
   /// If the value is <see cref="TimeSpan.Zero" /> then the notification will remain open until the user closes it.
   /// </summary>
   TimeSpan Expiration { get; }

   /// <summary>
   /// 显示一个进度条
   /// </summary>
   bool ShowProgress { get; }

   /// <summary>
   /// Gets an Action to be run when the notification is clicked.
   /// </summary>
   Action? OnClick { get; }

   /// <summary>
   /// Gets an Action to be run when the notification is closed.
   /// </summary>
   Action? OnClose { get; }
}