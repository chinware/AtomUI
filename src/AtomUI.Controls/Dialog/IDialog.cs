using System.ComponentModel;

namespace AtomUI.Controls;

public interface IDialog
{
    string? Title { get; set; }
    object? Result { get; set; }

    public event EventHandler? Closed;
    public event EventHandler? Opened;
    public event EventHandler<CancelEventArgs>? Closing;
    public event EventHandler? Accepted;
    public event EventHandler? Rejected;
    public event EventHandler<DialogFinishedEventArgs>? Finished;
    public event EventHandler<DialogButtonClickedEventArgs>? ButtonClicked;

    void Accept();
    void Reject();
    void Done(object? result);
    void Done();
}