using Avalonia.Input;

namespace AtomUI.Desktop.Controls;

internal interface IDialogPresenter : IAsyncDisposable
{
    event EventHandler<DialogPresenterCloseRequestedEventArgs>? CloseRequested;

    IInputElement FocusScope { get; }

    ValueTask ShowAsync(CancellationToken cancellationToken);

    ValueTask CloseAsync();
}

internal sealed class DialogPresenterCloseRequestedEventArgs : EventArgs
{
    public DialogCloseReason Reason { get; }
    public object? Result { get; }
    public DialogButton? SourceButton { get; }

    public DialogPresenterCloseRequestedEventArgs(
        DialogCloseReason reason,
        object? result = null,
        DialogButton? sourceButton = null)
    {
        Reason       = reason;
        Result       = result;
        SourceButton = sourceButton;
    }
}
