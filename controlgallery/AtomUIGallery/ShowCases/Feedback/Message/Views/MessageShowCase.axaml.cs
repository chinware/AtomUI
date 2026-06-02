using AtomUI.Desktop.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ReactiveUI.Avalonia;

namespace AtomUIGallery.ShowCases.Message;

public partial class MessageShowCase : ReactiveUserControl<MessageViewModel>
{
    private WindowMessageManager? _messageManager;

    public MessageShowCase()
    {
        InitializeComponent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _messageManager?.Dispose();
        _messageManager = null;
    }

    private void ShowSimpleMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            "Hello, AtomUI/Avalonia!"
        ));
    }

    private void ShowInfoMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Information,
            content: "This is a information message."
        ));
    }

    private void ShowSuccessMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Success,
            content: "This is a success message."
        ));
    }

    private void ShowWarningMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Warning,
            content: "This is a warning message."
        ));
    }

    private void ShowErrorMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Error,
            content: "This is a error message."
        ));
    }

    private void ShowLoadingMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Loading,
            content: "Action in progress..."
        ));
    }

    private void ShowSequentialMessage(object? sender, RoutedEventArgs e)
    {
        ShowMessage(new AtomUIMessage(
            type: MessageType.Loading,
            content: "Action in progress...",
            expiration: TimeSpan.FromSeconds(2.5),
            onClose: () =>
            {
                ShowMessage(new AtomUIMessage(
                    type: MessageType.Success,
                    expiration: TimeSpan.FromSeconds(2.5),
                    content: "Loading finished",
                    onClose: () =>
                    {
                        ShowMessage(new AtomUIMessage(
                            type: MessageType.Information,
                            expiration: TimeSpan.FromSeconds(2.5),
                            content: "Loading finished"
                        ));
                    }
                ));
            }
        ));
    }

    private void ShowMessage(AtomUIMessage message)
    {
        GetMessageManager()?.Show(message);
    }

    private WindowMessageManager? GetMessageManager()
    {
        if (_messageManager is not null)
        {
            return _messageManager;
        }

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
        {
            return null;
        }

        _messageManager = new WindowMessageManager(topLevel)
        {
            MaxItems = 10
        };
        return _messageManager;
    }
}
