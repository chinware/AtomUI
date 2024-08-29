using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AtomUI.Controls;

public class Notification : INotification, INotifyPropertyChanged
{
   private string? _title;
   private object? _content;

   /// <summary>
   /// Initializes a new instance of the <see cref="Notification"/> class.
   /// </summary>
   /// <param name="title">The title of the notification.</param>
   /// <param name="content">The message to be displayed in the notification.</param>
   /// <param name="type">The <see cref="NotificationType"/> of the notification.</param>
   /// <param name="expiration">The expiry time at which the notification will close. 
   /// Use <see cref="TimeSpan.Zero"/> for notifications that will remain open.</param>
   /// <param name="onClick">An Action to call when the notification is clicked.</param>
   /// <param name="onClose">An Action to call when the notification is closed.</param>
   public Notification(string? title,
                       object? content,
                       NotificationType type = NotificationType.Information,
                       TimeSpan? expiration = null,
                       Action? onClick = null,
                       Action? onClose = null)
   {
      Title = title;
      _content = content;
      Type = type;
      Expiration = expiration.HasValue ? expiration.Value : TimeSpan.FromSeconds(5);
      OnClick = onClick;
      OnClose = onClose;
   }

   /// <summary>
   /// Initializes a new instance of the <see cref="Notification"/> class.
   /// </summary>
   public Notification() : this(null, null) { }

   /// <inheritdoc/>
   public string? Title
   {
      get => _title;
      set
      {
         if (_title != value) {
            _title = value;
            OnPropertyChanged();
         }
      }
   }

   /// <inheritdoc/>
   public object? Content
   {
      get => _content;
      set
      {
         if (!object.ReferenceEquals(_content, value)) {
            _content = value;
            OnPropertyChanged();
         }
      }
   }

   /// <inheritdoc/>
   public NotificationType Type { get; set; }

   /// <inheritdoc/>
   public TimeSpan Expiration { get; set; }

   /// <inheritdoc/>
   public Action? OnClick { get; set; }

   /// <inheritdoc/>
   public Action? OnClose { get; set; }

   public event PropertyChangedEventHandler? PropertyChanged;

   protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
   {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
   }
}