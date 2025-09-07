namespace AtomUI.Controls;

public interface IDialog
{
    object? ViewModel { get; set; }
    string? Title { get; set; }
    int Result { get; set; }
    bool IsModalMode { get; set; }
    bool IsCloseButtonEnabled { get; set; }
    bool IsDragMoveable { get; set; }

    event EventHandler? Accepted;
    event EventHandler? Rejected;

    void Accept();
    void Reject();
    void Done(int result);
}