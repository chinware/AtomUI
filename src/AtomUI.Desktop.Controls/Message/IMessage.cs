using AtomUI.IconPkg;

namespace AtomUI.Controls;

public interface IMessage
{
   /// <summary>
   /// Gets the message.
   /// </summary>
   string Content { get; }

   /// <summary>
   /// 自定义图标
   /// </summary>
   Icon? Icon { get; }

   /// <summary>
   /// Gets the <see cref="MessageType" /> of the notification.
   /// </summary>
   MessageType Type { get; }

   /// <summary>
   /// Gets the expiration time of the notification after which it will automatically close.
   /// If the value is <see cref="TimeSpan.Zero" /> then the notification will remain open until the user closes it.
   /// </summary>
   TimeSpan Expiration { get; }

   /// <summary>
   /// Gets an Action to be run when the message is closed.
   /// </summary>
   Action? OnClose { get; }
}