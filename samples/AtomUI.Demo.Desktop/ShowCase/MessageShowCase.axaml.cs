using AtomUI.Controls.Message;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class MessageShowCase : UserControl
{
   private WindowMessageManager? _messageManager;

   public MessageShowCase()
   {
      InitializeComponent();
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      var topLevel = TopLevel.GetTopLevel(this);
      _messageManager = new WindowMessageManager(topLevel)
      {
         MaxItems = 10
      };
   }

   private void ShowSimpleMessage(object? sender, RoutedEventArgs e)
   {
      _messageManager?.Show(new Message(
                               content: "Hello, AtomUI/Avalonia!"
                            ));
   }

   private void ShowInfoMessage(object? sender, RoutedEventArgs e)
   {
      _messageManager?.Show(new Message(
                               type: MessageType.Information,
                               content: "This is a information message."
                            ));
   }

   private void ShowSuccessMessage(object? sender, RoutedEventArgs e)
   {
      _messageManager?.Show(new Message(
                               type: MessageType.Success,
                               content: "This is a success message."
                            ));
   }

   private void ShowWarningMessage(object? sender, RoutedEventArgs e)
   {
      _messageManager?.Show(new Message(
                               type: MessageType.Warning,
                               content: "This is a warning message."
                            ));
   }

   private void ShowErrorMessage(object? sender, RoutedEventArgs e)
   {
      _messageManager?.Show(new Message(
                               type: MessageType.Error,
                               content: "This is a error message."
                            ));
   }

   private void ShowLoadingMessage(object? sender, RoutedEventArgs e)
   {
      _messageManager?.Show(new Message(
                               type: MessageType.Loading,
                               content: "Action in progress..."
                            ));
   }

   private void ShowSequentialMessage(object? sender, RoutedEventArgs e)
   {
      _messageManager?.Show(new Message(
                               type: MessageType.Loading,
                               content: "Action in progress...",
                               expiration: TimeSpan.FromSeconds(2.5),
                               onClose: () =>
                               {
                                  _messageManager?.Show(new Message(
                                                           type: MessageType.Success,
                                                           expiration: TimeSpan.FromSeconds(2.5),
                                                           content: "Loading finished",
                                                           onClose: () =>
                                                           {
                                                              _messageManager?.Show(new Message(
                                                                 type: MessageType.Information,
                                                                 expiration: TimeSpan.FromSeconds(2.5),
                                                                 content: "Loading finished"
                                                              ));
                                                           }
                                                        ));
                               }
                            ));
   }
}