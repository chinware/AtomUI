namespace AtomUI.Controls.Utils;

public interface ICellEditBinding
{
    bool IsValid { get; }
    IEnumerable<Exception> ValidationErrors { get; }
    IObservable<bool> ValidationChanged { get; }
    bool CommitEdit();
}
