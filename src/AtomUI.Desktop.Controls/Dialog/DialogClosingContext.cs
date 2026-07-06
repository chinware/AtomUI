namespace AtomUI.Desktop.Controls;

public sealed class DialogClosingContext
{
    internal DialogClosingContext(
        Dialog dialog,
        object? result,
        DialogCloseReason reason,
        DialogButton? sourceButton,
        CancellationToken cancellationToken)
    {
        Dialog            = dialog;
        Result            = result;
        DialogCode        = result is DialogCode code ? code : null;
        Reason            = reason;
        SourceButton      = sourceButton;
        CancellationToken = cancellationToken;
    }

    public Dialog Dialog { get; }
    public object? Result { get; }
    public DialogCode? DialogCode { get; }
    public DialogCloseReason Reason { get; }
    public DialogButton? SourceButton { get; }
    public CancellationToken CancellationToken { get; }
}
